using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceLayer.Interfaces;
using ServiceLayer.Models.Publisher;
using StoreAPI.Controllers;

namespace StoreAPI.Tests.IntegrationTests
{
    [TestClass]
    public class PublishersControllerTests
    {
        private Mock<IPublisherService>? _mockPublisherService;
        private Mock<ILogger<PublishersController>>? _mockLogger;
        private PublishersController? _controller;

        [TestInitialize]
        public void SetUp()
        {
            _mockPublisherService = new Mock<IPublisherService>();
            _mockLogger = new Mock<ILogger<PublishersController>>();
            _controller = new PublishersController(_mockPublisherService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task CreatePublisher_ReturnsCreatedAtActionResult_WithPublisherModel_WhenModelStateIsValid()
        {
            // Arrange
            var newPublisherDto = new PublisherCreateDto
            {
                Publisher = new PublisherResponseDto
                {
                    CompanyName = "Test Publisher",
                    Description = "Test Description",
                    HomePage = "TestHomePage.com"
                }
            };

            var createdPublisherModel = new PublisherResponseDto
            {
                CompanyName = "Test Publisher",
                Description = "Test Description",
                HomePage = "TestHomePage.com"
            };

            _mockPublisherService!
                .Setup(s => s.CreatePublisherAsync(newPublisherDto))
                .ReturnsAsync(createdPublisherModel);

            // Act
            var result = await _controller!.CreatePublisher(newPublisherDto) as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            var returnedPublisherModel = result.Value as PublisherResponseDto;
            Assert.IsNotNull(returnedPublisherModel);
            Assert.AreEqual(createdPublisherModel.CompanyName, returnedPublisherModel.CompanyName);
            Assert.AreEqual(createdPublisherModel.Description, returnedPublisherModel.Description);
            Assert.AreEqual(createdPublisherModel.HomePage, returnedPublisherModel.HomePage);
        }

        [TestMethod]
        public async Task CreatePublisher_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var publisherDto = new PublisherCreateDto
            {
                Publisher = new PublisherResponseDto
                {
                    CompanyName = "Test Publisher",
                    Description = "Test Description",
                    HomePage = "TestHomePage.com"
                }
            };

            _controller!.ModelState.AddModelError("Error", "Some kind of error");

            // Act
            var result = await _controller.CreatePublisher(publisherDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetAllPublishers_ReturnsOkObjectResult_WithListOfPublisherModels_WhenOperationIsSuccessful()
        {
            // Arrange
            var expectedPublisherModels = new List<PublisherModel>
            {
                new PublisherModel
                {
                    Id = "1",
                    CompanyName = "Test publisher 1",
                    Description = "Test description 1",
                    HomePage = "TestHomepage1.com"
                },
                new PublisherModel
                {
                    Id = "2",
                    CompanyName = "Test publisher 2",
                    Description = "Test description 2",
                    HomePage = "TestHomepage2.com"
                }
            };

            _mockPublisherService!
                .Setup(s => s.GetAllModelsAsync())
                .ReturnsAsync(expectedPublisherModels);

            // Act
            var result = await _controller!.GetAllPublishers() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var returnedPublisherModels = result.Value as List<PublisherModel>;
            Assert.IsNotNull(returnedPublisherModels);
            Assert.AreEqual(expectedPublisherModels.Count, returnedPublisherModels.Count);
        }

        [TestMethod]
        public async Task GetAllPublishers_ReturnsStatusCode500_WhenExceptionIsThrown()
        {
            // Arrange
            _mockPublisherService!
                .Setup(s => s.GetAllModelsAsync())
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller!.GetAllPublishers();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var resultAsStatusResult = result as ObjectResult;
            Assert.AreEqual(500, resultAsStatusResult!.StatusCode);
            Assert.AreEqual("Error while getting all publishers.", resultAsStatusResult.Value);
        }

        [TestMethod]
        public async Task GetPublisherDetails_ReturnsOkObjectResult_WithPublisherDetails_WhenPublisherExists()
        {
            // Arrange
            var publisherName = "Test Publisher";

            var expectedPublisherDetails = new PublisherModel
            {
                Id = "1",
                CompanyName = publisherName,
                Description = "Test Description",
                HomePage = "TestHomePage.com"
            };

            _mockPublisherService!
                .Setup(s => s.GetPublisherModelDescriptionAsync(publisherName))
                .ReturnsAsync(expectedPublisherDetails);

            // Act
            var result = await _controller!.GetPublisherDetails(publisherName) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var returnedPublisherDetails = result.Value as PublisherModel;
            Assert.IsNotNull(returnedPublisherDetails);
            Assert.AreEqual(expectedPublisherDetails, returnedPublisherDetails);
        }

        [TestMethod]
        public async Task GetPublisherDetails_ReturnsNotFound_WhenPublisherDoesNotExist()
        {
            // Arrange
            var publisherName = "Test Publisher";

            _mockPublisherService!
                .Setup(s => s.GetPublisherModelDescriptionAsync(publisherName))
                .ReturnsAsync((PublisherModel?)null);

            // Act
            var result = await _controller!.GetPublisherDetails(publisherName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeletePublisherById_ReturnsNoContent_WhenPublisherExistsAndIsSuccessfullyDeleted()
        {
            // Arrange
            var publisherId = "1";

            _mockPublisherService!
                .Setup(s => s.DeletePublisherAsync(publisherId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller!.DeletePublisherById(publisherId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeletePublisherById_ReturnsNotFound_WhenPublisherDoesNotExist()
        {
            // Arrange
            var publisherId = "1";

            _mockPublisherService!
                .Setup(s => s.DeletePublisherAsync(publisherId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller!.DeletePublisherById(publisherId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetGamesNameByPublisherId_ReturnsOkResult_WithListOfGames_WhenCompanyExistAndGamesExist()
        {
            // Arrange
            var publisherCompanyName = "Test Publisher Company Name";
            var expectedGames = new List<GetGameNameByPublisherDto>
            {
                new GetGameNameByPublisherDto
                {
                    Name = "Test Game 1"
                },
                new GetGameNameByPublisherDto
                {
                    Name = "Test Game 2"
                },
            };

            _mockPublisherService!
                .Setup(s => s.GetGamesNameByPublisherCompanyName(publisherCompanyName))
                .ReturnsAsync(expectedGames);

            // Act
            var actionResult = await _controller!.GetGamesNameByPublisherId(publisherCompanyName);

            // Assert
            Assert.IsNotNull(actionResult.Result);
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));

            var okResult = actionResult.Result as OkObjectResult;
            var returnedGames = okResult!.Value as List<GetGameNameByPublisherDto>;

            Assert.IsNotNull(returnedGames);
            Assert.AreEqual(expectedGames.Count, returnedGames.Count);
        }

        [TestMethod]
        public async Task UpdatePublisher_ReturnsCreatedAtAction_WhenPublisherExistsAndIsSuccessfullyUpdated()
        {
            // Arrange
            var updatePublisherDto = new PublisherUpdateDto
            {
                Publisher = new PublisherResponseForUpdateDto
                {
                    Id = "1",
                    CompanyName = "Test publisher",
                    Description = "Test description",
                    HomePage = "TestHomePage.com"
                }
            };

            var expectedUpdatedPublisher = new PublisherResponseForUpdateDto
            {
                Id = "1",
                CompanyName = "Test publisher",
                Description = "Test description",
                HomePage = "TestHomePage.com"
            };

            _mockPublisherService!
                .Setup(s => s.UpdatePublisherAsync(updatePublisherDto))
                .ReturnsAsync(expectedUpdatedPublisher);

            // Act
            var result = await _controller!.UpdatePublisher(updatePublisherDto) as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            var returnedPublisher = result.Value as PublisherUpdateDto;
            Assert.IsNotNull(returnedPublisher);
            Assert.AreEqual(expectedUpdatedPublisher.Id, returnedPublisher.Publisher!.Id);
            Assert.AreEqual(expectedUpdatedPublisher.CompanyName, returnedPublisher.Publisher.CompanyName);
            Assert.AreEqual(expectedUpdatedPublisher.Description, returnedPublisher.Publisher.Description);
            Assert.AreEqual(expectedUpdatedPublisher.HomePage, returnedPublisher.Publisher.HomePage);
        }

        [TestMethod]
        public async Task UpdatePublisher_ReturnsBadRequest_WhenPublisherIsNull()
        {
            #nullable disable
            // Arrange
            PublisherUpdateDto publisherDto = null;

            // Act
            var result = await _controller!.UpdatePublisher(publisherDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            #nullable enable
        }

        [TestMethod]
        public async Task UpdatePublisher_ReturnsNotFound_WhenPublisherDoesNotExist()
        {
            // Arrange
            var nonExistantPublisherDto = new PublisherUpdateDto
            {
                Publisher = new PublisherResponseForUpdateDto
                {
                    Id = "1",
                    CompanyName = "Test publisher",
                    Description = "Test description",
                    HomePage = "TestHomePage.com"
                }
            };

            _mockPublisherService!
                .Setup(s => s.UpdatePublisherAsync(nonExistantPublisherDto))
                .ThrowsAsync(new ArgumentException());

            // Act
            var result = await _controller!.UpdatePublisher(nonExistantPublisherDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
