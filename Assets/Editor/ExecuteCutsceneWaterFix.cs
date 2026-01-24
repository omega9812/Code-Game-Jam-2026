using UnityEngine;
using UnityEditor;

public class ExecuteCutsceneWaterFix
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        // Execute the fix automatically when the script is loaded
        EditorApplication.delayCall += () =>
        {
            FixCutsceneWaterEffect.FixWaterEffect();
        };
    }
}