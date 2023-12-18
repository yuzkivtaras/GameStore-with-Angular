using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace StoreAPI.Tests.DataAccessLayerTests
{
    [TestClass]
    public class PublisherRepositoryTests
    {
        private GameStoreDbContext? _context;
        private PublisherRepository? _publisherRepository;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _context = new GameStoreDbContext(UnitTestHelper.GetUnitTestDbOptions());
            _publisherRepository = new PublisherRepository(_context);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnPublisherById()
        {
            // Arrange
            var publisherId = "1";
            var expectedPublisher = _context!.Publishers.Find(publisherId);

            // Act
            var actualPublisher = await _publisherRepository!.GetByIdAsync(publisherId);

            // Assert
            Assert.IsNotNull(actualPublisher);
            Assert.AreEqual(expectedPublisher!.Id, actualPublisher?.Id);
            Assert.AreEqual(expectedPublisher.CompanyName, actualPublisher?.CompanyName);
            Assert.AreEqual(expectedPublisher.Description, actualPublisher?.Description);
            Assert.AreEqual(expectedPublisher.HomePage, actualPublisher?.HomePage);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldCreateNewPublisher()
        {
            // Arrange
            var newPublisher = new Publisher { Id = "3", CompanyName = "Company3", Description = "Description3", HomePage = "www.company3.com" };

            // Act
            var createdPublisher = await _publisherRepository!.CreateAsync(newPublisher);

            // Assert
            Assert.IsNotNull(createdPublisher);
            Assert.AreEqual(newPublisher.Id, createdPublisher.Id);
            Assert.AreEqual(newPublisher.CompanyName, createdPublisher.CompanyName);
            Assert.AreEqual(newPublisher.Description, createdPublisher.Description);
            Assert.AreEqual(newPublisher.HomePage, createdPublisher.HomePage);

            var dbPublisher = _context!.Publishers.FirstOrDefault(p => p.Id == newPublisher.Id);
            Assert.IsNotNull(dbPublisher);
            Assert.AreEqual(newPublisher.Id, dbPublisher.Id);
            Assert.AreEqual(newPublisher.CompanyName, dbPublisher.CompanyName);
            Assert.AreEqual(newPublisher.Description, dbPublisher.Description);
            Assert.AreEqual(newPublisher.HomePage, dbPublisher.HomePage);
        }

        [TestMethod]
        public async Task DeletePublisherByIdAsync_ShouldDeletePublisherAndUpdateGames()
        {
            // Arrange
            var publisherToDelete = new Publisher { Id = "4", CompanyName = "Company4", Description = "Description4", HomePage = "www.company4.com" };
            _context!.Publishers.Add(publisherToDelete);
            await _context.SaveChangesAsync();

            var gameWithPublisher = new Game { Id = "4", Name = "GameWithPublisher", Key = "gameKey4", Description = "Game with publisher to delete", PublisherId = publisherToDelete.Id, Publisher = publisherToDelete };
            _context.Games.Add(gameWithPublisher);
            await _context.SaveChangesAsync();

            // Act
            var deletedPublisher = await _publisherRepository!.DeletePublisherByIdAsync(publisherToDelete.Id);

            // Assert
            Assert.IsNotNull(deletedPublisher);
            Assert.AreEqual(publisherToDelete.Id, deletedPublisher?.Id);

            var dbPublisher = _context.Publishers.FirstOrDefault(p => p.Id == publisherToDelete.Id);
            Assert.IsNull(dbPublisher);

            var dbGame = _context.Games.FirstOrDefault(g => g.Id == gameWithPublisher.Id);
            Assert.IsNotNull(dbGame);
            Assert.IsNull(dbGame.PublisherId);
            Assert.IsNull(dbGame.Publisher);
        }

        [TestMethod]
        public async Task DeletePublisherByIdAsync_ShouldReturnNull_WhenPublisherNotFound()
        {
            // Arrange
            var nonExistingPublisherId = "100";

            // Act
            var result = await _publisherRepository!.DeletePublisherByIdAsync(nonExistingPublisherId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllPublishers()
        {
            // Arrange
            var expectedPublishers = _context!.Publishers.ToList();

            // Act
            var actualPublishers = await _publisherRepository!.GetAllAsync();

            // Assert
            Assert.AreEqual(expectedPublishers.Count, actualPublishers.Count());
            var actualPublishersList = actualPublishers.ToList();

            for (int i = 0; i < expectedPublishers.Count; i++)
            {
                Assert.AreEqual(expectedPublishers[i].Id, actualPublishersList[i].Id);
                Assert.AreEqual(expectedPublishers[i].CompanyName, actualPublishersList[i].CompanyName);
                Assert.AreEqual(expectedPublishers[i].Description, actualPublishersList[i].Description);
                Assert.AreEqual(expectedPublishers[i].HomePage, actualPublishersList[i].HomePage);
            }
        }

        [TestMethod]
        public async Task GetPublisherDetailsByCompanyNameAsync_ShouldReturnPublisherDetailsForCompanyName()
        {
            // Arrange
            var companyName = "Company1";
            var expectedPublisher = _context!.Publishers.FirstOrDefault(p => p.CompanyName == companyName);

            // Act
            var actualPublisher = await _publisherRepository!.GetPublisherDetailsByCompanyNameAsync(companyName);

            // Assert
            Assert.IsNotNull(actualPublisher);
            Assert.AreEqual(expectedPublisher!.Id, actualPublisher?.Id);
            Assert.AreEqual(expectedPublisher.CompanyName, actualPublisher?.CompanyName);
            Assert.AreEqual(expectedPublisher.Description, actualPublisher?.Description);
            Assert.AreEqual(expectedPublisher.HomePage, actualPublisher?.HomePage);
        }

        [TestMethod]
        public async Task GetPublisherDetailsByCompanyNameAsync_ShouldReturnNull_WhenCompanyNameNotFound()
        {
            // Arrange
            var nonExistingCompanyName = "NonExistingCompany";

            // Act
            var result = await _publisherRepository!.GetPublisherDetailsByCompanyNameAsync(nonExistingCompanyName);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetGamesByPublisherDetailsByCompanyName_ShouldReturnGamesForPublisher()
        {
            // Arrange
            var companyName = "Company1";
            var expectedGames = _context!.Games.Include(g => g.Publisher).Where(g => g.Publisher!.CompanyName == companyName).ToList();

            // Act
            var actualGames = await _publisherRepository!.GetGamesByPublisherCompanyName(companyName);

            // Assert
            Assert.AreEqual(expectedGames.Count, actualGames.Count());
            var actualGamesList = actualGames.ToList();

            for (int i = 0; i < expectedGames.Count; i++)
            {
                Assert.AreEqual(expectedGames[i].Id, actualGamesList[i].Id);
                Assert.AreEqual(expectedGames[i].Name, actualGamesList[i].Name);
                Assert.AreEqual(expectedGames[i].Description, actualGamesList[i].Description);
            }
        }

        [TestMethod]
        public async Task GetGamesByPublisherDetailsByCompanyName_ShouldReturnEmpty_WhenCompanyNameNotFound()
        {
            // Arrange
            var nonExistingCompanyName = "NonExistingCompany";

            // Act
            var result = await _publisherRepository!.GetGamesByPublisherCompanyName(nonExistingCompanyName);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetGamesByPublisherCompanyName_ShouldReturnGamesForPublisher()
        {
            // Arrange
            var companyName = "Company1";
            var expectedGames = _context!.Games.Include(g => g.Publisher).Where(g => g.Publisher!.CompanyName == companyName).ToList();

            // Act
            var actualGames = await _publisherRepository!.GetGamesByPublisherCompanyName(companyName);

            // Assert
            Assert.AreEqual(expectedGames.Count, actualGames.Count());
            var actualGamesList = actualGames.ToList();

            for (int i = 0; i < expectedGames.Count; i++)
            {
                Assert.AreEqual(expectedGames[i].Id, actualGamesList[i].Id);
                Assert.AreEqual(expectedGames[i].Name, actualGamesList[i].Name);
                Assert.AreEqual(expectedGames[i].Description, actualGamesList[i].Description);
            }
        }

        [TestMethod]
        public async Task GetGamesByPublisherCompanyName_ShouldReturnEmpty_WhenCompanyNameNotFound()
        {
            // Arrange
            var nonExistingCompanyName = "NonExistingCompany";

            // Act
            var result = await _publisherRepository!.GetGamesByPublisherCompanyName(nonExistingCompanyName);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdatePublisher()
        {
            // Arrange
            var existingPublisher = new Publisher { Id = "5", CompanyName = "Company5", Description = "Description5", HomePage = "www.company5.com" };
            _context!.Publishers.Add(existingPublisher);
            await _context.SaveChangesAsync();

            var updatedPublisher = new Publisher { Id = existingPublisher.Id, CompanyName = "Updated Company5", Description = "Updated Description5", HomePage = "www.updatedcompany5.com" };

            // Act
            var actualUpdatedPublisher = await _publisherRepository!.UpdateAsync(updatedPublisher);

            // Assert
            Assert.IsNotNull(actualUpdatedPublisher);
            Assert.AreEqual(updatedPublisher.Id, actualUpdatedPublisher?.Id);
            Assert.AreEqual(updatedPublisher.CompanyName, actualUpdatedPublisher?.CompanyName);
            Assert.AreEqual(updatedPublisher.Description, actualUpdatedPublisher?.Description);
            Assert.AreEqual(updatedPublisher.HomePage, actualUpdatedPublisher?.HomePage);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowException_WhenPublisherNotFound()
        {
            // Arrange
            var nonExistingPublisher = new Publisher { Id = "100", CompanyName = "NonExistingCompany", Description = "NonExistingDescription", HomePage = "www.nonexistingcompany.com" };

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _publisherRepository!.UpdateAsync(nonExistingPublisher));
        }
    }
}
