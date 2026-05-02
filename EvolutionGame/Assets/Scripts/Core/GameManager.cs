using System;
using System.Collections.Generic;
using System.Linq;
using EvolutionGame.Cards;
using EvolutionGame.Properties;
using UnityEngine;

namespace EvolutionGame.Core
{
    /// <summary>
    /// Главный контроллер игровой сессии. Прикреплён к GameObject в сцене Game.
    /// Управляет всем игровым циклом: создание состояния, переход между фазами,
    /// обработка действий игроков и завершение партии.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Параметры партии")]
        [Tooltip("Количество игроков-людей (от 0 до 4 совместно с ботами).")]
        [Range(0, 4)] public int humanPlayersCount = 2;

        [Tooltip("Количество ботов (для режима «Игра с ботом»).")]
        [Range(0, 3)] public int botPlayersCount = 0;

        [Tooltip("Зерно генератора случайных чисел. 0 = случайное.")]
        public int randomSeed = 0;

        /// <summary>Текущее состояние партии. Доступно UI и AI.</summary>
        public GameState State { get; private set; }

        /// <summary>Событие, возникающее при переходе на новую фазу.</summary>
        public event Action<GamePhase> OnPhaseChanged;

        /// <summary>Событие, возникающее при завершении партии. Передаёт победителя.</summary>
        public event Action<Player> OnGameOver;

        /// <summary>Событие, возникающее при смене активного игрока.</summary>
        public event Action<Player> OnPlayerTurnChanged;

        private System.Random _foodRng;

        private void Start()
        {
            InitializeGame();
            BeginRound();
        }

        /// <summary>
        /// Инициализирует игроков, колоду и стартовое состояние партии.
        /// </summary>
        private void InitializeGame()
        {
            int totalPlayers = humanPlayersCount + botPlayersCount;
            if (totalPlayers < 2 || totalPlayers > 4)
            {
                Debug.LogError(
                    $"Некорректное число игроков: {totalPlayers}. Должно быть от 2 до 4.");
                totalPlayers = Mathf.Clamp(totalPlayers, 2, 4);
            }

            var players = new List<Player>();
            string[] colors = { "#E74C3C", "#3498DB", "#2ECC71", "#F39C12" };
            for (int i = 0; i < humanPlayersCount; i++)
            {
                players.Add(new Player(i + 1, $"Игрок {i + 1}", false, colors[i]));
            }
            for (int i = 0; i < botPlayersCount; i++)
            {
                int idx = humanPlayersCount + i;
                players.Add(new Player(idx + 1, $"Бот {i + 1}", true, colors[idx]));
            }

            int? seed = randomSeed == 0 ? (int?)null : randomSeed;
            _foodRng = seed.HasValue ? new System.Random(seed.Value + 1) : new System.Random();

            var deck = new Deck(DeckFactory.CreateStandardDeck(), seed);
            deck.Shuffle();

            State = new GameState(players, deck);

            // Раздача стартовой руки: каждому игроку по 6 карт
            foreach (var player in players)
            {
                var initialHand = deck.DrawMany(6);
                foreach (var card in initialHand) player.TakeCard(card);
            }

            Debug.Log($"Игра инициализирована. Игроков: {players.Count}, карт в колоде: {deck.Count}");
        }

        /// <summary>
        /// Начинает новый игровой цикл с фазы развития.
        /// </summary>
        private void BeginRound()
        {
            foreach (var p in State.Players) p.ResetPhaseFlags();
            State.CurrentPhase = GamePhase.Development;
            State.CurrentPlayerIndex = 0;
            OnPhaseChanged?.Invoke(GamePhase.Development);
            OnPlayerTurnChanged?.Invoke(State.CurrentPlayer);
            Debug.Log($"Раунд {State.RoundNumber}. Начинается фаза развития.");
        }

        /// <summary>
        /// Действие игрока: разыграть карту как новое существо.
        /// </summary>
        public void PlayCardAsCreature(Player player, Card card)
        {
            ValidateTurn(player, GamePhase.Development);
            int newId = State.GetNextCreatureId();
            player.CreateCreatureFromCard(card, newId);
            AdvanceTurn();
        }

        /// <summary>
        /// Действие игрока: разыграть карту как свойство на существо.
        /// </summary>
        public void PlayCardAsProperty(Player player, Card card, int propertyIndex, Creature target)
        {
            ValidateTurn(player, GamePhase.Development);
            if (target.OwnerPlayerId != player.Id)
            {
                var prop = card.GetPropertyAt(propertyIndex);
                // Только "Паразит" разрешено играть на чужих существ
                if (prop.Name != "Паразит")
                    throw new InvalidOperationException(
                        "Свойство можно накладывать только на свои существа (кроме «Паразита»).");
            }

            var property = card.GetPropertyAt(propertyIndex);
            if (!target.CanAddProperty(property))
                throw new InvalidOperationException(
                    $"Нельзя наложить '{property.Name}' на это существо (дублирование).");

            target.AddProperty(property);
            player.RemoveFromHand(card);
            player.Discard(card);
            AdvanceTurn();
        }

        /// <summary>
        /// Действие игрока: спасовать в текущей фазе.
        /// </summary>
        public void Pass(Player player)
        {
            if (State.CurrentPhase == GamePhase.Development)
            {
                player.HasPassedDevelopment = true;
            }
            else if (State.CurrentPhase == GamePhase.Feeding)
            {
                player.HasFinishedFeeding = true;
            }
            AdvanceTurn();
        }

        private void ValidateTurn(Player player, GamePhase expected)
        {
            if (State.CurrentPhase != expected)
                throw new InvalidOperationException(
                    $"Действие недоступно: ожидалась фаза {expected}, а сейчас {State.CurrentPhase}.");
            if (State.CurrentPlayer.Id != player.Id)
                throw new InvalidOperationException(
                    $"Сейчас не ход игрока #{player.Id}.");
        }

        /// <summary>
        /// Сдвигает ход на следующего игрока, либо переходит в следующую фазу,
        /// если все спасовали или закончили действия.
        /// </summary>
        private void AdvanceTurn()
        {
            // Автоматический пас, если у игрока закончились карты в фазе развития
            if (State.CurrentPhase == GamePhase.Development &&
                State.CurrentPlayer.Hand.Count == 0)
            {
                State.CurrentPlayer.HasPassedDevelopment = true;
            }

            if (State.AllPlayersFinishedCurrentPhase())
            {
                MoveToNextPhase();
                return;
            }
            State.TryMoveToNextActivePlayer();
            OnPlayerTurnChanged?.Invoke(State.CurrentPlayer);
        }

        private void MoveToNextPhase()
        {
            switch (State.CurrentPhase)
            {
                case GamePhase.Development:
                    State.CurrentPhase = GamePhase.FoodDetermination;
                    DetermineFoodPool();
                    break;
                case GamePhase.FoodDetermination:
                    State.CurrentPhase = GamePhase.Feeding;
                    foreach (var p in State.Players) p.HasFinishedFeeding = false;
                    State.CurrentPlayerIndex = 0;
                    break;
                case GamePhase.Feeding:
                    State.CurrentPhase = GamePhase.Extinction;
                    PerformExtinction();
                    break;
                case GamePhase.Extinction:
                    if (State.MainDeck.IsEmpty || State.IsLastRound)
                    {
                        FinishGame();
                        return;
                    }
                    State.RoundNumber++;
                    BeginRound();
                    return;
            }
            OnPhaseChanged?.Invoke(State.CurrentPhase);
        }

        /// <summary>
        /// Случайным образом определяет размер кормовой базы по правилам:
        /// 2 игрока → 3-8, 3 игрока → 2-12, 4 игрока → 4-14.
        /// </summary>
        private void DetermineFoodPool()
        {
            int n = State.Players.Count;
            (int min, int max) range = n switch
            {
                2 => (3, 8),
                3 => (2, 12),
                4 => (4, 14),
                _ => (3, 10)
            };
            State.FoodPool = _foodRng.Next(range.min, range.max + 1);
            Debug.Log($"Кормовая база этого хода: {State.FoodPool}");
            // Сразу автоматически переходим к фазе питания
            MoveToNextPhase();
        }

        /// <summary>
        /// Удаляет ненакормленных существ и раздаёт карты на следующий цикл.
        /// </summary>
        private void PerformExtinction()
        {
            foreach (var player in State.Players)
            {
                var dying = player.Creatures.Where(c => !c.IsFed && !c.IsHibernating).ToList();
                foreach (var creature in dying)
                {
                    player.RemoveCreature(creature);
                    Debug.Log($"Игрок {player.Name} теряет существо #{creature.Id}.");
                }
                // Сброс счётчиков еды для оставшихся существ
                foreach (var c in player.Creatures) c.ResetForNewTurn();
            }

            // Раздача новых карт
            foreach (var player in State.Players)
            {
                int cardsToDraw = player.Creatures.Count + 1;
                if (player.Creatures.Count == 0 && player.Hand.Count == 0)
                    cardsToDraw = 6;
                var newCards = State.MainDeck.DrawMany(cardsToDraw);
                foreach (var c in newCards) player.TakeCard(c);
            }

            if (State.MainDeck.IsEmpty)
            {
                State.IsLastRound = true;
                Debug.Log("Колода пуста. Следующий цикл будет последним.");
            }

            MoveToNextPhase();
        }

        /// <summary>
        /// Подсчёт очков и определение победителя.
        /// </summary>
        private void FinishGame()
        {
            State.CurrentPhase = GamePhase.GameOver;
            OnPhaseChanged?.Invoke(GamePhase.GameOver);

            var ranking = State.Players
                .OrderByDescending(p => p.CalculateScore())
                .ThenByDescending(p => p.DiscardPile.Count)
                .ToList();

            var winner = ranking.First();
            Debug.Log($"=== Игра окончена. Победитель: {winner.Name} ({winner.CalculateScore()} очков) ===");
            foreach (var p in ranking)
            {
                Debug.Log($"  {p.Name}: {p.CalculateScore()} очков, сброс: {p.DiscardPile.Count}");
            }
            OnGameOver?.Invoke(winner);
        }
    }
}
