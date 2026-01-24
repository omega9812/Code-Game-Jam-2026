using UnityEngine;
using AC;

public class ClownConversationTracker : MonoBehaviour
{
    [Header("Conversation Tracking")]
    public bool hasSpokenBefore = false;
    
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
        // Check if this conversation involves the clown
        // Since we can't easily check the conversation participants,
        // we'll check if the player is close to this clown when the conversation ends
        if (KickStarter.player != null)
        {
            float distance = Vector3.Distance(transform.position, KickStarter.player.transform.position);
            if (distance < 3f) // Assuming 3 units is close enough
            {
                // Check if this is the second conversation
                if (hasSpokenBefore)
                {
                    // Find the WhackAMoleMinigameManager
                    WhackAMoleMinigameManager manager = FindObjectOfType<WhackAMoleMinigameManager>();
                    if (manager != null)
                    {
                        // Launch the minigame
                        manager.StartCoroutine(manager.LaunchMinigame());
                        Debug.Log("Launching whack-a-mole minigame after second conversation");
                    }
                }
                else
                {
                    // First conversation
                    hasSpokenBefore = true;
                    Debug.Log("Clown conversation completed, hasSpokenBefore set to true");
                }
            }
        }
    }
}