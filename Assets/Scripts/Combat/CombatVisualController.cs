using System.Collections;
using DG.Tweening;
using DungeonPairs.Audio;
using UnityEngine;

namespace DungeonPairs.Combat
{
    public class CombatVisualController : MonoBehaviour
    {
        [SerializeField] private AudioController audioController;
        [SerializeField] private FighterVisualFreezer playerFreezer;
        [SerializeField] private FighterVisualFreezer enemyFreezer;
        [SerializeField] private float frozenAnimationSpeed = 0.15f;
        [SerializeField] private FighterAnimationController playerAnimator;
        [SerializeField] private FighterAnimationController enemyAnimator;
        [SerializeField] private float hitAnimationDelay = 0.2f;
        [SerializeField] private float comboHitAnimationDelay = 0.35f;

        [SerializeField] private Transform playerHealPoint;
        [SerializeField] private GameObject healParticlePrefab;

        [SerializeField] private Transform enemyCastPoint;
        [SerializeField] private Transform playerImpactPoint;
        [SerializeField] private GameObject shuffleTornadoPrefab;
        [SerializeField] private float shuffleTornadoTravelDuration = 0.9f;
        [SerializeField] private float shufflePlayerLiftHeight = 0.35f;
        [SerializeField] private float shufflePlayerLiftDuration = 0.35f;
        [SerializeField] private Transform playerVisualRoot;

        [SerializeField] private Transform playerShieldPoint;
        [SerializeField] private GameObject shieldBubblePrefab;
        private GameObject activeShieldBubble;

        private void Awake()
        {
            playerAnimator?.ConfigureAudio(audioController, FighterId.Player);
            enemyAnimator?.ConfigureAudio(audioController, FighterId.Enemy);
        }

        public void PlayDamage(FighterId target, int amount)
        {
            Debug.Log($"Damage visual: {target} took {amount} damage.", this);
            if (target == FighterId.Player)
            {
                enemyAnimator?.PlayAttack();
                StartCoroutine(PlayHitAfterDelay(playerAnimator, hitAnimationDelay));
            }
            else if (target == FighterId.Enemy)
            {
                playerAnimator?.PlayAttack();
                StartCoroutine(PlayHitAfterDelay(enemyAnimator, hitAnimationDelay));
            }
        }
        public void PlayFinisherDamage(FighterId target, int amount)
        {
            Debug.Log($"Finisher damage visual: {target} took {amount} damage.", this);

            if (target == FighterId.Player)
            {
                enemyAnimator?.PlayAttack();
            }
            else if (target == FighterId.Enemy)
            {
                playerAnimator?.PlayAttack();
            }
        }
        public void PlayComboFinisherDamage(FighterId target, int amount)
        {
            Debug.Log($"Combo finisher damage visual: {target} took {amount} damage.", this);

            if (target == FighterId.Player)
            {
                enemyAnimator?.PlayComboAttack();
            }
            else if (target == FighterId.Enemy)
            {
                playerAnimator?.PlayComboAttack();
            }
        }
        private IEnumerator PlayHitAfterDelay(FighterAnimationController targetAnimator, float delay)
        {
            if (targetAnimator == null)
            {
                yield break;
            }

            yield return new WaitForSeconds(delay);

            targetAnimator.PlayHit();
        }

        public void PlayFreezeFighter(FighterId target, float duration)
        {
            FighterVisualFreezer freezer = target == FighterId.Player ? playerFreezer : enemyFreezer;
            FighterAnimationController animator = target == FighterId.Player ? playerAnimator : enemyAnimator;
            audioController?.PlayFreeze();
            if (freezer == null)
            {
                Debug.Log($"Freeze visual missing for {target}.", this);
                return;
            }

            freezer.SetFrozen(true);
            animator?.SetAnimationSpeed(frozenAnimationSpeed);

            StartCoroutine(UnfreezeFighterAfterDelay(freezer, animator, duration));
        }

        private IEnumerator UnfreezeFighterAfterDelay(
            FighterVisualFreezer freezer,
            FighterAnimationController animator,
            float duration)
        {
            yield return new WaitForSeconds(duration);

            freezer.SetFrozen(false);
            animator?.SetAnimationSpeed(1f);
        }
        public void PlayComboDamage(FighterId target, int amount)
        {
            Debug.Log($"Combo damage visual: {target} took {amount} damage.", this);

            if (target == FighterId.Player)
            {
                enemyAnimator?.PlayComboAttack();
                StartCoroutine(PlayHitAfterDelay(playerAnimator, comboHitAnimationDelay));
            }
            else if (target == FighterId.Enemy)
            {
                playerAnimator?.PlayComboAttack();
                StartCoroutine(PlayHitAfterDelay(enemyAnimator, comboHitAnimationDelay));
            }
        }
        public void PlayHeal(FighterId target, int amount)
        {
            Debug.Log($"Heal visual: {target} healed {amount}.", this);
            audioController?.PlayHeal();
            if (target == FighterId.Player)
            {
                SpawnHealParticle(playerHealPoint);
            }
        }
        private void SpawnHealParticle(Transform healPoint)
        {
            if (healParticlePrefab == null || healPoint == null)
            {
                Debug.LogWarning("Heal particle prefab or heal point is missing.", this);
                return;
            }

            GameObject particle = Instantiate(
                healParticlePrefab,
                healPoint.position,
                healPoint.rotation);

            Destroy(particle, 2f);
        }

        public void PlayShield(FighterId target, int amount)
        {
            Debug.Log($"Shield visual: {target} gained {amount} shield.", this);
            audioController?.PlayShieldActivate();
            if (target != FighterId.Player)
            {
                return;
            }

            ShowShieldBubble(playerShieldPoint);
        }
        private void ShowShieldBubble(Transform shieldPoint)
        {
            if (shieldBubblePrefab == null || shieldPoint == null)
            {
                Debug.LogWarning("Shield bubble prefab or shield point is missing.", this);
                return;
            }

            if (activeShieldBubble != null)
            {
                return;
            }

            activeShieldBubble = Instantiate(
                shieldBubblePrefab,
                shieldPoint.position,
                shieldPoint.rotation,
                shieldPoint);

            activeShieldBubble.transform.localPosition = Vector3.zero;
            activeShieldBubble.transform.localScale = Vector3.zero;

            activeShieldBubble.transform
                .DOScale(Vector3.one, 0.18f)
                .SetEase(Ease.OutBack);
        }
        private void BreakShieldBubble()
        {
            if (activeShieldBubble == null)
            {
                return;
            }

            GameObject bubble = activeShieldBubble;
            activeShieldBubble = null;

            bubble.transform
                .DOScale(Vector3.zero, 0.18f)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(bubble));
        }
        public void PlayFreezePlayer(float duration)
        {
            PlayFreezeFighter(FighterId.Player, duration);
        }

        public void PlayEnemyCastShuffle()
        {
            Debug.Log("Enemy casts shuffle toward the board.", this);

            enemyAnimator?.PlayCast();

            if (enemyCastPoint == null || playerImpactPoint == null || shuffleTornadoPrefab == null)
            {
                return;
            }

            GameObject tornado = Instantiate(
                shuffleTornadoPrefab,
                enemyCastPoint.position,
                Quaternion.identity);

            tornado.transform
                .DOMove(playerImpactPoint.position, shuffleTornadoTravelDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    Destroy(tornado);
                    PlayShufflePlayerLift();
                });
        }
        private void PlayShufflePlayerLift()
        {
            if (playerVisualRoot == null)
            {
                return;
            }

            Vector3 startPosition = playerVisualRoot.localPosition;
            Quaternion startRotation = playerVisualRoot.localRotation;

            playerVisualRoot.DOKill();

            Sequence sequence = DOTween.Sequence();

            sequence.Append(playerVisualRoot
                .DOLocalMoveY(startPosition.y + shufflePlayerLiftHeight, shufflePlayerLiftDuration * 0.5f)
                .SetEase(Ease.OutQuad));

            sequence.Join(playerVisualRoot
                .DOShakeRotation(shufflePlayerLiftDuration, new Vector3(0f, 0f, 8f), vibrato: 10));

            sequence.Append(playerVisualRoot
                .DOLocalMoveY(startPosition.y, shufflePlayerLiftDuration * 0.5f)
                .SetEase(Ease.InQuad));

            sequence.OnComplete(() =>
            {
                playerVisualRoot.localPosition = startPosition;
                playerVisualRoot.localRotation = startRotation;
            });
        }

        public void PlayShieldBlock(FighterId target)
        {
            Debug.Log($"Shield blocked damage for {target}.", this);
            audioController?.PlayShieldBlock();
            if (target != FighterId.Player)
            {
                return;
            }

            enemyAnimator?.PlayAttack();
            StartCoroutine(BreakShieldAfterDelay());
        }
        public void PlayDeath(FighterId target)
        {
            audioController?.PlayEnemyDeath();
            if (target == FighterId.Player)
            {
                playerAnimator?.PlayDeath();
            }
            else if (target == FighterId.Enemy)
            {
                enemyAnimator?.PlayDeath();
            }
        }
        private IEnumerator BreakShieldAfterDelay()
        {
            yield return new WaitForSeconds(hitAnimationDelay);

            BreakShieldBubble();
        }

        public void SetEnemyVisual(
        FighterAnimationController animationController,
        FighterVisualFreezer freezer)
        {
            enemyAnimator = animationController;
            enemyFreezer = freezer;
            enemyAnimator?.ConfigureAudio(audioController, FighterId.Enemy);
        }

    }
}
