using DungeonPairs.Audio;
using UnityEngine;

namespace DungeonPairs.Combat
{
    public class FighterAnimationController : MonoBehaviour
    {
        [SerializeField] private AudioController audioController;
        [SerializeField] private FighterId fighterId;
        [SerializeField] private Animator animator;
        [SerializeField] private string hitTriggerName = "Hit";
        [SerializeField] private string attackTriggerName = "Attack";
        [SerializeField] private string comboAttackTriggerName = "ComboAttack";
        [SerializeField] private string healTriggerName = "Heal";
        [SerializeField] private string castTriggerName = "Cast";

        [SerializeField] private string deathTriggerName = "Death";

        public void PlayHit()
        {
            if (animator == null)
            {
                Debug.LogWarning("FighterAnimationController has no Animator.", this);
                return;
            }

            animator.ResetTrigger(hitTriggerName);
            animator.SetTrigger(hitTriggerName);
        }
        public void PlayAttackSfx()
        {
            if (fighterId == FighterId.Player)
            {
                audioController?.PlayPlayerAttack();
            }
            else if (fighterId == FighterId.Enemy)
            {
                audioController?.PlayEnemyAttack();
            }
        }
        public void PlayComboAttackSfx()
        {
            if (fighterId == FighterId.Player)
            {
                audioController?.PlayComboComplete();
            }
        }
        public void ConfigureAudio(AudioController audio, FighterId id)
        {
            audioController = audio;
            fighterId = id;
        }

        public void PlayAttack()
        {
            if (animator == null)
            {
                Debug.LogWarning("FighterAnimationController has no Animator.", this);
                return;
            }

            animator.ResetTrigger(attackTriggerName);
            animator.SetTrigger(attackTriggerName);
        }

        public void PlayComboAttack()
        {
            if (animator == null)
            {
                Debug.LogWarning("FighterAnimationController has no Animator.", this);
                return;
            }

            animator.ResetTrigger(comboAttackTriggerName);
            animator.SetTrigger(comboAttackTriggerName);
        }

        public void PlayHeal()
        {
            if (animator == null)
            {
                Debug.LogWarning("FighterAnimationController has no Animator.", this);
                return;
            }

            animator.ResetTrigger(healTriggerName);
            animator.SetTrigger(healTriggerName);
        }
        public void PlayCast()
        {
            if (animator == null)
            {
                Debug.LogWarning("FighterAnimationController has no Animator.", this);
                return;
            }

            animator.ResetTrigger(castTriggerName);
            animator.SetTrigger(castTriggerName);
        }
        public void PlayDeath()
        {
            if (animator == null)
            {
                Debug.LogWarning("FighterAnimationController has no Animator.", this);
                return;
            }

            animator.ResetTrigger(deathTriggerName);
            animator.SetTrigger(deathTriggerName);
        }
        public void SetAnimationSpeed(float speed)
        {
            if (animator == null)
            {
                return;
            }

            animator.speed = speed;
        }
    }
}
