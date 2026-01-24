using UnityEngine;
using UnityEditor;

public class EndHammerGame : MonoBehaviour
{
    [MenuItem("Tools/End Hammer Game")]
    public static void EndGame()
    {
        // Find the UIHammerGameManager
        UIHammerStrengthGame hammerGame = GameObject.FindFirstObjectByType<UIHammerStrengthGame>();
        if (hammerGame == null)
        {
            Debug.LogError("Could not find UIHammerStrengthGame in the scene!");
            return;
        }

        // End the game
        hammerGame.EndGame();
        Debug.Log("Hammer game ended!");
    }
}