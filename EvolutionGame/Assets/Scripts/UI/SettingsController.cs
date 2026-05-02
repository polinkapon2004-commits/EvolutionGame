using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

namespace EvolutionGame.UI
{
    /// <summary>
    /// Контроллер сцены настроек.
    /// Управляет громкостью музыки, звуковых эффектов и режимом окна/полного экрана.
    /// Сохраняет настройки в PlayerPrefs.
    /// </summary>
    public class SettingsController : MonoBehaviour
    {
        [Header("Ползунки громкости")]
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;

        [Header("Кнопки")]
        public Button fullscreenToggleButton;
        public Button doneButton;

        [Header("Аудиосмеситель")]
        public AudioMixer audioMixer;

        // Ключи для сохранения настроек в PlayerPrefs
        private const string KEY_MUSIC = "MusicVolume";
        private const string KEY_SFX = "SfxVolume";
        private const string KEY_FULLSCREEN = "Fullscreen";

        private void Start()
        {
            LoadSettings();

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            if (fullscreenToggleButton != null)
                fullscreenToggleButton.onClick.AddListener(OnFullscreenToggle);
            if (doneButton != null)
                doneButton.onClick.AddListener(OnDoneClicked);
        }

        private void LoadSettings()
        {
            float music = PlayerPrefs.GetFloat(KEY_MUSIC, 0.75f);
            float sfx = PlayerPrefs.GetFloat(KEY_SFX, 1.0f);
            bool fullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, 1) == 1;

            if (musicVolumeSlider != null) musicVolumeSlider.value = music;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfx;
            ApplyMusicVolume(music);
            ApplySfxVolume(sfx);
            Screen.fullScreen = fullscreen;
        }

        public void OnMusicVolumeChanged(float value)
        {
            ApplyMusicVolume(value);
        }

        public void OnSfxVolumeChanged(float value)
        {
            ApplySfxVolume(value);
        }

        private void ApplyMusicVolume(float linear)
        {
            // Преобразуем линейное значение [0..1] в децибелы [-80..0] для аудиомиксера
            float db = linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
            if (audioMixer != null) audioMixer.SetFloat("MusicVolume", db);
        }

        private void ApplySfxVolume(float linear)
        {
            float db = linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
            if (audioMixer != null) audioMixer.SetFloat("SfxVolume", db);
        }

        public void OnFullscreenToggle()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        public void OnDoneClicked()
        {
            SaveSettings();
            SceneManager.LoadScene("MainMenu");
        }

        private void SaveSettings()
        {
            if (musicVolumeSlider != null)
                PlayerPrefs.SetFloat(KEY_MUSIC, musicVolumeSlider.value);
            if (sfxVolumeSlider != null)
                PlayerPrefs.SetFloat(KEY_SFX, sfxVolumeSlider.value);
            PlayerPrefs.SetInt(KEY_FULLSCREEN, Screen.fullScreen ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
