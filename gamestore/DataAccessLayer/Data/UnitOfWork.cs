using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GameStoreDbContext _dbContext;

        public UnitOfWork(GameStoreDbContext dbContext)
        {
            _dbContext = dbContext;
            GameRepository = new GameRepository(_dbContext);
            GenreRepository = new GenreRepository(_dbContext);
            PlatformRepository = new PlatformRepository(_dbContext);
            PublisherRepository = new PublisherRepository(_dbContext);
            OrderRepository = new OrderRepository(_dbContext);
        }

        public IGameRepository GameRepository { get;}
        public IGenreRepository GenreRepository { get;}
        public IPlatformRepository PlatformRepository { get;}
        public IPublisherRepository PublisherRepository { get;}
        public IOrderRepository OrderRepository { get; }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
