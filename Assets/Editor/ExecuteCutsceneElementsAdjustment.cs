using UnityEngine;
using UnityEditor;

public class ExecuteCutsceneElementsAdjustment
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        // Execute the adjustment automatically when the script is loaded
        EditorApplication.delayCall += () =>
        {
            AdjustCutsceneElements.AdjustElements();
        };
    }
}