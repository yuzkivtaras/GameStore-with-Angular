using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace StoreAPI.Tests.DataAccessLayerTests
{
    [TestClass]
    public class GenreRepositoryTests
    {
        private GameStoreDbContext? _context;
        private GenreRepository? _genreRepository;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _context = new GameStoreDbContext(UnitTestHelper.GetUnitTestDbOptions());
            _genreRepository = new GenreRepository(_context);
        }


        [TestMethod]
        public void GetAll_ShouldReturnAllGenres()
        {
            // Arrange
            var expectedGenres = _context?.Genres.ToList();

            // Act
            var actualGenres = _genreRepository?.GetAll().ToList();

            // Assert
            Assert.AreEqual(expectedGenres?.Count, actualGenres?.Count);
            for (int i = 0; i < expectedGenres?.Count; i++)
            {
                Assert.AreEqual(expectedGenres[i].Id, actualGenres?[i].Id);
                Assert.AreEqual(expectedGenres[i].Name, actualGenres?[i].Name);
            }
        }

        [TestMethod]
        public async Task GetByIdsAsync_ShouldReturnGenresByIds()
        {
            // Arrange
            var expectedGenreIds = _context?.Genres.Select(g => g.Id).ToList();

            // Act
            var actualGenres = await _genreRepository!.GetByIdsAsync(expectedGenreIds!);

            // Assert
            Assert.IsNotNull(actualGenres);
            Assert.AreEqual(expectedGenreIds!.Count, actualGenres.Count);

            foreach (var expectedId in expectedGenreIds)
            {
                Assert.IsTrue(actualGenres.Any(g => g.Id == expectedId));
            }
        }

        [TestMethod]
        public async Task DeleteGenreByIdAsync_ShouldDeleteGenre()
        {
            // Arrange
            var genreToAdd = new Genre { Id = "4", Name = "Genre4" };
            _context!.Genres.Add(genreToAdd);
            await _context.SaveChangesAsync();

            // Act
            var deletedGenre = await _genreRepository!.DeleteGenreByIdAsync(genreToAdd.Id);

            // Assert
            Assert.IsNotNull(deletedGenre);
            Assert.AreEqual(genreToAdd.Id, deletedGenre.Id);
            Assert.AreEqual(genreToAdd.Name, deletedGenre.Name);

            var retrievedGenreAfterDeletion = await _context.Genres.FindAsync(genreToAdd.Id);
            Assert.IsNull(retrievedGenreAfterDeletion);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllGenres()
        {
            // Arrange
            var expectedGenres = _context!.Genres.ToList();

            // Act
            var actualGenres = await _genreRepository!.GetAllAsync();

            // Assert
            Assert.AreEqual(expectedGenres.Count, actualGenres.Count());

            for (int i = 0; i < expectedGenres.Count; i++)
            {
                Assert.AreEqual(expectedGenres[i].Id, actualGenres.ElementAt(i).Id);
                Assert.AreEqual(expectedGenres[i].Name, actualGenres.ElementAt(i).Name);
            }
        }

        [TestMethod]
        public async Task GetGenreDetailsByIdAsync_ShouldReturnGenreDetails()
        {
            // Arrange
            var targetGenreId = _context!.Genres.FirstOrDefault()?.Id;
            Assert.IsNotNull(targetGenreId, "No genres available in the context to test.");

            var expectedGenre = _context?.Genres.FirstOrDefault(g => g.Id == targetGenreId);

            // Act
            var actualGenre = await _genreRepository!.GetGenreDetailsByIdAsync(targetGenreId);

            // Assert
            Assert.IsNotNull(actualGenre);
            Assert.AreEqual(expectedGenre!.Id, actualGenre.Id);
            Assert.AreEqual(expectedGenre!.Name, actualGenre.Name);
        }

        [TestMethod]
        public async Task GetGamesByGenreId_ShouldReturnGames()
        {
            // Arrange
            var genre1 = new Genre { Id = "10", Name = "Name1" };
            _context!.Genres.Add(genre1);

            var genre2 = new Genre { Id = "11", Name = "Name2" };
            _context.Genres.Add(genre2);

            var game1 = new Game { Key = "1", Name = "Game1" };
            var game2 = new Game { Key = "2", Name = "Game2" };
            var game3 = new Game { Key = "3", Name = "Game3" };
            _context.Games.AddRange(game1, game2, game3);

            _context.GameGenres.AddRange(
                new GameGenre { GamesKey = game1.Key, GenresId = genre1.Id },
                new GameGenre { GamesKey = game1.Key, GenresId = genre2.Id },
                new GameGenre { GamesKey = game2.Key, GenresId = genre1.Id },
                new GameGenre { GamesKey = game3.Key, GenresId = genre2.Id });

            await _context.SaveChangesAsync();

            // Act
            var gamesForGenre1 = await _genreRepository!.GetGamesByGenreId(genre1.Id);

            // Assert
            Assert.IsNotNull(gamesForGenre1);
            Assert.AreEqual(2, gamesForGenre1.Count());
            Assert.IsTrue(gamesForGenre1.Any(g => g.Name == game1.Name));
            Assert.IsTrue(gamesForGenre1.Any(g => g.Name == game2.Name));
        }

        [TestMethod]
        public async Task GetGamesByParentId_ShouldReturnGamesByParentGenreId()
        {
            // Arrange
            var parentId = "1";
            var expectedGames = _context!.GameGenres
                .Include(x => x.Game)
                .Where(g => g.Genre!.ParentGenreId == parentId)
                .Select(g => g.Game)
                .OfType<Game>()
                .ToList();

            // Act
            var actualGames = await _genreRepository!.GetGamesByParentId(parentId);

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
        public async Task UpdateAsync_ShouldUpdateGenre()
        {
            // Arrange
            _context!.Genres.RemoveRange(_context.Genres.Where(g => g.Id == "1" || g.Id == "2"));
            await _context.SaveChangesAsync();

            var existingGenre = new Genre { Id = "1", Name = "TestGenre", ParentGenreId = null };
            _context!.Genres.Add(existingGenre);
            await _context.SaveChangesAsync();

            var updatedGenre = new Genre { Id = existingGenre.Id, Name = "UpdatedTestGenre", ParentGenreId = "2" };

            // Act
            var actualUpdatedGenre = await _genreRepository!.UpdateAsync(updatedGenre);

            // Assert
            Assert.IsNotNull(actualUpdatedGenre);
            Assert.AreEqual(updatedGenre.Id, actualUpdatedGenre?.Id);
            Assert.AreEqual(updatedGenre.Name, actualUpdatedGenre?.Name);
            Assert.AreEqual(updatedGenre.ParentGenreId, actualUpdatedGenre?.ParentGenreId);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowArgumentException_WhenGenreNotFound()
        {
            // Arrange
            var nonExistingGenre = new Genre { Id = "999", Name = "NonExistingGenre", ParentGenreId = null };

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _genreRepository!.UpdateAsync(nonExistingGenre));
        }

        [TestMethod]
        public async Task CreateAsync_ShouldCreateNewGenre()
        {
            // Arrange
            _context!.Genres.RemoveRange(_context.Genres.Where(g => g.Id == "1" || g.Id == "2"));
            await _context.SaveChangesAsync();

            var parentGenre = new Genre { Id = "1", Name = "ParentGenre", ParentGenreId = null };
            _context.Genres.Add(parentGenre);
            await _context.SaveChangesAsync();

            var newGenre = new Genre { Id = "2", Name = "NewGenre", ParentGenreId = parentGenre.Id };

            // Act
            var createdGenre = await _genreRepository!.CreateAsync(newGenre);

            // Assert
            Assert.IsNotNull(createdGenre);
            Assert.AreEqual(newGenre.Id, createdGenre.Id);
            Assert.AreEqual(newGenre.Name, createdGenre.Name);
            Assert.AreEqual(newGenre.ParentGenreId, createdGenre.ParentGenreId);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldThrowArgumentException_WhenParentGenreIdNotFound()
        {
            // Arrange
            var nonExistingParentId = "999";
            var newGenre = new Genre { Id = "2", Name = "NewGenre", ParentGenreId = nonExistingParentId };

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _genreRepository!.CreateAsync(newGenre));
        }
    }
}
