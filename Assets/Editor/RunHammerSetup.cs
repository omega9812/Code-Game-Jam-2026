using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class RunHammerSetup : MonoBehaviour
{
    [MenuItem("Tools/Run Hammer Game Setup Now")]
    public static void Execute()
    {
        // 1. Remove duplicate UIHammerGameManager (the one without components)
        RemoveDuplicateManagers();
        
        // 2. Setup the HammerGameCanvas UI properly
        SetupHammerGameUI();
        
        // 3. Configure the UIHammerStrengthGame component
        ConfigureHammerGameManager();
        
        // 4. Mark scene dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("Hammer Game setup complete!");
    }
    
    static void RemoveDuplicateManagers()
    {
        var allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject managerToKeep = null;
        
        foreach (var obj in allObjects)
        {
            if (obj.name == "UIHammerGameManager")
            {
                var script = obj.GetComponent<UIHammerStrengthGame>();
                if (script != null)
                {
                    managerToKeep = obj;
                    Debug.Log("Keeping UIHammerGameManager with script");
                }
                else
                {
                    // This is the duplicate without the script - delete it
                    Debug.Log("Removing duplicate UIHammerGameManager (no script)");
                    Undo.DestroyObjectImmediate(obj);
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
        
        // Configure canvas scaler for consistent sizing
        var scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            EditorUtility.SetDirty(scaler);
        }
        
        // Keep canvas active for now to see changes
        canvas.SetActive(true);
        
        // Setup GameBackground - full screen dark overlay
        var background = GameObject.Find("HammerGameCanvas/GameBackground");
        if (background != null)
        {
            var bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            background.transform.SetAsFirstSibling();
            
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
            standRect.pivot = new Vector2(0.5f, 0.5f);
            standRect.anchoredPosition = new Vector2(0, 0);
            standRect.sizeDelta = new Vector2(150, 500);
            
            var standImage = stand.GetComponent<Image>();
            if (standImage != null)
            {
                standImage.preserveAspect = true;
                // Load the stand sprite
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/HammerGame/HammerStand.png");
                if (sprite != null)
                    standImage.sprite = sprite;
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
            weightRect.anchoredPosition = new Vector2(0, -200); // Start at bottom of tower
            weightRect.sizeDelta = new Vector2(50, 50);
            
            var weightImage = weight.GetComponent<Image>();
            if (weightImage != null)
            {
                weightImage.color = new Color(1f, 0.2f, 0.2f, 1f); // Red ball
            }
            EditorUtility.SetDirty(weight);
        }
        
        // Setup HammerImage - the hammer/mallet at the bottom left
        var hammer = GameObject.Find("HammerGameCanvas/HammerImage");
        if (hammer != null)
        {
            var hammerRect = hammer.GetComponent<RectTransform>();
            hammerRect.anchorMin = new Vector2(0.5f, 0.5f);
            hammerRect.anchorMax = new Vector2(0.5f, 0.5f);
            hammerRect.pivot = new Vector2(0.5f, 0.5f);
            hammerRect.anchoredPosition = new Vector2(-200, -200);
            hammerRect.sizeDelta = new Vector2(180, 180);
            
            var hammerImage = hammer.GetComponent<Image>();
            if (hammerImage != null)
            {
                hammerImage.preserveAspect = true;
                // Load the hammer sprite
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/HammerGame/Poele_0.png");
                if (sprite != null)
                    hammerImage.sprite = sprite;
            }
            EditorUtility.SetDirty(hammer);
        }
        
        // Remove duplicate elements
        RemoveDuplicateChildren(canvas, "TimerText");
        RemoveDuplicateChildren(canvas, "ChargeSlider");
        
        // Setup TimerText - at the top
        var timer = GameObject.Find("HammerGameCanvas/TimerText");
        if (timer != null)
        {
            var timerRect = timer.GetComponent<RectTransform>();
            timerRect.anchorMin = new Vector2(0.5f, 1f);
            timerRect.anchorMax = new Vector2(0.5f, 1f);
            timerRect.pivot = new Vector2(0.5f, 1f);
            timerRect.anchoredPosition = new Vector2(0, -30);
            timerRect.sizeDelta = new Vector2(300, 80);
            
            // Add TMP component if missing
            var tmpText = timer.GetComponent<TextMeshProUGUI>();
            if (tmpText == null)
            {
                // Remove old Text component
                var oldText = timer.GetComponent<Text>();
                if (oldText != null)
                    Undo.DestroyObjectImmediate(oldText);
                
                tmpText = timer.AddComponent<TextMeshProUGUI>();
            }
            
            if (tmpText != null)
            {
                tmpText.text = "3.0s";
                tmpText.fontSize = 56;
                tmpText.alignment = TextAlignmentOptions.Center;
                tmpText.color = Color.white;
                tmpText.fontStyle = FontStyles.Bold;
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
            instrRect.anchoredPosition = new Vector2(0, -110);
            instrRect.sizeDelta = new Vector2(700, 60);
            
            // Add TMP component if missing
            var tmpText = instruction.GetComponent<TextMeshProUGUI>();
            if (tmpText == null)
            {
                var oldText = instruction.GetComponent<Text>();
                if (oldText != null)
                    Undo.DestroyObjectImmediate(oldText);
                
                tmpText = instruction.AddComponent<TextMeshProUGUI>();
            }
            
            if (tmpText != null)
            {
                tmpText.text = "Click to start! Spam click to charge!";
                tmpText.fontSize = 36;
                tmpText.alignment = TextAlignmentOptions.Center;
                tmpText.color = new Color(1f, 0.9f, 0.3f, 1f); // Yellow
            }
            EditorUtility.SetDirty(instruction);
        }
        
        // Setup ChargeSlider - vertical bar on the right side
        var slider = GameObject.Find("HammerGameCanvas/ChargeSlider");
        if (slider != null)
        {
            var sliderRect = slider.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(1f, 0.5f);
            sliderRect.anchorMax = new Vector2(1f, 0.5f);
            sliderRect.pivot = new Vector2(1f, 0.5f);
            sliderRect.anchoredPosition = new Vector2(-50, 0);
            sliderRect.sizeDelta = new Vector2(40, 400);
            
            var sliderComp = slider.GetComponent<Slider>();
            if (sliderComp != null)
            {
                sliderComp.minValue = 0;
                sliderComp.maxValue = 1;
                sliderComp.value = 0;
                sliderComp.interactable = false;
                sliderComp.direction = Slider.Direction.BottomToTop;
            }
            EditorUtility.SetDirty(slider);
        }
        
        EditorUtility.SetDirty(canvas);
        Debug.Log("UI setup complete!");
    }
    
    static void RemoveDuplicateChildren(GameObject parent, string childName)
    {
        var children = parent.GetComponentsInChildren<Transform>(true);
        int count = 0;
        foreach (var child in children)
        {
            if (child.name == childName)
            {
                count++;
                if (count > 1)
                {
                    Debug.Log($"Removing duplicate {childName}");
                    Undo.DestroyObjectImmediate(child.gameObject);
                }
            }
        }
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
        
        // Assign canvas reference
        script.gameCanvas = GameObject.Find("HammerGameCanvas");
        
        // Assign UI references
        script.hammerImage = GameObject.Find("HammerGameCanvas/HammerImage")?.GetComponent<Image>();
        script.hammerRect = script.hammerImage?.GetComponent<RectTransform>();
        script.weightImage = GameObject.Find("HammerGameCanvas/WeightImage")?.GetComponent<Image>();
        script.weightRect = script.weightImage?.GetComponent<RectTransform>();
        script.standImage = GameObject.Find("HammerGameCanvas/StandImage")?.GetComponent<Image>();
        script.chargeBar = GameObject.Find("HammerGameCanvas/ChargeSlider")?.GetComponent<Slider>();
        script.gameBackground = GameObject.Find("HammerGameCanvas/GameBackground")?.GetComponent<Image>();
        
        // Assign TMP text components
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
        script.baseChargePerClick = 0.035f;
        script.chargePerClick = 0.035f;
        script.maxCharge = 1f;
        script.victoryThreshold = 0.85f;
        
        // Weight movement settings (vertical tower)
        script.weightMinY = -200f;
        script.weightMaxY = 200f;
        script.weightMoveSpeed = 8f;
        
        // Hammer animation
        script.hammerBackX = -80f;
        script.hammerImpactX = -200f;
        script.growScale = 1.3f;
        script.shrinkScale = 0.8f;
        script.scaleAnimSpeed = 12f;
        
        // Adaptive help
        script.chargeAfterTwoFails = 0.045f;
        script.extraChargePerFail = 0.01f;
        
        EditorUtility.SetDirty(script);
        EditorUtility.SetDirty(manager);
        
        Debug.Log("UIHammerStrengthGame configured successfully!");
    }
}
