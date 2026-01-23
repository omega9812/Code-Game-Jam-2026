using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EatCottonCandy : MonoBehaviour
{
    [Header("Sprites (ordre : complet -> ... -> dernier)")]
    public Sprite[] stages;

    [Header("Nombre de clics pour passer au sprite suivant")]
    public int clicksPerStage = 5;

    [Header("Dernier sprite : clics supplémentaires avant fin")]
    public int finishClicksNeeded = 5;

    [Header("Effet de clic (recul)")]
    public float clickScale = 0.9f;        // 0.9 = petit recul
    public float clickAnimSpeed = 25f;     // plus grand = plus rapide
    public float clickHoldTime = 0.05f;    // temps "reculé"

    [Header("Fin du mini-jeu")]
    public ParticleSystem smokePS;         // glisse ton SmokeEffect (Particle System)
    public float disappearDuration = 0.25f;
    public string menuSceneName = "MenuScene";

    private SpriteRenderer sr;
    private Camera cam;

    private int clickCount = 0;
    private int stageIndex = 0;
    private int finishClicks = 0;

    private Vector3 originalScale;
    private bool canClick = true;
    private bool isFinishing = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        originalScale = transform.localScale;

        clickCount = 0;
        stageIndex = 0;
        finishClicks = 0;

        // Met le premier sprite seulement si correctement rempli
        if (stages != null && stages.Length > 0 && stages[0] != null)
            sr.sprite = stages[0];
        else
            Debug.LogWarning("Stages non configuré ou stages[0] vide : garde le sprite actuel.");

        // Ne doit pas jouer au démarrage
        if (smokePS != null)
            smokePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Update()
    {
        if (!canClick || isFinishing) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (cam == null) cam = Camera.main;

            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                HandleClick();
            }
        }
    }

    void HandleClick()
    {
        if (stages == null || stages.Length == 0) return;

        // Lance l'anim de clic et bloque les clics pendant l'anim
        StartCoroutine(ClickEffect());

        bool isLastStage = stageIndex >= stages.Length - 1;

        // --- Cas : on est déjà sur le dernier sprite -> il faut encore cliquer finishClicksNeeded fois ---
        if (isLastStage)
        {
            finishClicks++;

            if (finishClicks >= finishClicksNeeded)
            {
                StartCoroutine(FinishCandy());
            }
            return;
        }

        // --- Cas normal : on avance de sprite tous les clicksPerStage clics ---
        clickCount++;

        if (clickCount % clicksPerStage == 0)
        {
            stageIndex++;

            if (stageIndex < stages.Length && stages[stageIndex] != null)
                sr.sprite = stages[stageIndex];

            // IMPORTANT : on ne déclenche PAS la fin ici,
            // on attend les clics supplémentaires sur le dernier sprite.
        }
    }

    IEnumerator ClickEffect()
    {
        canClick = false;

        // Recul instantané
        transform.localScale = originalScale * clickScale;

        // mini délai
        yield return new WaitForSeconds(clickHoldTime);

        // Retour rapide
        float t = 0f;
        Vector3 from = originalScale * clickScale;
        Vector3 to = originalScale;

        while (t < 1f)
        {
            t += Time.deltaTime * clickAnimSpeed;
            transform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.localScale = originalScale;
        canClick = true;
    }

    IEnumerator FinishCandy()
    {
        if (isFinishing) yield break;
        isFinishing = true;
        canClick = false;

        float smokeTime = 0.5f;

        // Fumée
        if (smokePS != null)
        {
            smokePS.transform.position = transform.position;
            smokePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            smokePS.Play();

            // Durée totale approximative de l'effet
            var main = smokePS.main;
            smokeTime = main.duration + main.startLifetime.constantMax;
        }
        else
        {
            Debug.LogWarning("SmokePS non assigné dans l'Inspector !");
        }

        // Disparition (scale -> 0)
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, disappearDuration);
            transform.localScale = Vector3.Lerp(start, Vector3.zero, t);
            yield return null;
        }

        transform.localScale = Vector3.zero;

        // On laisse le temps à la fumée d'être visible
        yield return new WaitForSeconds(1f);

        // Retour au menu
        SceneManager.LoadScene(menuSceneName);
    }
}
