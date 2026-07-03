using UnityEngine;

namespace DungeonPairs.Cards.Effects
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Card Effects/Freeze Board")]
    public class FreezeBoardEffect : CardEffect
    {
        [SerializeField] private float duration = 3f;
        [SerializeField] private float speedMultiplier = 0.5f;
        public override float CompletionDelay => duration;

        public override void Apply(CardEffectContext context)
        {
            context.CombatVisuals.PlayFreezePlayer(duration);
            context.Board.Status.ApplyFreeze(duration, speedMultiplier);
            context.Board.Visuals.PlayFreeze(duration);
        }
    }
}