using UnityEngine;

public class SetupStreetBackground : MonoBehaviour
{
    public static void Execute()
    {
        // Find the StreetBackground GameObject
        GameObject streetBackground = GameObject.Find("StreetBackground");
        
        if (streetBackground == null)
        {
            Debug.LogError("StreetBackground GameObject not found!");
            return;
        }
        
        // Set position and scale
        streetBackground.transform.position = new Vector3(0, -2, 1); // Behind characters
        streetBackground.transform.localScale = new Vector3(20, 5, 1); // Cover the screen
        
        // Add a SpriteRenderer component
        SpriteRenderer spriteRenderer = streetBackground.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = streetBackground.AddComponent<SpriteRenderer>();
        }
        
        // Create a simple colored material
        spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f); // Dark gray for the road
        
        // Create a sidewalk
        GameObject sidewalk = new GameObject("Sidewalk");
        sidewalk.transform.SetParent(streetBackground.transform);
        sidewalk.transform.localPosition = new Vector3(0, 0.4f, -0.1f);
        sidewalk.transform.localScale = new Vector3(1, 0.2f, 1);
        
        SpriteRenderer sidewalkRenderer = sidewalk.AddComponent<SpriteRenderer>();
        sidewalkRenderer.color = new Color(0.7f, 0.7f, 0.7f); // Light gray for the sidewalk
        
        Debug.Log("Street background setup complete!");
    }
}