using Moq;
using StoreAPI.Controllers;
using ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Models.Game;
using DataAccessLayer.Entities;

namespace StoreAPI.Tests.IntegrationTests
{
    [TestClass]
    public class GamesControllerTests
    {
        private Mock<IGameService>? _mockGameService;
        private Mock<ILogger<GamesController>>? _mockLogger;
        private GamesController? _controller;

        [TestInitialize]
        public void SetUp()
        {
            _mockGameService = new Mock<IGameService>();
            _mockLogger = new Mock<ILogger<GamesController>>();
            _controller = new GamesController(_mockGameService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task CreateGame_GivenValidGameCreateDto_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var gameCreateDto = new GameCreateDto
            {
                Game = new GameResponseDto
                {
                    Name = "Game 1",
                    Key = "Key1",
                    Description = "This is a test game",
                    UnitInStock = 50,
                    Price = 59,
                    Discontinued = 0
                },
                Genres = new List<string> { "Genre1", "Genre2" },
                Platforms = new List<string> { "Platform1", "Platform2" },
                Publisher = "Test Publisher"
            };
            GameResponseDto? createdGameResponseDto = gameCreateDto.Game;

            _mockGameService!
                .Setup(x => x.CreateGameAsync(It.Is<GameCreateDto>(g => g.Game!.Name == "Game 1" && g.Game.Key == "Key1")))
                .ReturnsAsync(createdGameResponseDto);

            // Act
            var result = await _controller!.CreateGame(gameCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            var returnValue = createdAtActionResult?.Value as GameResponseDto;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(gameCreateDto.Game, returnValue);

            _mockGameService.Verify(x => x.CreateGameAsync(It.Is<GameCreateDto>(g => g.Game!.Name == "Game 1" && g.Game.Key == "Key1")), Times.Once);
        }

        [TestMethod]
        public async Task DeleteGameById_ReturnsNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            var nonExistingGameKey = "non-existing-game-key";

            _mockGameService!
                .Setup(service => service.DeleteGameAsync(nonExistingGameKey))
                .ReturnsAsync(false);

            // Act
            var result = await _controller!.DeleteGameById(nonExistingGameKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteGameById_ReturnsNoContent_WhenGameExistsAndIsDeletedSuccessfully()
        {
            // Arrange
            var existingGameKey = "existing-game-key";

            _mockGameService!
                .Setup(service => service.DeleteGameAsync(existingGameKey))
                .ReturnsAsync(true);

            // Act
            var result = await _controller!.DeleteGameById(existingGameKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetGameDetails_ValidKey_ReturnsGameDetails()
        {
            // Arrange
            var gameKey = "ValidKey";
            var expectedGameDetails = new GameModel
            {
                Name = "Test Game",
                Description = "This is a test game",
            };

            _mockGameService!
                .Setup(s => s.GetModelModelDescriptionAsync(gameKey))
                .ReturnsAsync(expectedGameDetails);

            // Act
            var result = await _controller!.GetGameDetails(gameKey);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var gameDetails = okResult.Value as GameModel;
            Assert.IsNotNull(gameDetails);
            Assert.AreEqual(expectedGameDetails.Name, gameDetails.Name);
            Assert.AreEqual(expectedGameDetails.Description, gameDetails.Description);

            _mockGameService.Verify(s => s.GetModelModelDescriptionAsync(gameKey), Times.Once);
        }

        [TestMethod]
        public async Task GetGameDetails_InvalidKey_ReturnsNotFound()
        {
            // Arrange
            var gameKey = "InvalidKey";

            _mockGameService!
                .Setup(s => s.GetModelModelDescriptionAsync(gameKey))
                .ReturnsAsync((GameModel?)null);

            // Act
            var result = await _controller!.GetGameDetails(gameKey);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            _mockGameService.Verify(s => s.GetModelModelDescriptionAsync(gameKey), Times.Once);
        }

        [TestMethod]
        public async Task GetGameById_ValidId_ReturnsGameDetails()
        {
            // Arrange
            var gameId = "ValidId";
            var expectedGameDetails = new GameModel
            {
                Name = "Test Game",
                Description = "This is a test game",
            };

            _mockGameService!
                .Setup(s => s.GetModelModelDescriptionAsync(gameId))
                .ReturnsAsync(expectedGameDetails);

            // Act
            var result = await _controller!.GetGameById(gameId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var gameDetails = okResult.Value as GameModel;
            Assert.IsNotNull(gameDetails);
            Assert.AreEqual(expectedGameDetails.Name, gameDetails.Name);
            Assert.AreEqual(expectedGameDetails.Description, gameDetails.Description);

            _mockGameService.Verify(s => s.GetModelModelDescriptionAsync(gameId), Times.Once);
        }

        [TestMethod]
        public async Task GetGameById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var gameId = "InvalidId";

            _mockGameService!
                .Setup(s => s.GetModelModelDescriptionAsync(gameId))
                .ReturnsAsync((GameModel?)null);

            // Act
            var result = await _controller!.GetGameById(gameId);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            _mockGameService.Verify(s => s.GetModelModelDescriptionAsync(gameId), Times.Once);
        }

        [TestMethod]
        public async Task UpdateGame_ValidDto_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var gameUpdateDto = new GameUpdateDto
            {
                Game = new GameResponseForUpdateDto
                {
                    Name = "Test Game",
                    Description = "This is a test game",
                },
            };

            _mockGameService!
                .Setup(s => s.UpdateGameAsync(gameUpdateDto))
                .ReturnsAsync(gameUpdateDto);

            // Act
            var result = await _controller!.UpdateGame(gameUpdateDto);

            // Assert
            var createdAtResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult);
            Assert.AreEqual(201, createdAtResult.StatusCode);

            var returnDto = createdAtResult.Value as GameUpdateDto;
            Assert.IsNotNull(returnDto);
            Assert.AreEqual(gameUpdateDto.Game.Name, returnDto.Game!.Name);
            Assert.AreEqual(gameUpdateDto.Game.Description, returnDto.Game.Description);

            _mockGameService.Verify(s => s.UpdateGameAsync(gameUpdateDto), Times.Once);
        }

        [TestMethod]
        public async Task UpdateGame_UpdateThrowsException_ReturnsNotFound()
        {
            // Arrange
            var gameUpdateDto = new GameUpdateDto
            {
                Game = new GameResponseForUpdateDto
                {
                    Name = "Test Game",
                    Description = "This is a test game",
                },
            };

            _mockGameService!
                .Setup(s => s.UpdateGameAsync(gameUpdateDto))
                .ThrowsAsync(new ArgumentException());

            // Act
            var result = await _controller!.UpdateGame(gameUpdateDto);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            _mockGameService.Verify(s => s.UpdateGameAsync(gameUpdateDto), Times.Once);
        }

        [TestMethod]
        public async Task GetAllGames_ReturnsOkObjectResult_WithListOfGameModels()
        {
            // Arrange
            var gameEntities = new List<Game>
            {
                new Game {  Name = "Test Game1", Key = "Alias1", Description = "Test Game Description1", UnitInStock = 5,Price = 50, Discontinued = 0, },
                new Game { Name = "Test Game2", Key = "Alias2", Description = "Test Game Description2", UnitInStock = 5,Price = 50, Discontinued = 0, },                
            };

            var gameModels = gameEntities
                 .Select(game => new GameModel
                 {
                 Name = game.Name,
                 Key = game.Key,
                 Description = game.Description,
                 UnitInStock = game.UnitInStock,
                 Price = game.Price,
                 Discontinued = game.Discontinued,
                 Genres = game.GameGenres
                    .Where(gg => gg.Genre!.Name != null)
                    .Select(gg => gg.Genre!.Name!)
                    .ToList(),
                 Platforms = game.GamePlatforms
                    .Where(gp => gp.Platform!.Type != null)
                    .Select(gp => gp.Platform!.Type!)
                    .ToList()
                 })
                 .ToList();

            _mockGameService!
                .Setup(s => s.GetAllModelsAsync())
                .ReturnsAsync(gameModels);

            // Act
            var result = await _controller!.GetAllGames() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var returnedGameModels = result.Value as IEnumerable<GameModel>;
            Assert.IsNotNull(returnedGameModels);
            Assert.AreEqual(gameModels.Count, returnedGameModels.Count());
        }

        [TestMethod]
        public async Task GetPlatformsNameByGameId_ReturnsOkObjectResult_WithListOfPlatforms()
        {
            // Arrange
            var gameId = "game1";
            var expectedPlatforms = new List<GetPlatformNameByGameDto>
            {
                new GetPlatformNameByGameDto { Id = "game1", Type = "Type1" },
                new GetPlatformNameByGameDto { Id = "game1", Type = "Type2" },
            };

            _mockGameService!
                .Setup(s => s.GetPlatformsNameByGame(gameId))
                .ReturnsAsync(expectedPlatforms);

            // Act
            var actionResult = await _controller!.GetPlatformsNameByGameId(gameId);

            // Assert
            Assert.IsNotNull(actionResult.Result);
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));

            var okResult = actionResult.Result as OkObjectResult;
            var returnedPlatforms = okResult!.Value as IEnumerable<GetPlatformNameByGameDto>;

            Assert.IsNotNull(returnedPlatforms);
            Assert.AreEqual(expectedPlatforms.Count, returnedPlatforms.Count());
        }
    }
}