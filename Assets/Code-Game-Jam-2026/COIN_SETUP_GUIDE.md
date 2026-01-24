# Coin Pickup and NPC Interaction Setup Guide

## Overview
This guide explains how to complete the Adventure Creator setup for the coin pickup and Brody NPC interaction system.

## What's Already Done ✓

1. **Coin GameObject Created**
   - Position: (2, 0, 0) in the scene
   - Has a gold coin sprite
   - Has Hotspot component configured
   - Has Interaction component with two actions:
     - Action 1: Adds coin to inventory
     - Action 2: Removes coin from scene

2. **Inventory Item**
   - "Coin" item already exists in the Inventory Manager (ID: 0)

3. **Conversation Structure**
   - Brody's conversation has two dialogue options set up:
     - Option 1: "Hello there!"
     - Option 2: "I have a coin for you"

## Manual Steps Required in Adventure Creator

### Step 1: Configure Brody's Dialogue Responses

1. **Open the Scene** and select "Brody le Barbaman: Talk to" GameObject
2. **In the Dialog component**, you need to add speech responses:
   - Click on the Dialog component
   - Add speeches for each dialogue option
   - For Option 0 (Hello): Add Brody's greeting response
   - For Option 1 (Coin): Add Brody's thank you response

### Step 2: Add Inventory Check for Coin Option

To make the "I have a coin for you" option only appear when the player has the coin:

1. **Select "Brody le Barbaman: Talk to"** GameObject
2. **In the Conversation component**:
   - Find Option 1 ("I have a coin for you")
   - Click on the option to expand it
   - Look for "Condition" or "Visibility" settings
   - Add an **Inventory Check** condition:
     - Check if player has "Coin" (ID: 0)
     - Count should be >= 1

### Step 3: Add Action to Remove Coin When Given to Brody

1. **Still in the Conversation component**, for Option 1:
   - Add an **ActionList** that runs when this option is selected
   - In this ActionList, add:
     - **Action: Inventory > Add or remove**
       - Action: Remove
       - Item: Coin (ID: 0)
       - Amount: 1

### Step 4: Test the System

1. **Enter Play Mode**
2. **Walk to the coin** and click on it
   - The coin should be added to your inventory
   - The coin should disappear from the scene
3. **Walk to Brody** and click to talk
   - You should see both dialogue options
   - The "I have a coin for you" option should only be visible if you have the coin
4. **Select the coin option**
   - Brody should respond
   - The coin should be removed from your inventory

## Alternative: Using Adventure Creator's Visual Editor

If you prefer to use Adventure Creator's built-in editors:

### For Dialogue Setup:
1. Go to **Adventure Creator > Editors > Conversation Editor**
2. Select "Brody le Barbaman: Talk to"
3. Use the visual editor to:
   - Add dialogue text
   - Set up conditions
   - Configure actions

### For Inventory Checks:
1. Go to **Adventure Creator > Editors > Inventory Manager**
2. Verify the Coin item is configured correctly
3. You can add custom interactions here if needed

## Troubleshooting

**Coin doesn't disappear when clicked:**
- Check that the Interaction component has both actions (Add to inventory + Remove object)
- Verify the Hotspot's "Use" button is linked to the Interaction

**Dialogue option always visible:**
- Make sure you've added the Inventory Check condition to Option 1
- Verify the condition checks for Coin ID: 0 with count >= 1

**Coin not added to inventory:**
- Check the Inventory Manager has the Coin item (ID: 0)
- Verify the ActionInventorySet action in the coin's Interaction is configured correctly

## Files Modified/Created

- `Assets/Code-Game-Jam-2026/Sprites/Coin.png` - Coin sprite image
- `Assets/Code-Game-Jam-2026/Scripts/Editor/SetupCoinAndBrody.cs` - Setup script (can be deleted after use)
- Scene: "New Scene" - Contains the Coin GameObject

## Next Steps

After completing the manual steps above, you can extend this system by:
- Adding more dialogue options based on inventory items
- Creating multiple coins or collectibles
- Adding sound effects for coin pickup
- Creating a UI display for the inventory
