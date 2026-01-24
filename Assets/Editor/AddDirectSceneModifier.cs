using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class AddDirectSceneModifier : Editor
{
    [MenuItem("Tools/Add Direct Scene Modifier")]
    public static void AddModifier()
    {
        // Create a new GameObject for the modifier
        GameObject modifierObject = new GameObject("SceneModifier");
        modifierObject.AddComponent<DirectSceneModifier>();
        
        Debug.Log("Added DirectSceneModifier to the scene");
        
        // Mark the scene as dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}

// Auto-execute when the script is loaded
[InitializeOnLoad]
public class AutoAddDirectSceneModifier
{
    static AutoAddDirectSceneModifier()
    {
        EditorApplication.delayCall += () =>
        {
            AddDirectSceneModifier.AddModifier();
        };
    }
}