using System;
using EvolutionGame.Core;

namespace EvolutionGame.Properties
{
    /// <summary>
    /// Абстрактный базовый класс для всех свойств существ.
    /// Каждое свойство переопределяет нужные ему методы для реализации специфической логики.
    /// </summary>
    public abstract class Property
    {
        /// <summary>Отображаемое название свойства.</summary>
        public string Name { get; protected set; }

        /// <summary>Полное описание свойства для отображения во всплывающей подсказке.</summary>
        public string Description { get; protected set; }

        /// <summary>Тип свойства (пассивное, активное, реактивное, парное).</summary>
        public PropertyType Type { get; protected set; }

        /// <summary>Дополнительная еда, требуемая существу из-за этого свойства.</summary>
        public int ExtraFoodRequired { get; protected set; } = 0;

        /// <summary>Дополнительные очки, получаемые за это свойство при подсчёте.</summary>
        public int BonusPoints { get; protected set; } = 0;

        /// <summary>Является ли свойство парным (требует двух существ).</summary>
        public bool IsPaired => Type == PropertyType.Paired;

        /// <summary>
        /// Вызывается при наложении свойства на существо.
        /// </summary>
        public virtual void OnApplied(Cards.Creature creature) { }

        /// <summary>
        /// Вызывается при попытке атаки на это существо.
        /// Возвращает true, если атака продолжается, false — если атака отклоняется.
        /// </summary>
        public virtual bool OnAttacked(Cards.Creature attacker, Cards.Creature target)
        {
            return true;
        }

        /// <summary>
        /// Может ли свойство быть активировано в текущем состоянии игры.
        /// По умолчанию возвращает false (свойство не требует активации).
        /// </summary>
        public virtual bool CanBeUsed(Cards.Creature creature, GameState state)
        {
            return false;
        }

        /// <summary>
        /// Вызывается, когда существо получает еду.
        /// Возвращает количество дополнительной еды, добавляемой механикой свойства.
        /// </summary>
        public virtual int OnFeeding(Cards.Creature creature)
        {
            return 0;
        }

        public override string ToString() => Name;
    }
}
