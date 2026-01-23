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
    public float missPenaltySeconds = 0.5f;

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

    private Mole currentMole;
    private float minSpawnDelay;
    private float maxSpawnDelay;

    void Start()
    {
        timeLeft = gameDuration;
        minSpawnDelay = baseMinSpawnDelay;
        maxSpawnDelay = baseMaxSpawnDelay;

        if (penaltyText != null)
            penaltyText.gameObject.SetActive(false);

        UpdateUI();
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        isRunning = true;

        currentMole = Instantiate(molePrefab);
        currentMole.gameObject.SetActive(false);

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0f) timeLeft = 0f;

            ApplyDifficulty();
            UpdateUI();

            yield return null;

            if (!currentMole.gameObject.activeSelf && timeLeft > 0f)
            {
                float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
                yield return new WaitForSeconds(delay);

                if (timeLeft <= 0f) break;

                int idx = Random.Range(0, holes.Length);
                currentMole.SpawnAt(holes[idx].position);
            }
        }

        isRunning = false;
        StartCoroutine(EndAndReturn());
    }

    void Update()
    {
        if (!isRunning) return;

        if (Input.GetMouseButtonDown(0))
        {
            PlaySound(hammerClick);

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

            if (!hitMole)
            {
                timeLeft -= missPenaltySeconds;
                if (timeLeft < 0f) timeLeft = 0f;

                PlaySound(buzzer);
                StartCoroutine(ShowPenalty());
                UpdateUI();
            }
        }
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

        penaltyText.gameObject.SetActive(true);
        penaltyText.alpha = 1f;

        Vector3 startPos = penaltyText.rectTransform.localPosition;
        Vector3 upPos = startPos + Vector3.up * 20f;

        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            penaltyText.rectTransform.localPosition = Vector3.Lerp(startPos, upPos, t / 0.5f);
            penaltyText.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
            yield return null;
        }

        penaltyText.gameObject.SetActive(false);
        penaltyText.rectTransform.localPosition = startPos;
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

        if (currentMole != null)
        {
            currentMole.visibleTime = Mathf.Max(0.2f, baseVisibleTime * moleMult);
            currentMole.appearStepTime = Mathf.Max(0.03f, baseAppearStepTime * moleMult);
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (timerText != null)
            timerText.text = timeLeft.ToString("0.0") + "s";
    }

    IEnumerator EndAndReturn()
    {
        bool win = score >= scoreToWin;

        if (win)
            PlaySound(victoryMusic);

        if (endText != null)
        {
            endText.gameObject.SetActive(true);
            endText.text = win ? "GAGNÉ !" : "PERDU !";
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(menuSceneName);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
