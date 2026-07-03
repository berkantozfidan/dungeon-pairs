using System.Collections.Generic;
using UnityEngine;

namespace DungeonPairs.Combat
{
    public class FighterVisualFreezer : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Material freezeMaterial;

        private readonly Dictionary<Renderer, Material[]> originalMaterials = new();
        private void Awake()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>(true);
            }
        }
        public void SetFrozen(bool frozen)
        {
            if (frozen)
            {
                ApplyFreeze();
            }
            else
            {
                Restore();
            }
        }

        private void ApplyFreeze()
        {
            if (freezeMaterial == null)
            {
                Debug.LogWarning("FighterVisualFreezer has no freeze material.", this);
                return;
            }

            foreach (Renderer targetRenderer in renderers)
            {
                if (targetRenderer == null)
                {
                    continue;
                }

                if (!originalMaterials.ContainsKey(targetRenderer))
                {
                    originalMaterials[targetRenderer] = targetRenderer.materials;
                }

                Material[] frozenMaterials = new Material[targetRenderer.materials.Length];

                for (int i = 0; i < frozenMaterials.Length; i++)
                {
                    frozenMaterials[i] = freezeMaterial;
                }

                targetRenderer.materials = frozenMaterials;
            }
        }

        private void Restore()
        {
            foreach (var pair in originalMaterials)
            {
                if (pair.Key != null)
                {
                    pair.Key.materials = pair.Value;
                }
            }

            originalMaterials.Clear();
        }
    }
}