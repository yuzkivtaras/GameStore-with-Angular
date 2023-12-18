using AutoMapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Moq;
using ServiceLayer.Models.Genre;
using ServiceLayer.Services;

namespace StoreAPI.Tests.ServiceLayerTests
{
    [TestClass]
    public class GenreServiceTests
    {
        private Mock<IUnitOfWork>? _mockUnitOfWork;
        private Mock<IMapper>? _mockMapper;
        private GenreService? _genreService;

        public GenreServiceTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _genreService = new GenreService(_mockUnitOfWork.Object, UnitTestHelper.CreateMapperProfile());
        }

        [TestMethod]
        public async Task CreateGenreAsync_ShouldCreateNewGenreAndReturnGenreResponseDto()
        {
            // Arrange
            var genreCreateDto = new GenreCreateDto
            {
                Genre = new GenreResponseDto
                {
                    Name = "TestGenre",
                    ParentGenreId = "1"
                }
            };

            var expectedCreatedGenre = new Genre
            {
                Name = genreCreateDto.Genre.Name,
                ParentGenreId = genreCreateDto.Genre.ParentGenreId
            };

            _mockUnitOfWork!
                .Setup(uow => uow.GenreRepository.CreateAsync(It.IsAny<Genre>()))
                .ReturnsAsync(expectedCreatedGenre);

            // Act
            var result = await _genreService!.CreateGenreAsync(genreCreateDto);

            // Assert
            Assert.IsNotNull(result, "Result of CreateGenreAsync is null.");
            Assert.AreEqual(expectedCreatedGenre.Name, result.Name, "Created genre name does not match the expected genre name.");
            Assert.AreEqual(expectedCreatedGenre.ParentGenreId, result.ParentGenreId, "Created genre parent genre ID does not match the expected genre parent genre ID.");
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.CreateAsync(It.Is<Genre>(g => g.Name == expectedCreatedGenre.Name && g.ParentGenreId == expectedCreatedGenre.ParentGenreId)), Times.Once);
        }

        [TestMethod]
        public async Task DeleteGenreModelAsync_ShouldDeleteGenreAndReturnDeletedGenreModel()
        {
            // Arrange
            string genreIdToDelete = "1";

            var deletedGenre = new Genre
            {
                Id = genreIdToDelete,
                Name = "TestGenre",
                ParentGenreId = "2"
            };

            var expectedDeletedGenreModel = new GenreModel
            {
                Id = deletedGenre.Id,
                Name = deletedGenre.Name,
                ParentGenreId = deletedGenre.ParentGenreId
            };

            _mockUnitOfWork!
                .Setup(uow => uow.GenreRepository.DeleteGenreByIdAsync(genreIdToDelete))
                .ReturnsAsync(deletedGenre);

            // Act
            var result = await _genreService!.DeleteGenreModelAsync(genreIdToDelete);

            // Assert
            Assert.IsNotNull(result, "Result of DeleteGenreModelAsync is null.");
            Assert.AreEqual(expectedDeletedGenreModel.Id, result.Id, "Deleted genre model ID does not match the expected genre model ID.");
            Assert.AreEqual(expectedDeletedGenreModel.Name, result.Name, "Deleted genre model name does not match the expected genre model name.");
            Assert.AreEqual(expectedDeletedGenreModel.ParentGenreId, result.ParentGenreId, "Deleted genre model parent genre ID does not match the expected genre model parent genre ID.");
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.DeleteGenreByIdAsync(genreIdToDelete), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllModelsAsync_ShouldReturnAllGenreModels()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { Id = "1", Name = "TestGenre1", ParentGenreId = "2" },
                new Genre { Id = "2", Name = "TestGenre2", ParentGenreId = "3" },
                new Genre { Id = "3", Name = "TestGenre3", ParentGenreId = "4" }
            };

            _mockUnitOfWork!
                .Setup(uow => uow.GenreRepository.GetAllAsync())
                .ReturnsAsync(genres);

            // Act
            var result = await _genreService!.GetAllModelsAsync();

            // Assert
            Assert.IsNotNull(result, "Result of GetAllModelsAsync is null.");
            Assert.AreEqual(genres.Count, result.Count(), "Count of fetched genre models does not match the expected count.");

            int index = 0;
            foreach (var genreModel in result)
            {
                Assert.AreEqual(genres[index].Id, genreModel.Id, $"Fetched genre model at index {index} has an unexpected ID.");
                Assert.AreEqual(genres[index].Name, genreModel.Name, $"Fetched genre model at index {index} has an unexpected name.");
                Assert.AreEqual(genres[index].ParentGenreId, genreModel.ParentGenreId, $"Fetched genre model at index {index} has an unexpected parent genre ID.");
                index++;
            }

            _mockUnitOfWork.Verify(uow => uow.GenreRepository.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetGenreModelDescriptionAsync_ShouldReturnGenreModelDescription()
        {
            // Arrange
            string genreIdToGet = "1";

            var genreEntity = new Genre { Id = genreIdToGet, Name = "TestGenre", ParentGenreId = "2" };

            _mockUnitOfWork!
                .Setup(uow => uow.GenreRepository.GetGenreDetailsByIdAsync(genreIdToGet))
                .ReturnsAsync(genreEntity);

            // Act
            var result = await _genreService!.GetGenreModelDescriptionAsync(genreIdToGet);

            // Assert
            Assert.IsNotNull(result, "Result of GetGenreModelDescriptionAsync is null.");
            Assert.AreEqual(genreEntity.Id, result.Id, "Fetched genre model ID does not match the expected genre ID.");
            Assert.AreEqual(genreEntity.Name, result.Name, "Fetched genre model name does not match the expected genre name.");
            Assert.AreEqual(genreEntity.ParentGenreId, result.ParentGenreId, "Fetched genre model parent genre ID does not match the expected genre parent genre ID.");
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.GetGenreDetailsByIdAsync(genreIdToGet), Times.Once);
        }

        [TestMethod]
        public async Task GetGenresByIdsAsync_ShouldReturnGenresByIds()
        {
            // Arrange
            var genreIdsToGet = new List<string> { "1", "2", "3" };

            var genres = new List<Genre>
            {
                new Genre { Id = "1", Name = "TestGenre1" },
                new Genre { Id = "2", Name = "TestGenre2" },
                new Genre { Id = "3", Name = "TestGenre3" },
                new Genre { Id = "4", Name = "TestGenre4" },
                new Genre { Id = "5", Name = "TestGenre5" }
            };

            _mockUnitOfWork!
                .Setup(uow => uow.GenreRepository.GetByIdsAsync(genreIdsToGet))
                .ReturnsAsync(genres.Where(g => genreIdsToGet.Contains(g.Id)).ToList());

            // Act
            var result = await _genreService!.GetGenresByIdsAsync(genreIdsToGet);

            // Assert
            Assert.IsNotNull(result, "Result of GetGenresByIdsAsync is null.");
            Assert.AreEqual(genreIdsToGet.Count, result.Count, "Fetched genres count does not match the expected count.");
            foreach (var genre in result)
            {
                Assert.IsTrue(genreIdsToGet.Contains(genre.Id), "Fetched genre is not on the list of expected genre IDs.");
            }
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.GetByIdsAsync(genreIdsToGet), Times.Once);
        }

        [TestMethod]
        public async Task GetGamesNameByGenreId_ShouldReturnGameNamesByGenreId()
        {
            // Arrange
            string genreIdToGet = "1";

            var games = new List<Game>
            {
                new Game { Id = "1", Name = "TestGame1" },
                new Game { Id = "2", Name = "TestGame2" },
                new Game { Id = "3", Name = "TestGame3" },
            };

            var gameGenres = new List<GameGenre>
            {
                new GameGenre { GamesKey = "1", Game = games[0], GenresId = "1", Genre = new Genre { Id = "1", Name = "TestGenre1" } },
                new GameGenre { GamesKey = "2", Game = games[1], GenresId = "1", Genre = new Genre { Id = "1", Name = "TestGenre1" } },
                new GameGenre { GamesKey = "3", Game = games[2], GenresId = "2", Genre = new Genre { Id = "2", Name = "TestGenre2" } },
            };

            var expectedResultGames = games.Where(g => gameGenres.Any(gg => gg.Game == g && gg.GenresId == genreIdToGet)).ToList();

            _mockUnitOfWork!
                .Setup(uow => uow.GenreRepository.GetGamesByGenreId(genreIdToGet))
                .ReturnsAsync(expectedResultGames);

            // Act
            var result = await _genreService!.GetGamesNameByGenreId(genreIdToGet);

            // Assert
            Assert.IsNotNull(result, "Result of GetGamesNameByGenreId is null.");
            Assert.AreEqual(expectedResultGames.Count, result.Count(), "Fetched games count does not match the expected count.");

            int index = 0;
            foreach (var gameDto in result)
            {
                Assert.AreEqual(expectedResultGames[index].Id, gameDto.Id, $"Fetched game at index {index} has an unexpected ID.");
                Assert.AreEqual(expectedResultGames[index].Name, gameDto.Name, $"Fetched game at index {index} has an unexpected name.");
                index++;
            }
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.GetGamesByGenreId(genreIdToGet), Times.Once);
        }

        [TestMethod]
        public async Task GetGamesNameByParentId_ShouldReturnGameNamesByParentId()
        {
            // Arrange
            string parentIdToGet = "1";

            var games = new List<Game>
            {
                new Game { Id = "1", Name = "TestGame1" },
                new Game { Id = "2", Name = "TestGame2" },
                new Game { Id = "3", Name = "TestGame3" },
            };

            var gameGenres = new List<GameGenre>
            {
                new GameGenre { GamesKey = "1", Game = games[0], GenresId = "1", Genre = new Genre { Id = "1", Name = "TestGenre1", ParentGenreId = "1" } },
                new GameGenre { GamesKey = "2", Game = games[1], GenresId = "2", Genre = new Genre { Id = "2", Name = "TestGenre2", ParentGenreId = "1" } },
                new GameGenre { GamesKey = "3", Game = games[2], GenresId = "3", Genre = new Genre { Id = "3", Name = "TestGenre3", ParentGenreId = "2" } },
            };

            var expectedResultGames = games.Where(g => gameGenres.Any(gg => gg.Game == g && gg.Genre!.ParentGenreId == parentIdToGet)).ToList();

            _mockUnitOfWork!
                .Setup(uow => uow.GenreRepository.GetGamesByParentId(parentIdToGet))
                .ReturnsAsync(expectedResultGames);

            // Act
            var result = await _genreService!.GetGamesNameByParentId(parentIdToGet);

            // Assert
            Assert.IsNotNull(result, "Result of GetGamesNameByParentId is null.");
            Assert.AreEqual(expectedResultGames.Count, result.Count(), "Fetched games count does not match the expected count.");

            int index = 0;
            foreach (var gameDto in result)
            {
                Assert.AreEqual(expectedResultGames[index].Id, gameDto.Id, $"Fetched game at index {index} has an unexpected ID.");
                Assert.AreEqual(expectedResultGames[index].Name, gameDto.Name, $"Fetched game at index {index} has an unexpected name.");
                index++;
            }
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.GetGamesByParentId(parentIdToGet), Times.Once);
        }    
    }
}
