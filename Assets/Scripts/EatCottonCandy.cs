using UnityEngine;

public class EatCottonCandy : MonoBehaviour
{
    public Sprite[] stages;
    public int clicksPerStage = 5;

    private int clickCount = 0;
    private int stageIndex = 0;
    private SpriteRenderer sr;
    private Camera cam;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        if (stages.Length > 0)
            sr.sprite = stages[0];
    }

    void Update()
    {
        // Détecte le clic gauche de la souris
        if (Input.GetMouseButtonDown(0))
        {
            // Position de la souris dans le monde
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            // Raycast 2D pour savoir si on clique sur l'objet
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Eat();
            }
        }
    }

    void Eat()
    {
        if (stageIndex >= stages.Length - 1)
            return;

        clickCount++;

        if (clickCount % clicksPerStage == 0)
        {
            stageIndex++;
            sr.sprite = stages[stageIndex];
        }
    }
}
