using System;

namespace EvolutionGame.Core
{
    /// <summary>
    /// Фазы игрового цикла, через которые последовательно проходит каждая партия.
    /// </summary>
    public enum GamePhase
    {
        /// <summary>Стартовая фаза, в которой игроки разыгрывают карты для создания существ и наделения их свойствами.</summary>
        Development,

        /// <summary>Фаза, в которой случайным образом определяется размер кормовой базы текущего раунда.</summary>
        FoodDetermination,

        /// <summary>Фаза, в которой игроки кормят своих существ и используют активные свойства.</summary>
        Feeding,

        /// <summary>Фаза, в которой ненакормленные существа вымирают, а игроки добирают новые карты.</summary>
        Extinction,

        /// <summary>Завершение партии и подсчёт итоговых очков.</summary>
        GameOver
    }

    /// <summary>
    /// Типы свойств существ для категоризации в интерфейсе и логике.
    /// </summary>
    public enum PropertyType
    {
        /// <summary>Свойство применяется единожды и не требует активации игроком.</summary>
        Passive,

        /// <summary>Свойство активируется игроком в свой ход в фазе питания.</summary>
        Active,

        /// <summary>Свойство срабатывает в ответ на действия другого игрока.</summary>
        Reactive,

        /// <summary>Свойство накладывается одновременно на двух существ.</summary>
        Paired
    }

    /// <summary>
    /// Возможные действия игрока в фазе развития.
    /// </summary>
    public enum DevelopmentAction
    {
        CreateCreature,
        AddProperty,
        Pass
    }

    /// <summary>
    /// Возможные действия игрока в фазе питания.
    /// </summary>
    public enum FeedingAction
    {
        TakeFood,
        UseActiveProperty,
        Pass
    }
}
