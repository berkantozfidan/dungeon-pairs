using System;

namespace DungeonPairs.Combat
{
    [Serializable]
    public class FighterState
    {
        public FighterState(int maxHealth)
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public bool HasBlockShield { get; private set; }
        public bool IsDefeated => Health <= 0;

        public bool Damage(int amount)
        {
            if (amount <= 0 || IsDefeated)
            {
                return false;
            }

            if (HasBlockShield)
            {
                HasBlockShield = false;
                return true;
            }

            Health = Math.Max(0, Health - amount);
            return false;
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDefeated)
            {
                return;
            }

            Health = Math.Min(MaxHealth, Health + amount);
        }
        public void ActivateBlockShield()
        {
            if (IsDefeated)
            {
                return;
            }

            HasBlockShield = true;
        }
    }
}