using System;
using System.Collections;
using System.Collections.Generic;
using DungeonPairs.Board;
using DungeonPairs.Cards;
using DungeonPairs.Decks;
using DungeonPairs.Enemies;
using DungeonPairs.Levels;
using DungeonPairs.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonPairs.Combat
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField] private List<LevelDefinition> levels = new();
        [SerializeField] private float nextLevelDelay = 1.25f;

        private int activeLevelIndex;
        [SerializeField] private CardSelectionController cardSelectionController;
        [SerializeField] private float startingPreviewDuration = 2f;
        [SerializeField] private float nextDeckPreviewDuration = 1.25f;
        [SerializeField] private BoardVisualController boardVisualController;
        public event Action<BattleResult> BattleEnded;
        public event Action<float> DeckPreviewStarted;
        private LevelDefinition activeLevel;
        private bool battleEnded;
        private Coroutine deckPreviewRoutine;
        [SerializeField] private LevelDefinition startingLevel;
        [SerializeField] private CombatController combatController;
        [SerializeField] private EnemyController enemyController;
        [SerializeField] private DeckController deckController;
        [SerializeField] private CombatVisualController combatVisualController;
        [SerializeField] private BattleTransitionView transitionView;

        [SerializeField] private float deathStartDelay = 0.55f;
        public int CurrentLevelNumber => activeLevelIndex + 1;
        private int activeDeckIndex;
        private void OnEnable()
        {
            if (deckController != null)
            {
                deckController.DeckCompleted += HandleDeckCompleted;
            }

            if (combatController != null)
            {
                combatController.FighterDefeated += HandleFighterDefeated;
            }
        }

        private void OnDisable()
        {
            if (deckController != null)
            {
                deckController.DeckCompleted -= HandleDeckCompleted;
            }

            if (combatController != null)
            {
                combatController.FighterDefeated -= HandleFighterDefeated;
            }
        }
        private void Start()
        {
            if (levels.Count > 0)
            {
                LoadLevelAt(0);
                return;
            }

            LoadLevel(startingLevel);
        }
        private void LoadLevelAt(int levelIndex)
        {
            activeLevelIndex = levelIndex;
            LoadLevel(levels[activeLevelIndex]);
        }
        private void HandleDeckCompleted()
        {
            if (battleEnded || activeLevel == null)
            {
                return;
            }

            activeDeckIndex++;

            if (activeDeckIndex < activeLevel.Decks.Count)
            {
                bool hasNextDeckAfterSwap = activeDeckIndex + 1 < activeLevel.Decks.Count;

                cardSelectionController.SetLocked(true);
                enemyController.SetCombatActive(false);

                if (boardVisualController != null)
                {
                    boardVisualController.PlayDeckAdvanceTransition(
                        onSwap: () =>
                        {
                            deckController.LoadDeck(activeLevel.Decks[activeDeckIndex]);
                            Debug.Log($"Loaded deck {activeDeckIndex + 1}/{activeLevel.Decks.Count}", this);
                            StartDeckPreview(nextDeckPreviewDuration);
                        },
                        hasNextDeckAfterSwap: hasNextDeckAfterSwap);
                }
                else
                {
                    deckController.LoadDeck(activeLevel.Decks[activeDeckIndex]);
                    Debug.Log($"Loaded deck {activeDeckIndex + 1}/{activeLevel.Decks.Count}", this);
                    StartDeckPreview(nextDeckPreviewDuration);
                }

                return;
            }

            battleEnded = true;
            Debug.Log("Level lost: all decks completed but enemy is still alive.", this);
            BattleEnded?.Invoke(BattleResult.Defeat);
        }

        private void HandleFighterDefeated(FighterId fighterId)
        {
            if (battleEnded)
            {
                return;
            }

            if (fighterId == FighterId.Player)
            {
                battleEnded = true;
                cardSelectionController.SetLocked(true);
                enemyController.SetCombatActive(false);

                StartCoroutine(PlayerDefeatedRoutine());
                return;
            }

            bool hasNextLevel = levels.Count > 0 && activeLevelIndex + 1 < levels.Count;

            battleEnded = true;
            cardSelectionController.SetLocked(true);
            enemyController.SetCombatActive(false);

            StartCoroutine(EnemyDefeatedRoutine(hasNextLevel));
        }
        private IEnumerator PlayerDefeatedRoutine()
        {
            yield return new WaitForSeconds(deathStartDelay);

            combatVisualController?.PlayDeath(FighterId.Player);

            yield return new WaitForSeconds(nextLevelDelay);

            if (transitionView != null)
            {
                yield return transitionView.PlayTransition(() =>
                {
                    RestartScene();
                });
            }
            else
            {
                RestartScene();
            }
        }
        private IEnumerator EnemyDefeatedRoutine(bool hasNextLevel)
        {
            yield return new WaitForSeconds(deathStartDelay);

            if (combatVisualController != null)
            {
                combatVisualController.PlayDeath(FighterId.Enemy);
            }

            yield return new WaitForSeconds(nextLevelDelay);

            if (hasNextLevel)
            {
                Debug.Log("Enemy defeated. Loading next level.", this);

                if (transitionView != null)
                {
                    yield return transitionView.PlayTransition(() =>
                    {
                        LoadLevelAt(activeLevelIndex + 1);
                    });
                }
                else
                {
                    LoadLevelAt(activeLevelIndex + 1);
                }

                yield break;
            }


            if (transitionView != null)
            {
                yield return transitionView.PlayFinalFade(() =>
                {
                    RestartScene();
                });
            }
            else
            {
                RestartScene();
            }
        }
        private void RestartScene()
        {
            Time.timeScale = 1f;

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex);
        }
        public void LoadLevel(LevelDefinition level)
        {
            activeLevel = level;
            battleEnded = false;
            if (level == null)
            {
                Debug.LogError("BattleController needs a level definition.", this);
                return;
            }

            if (level.Enemy == null)
            {
                Debug.LogError($"Level has no enemy: {level.DisplayName}", level);
                return;
            }

            if (level.Decks.Count == 0)
            {
                Debug.LogError($"Level has no decks: {level.DisplayName}", level);
                return;
            }

            activeDeckIndex = 0;
            combatController.InitializeEnemy(level.Enemy.MaxHealth);
            enemyController.Initialize(level.Enemy);
            deckController.LoadDeck(level.Decks[activeDeckIndex]);
            StartDeckPreview(startingPreviewDuration);

            if (boardVisualController != null)
            {
                boardVisualController.SetNextDeckPreviewVisible(level.Decks.Count > 1);
            }
        }
        private void StartDeckPreview(float duration)
        {
            if (deckPreviewRoutine != null)
            {
                StopCoroutine(deckPreviewRoutine);
            }
            DeckPreviewStarted?.Invoke(duration);
            deckPreviewRoutine = StartCoroutine(DeckPreviewRoutine(duration));
        }
        private IEnumerator DeckPreviewRoutine(float duration)
        {
            cardSelectionController.SetLocked(true);
            enemyController.SetCombatActive(false);

            deckController.RevealAllCards(true);

            yield return new WaitForSeconds(duration);

            deckController.HideAllCards();

            if (!battleEnded)
            {
                cardSelectionController.SetLocked(false);
                enemyController.SetCombatActive(true);
            }

            deckPreviewRoutine = null;
        }

    }
}