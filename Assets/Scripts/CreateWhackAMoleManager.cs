using UnityEngine;

public class CreateWhackAMoleManager : MonoBehaviour
{
    public static void Execute()
    {
        // Create a new GameObject for the manager
        GameObject managerObject = new GameObject("WhackAMoleMinigameManager");
        
        WhackAMoleMinigameManager manager = managerObject.AddComponent<WhackAMoleMinigameManager>();
        
        manager.clownObjectName = "Clown";
        manager.clownObject = GameObject.Find(manager.clownObjectName);
        
        Debug.Log("Whack-A-Mole Manager added to the scene. Assign 'whackAMoleMinigame' in Inspector.");
    }
}