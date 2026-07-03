using DungeonPairs.Board;
using DungeonPairs.Combat;
using UnityEngine;

namespace DungeonPairs.Enemies
{
    public readonly struct EnemyActionContext
    {
        public EnemyActionContext(
            CombatController combat,
            CombatVisualController combatVisuals,
            BoardController board,
            MonoBehaviour coroutineRunner)
        {
            Combat = combat;
            CombatVisuals = combatVisuals;
            Board = board;
            CoroutineRunner = coroutineRunner;
        }

        public CombatController Combat { get; }
        public CombatVisualController CombatVisuals { get; }
        public BoardController Board { get; }
        public MonoBehaviour CoroutineRunner { get; }
    }
}