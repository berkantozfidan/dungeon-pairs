using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DungeonPairs.Audio;
using DungeonPairs.Cards;
using UnityEngine;

namespace DungeonPairs.Decks
{
    public class DeckController : MonoBehaviour
    {
        [SerializeField] private AudioController audioController;
        [SerializeField] private float shuffleMoveDuration = 0.45f;
        [SerializeField] private Ease shuffleEase = Ease.InOutQuad;
        public event Action DeckCompleted;
        private bool isDeckCompleted;
        [SerializeField] private CardView cardPrefab;
        [SerializeField] private Transform boardRoot;
        [SerializeField] private CardSelectionController selectionController;
        [SerializeField] private int columns = 4;
        [SerializeField] private Vector2 spacing = new Vector2(1f, 1.25f);

        private readonly List<CardView> activeCards = new();

        public IReadOnlyList<CardView> ActiveCards => activeCards;

       
        public void LoadDeck(DeckDefinition deck)
        {
            if (deck == null)
            {
                Debug.LogError("DeckController needs a deck definition.", this);
                return;
            }

            ClearDeck();
            isDeckCompleted = false;
            List<CardDefinition> shuffledCards = new(deck.Cards);
            Shuffle(shuffledCards);
            SpawnCards(shuffledCards);
        }

        public void ShuffleActiveCards()
        {
            List<CardView> movableCards = new();
            List<Vector3> positions = new();

            foreach (CardView card in activeCards)
            {
                if (card != null && !card.IsMatched)
                {
                    movableCards.Add(card);
                    positions.Add(card.transform.localPosition);
                }
            }

            Shuffle(positions);

            for (int i = 0; i < movableCards.Count; i++)
            {
                Transform cardTransform = movableCards[i].transform;

                cardTransform
                    .DOLocalMove(positions[i], shuffleMoveDuration)
                    .SetEase(shuffleEase);

                cardTransform
                    .DOPunchScale(Vector3.one * 0.12f, shuffleMoveDuration, vibrato: 6, elasticity: 0.6f);

                cardTransform
                    .DOLocalRotate(
                        new Vector3(0f, 0f, UnityEngine.Random.Range(-8f, 8f)),
                        shuffleMoveDuration * 0.5f)
                    .SetLoops(2, LoopType.Yoyo);
            }
        }
        public void RevealAllCards(bool playSound = false)
        {
            if (playSound)
            {
                audioController?.PlayCardFlip();
            }

            foreach (CardView card in activeCards)
            {
                if (card != null && !card.IsMatched)
                {
                    card.Reveal(false);
                }
            }
        }

        public void HideAllCards()
        {
            foreach (CardView card in activeCards)
            {
                if (card != null && !card.IsMatched)
                {
                    card.Hide();
                }
            }
        }

        private void SpawnCards(IReadOnlyList<CardDefinition> cards)
        {
            if (cardPrefab == null || boardRoot == null || selectionController == null)
            {
                Debug.LogError("DeckController references are missing.", this);
                return;
            }

            int rows = Mathf.CeilToInt(cards.Count / (float)columns);
            Vector2 startOffset = new Vector2(
                -(columns - 1) * spacing.x * 0.5f,
                (rows - 1) * spacing.y * 0.5f);

            for (int i = 0; i < cards.Count; i++)
            {
                int x = i % columns;
                int y = i / columns;

                Vector3 localPosition = new Vector3(
                    startOffset.x + x * spacing.x,
                    startOffset.y - y * spacing.y,
                    0f);

                CardView card = Instantiate(cardPrefab, boardRoot);
                card.transform.localPosition = localPosition;
                card.Initialize(cards[i], selectionController.SelectCard, audioController);
                card.Resolved += HandleCardResolved;
                activeCards.Add(card);
            }
        }
        private void HandleCardResolved(CardView resolvedCard)
        {
            if (isDeckCompleted)
            {
                return;
            }

            foreach (CardView card in activeCards)
            {
                if (card != null &&
                    !card.Definition.IsOptionalForDeckCompletion &&
                    !card.IsMatched)
                {
                    return;
                }
            }

            isDeckCompleted = true;
            DeckCompleted?.Invoke();
        }
        private void ClearDeck()
        {
            foreach (CardView card in activeCards)
            {
                if (card != null)
                {
                    card.Resolved -= HandleCardResolved;
                    Destroy(card.gameObject);
                }
            }

            activeCards.Clear();
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
        public void ApplyFreezeMaterialToCards(Material freezeMaterial)
        {
            foreach (CardView card in activeCards)
            {
                if (card != null)
                {
                    card.ApplyFreezeMaterial(freezeMaterial);
                }
            }
        }

        public void RestoreCardMaterials()
        {
            foreach (CardView card in activeCards)
            {
                if (card != null)
                {
                    card.RestoreOriginalMaterial();
                }
            }
        }

        public void RevealRandomCardsTemporarily(int count, float duration)
        {
            audioController?.PlayScoutReveal();
            StartCoroutine(RevealRandomCardsRoutine(count, duration));
        }
        private IEnumerator RevealRandomCardsRoutine(int count, float duration)
        {
            List<CardView> hiddenCards = new();

            foreach (CardView card in activeCards)
            {
                if (card != null && !card.IsMatched && !card.IsRevealed)
                {
                    hiddenCards.Add(card);
                }
            }

            Shuffle(hiddenCards);

            int revealCount = Mathf.Min(count, hiddenCards.Count);
            List<CardView> revealedCards = new();

            for (int i = 0; i < revealCount; i++)
            {
                CardView card = hiddenCards[i];
                card.Reveal(false);
                revealedCards.Add(card);
            }

            yield return new WaitForSeconds(duration);

            foreach (CardView card in revealedCards)
            {
                if (card != null && !card.IsMatched)
                {
                    card.Hide();
                }
            }
        }
    }
}