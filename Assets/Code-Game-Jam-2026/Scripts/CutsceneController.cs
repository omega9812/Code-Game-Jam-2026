using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Use fully qualified name to avoid ambiguity
using SceneManagement = UnityEngine.SceneManagement;
using AC;

public class CutsceneController : MonoBehaviour
{
    [Header("Characters")]
    public Transform bobTransform;
    public Transform clownTransform;
    
    [Header("Positions")]
    public Transform bobStartPosition;
    public Transform bobEndPosition;
    public Transform clownHidingPosition;
    
    [Header("Water Spray")]
    public GameObject waterSprayEffect;
    
    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    
    // Animation references
    private Animator bobAnimator;
    private Animator clownAnimator;
    
    // Cutscene state
    private bool cutsceneStarted = false;
    
    void Start()
    {
        // Get animator references
        if (bobTransform != null)
            bobAnimator = bobTransform.GetComponentInChildren<Animator>();
        
        if (clownTransform != null)
            clownAnimator = clownTransform.GetComponentInChildren<Animator>();
        
        // Set initial positions
        if (bobTransform != null && bobStartPosition != null)
            bobTransform.position = bobStartPosition.position;
        
        if (clownTransform != null && clownHidingPosition != null)
            clownTransform.position = clownHidingPosition.position;
        
        // Make sure water spray effect is initially disabled
        if (waterSprayEffect != null)
        {
            waterSprayEffect.SetActive(false);
        }
        
        // Create dialogue UI if not already in scene
        if (dialoguePanel == null)
        {
            // Create dialogue panel
            dialoguePanel = new GameObject("DialoguePanel");
            dialoguePanel.transform.SetParent(transform);
            
            // Add Canvas component
            Canvas canvas = dialoguePanel.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // Add CanvasScaler component
            CanvasScaler canvasScaler = dialoguePanel.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add GraphicRaycaster component
            dialoguePanel.AddComponent<GraphicRaycaster>();
            
            // Create panel background
            GameObject panelBg = new GameObject("PanelBackground");
            panelBg.transform.SetParent(dialoguePanel.transform, false);
            
            // Add RectTransform component to panel
            RectTransform panelRectTransform = panelBg.AddComponent<RectTransform>();
            panelRectTransform.anchorMin = new Vector2(0.1f, 0.1f);
            panelRectTransform.anchorMax = new Vector2(0.9f, 0.3f);
            panelRectTransform.offsetMin = Vector2.zero;
            panelRectTransform.offsetMax = Vector2.zero;
            
            // Add Image component to panel
            Image panelImage = panelBg.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);
            
            // Create text
            GameObject textObject = new GameObject("DialogueText");
            textObject.transform.SetParent(panelBg.transform, false);
            
            // Add RectTransform component to text
            RectTransform textRectTransform = textObject.AddComponent<RectTransform>();
            textRectTransform.anchorMin = new Vector2(0.05f, 0.1f);
            textRectTransform.anchorMax = new Vector2(0.95f, 0.9f);
            textRectTransform.offsetMin = Vector2.zero;
            textRectTransform.offsetMax = Vector2.zero;
            
            // Add Text component to text
            dialogueText = textObject.AddComponent<Text>();
            dialogueText.text = "";
            dialogueText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            dialogueText.fontSize = 24;
            dialogueText.color = Color.white;
            dialogueText.alignment = TextAnchor.MiddleLeft;
            
            // Initially hide the dialogue panel
            dialoguePanel.SetActive(false);
        }
        
        // Start the cutscene
        StartCoroutine(PlayCutscene());
    }
    
    IEnumerator PlayCutscene()
    {
        if (cutsceneStarted)
            yield break;
            
        cutsceneStarted = true;
        
        // Wait a moment before starting
        yield return new WaitForSeconds(1f);
        
        // 1. Bob walks down the street
        if (bobAnimator != null)
        {
            // Set walking animation using SPUM's RunState parameter
            bobAnimator.SetInteger("RunState", 1); // 1 = walk
        }
        
        // Move Bob from start to end position
        float walkDuration = 3f;
        float elapsedTime = 0f;
        Vector3 startPos = bobTransform.position;
        Vector3 endPos = bobEndPosition.position;
        
        while (elapsedTime < walkDuration)
        {
            bobTransform.position = Vector3.Lerp(startPos, endPos, elapsedTime / walkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        bobTransform.position = endPos;
        
        // 2. Bob stops walking
        if (bobAnimator != null)
        {
            bobAnimator.SetInteger("RunState", 0); // 0 = idle
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 3. Clown sprays water
        if (waterSprayEffect != null)
        {
            waterSprayEffect.SetActive(true);
            
            // Make sure the particle system is playing
            ParticleSystem ps = waterSprayEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
        
        yield return new WaitForSeconds(1f);
        
        // 4. Bob reacts (damaged animation)
        if (bobAnimator != null)
        {
            bobAnimator.SetTrigger("Damaged"); // SPUM already has this trigger
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // Turn off water spray
        if (waterSprayEffect != null)
        {
            waterSprayEffect.SetActive(false);
        }
        
        // 5. Clown laughs
        if (clownAnimator != null)
        {
            clownAnimator.SetTrigger("AttackState"); // Using SPUM's attack animation as "laugh"
        }
        
        // 6. Show dialogue - Clown laughing
        if (dialoguePanel != null && dialogueText != null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = "Hahaha! Je t'ai eu!";
        }
        
        yield return new WaitForSeconds(2f);
        
        // 7. Clown invites Bob to the fair
        if (dialogueText != null)
        {
            dialogueText.text = "Hey, come to the nearby fair with me!";
        }
        
        yield return new WaitForSeconds(3f);
        
        // Hide dialogue
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        
        // Wait a moment before transitioning to the next scene
        yield return new WaitForSeconds(1f);
        
        // Load the "New Scene"
        Debug.Log("Cutscene completed! Loading New Scene...");
        SceneManagement.SceneManager.LoadScene("New Scene");
    }
}