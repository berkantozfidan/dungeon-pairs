using System.Collections.Generic;
using DungeonPairs.Cards;
using UnityEngine;

namespace DungeonPairs.Decks
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Decks/Deck Definition")]
    public class DeckDefinition : ScriptableObject
    {
        [SerializeField] private List<CardDefinition> cards = new();

        public IReadOnlyList<CardDefinition> Cards => cards;
    }
}