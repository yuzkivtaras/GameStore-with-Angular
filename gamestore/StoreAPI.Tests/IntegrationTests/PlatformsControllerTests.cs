using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceLayer.Interfaces;
using ServiceLayer.Models;
using ServiceLayer.Models.Platform;
using StoreAPI.Controllers;

namespace StoreAPI.Tests.IntegrationTests
{
    [TestClass]
    public class PlatformsControllerTests
    {
        private Mock<IPlatformService>? _mockPlatformService;
        private Mock<ILogger<PlatformsController>>? _mockLogger;
        private PlatformsController? _controller;

        [TestInitialize]
        public void SetUp()
        {
            _mockPlatformService = new Mock<IPlatformService>();
            _mockLogger = new Mock<ILogger<PlatformsController>>();
            _controller = new PlatformsController(_mockPlatformService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task CreatePlatform_ReturnsCreatedAtActionResult_WithPlatformModel_WhenModelStateIsValid()
        {
            // Arrange
            var newPlatformDto = new PlatformCreateDto
            {
                Platform = new PlatformResponseDto
                {
                    Type = "Test Platform",
                }
            };

            var createdPlatformModel = new PlatformResponseDto
            {
                Type = "Test Platform",
            };

            _mockPlatformService!
                .Setup(s => s.CreatePlatformAsync(newPlatformDto))
                .ReturnsAsync(createdPlatformModel);

            // Act
            var result = await _controller!.CreatePlatform(newPlatformDto) as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            var returnedPlatformModel = result.Value as PlatformResponseDto;
            Assert.IsNotNull(returnedPlatformModel);
            Assert.AreEqual(createdPlatformModel.Type, returnedPlatformModel.Type);
        }

        [TestMethod]
        public async Task CreatePlatform_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var platformDto = new PlatformCreateDto
            {
                Platform = new PlatformResponseDto
                {
                    Type = "Test Platform",
                }
            };

            _controller!.ModelState.AddModelError("Error", "Some kind of error");

            // Act
            var result = await _controller.CreatePlatform(platformDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetPlatformDetails_ReturnsOkObjectResult_WithPlatformDetails_WhenPlatformExists()
        {
            // Arrange
            var platformId = "1";

            var expectedPlatformDetails = new PlatformModel
            {
                Id = "1",
                Type = "Test platform"
            };

            _mockPlatformService!
                .Setup(s => s.GetPlatformModelDescriptionAsync(platformId))
                .ReturnsAsync(expectedPlatformDetails);

            // Act
            var result = await _controller!.GetPlatformDetails(platformId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var returnedPlatformDetails = result.Value as PlatformModel;
            Assert.IsNotNull(returnedPlatformDetails);
            Assert.AreEqual(expectedPlatformDetails, returnedPlatformDetails);
        }

        [TestMethod]
        public async Task GetPlatformDetails_ReturnsNotFound_WhenPlatformDoesNotExist()
        {
            // Arrange
            var platformId = "1";

            _mockPlatformService!
                .Setup(s => s.GetPlatformModelDescriptionAsync(platformId))
                .ReturnsAsync((PlatformModel?)null);

            // Act
            var result = await _controller!.GetPlatformDetails(platformId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetAllPlatforms_ReturnsOkObjectResult_WithListOfPlatformModels_WhenOperationIsSuccessful()
        {
            // Arrange
            var expectedPlatformModels = new List<PlatformModel>
            {
                new PlatformModel
                {
                    Id = "1",
                    Type = "Test platform 1"
                },
                new PlatformModel
                {
                    Id = "2",
                    Type = "Test platform 2"
                }
            };

            _mockPlatformService!
                .Setup(s => s.GetAllModelsAsync())
                .ReturnsAsync(expectedPlatformModels);

            // Act
            var result = await _controller!.GetAllPlatforms() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var returnedPlatformModels = result.Value as List<PlatformModel>;
            Assert.IsNotNull(returnedPlatformModels);
            Assert.AreEqual(expectedPlatformModels.Count, returnedPlatformModels.Count);
        }

        [TestMethod]
        public async Task GetAllPlatforms_ReturnsStatusCode500_WhenExceptionIsThrown()
        {
            // Arrange
            _mockPlatformService!
                .Setup(s => s.GetAllModelsAsync())
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller!.GetAllPlatforms();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var resultAsStatusResult = result as ObjectResult;
            Assert.AreEqual(500, resultAsStatusResult!.StatusCode);
            Assert.AreEqual("Error while getting all platforms.", resultAsStatusResult.Value);
        }

        [TestMethod]
        public async Task DeletePlatformById_ReturnsNoContent_WhenPlatformExistsAndIsSuccessfullyDeleted()
        {
            // Arrange
            var platformId = "1";

            _mockPlatformService!
                .Setup(s => s.DeletePlatformAsync(platformId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller!.DeletePlatformById(platformId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeletePlatformById_ReturnsNotFound_WhenPlatformDoesNotExist()
        {
            // Arrange
            var platformId = "1";

            _mockPlatformService!
                .Setup(s => s.DeletePlatformAsync(platformId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller!.DeletePlatformById(platformId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdatePlatform_ReturnsCreatedAtActionResult_WhenPlatformDtoIsValidAndExists()
        {
            // Arrange
            var platformDto = new PlatformUpdateDto
            {
                Platform = new PlatformResponseForUpdateDto
                {
                    Id = "1",
                    Type = "Test Platform",
                }
            };

            _mockPlatformService!
                .Setup(s => s.UpdatePlatformAsync(platformDto))
                .ReturnsAsync(platformDto.Platform);

            // Act
            var result = await _controller!.UpdatePlatform(platformDto) as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.AreEqual("UpdatePlatform", result.ActionName);
        }

        [TestMethod]
        public async Task UpdatePlatform_ReturnsNotFoundWhenPlatformDoesNotExist()
        {
            // Arrange
            var platformDto = new PlatformUpdateDto
            {
                Platform = new PlatformResponseForUpdateDto
                {
                    Id = "1",
                    Type = "Test Platform",
                }
            };

            _mockPlatformService!
                .Setup(s => s.UpdatePlatformAsync(platformDto))
                .ThrowsAsync(new ArgumentException());

            // Act
            var result = await _controller!.UpdatePlatform(platformDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetGamesByPlatformId_ReturnsOkResult_WithListOfGames_WhenGamesExist()
        {
            // Arrange
            var platformId = "1";

            var expectedGames = new List<GetGameNameByPlatformDto>
            {
                new GetGameNameByPlatformDto
                {
                    Id = "1",
                    Name = "Test Game 1"
                },
                new GetGameNameByPlatformDto
                {
                    Id = "2",
                    Name = "Test Game 2"
                },
            };

            _mockPlatformService!
                .Setup(s => s.GetGamesNameByPlatformId(platformId))
                .ReturnsAsync(expectedGames);

            // Act
            var actionResult = await _controller!.GetGamesByPlatformId(platformId);

            // Assert
            Assert.IsNotNull(actionResult.Result);
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));

            var okResult = actionResult.Result as OkObjectResult;
            var returnedGames = okResult!.Value as List<GetGameNameByPlatformDto>;

            Assert.IsNotNull(returnedGames);
            Assert.AreEqual(expectedGames.Count, returnedGames.Count);
        }
    }
}
