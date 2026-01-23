using UnityEngine;
using System.Collections;

public class Mole : MonoBehaviour
{
    [Header("Sprites taupe")]
    public Sprite appear1;
    public Sprite appear2;
    public Sprite hitSprite;

    [Header("Timings")]
    public float appearStepTime = 0.06f;
    public float visibleTime = 0.9f;
    public float hitHoldTime = 0.15f;

    private SpriteRenderer sr;
    private bool isActive = false;
    private bool isHit = false;
    private Coroutine lifeRoutine;

    public bool CanBeHit => isActive && !isHit;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = null;
    }

    public void SpawnAt(Vector3 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);

        isActive = false;
        isHit = false;

        if (lifeRoutine != null) StopCoroutine(lifeRoutine);
        lifeRoutine = StartCoroutine(LifeCycle());
    }

    IEnumerator LifeCycle()
    {
        sr.sprite = appear1;
        yield return new WaitForSeconds(appearStepTime);

        sr.sprite = appear2;
        isActive = true;

        float t = 0f;
        while (t < visibleTime)
        {
            t += Time.deltaTime;
            if (isHit) yield break;
            yield return null;
        }

        Despawn();
    }

    public IEnumerator Hit()
    {
        if (!CanBeHit) yield break;

        isHit = true;
        isActive = false;

        sr.sprite = hitSprite;
        yield return new WaitForSeconds(hitHoldTime);

        Despawn();
    }

    public void Despawn()
    {
        isActive = false;
        isHit = false;
        sr.sprite = null;
        gameObject.SetActive(false);
    }
}
