using UnityEngine;

public class CreateStreetBackground : MonoBehaviour
{
    public static void Execute()
    {
        // Create street background
        GameObject streetBackground = new GameObject("StreetBackground");
        SpriteRenderer streetRenderer = streetBackground.AddComponent<SpriteRenderer>();
        
        // Create a simple colored sprite for the street
        Texture2D streetTexture = new Texture2D(100, 100);
        Color[] colors = new Color[100 * 100];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0.3f, 0.3f, 0.3f); // Dark gray for the street
        }
        streetTexture.SetPixels(colors);
        streetTexture.Apply();
        
        Sprite streetSprite = Sprite.Create(streetTexture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
        streetRenderer.sprite = streetSprite;
        
        // Position and scale the street
        streetBackground.transform.position = new Vector3(0, -4, 1); // Behind characters
        streetBackground.transform.localScale = new Vector3(20, 2, 1); // Cover the screen width
        
        // Create sidewalk
        GameObject sidewalk = new GameObject("Sidewalk");
        sidewalk.transform.SetParent(streetBackground.transform);
        SpriteRenderer sidewalkRenderer = sidewalk.AddComponent<SpriteRenderer>();
        
        // Create a simple colored sprite for the sidewalk
        Texture2D sidewalkTexture = new Texture2D(100, 100);
        Color[] sidewalkColors = new Color[100 * 100];
        for (int i = 0; i < sidewalkColors.Length; i++)
        {
            sidewalkColors[i] = new Color(0.7f, 0.7f, 0.7f); // Light gray for the sidewalk
        }
        sidewalkTexture.SetPixels(sidewalkColors);
        sidewalkTexture.Apply();
        
        Sprite sidewalkSprite = Sprite.Create(sidewalkTexture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
        sidewalkRenderer.sprite = sidewalkSprite;
        
        // Position and scale the sidewalk
        sidewalk.transform.localPosition = new Vector3(0, 0.6f, -0.1f);
        sidewalk.transform.localScale = new Vector3(1, 0.2f, 1);
        
        Debug.Log("Street background created successfully!");
    }
}