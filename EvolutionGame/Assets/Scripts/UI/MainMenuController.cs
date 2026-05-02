using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EvolutionGame.UI
{
    /// <summary>
    /// Контроллер сцены главного меню.
    /// Прикреплён к GameObject Canvas в сцене MainMenu.
    /// Обрабатывает нажатия на кнопки "Начало игры", "Игра с ботом", "Настройки", "Выход".
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Кнопки главного меню")]
        public Button startGameButton;
        public Button startWithBotButton;
        public Button settingsButton;
        public Button exitButton;

        [Header("Панели меню")]
        public GameObject mainPanel;
        public GameObject playerCountPanel;
        public GameObject botCountPanel;

        [Header("Настройки запуска игры")]
        public string gameSceneName = "GameScene";
        public string settingsSceneName = "Settings";

        private void Awake()
        {
            if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGameClicked);
            if (startWithBotButton != null) startWithBotButton.onClick.AddListener(OnStartWithBotClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
        }

        public void OnStartGameClicked()
        {
            // Показывает панель выбора количества игроков (от 2 до 4)
            mainPanel.SetActive(false);
            playerCountPanel.SetActive(true);
        }

        public void OnStartWithBotClicked()
        {
            // Показывает панель выбора количества ботов (от 1 до 3)
            mainPanel.SetActive(false);
            botCountPanel.SetActive(true);
        }

        /// <summary>
        /// Вызывается из панели выбора количества игроков по нажатию на 2/3/4.
        /// </summary>
        public void StartMultiplayerGame(int humanCount)
        {
            GameLaunchData.HumanPlayers = humanCount;
            GameLaunchData.BotPlayers = 0;
            SceneManager.LoadScene(gameSceneName);
        }

        /// <summary>
        /// Вызывается из панели выбора количества ботов по нажатию на 1/2/3.
        /// </summary>
        public void StartGameWithBots(int botCount)
        {
            GameLaunchData.HumanPlayers = 1;
            GameLaunchData.BotPlayers = botCount;
            SceneManager.LoadScene(gameSceneName);
        }

        public void OnSettingsClicked()
        {
            SceneManager.LoadScene(settingsSceneName);
        }

        public void OnExitClicked()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }

    /// <summary>
    /// Статический контейнер параметров запуска игры,
    /// чтобы пробросить их между сценами без зависимостей.
    /// </summary>
    public static class GameLaunchData
    {
        public static int HumanPlayers { get; set; } = 2;
        public static int BotPlayers { get; set; } = 0;
    }
}
