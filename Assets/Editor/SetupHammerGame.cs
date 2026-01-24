using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class SetupHammerGame : MonoBehaviour
{
    [MenuItem("Tools/Setup Hammer Game")]
    public static void Execute()
    {
        // 1. Remove duplicate UIHammerGameManager (the one without components)
        RemoveDuplicateManagers();
        
        // 2. Setup the HammerGameCanvas UI properly
        SetupHammerGameUI();
        
        // 3. Configure the UIHammerStrengthGame component
        ConfigureHammerGameManager();
        
        Debug.Log("Hammer Game setup complete!");
    }
    
    static void RemoveDuplicateManagers()
    {
        var managers = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        GameObject managerToKeep = null;
        
        foreach (var t in managers)
        {
            if (t.name == "UIHammerGameManager")
            {
                var script = t.GetComponent<UIHammerStrengthGame>();
                if (script != null)
                {
                    managerToKeep = t.gameObject;
                }
                else
                {
                    // This is the duplicate without the script - delete it
                    Debug.Log("Removing duplicate UIHammerGameManager");
                    Undo.DestroyObjectImmediate(t.gameObject);
                }
            }
        }
    }
    
    static void SetupHammerGameUI()
    {
        var canvas = GameObject.Find("HammerGameCanvas");
        if (canvas == null)
        {
            Debug.LogError("HammerGameCanvas not found!");
            return;
        }
        
        var canvasRect = canvas.GetComponent<RectTransform>();
        
        // Configure canvas scaler for consistent sizing
        var scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            EditorUtility.SetDirty(scaler);
        }
        
        // Set canvas to be inactive by default (will be activated by Adventure Creator)
        canvas.SetActive(false);
        
        // Setup GameBackground - full screen dark overlay
        var background = GameObject.Find("HammerGameCanvas/GameBackground");
        if (background != null)
        {
            var bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgRect.SetAsFirstSibling();
            
            var bgImage = background.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            }
            EditorUtility.SetDirty(background);
        }
        
        // Setup StandImage - the vertical tower (high striker pole)
        var stand = GameObject.Find("HammerGameCanvas/StandImage");
        if (stand != null)
        {
            var standRect = stand.GetComponent<RectTransform>();
            standRect.anchorMin = new Vector2(0.5f, 0.5f);
            standRect.anchorMax = new Vector2(0.5f, 0.5f);
            standRect.pivot = new Vector2(0.5f, 0f);
            standRect.anchoredPosition = new Vector2(0, -250);
            standRect.sizeDelta = new Vector2(120, 500);
            
            var standImage = stand.GetComponent<Image>();
            if (standImage != null)
            {
                standImage.preserveAspect = true;
            }
            EditorUtility.SetDirty(stand);
        }
        
        // Setup WeightImage - the ball/puck that rises
        var weight = GameObject.Find("HammerGameCanvas/WeightImage");
        if (weight != null)
        {
            var weightRect = weight.GetComponent<RectTransform>();
            weightRect.anchorMin = new Vector2(0.5f, 0.5f);
            weightRect.anchorMax = new Vector2(0.5f, 0.5f);
            weightRect.pivot = new Vector2(0.5f, 0.5f);
            weightRect.anchoredPosition = new Vector2(0, -220); // Start at bottom of tower
            weightRect.sizeDelta = new Vector2(60, 60);
            
            var weightImage = weight.GetComponent<Image>();
            if (weightImage != null)
            {
                weightImage.color = new Color(1f, 0.3f, 0.3f, 1f); // Red ball
            }
            EditorUtility.SetDirty(weight);
        }
        
        // Setup HammerImage - the hammer/mallet at the bottom
        var hammer = GameObject.Find("HammerGameCanvas/HammerImage");
        if (hammer != null)
        {
            var hammerRect = hammer.GetComponent<RectTransform>();
            hammerRect.anchorMin = new Vector2(0.5f, 0.5f);
            hammerRect.anchorMax = new Vector2(0.5f, 0.5f);
            hammerRect.pivot = new Vector2(0.5f, 0.5f);
            hammerRect.anchoredPosition = new Vector2(-150, -300);
            hammerRect.sizeDelta = new Vector2(150, 150);
            
            var hammerImage = hammer.GetComponent<Image>();
            if (hammerImage != null)
            {
                hammerImage.preserveAspect = true;
            }
            EditorUtility.SetDirty(hammer);
        }
        
        // Remove duplicate TimerText if exists
        var timerTexts = canvas.GetComponentsInChildren<Transform>(true);
        int timerCount = 0;
        foreach (var t in timerTexts)
        {
            if (t.name == "TimerText")
            {
                timerCount++;
                if (timerCount > 1)
                {
                    Undo.DestroyObjectImmediate(t.gameObject);
                }
            }
        }
        
        // Remove duplicate ChargeSlider if exists
        int sliderCount = 0;
        foreach (var t in timerTexts)
        {
            if (t != null && t.name == "ChargeSlider")
            {
                sliderCount++;
                if (sliderCount > 1)
                {
                    Undo.DestroyObjectImmediate(t.gameObject);
                }
            }
        }
        
        // Setup TimerText - at the top
        var timer = GameObject.Find("HammerGameCanvas/TimerText");
        if (timer != null)
        {
            var timerRect = timer.GetComponent<RectTransform>();
            timerRect.anchorMin = new Vector2(0.5f, 1f);
            timerRect.anchorMax = new Vector2(0.5f, 1f);
            timerRect.pivot = new Vector2(0.5f, 1f);
            timerRect.anchoredPosition = new Vector2(0, -50);
            timerRect.sizeDelta = new Vector2(300, 80);
            
            // Check for TMP or regular Text
            var tmpText = timer.GetComponent<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = "3.0s";
                tmpText.fontSize = 48;
                tmpText.alignment = TextAlignmentOptions.Center;
                tmpText.color = Color.white;
            }
            else
            {
                var text = timer.GetComponent<Text>();
                if (text != null)
                {
                    text.text = "3.0s";
                    text.fontSize = 48;
                    text.alignment = TextAnchor.MiddleCenter;
                    text.color = Color.white;
                }
            }
            EditorUtility.SetDirty(timer);
        }
        
        // Setup InstructionText - below timer
        var instruction = GameObject.Find("HammerGameCanvas/InstructionText");
        if (instruction != null)
        {
            var instrRect = instruction.GetComponent<RectTransform>();
            instrRect.anchorMin = new Vector2(0.5f, 1f);
            instrRect.anchorMax = new Vector2(0.5f, 1f);
            instrRect.pivot = new Vector2(0.5f, 1f);
            instrRect.anchoredPosition = new Vector2(0, -130);
            instrRect.sizeDelta = new Vector2(600, 60);
            
            var tmpText = instruction.GetComponent<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = "Click to start! Spam click to charge!";
                tmpText.fontSize = 32;
                tmpText.alignment = TextAlignmentOptions.Center;
                tmpText.color = Color.yellow;
            }
            else
            {
                var text = instruction.GetComponent<Text>();
                if (text != null)
                {
                    text.text = "Click to start! Spam click to charge!";
                    text.fontSize = 32;
                    text.alignment = TextAnchor.MiddleCenter;
                    text.color = Color.yellow;
                }
            }
            EditorUtility.SetDirty(instruction);
        }
        
        // Setup ChargeSlider - vertical on the side
        var slider = GameObject.Find("HammerGameCanvas/ChargeSlider");
        if (slider != null)
        {
            var sliderRect = slider.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
            sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
            sliderRect.pivot = new Vector2(0.5f, 0.5f);
            sliderRect.anchoredPosition = new Vector2(200, 0);
            sliderRect.sizeDelta = new Vector2(30, 400);
            sliderRect.localRotation = Quaternion.Euler(0, 0, 90); // Rotate to be vertical
            
            var sliderComp = slider.GetComponent<Slider>();
            if (sliderComp != null)
            {
                sliderComp.minValue = 0;
                sliderComp.maxValue = 1;
                sliderComp.value = 0;
                sliderComp.interactable = false;
            }
            EditorUtility.SetDirty(slider);
        }
        
        EditorUtility.SetDirty(canvas);
    }
    
    static void ConfigureHammerGameManager()
    {
        var manager = GameObject.Find("UIHammerGameManager");
        if (manager == null)
        {
            Debug.LogError("UIHammerGameManager not found!");
            return;
        }
        
        var script = manager.GetComponent<UIHammerStrengthGame>();
        if (script == null)
        {
            Debug.LogError("UIHammerStrengthGame component not found!");
            return;
        }
        
        // Assign UI references
        script.hammerImage = GameObject.Find("HammerGameCanvas/HammerImage")?.GetComponent<Image>();
        script.hammerRect = script.hammerImage?.GetComponent<RectTransform>();
        script.weightImage = GameObject.Find("HammerGameCanvas/WeightImage")?.GetComponent<Image>();
        script.weightRect = script.weightImage?.GetComponent<RectTransform>();
        script.standImage = GameObject.Find("HammerGameCanvas/StandImage")?.GetComponent<Image>();
        script.chargeBar = GameObject.Find("HammerGameCanvas/ChargeSlider")?.GetComponent<Slider>();
        script.gameBackground = GameObject.Find("HammerGameCanvas/GameBackground")?.GetComponent<Image>();
        
        // Try to find TMP text components
        var timerObj = GameObject.Find("HammerGameCanvas/TimerText");
        if (timerObj != null)
        {
            script.timerText = timerObj.GetComponent<TextMeshProUGUI>();
        }
        
        var instrObj = GameObject.Find("HammerGameCanvas/InstructionText");
        if (instrObj != null)
        {
            script.instructionText = instrObj.GetComponent<TextMeshProUGUI>();
        }
        
        // Load sprites
        script.hammerUp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/HammerGame/Poele_0.png");
        script.hammerHit = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/HammerGame/Poele_1.png");
        script.standSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/HammerGame/HammerStand.png");
        
        // Configure game settings for high striker
        script.chargeDuration = 3f;
        script.baseChargePerClick = 0.04f;
        script.chargePerClick = 0.04f;
        script.maxCharge = 1f;
        script.victoryThreshold = 0.85f;
        
        // Weight movement settings (vertical tower)
        script.weightMinY = -220f;
        script.weightMaxY = 220f;
        script.weightMoveSpeed = 8f;
        
        // Hammer animation
        script.hammerBackX = -50f;
        script.hammerImpactX = -150f;
        script.growScale = 1.3f;
        script.shrinkScale = 0.8f;
        script.scaleAnimSpeed = 10f;
        
        // Adaptive help
        script.chargeAfterTwoFails = 0.05f;
        script.extraChargePerFail = 0.015f;
        
        EditorUtility.SetDirty(script);
        EditorUtility.SetDirty(manager);
        
        Debug.Log("UIHammerStrengthGame configured successfully!");
    }
}
