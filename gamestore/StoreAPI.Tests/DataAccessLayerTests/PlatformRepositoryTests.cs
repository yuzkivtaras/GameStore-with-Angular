using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;

namespace StoreAPI.Tests.DataAccessLayerTests
{
    [TestClass]
    public class PlatformRepositoryTests
    {
        private GameStoreDbContext? _context;
        private PlatformRepository? _platformRepository;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _context = new GameStoreDbContext(UnitTestHelper.GetUnitTestDbOptions());
            _platformRepository = new PlatformRepository(_context);
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllPlatforms()
        {
            // Arrange
            var expectedPlatforms = _context?.Platforms.ToList();

            // Act
            var actualPlatforms = _platformRepository?.GetAll().ToList();

            // Assert
            Assert.AreEqual(expectedPlatforms?.Count, actualPlatforms?.Count);
            for (int i = 0; i < expectedPlatforms?.Count; i++)
            {
                Assert.AreEqual(expectedPlatforms[i].Id, actualPlatforms?[i].Id);
                Assert.AreEqual(expectedPlatforms[i].Type, actualPlatforms?[i].Type);
            }
        }

        [TestMethod]
        public async Task GetByIdsAsync_ShouldReturnPlatformsByIds()
        {
            // Arrange
            var platformIds = new List<string> { "1", "2" };
            var expectedPlatforms = _context?.Platforms.Where(p => platformIds.Contains(p.Id)).ToList();

            // Act
            var actualPlatforms = await _platformRepository!.GetByIdsAsync(platformIds);

            // Assert
            Assert.AreEqual(expectedPlatforms!.Count, actualPlatforms.Count);

            for (int i = 0; i < expectedPlatforms.Count; i++)
            {
                Assert.IsNotNull(actualPlatforms[i]);
                Assert.AreEqual(expectedPlatforms[i].Id, actualPlatforms[i].Id);
                Assert.AreEqual(expectedPlatforms[i].Type, actualPlatforms[i].Type);
            }
        }

        [TestMethod]
        public async Task CreateAsync_ShouldCreateNewPlatform()
        {
            // Arrange
            var newPlatform = new Platform { Id = "3", Type = "Type3" };

            // Act
            var createdPlatform = await _platformRepository!.CreateAsync(newPlatform);

            // Assert
            Assert.IsNotNull(createdPlatform);
            Assert.AreEqual(newPlatform.Id, createdPlatform.Id);
            Assert.AreEqual(newPlatform.Type, createdPlatform.Type);

            var dbPlatform = _context!.Platforms.FirstOrDefault(p => p.Id == newPlatform.Id);
            Assert.IsNotNull(dbPlatform);
            Assert.AreEqual(newPlatform.Id, dbPlatform.Id);
            Assert.AreEqual(newPlatform.Type, dbPlatform.Type);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllPlatforms()
        {
            // Arrange
            var expectedPlatforms = _context!.Platforms.ToList();

            // Act
            var actualPlatforms = await _platformRepository!.GetAllAsync();

            // Assert
            Assert.AreEqual(expectedPlatforms.Count, actualPlatforms.Count());

            for (int i = 0; i < expectedPlatforms.Count; i++)
            {
                Assert.AreEqual(expectedPlatforms[i].Id, actualPlatforms.ElementAt(i).Id);
                Assert.AreEqual(expectedPlatforms[i].Type, actualPlatforms.ElementAt(i).Type);
            }
        }

        [TestMethod]
        public async Task GetPlatformDetailsByIdAsync_ShouldReturnPlatformById()
        {
            // Arrange
            var platformId = "1";
            var expectedPlatform = _context!.Platforms.FirstOrDefault(p => p.Id == platformId);

            // Act
            var actualPlatform = await _platformRepository!.GetPlatformDetailsByIdAsync(platformId);

            // Assert
            Assert.IsNotNull(actualPlatform);
            Assert.AreEqual(expectedPlatform!.Id, actualPlatform.Id);
            Assert.AreEqual(expectedPlatform.Type, actualPlatform.Type);
        }

        [TestMethod]
        public async Task GetPlatformDetailsByIdAsync_ShouldReturnNull_ForNonExistingPlatform()
        {
            // Arrange
            var platformId = "NonExistingId";

            // Act
            var actualPlatform = await _platformRepository!.GetPlatformDetailsByIdAsync(platformId);

            // Assert
            Assert.IsNull(actualPlatform);
        }

        [TestMethod]
        public async Task FindPlatformAsync_ShouldReturnPlatformById()
        {
            // Arrange
            var platformId = "1";
            var expectedPlatform = _context!.Platforms.FirstOrDefault(p => p.Id == platformId);

            // Act
            var actualPlatform = await _platformRepository!.FindPlatformAsync(platformId);

            // Assert
            Assert.IsNotNull(actualPlatform);
            Assert.AreEqual(expectedPlatform!.Id, actualPlatform.Id);
            Assert.AreEqual(expectedPlatform.Type, actualPlatform.Type);
        }

        [TestMethod]
        public async Task FindPlatformAsync_ShouldReturnNull_ForNonExistingPlatform()
        {
            // Arrange
            var platformId = "NonExistingId";

            // Act
            var actualPlatform = await _platformRepository!.FindPlatformAsync(platformId);

            // Assert
            Assert.IsNull(actualPlatform);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdatePlatform()
        {
            // Arrange
            var updatedPlatformData = new Platform { Id = "1", Type = "UpdatedType1" };
            var originalPlatform = await _context!.Platforms.FindAsync(updatedPlatformData.Id);

            var originalPlatformType = originalPlatform!.Type;

            originalPlatform.Type = updatedPlatformData.Type;

            // Act
            await _platformRepository!.UpdateAsync(originalPlatform);
            await _context.SaveChangesAsync();

            var retrievedPlatform = await _context.Platforms.FindAsync(originalPlatform.Id);

            // Assert
            Assert.IsNotNull(retrievedPlatform);
            Assert.AreEqual(updatedPlatformData.Id, retrievedPlatform.Id);
            Assert.AreEqual(updatedPlatformData.Type, retrievedPlatform.Type);
            Assert.AreNotEqual(originalPlatformType, retrievedPlatform.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateAsync_ShouldThrowException_ForNonExistingPlatform()
        {
            // Arrange
            var nonExistingPlatformId = "NonExistingId";
            var newType = "UpdatedType";
            var nonExistingPlatform = new Platform { Id = nonExistingPlatformId, Type = newType };

            // Act
            await _platformRepository!.UpdateAsync(nonExistingPlatform);
        }

        [TestMethod]
        public async Task DeletePlatformByIdAsync_ShouldDeletePlatform()
        {
            // Arrange
            var platformToAdd = new Platform { Id = "3", Type = "NewType" };
            _context!.Platforms.Add(platformToAdd);
            await _context.SaveChangesAsync();

            // Act
            var deletedPlatform = await _platformRepository!.DeletePlatformByIdAsync(platformToAdd.Id);

            // Assert
            Assert.IsNotNull(deletedPlatform);
            Assert.AreEqual(platformToAdd.Id, deletedPlatform.Id);
            Assert.AreEqual(platformToAdd.Type, deletedPlatform.Type);

            var retrievedPlatformAfterDeletion = await _context.Platforms.FindAsync(platformToAdd.Id);
            Assert.IsNull(retrievedPlatformAfterDeletion);
        }

        [TestMethod]
        public async Task GetGamesByPlatformId_ShouldReturnGames()
        {
            // Arrange
            var platform1 = new Platform { Id = "10", Type = "Type1" };
            _context.Platforms.Add(platform1);

            var platform2 = new Platform { Id = "11", Type = "Type2" };
            _context.Platforms.Add(platform2);

            var game1 = new Game { Key = "1", Name = "Game1" };
            var game2 = new Game { Key = "2", Name = "Game2" };
            var game3 = new Game { Key = "3", Name = "Game3" };
            _context.Games.AddRange(game1, game2, game3);

            _context.GamePlatforms.AddRange(
                new GamePlatform { GamesKey = game1.Key, PlatformsId = platform1.Id },
                new GamePlatform { GamesKey = game1.Key, PlatformsId = platform2.Id },
                new GamePlatform { GamesKey = game2.Key, PlatformsId = platform1.Id },
                new GamePlatform { GamesKey = game3.Key, PlatformsId = platform2.Id });

            await _context.SaveChangesAsync();

            // Act
            var gamesForPlatform1 = await _platformRepository!.GetGamesByPlatformId(platform1.Id);

            // Assert
            Assert.IsNotNull(gamesForPlatform1);
            Assert.AreEqual(2, gamesForPlatform1.Count());
            Assert.IsTrue(gamesForPlatform1.Any(g => g.Name == game1.Name));
            Assert.IsTrue(gamesForPlatform1.Any(g => g.Name == game2.Name));
        }
    }
}
