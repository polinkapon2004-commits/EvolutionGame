using System;
using System.Collections.Generic;
using EvolutionGame.Cards;

namespace EvolutionGame.Core
{
    /// <summary>
    /// Колода игры. Согласно правилам "Эволюции" общая колода содержит 84 карты.
    /// Колода перетасовывается в начале партии и постепенно расходуется.
    /// </summary>
    public class Deck
    {
        private readonly List<Card> _cards;
        private readonly Random _rng;

        /// <summary>Количество карт, оставшихся в колоде.</summary>
        public int Count => _cards.Count;

        /// <summary>Признак того, что колода полностью пуста (запускает финальный цикл).</summary>
        public bool IsEmpty => _cards.Count == 0;

        public Deck(IEnumerable<Card> cards, int? seed = null)
        {
            _cards = new List<Card>(cards);
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>
        /// Перемешивает колоду по алгоритму Фишера — Йетса.
        /// </summary>
        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        /// <summary>
        /// Тянет одну карту из верха колоды. Возвращает null, если колода пуста.
        /// </summary>
        public Card Draw()
        {
            if (_cards.Count == 0) return null;
            var card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }

        /// <summary>
        /// Тянет указанное количество карт из колоды. Возвращает столько,
        /// сколько фактически было доступно.
        /// </summary>
        public List<Card> DrawMany(int count)
        {
            var result = new List<Card>();
            for (int i = 0; i < count; i++)
            {
                var card = Draw();
                if (card == null) break;
                result.Add(card);
            }
            return result;
        }
    }
}
