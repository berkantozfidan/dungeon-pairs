using UnityEngine;

namespace DungeonPairs.Cards.Effects
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Card Effects/Reveal Cards")]
    public class RevealCardsEffect : CardEffect
    {
        [SerializeField] private int revealCount = 3;
        [SerializeField] private float revealDuration = 1.5f;

        public override float CompletionDelay => revealDuration;

        public override void Apply(CardEffectContext context)
        {
            context.Board.Deck.RevealRandomCardsTemporarily(revealCount, revealDuration);
        }
    }
}