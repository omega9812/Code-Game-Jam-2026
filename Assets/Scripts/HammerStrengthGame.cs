using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HammerStrengthGame : MonoBehaviour
{
    [Header("Références")]
    public SpriteRenderer hammerSR;      // SpriteRenderer du marteau
    public Transform hammerTransform;    // Transform du marteau (même objet que hammerSR)
    public Transform weight;
    public Transform weightMin;
    public Transform weightMax;

    [Header("UI (optionnel)")]
    public Slider chargeBar;            // barre de charge
    public Text clickCounterText;       // Text (Legacy) pour afficher le nb de clics

    [Header("Sprites marteau (2 étapes)")]
    public Sprite hammerUp;             // normal
    public Sprite hammerHit;            // impact

    [Header("Charge")]
    public float chargeDuration = 2.0f;     // temps pour spam clic
    public float chargePerClick = 0.08f;    // gain par clic
    public float chargeDecayPerSec = 0.0f;  // 0 = pas de baisse
    public float maxCharge = 1.0f;

    [Header("Poids")]
    public float weightMoveSpeed = 10f;

    [Header("Impact / décalage marteau")]
    public float hammerHitTime = 0.12f;     // durée sprite HIT
    public float hammerImpactX = 0f;        // X=0 au moment du coup
    public float hammerMoveToImpactTime = 0.05f;  // vitesse du déplacement vers X=0
    public float hammerReturnTime = 0.08f;        // retour position initiale

    private float charge = 0f;
    private bool isCharging = false;
    private bool isBusy = false;

    private int clickCount = 0;
    private Vector3 hammerStartPos;

    void Start()
    {
        if (hammerTransform == null && hammerSR != null) hammerTransform = hammerSR.transform;
        if (hammerTransform != null) hammerStartPos = hammerTransform.position;

        ResetRound();
    }

    void Update()
    {
        if (isBusy) return;

        // Test : Espace lance une manche
        if (!isCharging && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ChargeThenHit());
        }

        // Détection clic : uniquement pendant la charge
        if (isCharging && Input.GetMouseButtonDown(0))
        {
            clickCount++;
            UpdateClickCounter();

            charge = Mathf.Clamp(charge + chargePerClick, 0f, maxCharge);
            UpdateUI();
        }

        // Option : la charge baisse si on arrête
        if (isCharging && chargeDecayPerSec > 0f)
        {
            charge = Mathf.Clamp(charge - chargeDecayPerSec * Time.deltaTime, 0f, maxCharge);
            UpdateUI();
        }
    }

    // Pour lancer via un bouton UI
    public void StartRound()
    {
        if (!isCharging && !isBusy)
            StartCoroutine(ChargeThenHit());
    }

    IEnumerator ChargeThenHit()
    {
        isBusy = true;
        isCharging = true;

        charge = 0f;
        clickCount = 0;
        UpdateUI();
        UpdateClickCounter();

        // Phase de charge (spam clic)
        float t = 0f;
        while (t < chargeDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        isCharging = false;

        float normalized = Mathf.InverseLerp(0f, maxCharge, charge);

        // Impact (sprite hit + déplacement X=0 + montée du poids)
        yield return HammerImpact(normalized);

        // Petite pause puis reset
        yield return new WaitForSeconds(0.8f);

        ResetRound();
        isBusy = false;
    }

    IEnumerator HammerImpact(float normalized)
    {
        if (hammerSR == null || hammerTransform == null) yield break;

        // Déplace le marteau vers X = 0 (impact)
        Vector3 impactPos = new Vector3(hammerImpactX, hammerTransform.position.y, hammerTransform.position.z);
        yield return MoveTransform(hammerTransform, impactPos, hammerMoveToImpactTime);

        // Sprite HIT
        if (hammerHit != null) hammerSR.sprite = hammerHit;

        // Le poids monte au moment de l'impact
        yield return MoveWeightToNormalized(normalized);

        // Maintient un peu le sprite HIT
        yield return new WaitForSeconds(hammerHitTime);

        // Retour sprite normal
        if (hammerUp != null) hammerSR.sprite = hammerUp;

        // Retour à la position de départ du marteau
        yield return MoveTransform(hammerTransform, hammerStartPos, hammerReturnTime);
    }

    IEnumerator MoveWeightToNormalized(float normalized)
    {
        if (weight == null || weightMin == null || weightMax == null) yield break;

        Vector3 target = Vector3.Lerp(weightMin.position, weightMax.position, Mathf.Clamp01(normalized));

        while (Vector3.Distance(weight.position, target) > 0.01f)
        {
            weight.position = Vector3.Lerp(weight.position, target, Time.deltaTime * weightMoveSpeed);
            yield return null;
        }

        weight.position = target;
    }

    IEnumerator MoveTransform(Transform tr, Vector3 target, float duration)
    {
        if (tr == null) yield break;

        if (duration <= 0.001f)
        {
            tr.position = target;
            yield break;
        }

        Vector3 start = tr.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            tr.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        tr.position = target;
    }

    void ResetRound()
    {
        charge = 0f;
        isCharging = false;
        UpdateUI();

        if (hammerSR != null && hammerUp != null)
            hammerSR.sprite = hammerUp;

        if (hammerTransform != null)
        {
            // met à jour la position "départ" si tu as déplacé le marteau dans l’éditeur
            hammerStartPos = hammerTransform.position;
        }

        // Poids en bas
        if (weight != null && weightMin != null)
            weight.position = weightMin.position;

        // Compteur clics à 0
        clickCount = 0;
        UpdateClickCounter();
    }

    void UpdateUI()
    {
        if (chargeBar == null) return;
        chargeBar.minValue = 0f;
        chargeBar.maxValue = maxCharge;
        chargeBar.value = charge;
    }

    void UpdateClickCounter()
    {
        if (clickCounterText == null) return;
        clickCounterText.text = "Clics : " + clickCount;
    }
}
