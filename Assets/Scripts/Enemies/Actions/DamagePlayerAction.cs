using DungeonPairs.Combat;
using UnityEngine;

namespace DungeonPairs.Enemies.Actions
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Enemy Actions/Damage Player")]
    public class DamagePlayerAction : EnemyAction
    {
        [SerializeField] private int amount = 8;
        public override void Execute(EnemyActionContext context)
        {
            bool wasBlocked = context.Combat.Damage(FighterId.Player, amount);

            if (wasBlocked)
            {
                context.CombatVisuals.PlayShieldBlock(FighterId.Player);
                return;
            }

            if (context.Combat.GetFighter(FighterId.Player).IsDefeated)
            {
                context.CombatVisuals.PlayFinisherDamage(FighterId.Player,amount);
                return;
            }

            context.CombatVisuals.PlayDamage(FighterId.Player, amount);
        }
    }
}