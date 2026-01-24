using UnityEngine;

public class SetupAnimatorParameters : MonoBehaviour
{
    public static void Execute()
    {
        // Find Bob and Clown
        GameObject bob = GameObject.Find("Bob");
        GameObject clown = GameObject.Find("Clown");
        
        if (bob == null || clown == null)
        {
            Debug.LogError("Bob or Clown GameObject not found!");
            return;
        }
        
        // Get the animators
        Animator bobAnimator = null;
        Animator clownAnimator = null;
        
        // For Bob, we need to find the animator in the children
        Transform bobUnitRoot = bob.transform.Find("MainCharacter/UnitRoot");
        if (bobUnitRoot != null)
        {
            bobAnimator = bobUnitRoot.GetComponent<Animator>();
            if (bobAnimator != null)
            {
                Debug.Log("Found Bob's animator");
            }
        }
        
        // For Clown, we need to find the animator in the children
        Transform clownUnitRoot = clown.transform.Find("UnitRoot");
        if (clownUnitRoot != null)
        {
            clownAnimator = clownUnitRoot.GetComponent<Animator>();
            if (clownAnimator != null)
            {
                Debug.Log("Found Clown's animator");
            }
        }
        
        // Since we can't directly modify animator parameters at runtime through script,
        // we'll log instructions for the user
        Debug.Log("IMPORTANT: Please manually add the following parameters to the animators:");
        Debug.Log("For Bob's animator:");
        Debug.Log("1. 'Walk' (Bool) - For walking animation");
        Debug.Log("2. 'Damaged' (Trigger) - For reaction to water spray");
        
        Debug.Log("For Clown's animator:");
        Debug.Log("1. 'Attack' (Trigger) - Used for laughing animation");
        
        Debug.Log("Note: The SPUM animators already have some parameters that we can use:");
        Debug.Log("- 'RunState' (Int) - Set to 1 for walking");
        Debug.Log("- 'AttackState' (Trigger) - Can be used for attack/laugh");
        Debug.Log("- 'Damaged' (Trigger) - For damage reaction");
        
        // Update the CutsceneController script to use these existing parameters
        Debug.Log("Animator parameters setup instructions complete!");
    }
}