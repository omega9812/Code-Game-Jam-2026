using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RunColliderConversion : EditorWindow
{
    [MenuItem("Tools/Convert All Colliders to Circle")]
    static void Init()
    {
        // Create a temporary GameObject with our converter component
        GameObject tempObject = new GameObject("TempColliderConverter");
        ColliderConverter converter = tempObject.AddComponent<ColliderConverter>();
        
        // Run the conversion
        converter.ConvertAllCollidersToCircle();
        
        // Clean up
        DestroyImmediate(tempObject);
        
        // Save the scene
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        
        Debug.Log("Collider conversion completed and scene saved");
    }
}