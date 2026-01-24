using UnityEngine;
using UnityEngine.UI;

public class CreateDialogueUI : MonoBehaviour
{
    public static void Execute()
    {
        // Find the DialogueBox GameObject
        GameObject dialogueBox = GameObject.Find("DialogueBox");
        
        if (dialogueBox == null)
        {
            Debug.LogError("DialogueBox GameObject not found!");
            return;
        }
        
        // Add Canvas component
        Canvas canvas = dialogueBox.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // Add CanvasScaler component
        CanvasScaler canvasScaler = dialogueBox.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        
        // Add GraphicRaycaster component
        dialogueBox.AddComponent<GraphicRaycaster>();
        
        // Create panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(dialogueBox.transform, false);
        
        // Add RectTransform component to panel
        RectTransform panelRectTransform = panel.AddComponent<RectTransform>();
        panelRectTransform.anchorMin = new Vector2(0.1f, 0.1f);
        panelRectTransform.anchorMax = new Vector2(0.9f, 0.3f);
        panelRectTransform.offsetMin = Vector2.zero;
        panelRectTransform.offsetMax = Vector2.zero;
        
        // Add Image component to panel
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        
        // Create text
        GameObject textObject = new GameObject("DialogueText");
        textObject.transform.SetParent(panel.transform, false);
        
        // Add RectTransform component to text
        RectTransform textRectTransform = textObject.AddComponent<RectTransform>();
        textRectTransform.anchorMin = new Vector2(0.05f, 0.1f);
        textRectTransform.anchorMax = new Vector2(0.95f, 0.9f);
        textRectTransform.offsetMin = Vector2.zero;
        textRectTransform.offsetMax = Vector2.zero;
        
        // Add Text component to text
        Text text = textObject.AddComponent<Text>();
        text.text = "Dialogue text will appear here";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        
        // Initially disable the dialogue box
        dialogueBox.SetActive(false);
        
        Debug.Log("Dialogue UI created successfully!");
    }
}