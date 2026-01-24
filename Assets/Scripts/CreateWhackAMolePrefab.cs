using UnityEngine;
using UnityEditor;
using TMPro;

#if UNITY_EDITOR
public class CreateWhackAMolePrefab : MonoBehaviour
{
    [MenuItem("Game/Create Whack-A-Mole Prefab")]
    static void CreatePrefab()
    {
        // Create the main game object
        GameObject whackAMoleGame = new GameObject("WhackAMoleMinigame");
        whackAMoleGame.transform.position = Vector3.zero;
        
        // Add the game controller
        WhackAMoleGameIntegrated gameController = whackAMoleGame.AddComponent<WhackAMoleGameIntegrated>();
        
        // Create canvas for UI
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(whackAMoleGame.transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Create background panel
        GameObject backgroundPanel = new GameObject("Background");
        backgroundPanel.transform.SetParent(canvasObj.transform);
        UnityEngine.UI.Image bgImage = backgroundPanel.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);
        RectTransform bgRect = backgroundPanel.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Create score text
        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.SetParent(canvasObj.transform);
        TextMeshProUGUI scoreText = scoreTextObj.AddComponent<TextMeshProUGUI>();
        scoreText.text = "Score: 0";
        scoreText.fontSize = 36;
        scoreText.alignment = TextAlignmentOptions.TopLeft;
        RectTransform scoreRect = scoreTextObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 1);
        scoreRect.anchorMax = new Vector2(0, 1);
        scoreRect.pivot = new Vector2(0, 1);
        scoreRect.sizeDelta = new Vector2(300, 50);
        scoreRect.anchoredPosition = new Vector2(20, -20);
        
        // Create timer text
        GameObject timerTextObj = new GameObject("TimerText");
        timerTextObj.transform.SetParent(canvasObj.transform);
        TextMeshProUGUI timerText = timerTextObj.AddComponent<TextMeshProUGUI>();
        timerText.text = "30.0s";
        timerText.fontSize = 36;
        timerText.alignment = TextAlignmentOptions.TopRight;
        RectTransform timerRect = timerTextObj.GetComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(1, 1);
        timerRect.anchorMax = new Vector2(1, 1);
        timerRect.pivot = new Vector2(1, 1);
        timerRect.sizeDelta = new Vector2(300, 50);
        timerRect.anchoredPosition = new Vector2(-20, -20);
        
        // Create penalty text
        GameObject penaltyTextObj = new GameObject("PenaltyText");
        penaltyTextObj.transform.SetParent(canvasObj.transform);
        TextMeshProUGUI penaltyText = penaltyTextObj.AddComponent<TextMeshProUGUI>();
        penaltyText.text = "-0.2s";
        penaltyText.fontSize = 36;
        penaltyText.color = Color.red;
        penaltyText.alignment = TextAlignmentOptions.Center;
        penaltyText.gameObject.SetActive(false);
        RectTransform penaltyRect = penaltyTextObj.GetComponent<RectTransform>();
        penaltyRect.anchorMin = new Vector2(0.5f, 0.5f);
        penaltyRect.anchorMax = new Vector2(0.5f, 0.5f);
        penaltyRect.pivot = new Vector2(0.5f, 0.5f);
        penaltyRect.sizeDelta = new Vector2(200, 50);
        penaltyRect.anchoredPosition = Vector2.zero;
        
        // Create end text
        GameObject endTextObj = new GameObject("EndText");
        endTextObj.transform.SetParent(canvasObj.transform);
        TextMeshProUGUI endText = endTextObj.AddComponent<TextMeshProUGUI>();
        endText.text = "GAGNÉ !";
        endText.fontSize = 72;
        endText.color = Color.yellow;
        endText.alignment = TextAlignmentOptions.Center;
        endText.gameObject.SetActive(false);
        RectTransform endRect = endTextObj.GetComponent<RectTransform>();
        endRect.anchorMin = new Vector2(0.5f, 0.5f);
        endRect.anchorMax = new Vector2(0.5f, 0.5f);
        endRect.pivot = new Vector2(0.5f, 0.5f);
        endRect.sizeDelta = new Vector2(400, 100);
        endRect.anchoredPosition = Vector2.zero;
        
        // Create holes container
        GameObject holesContainer = new GameObject("Holes");
        holesContainer.transform.SetParent(whackAMoleGame.transform);
        
        // Create 9 holes in a 3x3 grid
        Transform[] holes = new Transform[9];
        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;
            
            GameObject hole = new GameObject("Hole_" + i);
            hole.transform.SetParent(holesContainer.transform);
            
            // Position holes in a grid
            float x = (col - 1) * 2.5f;
            float y = (row - 1) * 2.0f;
            hole.transform.localPosition = new Vector3(x, y, 0);
            
            // Create visual representation of the hole
            GameObject holeVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            holeVisual.transform.SetParent(hole.transform);
            holeVisual.transform.localScale = new Vector3(1f, 0.1f, 1f);
            holeVisual.transform.localPosition = new Vector3(0, -0.05f, 0);
            holeVisual.GetComponent<Renderer>().material.color = new Color(0.3f, 0.2f, 0.1f);
            
            holes[i] = hole.transform;
        }
        
        // Create audio source
        GameObject audioObj = new GameObject("AudioSource");
        audioObj.transform.SetParent(whackAMoleGame.transform);
        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        
        // Create hammer cursor
        GameObject hammerCursor = new GameObject("HammerCursor");
        hammerCursor.transform.SetParent(whackAMoleGame.transform);
        HammerCursor hammerCursorScript = hammerCursor.AddComponent<HammerCursor>();
        SpriteRenderer hammerRenderer = hammerCursor.AddComponent<SpriteRenderer>();
        
        // Set up references in the game controller
        gameController.holes = holes;
        gameController.scoreText = scoreText;
        gameController.timerText = timerText;
        gameController.penaltyText = penaltyText;
        gameController.endText = endText;
        gameController.audioSource = audioSource;
        
        // Create the prefab
        #if UNITY_EDITOR
        string prefabPath = "Assets/Prefabs/WhackAMoleMinigame.prefab";
        
        // Ensure directory exists
        string directory = System.IO.Path.GetDirectoryName(prefabPath);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }
        
        // Create the prefab
        PrefabUtility.SaveAsPrefabAsset(whackAMoleGame, prefabPath);
        Debug.Log("Whack-A-Mole prefab created at: " + prefabPath);
        
        // Clean up the scene object
        Object.DestroyImmediate(whackAMoleGame);
        #endif
    }
}
#endif