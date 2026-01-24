using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIHammerStrengthGame : MonoBehaviour
{
    [Header("UI References")]
    public Image hammerImage;
    public RectTransform hammerRect;
    public Image weightImage;
    public RectTransform weightRect;
    public Image standImage;
    public Slider chargeBar;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI instructionText;
    public Image gameBackground;
    public GameObject gameCanvas;

    [Header("Sprites")]
    public Sprite hammerUp;
    public Sprite hammerHit;
    public Sprite weightSprite;
    public Sprite standSprite;

    [Header("Charge Settings")]
    public float chargeDuration = 3f;
    public float chargePerClick = 0.04f;
    public float maxCharge = 1f;

    [Header("Weight Movement (High Striker Tower)")]
    public float weightMoveSpeed = 8f;
    public float weightMinY = -220f;
    public float weightMaxY = 220f;

    [Header("Hammer Animation")]
    public float growScale = 1.3f;
    public float shrinkScale = 0.8f;
    public float scaleAnimSpeed = 10f;

    [Header("Hammer Movement")]
    public float hammerBackX = -50f;
    public float hammerImpactX = -150f;

    [Header("Shake Effect")]
    public float shakeDuration = 0.15f;
    public float shakeStrength = 15f;

    [Header("Audio")]
    public AudioClip clickClip;
    public AudioClip hitClip;
    public AudioClip dingClip;
    public AudioClip victoryMusic;
    public AudioClip failSound;

    public float clickVolume = 0.5f;
    public float hitVolume = 1f;
    public float dingVolume = 1f;
    public float victoryVolume = 1f;

    [Header("Victory")]
    public float victoryThreshold = 0.85f;
    public float delayAfterResult = 2f;

    [Header("Adaptive Help (Decreasing Difficulty)")]
    public float baseChargePerClick = 0.04f;
    public float chargeAfterTwoFails = 0.05f;
    public float extraChargePerFail = 0.015f;

    // Private state
    private int failCount = 0;
    private int attemptNumber = 0;
    private AudioSource audioSrc;
    private Vector2 hammerStartPos;
    private Vector3 hammerStartScale;

    private float charge = 0f;
    private int clickCount = 0;
    private bool isCharging = false;
    private bool roundRunning = false;
    private bool gameActive = false;
    private bool waitingForStart = false;

    // Callback for Adventure Creator
    public System.Action onGameComplete;
    public System.Action onGameWin;
    public System.Action onGameFail;

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
        if (audioSrc == null)
        {
            audioSrc = gameObject.AddComponent<AudioSource>();
        }

        // Find canvas if not assigned
        if (gameCanvas == null)
        {
            gameCanvas = GameObject.Find("HammerGameCanvas");
        }
    }

    void Start()
    {
        SetupUIReferences();
        
        if (hammerRect != null)
        {
            hammerStartPos = hammerRect.anchoredPosition;
            hammerStartScale = hammerRect.localScale;
        }

        chargePerClick = baseChargePerClick;
        
        // Start with game hidden
        if (gameCanvas != null)
        {
            gameCanvas.SetActive(false);
        }
    }

    void SetupUIReferences()
    {
        if (hammerImage == null)
            hammerImage = GameObject.Find("HammerGameCanvas/HammerImage")?.GetComponent<Image>();
        if (hammerRect == null && hammerImage != null)
            hammerRect = hammerImage.GetComponent<RectTransform>();
        
        if (weightImage == null)
            weightImage = GameObject.Find("HammerGameCanvas/WeightImage")?.GetComponent<Image>();
        if (weightRect == null && weightImage != null)
            weightRect = weightImage.GetComponent<RectTransform>();
        
        if (standImage == null)
            standImage = GameObject.Find("HammerGameCanvas/StandImage")?.GetComponent<Image>();
        
        if (chargeBar == null)
            chargeBar = GameObject.Find("HammerGameCanvas/ChargeSlider")?.GetComponent<Slider>();
        
        if (timerText == null)
            timerText = GameObject.Find("HammerGameCanvas/TimerText")?.GetComponent<TextMeshProUGUI>();
        
        if (instructionText == null)
            instructionText = GameObject.Find("HammerGameCanvas/InstructionText")?.GetComponent<TextMeshProUGUI>();
        
        if (gameBackground == null)
            gameBackground = GameObject.Find("HammerGameCanvas/GameBackground")?.GetComponent<Image>();
    }

    /// <summary>
    /// Call this method to start the hammer game (from Adventure Creator or other scripts)
    /// </summary>
    public void StartGame()
    {
        if (gameCanvas != null)
        {
            gameCanvas.SetActive(true);
        }
        
        gameActive = true;
        attemptNumber = 0;
        failCount = 0;
        chargePerClick = baseChargePerClick;
        
        ResetRound();
        waitingForStart = true;
        
        if (instructionText != null)
        {
            instructionText.text = "Click to pour commencer! Spam des clicks!";
        }
    }

    /// <summary>
    /// Call this to end the game and hide the UI
    /// </summary>
    public void EndGame()
    {
        gameActive = false;
        waitingForStart = false;
        roundRunning = false;
        isCharging = false;
        
        StopAllCoroutines();
        
        if (gameCanvas != null)
        {
            gameCanvas.SetActive(false);
        }
        
        onGameComplete?.Invoke();
    }

    void Update()
    {
        if (!gameActive) return;

        // Wait for first click to start the round
        if (waitingForStart && Input.GetMouseButtonDown(0))
        {
            waitingForStart = false;
            roundRunning = true;
            isCharging = true;
            attemptNumber++;

            if (instructionText != null)
            {
                instructionText.text = "DES CLICKS! DES CLICKS! DES CLICKS!";
            }

            AddCharge();
            StartCoroutine(ChargeTimer());
            return;
        }

        // During charging phase, count clicks
        if (isCharging && Input.GetMouseButtonDown(0))
        {
            AddCharge();
        }
    }

    void AddCharge()
    {
        clickCount++;
        Play(clickClip, clickVolume);
        charge = Mathf.Clamp(charge + chargePerClick, 0f, maxCharge);
        UpdateUI();
        
        // Visual feedback - pulse the weight slightly
        if (weightRect != null)
        {
            StartCoroutine(PulseWeight());
        }
    }

    IEnumerator PulseWeight()
    {
        Vector3 originalScale = weightRect.localScale;
        Vector3 pulseScale = originalScale * 1.1f;
        
        float t = 0;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            weightRect.localScale = Vector3.Lerp(originalScale, pulseScale, t / 0.05f);
            yield return null;
        }
        
        t = 0;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            weightRect.localScale = Vector3.Lerp(pulseScale, originalScale, t / 0.05f);
            yield return null;
        }
        
        weightRect.localScale = originalScale;
    }

    IEnumerator ChargeTimer()
    {
        float remaining = chargeDuration;

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;

            if (timerText != null)
                timerText.text = remaining.ToString("0.0") + "s";

            yield return null;
        }

        if (timerText != null)
            timerText.text = "TEMPS!";

        isCharging = false;

        if (instructionText != null)
        {
            instructionText.text = $"Clicks: {clickCount}";
        }

        // Now the ball rises based on charge
        float normalized = charge / maxCharge;
        yield return HammerImpact(normalized);
    }

    IEnumerator HammerImpact(float normalized)
    {
        if (instructionText != null)
            instructionText.text = "STRIKE!";

        // Move hammer back (wind up)
        Vector2 backPos = new Vector2(hammerStartPos.x + hammerBackX, hammerStartPos.y);
        yield return MoveHammer(backPos);

        // Scale hammer up
        yield return ScaleHammer(hammerStartScale * growScale);

        // Attack move and scale down
        Vector2 impactPos = new Vector2(hammerImpactX, hammerStartPos.y);
        yield return AttackMoveAndScale(impactPos);

        // Impact effect
        if (hammerImage != null && hammerHit != null)
            hammerImage.sprite = hammerHit;
        
        Play(hitClip, hitVolume);
        yield return ShakeHammer();

        // Now the weight/ball rises up the tower!
        yield return MoveWeightUp(normalized);

        // Return hammer to normal
        if (hammerImage != null && hammerUp != null)
            hammerImage.sprite = hammerUp;
        
        yield return MoveHammer(hammerStartPos);
        yield return ScaleHammer(hammerStartScale);

        // Check victory
        yield return HandleResult(normalized);
    }

    IEnumerator MoveWeightUp(float normalized)
    {
        if (weightRect == null) yield break;

        float targetY = Mathf.Lerp(weightMinY, weightMaxY, Mathf.Clamp01(normalized));
        Vector2 startPos = weightRect.anchoredPosition;
        Vector2 target = new Vector2(startPos.x, targetY);

        // Animate the ball rising up the tower
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Use ease out for satisfying rise
            t = 1f - Mathf.Pow(1f - t, 3f);
            
            weightRect.anchoredPosition = Vector2.Lerp(startPos, target, t);
            yield return null;
        }

        weightRect.anchoredPosition = target;

        // If reached the top, play ding
        if (normalized >= victoryThreshold)
        {
            Play(dingClip, dingVolume);
        }
    }

    IEnumerator HandleResult(float normalized)
    {
        yield return new WaitForSeconds(0.5f);

        if (normalized >= victoryThreshold)
        {
            // Victory!
            OnWin();
            if (instructionText != null)
            {
                if (normalized >= 1f)
                    instructionText.text = "MAXIMUM! TU ES UN CHAMPION!";
                else
                    instructionText.text = "VICTOIRE! Tu as du muscle!";
            }

            Play(victoryMusic, victoryVolume);
            yield return new WaitForSeconds(delayAfterResult);

            onGameWin?.Invoke();
            EndGame();
        }
        else
        {
            // Failed - try again with easier difficulty
            OnFail();
            
            float percentage = normalized * 100f;
            if (instructionText != null)
            {
                instructionText.text = $"Only {percentage:F0}%! Try again! (Attempt {attemptNumber + 1})";
            }

            if (failSound != null)
                Play(failSound, 0.5f);

            yield return new WaitForSeconds(1.5f);

            // Reset for another attempt
            ResetRound();
            waitingForStart = true;
            
            if (instructionText != null)
            {
                instructionText.text = "Click to try again! (It gets easier!)";
            }
        }
    }

    void OnFail()
    {
        failCount++;

        // Make it progressively easier
        if (failCount == 1)
        {
            chargePerClick = baseChargePerClick * 1.2f;
        }
        else if (failCount == 2)
        {
            chargePerClick = chargeAfterTwoFails;
        }
        else
        {
            chargePerClick += extraChargePerFail;
        }
        
        // Cap the help so it doesn't become trivial
        chargePerClick = Mathf.Min(chargePerClick, baseChargePerClick * 3f);
    }

    void OnWin()
    {
        failCount = 0;
        chargePerClick = baseChargePerClick;
    }

    IEnumerator MoveHammer(Vector2 target)
    {
        if (hammerRect == null) yield break;

        Vector2 start = hammerRect.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleAnimSpeed;
            hammerRect.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        hammerRect.anchoredPosition = target;
    }

    IEnumerator ScaleHammer(Vector3 targetScale)
    {
        if (hammerRect == null) yield break;

        Vector3 start = hammerRect.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleAnimSpeed;
            hammerRect.localScale = Vector3.Lerp(start, targetScale, t);
            yield return null;
        }

        hammerRect.localScale = targetScale;
    }

    IEnumerator AttackMoveAndScale(Vector2 targetPos)
    {
        if (hammerRect == null) yield break;

        Vector2 startPos = hammerRect.anchoredPosition;
        Vector3 startScale = hammerRect.localScale;
        Vector3 targetScale = hammerStartScale * shrinkScale;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleAnimSpeed * 1.5f; // Faster attack
            hammerRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            hammerRect.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        hammerRect.anchoredPosition = targetPos;
        hammerRect.localScale = targetScale;
    }

    IEnumerator ShakeHammer()
    {
        if (hammerRect == null) yield break;

        Vector2 originalPos = hammerRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float offsetX = Random.Range(-shakeStrength, shakeStrength);
            float offsetY = Random.Range(-shakeStrength, shakeStrength);

            hammerRect.anchoredPosition = new Vector2(
                originalPos.x + offsetX,
                originalPos.y + offsetY
            );

            yield return null;
        }

        hammerRect.anchoredPosition = originalPos;
    }

    void ResetRound()
    {
        charge = 0f;
        clickCount = 0;
        roundRunning = false;
        isCharging = false;

        if (hammerImage != null && hammerUp != null)
            hammerImage.sprite = hammerUp;
        
        if (hammerRect != null)
        {
            hammerRect.anchoredPosition = hammerStartPos;
            hammerRect.localScale = hammerStartScale;
        }

        // Reset weight to bottom of tower
        if (weightRect != null)
            weightRect.anchoredPosition = new Vector2(weightRect.anchoredPosition.x, weightMinY);

        UpdateUI();
        
        if (timerText != null)
            timerText.text = chargeDuration.ToString("0.0") + "s";
    }

    void UpdateUI()
    {
        if (chargeBar != null)
            chargeBar.value = charge / maxCharge;
    }

    void Play(AudioClip clip, float volume)
    {
        if (clip != null && audioSrc != null)
            audioSrc.PlayOneShot(clip, volume);
    }
}
