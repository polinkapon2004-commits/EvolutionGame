using System;
using System.Collections.Generic;
using System.Linq;
using EvolutionGame.Properties;

namespace EvolutionGame.Cards
{
    /// <summary>
    /// Класс существа на игровом поле. Существо принадлежит одному из игроков
    /// и может иметь набор уникальных свойств (исключение — "Жировой запас").
    /// </summary>
    public class Creature
    {
        /// <summary>Уникальный идентификатор существа в текущей партии.</summary>
        public int Id { get; private set; }

        /// <summary>Идентификатор игрока, которому принадлежит существо.</summary>
        public int OwnerPlayerId { get; private set; }

        /// <summary>Список свойств, наложенных на существо.</summary>
        public List<Property> Properties { get; private set; } = new List<Property>();

        /// <summary>Количество фишек еды, полученных существом в текущей фазе питания.</summary>
        public int CurrentFood { get; private set; } = 0;

        /// <summary>Количество фишек жира, накопленных по свойству "Жировой запас".</summary>
        public int FatStorage { get; private set; } = 0;

        /// <summary>Признак того, что существо использовало "Спячку" в прошлый ход.</summary>
        public bool WasHibernatingLastTurn { get; set; } = false;

        /// <summary>Признак того, что существо ядовито (после использования "Отбрасывания хвоста" сбрасывается).</summary>
        public bool IsAlive { get; private set; } = true;

        /// <summary>Признак активного состояния "Спячка" в текущей фазе питания.</summary>
        public bool IsHibernating { get; set; } = false;

        public Creature(int id, int ownerPlayerId)
        {
            Id = id;
            OwnerPlayerId = ownerPlayerId;
        }

        /// <summary>
        /// Базовая потребность существа в еде (1 единица), модифицированная свойствами.
        /// Свойства "Хищник" и "Большой" добавляют по +1, "Паразит" добавляет +2.
        /// </summary>
        public int RequiredFood
        {
            get
            {
                int baseRequirement = 1;
                foreach (var prop in Properties)
                {
                    baseRequirement += prop.ExtraFoodRequired;
                }
                return baseRequirement;
            }
        }

        /// <summary>Существо считается накормленным, когда CurrentFood >= RequiredFood.</summary>
        public bool IsFed => CurrentFood >= RequiredFood;

        /// <summary>
        /// Может ли существо принять данное свойство (проверка дублирования).
        /// </summary>
        public bool CanAddProperty(Property newProp)
        {
            if (newProp == null) return false;
            // "Жировой запас" — единственное свойство, которое может дублироваться
            if (newProp.Name == "Жировой запас") return true;
            return !Properties.Any(p => p.Name == newProp.Name);
        }

        /// <summary>Добавляет свойство существу, если это допустимо.</summary>
        public void AddProperty(Property prop)
        {
            if (!CanAddProperty(prop))
                throw new InvalidOperationException(
                    $"Свойство '{prop.Name}' уже наложено на существо #{Id} и не может дублироваться.");
            Properties.Add(prop);
            prop.OnApplied(this);
        }

        /// <summary>
        /// Кормит существо одной фишкой еды. Излишки уходят в "Жировой запас", если он есть.
        /// </summary>
        public void Feed(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                if (CurrentFood < RequiredFood)
                {
                    CurrentFood++;
                }
                else if (HasFatStorageCapacity())
                {
                    FatStorage++;
                }
                // если и жир заполнен — еда теряется
            }
        }

        /// <summary>Сколько свободных слотов в "Жировом запасе" у существа.</summary>
        public int FatStorageCapacity =>
            Properties.Count(p => p.Name == "Жировой запас");

        /// <summary>Есть ли свободное место в "Жировом запасе".</summary>
        public bool HasFatStorageCapacity() => FatStorage < FatStorageCapacity;

        /// <summary>Использует одну фишку жира для замены недостающей еды.</summary>
        public bool TryUseFat()
        {
            if (FatStorage > 0 && CurrentFood < RequiredFood)
            {
                FatStorage--;
                CurrentFood++;
                return true;
            }
            return false;
        }

        /// <summary>Помечает существо как погибшее (помещаемое в сброс).</summary>
        public void MarkDead() => IsAlive = false;

        /// <summary>Сбрасывает состояние существа в конце фазы вымирания (новая фаза питания).</summary>
        public void ResetForNewTurn()
        {
            CurrentFood = 0;
            IsHibernating = false;
        }

        /// <summary>Очки за существо при подсчёте: 2 + бонусы свойств.</summary>
        public int GetVictoryPoints()
        {
            int points = 2;
            foreach (var prop in Properties)
            {
                points += 1 + prop.BonusPoints;
            }
            return points;
        }

        public override string ToString() =>
            $"Creature[{Id}] of P{OwnerPlayerId}, props={Properties.Count}, food={CurrentFood}/{RequiredFood}";
    }
}
