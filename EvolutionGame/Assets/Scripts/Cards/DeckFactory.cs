using System;
using System.Collections.Generic;
using EvolutionGame.Properties;

namespace EvolutionGame.Cards
{
    /// <summary>
    /// Фабрика стандартной колоды настольной игры "Эволюция".
    /// Состав колоды — 84 карты с распределением, близким к оригинальной игре.
    /// </summary>
    public static class DeckFactory
    {
        /// <summary>
        /// Возвращает строго фиксированную таблицу количества карт каждого типа свойств,
        /// согласно которой собирается колода в начале партии.
        /// </summary>
        private static Dictionary<string, int> GetCardDistribution()
        {
            return new Dictionary<string, int>
            {
                { "Хищник",            4 },
                { "Большой",           4 },
                { "Быстрое",           4 },
                { "Взаимодействие",    4 },
                { "Водоплавающее",     4 },
                { "Камуфляж",          4 },
                { "Острое зрение",     4 },
                { "Мимикрия",          4 },
                { "Норное",            4 },
                { "Жировой запас",     8 },
                { "Сотрудничество",    4 },
                { "Спячка",            4 },
                { "Отбрасывание хвоста", 4 },
                { "Падальщик",         4 },
                { "Паразит",           4 },
                { "Пиратство",         4 },
                { "Симбиоз",           4 },
                { "Топотун",           4 },
                { "Ядовитое",          4 },
                // Итого: 4*18 + 8 + 4 = 84
            };
        }

        /// <summary>
        /// Создаёт стандартную колоду из 84 карт. Каждая карта несёт ровно одно свойство
        /// для упрощения логики (двухсвойственные карты — расширение для будущих версий).
        /// </summary>
        public static List<Card> CreateStandardDeck()
        {
            var distribution = GetCardDistribution();
            var cards = new List<Card>();
            int id = 1;

            foreach (var kv in distribution)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    var prop = PropertyImplementations.CreateByName(kv.Key);
                    var card = new Card(id++, prop, $"card_{kv.Key.ToLower()}");
                    cards.Add(card);
                }
            }

            return cards;
        }
    }
}
