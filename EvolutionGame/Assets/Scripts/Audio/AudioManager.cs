using UnityEngine;
using UnityEngine.Audio;

namespace EvolutionGame.Audio
{
    /// <summary>
    /// Глобальный синглтон-менеджер звука. Не разрушается между сценами.
    /// Воспроизводит фоновую музыку и звуковые эффекты.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Источники звука")]
        public AudioSource musicSource;
        public AudioSource sfxSource;

        [Header("Звуковые эффекты")]
        public AudioClip cardSelectSfx;
        public AudioClip cardPlaySfx;
        public AudioClip phaseChangeSfx;
        public AudioClip buttonClickSfx;

        [Header("Музыкальные треки")]
        public AudioClip menuMusic;
        public AudioClip gameMusic;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlayMenuMusic()
        {
            PlayMusic(menuMusic);
        }

        public void PlayGameMusic()
        {
            PlayMusic(gameMusic);
        }

        private void PlayMusic(AudioClip clip)
        {
            if (musicSource == null || clip == null) return;
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void PlayCardSelect() => PlaySfx(cardSelectSfx);
        public void PlayCardPlay() => PlaySfx(cardPlaySfx);
        public void PlayPhaseChange() => PlaySfx(phaseChangeSfx);
        public void PlayButtonClick() => PlaySfx(buttonClickSfx);

        private void PlaySfx(AudioClip clip)
        {
            if (sfxSource == null || clip == null) return;
            sfxSource.PlayOneShot(clip);
        }
    }
}
