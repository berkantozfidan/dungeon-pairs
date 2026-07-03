using System.Collections;
using DungeonPairs.Combat;
using UnityEngine;

namespace DungeonPairs.Cards.Effects
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Card Effects/Freeze Fighter")]
    public class FreezeFighterEffect : CardEffect
    {
        [SerializeField] private FighterId target = FighterId.Enemy;
        [SerializeField] private float duration = 3f;

        public override float CompletionDelay => duration;

        public override void Apply(CardEffectContext context)
        {
            context.CombatVisuals.PlayFreezeFighter(target, duration);

            if (target == FighterId.Enemy)
            {
                context.Enemy.SetFrozen(true);
                context.CoroutineRunner.StartCoroutine(UnfreezeEnemyAfterDelay(context));
            }
        }

        private IEnumerator UnfreezeEnemyAfterDelay(CardEffectContext context)
        {
            yield return new WaitForSeconds(duration);

            context.Enemy.SetFrozen(false);
        }
    }
}