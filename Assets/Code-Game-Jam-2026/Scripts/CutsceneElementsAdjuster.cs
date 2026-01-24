using UnityEngine;
using UnityEngine.UI;

public class CutsceneElementsAdjuster : MonoBehaviour
{
    void Awake()
    {
        // Adjust character sizes
        AdjustCharacterSize("Bob", 4.0f);
        AdjustCharacterSize("Clown", 4.0f);
        
        // Adjust dialogue text
        AdjustDialogueText();
        
        // Enhance water spray effect
        EnhanceWaterEffect();
        
        // Fix the CutsceneController references
        FixCutsceneControllerReferences();
        
        Debug.Log("Cutscene elements adjusted at runtime");
    }
    
    private void AdjustCharacterSize(string characterName, float newScale)
    {
        GameObject character = GameObject.Find(characterName);
        if (character == null)
        {
            Debug.LogError($"{characterName} not found in the scene!");
            return;
        }
        
        // Get current scale and preserve sign (for flipped characters)
        Vector3 currentScale = character.transform.localScale;
        float xSign = Mathf.Sign(currentScale.x);
        float ySign = Mathf.Sign(currentScale.y);
        float zSign = Mathf.Sign(currentScale.z);
        
        // Apply new scale while preserving sign
        character.transform.localScale = new Vector3(
            xSign * newScale,
            ySign * newScale,
            zSign * newScale
        );
        
        Debug.Log($"Adjusted {characterName}'s scale to {newScale}");
    }
    
    private void AdjustDialogueText()
    {
        // Find the dialogue text
        GameObject dialogueTextObj = GameObject.Find("DialogueBox/Panel/DialogueText");
        if (dialogueTextObj == null)
        {
            Debug.LogError("DialogueText not found in the scene!");
            return;
        }
        
        // Get the Text component
        Text dialogueText = dialogueTextObj.GetComponent<Text>();
        if (dialogueText == null)
        {
            Debug.LogError("Text component not found on DialogueText!");
            return;
        }
        
        // Adjust text properties
        dialogueText.fontSize = 36; // Increase font size
        dialogueText.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); // Use a clearer font
        dialogueText.alignment = TextAnchor.MiddleLeft;
        dialogueText.resizeTextForBestFit = true;
        dialogueText.resizeTextMinSize = 24;
        dialogueText.resizeTextMaxSize = 48;
        
        // Adjust the panel size
        GameObject panel = GameObject.Find("DialogueBox/Panel");
        if (panel != null)
        {
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                // Make the panel larger
                panelRect.anchorMin = new Vector2(0.1f, 0.05f);
                panelRect.anchorMax = new Vector2(0.9f, 0.35f);
                
                // Ensure the panel has a background
                Image panelImage = panel.GetComponent<Image>();
                if (panelImage != null)
                {
                    panelImage.color = new Color(0, 0, 0, 0.8f);
                }
            }
        }
        
        Debug.Log("Adjusted dialogue text properties");
    }
    
    private void EnhanceWaterEffect()
    {
        GameObject waterSprayEffect = GameObject.Find("WaterSprayEffect");
        if (waterSprayEffect == null)
        {
            Debug.LogError("WaterSprayEffect not found in the scene!");
            return;
        }
        
        ParticleSystem ps = waterSprayEffect.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            ps = waterSprayEffect.AddComponent<ParticleSystem>();
        }
        
        // Configure main module
        var main = ps.main;
        main.startSpeed = 5f;
        main.startSize = 0.3f;
        main.startLifetime = 1f;
        main.maxParticles = 1000;
        main.startColor = new Color(0.7f, 0.85f, 1f, 0.8f); // Light blue color
        
        // Configure emission
        var emission = ps.emission;
        emission.rateOverTime = 150;
        emission.enabled = true;
        
        // Configure shape
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.2f;
        shape.enabled = true;
        
        // Position the water spray effect properly
        Transform clownHidingPosition = GameObject.Find("ClownHidingPosition")?.transform;
        if (clownHidingPosition != null)
        {
            waterSprayEffect.transform.position = clownHidingPosition.position + new Vector3(0.5f, 0.2f, 0f);
            waterSprayEffect.transform.rotation = Quaternion.Euler(0, 0, -90); // Point horizontally
        }
        
        // Add a light to make the water particles more visible
        GameObject lightObj = new GameObject("WaterSprayLight");
        lightObj.transform.SetParent(waterSprayEffect.transform);
        lightObj.transform.localPosition = Vector3.zero;
        
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(0.7f, 0.85f, 1f);
        light.intensity = 2.5f;
        light.range = 5f;
        
        Debug.Log("Enhanced water spray effect");
    }
    
    private void FixCutsceneControllerReferences()
    {
        GameObject splashCutscene = GameObject.Find("SplashCutscene");
        if (splashCutscene == null)
        {
            Debug.LogError("SplashCutscene not found in the scene!");
            return;
        }
        
        CutsceneController controller = splashCutscene.GetComponent<CutsceneController>();
        if (controller == null)
        {
            Debug.LogError("CutsceneController component not found on SplashCutscene!");
            return;
        }
        
        // Set references
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