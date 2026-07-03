using DungeonPairs.Board;
using DungeonPairs.Combat;
using UnityEngine;

namespace DungeonPairs.Enemies
{
    public class EnemyActionContextBuilder : MonoBehaviour
    {
        [SerializeField] private CombatController combat;
        [SerializeField] private CombatVisualController combatVisuals;
        [SerializeField] private BoardController board;

        public EnemyActionContext Build()
        {
            return new EnemyActionContext(combat, combatVisuals, board, this);
        }
    }
}