using System;
using System.Collections.Generic;
using System.Linq;
using EvolutionGame.Cards;

namespace EvolutionGame.Core
{
    /// <summary>
    /// Состояние игры. Хранит ссылку на всех игроков, колоду, кормовую базу
    /// и текущую фазу. Является единственным источником истины об игре.
    /// </summary>
    public class GameState
    {
        /// <summary>Список игроков, участвующих в партии (от 2 до 4).</summary>
        public List<Player> Players { get; private set; }

        /// <summary>Основная колода карт.</summary>
        public Deck MainDeck { get; private set; }

        /// <summary>Текущая фаза игрового цикла.</summary>
        public GamePhase CurrentPhase { get; set; } = GamePhase.Development;

        /// <summary>Индекс игрока, чей сейчас ход.</summary>
        public int CurrentPlayerIndex { get; set; } = 0;

        /// <summary>Размер кормовой базы в текущей фазе питания.</summary>
        public int FoodPool { get; set; } = 0;

        /// <summary>Номер текущего игрового цикла (для статистики и тай-брейкеров).</summary>
        public int RoundNumber { get; set; } = 1;

        /// <summary>Признак того, что колода пуста и текущий цикл — последний.</summary>
        public bool IsLastRound { get; set; } = false;

        /// <summary>Игрок, ходящий в данный момент.</summary>
        public Player CurrentPlayer => Players[CurrentPlayerIndex];

        /// <summary>Глобальный счётчик ID существ для уникальной идентификации.</summary>
        private int _nextCreatureId = 1;

        public GameState(List<Player> players, Deck mainDeck)
        {
            if (players == null || players.Count < 2 || players.Count > 4)
                throw new ArgumentException("Игра поддерживает от 2 до 4 игроков.", nameof(players));

            Players = players;
            MainDeck = mainDeck ?? throw new ArgumentNullException(nameof(mainDeck));
        }

        /// <summary>Выдаёт новый уникальный идентификатор существа.</summary>
        public int GetNextCreatureId() => _nextCreatureId++;

        /// <summary>Все существа на поле всех игроков.</summary>
        public IEnumerable<Creature> AllCreatures =>
            Players.SelectMany(p => p.Creatures);

        /// <summary>
        /// Переходит ход к следующему игроку, который ещё не пасанул в текущей фазе.
        /// Возвращает true, если такой игрок найден; false — если все спасовали.
        /// </summary>
        public bool TryMoveToNextActivePlayer()
        {
            int n = Players.Count;
            for (int i = 1; i <= n; i++)
            {
                int idx = (CurrentPlayerIndex + i) % n;
                if (CurrentPhase == GamePhase.Development && !Players[idx].HasPassedDevelopment)
                {
                    CurrentPlayerIndex = idx;
                    return true;
                }
                if (CurrentPhase == GamePhase.Feeding && !Players[idx].HasFinishedFeeding)
                {
                    CurrentPlayerIndex = idx;
                    return true;
                }
            }
            return false;
        }

        /// <summary>Все ли игроки завершили свои действия в текущей фазе.</summary>
        public bool AllPlayersFinishedCurrentPhase()
        {
            return CurrentPhase switch
            {
                GamePhase.Development => Players.All(p => p.HasPassedDevelopment),
                GamePhase.Feeding => Players.All(p => p.HasFinishedFeeding),
                _ => true
            };
        }
    }
}
