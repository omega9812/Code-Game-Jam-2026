using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AdjustCutsceneElements : EditorWindow
{
    [MenuItem("Tools/Adjust Cutscene Elements")]
    public static void AdjustElements()
    {
        // Adjust character sizes
        AdjustCharacterSize("Bob", 4.0f);
        AdjustCharacterSize("Clown", 4.0f);
        
        // Adjust dialogue text
        AdjustDialogueText();
        
        Debug.Log("Cutscene elements adjusted successfully!");
    }
    
    private static void AdjustCharacterSize(string characterName, float newScale)
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
    
    private static void AdjustDialogueText()
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
}