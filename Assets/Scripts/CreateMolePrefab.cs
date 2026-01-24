using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateMolePrefab : MonoBehaviour
{
    public static void Execute()
    {
        // Create a mole prefab
        GameObject moleObj = new GameObject("Mole");
        
        // Add components
        SpriteRenderer sr = moleObj.AddComponent<SpriteRenderer>();
        Mole moleScript = moleObj.AddComponent<Mole>();
        CircleCollider2D collider = moleObj.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        
        // Set up the mole script
        moleScript.appearStepTime = 0.06f;
        moleScript.visibleTime = 0.9f;
        moleScript.hitHoldTime = 0.15f;
        
        // Create sprites for the mole
        Texture2D texture = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        
        // Create a simple circle for the mole
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float dx = x - 32;
                float dy = y - 32;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (dist < 30)
                {
                    colors[y * 64 + x] = new Color(0.6f, 0.4f, 0.2f, 1f); // Brown color for the mole
                }
                else
                {
                    colors[y * 64 + x] = new Color(0f, 0f, 0f, 0f); // Transparent
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        // Create sprites
        Sprite normalSprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        
        // Create hit sprite (red version)
        Texture2D hitTexture = new Texture2D(64, 64);
        Color[] hitColors = new Color[64 * 64];
        
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float dx = x - 32;
                float dy = y - 32;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (dist < 30)
                {
                    hitColors[y * 64 + x] = new Color(1f, 0.2f, 0.2f, 1f); // Red color for hit state
                }
                else
                {
                    hitColors[y * 64 + x] = new Color(0f, 0f, 0f, 0f); // Transparent
                }
            }
        }
        
        hitTexture.SetPixels(hitColors);
        hitTexture.Apply();
        
        Sprite hitSprite = Sprite.Create(hitTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        
        // Assign sprites to the mole
        moleScript.appear1 = normalSprite;
        moleScript.appear2 = normalSprite;
        moleScript.hitSprite = hitSprite;
        
        // Set the initial sprite
        sr.sprite = normalSprite;
        
        // Save the prefab
#if UNITY_EDITOR
        string prefabPath = "Assets/Prefabs";
        if (!System.IO.Directory.Exists(prefabPath))
        {
            System.IO.Directory.CreateDirectory(prefabPath);
        }
        
        string molePrefabPath = prefabPath + "/Mole.prefab";
        PrefabUtility.SaveAsPrefabAsset(moleObj, molePrefabPath);
        Debug.Log("Mole prefab created at: " + molePrefabPath);
        
        // Clean up
        Object.DestroyImmediate(moleObj);
#endif
    }
}