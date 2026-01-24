using UnityEngine;
using AC;
using System.Collections;
using UnityEngine.UI;

public class WhackAMoleMinigameManager : MonoBehaviour
{
    [Header("Minigame Setup")]
    // - remove prefab field
    // public GameObject whackAMolePrefab;
    // + reference to already-in-scene minigame
    public GameObject whackAMoleMinigame;
    public Transform minigameSpawnPoint;

    [Header("Clown Character")]
    public string clownObjectName = "Clown";
    public GameObject clownObject;

    [Header("Minigame Overlay")]
    [Range(0f, 1f)] public float dimAlpha = 1f;
    public int minigameSortingOrder = 2000;

    private GameObject overlayRoot;
    private Transform minigameContainer;

    private bool hasSpokenToClown = false;
    // - currentMinigame now just tracks if active (same GameObject ref)
    private bool isMinigameActive = false;

    private Coroutine launchRoutine;

    void Start()
    {
        EventManager.OnEndConversation += OnEndConversation;

        // Hide the minigame at start
        if (whackAMoleMinigame != null)
        {
            whackAMoleMinigame.SetActive(false);
        }
    }

    void OnDestroy()
    {
        EventManager.OnEndConversation -= OnEndConversation;
    }

    private void OnEndConversation(Conversation conversation)
    {
        // Resolve clown reference if not assigned
        if (clownObject == null && !string.IsNullOrEmpty(clownObjectName))
        {
            clownObject = GameObject.Find(clownObjectName);
        }

        if (clownObject == null)
        {
            Debug.LogError($"{nameof(WhackAMoleMinigameManager)}: 'clownObject' not assigned and '{clownObjectName}' not found.", this);
            return;
        }

        if (KickStarter.player != null)
        {
            // Check if the clown is close to the player
            float distance = Vector3.Distance(clownObject.transform.position, KickStarter.player.transform.position);
            if (distance < 3f) // Assuming 3 units is close enough
            {
                if (!hasSpokenToClown)
                {
                    // First time talking to the clown
                    hasSpokenToClown = true;
                    Debug.Log("First conversation with clown completed");
                }
                else
                {
                    // Second time talking to the clown
                    Debug.Log("Second conversation with clown completed, launching minigame");
                    StartCoroutine(LaunchMinigame());
                }
            }
        }
    }

    // + Adventure Creator "Object: Send message" entrypoint
    // Call this method name ("StartGame") from an ActionList.
    public void StartGame()
    {
        if (isMinigameActive || launchRoutine != null)
        {
            Debug.Log($"{nameof(WhackAMoleMinigameManager)}: StartGame ignored (already running).", this);
            return;
        }

        if (whackAMoleMinigame == null)
        {
            Debug.LogError($"{nameof(WhackAMoleMinigameManager)}: 'whackAMoleMinigame' not assigned!", this);
            return;
        }

        hasSpokenToClown = true;
        launchRoutine = StartCoroutine(LaunchMinigame_Wrapped());
    }

    private IEnumerator LaunchMinigame_Wrapped()
    {
        yield return StartCoroutine(LaunchMinigame());
        launchRoutine = null;
    }

    public IEnumerator LaunchMinigame()
    {
        yield return new WaitForSeconds(0.5f);

        if (whackAMoleMinigame == null)
        {
            Debug.LogError("whackAMoleMinigame reference is missing!");
            yield break;
        }

        // Build overlay UI
        EnsureOverlay();

        // Disable player movement
        if (KickStarter.player != null)
        {
            KickStarter.player.enabled = false;
        }

        // Parent the minigame under the overlay container
        whackAMoleMinigame.transform.SetParent(minigameContainer, false);

        // Activate the minigame
        whackAMoleMinigame.SetActive(true);
        isMinigameActive = true;

        // Convert any SpriteRenderers to UI Images
        ForceMinigameSorting(whackAMoleMinigame);

        // Set up the minigame controller
        SetupMinigame(whackAMoleMinigame);
    }

    // + create an overlay Canvas with a dark background and a container for the minigame
    private void EnsureOverlay()
    {
        if (overlayRoot != null) return;

        overlayRoot = new GameObject("WhackAMole_Overlay");
        DontDestroyOnLoad(overlayRoot);

        var canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(overlayRoot.transform, false);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = minigameSortingOrder - 1;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();

        // Dark background - fully opaque
        var dimGO = new GameObject("DimBackground");
        dimGO.transform.SetParent(canvasGO.transform, false);
        var dimImg = dimGO.AddComponent<Image>();
        dimImg.color = new Color(0f, 0f, 0f, dimAlpha);
        var dimRT = dimGO.GetComponent<RectTransform>();
        dimRT.anchorMin = Vector2.zero;
        dimRT.anchorMax = Vector2.one;
        dimRT.offsetMin = Vector2.zero;
        dimRT.offsetMax = Vector2.zero;

        // Container for minigame UI objects - increased size to fit all holes
        var containerGO = new GameObject("MinigameRoot");
        containerGO.transform.SetParent(canvasGO.transform, false);
        minigameContainer = containerGO.transform;

        // Center the container with larger dimensions
        var containerRT = containerGO.AddComponent<RectTransform>();
        containerRT.anchorMin = new Vector2(0.5f, 0.5f);
        containerRT.anchorMax = new Vector2(0.5f, 0.5f);
        containerRT.pivot = new Vector2(0.5f, 0.5f);
        containerRT.anchoredPosition = Vector2.zero;
        containerRT.sizeDelta = new Vector2(1200, 900); // Increased from 800x600
    }

    // + force all SpriteRenderers in the minigame hierarchy to render above world
    private void ForceMinigameSorting(GameObject root)
    {
        if (root == null) return;

        // Convert world-space sprites to UI Images
        var renderers = root.GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            var sr = renderers[i];
            var go = sr.gameObject;
            
            // Get the sprite before destroying
            Sprite sprite = sr.sprite;
            Color color = sr.color;
            
            // Remove SpriteRenderer
            Destroy(sr);
            
            // Add RectTransform if not present
            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt == null)
            {
                rt = go.AddComponent<RectTransform>();
            }
            
            // Add Image component
            Image img = go.AddComponent<Image>();
            img.sprite = sprite;
            img.color = color;
            img.raycastTarget = true;
            
            // Size the image appropriately (100x100 default)
            rt.sizeDelta = new Vector2(100, 100);
        }
    }

    // - remove CreateWhackAMolePrefab() entirely

    private void SetupMinigame(GameObject minigameObject)
    {
        if (minigameObject == null) return;

        WhackAMoleGame gameController = minigameObject.GetComponent<WhackAMoleGame>();
        if (gameController == null) return;

        // Add end handler if not already present
        MinigameEndHandler endHandler = minigameObject.GetComponent<MinigameEndHandler>();
        if (endHandler == null)
        {
            endHandler = minigameObject.AddComponent<MinigameEndHandler>();
        }
        endHandler.manager = this;
    }

    public void EndMinigame(bool won)
    {
        // Re-enable player movement
        if (KickStarter.player != null)
        {
            KickStarter.player.enabled = true;
        }

        // Deactivate the minigame (don't destroy, just hide)
        if (whackAMoleMinigame != null)
        {
            whackAMoleMinigame.SetActive(false);
            // Unparent from overlay
            whackAMoleMinigame.transform.SetParent(null);
        }

        isMinigameActive = false;

        // Destroy overlay
        if (overlayRoot != null)
        {
            Destroy(overlayRoot);
            overlayRoot = null;
            minigameContainer = null;
        }

        if (launchRoutine != null)
        {
            StopCoroutine(launchRoutine);
            launchRoutine = null;
        }

        if (won)
        {
            Debug.Log("Player won the minigame!");
        }
        else
        {
            Debug.Log("Player lost the minigame!");
        }
    }
}

// Helper class to handle the end of the minigame
public class MinigameEndHandler : MonoBehaviour
{
    public WhackAMoleMinigameManager manager;
    private WhackAMoleGame gameController;
    private bool gameEnded = false;

    void Start()
    {
        gameController = GetComponent<WhackAMoleGame>();
    }

    // Create public properties to access the private fields in WhackAMoleGame
    private float TimeLeft
    {
        get
        {
            // Use reflection to access the private field
            System.Reflection.FieldInfo field = typeof(WhackAMoleGame).GetField("timeLeft",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null && gameController != null)
            {
                return (float)field.GetValue(gameController);
            }
            return 0f;
        }
    }

    private int Score
    {
        get
        {
            // Use reflection to access the private field
            System.Reflection.FieldInfo field = typeof(WhackAMoleGame).GetField("score",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null && gameController != null)
            {
                return (int)field.GetValue(gameController);
            }
            return 0;
        }
    }

    void Update()
    {
        if (gameController != null && !gameEnded)
        {
            // Check if the game has ended
            if (TimeLeft <= 0f)
            {
                gameEnded = true;
                bool won = Score >= gameController.scoreToWin;
                StartCoroutine(EndAfterDelay(won));
            }
        }
    }

    IEnumerator EndAfterDelay(bool won)
    {
        yield return new WaitForSeconds(2f);

        if (manager != null)
        {
            manager.EndMinigame(won);
        }
    }
}