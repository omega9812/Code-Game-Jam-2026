using UnityEngine;

public class CreateWaterSpray : MonoBehaviour
{
    public static void Execute()
    {
        // Find the WaterSprayEffect GameObject
        GameObject waterSprayEffect = GameObject.Find("WaterSprayEffect");
        
        if (waterSprayEffect == null)
        {
            Debug.LogError("WaterSprayEffect GameObject not found!");
            return;
        }
        
        // Add a Particle System component
        ParticleSystem particleSystem = waterSprayEffect.AddComponent<ParticleSystem>();
        
        // Configure the particle system for water spray
        var main = particleSystem.main;
        main.startSpeed = 5.0f;
        main.startSize = 0.1f;
        main.startLifetime = 1.0f;
        main.maxParticles = 1000;
        
        var emission = particleSystem.emission;
        emission.rateOverTime = 100;
        
        var shape = particleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15.0f;
        
        var colorOverLifetime = particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;
        
        // Position the water spray effect
        waterSprayEffect.transform.position = new Vector3(0, -1, 0);
        waterSprayEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
        
        // Initially disable the effect
        waterSprayEffect.SetActive(false);
        
        Debug.Log("Water spray effect created successfully!");
    }
}