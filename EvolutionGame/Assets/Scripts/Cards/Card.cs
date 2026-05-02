using System;
using System.Collections.Generic;
using EvolutionGame.Properties;

namespace EvolutionGame.Cards
{
    /// <summary>
    /// Класс игровой карты. Каждая карта содержит одно или два свойства,
    /// одно из которых игрок выбирает при разыгрывании.
    /// При разыгрывании "рубашкой вверх" любая карта превращается в новое существо.
    /// </summary>
    public class Card
    {
        /// <summary>Уникальный идентификатор карты в колоде.</summary>
        public int Id { get; private set; }

        /// <summary>Список доступных свойств на карте (1 или 2 варианта на выбор).</summary>
        public List<Property> AvailableProperties { get; private set; }

        /// <summary>Спрайт лицевой стороны карты для отображения в UI.</summary>
        public string FrontSpriteName { get; private set; }

        /// <summary>Краткое название карты для отладки и логирования.</summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Конструктор карты с одним свойством.
        /// </summary>
        public Card(int id, Property property, string spriteName = "")
        {
            Id = id;
            AvailableProperties = new List<Property> { property };
            DisplayName = property.Name;
            FrontSpriteName = string.IsNullOrEmpty(spriteName) ? "card_default" : spriteName;
        }

        /// <summary>
        /// Конструктор карты с двумя свойствами на выбор.
        /// </summary>
        public Card(int id, Property property1, Property property2, string spriteName = "")
        {
            if (property1 == null) throw new ArgumentNullException(nameof(property1));
            if (property2 == null) throw new ArgumentNullException(nameof(property2));

            Id = id;
            AvailableProperties = new List<Property> { property1, property2 };
            DisplayName = $"{property1.Name} / {property2.Name}";
            FrontSpriteName = string.IsNullOrEmpty(spriteName) ? "card_default" : spriteName;
        }

        /// <summary>
        /// Возвращает свойство по индексу (0 или 1) для разыгрывания на существе.
        /// </summary>
        public Property GetPropertyAt(int index)
        {
            if (index < 0 || index >= AvailableProperties.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return AvailableProperties[index];
        }

        /// <summary>
        /// Содержит ли карта парные свойства (требующие двух существ).
        /// </summary>
        public bool HasPairedProperty()
        {
            return AvailableProperties.Exists(p => p.IsPaired);
        }

        public override string ToString() => $"Card[{Id}] {DisplayName}";
    }
}
