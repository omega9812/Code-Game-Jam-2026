using UnityEngine;
using UnityEngine.UI;

public class DirectSceneModifier : MonoBehaviour
{
    void Start()
    {
        // Directly modify the scene objects
        ModifySceneObjects();
    }

    void ModifySceneObjects()
    {
        // Find Bob and adjust scale
        GameObject bob = GameObject.Find("Bob");
        if (bob != null)
        {
            bob.transform.localScale = new Vector3(4.0f, 4.0f, 4.0f);
            Debug.Log("Bob scale set to 4.0");
        }

        // Find Clown and adjust scale
        GameObject clown = GameObject.Find("Clown");
        if (clown != null)
        {
            clown.transform.localScale = new Vector3(-4.0f, 4.0f, -4.0f); // Preserve negative X and Z for flipping
            Debug.Log("Clown scale set to 4.0");
        }

        // Find DialogueText and adjust properties
        GameObject dialogueText = GameObject.Find("DialogueBox/Panel/DialogueText");
        if (dialogueText != null)
        {
            Text textComponent = dialogueText.GetComponent<Text>();
            if (textComponent != null)
            {
                textComponent.fontSize = 36;
                textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                Debug.Log("DialogueText font size set to 36");
            }
        }

        // Find DialogueBox/Panel and adjust properties
        GameObject panel = GameObject.Find("DialogueBox/Panel");
        if (panel != null)
        {
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(800, 200);
                Debug.Log("Panel size adjusted");
            }
        }

        // Find WaterSprayEffect and enhance it
        GameObject waterSpray = GameObject.Find("WaterSprayEffect");
        if (waterSpray != null)
        {
            ParticleSystem ps = waterSpray.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startSize = 0.3f;
                main.startSpeed = 5.0f;
                
                var emission = ps.emission;
                emission.rateOverTime = 150;
                
                Debug.Log("WaterSprayEffect enhanced");
            }
            
            // Add a light to the water spray
            if (!waterSpray.transform.Find("Light"))
            {
                GameObject light = new GameObject("Light");
                light.transform.SetParent(waterSpray.transform);
                light.transform.localPosition = Vector3.zero;
                
                Light lightComponent = light.AddComponent<Light>();
                lightComponent.type = LightType.Point;
                lightComponent.color = new Color(0.7f, 0.85f, 1.0f);
                lightComponent.intensity = 2.0f;
                lightComponent.range = 5.0f;
                
                Debug.Log("Light added to WaterSprayEffect");
            }
        }
    }
}