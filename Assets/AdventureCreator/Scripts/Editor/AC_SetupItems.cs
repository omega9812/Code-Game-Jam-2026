using UnityEngine;
using UnityEditor;
using AC;
using System.Collections.Generic;

public class AC_SetupItems
{
    public static void SetupCoinAndBrodyCottonCandy()
    {
        // 1. Get Managers
        InventoryManager invManager = AssetDatabase.LoadAssetAtPath<InventoryManager>("Assets/Code-Game-Jam-2026/Managers/Code-Game-Jam-2026_InventoryManager.asset");
        if (invManager == null)
        {
            Debug.LogError("Inventory Manager not found!");
            return;
        }

        // 2. Create Inventory Items
        InvItem coinItem = CreateItem(invManager, "Coin", "A shiny gold coin.");
        InvItem cottonCandyItem = CreateItem(invManager, "Cotton Candy", "Sweet and fluffy.");
        EditorUtility.SetDirty(invManager);
        AssetDatabase.SaveAssets();

        // 3. Create Coin Hotspot in Scene
        GameObject coinObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        coinObj.name = "Coin";
        coinObj.transform.position = new Vector3(2f, 0.5f, 0f);
        coinObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.1f);
        
        Hotspot hotspot = coinObj.AddComponent<Hotspot>();
        hotspot.hotspotName = "Coin";
        
        // 4. Create Interaction ActionList for taking the coin
        ActionListAsset takeCoinAL = ScriptableObject.CreateInstance<ActionListAsset>();
        takeCoinAL.name = "Take Coin";
        AssetDatabase.CreateAsset(takeCoinAL, "Assets/Code-Game-Jam-2026/TakeCoin.asset");

        // Add Actions to ActionList
        // Action 1: Add Item (Using ActionInventorySet instead of ActionInventoryAdd)
        ActionInventorySet addAction = ActionInventorySet.CreateNew_Add(coinItem.id);
        takeCoinAL.actions.Add(addAction);
        
        // Action 2: Object Visibility (Hide Coin)
        ActionVisible hideAction = ActionVisible.CreateNew(coinObj, VisState.Invisible);
        takeCoinAL.actions.Add(hideAction);

        // Action 3: Dialogue
        ActionSpeech speechAction = ScriptableObject.CreateInstance<ActionSpeech>();
        speechAction.messageText = "Picked up a coin!";
        takeCoinAL.actions.Add(speechAction);

        EditorUtility.SetDirty(takeCoinAL);
        AssetDatabase.SaveAssets();
        
        // Assign Interaction to Hotspot
        AC.Button useButton = new AC.Button();
        useButton.assetFile = takeCoinAL;
        hotspot.useButtons.Add(useButton);

        // 5. Update Brody's Conversation
        GameObject brodyTalk = GameObject.Find("Brody le Barbaman: Talk to");
        if (brodyTalk != null)
        {
            Conversation conv = brodyTalk.GetComponent<Conversation>();
            if (conv == null) conv = brodyTalk.AddComponent<Conversation>();

            // Create a new option
            ButtonDialog newOption = new ButtonDialog(new int[] { conv.options.Count });
            newOption.label = "Buy Cotton Candy (1 Coin)";
            conv.options.Add(newOption);
            EditorUtility.SetDirty(conv);
        }

        Debug.Log("AC Setup Complete: Coin created and Brody updated.");
    }

    private static InvItem CreateItem(InventoryManager manager, string name, string label)
    {
        foreach (InvItem existing in manager.items)
        {
            if (existing.label == name) return existing;
        }

        int id = 0;
        foreach (InvItem item in manager.items)
        {
            if (item.id >= id) id = item.id + 1;
        }

        InvItem newItem = new InvItem(id);
        newItem.label = name;
        newItem.altLabel = label;
        manager.items.Add(newItem);
        return newItem;
    }
}
