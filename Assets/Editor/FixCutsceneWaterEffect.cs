using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FixCutsceneWaterEffect : EditorWindow
{
    [MenuItem("Tools/Fix Cutscene Water Effect")]
    public static void FixWaterEffect()
    {
        // Find the SplashCutscene GameObject
        GameObject splashCutscene = GameObject.Find("SplashCutscene");
        if (splashCutscene == null)
        {
            Debug.LogError("SplashCutscene not found in the scene!");
            return;
        }

        // Find the WaterSprayEffect GameObject
        GameObject waterSprayEffect = GameObject.Find("WaterSprayEffect");
        if (waterSprayEffect == null)
        {
            Debug.LogError("WaterSprayEffect not found in the scene!");
            return;
        }

        // Get the CutsceneController component
        CutsceneController controller = splashCutscene.GetComponent<CutsceneController>();
        if (controller == null)
        {
            Debug.LogError("CutsceneController component not found on SplashCutscene!");
            return;
        }

        // Set the waterSprayEffect reference
        controller.waterSprayEffect = waterSprayEffect;

        // Find the position references
        Transform bobStartPosition = GameObject.Find("BobStartPosition")?.transform;
        Transform bobEndPosition = GameObject.Find("BobEndPosition")?.transform;
        Transform clownHidingPosition = GameObject.Find("ClownHidingPosition")?.transform;

        if (bobStartPosition != null)
        {
            controller.bobStartPosition = bobStartPosition;
            Debug.Log("Set bobStartPosition reference in CutsceneController");
        }
        else
        {
            Debug.LogWarning("BobStartPosition not found in the scene!");
        }

        if (bobEndPosition != null)
        {
            controller.bobEndPosition = bobEndPosition;
            Debug.Log("Set bobEndPosition reference in CutsceneController");
        }
        else
        {
            Debug.LogWarning("BobEndPosition not found in the scene!");
        }

        if (clownHidingPosition != null)
        {
            controller.clownHidingPosition = clownHidingPosition;
            Debug.Log("Set clownHidingPosition reference in CutsceneController");
        }
        else
        {
            Debug.LogWarning("ClownHidingPosition not found in the scene!");
        }

        // Position the water spray effect near the clown
        if (clownHidingPosition != null)
        {
            // Position the water spray slightly in front of the clown
            waterSprayEffect.transform.position = clownHidingPosition.position + new Vector3(0.5f, 0f, 0f);
            waterSprayEffect.transform.rotation = Quaternion.Euler(0, 0, -90); // Point horizontally
            Debug.Log("Positioned water spray effect near the clown");
        }

        // Disable the CutsceneManager to avoid conflicts
        CutsceneManager manager = splashCutscene.GetComponent<CutsceneManager>();
        if (manager != null)
        {
            manager.enabled = false;
            Debug.Log("Disabled CutsceneManager to avoid conflicts with CutsceneController");
        }

        Debug.Log("Cutscene water effect fixed successfully!");
        
        // Mark the scene as dirty so the changes can be saved
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}