using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IGenreRepository : IRepository<Genre>
    {
        Task<Genre> CreateAsync(Genre genre);
        Task<Genre?> DeleteGenreByIdAsync(string id);
        Task<Genre?> GetGenreDetailsByIdAsync(string id);
        Task<Genre?> UpdateAsync(Genre entity);

        Task<List<Genre>> GetByIdsAsync(List<string> ids);
        IQueryable<Genre> GetAll();

        Task<IEnumerable<Game>> GetGamesByGenreId(string genreId);
        Task<IEnumerable<Game>> GetGamesByParentId(string parentId);
    }
}
