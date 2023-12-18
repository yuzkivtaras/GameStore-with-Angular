using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IPublisherRepository : IRepository<Publisher>
    {
        Task<Publisher?> GetPublisherDetailsByCompanyNameAsync(string companyname);
        Task<Publisher?> DeletePublisherByIdAsync(string id);
        Task<Publisher?> CreateAsync(Publisher publisher);
        Task<Publisher?> UpdateAsync(Publisher entity);

        Task<Publisher?> GetByIdAsync(string id);
        Task<IEnumerable<Game>> GetGamesByPublisherCompanyName(string companyname);
    }
}
