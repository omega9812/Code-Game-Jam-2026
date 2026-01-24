using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ImmediateSceneModification
{
    public static void Execute()
    {
        // Find Bob and adjust scale
        GameObject bob = GameObject.Find("Bob");
        if (bob != null)
        {
            Vector3 scale = bob.transform.localScale;
            float xSign = Mathf.Sign(scale.x);
            float ySign = Mathf.Sign(scale.y);
            float zSign = Mathf.Sign(scale.z);
            bob.transform.localScale = new Vector3(xSign * 4.0f, ySign * 4.0f, zSign * 4.0f);
            Debug.Log("Bob scale set to 4.0");
        }
        else
        {
            Debug.LogError("Bob not found in scene");
        }

        // Find Clown and adjust scale
        GameObject clown = GameObject.Find("Clown");
        if (clown != null)
        {
            Vector3 scale = clown.transform.localScale;
            float xSign = Mathf.Sign(scale.x);
            float ySign = Mathf.Sign(scale.y);
            float zSign = Mathf.Sign(scale.z);
            clown.transform.localScale = new Vector3(xSign * 4.0f, ySign * 4.0f, zSign * 4.0f);
            Debug.Log("Clown scale set to 4.0");
        }
        else
        {
            Debug.LogError("Clown not found in scene");
        }

        // Find DialogueText and adjust properties
        GameObject dialogueText = GameObject.Find("DialogueBox/Panel/DialogueText");
        if (dialogueText != null)
        {
            Text textComponent = dialogueText.GetComponent<Text>();
            if (textComponent != null)
            {
                textComponent.fontSize = 36;
                textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                textComponent.resizeTextForBestFit = true;
                textComponent.resizeTextMinSize = 24;
                textComponent.resizeTextMaxSize = 48;
                Debug.Log("DialogueText font size set to 36");
            }
        }
        else
        {
            Debug.LogError("DialogueText not found in scene");
        }

        // Find DialogueBox/Panel and adjust properties
        GameObject panel = GameObject.Find("DialogueBox/Panel");
        if (panel != null)
        {
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.1f, 0.05f);
                rectTransform.anchorMax = new Vector2(0.9f, 0.35f);
                Debug.Log("Panel size adjusted");
            }
            
            Image image = panel.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color(0, 0, 0, 0.8f);
            }
        }
        else
        {
            Debug.LogError("Panel not found in scene");
        }

        // Find WaterSprayEffect and enhance it
        GameObject waterSpray = GameObject.Find("WaterSprayEffect");
        if (waterSpray != null)
        {
            ParticleSystem ps = waterSpray.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startSize = 0.3f;
                main.startSpeed = 5.0f;
                main.startColor = new Color(0.7f, 0.85f, 1f, 0.8f);
                
                var emission = ps.emission;
                emission.rateOverTime = 150;
                
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = 15f;
                shape.radius = 0.2f;
                
                Debug.Log("WaterSprayEffect enhanced");
            }
            
            // Position the water spray effect properly
            Transform clownHidingPosition = GameObject.Find("ClownHidingPosition")?.transform;
            if (clownHidingPosition != null)
            {
                waterSpray.transform.position = clownHidingPosition.position + new Vector3(0.5f, 0.2f, 0f);
                waterSpray.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            
            // Add a light to the water spray
            Transform existingLight = waterSpray.transform.Find("WaterSprayLight");
            if (existingLight == null)
            {
                GameObject light = new GameObject("WaterSprayLight");
                light.transform.SetParent(waterSpray.transform);
                light.transform.localPosition = Vector3.zero;
                
                Light lightComponent = light.AddComponent<Light>();
                lightComponent.type = LightType.Point;
                lightComponent.color = new Color(0.7f, 0.85f, 1.0f);
                lightComponent.intensity = 2.5f;
                lightComponent.range = 5.0f;
                
                Debug.Log("Light added to WaterSprayEffect");
            }
        }
        else
        {
            Debug.LogError("WaterSprayEffect not found in scene");
        }
        
        // Fix the CutsceneController references
        GameObject splashCutscene = GameObject.Find("SplashCutscene");
        if (splashCutscene != null)
        {
            CutsceneController controller = splashCutscene.GetComponent<CutsceneController>();
            if (controller != null)
            {
                controller.bobTransform = GameObject.Find("Bob")?.transform;
                controller.clownTransform = GameObject.Find("Clown")?.transform;
                controller.bobStartPosition = GameObject.Find("BobStartPosition")?.transform;
                controller.bobEndPosition = GameObject.Find("BobEndPosition")?.transform;
                controller.clownHidingPosition = GameObject.Find("ClownHidingPosition")?.transform;
                controller.waterSprayEffect = GameObject.Find("WaterSprayEffect");
                controller.dialoguePanel = GameObject.Find("DialogueBox/Panel");
                controller.dialogueText = GameObject.Find("DialogueBox/Panel/DialogueText")?.GetComponent<Text>();
                
                // Disable CutsceneManager to avoid conflicts
                CutsceneManager manager = splashCutscene.GetComponent<CutsceneManager>();
                if (manager != null)
                {
                    manager.enabled = false;
                }
                
                Debug.Log("Fixed CutsceneController references");
            }
        }
        else
        {
            Debug.LogError("SplashCutscene not found in scene");
        }
        
        Debug.Log("Immediate scene modification completed");
    }
}