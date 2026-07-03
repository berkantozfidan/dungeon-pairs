using UnityEngine;
namespace DungeonPairs.Cards
{
    public abstract class CardEffect : ScriptableObject
    {
         public virtual float CompletionDelay => 0f;
        public abstract void Apply(CardEffectContext context);

    }
} 