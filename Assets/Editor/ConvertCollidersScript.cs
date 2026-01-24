using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ConvertCollidersScript
{
    public static void Execute()
    {
        // Find all game objects with colliders
        List<GameObject> objectsWithColliders = new List<GameObject>();
        
        // Find all colliders in the scene
        BoxCollider2D[] boxColliders = Object.FindObjectsOfType<BoxCollider2D>();
        PolygonCollider2D[] polygonColliders = Object.FindObjectsOfType<PolygonCollider2D>();
        CapsuleCollider2D[] capsuleColliders = Object.FindObjectsOfType<CapsuleCollider2D>();
        
        // Add objects with box colliders
        foreach (BoxCollider2D collider in boxColliders)
        {
            if (!objectsWithColliders.Contains(collider.gameObject))
            {
                objectsWithColliders.Add(collider.gameObject);
            }
        }
        
        // Add objects with polygon colliders
        foreach (PolygonCollider2D collider in polygonColliders)
        {
            if (!objectsWithColliders.Contains(collider.gameObject))
            {
                objectsWithColliders.Add(collider.gameObject);
            }
        }
        
        // Add objects with capsule colliders
        foreach (CapsuleCollider2D collider in capsuleColliders)
        {
            if (!objectsWithColliders.Contains(collider.gameObject))
            {
                objectsWithColliders.Add(collider.gameObject);
            }
        }
        
        // Process each object
        int convertedCount = 0;
        foreach (GameObject obj in objectsWithColliders)
        {
            if (ProcessGameObject(obj))
            {
                convertedCount++;
            }
        }
        
        Debug.Log($"Converted {convertedCount} objects to use circle colliders");
        
        // Save the scene
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
    }
    
    private static bool ProcessGameObject(GameObject gameObject)
    {
        // Skip if already processed or null
        if (gameObject == null)
            return false;
            
        // Get all colliders on the object
        List<Collider2D> collidersToRemove = new List<Collider2D>();
        
        // Check for box colliders
        BoxCollider2D[] boxColliders = gameObject.GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D collider in boxColliders)
        {
            collidersToRemove.Add(collider);
        }
        
        // Check for polygon colliders
        PolygonCollider2D[] polygonColliders = gameObject.GetComponents<PolygonCollider2D>();
        foreach (PolygonCollider2D collider in polygonColliders)
        {
            collidersToRemove.Add(collider);
        }
        
        // Check for capsule colliders
        CapsuleCollider2D[] capsuleColliders = gameObject.GetComponents<CapsuleCollider2D>();
        foreach (CapsuleCollider2D collider in capsuleColliders)
        {
            collidersToRemove.Add(collider);
        }
        
        // Skip if no colliders to convert
        if (collidersToRemove.Count == 0)
            return false;
            
        // Calculate appropriate radius based on the first collider
        Collider2D firstCollider = collidersToRemove[0];
        float radius = 0.5f;
        Vector2 offset = firstCollider.offset;
        bool isTrigger = firstCollider.isTrigger;
        
        if (firstCollider is BoxCollider2D)
        {
            BoxCollider2D boxCollider = firstCollider as BoxCollider2D;
            // Use half of the average of width and height for the radius
            radius = (boxCollider.size.x + boxCollider.size.y) * 0.25f;
        }
        else if (firstCollider is PolygonCollider2D)
        {
            PolygonCollider2D polyCollider = firstCollider as PolygonCollider2D;
            // Find the furthest point from the center to determine radius
            float maxDistance = 0f;
            
            for (int i = 0; i < polyCollider.pathCount; i++)
            {
                Vector2[] points = polyCollider.GetPath(i);
                foreach (Vector2 point in points)
                {
                    float distance = Vector2.Distance(point, Vector2.zero);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }
                }
            }
            
            radius = maxDistance;
        }
        else if (firstCollider is CapsuleCollider2D)
        {
            CapsuleCollider2D capsuleCollider = firstCollider as CapsuleCollider2D;
            // Use the larger dimension for the radius
            radius = Mathf.Max(capsuleCollider.size.x, capsuleCollider.size.y) * 0.5f;
        }
        
        // Special handling for character colliders - make them slightly smaller
        if (gameObject.name.Contains("Bob") || 
            gameObject.name.Contains("Brody") || 
            gameObject.name.Contains("Fille") || 
            gameObject.name.Contains("Clown") || 
            gameObject.name.Contains("Thor"))
        {
            radius *= 0.8f; // Make character colliders 80% of their original size
        }
        
        // Special handling for objects that should have larger spacing
        if (gameObject.name.Contains("BarbaStand") || 
            gameObject.name.Contains("hammer") || 
            gameObject.name.Contains("Mayo"))
        {
            radius *= 0.9f; // Make object colliders 90% of their original size
        }
        
        // Remove all existing colliders
        foreach (Collider2D collider in collidersToRemove)
        {
            Object.DestroyImmediate(collider);
        }
        
        // Add the new circle collider
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.offset = offset;
        circleCollider.radius = radius;
        circleCollider.isTrigger = isTrigger;
        
        Debug.Log($"Converted {gameObject.name} to CircleCollider2D with radius {radius}");
        return true;
    }
}