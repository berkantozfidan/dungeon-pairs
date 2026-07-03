using DungeonPairs.Decks;
using UnityEngine;

namespace DungeonPairs.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private BoardStatusController status;
        [SerializeField] private BoardVisualController visuals;
        [SerializeField] private DeckController deck;

        public BoardStatusController Status => status;
        public BoardVisualController Visuals => visuals;
        public DeckController Deck => deck;
    }
}