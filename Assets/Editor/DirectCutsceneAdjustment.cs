using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class DirectCutsceneAdjustment : Editor
{
    [MenuItem("Tools/Direct Cutscene Adjustment")]
    public static void AdjustCutsceneDirectly()
    {
        // Execute the script directly
        string scriptContent = @"
using UnityEngine;
using UnityEngine.UI;

public class CutsceneAdjuster : MonoBehaviour
{
    void Start()
    {
        // Adjust Bob character
        GameObject bob = GameObject.Find(""Bob"");
        if (bob != null)
        {
            Vector3 bobScale = bob.transform.localScale;
            float xSign = Mathf.Sign(bobScale.x);
            float ySign = Mathf.Sign(bobScale.y);
            float zSign = Mathf.Sign(bobScale.z);
            bob.transform.localScale = new Vector3(xSign * 4.0f, ySign * 4.0f, zSign * 4.0f);
            Debug.Log(""Adjusted Bob's scale to 4.0"");
        }
        else
        {
            Debug.LogError(""Bob not found in scene"");
        }

        // Adjust Clown character
        GameObject clown = GameObject.Find(""Clown"");
        if (clown != null)
        {
            Vector3 clownScale = clown.transform.localScale;
            float xSign = Mathf.Sign(clownScale.x);
            float ySign = Mathf.Sign(clownScale.y);
            float zSign = Mathf.Sign(clownScale.z);
            clown.transform.localScale = new Vector3(xSign * 4.0f, ySign * 4.0f, zSign * 4.0f);
            Debug.Log(""Adjusted Clown's scale to 4.0"");
        }
        else
        {
            Debug.LogError(""Clown not found in scene"");
        }

        // Adjust dialogue text
        GameObject dialogueTextObj = GameObject.Find(""DialogueBox/Panel/DialogueText"");
        if (dialogueTextObj != null)
        {
            Text dialogueText = dialogueTextObj.GetComponent<Text>();
            if (dialogueText != null)
            {
                dialogueText.fontSize = 36;
                dialogueText.font = Resources.GetBuiltinResource<Font>(""Arial.ttf"");
                dialogueText.alignment = TextAnchor.MiddleLeft;
                dialogueText.resizeTextForBestFit = true;
                dialogueText.resizeTextMinSize = 24;
                dialogueText.resizeTextMaxSize = 48;
                Debug.Log(""Adjusted dialogue text properties"");
            }
        }
        else
        {
            Debug.LogError(""DialogueText not found in scene"");
        }

        // Adjust dialogue panel
        GameObject panel = GameObject.Find(""DialogueBox/Panel"");
        if (panel != null)
        {
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                panelRect.anchorMin = new Vector2(0.1f, 0.05f);
                panelRect.anchorMax = new Vector2(0.9f, 0.35f);
                
                Image panelImage = panel.GetComponent<Image>();
                if (panelImage != null)
                {
                    panelImage.color = new Color(0, 0, 0, 0.8f);
                }
                Debug.Log(""Adjusted dialogue panel"");
            }
        }
        else
        {
            Debug.LogError(""Panel not found in scene"");
        }

        // Enhance water spray effect
        GameObject waterSprayEffect = GameObject.Find(""WaterSprayEffect"");
        if (waterSprayEffect != null)
        {
            ParticleSystem ps = waterSprayEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startSpeed = 5f;
                main.startSize = 0.3f;
                main.startLifetime = 1f;
                main.maxParticles = 1000;
                main.startColor = new Color(0.7f, 0.85f, 1f, 0.8f);

                var emission = ps.emission;
                emission.rateOverTime = 150;
                emission.enabled = true;

                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = 15f;
                shape.radius = 0.2f;
                shape.enabled = true;

                // Position the water spray effect properly
                Transform clownHidingPosition = GameObject.Find(""ClownHidingPosition"")?.transform;
                if (clownHidingPosition != null)
                {
                    waterSprayEffect.transform.position = clownHidingPosition.position + new Vector3(0.5f, 0.2f, 0f);
                    waterSprayEffect.transform.rotation = Quaternion.Euler(0, 0, -90);
                }

                // Add a light to make the water particles more visible
                GameObject lightObj = new GameObject(""WaterSprayLight"");
                lightObj.transform.SetParent(waterSprayEffect.transform);
                lightObj.transform.localPosition = Vector3.zero;
                
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = new Color(0.7f, 0.85f, 1f);
                light.intensity = 2.5f;
                light.range = 5f;

                Debug.Log(""Enhanced water spray effect"");
            }
        }
        else
        {
            Debug.LogError(""WaterSprayEffect not found in scene"");
        }

        // Fix the CutsceneController references
        GameObject splashCutscene = GameObject.Find(""SplashCutscene"");
        if (splashCutscene != null)
        {
            CutsceneController controller = splashCutscene.GetComponent<CutsceneController>();
            if (controller != null)
            {
                controller.bobTransform = GameObject.Find(""Bob"")?.transform;
                controller.clownTransform = GameObject.Find(""Clown"")?.transform;
                controller.bobStartPosition = GameObject.Find(""BobStartPosition"")?.transform;
                controller.bobEndPosition = GameObject.Find(""BobEndPosition"")?.transform;
                controller.clownHidingPosition = GameObject.Find(""ClownHidingPosition"")?.transform;
                controller.waterSprayEffect = GameObject.Find(""WaterSprayEffect"");
                controller.dialoguePanel = GameObject.Find(""DialogueBox/Panel"");
                controller.dialogueText = GameObject.Find(""DialogueBox/Panel/DialogueText"")?.GetComponent<Text>();
                
                // Disable CutsceneManager to avoid conflicts
                CutsceneManager manager = splashCutscene.GetComponent<CutsceneManager>();
                if (manager != null)
                {
                    manager.enabled = false;
                }
                
                Debug.Log(""Fixed CutsceneController references"");
            }
        }
        else
        {
            Debug.LogError(""SplashCutscene not found in scene"");
        }

        // Destroy this GameObject after execution
        Destroy(gameObject);
    }
}
";

        // Create a temporary GameObject with the script
        GameObject tempGO = new GameObject("TempCutsceneAdjuster");
        string tempScriptPath = "Assets/Editor/TempCutsceneAdjuster.cs";
        
        // Write the temporary script
        System.IO.File.WriteAllText(tempScriptPath, scriptContent);
        AssetDatabase.Refresh();
        
        // Add the script component
        MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(tempScriptPath);
        if (script != null)
        {
            System.Type scriptType = script.GetClass();
            if (scriptType != null)
            {
                tempGO.AddComponent(scriptType);
                Debug.Log("Added CutsceneAdjuster script to temporary GameObject");
            }
        }
        
        // Mark the scene as dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        Debug.Log("Direct cutscene adjustment completed");
    }
}