using System;
using EvolutionGame.Core;
using EvolutionGame.Cards;

namespace EvolutionGame.Properties
{
    // ===== ЗАЩИТНЫЕ СВОЙСТВА =====

    public class BigProperty : Property
    {
        public BigProperty()
        {
            Name = "Большой";
            Description = "Не может быть атаковано Хищником без свойства «Большой».";
            Type = PropertyType.Passive;
            ExtraFoodRequired = 1;
            BonusPoints = 1;
        }

        public override bool OnAttacked(Creature attacker, Creature target)
        {
            // Атака отклоняется, если у нападающего нет свойства «Большой»
            return attacker.HasProperty<BigProperty>();
        }
    }

    public class CamouflageProperty : Property
    {
        public CamouflageProperty()
        {
            Name = "Камуфляж";
            Description = "Может быть атаковано только хищником со свойством «Острое зрение».";
            Type = PropertyType.Passive;
        }

        public override bool OnAttacked(Creature attacker, Creature target)
        {
            return attacker.HasProperty<SharpVisionProperty>();
        }
    }

    public class SharpVisionProperty : Property
    {
        public SharpVisionProperty()
        {
            Name = "Острое зрение";
            Description = "Хищник с этим свойством может атаковать существо со свойством «Камуфляж».";
            Type = PropertyType.Passive;
        }
    }

    public class BurrowingProperty : Property
    {
        public BurrowingProperty()
        {
            Name = "Норное";
            Description = "Когда животное накормлено, оно не может быть атаковано хищником.";
            Type = PropertyType.Passive;
        }

        public override bool OnAttacked(Creature attacker, Creature target)
        {
            // Защита работает только если существо накормлено
            return !target.IsFed;
        }
    }

    public class SwimmingProperty : Property
    {
        public SwimmingProperty()
        {
            Name = "Водоплавающее";
            Description = "Не может быть атаковано хищником без этого же свойства.";
            Type = PropertyType.Passive;
        }

        public override bool OnAttacked(Creature attacker, Creature target)
        {
            return attacker.HasProperty<SwimmingProperty>();
        }
    }

    public class FastProperty : Property
    {
        private static readonly Random _random = new Random();

        public FastProperty()
        {
            Name = "Быстрое";
            Description = "С шансом 50% не будет съедено при атаке хищника.";
            Type = PropertyType.Reactive;
        }

        public override bool OnAttacked(Creature attacker, Creature target)
        {
            // С вероятностью 50% существо избегает атаки
            return _random.Next(2) == 0;
        }
    }

    // ===== АТАКУЮЩИЕ И АКТИВНЫЕ СВОЙСТВА =====

    public class PredatorProperty : Property
    {
        public PredatorProperty()
        {
            Name = "Хищник";
            Description = "Атакует другое существо, получая 2 фишки еды.";
            Type = PropertyType.Active;
            ExtraFoodRequired = 1;
            BonusPoints = 1;
        }

        public override bool CanBeUsed(Creature creature, GameState state)
        {
            // Можно использовать только в фазу питания и только если хищник не накормлен
            return state.CurrentPhase == GamePhase.Feeding
                   && !creature.IsFed
                   && !creature.HasUsedPredatorThisTurn;
        }
    }

    public class PiracyProperty : Property
    {
        public PiracyProperty()
        {
            Name = "Пиратство";
            Description = "Один раз за цикл забирает 1 фишку еды у другого ненакормленного существа.";
            Type = PropertyType.Active;
        }

        public override bool CanBeUsed(Creature creature, GameState state)
        {
            return state.CurrentPhase == GamePhase.Feeding
                   && !creature.HasUsedPiracyThisCycle
                   && !creature.IsFed;
        }
    }

    public class StomperProperty : Property
    {
        public StomperProperty()
        {
            Name = "Топотун";
            Description = "При использовании уничтожает одну фишку из кормовой базы.";
            Type = PropertyType.Active;
        }

        public override bool CanBeUsed(Creature creature, GameState state)
        {
            return state.CurrentPhase == GamePhase.Feeding && state.AvailableFood > 0;
        }
    }

    public class ParasiteProperty : Property
    {
        public ParasiteProperty()
        {
            Name = "Паразит";
            Description = "Накладывается на чужое существо, увеличивая его потребность в еде на 2.";
            Type = PropertyType.Active;
            ExtraFoodRequired = 2;
            BonusPoints = 2;
        }
    }

    // ===== СПЕЦИАЛЬНЫЕ СВОЙСТВА =====

    public class FatTissueProperty : Property
    {
        public FatTissueProperty()
        {
            Name = "Жировой запас";
            Description = "Позволяет накапливать излишки еды для следующих фаз питания.";
            Type = PropertyType.Passive;
        }

        public override int OnFeeding(Creature creature)
        {
            // Жировой запас позволяет получать еду сверх потребности существа
            return 0; // Логика накопления реализована в FeedingController
        }
    }

    public class HibernationProperty : Property
    {
        public HibernationProperty()
        {
            Name = "Спячка";
            Description = "Считается накормленным без еды. Нельзя использовать два хода подряд.";
            Type = PropertyType.Active;
        }

        public override bool CanBeUsed(Creature creature, GameState state)
        {
            return state.CurrentPhase == GamePhase.Feeding
                   && !creature.WasInHibernationLastTurn
                   && !state.IsLastTurn;
        }
    }

    public class TailLossProperty : Property
    {
        public TailLossProperty()
        {
            Name = "Отбрасывание хвоста";
            Description = "При атаке хищника свойство сбрасывается вместо смерти существа.";
            Type = PropertyType.Reactive;
        }

        public override bool OnAttacked(Creature attacker, Creature target)
        {
            // Существо теряет это свойство вместо смерти, хищник получает 1 еду вместо 2
            target.RemoveProperty(this);
            attacker.AddFood(1);
            return false; // Атака не приводит к смерти
        }
    }

    public class MimicryProperty : Property
    {
        public MimicryProperty()
        {
            Name = "Мимикрия";
            Description = "Перенаправляет атаку на другое существо игрока. Один раз за ход.";
            Type = PropertyType.Reactive;
        }
    }

    public class ScavengerProperty : Property
    {
        public ScavengerProperty()
        {
            Name = "Падальщик";
            Description = "Получает 1 фишку еды, когда другое существо съедено хищником.";
            Type = PropertyType.Reactive;
        }
    }

    public class PoisonousProperty : Property
    {
        public PoisonousProperty()
        {
            Name = "Ядовитое";
            Description = "Хищник, съевший это существо, погибает в фазу вымирания.";
            Type = PropertyType.Reactive;
        }
    }

    // ===== ПАРНЫЕ СВОЙСТВА =====

    public class SymbiosisProperty : Property
    {
        public SymbiosisProperty()
        {
            Name = "Симбиоз";
            Description = "Защищает второе существо, пока живо первое. Парное свойство.";
            Type = PropertyType.Paired;
        }
    }

    public class CooperationProperty : Property
    {
        public CooperationProperty()
        {
            Name = "Сотрудничество";
            Description = "При получении еды партнёр получает 1 еду из общего запаса. Парное.";
            Type = PropertyType.Paired;
        }
    }

    public class CommunicationProperty : Property
    {
        public CommunicationProperty()
        {
            Name = "Взаимодействие";
            Description = "Когда одно существо получает еду из базы, второе тоже получает. Парное.";
            Type = PropertyType.Paired;
        }
    }

    /// <summary>
    /// Статическая фабрика свойств. Создаёт экземпляр Property по русскому названию.
    /// Используется при формировании колоды и при сериализации/десериализации сохранений.
    /// </summary>
    public static class PropertyImplementations
    {
        public static Property CreateByName(string name)
        {
            return name switch
            {
                "Большой" => new BigProperty(),
                "Камуфляж" => new CamouflageProperty(),
                "Острое зрение" => new SharpVisionProperty(),
                "Норное" => new BurrowingProperty(),
                "Водоплавающее" => new SwimmingProperty(),
                "Быстрое" => new FastProperty(),
                "Хищник" => new PredatorProperty(),
                "Пиратство" => new PiracyProperty(),
                "Топотун" => new StomperProperty(),
                "Паразит" => new ParasiteProperty(),
                "Жировой запас" => new FatTissueProperty(),
                "Спячка" => new HibernationProperty(),
                "Отбрасывание хвоста" => new TailLossProperty(),
                "Мимикрия" => new MimicryProperty(),
                "Падальщик" => new ScavengerProperty(),
                "Ядовитое" => new PoisonousProperty(),
                "Симбиоз" => new SymbiosisProperty(),
                "Сотрудничество" => new CooperationProperty(),
                "Взаимодействие" => new CommunicationProperty(),
                _ => throw new System.ArgumentException(
                    $"Неизвестное название свойства: '{name}'", nameof(name))
            };
        }
    }
}
