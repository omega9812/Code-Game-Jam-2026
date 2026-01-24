using UnityEngine;
using UnityEditor;

public class AddWhackAMoleManager : EditorWindow
{
    [MenuItem("Game/Add Whack-A-Mole Manager")]
    static void Init()
    {
        // Create a new GameObject for the manager
        GameObject managerObject = new GameObject("WhackAMoleMinigameManager");
        
        // Add the manager component
        WhackAMoleMinigameManager manager = managerObject.AddComponent<WhackAMoleMinigameManager>();
        
        // Set default values
        manager.clownObjectName = "Clown";
        
        // Create a spawn point
        GameObject spawnPoint = new GameObject("MinigameSpawnPoint");
        spawnPoint.transform.position = Vector3.zero;
        
        // Assign the spawn point
        manager.minigameSpawnPoint = spawnPoint.transform;
        
        // Select the manager object
        Selection.activeGameObject = managerObject;
        
        Debug.Log("Whack-A-Mole Manager added to the scene. Please set up the clown object name if needed.");
    }
}