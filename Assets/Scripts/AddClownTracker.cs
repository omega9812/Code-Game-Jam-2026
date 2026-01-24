using UnityEngine;

public class AddClownTracker : MonoBehaviour
{
    public static void Execute()
    {
        // Find the clown object
        GameObject clownObject = GameObject.Find("Clown");
        if (clownObject == null)
        {
            Debug.LogError("Clown object not found in the scene!");
            return;
        }
        
        // Add the ClownConversationTracker component
        ClownConversationTracker tracker = clownObject.AddComponent<ClownConversationTracker>();
        
        Debug.Log("ClownConversationTracker added to the Clown object");
    }
}