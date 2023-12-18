using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IPlatformRepository : IRepository<Platform>
    {
        Task<Platform?> GetPlatformDetailsByIdAsync(string id);
        Task<Platform?> DeletePlatformByIdAsync(string id);
        Task<Platform?> CreateAsync(Platform platform);
        Task<Platform?> FindPlatformAsync(string id);
        Task<Platform> UpdateAsync(Platform entity);
        Task<List<Platform>> GetByIdsAsync(List<string> ids);

        IQueryable<Platform> GetAll();


        Task<IEnumerable<Game>> GetGamesByPlatformId(string platformId);
    }
}
