using System;
using UnityEngine;

namespace DungeonPairs.Combat
{
    public class CombatController : MonoBehaviour
    {
        [SerializeField] private int playerMaxHealth = 100;
        [SerializeField] private int defaultEnemyMaxHealth  = 50;

        private FighterState player;
        private FighterState enemy;

        public event Action StateChanged;
        public event Action<FighterId> FighterDefeated;

        public FighterState Player => player;
        public FighterState Enemy => enemy;

        private void Awake()
        {
            player = new FighterState(playerMaxHealth);
            enemy = new FighterState(defaultEnemyMaxHealth );
        }
        public void InitializeEnemy(int maxHealth)
        {
            enemy = new FighterState(maxHealth);
            StateChanged?.Invoke();
        }
        public void InitializePlayer(int maxHealth)
        {
            player = new FighterState(maxHealth);
            StateChanged?.Invoke();
        }
        public FighterState GetFighter(FighterId fighterId)
        {
            return fighterId == FighterId.Player ? player : enemy;
        }

        public bool Damage(FighterId target, int amount)
        {
            FighterState fighter = GetFighter(target);
            bool wasBlocked = fighter.Damage(amount);

            StateChanged?.Invoke();

            if (!wasBlocked && fighter.IsDefeated)
            {
                FighterDefeated?.Invoke(target);
            }

            return wasBlocked;
        }

        public void Heal(FighterId target, int amount)
        {
            GetFighter(target).Heal(amount);
            StateChanged?.Invoke();
        }

        public void ActivateBlockShield(FighterId target)
        {
            GetFighter(target).ActivateBlockShield();
            StateChanged?.Invoke();
        }
    }
}