using UnityEngine;
using UnityEditor;

public class TestHammerGame : MonoBehaviour
{
    [MenuItem("Tools/Test Hammer Game")]
    public static void TestGame()
    {
        // Find the UIHammerGameManager
        UIHammerStrengthGame hammerGame = GameObject.FindFirstObjectByType<UIHammerStrengthGame>();
        if (hammerGame == null)
        {
            Debug.LogError("Could not find UIHammerStrengthGame in the scene!");
            return;
        }

        // Start the game
        hammerGame.StartGame();
        Debug.Log("Hammer game started! The canvas should now be visible.");
        Debug.Log("Instructions: Click rapidly to fill the charge meter before time runs out!");
    }
}