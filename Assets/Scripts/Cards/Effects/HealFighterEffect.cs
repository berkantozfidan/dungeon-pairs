using DungeonPairs.Combat;
using UnityEngine;

namespace DungeonPairs.Cards.Effects
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Card Effects/Heal Fighter")]
    public class HealFighterEffect : CardEffect
    {
        [SerializeField] private FighterId target;
        [SerializeField] private int amount = 6;

        public override void Apply(CardEffectContext context)
        {
            context.Combat.Heal(target, amount);
            context.CombatVisuals.PlayHeal(target, amount);
        }
    }
}