using DungeonPairs.Combat;
using UnityEngine;

namespace DungeonPairs.Cards.Effects
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Card Effects/Combo Damage Fighter")]
    public class ComboDamageFighterEffect : CardEffect
    {
        [SerializeField] private FighterId target;
        [SerializeField] private int amount = 20;

        public override void Apply(CardEffectContext context)
        {
            context.Combat.Damage(target, amount);

            if (context.Combat.GetFighter(target).IsDefeated)
            {
                context.CombatVisuals.PlayComboFinisherDamage(target, amount);
                return;
            }

            context.CombatVisuals.PlayComboDamage(target, amount);
        }
    }
}