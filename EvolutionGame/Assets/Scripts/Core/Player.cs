using System;
using System.Collections.Generic;
using EvolutionGame.Cards;

namespace EvolutionGame.Core
{
    /// <summary>
    /// Класс игрока. Хранит руку, поле существ, сброс и состояние "пасует ли в текущей фазе".
    /// </summary>
    public class Player
    {
        /// <summary>Идентификатор игрока (1..4).</summary>
        public int Id { get; private set; }

        /// <summary>Имя игрока для отображения в интерфейсе.</summary>
        public string Name { get; set; }

        /// <summary>Является ли игрок ботом.</summary>
        public bool IsBot { get; private set; }

        /// <summary>Цвет существ этого игрока (HEX-строка для шейдера).</summary>
        public string Color { get; set; }

        /// <summary>Карты в руке игрока.</summary>
        public List<Card> Hand { get; private set; } = new List<Card>();

        /// <summary>Существа на поле игрока.</summary>
        public List<Creature> Creatures { get; private set; } = new List<Creature>();

        /// <summary>Колода сброса игрока (для подсчёта тай-брейкера в конце игры).</summary>
        public List<Card> DiscardPile { get; private set; } = new List<Card>();

        /// <summary>Спасовал ли игрок в текущей фазе развития.</summary>
        public bool HasPassedDevelopment { get; set; } = false;

        /// <summary>Завершил ли игрок свои действия в текущей фазе питания.</summary>
        public bool HasFinishedFeeding { get; set; } = false;

        public Player(int id, string name, bool isBot, string color)
        {
            Id = id;
            Name = name;
            IsBot = isBot;
            Color = color;
        }

        /// <summary>Берёт карту в руку.</summary>
        public void TakeCard(Card card) => Hand.Add(card);

        /// <summary>
        /// Удаляет карту из руки. Возвращает true, если карта была найдена и удалена.
        /// </summary>
        public bool RemoveFromHand(Card card) => Hand.Remove(card);

        /// <summary>Сбрасывает карту в свой сброс.</summary>
        public void Discard(Card card) => DiscardPile.Add(card);

        /// <summary>Создаёт новое существо у игрока на основе карты (карта при этом тратится).</summary>
        public Creature CreateCreatureFromCard(Card card, int newCreatureId)
        {
            if (!RemoveFromHand(card))
                throw new InvalidOperationException("Карта не найдена в руке игрока.");
            var creature = new Creature(newCreatureId, Id);
            Creatures.Add(creature);
            // Карта-"рубашка" уходит в сброс при создании существа
            Discard(card);
            return creature;
        }

        /// <summary>Удаляет вымершее существо с поля и помещает связанные карты в сброс.</summary>
        public void RemoveCreature(Creature creature)
        {
            Creatures.Remove(creature);
        }

        /// <summary>Вычисляет очки игрока в конце партии согласно правилам.</summary>
        public int CalculateScore()
        {
            int total = 0;
            foreach (var creature in Creatures)
            {
                total += creature.GetVictoryPoints();
            }
            return total;
        }

        /// <summary>Сбрасывает флаги "пасанул" в начале новой фазы развития.</summary>
        public void ResetPhaseFlags()
        {
            HasPassedDevelopment = false;
            HasFinishedFeeding = false;
        }

        public override string ToString() =>
            $"Player[{Id}] {Name} ({(IsBot ? "Bot" : "Human")}), hand={Hand.Count}, creatures={Creatures.Count}";
    }
}
