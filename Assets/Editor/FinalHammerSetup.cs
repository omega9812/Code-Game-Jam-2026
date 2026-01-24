using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class FinalHammerSetup
{
    public static void Execute()
    {
        Debug.Log("Running Final Hammer Game Setup...");
        
        // Fix weight positioning - should be on the tower
        FixWeightPosition();
        
        // Configure the UIHammerStrengthGame component
        ConfigureHammerGameManager();
        
        // Hide canvas by default
        HideCanvas();
        
        // Mark scene dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("Final Hammer Game setup complete!");
    }
    
    static void FixWeightPosition()
    {
        // The weight should be positioned on the tower, not to the right
        var weight = GameObject.Find("HammerGameCanvas/WeightImage");
        var stand = GameObject.Find("HammerGameCanvas/StandImage");
        
        if (weight != null && stand != null)
        {
            var weightRect = weight.GetComponent<RectTransform>();
            var standRect = stand.GetComponent<RectTransform>();
            
            // Position weight at the same X as the stand, at the bottom
            weightRect.anchoredPosition = new Vector2(standRect.anchoredPosition.x, -180);
            weightRect.sizeDelta = new Vector2(40, 40);
            
            // Make sure weight is in front of stand
            weight.transform.SetAsLastSibling();
            
            EditorUtility.SetDirty(weight);
        }
        
        // Also adjust the stand to be taller and more visible
        if (stand != null)
        {
            var standRect = stand.GetComponent<RectTransform>();
            standRect.sizeDelta = new Vector2(80, 450);
            standRect.anchoredPosition = new Vector2(0, 20);
            EditorUtility.SetDirty(stand);
        }
        
        // Adjust hammer position
        var hammer = GameObject.Find("HammerGameCanvas/HammerImage");
        if (hammer != null)
        {
            var hammerRect = hammer.GetComponent<RectTransform>();
            hammerRect.anchoredPosition = new Vector2(-180, -180);
            hammerRect.sizeDelta = new Vector2(150, 150);
            EditorUtility.SetDirty(hammer);
        }
    }
    
    static void ConfigureHammerGameManager()
    {
        var manager = GameObject.Find("UIHammerGameManager");
        if (manager == null) return;
        
        var script = manager.GetComponent<UIHammerStrengthGame>();
        if (script == null) return;
        
        // Update weight Y positions to match new layout
        script.weightMinY = -180f;
        script.weightMaxY = 220f;
        
        // Update hammer positions
        script.hammerBackX = -60f;
        script.hammerImpactX = -180f;
        
        EditorUtility.SetDirty(script);
        EditorUtility.SetDirty(manager);
    }
    
    static void HideCanvas()
    {
        var canvas = GameObject.Find("HammerGameCanvas");
        if (canvas != null)
        {
            canvas.SetActive(false);
            EditorUtility.SetDirty(canvas);
        }
    }
}
