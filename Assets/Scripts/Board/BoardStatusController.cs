using System;
using UnityEngine;

namespace DungeonPairs.Board
{
    public class BoardStatusController : MonoBehaviour
    {
        private bool isFrozen;
        private float inputSpeedMultiplier = 1f;

        public event Action StateChanged;

        public bool IsFrozen => isFrozen;
        public float InputSpeedMultiplier => inputSpeedMultiplier;
        public bool CanSelectCards => !isFrozen;

        public void ApplyFreeze(float duration, float speedMultiplier)
        {
            isFrozen = true;
            inputSpeedMultiplier = Mathf.Clamp(speedMultiplier, 0.1f, 1f);
            StateChanged?.Invoke();

            CancelInvoke(nameof(ClearFreeze));
            Invoke(nameof(ClearFreeze), duration);
        }

        private void ClearFreeze()
        {
            isFrozen = false;
            inputSpeedMultiplier = 1f;
            StateChanged?.Invoke();
        }
    }
}