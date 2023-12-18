using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceLayer.Interfaces;
using ServiceLayer.Models.Genre;
using StoreAPI.Controllers;

namespace StoreAPI.Tests.IntegrationTests
{
    [TestClass]
    public class GenresControllerTests
    {
        private Mock<IGenreService>? _mockGenreService;
        private Mock<ILogger<GenresController>>? _mockLogger;
        private GenresController? _controller;

        [TestInitialize]
        public void SetUp()
        {
            _mockGenreService = new Mock<IGenreService>();
            _mockLogger = new Mock<ILogger<GenresController>>();
            _controller = new GenresController(_mockGenreService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetAllGenres_ReturnsOkObjectResult_WithListOfGenreModels()
        {
            // Arrange
            var expectedGenreModels = new List<GenreModel>
            {
                new GenreModel { Id = Guid.NewGuid().ToString(), Name = "Genre1", ParentGenreId = "1" },
                new GenreModel { Id = Guid.NewGuid().ToString(), Name = "Genre2", ParentGenreId = "2" },
            };

            _mockGenreService!
                .Setup(s => s.GetAllModelsAsync())
                .ReturnsAsync(expectedGenreModels);

            // Act
            var result = await _controller!.GetAllGenres() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var returnedGenreModels = result.Value as List<GenreModel>;
            Assert.IsNotNull(returnedGenreModels);
            Assert.AreEqual(expectedGenreModels.Count, returnedGenreModels.Count);
        }

        [TestMethod]
        public async Task CreateGenre_ReturnsCreatedAtActionResult_WithGenreModel_WhenModelStateIsValid()
        {
            // Arrange
            var newGenreDto = new GenreCreateDto
            {
                Genre = new GenreResponseDto
                {
                    Name = "Test Genre",
                    ParentGenreId = "Test Parent"
                }
            };

            var createdGenreModel = new GenreResponseDto
            {
                Name = "Test Genre",
                ParentGenreId = "Test Parent"
            };

            _mockGenreService!
                .Setup(s => s.CreateGenreAsync(newGenreDto))
                .ReturnsAsync(createdGenreModel);

            // Act
            var result = await _controller!.CreateGenre(newGenreDto) as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            var returnedGenreModel = result.Value as GenreResponseDto;
            Assert.IsNotNull(returnedGenreModel);
            Assert.AreEqual(createdGenreModel.Name, returnedGenreModel.Name);
            Assert.AreEqual(createdGenreModel.ParentGenreId, returnedGenreModel.ParentGenreId);
        }

        [TestMethod]
        public async Task CreateGenre_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var genreDto = new GenreCreateDto
            {
                Genre = new GenreResponseDto
                {
                    Name = "Test Genre",
                    ParentGenreId = "Test Parent"
                }
            };

            _controller!.ModelState.AddModelError("Error", "Some kind of error");

            // Act
            var result = await _controller.CreateGenre(genreDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeleteGenreById_ReturnsOkObjectResult_WithGenreModel_WhenGenreExists()
        {
            // Arrange
            var genreId = "1";
            var expectedGenreModel = new GenreModel { Id = Guid.NewGuid().ToString(), Name = "Genre1", ParentGenreId = "1" };

            _mockGenreService!
                .Setup(s => s.DeleteGenreModelAsync(genreId))
                .ReturnsAsync(expectedGenreModel);

            // Act
            var result = await _controller!.DeleteGenreById(genreId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var returnedGenreModel = result.Value as GenreModel;
            Assert.IsNotNull(returnedGenreModel);
            Assert.AreEqual(expectedGenreModel, returnedGenreModel);
        }

        [TestMethod]
        public async Task GetGenreDetails_ReturnsOkObjectResult_WithGenreDetails_WhenGenreExists()
        {
            // Arrange
            var genreId = "1";

            var expectedGenreDetails = new GenreModel
            {
                Id = "1",
                Name = "Test genre",
                ParentGenreId = "2",
                ParentGenre = new GenreModel
                {
                    Id = "2",
                    Name = "Test parent genre"
                }
            };

            _mockGenreService!
                .Setup(s => s.GetGenreModelDescriptionAsync(genreId))
                .ReturnsAsync(expectedGenreDetails);

            // Act
            var result = await _controller!.GetGenreDetails(genreId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var returnedGenreDetails = result.Value as GenreModel;
            Assert.IsNotNull(returnedGenreDetails);
            Assert.AreEqual(expectedGenreDetails, returnedGenreDetails);
        }

        [TestMethod]
        public async Task GetGenreDetails_ReturnsNotFound_WhenGenreDoesNotExist()
        {
            // Arrange
            var genreId = "1";

            _mockGenreService!
                .Setup(s => s.GetGenreModelDescriptionAsync(genreId))
                .ReturnsAsync((GenreModel?)null);

            // Act
            var result = await _controller!.GetGenreDetails(genreId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetGamesNameByGenreId_ReturnsOkObjectResult_WithGamesList_WhenGamesExist()
        {
            // Arrange
            var genreId = "1";
            var expectedGames = new List<GetGameNameByGenreDto>
            {
                new GetGameNameByGenreDto { Id = Guid.NewGuid().ToString(), Name = "Genre1"},
                new GetGameNameByGenreDto { Id = Guid.NewGuid().ToString(), Name = "Genre2"},
            };

            _mockGenreService!
                .Setup(s => s.GetGamesNameByGenreId(genreId))
                .ReturnsAsync(expectedGames);

            // Act
            var actionResult = await _controller!.GetGamesNameByGenreId(genreId);

            // Assert
            Assert.IsNotNull(actionResult.Result);
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));

            var okResult = actionResult.Result as OkObjectResult;
            var returnedGames = okResult!.Value as IEnumerable<GetGameNameByGenreDto>;

            Assert.IsNotNull(returnedGames);
            Assert.AreEqual(expectedGames.Count, returnedGames.Count());
        }

        [TestMethod]
        public async Task GetGamesNameByGenreParentId_ReturnsOkObjectResult_WithGamesList_WhenGamesExist()
        {
            // Arrange
            var parentId = "1";

            var expectedGames = new List<GetGameNameByGenreParentDto>
            {
                new GetGameNameByGenreParentDto { Id = "1", Name = "name1", ParentId = "23" },
                new GetGameNameByGenreParentDto { Id = "2", Name = "name2", ParentId = "15" },
            };

            _mockGenreService!
                .Setup(s => s.GetGamesNameByParentId(parentId))
                .ReturnsAsync(expectedGames);

            // Act
            var actionResult = await _controller!.GetGamesNameByGenreParentId(parentId);

            // Assert
            Assert.IsNotNull(actionResult.Result);
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));

            var okResult = actionResult.Result as OkObjectResult;
            var returnedGames = okResult!.Value as IEnumerable<GetGameNameByGenreParentDto>;

            Assert.IsNotNull(returnedGames);
            Assert.AreEqual(expectedGames.Count, returnedGames.Count());
        }

        [TestMethod]
        public async Task UpdateGenre_ReturnsCreatedAtAction_WhenGenreDtoIsValidAndExists()
        {
            // Arrange
            var genreDto = new GenreUpdateDto
            {
                Genre = new GenreResponseForUpdateDto
                {
                    Id = "1",
                    Name = "Test Genre",
                    ParentGenreId = "2"
                }
            };

            _mockGenreService!
                .Setup(s => s.UpdateGenreAsync(genreDto))
                .ReturnsAsync(genreDto.Genre);

            // Act
            var result = await _controller!.UpdateGenre(genreDto);

            // Assert
            Assert.IsNotNull(result);
            var resultAsCreatedAtAction = result as CreatedAtActionResult;
            Assert.IsNotNull(resultAsCreatedAtAction);
            Assert.AreEqual(201, resultAsCreatedAtAction.StatusCode);
            Assert.AreEqual("UpdateGenre", resultAsCreatedAtAction.ActionName);
        }

        [TestMethod]
        public async Task UpdateGenre_ReturnsNotFound_WhenGenreDoesNotExist()
        {
            // Arrange
            var genreDto = new GenreUpdateDto
            {
                Genre = new GenreResponseForUpdateDto
                {
                    Id = "1",
                    Name = "Test Genre",
                    ParentGenreId = "2"
                }
            };

            _mockGenreService!
                .Setup(s => s.UpdateGenreAsync(genreDto))
                .ThrowsAsync(new ArgumentException());

            // Act
            var result = await _controller!.UpdateGenre(genreDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
