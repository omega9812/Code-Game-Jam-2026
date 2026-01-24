using UnityEngine;
using UnityEditor;
using AC;
using System.Collections.Generic;

public class SetupCoinAndBrody
{
    public static void Execute()
    {
        SetupCoin();
        SetupBrodyConversation();
        Debug.Log("Setup complete! Coin and Brody conversation configured.");
    }

    private static void SetupCoin()
    {
        GameObject coinObject = GameObject.Find("Coin");
        if (coinObject == null)
        {
            Debug.LogError("Coin GameObject not found!");
            return;
        }

        // Configure Hotspot
        Hotspot coinHotspot = coinObject.GetComponent<Hotspot>();
        if (coinHotspot != null)
        {
            coinHotspot.hotspotName = "Coin";
            coinHotspot.highlight = null;
            
            // Clear and setup use buttons
            coinHotspot.useButtons.Clear();
            AC.Button useButton = new AC.Button();
            useButton.playerAction = PlayerAction.DoNothing;
            coinHotspot.useButtons.Add(useButton);
        }

        // Configure Interaction
        Interaction coinInteraction = coinObject.GetComponent<Interaction>();
        if (coinInteraction != null)
        {
            coinInteraction.source = ActionListSource.InScene;
            coinInteraction.actions.Clear();

            // Action 1: Add coin to inventory
            ActionInventorySet addCoinAction = ScriptableObject.CreateInstance<ActionInventorySet>();
            addCoinAction.invAction = InvAction.Add;
            addCoinAction.invID = 0; // Coin ID
            addCoinAction.setAmount = true;
            addCoinAction.amount = 1;
            coinInteraction.actions.Add(addCoinAction);

            // Action 2: Remove the coin GameObject from scene
            ActionInstantiate removeAction = ScriptableObject.CreateInstance<ActionInstantiate>();
            removeAction.invAction = InvAction.Remove;
            removeAction.gameObject = coinObject;
            coinInteraction.actions.Add(removeAction);

            // Link interaction to hotspot
            if (coinHotspot != null && coinHotspot.useButtons.Count > 0)
            {
                coinHotspot.useButtons[0].interaction = coinInteraction;
            }
        }

        EditorUtility.SetDirty(coinObject);
        Debug.Log("Coin setup complete!");
    }

    private static void SetupBrodyConversation()
    {
        GameObject brodyConversation = GameObject.Find("Brody le Barbaman: Talk to");
        if (brodyConversation == null)
        {
            Debug.LogError("Brody conversation GameObject not found!");
            return;
        }

        Conversation conversation = brodyConversation.GetComponent<Conversation>();
        
        if (conversation == null)
        {
            Debug.LogError("Conversation component not found!");
            return;
        }

        // Clear existing options
        conversation.options.Clear();

        // Option 1: Default greeting (always available)
        ButtonDialog option1 = new ButtonDialog(new int[] { 0 });
        option1.label = "Hello there!";
        option1.isOn = true;
        conversation.options.Add(option1);

        // Option 2: Give coin (only when player has coin)
        ButtonDialog option2 = new ButtonDialog(new int[] { 1 });
        option2.label = "I have a coin for you";
        option2.isOn = true;
        conversation.options.Add(option2);

        EditorUtility.SetDirty(brodyConversation);
        Debug.Log("Brody conversation setup complete! You'll need to manually configure the dialog responses and inventory checks in the Adventure Creator editor.");
    }
}
