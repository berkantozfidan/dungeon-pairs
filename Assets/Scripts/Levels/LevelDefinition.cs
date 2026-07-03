using System.Collections.Generic;
using DungeonPairs.Decks;
using DungeonPairs.Enemies;
using UnityEngine;

namespace DungeonPairs.Levels
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Levels/Level Definition")]
    public class LevelDefinition : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private EnemyDefinition enemy;
        [SerializeField] private List<DeckDefinition> decks = new();

        public string DisplayName => displayName;
        public EnemyDefinition Enemy => enemy;
        public IReadOnlyList<DeckDefinition> Decks => decks;
    }
}