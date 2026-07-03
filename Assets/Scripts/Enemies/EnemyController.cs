using DungeonPairs.Combat;
using UnityEngine;

namespace DungeonPairs.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private Transform visualRoot;
        [SerializeField] private CombatVisualController combatVisuals;

        private GameObject activeVisual;


        [SerializeField] private EnemyDefinition enemyDefinition;
        [SerializeField] private EnemyActionContextBuilder contextBuilder;

        private float basicCooldownRemaining;
        private float heavyCooldownRemaining;
        private bool isActive;
        private bool isFrozen;
        public float CooldownRemaining => basicCooldownRemaining;
        public float BasicCooldownRemaining => basicCooldownRemaining;
        public float HeavyCooldownRemaining => heavyCooldownRemaining;
        public float BasicAttackCooldown => enemyDefinition != null ? enemyDefinition.BasicAttackCooldown : 0f;

        private void Start()
        {
            EnemyActionContext context = contextBuilder.Build();
            if (context.Combat != null)
            {
                context.Combat.FighterDefeated += HandleFighterDefeated;
            }
        }
        private void Update()
        {
            if (!isActive || isFrozen || enemyDefinition == null)
            {
                return;
            }

            TickBasicAttack();
            TickHeavyAttack();
        }
        public void Initialize(EnemyDefinition definition)
        {
            enemyDefinition = definition;

            if (enemyDefinition == null)
            {
                Debug.LogError("EnemyController received a null EnemyDefinition.", this);
                isActive = false;
                return;
            }

            isActive = true;
            isFrozen = false;

            SpawnVisual(enemyDefinition);

            basicCooldownRemaining = enemyDefinition.BasicAttackCooldown;
            heavyCooldownRemaining = enemyDefinition.HeavyAttackCooldown;
        }
        private void SpawnVisual(EnemyDefinition definition)
        {
            if (visualRoot == null)
            {
                return;
            }

            if (activeVisual != null)
            {
                Destroy(activeVisual);
            }

            if (definition.VisualPrefab == null)
            {
                return;
            }

            activeVisual = Instantiate(
                definition.VisualPrefab,
                visualRoot);

            activeVisual.transform.localPosition = Vector3.zero;
            activeVisual.transform.localRotation = Quaternion.identity;

            FighterAnimationController animationController =
                activeVisual.GetComponentInChildren<FighterAnimationController>(true);

            FighterVisualFreezer freezer =
                activeVisual.GetComponentInChildren<FighterVisualFreezer>(true);

            combatVisuals.SetEnemyVisual(animationController, freezer);
        }
        public void SetFrozen(bool frozen)
        {
            isFrozen = frozen;
        }
        public void SetCombatActive(bool active)
        {
            isActive = active;
        }
        private void OnDestroy()
        {
            if (contextBuilder == null)
            {
                return;
            }

            EnemyActionContext context = contextBuilder.Build();
            if (context.Combat != null)
            {
                context.Combat.FighterDefeated -= HandleFighterDefeated;
            }
        }

        private void TickBasicAttack()
        {
            if (enemyDefinition.BasicAttack == null)
            {
                return;
            }

            basicCooldownRemaining -= Time.deltaTime;

            if (basicCooldownRemaining <= 0f)
            {
                enemyDefinition.BasicAttack.Execute(contextBuilder.Build());
                basicCooldownRemaining = enemyDefinition.BasicAttackCooldown;
            }
        }

        private void TickHeavyAttack()
        {
            if (!enemyDefinition.HasHeavyAttack)
            {
                return;
            }

            heavyCooldownRemaining -= Time.deltaTime;

            if (heavyCooldownRemaining <= 0f)
            {
                enemyDefinition.HeavyAttack.Execute(contextBuilder.Build());
                heavyCooldownRemaining = enemyDefinition.HeavyAttackCooldown;
            }
        }

        private void HandleFighterDefeated(FighterId defeatedFighter)
        {
            isActive = false;
        }
    }
}