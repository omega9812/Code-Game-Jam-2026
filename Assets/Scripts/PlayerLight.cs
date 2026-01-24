using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickerLight2D : MonoBehaviour
{
    public Light2D light2D;
    public float minIntensity = 0.4f;
    public float maxIntensity = 1.0f;
    public float speed = 10f;

    void Update()
    {
        light2D.intensity = Mathf.Lerp(
            minIntensity,
            maxIntensity,
            Mathf.PerlinNoise(Time.time * speed, 0f)
        );
    }
}
