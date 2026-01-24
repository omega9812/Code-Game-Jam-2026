using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ConvertCollidersExecutor : EditorWindow
{
    [MenuItem("Tools/Convert All Colliders to Circle")]
    public static void Execute()
    {
        ConvertAllCollidersToCircle();
    }

    public static void ConvertAllCollidersToCircle()
    {
        // Find all colliders in the scene
        BoxCollider2D[] boxColliders = GameObject.FindObjectsOfType<BoxCollider2D>();
        PolygonCollider2D[] polygonColliders = GameObject.FindObjectsOfType<PolygonCollider2D>();
        CapsuleCollider2D[] capsuleColliders = GameObject.FindObjectsOfType<CapsuleCollider2D>();
        
        List<GameObject> processedObjects = new List<GameObject>();
        
        // Process box colliders
        foreach (BoxCollider2D boxCollider in boxColliders)
        {
            if (!processedObjects.Contains(boxCollider.gameObject))
            {
                ConvertToCircleCollider(boxCollider);
                processedObjects.Add(boxCollider.gameObject);
            }
        }
        
        // Process polygon colliders
        foreach (PolygonCollider2D polygonCollider in polygonColliders)
        {
            if (!processedObjects.Contains(polygonCollider.gameObject))
            {
                ConvertToCircleCollider(polygonCollider);
                processedObjects.Add(polygonCollider.gameObject);
            }
        }
        
        // Process capsule colliders
        foreach (CapsuleCollider2D capsuleCollider in capsuleColliders)
        {
            if (!processedObjects.Contains(capsuleCollider.gameObject))
            {
                ConvertToCircleCollider(capsuleCollider);
                processedObjects.Add(capsuleCollider.gameObject);
            }
        }
        
        Debug.Log($"Converted {processedObjects.Count} colliders to circle colliders");
    }
    
    static void ConvertToCircleCollider(Collider2D oldCollider)
    {
        GameObject gameObject = oldCollider.gameObject;
        Vector2 offset = oldCollider.offset;
        bool isTrigger = oldCollider.isTrigger;
        
        // Calculate appropriate radius based on collider type
        float radius = 0.5f;
        
        if (oldCollider is BoxCollider2D)
        {
            BoxCollider2D boxCollider = oldCollider as BoxCollider2D;
            // Use half of the average of width and height for the radius
            radius = (boxCollider.size.x + boxCollider.size.y) * 0.25f;
        }
        else if (oldCollider is PolygonCollider2D)
        {
            PolygonCollider2D polyCollider = oldCollider as PolygonCollider2D;
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
        else if (oldCollider is CapsuleCollider2D)
        {
            CapsuleCollider2D capsuleCollider = oldCollider as CapsuleCollider2D;
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
        
        // Remove the old collider
        Object.DestroyImmediate(oldCollider);
        
        // Add the new circle collider
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.offset = offset;
        circleCollider.radius = radius;
        circleCollider.isTrigger = isTrigger;
        
        Debug.Log($"Converted {gameObject.name} to CircleCollider2D with radius {radius}");
    }
}