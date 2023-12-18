using AutoMapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Moq;
using ServiceLayer.Models.Publisher;
using ServiceLayer.Services;

namespace StoreAPI.Tests.ServiceLayerTests
{
    [TestClass]
    public class PublisherServiceTests
    {
        private Mock<IUnitOfWork>? _mockUnitOfWork;
        private Mock<IMapper>? _mockMapper;
        private PublisherService? _publisherService;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();

            _publisherService = new PublisherService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [TestMethod]
        public async Task CreatePublisherAsync_CallsCreateAsyncAndReturnsPublisherResponseDto()
        {
            // Arrange
            var publisherCreateDto = new PublisherCreateDto
            {
                Publisher = new PublisherResponseDto
                {
                    CompanyName = "Test Company",
                    Description = "Test Description",
                    HomePage = "www.testhomepage.com"
                }
            };

            var publisher = new Publisher
            {
                CompanyName = publisherCreateDto.Publisher.CompanyName,
                Description = publisherCreateDto.Publisher.Description,
                HomePage = publisherCreateDto.Publisher.HomePage
            };

            var expectedResponseDto = new PublisherResponseDto
            {
                CompanyName = publisher.CompanyName,
                Description = publisher.Description,
                HomePage = publisher.HomePage
            };

            _mockUnitOfWork!
                .Setup(uow => uow.PublisherRepository.CreateAsync(publisher))
                .ReturnsAsync(publisher);

            _mockMapper!
                .Setup(m => m.Map<Publisher>(publisherCreateDto))
                .Returns(publisher);

            _mockMapper
                .Setup(m => m.Map<PublisherResponseDto>(publisher))
                .Returns(expectedResponseDto);

            // Act
            var result = await _publisherService!.CreatePublisherAsync(publisherCreateDto);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(expectedResponseDto.CompanyName, result.CompanyName);
            Assert.AreEqual(expectedResponseDto.Description, result.Description);
            Assert.AreEqual(expectedResponseDto.HomePage, result.HomePage);
            _mockUnitOfWork.Verify(u => u.PublisherRepository.CreateAsync(publisher), Times.Once);
        }

        [TestMethod]
        public async Task DeletePublisherAsync_CallsDeletePublisherByIdAsyncAndReturnsTrueWhenPublisherIsFound()
        {
            // Arrange
            var publisherId = "TestPublisherId";
            var deletedPublisher = new Publisher
            {
                Id = publisherId,
                CompanyName = "Test Company",
                Description = "Test Description",
                HomePage = "www.testhomepage.com"
            };

            _mockUnitOfWork!.Setup(uow => uow.PublisherRepository.DeletePublisherByIdAsync(publisherId))
                .ReturnsAsync(deletedPublisher);

            // Act
            var result = await _publisherService!.DeletePublisherAsync(publisherId);

            // Assert
            Assert.IsTrue(result);
            _mockUnitOfWork.Verify(uow => uow.PublisherRepository.DeletePublisherByIdAsync(publisherId), Times.Once);
        }

        [TestMethod]
        public async Task DeletePublisherAsync_CallsDeletePublisherByIdAsyncAndReturnsFalseWhenPublisherIsNotFound()
        {
            // Arrange
            var publisherId = "TestPublisherId";

            _mockUnitOfWork!.Setup(uow => uow.PublisherRepository.DeletePublisherByIdAsync(publisherId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _publisherService!.DeletePublisherAsync(publisherId);

            // Assert
            Assert.IsFalse(result);
            _mockUnitOfWork.Verify(uow => uow.PublisherRepository.DeletePublisherByIdAsync(publisherId), Times.Once);
        }

        [TestMethod]
        public async Task GetAllModelsAsync_ShouldReturnMappedPublishers()
        {
            // Arrange
            var publishers = new List<Publisher>
            {
                new Publisher
                {
                    CompanyName = "Test Company 1",
                    Description = "Test Description 1",
                    HomePage = "www.testhomepage1.com"
                },
                new Publisher
                {
                    CompanyName = "Test Company 2",
                    Description = "Test Description 2",
                    HomePage = "www.testhomepage2.com"
                }
            };

            var expectedModels = new List<PublisherModel>
            {
                new PublisherModel
                {
                    CompanyName = "Test Company 1",
                    Description = "Test Description 1",
                    HomePage = "www.testhomepage1.com"
                },
                new PublisherModel
                {
                    CompanyName = "Test Company 2",
                    Description = "Test Description 2",
                    HomePage = "www.testhomepage2.com"
                }
            };

            _mockUnitOfWork!
                .Setup(uow => uow.PublisherRepository.GetAllAsync())
                .ReturnsAsync(publishers);

            _mockMapper!
                .Setup(m => m.Map<IEnumerable<PublisherModel>>(publishers))
                .Returns(expectedModels);

            // Act
            var result = await _publisherService!.GetAllModelsAsync();

            // Assert
            CollectionAssert.AreEqual(expectedModels, result.ToList());
            _mockUnitOfWork.Verify(uow => uow.PublisherRepository.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetPublisherByIdAsync_GivenExistingId_ReturnsExpectedPublisher()
        {
            // Arrange
            var publisherId = "TestPublisherId";
            var expectedPublisher = new Publisher
            {
                CompanyName = "Test Company",
                Description = "Test Description",
                HomePage = "www.testhomepage.com"
            };

            _mockUnitOfWork!
                .Setup(uow => uow.PublisherRepository.GetByIdAsync(publisherId))
                .ReturnsAsync(expectedPublisher);

            // Act
            var result = await _publisherService!.GetPublisherByIdAsync(publisherId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPublisher, result);
            _mockUnitOfWork.Verify(uow => uow.PublisherRepository.GetByIdAsync(publisherId), Times.Once);
        }

        [TestMethod]
        public async Task GetPublisherByIdAsync_GivenNonExistingId_ReturnsNull()
        {
            // Arrange
            var publisherId = "NonExistingPublisherId";

            _mockUnitOfWork!
                .Setup(uow => uow.PublisherRepository.GetByIdAsync(publisherId))
                .ReturnsAsync(value: null);

            // Act
            var result = await _publisherService!.GetPublisherByIdAsync(publisherId);

            // Assert
            Assert.IsNull(result);
            _mockUnitOfWork.Verify(uow => uow.PublisherRepository.GetByIdAsync(publisherId), Times.Once);
        }

        [TestMethod]
        public async Task GetPublisherModelDescriptionAsync_GivenExistingCompanyName_ReturnsMappedPublisherModel()
        {
            // Arrange
            var companyName = "Test Company";
            var expectedPublisherEntity = new Publisher
            {
                // Assign necessary properties for your Publisher here
                CompanyName = companyName,
                Description = "Test Description",
                HomePage = "www.testhomepage.com"
            };

            var expectedPublisherModel = new PublisherModel
            {
                // Assign necessary properties for your PublisherModel here
                CompanyName = companyName,
                Description = "Test Description",
                HomePage = "www.testhomepage.com"
            };

            _mockUnitOfWork!
                .Setup(uow => uow.PublisherRepository.GetPublisherDetailsByCompanyNameAsync(companyName))
                .ReturnsAsync(expectedPublisherEntity);

            _mockMapper!
                .Setup(m => m.Map<PublisherModel>(expectedPublisherEntity))
                .Returns(expectedPublisherModel);

            // Act
            var result = await _publisherService!.GetPublisherModelDescriptionAsync(companyName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPublisherModel, result);
            _mockUnitOfWork.Verify(uow => uow.PublisherRepository.GetPublisherDetailsByCompanyNameAsync(companyName), Times.Once);
        }

        [TestMethod]
        public async Task GetPublisherModelDescriptionAsync_GivenNonExistingCompanyName_ReturnsNull()
        {
            // Arrange
            var companyName = "NonExistingCompanyName";

            _mockUnitOfWork!
                .Setup(uow => uow.PublisherRepository.GetPublisherDetailsByCompanyNameAsync(companyName))
                .ReturnsAsync(value: null);

            // Act
            var result = await _publisherService!.GetPublisherModelDescriptionAsync(companyName);

            // Assert
            Assert.IsNull(result);
            _mockUnitOfWork.Verify(uow => uow.PublisherRepository.GetPublisherDetailsByCompanyNameAsync(companyName), Times.Once);
        }

        [TestMethod]
        public async Task GetGamesNameByPublisherCompanyName_GivenExistingCompanyName_ReturnsExpectedGameNames()
        {
            // Arrange
            var companyName = "Test Company";
            var games = new List<Game>
            {
                new Game { Name = "Game 1" },
                new Game { Name = "Game 2" },
                new Game { Name = "Game 3" }
            };

            var expectedGameIdNameDtos = games.Select(game => new GetGameNameByPublisherDto { Name = game.Name });

            _mockUnitOfWork!
                .Setup(uow => uow.PublisherRepository.GetGamesByPublisherCompanyName(companyName))
                .ReturnsAsync(games);

            // Act
            var result = await _publisherService!.GetGamesNameByPublisherCompanyName(companyName);

            // Assert
            Assert.AreEqual(expectedGameIdNameDtos.Count(), result.Count());
            foreach (var expected in expectedGameIdNameDtos)
            {
                var dto = result.SingleOrDefault(x => x.Name == expected.Name);
                Assert.IsNotNull(dto);
            }
            _mockUnitOfWork.Verify(uow => uow.PublisherRepository.GetGamesByPublisherCompanyName(companyName), Times.Once);
        }

        [TestMethod]
        public async Task GetGamesNameByPublisherCompanyName_GivenNonExistingCompanyName_ReturnsEmptyList()
        {
            // Arrange
            var companyName = "NonExistingCompanyName";

            _mockUnitOfWork!
                .Setup(uow => uow.PublisherRepository.GetGamesByPublisherCompanyName(companyName))
                .ReturnsAsync(new List<Game>());

            // Act
            var result = await _publisherService!.GetGamesNameByPublisherCompanyName(companyName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockUnitOfWork.Verify(uow => uow.PublisherRepository.GetGamesByPublisherCompanyName(companyName), Times.Once);
        }

        [TestMethod]
        public async Task UpdatePublisherAsync_GivenValidPublisherUpdateDto_ReturnsExpectedPublisherResponseForUpdateDto()
        {
            // Arrange
            var publisherUpdateDto = new PublisherUpdateDto
            {
                Publisher = new PublisherResponseForUpdateDto
                {
                    Id = "TestPublisherId",
                    CompanyName = "Test Company",
                    Description = "Test Description",
                    HomePage = "www.testhomepage.com"
                }
            };

            var updatedPublisher = new Publisher
            {
                Id = publisherUpdateDto.Publisher.Id,
                CompanyName = publisherUpdateDto.Publisher.CompanyName,
                Description = publisherUpdateDto.Publisher.Description,
                HomePage = publisherUpdateDto.Publisher.HomePage
            };

            var expectedPublisherResponseForUpdateDto = new PublisherResponseForUpdateDto
            {
                CompanyName = updatedPublisher.CompanyName,
                Description = updatedPublisher.Description,
                HomePage = updatedPublisher.HomePage,
            };

            _mockUnitOfWork!
                .Setup(uow => uow.PublisherRepository.UpdateAsync(It.IsAny<Publisher>()))
                .ReturnsAsync(updatedPublisher);

            // Act
            var result = await _publisherService!.UpdatePublisherAsync(publisherUpdateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPublisherResponseForUpdateDto.CompanyName, result?.CompanyName);
            Assert.AreEqual(expectedPublisherResponseForUpdateDto.Description, result?.Description);
            Assert.AreEqual(expectedPublisherResponseForUpdateDto.HomePage, result?.HomePage);
            _mockUnitOfWork.Verify(uow => uow.PublisherRepository.UpdateAsync(It.IsAny<Publisher>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdatePublisherAsync_GivenNullPublisherReturnsNull()
        {
            // Arrange
            PublisherUpdateDto? publisherUpdateDto = null;

            _mockUnitOfWork!
                .Setup(uow => uow.PublisherRepository.UpdateAsync(It.IsAny<Publisher>()))
                .ReturnsAsync((Publisher?)null);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _publisherService!.UpdatePublisherAsync(publisherUpdateDto!));
        }
    }
}
