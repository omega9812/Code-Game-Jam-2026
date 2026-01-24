using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FixWaterEffect : EditorWindow
{
    [MenuItem("Tools/Fix Water Effect")]
    public static void FixEffect()
    {
        // Find the WaterSprayEffect GameObject
        GameObject waterSprayEffect = GameObject.Find("WaterSprayEffect");
        if (waterSprayEffect == null)
        {
            Debug.LogError("WaterSprayEffect not found in the scene!");
            return;
        }

        // Make sure the GameObject is active
        waterSprayEffect.SetActive(true);

        // Get or add ParticleSystem component
        ParticleSystem ps = waterSprayEffect.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            ps = waterSprayEffect.AddComponent<ParticleSystem>();
        }

        // Configure main module
        var main = ps.main;
        main.startSpeed = 5f;
        main.startSize = 0.2f;
        main.startLifetime = 1f;
        main.maxParticles = 1000;
        main.startColor = new Color(0.7f, 0.85f, 1f, 0.8f); // Light blue color

        // Configure emission
        var emission = ps.emission;
        emission.rateOverTime = 100;
        emission.enabled = true;

        // Configure shape
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.1f;
        shape.enabled = true;

        // Configure renderer
        var renderer = waterSprayEffect.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            renderer.material.SetColor("_Color", new Color(0.7f, 0.85f, 1f, 0.8f));
            renderer.sortMode = ParticleSystemSortMode.Distance;
            renderer.minParticleSize = 0.1f;
            renderer.maxParticleSize = 0.3f;
        }

        // Position the water spray effect properly
        Transform clownHidingPosition = GameObject.Find("ClownHidingPosition")?.transform;
        if (clownHidingPosition != null)
        {
            // Position the water spray slightly in front of the clown
            waterSprayEffect.transform.position = clownHidingPosition.position + new Vector3(0.5f, 0.2f, 0f);
            waterSprayEffect.transform.rotation = Quaternion.Euler(0, 0, -90); // Point horizontally
            Debug.Log("Positioned water spray effect near the clown");
        }

        // Add a light to make the water particles more visible
        Transform lightTransform = waterSprayEffect.transform.Find("WaterSprayLight");
        GameObject lightObj;
        
        if (lightTransform == null)
        {
            lightObj = new GameObject("WaterSprayLight");
            lightObj.transform.SetParent(waterSprayEffect.transform);
            lightObj.transform.localPosition = Vector3.zero;
            
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.7f, 0.85f, 1f);
            light.intensity = 1.5f;
            light.range = 3f;
        }

        // Find the SplashCutscene GameObject
        GameObject splashCutscene = GameObject.Find("SplashCutscene");
        if (splashCutscene != null)
        {
            // Get the CutsceneController component
            CutsceneController controller = splashCutscene.GetComponent<CutsceneController>();
            if (controller != null)
            {
                // Set the waterSprayEffect reference
                controller.waterSprayEffect = waterSprayEffect;
                Debug.Log("Set waterSprayEffect reference in CutsceneController");
            }
        }

        // Make sure the water effect is initially inactive
        waterSprayEffect.SetActive(false);

        Debug.Log("Water effect fixed successfully!");
        
        // Mark the scene as dirty so the changes can be saved
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}