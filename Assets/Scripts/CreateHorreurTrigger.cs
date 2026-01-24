using UnityEngine;
using UnityEditor;

public class CreateHorreurTrigger
{
    public static void Execute()
    {
        // Create a new GameObject
        GameObject triggerObject = new GameObject("HorreurSceneTrigger");
        
        // Add our component
        var trigger = triggerObject.AddComponent<InventoryHorreurTrigger>();
        
        // Try to find a light in the scene to assign
        Light[] lights = Object.FindObjectsOfType<Light>();
        if (lights.Length > 0)
        {
            trigger.SendMessage("SetSceneLight", lights[0], SendMessageOptions.DontRequireReceiver);
        }
        
        Debug.Log("Created HorreurSceneTrigger GameObject with InventoryHorreurTrigger component");
        
        // Select the created object
        Selection.activeGameObject = triggerObject;
    }
}