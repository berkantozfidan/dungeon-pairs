using System.Collections;
using UnityEngine;

namespace DungeonPairs.Cards.Effects
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Card Effects/Shuffle Board")]
    public class ShuffleBoardEffect : CardEffect
    {
        [SerializeField] private float shuffleDelay = 0.75f;
        [SerializeField] private float shuffleMoveDuration = 0.45f;

        public override float CompletionDelay => shuffleDelay + shuffleMoveDuration;
        public override void Apply(CardEffectContext context)
        {
            context.CombatVisuals.PlayEnemyCastShuffle();
            context.CoroutineRunner.StartCoroutine(ShuffleAfterDelay(context));
        }

        private IEnumerator ShuffleAfterDelay(CardEffectContext context)
        {
            yield return new WaitForSeconds(shuffleDelay);

            context.Board.Visuals.PlayShuffle();
            context.Board.Deck.ShuffleActiveCards();
        }
    }
}