using DungeonPairs.Combat;
using UnityEngine;

namespace DungeonPairs.Cards.Effects
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Card Effects/Damage Fighter")]
    public class DamageFighterEffect : CardEffect
    {
        [SerializeField] private FighterId target;
        [SerializeField] private int amount = 10;
        public override void Apply(CardEffectContext context)
        {
            bool wasBlocked = context.Combat.Damage(target, amount);

            if (wasBlocked)
            {
                context.CombatVisuals.PlayShieldBlock(target);
                return;
            }

            if (context.Combat.GetFighter(target).IsDefeated)
            {
                context.CombatVisuals.PlayFinisherDamage(target, amount);
                return;
            }

            context.CombatVisuals.PlayDamage(target, amount); 
        }
    }
}