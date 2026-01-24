using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class AddCutsceneAdjusterComponent : Editor
{
    [MenuItem("Tools/Add Cutscene Adjuster Component")]
    public static void AddComponent()
    {
        // Find the SplashCutscene GameObject
        GameObject splashCutscene = GameObject.Find("SplashCutscene");
        if (splashCutscene == null)
        {
            Debug.LogError("SplashCutscene not found in the scene!");
            return;
        }
        
        // Add the CutsceneElementsAdjuster component
        if (splashCutscene.GetComponent<CutsceneElementsAdjuster>() == null)
        {
            splashCutscene.AddComponent<CutsceneElementsAdjuster>();
            Debug.Log("Added CutsceneElementsAdjuster component to SplashCutscene");
        }
        else
        {
            Debug.Log("CutsceneElementsAdjuster component already exists on SplashCutscene");
        }
        
        // Mark the scene as dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}

// Auto-execute when the script is loaded
[InitializeOnLoad]
public class AutoAddCutsceneAdjusterComponent
{
    static AutoAddCutsceneAdjusterComponent()
    {
        EditorApplication.delayCall += () =>
        {
            AddCutsceneAdjusterComponent.AddComponent();
        };
    }
}