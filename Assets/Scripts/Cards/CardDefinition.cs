using UnityEngine;

namespace DungeonPairs.Cards
{
    [CreateAssetMenu(menuName = "Dungeon Pairs/Cards/Card Definition")]
    public class CardDefinition : ScriptableObject
    {
        [Header("Deck Completion")]
        [SerializeField] private bool optionalForDeckCompletion;
        public bool IsOptionalForDeckCompletion => optionalForDeckCompletion;
        
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;

        [Header("Rules")]
        [SerializeField] private CardTriggerType triggerType;
        [SerializeField] private CardEffect effect;

        [Header("Matching")]
        [SerializeField] private string matchKey;

        [Header("Combo")]
        [SerializeField] private string comboGroupId;
        [SerializeField] private int comboOrder;
        [SerializeField] private int comboLength;
        [SerializeField] private CardEffect comboCompleteEffect;

        [Header("Visuals")]
        [SerializeField] private Sprite icon;
        [SerializeField] private Color revealedColor = Color.white;
        [SerializeField] private GameObject revealedPrefab;
        [SerializeField] private Material revealedBackingMaterial;
        public Material RevealedBackingMaterial => revealedBackingMaterial;

        public string Id => id;
        public string DisplayName => displayName;
        public CardTriggerType TriggerType => triggerType;
        public CardEffect Effect => effect;
        public string MatchKey => matchKey;
        public string ComboGroupId => comboGroupId;
        public int ComboOrder => comboOrder;
        public int ComboLength => comboLength;
        public CardEffect ComboCompleteEffect => comboCompleteEffect;
        public Sprite Icon => icon;
        public Color RevealedColor => revealedColor;
        public GameObject RevealedPrefab => revealedPrefab;
    }
}