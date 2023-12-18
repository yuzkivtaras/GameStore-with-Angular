using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using ServiceLayer;

namespace StoreAPI.Tests
{
    public static class UnitTestHelper
    {
        public static DbContextOptions<GameStoreDbContext> GetUnitTestDbOptions()
        {
            var options = new DbContextOptionsBuilder<GameStoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new GameStoreDbContext(options))
            {
                SeedData(context);
            }

            using (var context = new GameStoreDbContext(options))
            {
                context.Database.EnsureDeleted();
                SeedData(context);
            }

            return options;
        }

        public static IMapper CreateMapperProfile()
        {
            var myProfile = new AutomapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));

            return new Mapper(configuration);
        }

        public static void SeedData(GameStoreDbContext context)
        {
            context.Games.AddRange(
                new Game { Id = "1", Name = "Name1", Key = "name1", Description = "Desc1", UnitInStock = 1, Discontinued = 10, PublisherId = "1" },
                new Game { Id = "2", Name = "Name2", Key = "name2", Description = "Desc2", UnitInStock = 2, Discontinued = 20, PublisherId = "2" });

            context.Genres.AddRange(
                new Genre { Id = "1", Name = "Name1" },
                new Genre { Id = "2", Name = "Name2" });

            context.Platforms.AddRange(
                new Platform { Id = "1", Type = "Type1"},
                new Platform { Id = "2", Type = "Type2" });

            context.Publishers.AddRange(
                new Publisher { Id = "1", CompanyName = "Company1", Description = "Description1", HomePage = "www.company1.com"},
                new Publisher { Id = "2", CompanyName = "Company2", Description = "Description2", HomePage = "www.company2.com" });

            context.SaveChanges();
        }
    }
}
