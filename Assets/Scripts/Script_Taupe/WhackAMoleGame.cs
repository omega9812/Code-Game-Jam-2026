using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class WhackAMoleGame : MonoBehaviour
{
    [Header("Positions (9 trous)")]
    public Transform[] holes;

    [Header("Prefab taupe")]
    public Mole molePrefab;

    [Header("Spawn - base")]
    public float baseMinSpawnDelay = 0.35f;
    public float baseMaxSpawnDelay = 0.80f;

    [Header("Taupe - base")]
    public float baseVisibleTime = 0.90f;
    public float baseAppearStepTime = 0.06f;

    [Header("Difficulté (paliers)")]
    public float t1 = 20f;
    public float t2 = 10f;
    public float t3 = 5f;

    public float spawnMultAt20 = 0.85f;
    public float spawnMultAt10 = 0.70f;
    public float spawnMultAt5 = 0.55f;

    public float moleMultAt20 = 0.85f;
    public float moleMultAt10 = 0.70f;
    public float moleMultAt5 = 0.55f;

    [Header("Jeu")]
    public float gameDuration = 30f;
    public int scorePerHit = 1;
    public int scoreToWin = 30;

    [Header("Pénalité missclick")]
    public float missPenaltySeconds = 0.2f;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI penaltyText;
    public TextMeshProUGUI endText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hammerClick;
    public AudioClip buzzer;
    public AudioClip victoryMusic;

    [Header("Fin")]
    public string menuSceneName = "MenuScene";

    private int score = 0;
    private float timeLeft;
    private bool isRunning = false;

    private Mole[] moles; // 2 taupes max
    private float minSpawnDelay;
    private float maxSpawnDelay;

    private Vector3 penaltyStartPos;

    void Start()
    {
        timeLeft = gameDuration;
        minSpawnDelay = baseMinSpawnDelay;
        maxSpawnDelay = baseMaxSpawnDelay;

        if (penaltyText != null)
        {
            penaltyText.gameObject.SetActive(false);
            penaltyStartPos = penaltyText.rectTransform.localPosition;
        }

        // instancie 2 taupes
        moles = new Mole[2];
        for (int i = 0; i < 2; i++)
        {
            moles[i] = Instantiate(molePrefab);
            moles[i].gameObject.SetActive(false);
        }

        isRunning = true;
        UpdateUI();

        // 2 boucles de spawn indépendantes
        StartCoroutine(SpawnLoop(0));
        StartCoroutine(SpawnLoop(1));
    }

    void Update()
    {
        if (!isRunning) return;

        // ✅ TIMER TOUJOURS ACTIF (même pendant les WaitForSeconds)
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;

        ApplyDifficulty();
        UpdateUI();

        if (timeLeft <= 0f)
        {
            isRunning = false;
            StartCoroutine(EndAndReturn());
            return;
        }

        // clic
        if (Input.GetMouseButtonDown(0))
        {
            PlaySound(hammerClick, 1f);

            bool hitMole = false;

            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);

            if (hit.collider != null)
            {
                Mole m = hit.collider.GetComponent<Mole>();
                if (m != null && m.CanBeHit)
                {
                    hitMole = true;
                    StartCoroutine(HandleHit(m));
                }
            }

            // missclick
            if (!hitMole)
            {
                timeLeft -= missPenaltySeconds;
                if (timeLeft < 0f) timeLeft = 0f;

                PlaySound(buzzer, 0.5f);
                StartCoroutine(ShowPenalty());
            }
        }
    }

    IEnumerator SpawnLoop(int moleIndex)
    {
        // Chaque taupe a sa boucle de spawn.
        // moleIndex 0 = taupe principale
        // moleIndex 1 = taupe secondaire (activée selon le temps / hasard)

        while (true)
        {
            if (!isRunning) yield break;

            // si taupe déjà active, on attend un peu et on re-teste
            if (moles[moleIndex].gameObject.activeSelf)
            {
                yield return null;
                continue;
            }

            // détermine si cette taupe a le droit de spawner
            if (!IsMoleAllowed(moleIndex))
            {
                yield return null;
                continue;
            }

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            if (!isRunning || timeLeft <= 0f) yield break;

            // re-check autorisation juste avant spawn
            if (!IsMoleAllowed(moleIndex)) continue;

            int idx = Random.Range(0, holes.Length);
            moles[moleIndex].SpawnAt(holes[idx].position);
        }
    }

    bool IsMoleAllowed(int moleIndex)
    {
        // taupe 0 toujours autorisée
        if (moleIndex == 0) return true;

        // taupe 1 : règles
        // >10s : jamais
        if (timeLeft > 10f) return false;

        // 5s..10s : parfois (30% de chance)
        if (timeLeft > 5f) return Random.value < 0.30f;

        // <=5s : toujours (donc 2 taupes)
        return true;
    }

    IEnumerator HandleHit(Mole m)
    {
        score += scorePerHit;
        UpdateUI();
        yield return m.Hit();
    }

    IEnumerator ShowPenalty()
    {
        if (penaltyText == null) yield break;

        penaltyText.rectTransform.localPosition = penaltyStartPos;
        penaltyText.alpha = 1f;
        penaltyText.text = "-" + missPenaltySeconds.ToString("0.0") + "s";
        penaltyText.gameObject.SetActive(true);

        Vector3 targetPos = penaltyStartPos + Vector3.up * 20f;

        float duration = 0.5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            penaltyText.rectTransform.localPosition = Vector3.Lerp(penaltyStartPos, targetPos, p);
            penaltyText.alpha = Mathf.Lerp(1f, 0f, p);

            yield return null;
        }

        penaltyText.gameObject.SetActive(false);
    }

    void ApplyDifficulty()
    {
        float spawnMult = 1f;
        float moleMult = 1f;

        if (timeLeft <= t3)
        {
            spawnMult = spawnMultAt5;
            moleMult = moleMultAt5;
        }
        else if (timeLeft <= t2)
        {
            spawnMult = spawnMultAt10;
            moleMult = moleMultAt10;
        }
        else if (timeLeft <= t1)
        {
            spawnMult = spawnMultAt20;
            moleMult = moleMultAt20;
        }

        minSpawnDelay = Mathf.Max(0.05f, baseMinSpawnDelay * spawnMult);
        maxSpawnDelay = Mathf.Max(minSpawnDelay + 0.05f, baseMaxSpawnDelay * spawnMult);

        for (int i = 0; i < moles.Length; i++)
        {
            if (moles[i] == null) continue;
            moles[i].visibleTime = Mathf.Max(0.2f, baseVisibleTime * moleMult);
            moles[i].appearStepTime = Mathf.Max(0.03f, baseAppearStepTime * moleMult);
        }
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (timerText != null) timerText.text = timeLeft.ToString("0.0") + "s";
    }

    IEnumerator EndAndReturn()
    {
        bool win = score >= scoreToWin;

        if (win) PlaySound(victoryMusic, 1f);

        if (endText != null)
        {
            endText.gameObject.SetActive(true);
            endText.text = win ? "GAGNÉ !" : "PERDU !";
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(menuSceneName);
    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip, volume);
    }
}
