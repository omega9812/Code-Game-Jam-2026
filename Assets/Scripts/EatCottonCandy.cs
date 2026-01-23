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
    public float clickScale = 0.9f;
    public float clickAnimSpeed = 25f;
    public float clickHoldTime = 0.05f;

    [Header("Fin du mini-jeu")]
    public ParticleSystem smokePS;
    public float disappearDuration = 0.25f;
    public float waitAfterSmoke = 0.2f;
    public string menuSceneName = "MenuScene";

    [Header("Sons")]
    public AudioClip crunchClip;   // son à chaque clic
    public AudioClip pouffClip;    // son à la fin (fumée)
    [Range(0f, 1f)] public float crunchVolume = 0.8f;
    [Range(0f, 1f)] public float pouffVolume = 1f;

    [Header("Timing son")]
    public float crunchBeforeSpriteDelay = 0.04f; // 40 ms

    private SpriteRenderer sr;
    private Camera cam;
    private AudioSource audioSrc;

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
        audioSrc = GetComponent<AudioSource>();

        originalScale = transform.localScale;

        clickCount = 0;
        stageIndex = 0;
        finishClicks = 0;

        if (sr == null)
        {
            Debug.LogError("Il manque un SpriteRenderer sur l'objet.");
            enabled = false;
            return;
        }

        // Met le premier sprite si stages est rempli
        if (stages != null && stages.Length > 0 && stages[0] != null)
            sr.sprite = stages[0];
        else
            Debug.LogWarning("Stages non configuré ou stages[0] vide : garde le sprite actuel.");

        // AudioSource recommandé mais pas obligatoire
        if (audioSrc != null)
            audioSrc.playOnAwake = false;

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

        bool isLastStage = stageIndex >= stages.Length - 1;

        // ---- Dernier sprite : clics supplémentaires ----
        if (isLastStage)
        {
            finishClicks++;

            // Son crunch aussi sur les bouchées finales (optionnel)
            StartCoroutine(ClickEffect(true));

            if (finishClicks >= finishClicksNeeded)
            {
                StartCoroutine(FinishCandy());
            }
            return;
        }

        // ---- Progression normale ----
        clickCount++;

        // Changement d'image = bouchée validée
        if (clickCount % clicksPerStage == 0)
        {
            stageIndex++;

            StartCoroutine(ClickEffect(true)); // crunch AVANT le croc

            if (stageIndex < stages.Length && stages[stageIndex] != null)
                sr.sprite = stages[stageIndex];
        }
        else
        {
            // Clic non compté → pas de son
            StartCoroutine(ClickEffect(false));
        }
    }




    IEnumerator ClickEffect(bool playCrunch)
    {
        canClick = false;

        // --- Son crunch AVANT le croc visuel ---
        if (playCrunch && crunchClip != null)
        {
            yield return new WaitForSeconds(crunchBeforeSpriteDelay);
            PlayOneShot(crunchClip, crunchVolume);
        }

        // --- Croc visuel (recul) ---
        transform.localScale = originalScale * clickScale;

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

        // Son "pouff" à la fin
        PlayOneShot(pouffClip, pouffVolume);

        // Fumée
        if (smokePS != null)
        {
            smokePS.transform.position = transform.position;
            smokePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            smokePS.Play();
        }
        else
        {
            Debug.LogWarning("SmokePS non assigné dans l'Inspector !");
        }

        // Disparition
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, disappearDuration);
            transform.localScale = Vector3.Lerp(start, Vector3.zero, t);
            yield return null;
        }

        transform.localScale = Vector3.zero;

        // Petit délai (réglable) pour laisser voir fumée/son
        yield return new WaitForSeconds(waitAfterSmoke);

        SceneManager.LoadScene(menuSceneName);
    }

    void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null) return;

        // Si pas d'AudioSource sur l'objet, Unity peut quand même jouer un son 2D
        if (audioSrc == null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, volume);
            return;
        }

        audioSrc.PlayOneShot(clip, volume);
    }
}
