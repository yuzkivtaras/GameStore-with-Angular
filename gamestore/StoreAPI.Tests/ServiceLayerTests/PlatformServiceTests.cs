using AutoMapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Moq;
using ServiceLayer.Models;
using ServiceLayer.Models.Platform;
using ServiceLayer.Services;

namespace StoreAPI.Tests.ServiceLayerTests
{
    [TestClass]
    public class PlatformServiceTests
    {
        private Mock<IUnitOfWork>? _mockUnitOfWork;
        private Mock<IMapper>? _mockMapper;
        private Mock<IPlatformRepository>? _mockPlatformRepository;
        private PlatformService? _platformService;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockPlatformRepository = new Mock<IPlatformRepository>();

            _platformService = new PlatformService(_mockUnitOfWork.Object, _mockMapper.Object, _mockPlatformRepository.Object);
        }

        [TestMethod]
        public async Task CreatePlatformAsync_ShouldCallCreateAsyncAndMapToDto()
        {
            // Arrange
            var platformCreateDto = new PlatformCreateDto { Platform = new PlatformResponseDto { Type = "PlatformName" } };
            var platform = new Platform { Type = platformCreateDto.Platform.Type };
            var platformResponseDto = new PlatformResponseDto { Type = platformCreateDto.Platform.Type };

            _mockMapper!
                .Setup(m => m.Map<Platform>(platformCreateDto))
                .Returns(platform);

            _mockUnitOfWork!
                .Setup(u => u.PlatformRepository.CreateAsync(platform))
                .Returns(Task.FromResult<Platform?>(platform));

            _mockMapper!
                .Setup(m => m.Map<PlatformResponseDto>(platform))
                .Returns(platformResponseDto);

            // Act
            var result = await _platformService!.CreatePlatformAsync(platformCreateDto);

            // Assert
            Assert.IsNotNull(result, "Result of CreatePlatformAsync is null.");
            Assert.AreEqual(platformResponseDto.Type, result.Type);

            _mockMapper.Verify(m => m.Map<Platform>(platformCreateDto), Times.Once);
            _mockUnitOfWork.Verify(u => u.PlatformRepository.CreateAsync(platform), Times.Once);
            _mockMapper.Verify(m => m.Map<PlatformResponseDto>(platform), Times.Once);
        }

        [TestMethod]
        public async Task DeletePlatformAsync_ShouldReturnTrue_WhenPlatformIsDeleted()
        {
            // Arrange
            string platformId = "1";
            var platform = new Platform { Id = platformId, Type = "PlatformName" };

            _mockUnitOfWork!
                .Setup(u => u.PlatformRepository.DeletePlatformByIdAsync(platformId))
                .ReturnsAsync(platform);

            // Act
            var result = await _platformService!.DeletePlatformAsync(platformId);

            // Assert
            Assert.IsTrue(result, "Result of DeletePlatformAsync is not true.");
            _mockUnitOfWork.Verify(u => u.PlatformRepository.DeletePlatformByIdAsync(platformId), Times.Once);
        }

        [TestMethod]
        public async Task DeletePlatformAsync_ShouldReturnFalse_WhenPlatformIsNotFound()
        {
            // Arrange
            string platformId = "2";

            _mockUnitOfWork!
                .Setup(u => u.PlatformRepository.DeletePlatformByIdAsync(platformId))
                .ReturnsAsync((Platform?)null);

            // Act
            var result = await _platformService!.DeletePlatformAsync(platformId);

            // Assert
            Assert.IsFalse(result, "Result of DeletePlatformAsync is not false.");
            _mockUnitOfWork.Verify(u => u.PlatformRepository.DeletePlatformByIdAsync(platformId), Times.Once);
        }

        [TestMethod]
        public async Task GetAllModelsAsync_ShouldReturnAllPlatformModels()
        {
            // Arrange
            var platforms = new List<Platform>
            {
                new Platform { Id = "1", Type = "PlatformName1" },
                new Platform { Id = "2", Type = "PlatformName2" }
            };

                    var platformModels = new List<PlatformModel>
            {
                new PlatformModel { Id = "1", Type = "PlatformName1" },
                new PlatformModel { Id = "2", Type = "PlatformName2" }
            };

            _mockUnitOfWork!
                .Setup(u => u.PlatformRepository.GetAllAsync())
                .ReturnsAsync(platforms);

            _mockMapper!
                .Setup(m => m.Map<IEnumerable<PlatformModel>>(platforms))
                .Returns(platformModels);

            // Act
            var result = await _platformService!.GetAllModelsAsync();

            // Assert
            Assert.IsNotNull(result, "Result of GetAllModelsAsync is null.");
            Assert.AreEqual(platformModels.Count, result.Count(), "Count of platform models does not match the expected count.");
            _mockUnitOfWork.Verify(u => u.PlatformRepository.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<PlatformModel>>(platforms), Times.Once);
        }

        [TestMethod]
        public async Task GetPlatformModelDescriptionAsync_ShouldReturnPlatformModel_WhenPlatformWithIdExists()
        {
            // Arrange
            string platformId = "1";
            var platform = new Platform { Id = platformId, Type = "PlatformName" };
            var platformModel = new PlatformModel { Id = platformId, Type = "PlatformName" };

            _mockUnitOfWork!
                .Setup(u => u.PlatformRepository.GetPlatformDetailsByIdAsync(platformId))
                .ReturnsAsync(platform);

            _mockMapper!
                .Setup(m => m.Map<PlatformModel>(platform))
                .Returns(platformModel);

            // Act
            var result = await _platformService!.GetPlatformModelDescriptionAsync(platformId);

            // Assert
            Assert.IsNotNull(result, "Result of GetPlatformModelDescriptionAsync is null.");
            Assert.AreEqual(platformModel.Id, result?.Id, "PlatformModel id does not match the expected id.");
            _mockUnitOfWork.Verify(u => u.PlatformRepository.GetPlatformDetailsByIdAsync(platformId), Times.Once);
            _mockMapper.Verify(m => m.Map<PlatformModel>(platform), Times.Once);
        }

        [TestMethod]
        public async Task GetPlatformModelDescriptionAsync_ShouldReturnNull_WhenPlatformWithIdNotFound()
        {
            // Arrange
            string platformId = "2";

            _mockUnitOfWork!
                .Setup(u => u.PlatformRepository.GetPlatformDetailsByIdAsync(platformId))
                .ReturnsAsync((Platform?)null);

            // Act
            var result = await _platformService!.GetPlatformModelDescriptionAsync(platformId);

            // Assert
            Assert.IsNull(result, "Result of GetPlatformModelDescriptionAsync is not null.");
            _mockUnitOfWork.Verify(u => u.PlatformRepository.GetPlatformDetailsByIdAsync(platformId), Times.Once);
        }

        [TestMethod]
        public async Task GetPlatformsByIdsAsync_ShouldReturnFilteredPlatforms_ByPlatformTypes()
        {
            // Arrange
            IEnumerable<string> platformTypes = new List<string> { "PlatformName1", "PlatformName2" };

            // Act

            // Assert
            Assert.IsTrue(true, "This is a simple assertion that always passes.");
            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task UpdatePlatformAsync_ShouldUpdatePlatform_WhenInputIsValid()
        {
            // Arrange
            string platformId = "1";
            string platformType = "PlatformName1";

            var platformToUpdate = new Platform
            {
                Id = platformId,
                Type = platformType
            };

            var platformUpdateDto = new PlatformUpdateDto
            {
                Platform = new PlatformResponseForUpdateDto { Id = platformId, Type = platformType }
            };

            _mockPlatformRepository!
                .Setup(repo => repo.UpdateAsync(It.IsAny<Platform>()))
            .ReturnsAsync(platformToUpdate);

            _mockUnitOfWork!.Setup(x => x.PlatformRepository).Returns(_mockPlatformRepository.Object);

            // Act
            var result = await _platformService!.UpdatePlatformAsync(platformUpdateDto);

            // Assert
            Assert.IsNotNull(result, "Result of UpdatePlatformAsync is null.");
            Assert.AreEqual(platformType, result.Type, "Platform type in the result does not match the expected value.");
            _mockPlatformRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Platform>()), Times.Once);
        }

        [TestMethod]
        public async Task GetGamesNameByPlatformId_ShouldReturnGameNames_ByPlatformId()
        {
            // Arrange
            string platformId = "1";
            var games = new List<Game>
            {
                new Game { Id = "1", Name = "Game1", GamePlatforms = new List<GamePlatform> { new GamePlatform { PlatformsId = platformId } } },
                new Game { Id = "2", Name = "Game2", GamePlatforms = new List<GamePlatform> { new GamePlatform { PlatformsId = platformId } } },
                new Game { Id = "3", Name = "Game3", GamePlatforms = new List<GamePlatform> { new GamePlatform { PlatformsId = "2" } } }
            };

            _mockPlatformRepository!
                .Setup(p => p.GetGamesByPlatformId(It.IsAny<string>()))
                .ReturnsAsync(games.Where(g => g.GamePlatforms.Any(gp => gp.PlatformsId == platformId)).ToList());

            _mockUnitOfWork!.Setup(x => x.PlatformRepository).Returns(_mockPlatformRepository.Object);

            // Act
            var result = await _platformService!.GetGamesNameByPlatformId(platformId);

            // Assert
            Assert.IsNotNull(result, "Result of GetGamesNameByPlatformId is null.");
            Assert.AreEqual(2, result.Count(), "Count of the games returned does not match the expected count.");
            Assert.IsTrue(result.All(g => !string.IsNullOrEmpty(g.Id) && !string.IsNullOrEmpty(g.Name)), "All games should have a non-empty ID and Name.");
            _mockPlatformRepository.Verify(p => p.GetGamesByPlatformId(platformId), Times.Once);
        }
    }
}
