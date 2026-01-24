using UnityEngine;
using System.Collections;

public class HammerCursor : MonoBehaviour
{
    [Header("Sprites marteau (3 étapes)")]
    public Sprite idle;
    public Sprite mid;
    public Sprite hit;

    [Header("Animation")]
    public float stepTime = 0.05f;

    [Header("Offset curseur")]
    public Vector2 mouseOffset = new Vector2(0f, -0.8f);

    private SpriteRenderer sr;
    private bool animating = false;

    void Start()
    {
        Cursor.visible = false; // cache la souris système
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = idle;
    }

    void Update()
    {
        FollowMouse();

        if (Input.GetMouseButtonDown(0) && !animating)
        {
            StartCoroutine(HammerAnim());
        }
    }

    void FollowMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        transform.position = worldPos + (Vector3)mouseOffset;
    }

    IEnumerator HammerAnim()
    {
        animating = true;

        if (mid != null)
            sr.sprite = mid;

        yield return new WaitForSeconds(stepTime);

        if (hit != null)
            sr.sprite = hit;

        yield return new WaitForSeconds(stepTime);

        if (idle != null)
            sr.sprite = idle;

        animating = false;
    }

    void OnDisable()
    {
        Cursor.visible = true; // sécurité quand on quitte la scène
    }
}
