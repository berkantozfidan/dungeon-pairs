using System.Collections;
using DungeonPairs.Combat;
using DungeonPairs.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonPairs.UI
{
    public class BattleUIController : MonoBehaviour
    {
        [SerializeField] private BattleController battleController;
        [SerializeField] private CombatController combat;
        [SerializeField] private EnemyController enemy;

        [SerializeField] private Image playerHealthFill;
        [SerializeField] private Image enemyHealthFill;
        [SerializeField] private Image enemyCooldownFill;
        [SerializeField] private float healthUpdateDelay = 0.25f;
        private Coroutine healthRefreshRoutine;

        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Image revealIconFill;
        private Coroutine previewIconRoutine;


        private void Start()
        {
            Refresh();
        }
        private void OnEnable()
        {
            if (combat != null)
            {
                combat.StateChanged += Refresh;
            }
            if (battleController != null)
            {
                battleController.DeckPreviewStarted += ShowDeckPreviewIcon;
            }
        }

        private void OnDisable()
        {
            if (combat != null)
            {
                combat.StateChanged -= Refresh;
            }
            if (battleController != null)
            {
                battleController.DeckPreviewStarted -= ShowDeckPreviewIcon;
            }
        }
        private void ShowDeckPreviewIcon(float duration)
        {
            if (previewIconRoutine != null)
            {
                StopCoroutine(previewIconRoutine);
            }

            previewIconRoutine = StartCoroutine(
                DeckPreviewIconRoutine(duration));
        }

        private IEnumerator DeckPreviewIconRoutine(float duration)
        {
            revealIconFill.gameObject.SetActive(true);

            float remaining = duration;

            while (remaining > 0f)
            {
                remaining -= Time.deltaTime;
                revealIconFill.fillAmount =
                    Mathf.Clamp01(remaining / duration);

                yield return null;
            }

            revealIconFill.fillAmount = 0f;
            revealIconFill.gameObject.SetActive(false);
            previewIconRoutine = null;
        }
        private void Update()
        {
            if (enemy != null && enemyCooldownFill != null)
            {
                float totalCooldown = enemy.BasicAttackCooldown;

                enemyCooldownFill.fillAmount = totalCooldown > 0f
                    ? Mathf.Clamp01(enemy.BasicCooldownRemaining / totalCooldown)
                    : 0f;
            }
        }

        private void Refresh()
        {
            if (combat == null)
            {
                return;
            }
            if (levelText != null && battleController != null)
            {
                levelText.text = $"Level {battleController.CurrentLevelNumber}";
            }


            if (healthRefreshRoutine != null)
            {
                StopCoroutine(healthRefreshRoutine);
            }

            healthRefreshRoutine = StartCoroutine(RefreshHealthBarsDelayed());
        }
        private IEnumerator RefreshHealthBarsDelayed()
        {
            yield return new WaitForSeconds(healthUpdateDelay);

            playerHealthFill.fillAmount =
                (float)combat.Player.Health / combat.Player.MaxHealth;

            enemyHealthFill.fillAmount =
                (float)combat.Enemy.Health / combat.Enemy.MaxHealth;

            healthRefreshRoutine = null;
        }
    }
}