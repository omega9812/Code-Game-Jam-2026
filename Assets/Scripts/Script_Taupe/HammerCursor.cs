using UnityEngine;
using UnityEngine.UI;
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
    public Vector2 mouseOffset = new Vector2(0f, -50f); // UI offset instead of world offset

    private Image img;
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private bool animating = false;

    void Start()
    {
        Cursor.visible = false; // cache la souris système
        
        // Try to get Image component first (UI-based)
        img = GetComponent<Image>();
        
        // If no Image, this is still using SpriteRenderer (shouldn't happen with new setup)
        if (img != null)
        {
            img.sprite = idle;
            rectTransform = GetComponent<RectTransform>();
            
            // Find parent canvas for coordinate conversion
            parentCanvas = GetComponentInParent<Canvas>();
        }
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
        if (img != null && rectTransform != null && parentCanvas != null)
        {
            // UI-based positioning
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                Input.mousePosition,
                parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
                out localPoint
            );

            rectTransform.anchoredPosition = localPoint + mouseOffset;
        }
    }

    IEnumerator HammerAnim()
    {
        animating = true;

        if (img != null)
        {
            if (mid != null)
                img.sprite = mid;

            yield return new WaitForSeconds(stepTime);

            if (hit != null)
                img.sprite = hit;

            yield return new WaitForSeconds(stepTime);

            if (idle != null)
                img.sprite = idle;
        }

        animating = false;
    }

    void OnDisable()
    {
        Cursor.visible = true; // sécurité quand on quitte la scène
    }
}
