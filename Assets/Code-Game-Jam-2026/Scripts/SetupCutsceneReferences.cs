using UnityEngine;

public class SetupCutsceneReferences : MonoBehaviour
{
    public static void Execute()
    {
        // Find the SplashCutscene object
        GameObject splashCutscene = GameObject.Find("SplashCutscene");
        if (splashCutscene == null)
        {
            Debug.LogError("SplashCutscene GameObject not found!");
            return;
        }
        
        // Get the CutsceneController component
        CutsceneController controller = splashCutscene.GetComponent<CutsceneController>();
        if (controller == null)
        {
            Debug.LogError("CutsceneController component not found on SplashCutscene!");
            return;
        }
        
        // Find Bob and Clown
        GameObject bob = GameObject.Find("Bob");
        GameObject clown = GameObject.Find("Clown");
        
        if (bob != null)
        {
            controller.bobTransform = bob.transform;
            Debug.Log("Set Bob reference in CutsceneController");
        }
        else
        {
            Debug.LogError("Bob GameObject not found!");
        }
        
        if (clown != null)
        {
            controller.clownTransform = clown.transform;
            Debug.Log("Set Clown reference in CutsceneController");
        }
        else
        {
            Debug.LogError("Clown GameObject not found!");
        }
        
        Debug.Log("Cutscene references setup complete!");
    }
}