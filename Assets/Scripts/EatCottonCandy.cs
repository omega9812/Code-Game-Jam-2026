using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AC; // Adventure Creator
using System.Collections;
using System.Collections.Generic;

public class EatCottonCandy : MonoBehaviour
{
    [Header("Cotton Candy Image")]
    public GameObject cottonCandyImageObject;

    [Header("Sprites (ordre complet - ... - dernier)")]
    public Sprite[] stages;

    [Header("Nombre de clics pour passer au sprite suivant")]
    public int clicksPerStage = 5;

    [Header("Dernier sprite: clics supplmentaires avant fin")]
    public int finishClicksNeeded = 5;

    [Header("Effet de clic (recul)")]
    public float clickScale = 0.9f;
    public float clickAnimSpeed = 25f;
    public float clickHoldTime = 0.05f;

    [Header("Fin du mini-jeu")]
    public ParticleSystem smokePS;
    public float disappearDuration = 0.25f;
    public float waitAfterSmoke = 0.2f;

    [Header("Sons")]
    public AudioClip crunchClip;
    public AudioClip pouffClip;
    [Range(0f, 1f)] public float crunchVolume = 0.8f;
    [Range(0f, 1f)] public float pouffVolume = 1f;

    [Header("Timing son")]
    public float crunchBeforeSpriteDelay = 0.04f;

    [Header("Canvas Control")]
    public Canvas canvas;

    // References
    private Image uiImage;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private AudioSource audioSrc;

    // Cache raycast helpers
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

    // State
    private int clickCount = 0;
    private int stageIndex = 0;
    private int finishClicks = 0;
    private Vector3 originalScale;
    private bool canClick = true;
    private bool isFinishing = false;

    void Awake()
    {
    }

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        if (audioSrc == null)
        {
            audioSrc = gameObject.AddComponent<AudioSource>();
        }

        if (cottonCandyImageObject == null)
        {
            Debug.LogError("Cotton Candy Image Object not assigned!");
            enabled = false;
            return;
        }

        uiImage = cottonCandyImageObject.GetComponent<Image>();
        rectTransform = cottonCandyImageObject.GetComponent<RectTransform>();

        if (uiImage == null)
        {
            Debug.LogError("Il manque un Image sur l'objet cottonCandyImageObject.");
            enabled = false;
            return;
        }

        // Make sure the image can receive raycasts
        uiImage.raycastTarget = true;

        originalScale = rectTransform.localScale;

        if (audioSrc != null) audioSrc.playOnAwake = false;
        if (smokePS != null) smokePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Setup CanvasGroup (keep as fade only)
        if (canvas != null)
        {
            canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvas.gameObject.SetActive(false);
        }

        // Ensure EventSystem exists + cache it
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogWarning("No EventSystem found in scene! UI clicks won't work reliably. Creating one...");
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
        }

        pointerEventData = new PointerEventData(eventSystem);

        // Optional: prove we are targeting the right Image
        Debug.Log($"EatCottonCandy ready. Target Image='{cottonCandyImageObject.name}', raycastTarget={uiImage.raycastTarget}");
    }

    void Update()
    {
        // Only accept clicks while minigame is active
        if (!canClick || isFinishing) return;
        if (canvas == null || !canvas.gameObject.activeInHierarchy) return;

        if (Input.GetMouseButtonDown(0))
        {
            // Raycast UI under cursor and see if we hit the candy object (or its children)
            if (IsPointerOverCottonCandyUI(Input.mousePosition))
            {
                HandleClick();
            }
        }
    }

    // New helper: robust UI hit test
    private bool IsPointerOverCottonCandyUI(Vector2 screenPos)
    {
        if (eventSystem == null || uiImage == null) return false;

        pointerEventData.position = screenPos;
        raycastResults.Clear();
        eventSystem.RaycastAll(pointerEventData, raycastResults);

        if (raycastResults.Count == 0) return false;

        Transform target = cottonCandyImageObject.transform;
        for (int i = 0; i < raycastResults.Count; i++)
        {
            var hitGO = raycastResults[i].gameObject;
            if (hitGO == null) continue;

            // accept direct hit or any child hit (in case the Image has nested graphics)
            if (hitGO.transform == target || hitGO.transform.IsChildOf(target))
            {
                return true;
            }
        }
        return false;
    }

    public void StartMinigame()
    {
        Debug.Log("StartMinigame called");
        
        // Reset state
        clickCount = 0;
        stageIndex = 0;
        finishClicks = 0;
        canClick = true;
        isFinishing = false;

        // Set first sprite
        if (stages != null && stages.Length > 0 && stages[0] != null)
        {
            uiImage.sprite = stages[0];
        }
        
        // Reset scale
        rectTransform.localScale = originalScale;

        // Show Canvas
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
            StartCoroutine(FadeInCanvas());
            Debug.Log("Canvas activated and fading in");
        }

        // Pause AC gameplay - disable player input
        if (KickStarter.stateHandler != null)
        {
            KickStarter.stateHandler.SetACState(false);
        }
    }

    IEnumerator FadeInCanvas()
    {
        float fadeT = 0f;
        
        while (fadeT < 1f)
        {
            fadeT += Time.deltaTime / 0.3f;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeT);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        Debug.Log("Canvas fully visible");
    }

    void HandleClick()
    {
        Debug.Log("HandleClick called!");
        
        if (!canClick || isFinishing)
        {
            Debug.Log("Click ignored - canClick: " + canClick + ", isFinishing: " + isFinishing);
            return;
        }

        if (stages == null || stages.Length == 0)
        {
            Debug.LogWarning("No stages defined!");
            return;
        }

        Debug.Log("Processing click - Stage: " + stageIndex + ", ClickCount: " + clickCount);

        bool isLastStage = stageIndex == stages.Length - 1;
        if (isLastStage)
        {
            finishClicks++;
            Debug.Log("Last stage - finish clicks: " + finishClicks + "/" + finishClicksNeeded);
            StartCoroutine(ClickEffect(true));
            if (finishClicks >= finishClicksNeeded) StartCoroutine(FinishCandy());
            return;
        }

        clickCount++;
        if (clickCount >= clicksPerStage)
        {
            clickCount = 0;
            stageIndex++;
            Debug.Log("Moving to stage: " + stageIndex);
            if (stageIndex < stages.Length && stages[stageIndex] != null)
            {
                uiImage.sprite = stages[stageIndex];
            }
        }
        StartCoroutine(ClickEffect(true));
    }

    IEnumerator ClickEffect(bool playCrunch)
    {
        canClick = false;

        if (playCrunch && crunchClip != null)
        {
            yield return new WaitForSeconds(crunchBeforeSpriteDelay);
            PlayOneShot(crunchClip, crunchVolume);
        }

        rectTransform.localScale = originalScale * clickScale;
        yield return new WaitForSeconds(clickHoldTime);

        float t = 0f;
        Vector3 from = originalScale * clickScale;
        Vector3 to = originalScale;
        while (t < 1f)
        {
            t += Time.deltaTime * clickAnimSpeed;
            rectTransform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        rectTransform.localScale = originalScale;
        canClick = true;
    }

    IEnumerator FinishCandy()
    {
        if (isFinishing) yield break;
        isFinishing = true;
        canClick = false;


        PlayOneShot(pouffClip, pouffVolume);

        if (smokePS != null)
        {
            smokePS.transform.position = cottonCandyImageObject.transform.position;
            smokePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            smokePS.Play();
        }

        // Disappear image
        Vector3 startScale = rectTransform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, disappearDuration);
            rectTransform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        rectTransform.localScale = Vector3.zero;

        yield return new WaitForSeconds(waitAfterSmoke);

        // Hide Canvas with fade
        if (canvas != null)
        {
            if (canvasGroup != null)
            {
                float fadeT = 0f;
                while (fadeT < 1f)
                {
                    fadeT += Time.deltaTime / 0.5f;
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeT);
                    yield return null;
                }
                canvasGroup.alpha = 0f;
            }
            canvas.gameObject.SetActive(false);
        }

        // Resume AC gameplay - enable player input
        if (KickStarter.stateHandler != null)
        {
            KickStarter.stateHandler.SetACState (true);
        }
    }

    void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null) return;
        if (audioSrc == null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, volume);
        }
        else
        {
            audioSrc.PlayOneShot(clip, volume);
        }
    }
}
