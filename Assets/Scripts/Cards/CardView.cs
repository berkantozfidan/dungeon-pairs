using System;
using System.Collections.Generic;
using DG.Tweening;
using DungeonPairs.Audio;
using TMPro;
using UnityEngine;

namespace DungeonPairs.Cards
{
    [RequireComponent(typeof(Collider))]
    public class CardView : MonoBehaviour
    {
        [SerializeField] private float flipDuration = 0.25f;
        [SerializeField] private Ease flipEase = Ease.OutQuad;
        [SerializeField] private Vector3 hiddenRotation = new Vector3(0, 90f, 0f);
        [SerializeField] private Vector3 revealedRotation = new Vector3(0f, -90f, 0f);
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Transform cardVisualRoot;
        [SerializeField] private Transform revealedContentRoot;
        [SerializeField] private Renderer revealedBackingRenderer;
        private AudioController audioController;

        public event Action<CardView> Resolved;
        private CardDefinition definition;
        private Action<CardView> clicked;
        private bool isRevealed;
        private bool isMatched;
        private GameObject revealedContentInstance;
        private Tween flipTween;
        private Tween feedbackTween;

        public CardDefinition Definition => definition;
        public bool IsRevealed => isRevealed;
        public bool IsMatched => isMatched;
        private readonly Dictionary<Renderer, Material[]> originalMaterials = new();
        private void Awake()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            SetHiddenVisual();
        }

        private void OnMouseDown()
        {
            if (isRevealed || isMatched)
            {
                return;
            }

            clicked?.Invoke(this);
        }

        public void Initialize(CardDefinition cardDefinition, Action<CardView> onClicked, AudioController audio)
        {
            definition = cardDefinition;
            clicked = onClicked;
            isRevealed = false;
            isMatched = false;
            audioController = audio;
            CreateRevealedContent();
            ApplyBackingMaterial();
            SetHiddenVisual();
        }
        private void ApplyBackingMaterial()
        {
            if (revealedBackingRenderer == null || definition == null)
            {
                return;
            }

            if (definition.RevealedBackingMaterial == null)
            {
                return;
            }

            revealedBackingRenderer.material = definition.RevealedBackingMaterial;
        }
        private void SetFlipRotation(Vector3 rotation, bool instant = false)
        {
            if (cardVisualRoot == null)
            {
                return;
            }

            flipTween?.Kill();

            if (instant)
            {
                cardVisualRoot.localRotation = Quaternion.Euler(rotation);
                return;
            }

            flipTween = cardVisualRoot
                .DOLocalRotate(rotation, flipDuration)
                .SetEase(flipEase);
        }

        public void Reveal(bool playSound = true)
        {
            if (playSound)
            {
                audioController?.PlayCardFlip();
            }

            isRevealed = true;

            if (label != null)
            {
                label.text = definition.DisplayName;
            }

            SetFlipRotation(revealedRotation);
        }

        public void Hide()
        {
            if (isMatched)
            {
                return;
            }

            isRevealed = false;
            SetHiddenVisual();
        }

        public void MarkMatched()
        {
            isMatched = true;
            isRevealed = true;
            SetFlipRotation(revealedRotation);
            Resolved?.Invoke(this);
        }

        public void MarkUsed()
        {
            isMatched = true;
            isRevealed = true;
            Resolved?.Invoke(this);
        }

        private void SetHiddenVisual()
        {
            if (label != null)
            {
                label.text = "?";
            }
            SetFlipRotation(hiddenRotation);
        }

        private void CreateRevealedContent()
        {
            if (revealedContentInstance != null)
            {
                Destroy(revealedContentInstance);
            }

            if (definition == null || definition.RevealedPrefab == null || revealedContentRoot == null)
            {
                return;
            }

            revealedContentInstance = Instantiate(definition.RevealedPrefab, revealedContentRoot);
            revealedContentInstance.transform.localPosition = Vector3.zero;
            revealedContentInstance.transform.localRotation = Quaternion.identity;
        }

        public void PlayMatchedFeedback()
        {
            if (cardVisualRoot == null)
            {
                return;
            }

            feedbackTween?.Kill();

            Vector3 startScale = Vector3.one;
            Vector3 startPosition = cardVisualRoot.localPosition;

            cardVisualRoot.localScale = startScale;
            cardVisualRoot.localPosition = startPosition;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(cardVisualRoot.DOPunchScale(
                Vector3.one * 0.25f,
                0.45f,
                vibrato: 8,
                elasticity: 0.8f));

            sequence.Join(cardVisualRoot.DOLocalMoveY(
                startPosition.y + 0.12f,
                0.18f)
                .SetLoops(2, LoopType.Yoyo));

            sequence.Append(cardVisualRoot.DOScale(
                Vector3.one * 0.9f,
                0.2f));

            feedbackTween = sequence;
        }

        public void PlayMismatchFeedback()
        {
            if (cardVisualRoot == null)
            {
                return;
            }

            feedbackTween?.Kill();

            Vector3 startPosition = cardVisualRoot.localPosition;
            cardVisualRoot.localPosition = startPosition;

            feedbackTween = cardVisualRoot
                .DOShakePosition(
                    duration: 0.35f,
                    strength: new Vector3(0.08f, 0f, 0f),
                    vibrato: 12,
                    randomness: 0f)
                .OnComplete(() =>
                {
                    cardVisualRoot.localPosition = startPosition;
                });
        }

        public void ApplyFreezeMaterial(Material freezeMaterial)
        {
            if (freezeMaterial == null)
            {
                return;
            }

            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

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

        public void RestoreOriginalMaterial()
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

        private void OnDestroy()
        {
            flipTween?.Kill();
            feedbackTween?.Kill();
        }
    }
}