using UnityEngine;

public class SetupAnimations : MonoBehaviour
{
    public static void Execute()
    {
        // Find Bob and Clown animators
        GameObject bob = GameObject.Find("Bob");
        GameObject clown = GameObject.Find("Clown");
        
        if (bob == null || clown == null)
        {
            Debug.LogError("Bob or Clown GameObject not found!");
            return;
        }
        
        // Get animators
        Animator bobAnimator = bob.GetComponentInChildren<Animator>();
        Animator clownAnimator = clown.GetComponentInChildren<Animator>();
        
        if (bobAnimator == null || clownAnimator == null)
        {
            Debug.LogError("Animator component not found on Bob or Clown!");
            return;
        }
        
        // Set up animator parameters for Bob
        // These parameters will be used in the CutsceneManager script
        
        // Check if parameters already exist to avoid duplicates
        foreach (AnimatorControllerParameter param in bobAnimator.parameters)
        {
            if (param.name == "isWalking" || param.name == "damaged")
            {
                Debug.Log("Parameters already exist on Bob's animator");
                return;
            }
        }
        
        // Since we can't directly add parameters to an animator at runtime,
        // we'll log instructions for the user to add them manually
        Debug.Log("Please add the following parameters to Bob's animator:");
        Debug.Log("1. 'isWalking' (Bool)");
        Debug.Log("2. 'damaged' (Trigger)");
        
        Debug.Log("Please add the following parameters to Clown's animator:");
        Debug.Log("1. 'laugh' (Trigger)");
        
        // Position the characters
        GameObject bobStartPosition = GameObject.Find("BobStartPosition");
        GameObject clownHidingPosition = GameObject.Find("ClownHidingPosition");
        
        if (bobStartPosition != null)
        {
            bob.transform.position = bobStartPosition.transform.position;
        }
        
        if (clownHidingPosition != null)
        {
            clown.transform.position = clownHidingPosition.transform.position;
        }
        
        Debug.Log("Animation setup complete!");
    }
}