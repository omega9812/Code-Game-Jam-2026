using UnityEngine;
using AC;
using System.Collections;

public class ClownMinigameTrigger : MonoBehaviour
{
    [Header("Minigame Setup")]
    public GameObject whackAMolePrefab;
    public Transform minigameSpawnPoint;
    
    [Header("Dialogue")]
    public int clownCharacterID = 0; // Set this to the ID of the clown character
    public Conversation firstConversation;
    public Conversation secondConversation;
    
    private bool hasSpokenOnce = false;
    private GameObject currentMinigame = null;
    
    void Start()
    {
        // Register for the OnEndConversation event
        EventManager.OnEndConversation += OnEndConversation;
    }
    
    void OnDestroy()
    {
        // Unregister from the event when destroyed
        EventManager.OnEndConversation -= OnEndConversation;
    }
    
    private void OnEndConversation(Conversation conversation)
    {
        // Check if this is the first conversation with the clown
        if (conversation == firstConversation && !hasSpokenOnce)
        {
            hasSpokenOnce = true;
            Debug.Log("First conversation with clown completed");
        }
        // Check if this is the second conversation with the clown
        else if (conversation == secondConversation && hasSpokenOnce)
        {
            Debug.Log("Second conversation with clown completed, launching minigame");
            StartCoroutine(LaunchMinigame());
        }
    }
    
    private IEnumerator LaunchMinigame()
    {
        // Wait a short moment before launching the minigame
        yield return new WaitForSeconds(0.5f);
        
        // Instantiate the minigame prefab
        if (whackAMolePrefab != null)
        {
            Vector3 spawnPosition = minigameSpawnPoint != null ? 
                minigameSpawnPoint.position : 
                new Vector3(0, 0, 0);
                
            // Disable player movement and interaction
            AC.KickStarter.player.enabled = false;
                
            currentMinigame = Instantiate(whackAMolePrefab, spawnPosition, Quaternion.identity);
            
            // Set up the minigame controller reference
            WhackAMoleGameIntegrated gameController = currentMinigame.GetComponent<WhackAMoleGameIntegrated>();
            if (gameController != null)
            {
                gameController.minigameTrigger = this;
            }
        }
        else
        {
            Debug.LogError("Whack-a-mole prefab not assigned!");
        }
    }
    
    // This method will be called by the WhackAMoleGame script when the game ends
    public void EndMinigame(bool won)
    {
        // Re-enable player movement and interaction
        AC.KickStarter.player.enabled = true;
        
        // Destroy the minigame
        if (currentMinigame != null)
        {
            Destroy(currentMinigame);
            currentMinigame = null;
        }
        
        // You could trigger different conversations based on whether the player won or lost
        if (won)
        {
            // Trigger "won" conversation
            Debug.Log("Player won the minigame!");
        }
        else
        {
            // Trigger "lost" conversation
            Debug.Log("Player lost the minigame!");
        }
    }
}