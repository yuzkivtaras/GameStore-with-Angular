using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StoreAPI.Tests.DataAccessLayerTests
{
    [TestClass]
    public class GameRepositoryTests
    {
        private GameStoreDbContext? _context;
        private GameRepository? _gameRepository;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _context = new GameStoreDbContext(UnitTestHelper.GetUnitTestDbOptions());
            _gameRepository = new GameRepository(_context);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldCreateNewGame()
        {
            // Arrange
            var newGame = new Game { Id = "3", Name = "Name3", Key = "name3", Description = "Desc3", UnitInStock = 3, Discontinued = 30, PublisherId = "1" };

            // Act
            var createdGame = await _gameRepository!.CreateAsync(newGame);

            // Assert
            Assert.IsNotNull(createdGame);
            Assert.AreEqual(newGame.Id, createdGame.Id);
            Assert.AreEqual(newGame.Name, createdGame.Name);
            Assert.AreEqual(newGame.Key, createdGame.Key);
            Assert.AreEqual(newGame.Description, createdGame.Description);
            Assert.AreEqual(newGame.UnitInStock, createdGame.UnitInStock);
            Assert.AreEqual(newGame.Discontinued, createdGame.Discontinued);
            Assert.AreEqual(newGame.PublisherId, createdGame.PublisherId);

            var dbGame = _context!.Games.FirstOrDefault(g => g.Id == newGame.Id);
            Assert.IsNotNull(dbGame);
            Assert.AreEqual(newGame.Id, dbGame.Id);
            Assert.AreEqual(newGame.Name, dbGame.Name);
            Assert.AreEqual(newGame.Key, dbGame.Key);
            Assert.AreEqual(newGame.Description, dbGame.Description);
            Assert.AreEqual(newGame.UnitInStock, dbGame.UnitInStock);
            Assert.AreEqual(newGame.Discontinued, dbGame.Discontinued);
            Assert.AreEqual(newGame.PublisherId, dbGame.PublisherId);
        }

        [TestMethod]
        public async Task DeleteGameById_ReturnsDeletedGame_WhenGameExists()
        {
            // Arrange
            string testKey = "name1"; 

            // Act
            var deletedGame = await _gameRepository!.DeleteGameByIdAsync(testKey);

            // Assert
            Assert.IsNotNull(deletedGame);
            Assert.AreEqual(testKey, deletedGame.Key);

            var gameInDb = await _context!.Games.FirstOrDefaultAsync(g => g.Key == deletedGame.Key);
            Assert.IsNull(gameInDb);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllGames()
        {
            // Arrange
            var expectedGames = _context!.Games.ToList();

            // Act
            var actualGames = await _gameRepository!.GetAllAsync();

            // Assert
            Assert.AreEqual(expectedGames.Count, actualGames.Count());
            var actualGamesList = actualGames.ToList();

            for (int i = 0; i < expectedGames.Count; i++)
            {
                Assert.AreEqual(expectedGames[i].Id, actualGamesList[i].Id);
                Assert.AreEqual(expectedGames[i].Name, actualGamesList[i].Name);
            }
        }

        [TestMethod]
        public async Task GetGameDetailsByKeyAsync_ShouldReturnGameDetailsByKey()
        {
            // Arrange
            var gameKey = "name1";
            var expectedGame = _context!.Games.FirstOrDefault(p => p.Key == gameKey);

            // Act
            var actualGame = await _gameRepository!.GetGameDetailsByKeyAsync(gameKey);

            // Assert
            Assert.IsNotNull(actualGame);
            Assert.AreEqual(expectedGame!.Id, actualGame?.Id);
            Assert.AreEqual(expectedGame.Name, actualGame?.Name);
            Assert.AreEqual(expectedGame.Key, actualGame?.Key);
        }

        [TestMethod]
        public async Task GetGameDetailsByKeyAsync_ShouldReturnNull_WhenKeyNotFound()
        {
            // Arrange
            var nonExistingKey = "non_existing_key";

            // Act
            var actualGame = await _gameRepository!.GetGameDetailsByKeyAsync(nonExistingKey);

            // Assert
            Assert.IsNull(actualGame);
        }

        [TestMethod]
        public async Task GetGameDetailsByIdAsync_ShouldReturnGameDetailsById()
        {
            // Arrange
            var gameId = "1";
            var expectedGame = _context!.Games.FirstOrDefault(p => p.Id == gameId);

            // Act
            var actualGame = await _gameRepository!.GetGameDetailsByIdAsync(gameId);

            // Assert
            Assert.IsNotNull(actualGame);
            Assert.AreEqual(expectedGame!.Id, actualGame?.Id);
            Assert.AreEqual(expectedGame.Name, actualGame?.Name);
            Assert.AreEqual(expectedGame.Key, actualGame?.Key);
        }

        [TestMethod]
        public async Task GetGameDetailsByIdAsync_ShouldReturnNull_WhenIdNotFound()
        {
            // Arrange
            var nonExistingId = "999";

            // Act
            var actualGame = await _gameRepository!.GetGameDetailsByIdAsync(nonExistingId);

            // Assert
            Assert.IsNull(actualGame);
        }

        [TestMethod]
        public async Task GetPlatformsByGame_ShouldReturnPlatformsByGameId()
        {
            // Arrange
            _context!.GamePlatforms.Add(new GamePlatform { GamesKey = "1", PlatformsId = "1" });
            _context.GamePlatforms.Add(new GamePlatform { GamesKey = "1", PlatformsId = "2" });
            _context.GamePlatforms.Add(new GamePlatform { GamesKey = "2", PlatformsId = "1" });
            await _context.SaveChangesAsync();

            var gameId = "1";
            var expectedPlatforms = _context.GamePlatforms
                .Include(gp => gp.Platform)
                .Where(gp => gp.GamesKey == gameId)
                .Select(gp => gp.Platform)
                .ToList();

            // Act
            var actualPlatforms = await _gameRepository!.GetPlatformsByGame(gameId);

            // Assert
            Assert.AreEqual(expectedPlatforms.Count, actualPlatforms.Count());
            var actualPlatformsList = actualPlatforms.ToList();

            for (int i = 0; i < expectedPlatforms.Count; i++)
            {
                Assert.AreEqual(expectedPlatforms[i]!.Id, actualPlatformsList[i]!.Id);
                Assert.AreEqual(expectedPlatforms[i]!.Type, actualPlatformsList[i]!.Type);
            }
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateGame()
        {
            // Arrange
            var existingGame = new Game { Id = "3", Name = "TestGame", Key = "testKey", Description = "Test description", UnitInStock = 1, Discontinued = 10, PublisherId = "1" };
            _context!.Games.Add(existingGame);
            await _context.SaveChangesAsync();

            var updatedGame = new Game { Id = existingGame.Id, Name = "Updated TestGame", Key = "testKey", Description = "Updated description", UnitInStock = 2, Discontinued = 20, PublisherId = "2" };

            // Act
            var actualUpdatedGame = await _gameRepository!.UpdateAsync(updatedGame);

            // Assert
            Assert.IsNotNull(actualUpdatedGame);
            Assert.AreEqual(updatedGame.Id, actualUpdatedGame?.Id);
            Assert.AreEqual(updatedGame.Name, actualUpdatedGame?.Name);
            Assert.AreEqual(updatedGame.Description, actualUpdatedGame?.Description);
            Assert.AreEqual(updatedGame.UnitInStock, actualUpdatedGame?.UnitInStock);
            Assert.AreEqual(updatedGame.Discontinued, actualUpdatedGame?.Discontinued);
            Assert.AreEqual(updatedGame.PublisherId, actualUpdatedGame?.PublisherId);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowArgumentException_WhenGameNotFound()
        {
            // Arrange
            var nonExistingGame = new Game { Id = "3", Name = "NonExistingGame", Key = "nonExistingKey", Description = "NonExistingGame description", UnitInStock = 1, Discontinued = 10, PublisherId = "1" };

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _gameRepository!.UpdateAsync(nonExistingGame));
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllPlatforms()
        {
            // Arrange
            var expectedPlatforms = _context!.Platforms.ToList();

            // Act
            var actualPlatforms = _gameRepository!.GetAll();

            // Assert
            Assert.AreEqual(expectedPlatforms.Count, actualPlatforms.Count());
            var actualPlatformsList = actualPlatforms.ToList();

            for (int i = 0; i < expectedPlatforms.Count; i++)
            {
                Assert.AreEqual(expectedPlatforms[i].Id, actualPlatformsList[i].Id);
                Assert.AreEqual(expectedPlatforms[i].Type, actualPlatformsList[i].Type);
            }
        }
    }
}
