using System;
using DG.Tweening;
using DungeonPairs.Audio;
using DungeonPairs.Decks;
using UnityEngine;

namespace DungeonPairs.Board
{
    public class BoardVisualController : MonoBehaviour
    {
        [SerializeField] private AudioController audioController;
        [SerializeField] private Transform boardArtRoot;
        [SerializeField] private GameObject nextDeckPreview;
        [SerializeField] private float deckTransitionDuration = 0.35f;
        [SerializeField] private Vector3 deckTransitionOffset = new Vector3(-0.35f, 0f, 0f);
        private Tween deckTransitionTween;
        [SerializeField] private DeckController deckController;
        [SerializeField] private Material cardFreezeMaterial;
        [SerializeField] private Renderer boardFreezeRenderer;
        private Material boardOriginalMaterial;
        private bool hasBoardOriginalMaterial;
        [SerializeField] private Material boardFreezeMaterial;

        [SerializeField] private float shuffleShakeDuration = 0.25f;
        [SerializeField] private Vector3 shuffleShakeStrength = new Vector3(5f, 5f, 0f);
        private void ApplyFreezeMaterialToBoard()
        {
            if (boardFreezeRenderer == null || boardFreezeMaterial == null)
            {
                return;
            }

            if (!hasBoardOriginalMaterial)
            {
                boardOriginalMaterial = boardFreezeRenderer.material;
                hasBoardOriginalMaterial = true;
            }

            boardFreezeRenderer.material = boardFreezeMaterial;
        }

        private void RestoreBoardMaterial()
        {
            if (boardFreezeRenderer == null || !hasBoardOriginalMaterial)
            {
                return;
            }

            boardFreezeRenderer.material = boardOriginalMaterial;
            boardOriginalMaterial = null;
            hasBoardOriginalMaterial = false;
        }
        public void SetNextDeckPreviewVisible(bool visible)
        {
            if (nextDeckPreview != null)
            {
                nextDeckPreview.SetActive(visible);
            }
        }
        public void PlayDeckAdvanceTransition(Action onSwap, bool hasNextDeckAfterSwap)
        {
            if (boardArtRoot == null)
            {
                onSwap?.Invoke();
                SetNextDeckPreviewVisible(hasNextDeckAfterSwap);
                return;
            }

            deckTransitionTween?.Kill();

            Vector3 startPosition = boardArtRoot.localPosition;
            Vector3 startScale = boardArtRoot.localScale;
            Quaternion startRotation = boardArtRoot.localRotation;

            Sequence sequence = DOTween.Sequence();

            
            sequence.Append(boardArtRoot
                .DOLocalMove(startPosition + new Vector3(-0.45f, 0.12f, -0.15f),
                    deckTransitionDuration)
                .SetEase(Ease.InQuad));

            sequence.Join(boardArtRoot
                .DOLocalRotate(new Vector3(0f, 0f, 4f),
                    deckTransitionDuration));

            sequence.Join(boardArtRoot
                .DOScale(startScale * 0.92f,
                    deckTransitionDuration));

            
            sequence.AppendCallback(() =>
            {
                onSwap?.Invoke();
                SetNextDeckPreviewVisible(hasNextDeckAfterSwap);

                boardArtRoot.localPosition =
                    startPosition + new Vector3(0.35f, -0.08f, 0.15f);

                boardArtRoot.localRotation =
                    Quaternion.Euler(0f, 0f, -3f);

                boardArtRoot.localScale = startScale * 0.94f;
            });

            
            sequence.Append(boardArtRoot
                .DOLocalMove(startPosition, deckTransitionDuration * 1.2f)
                .SetEase(Ease.OutBack, 1.3f));

            sequence.Join(boardArtRoot
                .DOLocalRotateQuaternion(startRotation,
                    deckTransitionDuration));

            sequence.Join(boardArtRoot
                .DOScale(startScale,
                    deckTransitionDuration));

            deckTransitionTween = sequence;
        }
        public void PlayFreeze(float duration)
        {
            deckController?.ApplyFreezeMaterialToCards(cardFreezeMaterial);
            ApplyFreezeMaterialToBoard();

            CancelInvoke(nameof(HideFreezeVisuals));
            Invoke(nameof(HideFreezeVisuals), duration);
        }
        private void HideFreezeVisuals()
        {
            deckController?.RestoreCardMaterials();
            RestoreBoardMaterial();

        }

        public void PlayShuffle()
        {
            Debug.Log("Board shuffle visual.", this);
            audioController?.PlayShuffle();
            if (boardArtRoot == null)
            {
                return;
            }

            boardArtRoot.DOKill();

            Vector3 startPosition = boardArtRoot.localPosition;

            boardArtRoot
                .DOShakePosition(
                    shuffleShakeDuration,
                    shuffleShakeStrength,
                    vibrato: 12,
                    randomness: 0f)
                .OnComplete(() =>
                {
                    boardArtRoot.localPosition = startPosition;
                });
        }

        private void OnDestroy()
        {
            deckTransitionTween?.Kill();
        }
    }
}