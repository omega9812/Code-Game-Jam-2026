using UnityEngine;
using AC;

namespace AC.SPUM
{
    public class AC_SPUM_Bridge : MonoBehaviour
    {
        private Player player;
        private SPUM_Prefabs spumPrefabs;
        private CharState lastState;
        private Transform unitRoot;

        private void OnEnable()
        {
            player = GetComponent<Player>();
            spumPrefabs = GetComponentInChildren<SPUM_Prefabs>();
            
            if (spumPrefabs != null)
            {
                spumPrefabs.OverrideControllerInit();
                unitRoot = spumPrefabs.transform;
            }
        }

        private void Update()
        {
            if (player == null || spumPrefabs == null) return;

            CharState currentState = player.charState;
            
            if (currentState != lastState)
            {
                UpdateAnimation(currentState);
                lastState = currentState;
            }

            UpdateFacing();
        }

        private void UpdateFacing()
        {
            if (unitRoot == null) return;

            // Only flip when moving or explicitly facing left/right
            string direction = player.spriteDirection;
            Vector3 targetScale = unitRoot.localScale;

            if (direction.Contains("R"))
            {
                targetScale.x = -1f;
            }
            else if (direction.Contains("L"))
            {
                targetScale.x = 1f;
            }
            UpdateAnimation(player.charState);

            if (Mathf.Abs(unitRoot.localScale.x - targetScale.x) > 0.01f)
            {
                unitRoot.localScale = targetScale;
                // Force animation update to prevent sluggishness when flipping
                UpdateAnimation(player.charState);
            }
        }

        private void UpdateAnimation(CharState state)
        {
            switch (state)
            {
                case CharState.Idle:
                    spumPrefabs.PlayAnimation(PlayerState.IDLE, 0);
                    break;

                case CharState.Move:
                    spumPrefabs.PlayAnimation(PlayerState.MOVE, 0);
                    break;

                // Add more states if needed (e.g., Talk, Custom)
            }
        }
    }
}
