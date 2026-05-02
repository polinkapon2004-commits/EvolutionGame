using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EvolutionGame.Cards;
using EvolutionGame.Core;

namespace EvolutionGame.UI
{
    /// <summary>
    /// Контроллер отображения карт в руке текущего игрока.
    /// Создаёт визуальные представления карт и обрабатывает их выбор.
    /// </summary>
    public class HandUIController : MonoBehaviour
    {
        [Header("Префаб визуального представления карты")]
        public GameObject cardPrefab;

        [Header("Контейнер для карт (Horizontal Layout)")]
        public Transform cardsContainer;

        [Header("Ссылка на менеджер игры")]
        public GameManager gameManager;

        private readonly List<GameObject> _spawnedCards = new List<GameObject>();
        private Card _selectedCard;

        public Card SelectedCard => _selectedCard;

        private void Start()
        {
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.OnPlayerTurnChanged += RefreshHand;
            }
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnPlayerTurnChanged -= RefreshHand;
            }
        }

        /// <summary>
        /// Перерисовывает руку игрока, чей сейчас ход.
        /// </summary>
        public void RefreshHand(Player player)
        {
            ClearCards();
            if (player.IsBot) return;

            foreach (var card in player.Hand)
            {
                if (cardPrefab == null) continue;
                var cardObj = Instantiate(cardPrefab, cardsContainer);
                var view = cardObj.GetComponent<CardView>();
                if (view != null)
                {
                    view.Bind(card, this);
                }
                _spawnedCards.Add(cardObj);
            }
        }

        private void ClearCards()
        {
            foreach (var go in _spawnedCards) Destroy(go);
            _spawnedCards.Clear();
            _selectedCard = null;
        }

        /// <summary>
        /// Выделяет карту по нажатию (вызывается из CardView).
        /// </summary>
        public void SelectCard(Card card)
        {
            _selectedCard = card;
        }
    }
}
