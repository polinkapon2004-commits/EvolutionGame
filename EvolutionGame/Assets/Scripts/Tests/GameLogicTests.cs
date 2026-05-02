using NUnit.Framework;
using EvolutionGame.Cards;
using EvolutionGame.Core;
using EvolutionGame.Properties;

namespace EvolutionGame.Tests
{
    /// <summary>
    /// Юнит-тесты для ключевых классов модели игры.
    /// Запускаются через Unity Test Framework (Window → General → Test Runner).
    /// </summary>
    public class GameLogicTests
    {
        [Test]
        public void DeckFactory_CreatesEightyFourCards()
        {
            var deck = DeckFactory.CreateStandardDeck();
            Assert.AreEqual(84, deck.Count, "Стандартная колода должна содержать 84 карты.");
        }

        [Test]
        public void Deck_DrawReturnsCardAndDecreasesSize()
        {
            var cards = DeckFactory.CreateStandardDeck();
            var deck = new Deck(cards, seed: 42);
            int before = deck.Count;
            var card = deck.Draw();
            Assert.NotNull(card);
            Assert.AreEqual(before - 1, deck.Count);
        }

        [Test]
        public void Deck_DrawFromEmptyDeckReturnsNull()
        {
            var deck = new Deck(System.Array.Empty<Card>());
            Assert.IsNull(deck.Draw());
        }

        [Test]
        public void Creature_DefaultRequiresOneFood()
        {
            var creature = new Creature(1, 1);
            Assert.AreEqual(1, creature.RequiredFood);
        }

        [Test]
        public void Creature_PredatorPropertyIncreasesFoodRequirement()
        {
            var creature = new Creature(1, 1);
            creature.AddProperty(new PredatorProperty());
            Assert.AreEqual(2, creature.RequiredFood,
                "Хищник должен увеличивать потребность в еде на +1.");
        }

        [Test]
        public void Creature_FedReturnsTrueWhenEnoughFood()
        {
            var creature = new Creature(1, 1);
            Assert.IsFalse(creature.IsFed);
            creature.Feed(1);
            Assert.IsTrue(creature.IsFed);
        }

        [Test]
        public void Creature_DuplicatePropertyRejectedExceptFatStorage()
        {
            var creature = new Creature(1, 1);
            creature.AddProperty(new BigProperty());
            Assert.IsFalse(creature.CanAddProperty(new BigProperty()));

            creature.AddProperty(new FatTissueProperty());
            Assert.IsTrue(creature.CanAddProperty(new FatTissueProperty()),
                "Жировой запас должен разрешать дублирование.");
        }

        [Test]
        public void Creature_FatStorageStoresExcessFood()
        {
            var creature = new Creature(1, 1);
            creature.AddProperty(new FatTissueProperty()); // capacity = 1
            creature.Feed(2); // 1 в еду, 1 в жир
            Assert.IsTrue(creature.IsFed);
            Assert.AreEqual(1, creature.FatStorage);
        }

        [Test]
        public void Creature_VictoryPointsCalculatedCorrectly()
        {
            // 2 (за существо) + 1+1 (за Большой) + 1+1 (за Хищник, +1 бонус) = 6
            var creature = new Creature(1, 1);
            creature.AddProperty(new BigProperty());
            creature.AddProperty(new PredatorProperty());
            int expected = 2 + 1 + (1 + 1); // см. формулу в Creature.GetVictoryPoints
            Assert.AreEqual(expected, creature.GetVictoryPoints());
        }

        [Test]
        public void Player_RemoveFromHandReturnsTrueOnlyIfPresent()
        {
            var player = new Player(1, "Test", false, "#FFFFFF");
            var card = new Card(1, new BigProperty());
            player.TakeCard(card);
            Assert.IsTrue(player.RemoveFromHand(card));
            Assert.IsFalse(player.RemoveFromHand(card));
        }

        [Test]
        public void GameState_ThrowsOnInvalidPlayerCount()
        {
            var deck = new Deck(System.Array.Empty<Card>());
            Assert.Throws<System.ArgumentException>(() =>
                new GameState(new System.Collections.Generic.List<Player>
                {
                    new Player(1, "Solo", false, "#FFF")
                }, deck));
        }

        [Test]
        public void PropertyImplementations_FactoryReturnsCorrectType()
        {
            var prop = PropertyImplementations.CreateByName("Хищник");
            Assert.IsInstanceOf<PredatorProperty>(prop);
            Assert.AreEqual(1, prop.ExtraFoodRequired);
        }

        [Test]
        public void PropertyImplementations_UnknownNameThrows()
        {
            Assert.Throws<System.ArgumentException>(() =>
                PropertyImplementations.CreateByName("НесуществующееСвойство"));
        }
    }
}
