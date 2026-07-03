using System.Collections;
using System.Collections.Generic;
using DungeonPairs.Audio;
using DungeonPairs.UI;
using UnityEngine;

namespace DungeonPairs.Cards
{
    public class ComboController : MonoBehaviour
    {
        [SerializeField] private AudioController audioController;
        [SerializeField] private ComboProgressView comboProgressView;
        [SerializeField] private CardSelectionController selectionController;
        [SerializeField] private float invalidComboHideDelay = 0.7f;

        [SerializeField] private CardEffectContextBuilder contextBuilder;

        private readonly List<CardView> activeComboCards = new();
        private string activeGroupId;
        private int expectedOrder = 1;
        public bool HasActiveCombo => activeComboCards.Count > 0;
        public void HandleCard(CardView card)
        {
            CardDefinition definition = card.Definition;

            if (string.IsNullOrWhiteSpace(definition.ComboGroupId))
            {
                Debug.LogWarning($"Sequence card has no combo group: {definition.DisplayName}", card);
                card.Hide();
                return;
            }

            if (activeComboCards.Count == 0)
            {
                TryStartCombo(card);
                return;
            }

            TryContinueCombo(card);
        }

        public void CancelActiveComboWith(CardView breakerCard)
        {
            StartCoroutine(CancelActiveComboRoutine(breakerCard));
        }
        private IEnumerator CancelActiveComboRoutine(CardView breakerCard)
        {
            selectionController.SetLocked(true);

            if (comboProgressView != null)
            {
                comboProgressView.Fail();
            }

            yield return new WaitForSeconds(invalidComboHideDelay);

            HideActiveComboCards();

            if (breakerCard != null && !breakerCard.IsMatched)
            {
                breakerCard.Hide();
            }

            ResetCombo();
            selectionController.SetLocked(false);
        }
        public void CancelActiveComboOnly()
        {
            StartCoroutine(CancelActiveComboOnlyRoutine());
        }

        private IEnumerator CancelActiveComboOnlyRoutine()
        {
            selectionController.SetLocked(true);

            if (comboProgressView != null)
            {
                comboProgressView.Fail();
            }

            yield return new WaitForSeconds(invalidComboHideDelay);

            HideActiveComboCards();
            ResetCombo();

            selectionController.SetLocked(false);
        }
        private void TryStartCombo(CardView card)
        {
            CardDefinition definition = card.Definition;

            if (definition.ComboOrder != 1)
            {
                StartCoroutine(RejectComboCard(card));
                return;
            }

            activeGroupId = definition.ComboGroupId;
            expectedOrder = 2;
            activeComboCards.Add(card);

            if (comboProgressView != null)
            {
                comboProgressView.Show(definition.ComboLength);
                comboProgressView.SetProgress(activeComboCards.Count);
                audioController?.PlayComboStep(activeComboCards.Count);
            }
        }

        private void TryContinueCombo(CardView card)
        {
            CardDefinition definition = card.Definition;

            bool isSameGroup = definition.ComboGroupId == activeGroupId;
            bool isExpectedOrder = definition.ComboOrder == expectedOrder;

            if (!isSameGroup || !isExpectedOrder)
            {
                StartCoroutine(RejectComboCard(card, hideActiveCombo: true));
                return;
            }

            activeComboCards.Add(card);
            expectedOrder++;

            if (comboProgressView != null)
            {
                comboProgressView.SetProgress(activeComboCards.Count);
                audioController?.PlayComboStep(activeComboCards.Count);
            }

            if (activeComboCards.Count >= definition.ComboLength)
            {
                CompleteCombo(definition);
            }
        }

        private IEnumerator RejectComboCard(CardView rejectedCard, bool hideActiveCombo = false)
        {
            audioController?.PlayComboFail();
            selectionController.SetLocked(true);

            if (comboProgressView != null)
            {
                comboProgressView.Fail();
            }

            yield return new WaitForSeconds(invalidComboHideDelay);

            rejectedCard.Hide();

            if (hideActiveCombo)
            {
                HideActiveComboCards();
            }

            ResetCombo();
            selectionController.SetLocked(false);
        }
        private void CompleteCombo(CardDefinition finalCardDefinition)
        {
            CardEffect completionEffect = finalCardDefinition.ComboCompleteEffect;

            if (completionEffect != null)
            {
                completionEffect.Apply(contextBuilder.Build());
            }
            else
            {
                Debug.LogWarning(
                    $"Combo completed but has no effect: {finalCardDefinition.ComboGroupId}",
                    this);
            }

            foreach (CardView comboCard in activeComboCards)
            {
                comboCard.MarkMatched();
                comboCard.PlayMatchedFeedback();
            }

            if (comboProgressView != null)
            {
                comboProgressView.Complete();
            }

            ResetCombo();
        }

        private void HideActiveComboCards()
        {
            foreach (CardView comboCard in activeComboCards)
            {
                comboCard.Hide();
            }
        }

        private void ResetCombo()
        {
            activeComboCards.Clear();
            activeGroupId = null;
            expectedOrder = 1;
        }
    }
}