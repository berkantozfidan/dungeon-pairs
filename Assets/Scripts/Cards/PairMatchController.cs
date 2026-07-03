using System.Collections;
using DungeonPairs.Audio;
using UnityEngine;

namespace DungeonPairs.Cards
{
    public class PairMatchController : MonoBehaviour
    {
        [SerializeField] private AudioController audioController;
        [SerializeField] private float pairRevealPause = 0.35f;
        [SerializeField] private CardSelectionController selectionController;
        [SerializeField] private CardEffectContextBuilder contextBuilder;
        [SerializeField] private float mismatchHideDelay = 0.7f;
        public bool HasPendingPair => firstCard != null;
        private CardView firstCard;
        private CardView secondCard;

        public void HandleCard(CardView card)
        {
            if (firstCard == null)
            {
                firstCard = card;
                return;
            }

            secondCard = card;
            StartCoroutine(ResolvePair());
        }

        private IEnumerator ResolvePair()
        {
            selectionController.SetLocked(true);

            yield return new WaitForSeconds(pairRevealPause);

            if (IsMatch(firstCard, secondCard))
            {
                audioController?.PlayCardMatch();

                CardEffectContext context = contextBuilder.Build();
                CardEffect effect = firstCard.Definition.Effect;

                if (effect != null)
                {
                    effect.Apply(context);
                }

                firstCard.MarkMatched();
                secondCard.MarkMatched();

                firstCard.PlayMatchedFeedback();
                secondCard.PlayMatchedFeedback();

                firstCard = null;
                secondCard = null;

                bool fighterDefeated =
                    context.Combat.Player.IsDefeated ||
                    context.Combat.Enemy.IsDefeated;

                if (!fighterDefeated)
                {
                    selectionController.SetLocked(false);
                }

                yield break;
            }

            audioController?.PlayCardMismatch();

            firstCard.PlayMismatchFeedback();
            secondCard.PlayMismatchFeedback();

            yield return new WaitForSeconds(mismatchHideDelay);

            firstCard.Hide();
            secondCard.Hide();

            firstCard = null;
            secondCard = null;
            selectionController.SetLocked(false);
        }

        public void CancelPendingPairWith(CardView otherCard)
        {
            StartCoroutine(CancelPendingPairRoutine(otherCard));
        }

        private IEnumerator CancelPendingPairRoutine(CardView otherCard)
        {
            selectionController.SetLocked(true);

            yield return new WaitForSeconds(mismatchHideDelay);

            if (firstCard != null)
            {
                firstCard.Hide();
            }

            if (otherCard != null)
            {
                otherCard.Hide();
            }

            firstCard = null;
            secondCard = null;
            selectionController.SetLocked(false);
        }

        private static bool IsMatch(CardView first, CardView second)
        {
            string firstKey = first.Definition.MatchKey;
            string secondKey = second.Definition.MatchKey;

            return !string.IsNullOrWhiteSpace(firstKey)
                && firstKey == secondKey;
        }

        public void HidePendingPairCard()
        {
            if (firstCard != null)
            {
                firstCard.Hide();
            }

            firstCard = null;
            secondCard = null;
        }
    }
}