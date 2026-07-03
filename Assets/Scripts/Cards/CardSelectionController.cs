using System.Collections;
using DungeonPairs.Board;
using UnityEngine;

namespace DungeonPairs.Cards
{
    public class CardSelectionController : MonoBehaviour
    {
        [SerializeField] private BoardController board;
        [SerializeField] private PairMatchController pairMatchController;
        [SerializeField] private ComboController comboController;
        [SerializeField] private CardEffectContextBuilder contextBuilder;

        private bool isLocked;

        public void SelectCard(CardView card)
        {
            Debug.Log(card.Definition.DisplayName);
            if (isLocked || card == null || card.IsMatched)
            {
                return;
            }

            if (board != null && !board.Status.CanSelectCards)
            {
                return;
            }

            if (pairMatchController.HasPendingPair && card.Definition.TriggerType != CardTriggerType.PairMatch)
            {
                card.Reveal();

                if (card.Definition.TriggerType == CardTriggerType.InstantReveal)
                {
                    ResolveInstantCard(card, true);
                    return;
                }

                pairMatchController.CancelPendingPairWith(card);
                return;
            }

            if (comboController.HasActiveCombo && card.Definition.TriggerType != CardTriggerType.Sequence)
            {
                card.Reveal();

                if (card.Definition.TriggerType == CardTriggerType.InstantReveal)
                {
                    ResolveInstantCard(card, hidePendingPairAfterEffect: false, cancelActiveComboAfterEffect: true);
                    return;
                }

                comboController.CancelActiveComboWith(card);
                return;
            }

            card.Reveal();

            switch (card.Definition.TriggerType)
            {
                case CardTriggerType.PairMatch:
                    pairMatchController.HandleCard(card);
                    break;

                case CardTriggerType.InstantReveal:
                    ResolveInstantCard(card);
                    break;

                case CardTriggerType.Sequence:
                    comboController.HandleCard(card);
                    break;

                default:
                    Debug.LogWarning($"Unhandled trigger type: {card.Definition.TriggerType}", this);
                    card.Hide();
                    break;
            }
        }

        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }

        private void ResolveInstantCard(CardView card, bool hidePendingPairAfterEffect = false,
        bool cancelActiveComboAfterEffect = false)
        {
            if (card.Definition.Effect == null)
            {
                Debug.LogWarning($"Instant card has no effect: {card.Definition.DisplayName}", card);
                card.Hide();
                return;
            }

            StartCoroutine(ResolveInstantCardRoutine(card, hidePendingPairAfterEffect,
            cancelActiveComboAfterEffect));
        }
        private IEnumerator ResolveInstantCardRoutine(CardView card, bool hidePendingPairAfterEffect,
        bool cancelActiveComboAfterEffect)
        {
            SetLocked(true);

            CardEffect effect = card.Definition.Effect;
            effect.Apply(contextBuilder.Build());

            if (effect.CompletionDelay > 0f)
            {
                yield return new WaitForSeconds(effect.CompletionDelay);
            }

            if (hidePendingPairAfterEffect)
            {
                pairMatchController.HidePendingPairCard();
            }
            if (cancelActiveComboAfterEffect)
            {
                comboController.CancelActiveComboOnly();
            }

            card.MarkUsed();

            SetLocked(false);
        }
    }

}