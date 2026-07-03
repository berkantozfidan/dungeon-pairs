using DungeonPairs.Combat;
using UnityEngine;

namespace DungeonPairs.Cards.Effects
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Card Effects/Shield Fighter")]
    public class ShieldFighterEffect : CardEffect
    {
        [SerializeField] private FighterId target;
        [SerializeField] private int amount = 8;

        public override void Apply(CardEffectContext context)
        {
            context.Combat.ActivateBlockShield(target);
            context.CombatVisuals.PlayShield(target, amount);
        }
    }
}