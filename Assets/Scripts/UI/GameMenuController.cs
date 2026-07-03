using DungeonPairs.Audio;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DungeonPairs.UI
{
    public class GameMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel;

        [SerializeField] private AudioController audioController;

        [SerializeField] private Image musicIcon;
        [SerializeField] private Sprite musicOnSprite;
        [SerializeField] private Sprite musicOffSprite;


        [SerializeField] private Image sfxIcon;
        [SerializeField] private Sprite sfxOnSprite;
        [SerializeField] private Sprite sfxOffSprite;

        private bool gameStarted;
        private bool isPaused;

        private void Awake()
        {
            Time.timeScale = 0f;
            menuPanel.SetActive(true);

            gameStarted = false;
            isPaused = true;
        }

        private void Update()
        {
            if (!gameStarted ||
                Keyboard.current == null ||
                !Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                return;
            }

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void PrimaryAction()
        {
            gameStarted = true;
            ResumeGame();
        }

        public void PauseGame()
        {
            if (!gameStarted)
            {
                return;
            }

            isPaused = true;
            Time.timeScale = 0f;
            menuPanel.SetActive(true);
        }


        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;
            menuPanel.SetActive(false);
        }


        public void ToggleMusic()
        {
            if (audioController == null)
            {
                return;
            }

            audioController.ToggleMusic();

            musicIcon.sprite = audioController.IsMusicMuted
                ? musicOffSprite
                : musicOnSprite;
        }

        public void ToggleSfx()
        {
            if (audioController == null)
            {
                return;
            }

            audioController.ToggleSfx();

            sfxIcon.sprite = audioController.IsSfxMuted
                ? sfxOffSprite
                : sfxOnSprite;
        }
    }
}