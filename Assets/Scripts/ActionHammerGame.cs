/*
 *  ActionHammerGame.cs
 *  
 *  Custom Adventure Creator action to start the Hammer Strength minigame.
 *  Use this action in Thor's conversation to trigger the high striker game.
 */

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{
    [System.Serializable]
    public class ActionHammerGame : Action
    {
        public UIHammerStrengthGame hammerGame;
        public int hammerGameConstantID = 0;
        public int hammerGameParameterID = -1;
        
        public bool waitUntilComplete = true;
        
        private bool isWaiting = false;
        private bool gameCompleted = false;
        private bool gameWon = false;
        
        public override ActionCategory Category { get { return ActionCategory.Custom; } }
        public override string Title { get { return "Start Hammer Game"; } }
        public override string Description { get { return "Starts the Hammer Strength minigame (High Striker). The player must spam click to fill the meter before time runs out."; } }
        
        
        public override void AssignValues(List<ActionParameter> parameters)
        {
            hammerGame = AssignFile<UIHammerStrengthGame>(parameters, hammerGameParameterID, hammerGameConstantID, hammerGame);
            
            // Try to find the hammer game if not assigned
            if (hammerGame == null)
            {
                hammerGame = GameObject.FindFirstObjectByType<UIHammerStrengthGame>();
            }
        }
        
        
        public override float Run()
        {
            if (hammerGame == null)
            {
                LogWarning("No UIHammerStrengthGame found in scene!");
                return 0f;
            }
            
            if (!isRunning)
            {
                isRunning = true;
                isWaiting = true;
                gameCompleted = false;
                
                // Subscribe to game events
                hammerGame.onGameComplete += OnGameComplete;
                hammerGame.onGameWin += OnGameWin;
                hammerGame.onGameFail += OnGameFail;
                
                // Start the game
                hammerGame.StartGame();
                
                if (waitUntilComplete)
                {
                    return defaultPauseTime;
                }
                else
                {
                    return 0f;
                }
            }
            else
            {
                // Check if game is complete
                if (gameCompleted || !isWaiting)
                {
                    isRunning = false;
                    return 0f;
                }
                else
                {
                    return defaultPauseTime;
                }
            }
        }
        
        
        private void OnGameComplete()
        {
            gameCompleted = true;
            isWaiting = false;
            
            // Unsubscribe from events
            if (hammerGame != null)
            {
                hammerGame.onGameComplete -= OnGameComplete;
                hammerGame.onGameWin -= OnGameWin;
                hammerGame.onGameFail -= OnGameFail;
            }
        }
        
        private void OnGameWin()
        {
            gameWon = true;
        }
        
        private void OnGameFail()
        {
            gameWon = false;
        }
        
        
        public override void Skip()
        {
            if (hammerGame != null)
            {
                hammerGame.onGameComplete -= OnGameComplete;
                hammerGame.onGameWin -= OnGameWin;
                hammerGame.onGameFail -= OnGameFail;
                hammerGame.EndGame();
            }
            
            isRunning = false;
            isWaiting = false;
            gameCompleted = true;
        }
        
        
        #if UNITY_EDITOR
        
        public override void ShowGUI(List<ActionParameter> parameters)
        {
            hammerGameParameterID = Action.ChooseParameterGUI("Hammer Game:", parameters, hammerGameParameterID, ParameterType.GameObject);
            if (hammerGameParameterID >= 0)
            {
                hammerGameConstantID = 0;
                hammerGame = null;
            }
            else
            {
                hammerGame = (UIHammerStrengthGame)EditorGUILayout.ObjectField("Hammer Game:", hammerGame, typeof(UIHammerStrengthGame), true);
                
                hammerGameConstantID = FieldToID<UIHammerStrengthGame>(hammerGame, hammerGameConstantID);
                hammerGame = IDToField<UIHammerStrengthGame>(hammerGame, hammerGameConstantID, false);
            }
            
            waitUntilComplete = EditorGUILayout.Toggle("Wait until complete?", waitUntilComplete);
        }
        
        
        public override string SetLabel()
        {
            if (hammerGame != null)
            {
                return hammerGame.name;
            }
            return string.Empty;
        }
        
        #endif
        
        
        /**
         * <summary>Creates a new instance of the 'Custom: Start Hammer Game' Action</summary>
         * <param name = "hammerGameObject">The UIHammerStrengthGame to start</param>
         * <param name = "waitUntilFinish">If True, the Action will wait until the game is complete</param>
         * <returns>The generated Action</returns>
         */
        public static ActionHammerGame CreateNew(UIHammerStrengthGame hammerGameObject, bool waitUntilFinish = true)
        {
            ActionHammerGame newAction = CreateInstance<ActionHammerGame>();
            newAction.hammerGame = hammerGameObject;
            newAction.waitUntilComplete = waitUntilFinish;
            return newAction;
        }
    }
}