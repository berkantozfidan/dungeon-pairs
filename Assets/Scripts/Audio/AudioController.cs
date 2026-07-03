using UnityEngine;

namespace DungeonPairs.Audio
{
    public class AudioController : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Music")]
        [SerializeField] private AudioClip backgroundMusic;

        [Header("Card SFX")]
        [SerializeField] private AudioClip cardFlip;
        [SerializeField] private AudioClip cardMatch;
        [SerializeField] private AudioClip cardMismatch;
        [SerializeField] private AudioClip shuffle;
        [SerializeField] private AudioClip scoutReveal;
        [SerializeField] private AudioClip comboStep1;
        [SerializeField] private AudioClip comboStep2;
        [SerializeField] private AudioClip comboStep3;
        [SerializeField] private AudioClip comboComplete;
        [SerializeField] private AudioClip comboFail;

        [Header("Combat SFX")]
        [SerializeField] private AudioClip playerAttack;
        [SerializeField] private AudioClip enemyAttack;
        [SerializeField] private AudioClip shieldActivate;
        [SerializeField] private AudioClip shieldBlock;
        [SerializeField] private AudioClip heal;
        [SerializeField] private AudioClip freeze;
        [SerializeField] private AudioClip enemyDeath;

        public bool IsMusicMuted =>
            musicSource == null || musicSource.mute;

        public bool IsSfxMuted =>
            sfxSource == null || sfxSource.mute;

        private void Start()
        {
            PlayMusic();
        }

        private void PlayMusic()
        {
            if (musicSource == null || backgroundMusic == null)
            {
                return;
            }

            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        private void PlaySfx(AudioClip clip)
        {
            if (sfxSource == null || clip == null)
            {
                return;
            }

            sfxSource.PlayOneShot(clip);
        }

        public void PlayCardFlip() => PlaySfx(cardFlip);
        public void PlayCardMatch() => PlaySfx(cardMatch);
        public void PlayCardMismatch() => PlaySfx(cardMismatch);
        public void PlayShuffle() => PlaySfx(shuffle);
        public void PlayScoutReveal() => PlaySfx(scoutReveal);
        public void PlayPlayerAttack() => PlaySfx(playerAttack);
        public void PlayEnemyAttack() => PlaySfx(enemyAttack);
        public void PlayShieldActivate() => PlaySfx(shieldActivate);
        public void PlayShieldBlock() => PlaySfx(shieldBlock);
        public void PlayHeal() => PlaySfx(heal);
        public void PlayFreeze() => PlaySfx(freeze);
        public void PlayEnemyDeath() => PlaySfx(enemyDeath);

        public void PlayComboStep(int step)
        {
            switch (step)
            {
                case 1:
                    PlaySfx(comboStep1);
                    break;
                case 2:
                    PlaySfx(comboStep2);
                    break;
                case 3:
                    PlaySfx(comboStep3);
                    break;
                default:
                    PlaySfx(comboStep3);
                    break;
            }
        }

        public void PlayComboComplete() => PlaySfx(comboComplete);
        public void PlayComboFail() => PlaySfx(comboFail);

        public void ToggleMusic()
        {
            if (musicSource == null)
            {
                return;
            }

            musicSource.mute = !musicSource.mute;
        }

        public void ToggleSfx()
        {
            if (sfxSource == null)
            {
                return;
            }

            sfxSource.mute = !sfxSource.mute;
        }
    }
}