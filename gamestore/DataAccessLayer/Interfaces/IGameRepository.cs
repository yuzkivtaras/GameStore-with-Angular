using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IGameRepository : IRepository<Game>
    {
        Task<Game?> GetGameDetailsByKeyAsync(string key);
        Task<Game?> GetGameDetailsByIdAsync(string id);
        Task<Game> CreateAsync(Game game);
        Task<Game?> UpdateAsync(Game game);
        Task<Game?> DeleteGameByIdAsync(string id);

        IQueryable<Platform> GetAll();
        Task<IEnumerable<Platform?>> GetPlatformsByGame(string platformId);
    }
}
