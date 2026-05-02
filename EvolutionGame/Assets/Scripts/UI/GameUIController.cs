using UnityEngine;
using UnityEngine.UI;
using EvolutionGame.Core;

namespace EvolutionGame.UI
{
    /// <summary>
    /// Контроллер UI игровой сцены. Отображает текущую фазу,
    /// активного игрока, счётчик кормовой базы и панели игроков.
    /// Подписан на события GameManager.
    /// </summary>
    public class GameUIController : MonoBehaviour
    {
        [Header("Информационные панели")]
        public Text phaseText;
        public Text currentPlayerText;
        public Text foodPoolText;
        public Text roundText;

        [Header("Окно завершения партии")]
        public GameObject gameOverPanel;
        public Text winnerText;

        [Header("Ссылка на менеджер игры")]
        public GameManager gameManager;

        private void Start()
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            if (gameManager != null)
            {
                gameManager.OnPhaseChanged += UpdatePhaseDisplay;
                gameManager.OnPlayerTurnChanged += UpdateCurrentPlayer;
                gameManager.OnGameOver += HandleGameOver;
            }

            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnPhaseChanged -= UpdatePhaseDisplay;
                gameManager.OnPlayerTurnChanged -= UpdateCurrentPlayer;
                gameManager.OnGameOver -= HandleGameOver;
            }
        }

        private void UpdatePhaseDisplay(GamePhase phase)
        {
            if (phaseText == null) return;
            string display = phase switch
            {
                GamePhase.Development => "Фаза развития",
                GamePhase.FoodDetermination => "Определение кормовой базы",
                GamePhase.Feeding => "Фаза питания",
                GamePhase.Extinction => "Фаза вымирания",
                GamePhase.GameOver => "Игра окончена",
                _ => phase.ToString()
            };
            phaseText.text = display;

            if (foodPoolText != null && gameManager != null)
                foodPoolText.text = $"Еды: {gameManager.State.FoodPool}";
            if (roundText != null && gameManager != null)
                roundText.text = $"Раунд {gameManager.State.RoundNumber}";
        }

        private void UpdateCurrentPlayer(Player player)
        {
            if (currentPlayerText != null)
                currentPlayerText.text = $"Ходит: {player.Name}";
        }

        private void HandleGameOver(Player winner)
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            if (winnerText != null)
                winnerText.text = $"Победитель: {winner.Name}\n" +
                                  $"Очки: {winner.CalculateScore()}";
        }
    }
}
