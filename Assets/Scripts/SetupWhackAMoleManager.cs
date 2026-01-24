using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SetupWhackAMoleManager : MonoBehaviour
{
    public static void Execute()
    {
        // Find the WhackAMoleMinigameManager
        WhackAMoleMinigameManager manager = GameObject.FindObjectOfType<WhackAMoleMinigameManager>();
        if (manager == null)
        {
            Debug.LogError("WhackAMoleMinigameManager not found in the scene!");
            return;
        }
        
        // Load the mole prefab
#if UNITY_EDITOR
        GameObject molePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Mole.prefab");
        if (molePrefab == null)
        {
            Debug.LogError("Mole prefab not found at Assets/Prefabs/Mole.prefab");
            return;
        }
        
        // Create the whack-a-mole game prefab
        GameObject whackAMoleGame = new GameObject("WhackAMoleGame");
        
        // Add the game controller
        WhackAMoleGame gameController = whackAMoleGame.AddComponent<WhackAMoleGame>();
        gameController.gameDuration = 30f;
        gameController.scoreToWin = 20;
        
        // Create holes container
        GameObject holesContainer = new GameObject("Holes");
        holesContainer.transform.SetParent(whackAMoleGame.transform);
        
        // Create 9 holes in a 3x3 grid
        Transform[] holes = new Transform[9];
        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;
            
            GameObject hole = new GameObject("Hole_" + i);
            hole.transform.SetParent(holesContainer.transform);
            
            // Position holes in a grid
            float x = (col - 1) * 2.5f;
            float y = (row - 1) * 2.0f;
            hole.transform.localPosition = new Vector3(x, y, 0);
            
            holes[i] = hole.transform;
        }
        
        // Set up the holes in the game controller
        gameController.holes = holes;
        
        // Set up the mole prefab in the game controller
        gameController.molePrefab = molePrefab.GetComponent<Mole>();
        
        // Create audio source
        GameObject audioObj = new GameObject("AudioSource");
        audioObj.transform.SetParent(whackAMoleGame.transform);
        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        gameController.audioSource = audioSource;
        
        // Create hammer cursor
        GameObject hammerCursor = new GameObject("HammerCursor");
        hammerCursor.transform.SetParent(whackAMoleGame.transform);
        HammerCursor hammerCursorScript = hammerCursor.AddComponent<HammerCursor>();
        SpriteRenderer hammerRenderer = hammerCursor.AddComponent<SpriteRenderer>();
        
        // Save the prefab
        string prefabPath = "Assets/Prefabs";
        if (!System.IO.Directory.Exists(prefabPath))
        {
            System.IO.Directory.CreateDirectory(prefabPath);
        }
        
        string whackAMolePrefabPath = prefabPath + "/WhackAMoleGame.prefab";
        PrefabUtility.SaveAsPrefabAsset(whackAMoleGame, whackAMolePrefabPath);
        Debug.Log("WhackAMoleGame prefab created at: " + whackAMolePrefabPath);
        
        // Clean up
        Object.DestroyImmediate(whackAMoleGame);
        
        // Assign the prefab to the manager
        GameObject whackAMolePrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(whackAMolePrefabPath);
        if (whackAMolePrefabAsset != null)
        {
            // manager.whackAMolePrefab = whackAMolePrefabAsset;
            Debug.Log("WhackAMoleGame prefab assigned to the manager");
        }
        else
        {
            Debug.LogError("Failed to load WhackAMoleGame prefab");
        }
#endif
    }
}