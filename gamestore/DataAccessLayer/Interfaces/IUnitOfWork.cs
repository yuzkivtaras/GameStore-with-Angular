
namespace DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IGameRepository GameRepository { get; }
        IGenreRepository GenreRepository { get; }
        IPlatformRepository PlatformRepository { get; } 
        IPublisherRepository PublisherRepository { get; }
        IOrderRepository OrderRepository { get; }

        Task SaveAsync();
    }
}
