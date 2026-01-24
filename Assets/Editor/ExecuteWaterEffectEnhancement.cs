using UnityEngine;
using UnityEditor;

public class ExecuteWaterEffectEnhancement
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        // Execute the enhancement automatically when the script is loaded
        EditorApplication.delayCall += () =>
        {
            EnhanceWaterEffect.EnhanceEffect();
        };
    }
}