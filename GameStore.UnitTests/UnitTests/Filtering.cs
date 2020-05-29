using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.UnitTests {

    [TestClass]
    public class Filtering {

        [TestMethod]
        public void isFilter_Games () {
            // Организация (arrange)
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup( m => m.Games ).Returns( new List<Game>
            {
                new Game { GameId = 1, Name = "Игра1", Category="Cat1"},
                new Game { GameId = 2, Name = "Игра2", Category="Cat2"},
                new Game { GameId = 3, Name = "Игра3", Category="Cat1"},
                new Game { GameId = 4, Name = "Игра4", Category="Cat2"},
                new Game { GameId = 5, Name = "Игра5", Category="Cat3"}
            } );
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            // Action
            List<Game> result = ((GamesListViewModel)controller.List("Cat2", 1).Model)
                .Games.ToList();

            // Assert
            Assert.AreEqual( result.Count(), 2 );
            Assert.IsTrue( result[0].Name == "Игра2" && result[0].Category == "Cat2" );
            Assert.IsTrue( result[1].Name == "Игра4" && result[1].Category == "Cat2" );
        }
    }
}
