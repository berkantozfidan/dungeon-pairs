using UnityEngine;

namespace DungeonPairs.Enemies
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Enemies/Enemy Definition")]
    public class EnemyDefinition : ScriptableObject
    {
        [Header("Visual")]
        [SerializeField] private GameObject visualPrefab;
        public GameObject VisualPrefab => visualPrefab;

        [SerializeField] private string displayName;
        [SerializeField] private int maxHealth = 50;

        [Header("Basic Attack")]
        [SerializeField] private EnemyAction basicAttack;
        [SerializeField] private float basicAttackCooldown = 5f;

        [Header("Heavy Attack")]
        [SerializeField] private EnemyAction heavyAttack;
        [SerializeField] private float heavyAttackCooldown = 0f;

        public string DisplayName => displayName;
        public int MaxHealth => maxHealth;
        public EnemyAction BasicAttack => basicAttack;
        public float BasicAttackCooldown => basicAttackCooldown;
        public EnemyAction HeavyAttack => heavyAttack;
        public float HeavyAttackCooldown => heavyAttackCooldown;
        public bool HasHeavyAttack => heavyAttack != null && heavyAttackCooldown > 0f;


    }
}