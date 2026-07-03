using DungeonPairs.Board;
using DungeonPairs.Combat;
using DungeonPairs.Enemies;
using UnityEngine;

namespace DungeonPairs.Cards
{
    public readonly struct CardEffectContext
    {
        public CardEffectContext(
            CombatController combat,
            CombatVisualController combatVisuals,
            BoardController board,
            EnemyController enemy,
            MonoBehaviour coroutineRunner)
        {
            Combat = combat;
            CombatVisuals = combatVisuals;
            Board = board;
            Enemy = enemy;
            CoroutineRunner = coroutineRunner;
        }

        public CombatController Combat { get; }
        public CombatVisualController CombatVisuals { get; }
        public BoardController Board { get; }
        public EnemyController Enemy { get; }
        public MonoBehaviour CoroutineRunner { get; }
    }
}