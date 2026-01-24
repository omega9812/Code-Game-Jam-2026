using UnityEngine;
using System.Collections.Generic;

public class ColliderConverter : MonoBehaviour
{
    public void ConvertAllCollidersToCircle()
    {
        // Find all colliders in the scene
        BoxCollider2D[] boxColliders = FindObjectsOfType<BoxCollider2D>();
        PolygonCollider2D[] polygonColliders = FindObjectsOfType<PolygonCollider2D>();
        CapsuleCollider2D[] capsuleColliders = FindObjectsOfType<CapsuleCollider2D>();
        
        List<GameObject> objectsToProcess = new List<GameObject>();
        
        // Collect all objects with colliders
        foreach (BoxCollider2D collider in boxColliders)
        {
            if (!objectsToProcess.Contains(collider.gameObject))
            {
                objectsToProcess.Add(collider.gameObject);
            }
        }
        
        foreach (PolygonCollider2D collider in polygonColliders)
        {
            if (!objectsToProcess.Contains(collider.gameObject))
            {
                objectsToProcess.Add(collider.gameObject);
            }
        }
        
        foreach (CapsuleCollider2D collider in capsuleColliders)
        {
            if (!objectsToProcess.Contains(collider.gameObject))
            {
                objectsToProcess.Add(collider.gameObject);
            }
        }
        
        // Process each object
        foreach (GameObject obj in objectsToProcess)
        {
            ConvertObjectColliders(obj);
        }
        
        Debug.Log($"Converted {objectsToProcess.Count} objects to use circle colliders");
    }
    
    void ConvertObjectColliders(GameObject gameObject)
    {
        // Get all colliders on the object
        Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
        
        if (colliders.Length == 0)
            return;
            
        // Calculate appropriate radius
        float radius = 0.5f;
        Vector2 offset = Vector2.zero;
        bool isTrigger = false;
        
        // Use the first collider to determine properties
        Collider2D firstCollider = colliders[0];
        offset = firstCollider.offset;
        isTrigger = firstCollider.isTrigger;
        
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
        else if (firstCollider is CircleCollider2D)
        {
            // Already a circle collider, just return
            return;
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
        foreach (Collider2D collider in colliders)
        {
            DestroyImmediate(collider);
        }
        
        // Add the new circle collider
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.offset = offset;
        circleCollider.radius = radius;
        circleCollider.isTrigger = isTrigger;
        
        Debug.Log($"Converted {gameObject.name} to CircleCollider2D with radius {radius}");
    }
}