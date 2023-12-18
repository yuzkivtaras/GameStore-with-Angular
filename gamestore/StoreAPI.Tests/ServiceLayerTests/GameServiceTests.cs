using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ServiceLayer.Models.Game;
using ServiceLayer.Services;

namespace StoreAPI.Tests.ServiceLayerTests
{
    [TestClass]
    public class GameServiceTests
    {
        private Mock<IUnitOfWork>? _mockUnitOfWork;
        private Mock<IMemoryCache>? _mockMemoryCache;
        private GameService? _gameService;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMemoryCache = new Mock<IMemoryCache>();

            _gameService = new GameService(_mockUnitOfWork.Object, UnitTestHelper.CreateMapperProfile(), _mockMemoryCache.Object);
        }

        [TestMethod]
        public async Task GameService_CreateGameAsync_ShouldCreateGameAndReturnResponseDto()
        {
            // Arrange
            var gameCreateDto = new GameCreateDto
            {
                Game = new GameResponseDto
                {
                    Name = "Test Game",
                    Key = "TestKey",
                    Description = "Test Description",
                    UnitInStock = 5,
                    Price = 10,
                    Discontinued = 10
                },
                Publisher = "Publisher1",
                Platforms = new List<string> { "1", "2" },
                Genres = new List<string> { "1", "2" }
            };

            // Arrange mocks
            var publisher = new Publisher { Id = gameCreateDto.Publisher };
            _mockUnitOfWork!.Setup(uow => uow.PublisherRepository.GetByIdAsync(gameCreateDto.Publisher)).ReturnsAsync(publisher);

            var platforms = new List<Platform>
            {
                new Platform { Id = "1" },
                new Platform { Id = "2" }
            };
            _mockUnitOfWork!.Setup(uow => uow.PlatformRepository.GetAll()).Returns(platforms.AsQueryable());

            var genres = new List<Genre>
            {
                new Genre { Id = "1" },
                new Genre { Id = "1" }
            };
            _mockUnitOfWork!.Setup(uow => uow.GenreRepository.GetAll()).Returns(genres.AsQueryable());

            var createdGame = new Game();
            _mockUnitOfWork!.Setup(uow => uow.GameRepository.CreateAsync(It.IsAny<Game>())).ReturnsAsync(createdGame);

            // Act
            var gameResponseDto = await _gameService!.CreateGameAsync(gameCreateDto);

            // Assert
            Assert.IsNotNull(gameResponseDto);

            // Verify repository and UoW interactions
            _mockUnitOfWork!.Verify(uow => uow.PublisherRepository.GetByIdAsync(gameCreateDto.Publisher), Times.Once);
            _mockUnitOfWork!.Verify(uow => uow.PlatformRepository.GetAll(), Times.AtLeastOnce);
            _mockUnitOfWork!.Verify(uow => uow.GenreRepository.GetAll(), Times.AtLeastOnce);
            _mockUnitOfWork!.Verify(uow => uow.GameRepository.CreateAsync(It.IsAny<Game>()), Times.Once);
            _mockUnitOfWork!.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [TestMethod]
        public async Task DeleteGameAsync_ReturnsTrue_WhenGameIsDeletedSuccessfully()
        {
            // Arrange
            string testKey = "game1";
            var game = new Game { Key = testKey };

            _mockUnitOfWork!
                .Setup(uow => uow.GameRepository.DeleteGameByIdAsync(testKey))
                .ReturnsAsync(game);

            // Act
            var result = await _gameService!.DeleteGameAsync(testKey);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteGameAsync_ReturnsFalse_WhenNoGameIsDeleted()
        {
            // Arrange
            string testKey = "non-existing-game-key";

            _mockUnitOfWork!
                .Setup(uow => uow.GameRepository.DeleteGameByIdAsync(testKey))
                .ReturnsAsync((Game?)null);

            // Act
            var result = await _gameService!.DeleteGameAsync(testKey);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GameService_DownloadGameAsync_ShouldReturnResponseForDownloadDto()
        {
            // Arrange
            var gameDownloadDto = new GameDownloadDto
            {
                Game = new GameResponseDto
                {
                    Key = "TestGameKey"
                }
            };

            var gameDetails = new Game
            {
                Name = "Test Game",
                Key = "TestGameKey",
                Description = "Test Description",
                UnitInStock = 5,
                Price = 10,
                Discontinued = 1
            };

            // Arrange mocks
            _mockUnitOfWork!.Setup(uow => uow.GameRepository.GetGameDetailsByKeyAsync(gameDownloadDto.Game.Key)).ReturnsAsync(gameDetails);

            // Act
            var gameResponseForDownloadDto = await _gameService!.DownloadGameAsync(gameDownloadDto);

            // Assert
            Assert.IsNotNull(gameResponseForDownloadDto);
            Assert.AreEqual(gameDetails.Name, gameResponseForDownloadDto.Name);
            Assert.AreEqual(gameDetails.Key, gameResponseForDownloadDto.Key);
            Assert.AreEqual(gameDetails.Description, gameResponseForDownloadDto.Description);
            Assert.AreEqual(gameDetails.UnitInStock, gameResponseForDownloadDto.UnitInStock);
            Assert.AreEqual(gameDetails.Price, gameResponseForDownloadDto.Price);
            Assert.AreEqual(gameDetails.Discontinued, gameResponseForDownloadDto.Discontinued);
            Assert.IsNotNull(gameResponseForDownloadDto.FileName);
            Assert.IsNotNull(gameResponseForDownloadDto.FileContent);

            // Verify repository interactions
            _mockUnitOfWork!.Verify(uow => uow.GameRepository.GetGameDetailsByKeyAsync(gameDownloadDto.Game.Key), Times.Once);
        }

        [TestMethod]
        public async Task GameService_GetAllModelsAsync_ShouldReturnGameModels()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game { Key = "GameKey1", Name = "Game1", Description = "Game description 1", UnitInStock = 10, Price = 29.99M },
                new Game { Key = "GameKey2", Name = "Game2", Description = "Game description 2", UnitInStock = 5, Price = 19.99M }
            };

            // Arrange mocks
            _mockUnitOfWork!.Setup(uow => uow.GameRepository.GetAllAsync()).ReturnsAsync(games);

            // Act
            var gameModels = await _gameService!.GetAllModelsAsync();

            // Assert
            Assert.IsNotNull(gameModels);
            Assert.AreEqual(games.Count, gameModels.Count());

            var gameModelsList = gameModels.ToList();

            for (int i = 0; i < games.Count; i++)
            {
                Assert.AreEqual(games[i].Key, gameModelsList[i].Key);
                Assert.AreEqual(games[i].Name, gameModelsList[i].Name);
            }

            // Verify repository interactions
            _mockUnitOfWork!.Verify(uow => uow.GameRepository.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GameService_GetModelModelDescriptionAsync_ShouldReturnGameModel()
        {
            // Arrange
            var gameKey = "GameKey1";
            var game = new Game { Key = gameKey, Name = "Game1", Description = "Game description 1", UnitInStock = 10, Price = 29.99M };

            // Arrange mocks
            _mockUnitOfWork!.Setup(uow => uow.GameRepository.GetGameDetailsByKeyAsync(gameKey)).ReturnsAsync(game);

            // Act
            var gameModel = await _gameService!.GetModelModelDescriptionAsync(gameKey);

            // Assert
            Assert.IsNotNull(gameModel);
            Assert.AreEqual(game.Key, gameModel?.Key);
            Assert.AreEqual(game.Name, gameModel?.Name);

            // Verify repository interactions
            _mockUnitOfWork!.Verify(uow => uow.GameRepository.GetGameDetailsByKeyAsync(gameKey), Times.Once);
        }

        [TestMethod]
        public async Task GameService_GetModelModelDescriptionAsync_ShouldReturnNull_WhenGameNotFound()
        {
            // Arrange
            var gameKey = "NonExistingGameKey";

            // Arrange mocks
            _mockUnitOfWork!.Setup(uow => uow.GameRepository.GetGameDetailsByKeyAsync(gameKey)).ReturnsAsync((Game?)null);

            // Act
            var gameModel = await _gameService!.GetModelModelDescriptionAsync(gameKey);

            // Assert
            Assert.IsNull(gameModel);

            // Verify repository interactions
            _mockUnitOfWork!.Verify(uow => uow.GameRepository.GetGameDetailsByKeyAsync(gameKey), Times.Once);
        }

        [TestMethod]
        public async Task GameService_GetModelModelDescriptionByIdAsync_ShouldReturnGameModel()
        {
            var gameId = "1";
            var gameEntity = new Game
            {
                Id = gameId,
                Name = "Test Game",
                Key = "test-game",
                Description = "Test game description",
                UnitInStock = 10,
                Price = 29,
                Discontinued = 10,
            };

            // Arrange mocks
            _mockUnitOfWork!.Setup(uow => uow.GameRepository.GetGameDetailsByIdAsync(gameId)).ReturnsAsync(gameEntity);

            // Act
            var gameModel = await _gameService!.GetModelModelDescriptionByIdAsync(gameId);

            // Assert
            Assert.IsNotNull(gameModel);
            Assert.AreEqual(gameEntity.Id, gameModel.Id);
            Assert.AreEqual(gameEntity.Name, gameModel.Name);
            Assert.AreEqual(gameEntity.Key, gameModel.Key);
            Assert.AreEqual(gameEntity.Description, gameModel.Description);
            Assert.AreEqual(gameEntity.UnitInStock, gameModel.UnitInStock);
            Assert.AreEqual(gameEntity.Price, gameModel.Price);
            Assert.AreEqual(gameEntity.Discontinued, gameModel.Discontinued);

            // Verify repository interactions
            _mockUnitOfWork!.Verify(uow => uow.GameRepository.GetGameDetailsByIdAsync(gameId), Times.Once);
        }

        [TestMethod]
        public async Task GameService_GetModelModelDescriptionByIdAsync_ShouldReturnNull_WhenGameNotFound()
        {
            // Arrange
            var gameId = "NonExistingGameId";

            // Arrange mocks
            _mockUnitOfWork!.Setup(uow => uow.GameRepository.GetGameDetailsByIdAsync(gameId)).ReturnsAsync((Game?)null);

            // Act
            var gameModel = await _gameService!.GetModelModelDescriptionByIdAsync(gameId);

            // Assert
            Assert.IsNull(gameModel);

            // Verify repository interactions
            _mockUnitOfWork!.Verify(uow => uow.GameRepository.GetGameDetailsByIdAsync(gameId), Times.Once);
        }

        [TestMethod]
        public async Task GameService_GetPlatformsNameByGame_ShouldReturnPlatformNameByGameDtos()
        {
            // Arrange
            var gameId = "1";
            var gamePlatforms = new List<Platform>
            {
                new Platform { Id = "1", Type = "Platform1" },
                new Platform { Id = "2", Type = "Platform2" }
            };

            // Arrange mocks
            _mockUnitOfWork!.Setup(uow => uow.GameRepository.GetPlatformsByGame(gameId)).ReturnsAsync(gamePlatforms);

            // Act
            var platformsNameByGameDtos = await _gameService!.GetPlatformsNameByGame(gameId);

            // Assert
            Assert.IsNotNull(platformsNameByGameDtos);
            Assert.AreEqual(gamePlatforms.Count, platformsNameByGameDtos.Count());

            var platformsNameByGameDtosList = platformsNameByGameDtos.ToList();

            for (int i = 0; i < gamePlatforms.Count; i++)
            {
                Assert.AreEqual(gamePlatforms[i].Id, platformsNameByGameDtosList[i].Id);
                Assert.AreEqual(gamePlatforms[i].Type, platformsNameByGameDtosList[i].Type);
            }

            // Verify repository interactions
            _mockUnitOfWork!.Verify(uow => uow.GameRepository.GetPlatformsByGame(gameId), Times.Once);
        }

        [TestMethod]
        public async Task GameService_UpdateGameAsync_ShouldUpdateGameAndReturnResponseForUpdateDto()
        {
            string gameId = "1";
            string originalGameName = "Original Game Name";
            string updatedGameName = "Updated Game Name";
            int updatedUnitInStock = 5;
            decimal updatedPrice = 40;
            int updatedDiscontinued = 10;

            var gameUpdateDto = new GameUpdateDto
            {
                Game = new GameResponseForUpdateDto
                {
                    Id = gameId,
                    Name = updatedGameName,
                    UnitInStock = updatedUnitInStock,
                    Price = updatedPrice,
                    Discontinued = updatedDiscontinued,
                }
            };

            var game = new Game
            {
                Id = gameId,
                Name = originalGameName,
            };

            _mockUnitOfWork!
                .Setup(uow => uow.GameRepository.GetGameDetailsByIdAsync(gameId))
                .ReturnsAsync(game);

            _mockUnitOfWork
                .Setup(uow => uow.GameRepository.UpdateAsync(game))
                .ReturnsAsync(game);

            // Act
            _ = await _gameService!.UpdateGameAsync(gameUpdateDto);

            // Assert
            Assert.AreEqual(updatedGameName, game.Name, "Updated game name does not match the expected game name");
            Assert.AreEqual(updatedUnitInStock, game.UnitInStock, "Updated game unit in stock does not match the expected value");
            Assert.AreEqual(updatedPrice, game.Price, "Updated game price does not match the expected value");
            Assert.AreEqual(updatedDiscontinued, game.Discontinued, "Updated game discontinued status does not match the expected value");

            _mockUnitOfWork.Verify(uow => uow.GameRepository.GetGameDetailsByIdAsync(gameId), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.GameRepository.UpdateAsync(game), Times.Once);
        }
    }
}
