using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;


public class HammerStrengthGame : MonoBehaviour
{
    [Header("Références")]
    public SpriteRenderer hammerSR;
    public Transform hammerTransform;
    public Transform weight;
    public Transform weightMin;
    public Transform weightMax;

    [Header("UI")]
    public Slider chargeBar;
    public TextMeshProUGUI timerText;

    [Header("Sprites poêle")]
    public Sprite hammerUp;
    public Sprite hammerHit;

    [Header("Charge")]
    public float chargeDuration = 2f;
    public float chargePerClick = 0.045f;
    public float maxCharge = 1f;

    [Header("Poids")]
    public float weightMoveSpeed = 10f;

    [Header("Animation poêle")]
    public float growScale = 1.5f;
    public float shrinkScale = 0.7f;
    public float scaleAnimSpeed = 8f;

    [Header("Déplacement poêle")]
    public float hammerBackX = -0.6f;
    public float hammerImpactX = 0f; // IMPACT TOUJOURS À X=0

    [Header("Shake impact")]
    public float shakeDuration = 0.12f;
    public float shakeStrength = 0.08f;

    [Header("Sons")]
    public AudioClip clickClip;
    public AudioClip hitClip;
    public AudioClip dingClip;
    public AudioClip victoryMusic;

    public float clickVolume = 0.6f;
    public float hitVolume = 1f;
    public float dingVolume = 1f;
    public float victoryVolume = 1f;

    [Header("Victoire")]
    public float victoryThreshold = 0.9f;
    public float delayAfterDing = 1.5f;
    public string menuSceneName = "MenuScene";

    [Header("Aide adaptative")]
    public float baseChargePerClick = 0.045f;
    public float chargeAfterTwoFails = 0.05f;
    public float extraChargePerFail = 0.01f;

    private int failCount = 0;

    private AudioSource audioSrc;
    private Vector3 hammerStartPos;
    private Vector3 hammerStartScale;

    private float charge = 0f;
    private bool isCharging = false;
    private bool roundRunning = false;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();

        if (hammerTransform == null)
            hammerTransform = hammerSR.transform;

        hammerStartPos = hammerTransform.position;
        hammerStartScale = hammerTransform.localScale;

        chargePerClick = baseChargePerClick;

        ResetRound();
    }

    void Update()
    {
        if (!roundRunning && Input.GetMouseButtonDown(0))
        {
            roundRunning = true;
            isCharging = true;

            AddCharge();
            StartCoroutine(ChargeTimer());
            return;
        }

        if (isCharging && Input.GetMouseButtonDown(0))
        {
            AddCharge();
        }
    }

    void AddCharge()
    {
        Play(clickClip, clickVolume);
        charge = Mathf.Clamp(charge + chargePerClick, 0f, maxCharge);
        UpdateUI();
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
            timerText.text = "0.0s";

        isCharging = false;

        float normalized = charge / maxCharge;
        yield return HammerImpact(normalized);
    }


    IEnumerator HammerImpact(float normalized)
    {
        // RECUL
        Vector3 backPos = new Vector3(
            hammerStartPos.x + hammerBackX,
            hammerStartPos.y,
            hammerStartPos.z
        );
        yield return MoveTransform(backPos);

        // GROSSISSEMENT
        yield return ScaleHammer(hammerStartScale * growScale);

        // ATTAQUE vers X=0 + rétrécit
        Vector3 impactPos = new Vector3(
            hammerImpactX,
            hammerTransform.position.y,
            hammerTransform.position.z
        );
        yield return AttackMoveAndScale(impactPos);

        // LOCK X=0
        hammerTransform.position = new Vector3(
            hammerImpactX,
            hammerTransform.position.y,
            hammerTransform.position.z
        );

        // IMPACT
        hammerSR.sprite = hammerHit;
        Play(hitClip, hitVolume);
        yield return ShakeHammer();

        // POIDS MONTE
        yield return MoveWeight(normalized);

        // RETOUR NORMAL
        hammerSR.sprite = hammerUp;
        yield return MoveTransform(hammerStartPos);
        yield return ScaleHammer(hammerStartScale);

        // VICTOIRE ?
        yield return HandleVictory(normalized);
    }

    IEnumerator HandleVictory(float normalized)
    {
        if (normalized >= 1f)
        {
            OnWin();

            Play(dingClip, dingVolume);
            yield return new WaitForSeconds(delayAfterDing);

            Play(victoryMusic, victoryVolume);
            yield return new WaitForSeconds(1.5f);

            SceneManager.LoadScene(menuSceneName);
            yield break;
        }

        if (normalized >= victoryThreshold)
        {
            OnWin();

            Play(victoryMusic, victoryVolume);
            yield return new WaitForSeconds(1.5f);

            SceneManager.LoadScene(menuSceneName);
            yield break;
        }

        OnFail();
        ResetRound();
    }

    void OnFail()
    {
        failCount++;

        if (failCount == 2)
            chargePerClick = chargeAfterTwoFails;
        else if (failCount > 2)
            chargePerClick += extraChargePerFail;
    }

    void OnWin()
    {
        failCount = 0;
        chargePerClick = baseChargePerClick;
    }

    IEnumerator MoveWeight(float normalized)
    {
        Vector3 target = Vector3.Lerp(
            weightMin.position,
            weightMax.position,
            Mathf.Clamp01(normalized)
        );

        while (Vector3.Distance(weight.position, target) > 0.01f)
        {
            weight.position = Vector3.Lerp(
                weight.position,
                target,
                Time.deltaTime * weightMoveSpeed
            );
            yield return null;
        }

        weight.position = target;
    }

    IEnumerator MoveTransform(Vector3 target)
    {
        Vector3 start = hammerTransform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleAnimSpeed;
            hammerTransform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        hammerTransform.position = target;
    }

    IEnumerator ScaleHammer(Vector3 targetScale)
    {
        Vector3 start = hammerTransform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleAnimSpeed;
            hammerTransform.localScale = Vector3.Lerp(start, targetScale, t);
            yield return null;
        }

        hammerTransform.localScale = targetScale;
    }

    IEnumerator AttackMoveAndScale(Vector3 targetPos)
    {
        Vector3 startPos = hammerTransform.position;
        Vector3 startScale = hammerTransform.localScale;
        Vector3 targetScale = hammerStartScale * shrinkScale;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleAnimSpeed;
            hammerTransform.position = Vector3.Lerp(startPos, targetPos, t);
            hammerTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        hammerTransform.position = targetPos;
        hammerTransform.localScale = targetScale;
    }

    IEnumerator ShakeHammer()
    {
        Vector3 originalPos = hammerTransform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float offsetX = Random.Range(-shakeStrength, shakeStrength);
            float offsetY = Random.Range(-shakeStrength, shakeStrength);

            hammerTransform.position = new Vector3(
                originalPos.x + offsetX,
                originalPos.y + offsetY,
                originalPos.z
            );

            yield return null;
        }

        hammerTransform.position = originalPos;
    }

    void ResetRound()
    {
        charge = 0f;
        roundRunning = false;
        isCharging = false;

        hammerSR.sprite = hammerUp;
        hammerTransform.position = hammerStartPos;
        hammerTransform.localScale = hammerStartScale;

        weight.position = weightMin.position;
        UpdateUI();
        if (timerText != null)
            timerText.text = chargeDuration.ToString("0.0") + "s";

    }

    void UpdateUI()
    {
        if (chargeBar != null)
            chargeBar.value = charge;
    }

    void Play(AudioClip clip, float volume)
    {
        if (clip != null)
            audioSrc.PlayOneShot(clip, volume);
    }
}
