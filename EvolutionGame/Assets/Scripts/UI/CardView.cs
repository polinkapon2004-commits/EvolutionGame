using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EvolutionGame.Cards;

namespace EvolutionGame.UI
{
    /// <summary>
    /// Визуальное представление одной карты в руке игрока.
    /// Прикреплён к префабу cardPrefab. Реагирует на наведение курсора и клики.
    /// </summary>
    public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Элементы карты")]
        public Image frontImage;
        public Text propertyNameText;
        public Text descriptionText;
        public GameObject highlightFrame;

        private Card _card;
        private HandUIController _hand;

        /// <summary>
        /// Привязывает данные модели Card к визуальному представлению.
        /// </summary>
        public void Bind(Card card, HandUIController hand)
        {
            _card = card;
            _hand = hand;

            if (propertyNameText != null)
                propertyNameText.text = card.DisplayName;
            if (descriptionText != null && card.AvailableProperties.Count > 0)
                descriptionText.text = card.AvailableProperties[0].Description;
            if (highlightFrame != null) highlightFrame.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // При наведении показываем подсказку (упрощённо: меняем масштаб)
            transform.localScale = Vector3.one * 1.1f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_hand == null || _card == null) return;
            _hand.SelectCard(_card);
            if (highlightFrame != null) highlightFrame.SetActive(true);
        }

        public void Deselect()
        {
            if (highlightFrame != null) highlightFrame.SetActive(false);
        }
    }
}
