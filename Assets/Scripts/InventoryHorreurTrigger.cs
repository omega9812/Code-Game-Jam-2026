using UnityEngine;
using System.Collections;
using AC;

/// <summary>
/// Checks if the player has both "batonnet" and "gilbert" items in their inventory,
/// then triggers a cutscene and loads the "Horreur" scene.
/// </summary>
public class InventoryHorreurTrigger : MonoBehaviour
{
    [Header("Item Names")]
    [SerializeField] private string batonnetItemName = "batonnet";
    [SerializeField] private string gilbertItemName = "gilbert";
    
    [Header("Scene Settings")]
    [SerializeField] private string horreurSceneName = "Horreur";
    
    [Header("Effect Settings")]
    [SerializeField] private float fadeOutDuration = 2.0f;
    [SerializeField] private Light sceneLight;
    [SerializeField] private float lightFadeDuration = 2.5f;
    [SerializeField] private float delayBeforeCutscene = 20.0f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip horrorSound;
    [SerializeField] private float audioVolume = 1f;
    
    private bool hasTriggeredCutscene = false;
    private AudioSource audioSource;
    
    private void Start()
    {
        // If no light is assigned, try to find one in the scene
        if (sceneLight == null)
        {
            Light[] lights = FindObjectsOfType<Light>();
            if (lights.Length > 0)
            {
                sceneLight = lights[0];
            }
        }
        
        // Add audio source if needed
        if (horrorSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    
    private void Update()
    {
        // Only check if we haven't triggered the cutscene yet
        if (!hasTriggeredCutscene)
        {
            CheckInventoryAndTriggerCutscene();
        }
    }
    
    private void CheckInventoryAndTriggerCutscene()
    {
        // Check if both items are in the inventory
        if (HasItem(batonnetItemName) && HasItem(gilbertItemName))
        {
            hasTriggeredCutscene = true;
            StartCoroutine(PlayCutsceneAndLoadScene());
        }
    }
    
    private bool HasItem(string itemName)
    {
        // Use Adventure Creator's inventory system to check if the player has the item
        if (KickStarter.runtimeInventory != null)
        {
            InvItem item = KickStarter.runtimeInventory.GetItem(itemName);
            return item != null;
        }
        return false;
    }
    
    // Method to set the scene light via SendMessage
    private void SetSceneLight(Light light)
    {
        if (light != null)
        {
            sceneLight = light;
            Debug.Log("Scene light assigned: " + light.name);
        }
    }
    
    private IEnumerator PlayCutsceneAndLoadScene()
    {
        // Wait for the specified delay before starting the cutscene
        Debug.Log("Both items detected. Waiting " + delayBeforeCutscene + " seconds before starting cutscene...");
        yield return new WaitForSeconds(delayBeforeCutscene);
        Debug.Log("Starting cutscene now!");
        
        // Disable player control if possible
        if (KickStarter.player != null)
        {
            KickStarter.player.EndPath();
            KickStarter.player.Halt();
        }
        
        // Play horror sound if available
        if (audioSource != null && horrorSound != null)
        {
            audioSource.clip = horrorSound;
            audioSource.volume = audioVolume;
            audioSource.Play();
        }
        
        // Fade out the screen using AC's built-in methods
        if (KickStarter.mainCamera != null)
        {
            KickStarter.mainCamera.FadeOut(fadeOutDuration);
        }
        
        // Fade light
        if (sceneLight != null)
        {
            float startTime = Time.time;
            float endTime = startTime + lightFadeDuration;
            float originalIntensity = sceneLight.intensity;
            
            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / lightFadeDuration;
                sceneLight.intensity = Mathf.Lerp(originalIntensity, 0f, t);
                yield return null;
            }
            
            sceneLight.intensity = 0f;
        }
        
        // Wait for fade to complete
        yield return new WaitForSeconds(fadeOutDuration);
        
        // Load the Horreur scene using Adventure Creator's scene switching
        if (KickStarter.sceneChanger != null)
        {
            KickStarter.sceneChanger.ChangeScene(horreurSceneName, true, false);
        }
        else
        {
            // Fallback to Unity's scene loading if AC's scene changer is not available
            UnityEngine.SceneManagement.SceneManager.LoadScene(horreurSceneName);
        }
    }
}