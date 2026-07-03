using DungeonPairs.Board;
using DungeonPairs.Combat;
using DungeonPairs.Enemies;
using UnityEngine;

namespace DungeonPairs.Cards
{
    public class CardEffectContextBuilder : MonoBehaviour
    {
        [SerializeField] private EnemyController enemy;
        [SerializeField] private CombatController combat;
        [SerializeField] private CombatVisualController combatVisuals;
        [SerializeField] private BoardController board;
        public CardEffectContext Build()
        {
            return new CardEffectContext(combat, combatVisuals, board, enemy, this);
        }
    }
}