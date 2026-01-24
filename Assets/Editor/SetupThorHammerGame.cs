using UnityEngine;
using UnityEditor;
using AC;
using System.Collections.Generic;

public class SetupThorHammerGame : MonoBehaviour
{
    [MenuItem("Tools/Setup Thor Hammer Game")]
    public static void SetupThorConversation()
    {
        // Find Thor's interaction
        GameObject thorTalkTo = GameObject.Find("Thor: Talk to");
        if (thorTalkTo == null)
        {
            Debug.LogError("Could not find 'Thor: Talk to' GameObject!");
            return;
        }

        Interaction thorInteraction = thorTalkTo.GetComponent<Interaction>();
        if (thorInteraction == null)
        {
            Debug.LogError("Thor: Talk to does not have an Interaction component!");
            return;
        }

        // Find the UIHammerGameManager
        UIHammerStrengthGame hammerGame = GameObject.FindFirstObjectByType<UIHammerStrengthGame>();
        if (hammerGame == null)
        {
            Debug.LogError("Could not find UIHammerStrengthGame in the scene!");
            return;
        }

        // Clear existing actions
        thorInteraction.actions.Clear();

        // Find Thor character
        GameObject thorObject = GameObject.Find("Thor");
        AC.Char thorChar = null;
        if (thorObject != null)
        {
            thorChar = thorObject.GetComponent<AC.Char>();
        }

        // Create dialogue action
        ActionSpeech speechAction = ScriptableObject.CreateInstance<ActionSpeech>();
        speechAction.isPlayer = false;
        speechAction.messageText = "Greetings, mortal! Think you have the strength to ring the bell? Show me what you've got!";
        speechAction.isBackground = false;
        speechAction.waitTimeOffset = 0f;
        if (thorChar != null)
        {
            speechAction.speaker = thorChar;
        }
        
        // Create hammer game action
        ActionHammerGame hammerGameAction = ActionHammerGame.CreateNew(hammerGame, true);
        
        // Create result dialogue actions (for future use)
        ActionSpeech winSpeech = ScriptableObject.CreateInstance<ActionSpeech>();
        winSpeech.isPlayer = false;
        winSpeech.messageText = "By Odin's beard! You've got the strength of a true warrior!";
        if (thorChar != null)
        {
            winSpeech.speaker = thorChar;
        }
        
        ActionSpeech loseSpeech = ScriptableObject.CreateInstance<ActionSpeech>();
        loseSpeech.isPlayer = false;
        loseSpeech.messageText = "Ha! You'll need more training before you can match the might of Thor!";
        if (thorChar != null)
        {
            loseSpeech.speaker = thorChar;
        }

        // Add actions to the interaction
        thorInteraction.actions.Add(speechAction);
        thorInteraction.actions.Add(hammerGameAction);
        
        // Note: For now, we'll just add the hammer game action
        // In a full implementation, you'd use ActionCheck to check if the player won
        // and branch to different dialogues
        
        Debug.Log("Thor's hammer game conversation has been set up!");
        
        // Mark the scene as dirty so changes are saved
        EditorUtility.SetDirty(thorInteraction);
        EditorUtility.SetDirty(thorTalkTo);
    }
}