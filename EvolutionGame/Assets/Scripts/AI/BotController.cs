using System;
using System.Collections.Generic;
using System.Linq;
using EvolutionGame.Cards;
using EvolutionGame.Core;
using UnityEngine;

namespace EvolutionGame.AI
{
    /// <summary>
    /// Контроллер ИИ для игроков-ботов.
    /// Принимает случайные, но валидные решения. Стремится:
    /// 1) Создавать сбалансированных существ (не слишком много свойств на одном).
    /// 2) Атаковать слабых противников при наличии свойства "Хищник".
    /// 3) Кормить наиболее ценных существ в первую очередь.
    /// </summary>
    public class BotController : MonoBehaviour
    {
        [Tooltip("Задержка между действиями бота, чтобы игрок видел его ходы.")]
        public float actionDelay = 1.0f;

        [Tooltip("Ссылка на менеджер игры.")]
        public GameManager gameManager;

        private System.Random _rng = new System.Random();

        /// <summary>
        /// Принять решение бота в текущем состоянии. Вызывается извне, когда наступает ход бота.
        /// </summary>
        public void TakeTurn(Player botPlayer)
        {
            if (!botPlayer.IsBot)
                throw new ArgumentException("BotController может действовать только за бота.");

            switch (gameManager.State.CurrentPhase)
            {
                case GamePhase.Development:
                    DecideDevelopmentAction(botPlayer);
                    break;
                case GamePhase.Feeding:
                    DecideFeedingAction(botPlayer);
                    break;
            }
        }

        private void DecideDevelopmentAction(Player bot)
        {
            if (bot.Hand.Count == 0)
            {
                gameManager.Pass(bot);
                return;
            }

            // Стратегия: если у бота нет существ — обязательно создать новое.
            // Иначе — с вероятностью 30% создать ещё одно, в остальных случаях добавить свойство.
            bool needNewCreature = bot.Creatures.Count == 0 || _rng.NextDouble() < 0.3;

            if (needNewCreature)
            {
                var card = bot.Hand[_rng.Next(bot.Hand.Count)];
                gameManager.PlayCardAsCreature(bot, card);
                return;
            }

            // Пытаемся наложить свойство, при невозможности — создаём существо
            for (int attempt = 0; attempt < bot.Hand.Count; attempt++)
            {
                var card = bot.Hand[_rng.Next(bot.Hand.Count)];
                int propIdx = _rng.Next(card.AvailableProperties.Count);
                var prop = card.GetPropertyAt(propIdx);

                Creature target;
                if (prop.Name == "Паразит")
                {
                    // Цель Паразита — самое сильное чужое существо
                    var enemyCreatures = gameManager.State.Players
                        .Where(p => p.Id != bot.Id)
                        .SelectMany(p => p.Creatures)
                        .Where(c => c.CanAddProperty(prop))
                        .ToList();
                    if (enemyCreatures.Count == 0) continue;
                    target = enemyCreatures.OrderByDescending(c => c.Properties.Count).First();
                }
                else
                {
                    var validTargets = bot.Creatures.Where(c => c.CanAddProperty(prop)).ToList();
                    if (validTargets.Count == 0) continue;
                    target = validTargets[_rng.Next(validTargets.Count)];
                }

                try
                {
                    gameManager.PlayCardAsProperty(bot, card, propIdx, target);
                    return;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // Если ничего не получилось — пасуем
            gameManager.Pass(bot);
        }

        private void DecideFeedingAction(Player bot)
        {
            // В упрощённой версии бот в фазе питания просто пасует.
            // В будущих расширениях здесь будет приоритизация существ по ценности
            // и использование активных свойств "Хищник" и "Пиратство".
            gameManager.Pass(bot);
        }
    }
}
