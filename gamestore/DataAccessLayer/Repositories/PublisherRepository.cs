using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly GameStoreDbContext _dbContext;

        public PublisherRepository(GameStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Publisher?> GetByIdAsync(string id)
        {
            return await _dbContext.Publishers.FindAsync(id);
        }

        public async Task<Publisher?> CreateAsync(Publisher publisher)
        {
            _dbContext.Publishers.Add(publisher);
            await _dbContext.SaveChangesAsync();
            return publisher;
        }

        public async Task<Publisher?> DeletePublisherByIdAsync(string id)
        {
            Publisher? publisher = await _dbContext.Publishers.Include(p => p.Games).FirstOrDefaultAsync(p => p.Id == id);
            if (publisher == null)
            {
                return null;
            }

            if (publisher.Games != null)
            {
                publisher.Games.ToList().ForEach(game =>
                {
                    game.PublisherId = null;
                    game.Publisher = null;
                });

                await _dbContext.SaveChangesAsync();
            }

            _dbContext.Publishers.Remove(publisher);
            await _dbContext.SaveChangesAsync();

            return publisher;
        }

        public async Task<IEnumerable<Publisher>> GetAllAsync()
        {
            return await _dbContext.Publishers.ToListAsync();
        }

        public async Task<Publisher?> GetPublisherDetailsByCompanyNameAsync(string companyname)
        {
            var publisher = await _dbContext.Publishers
                .FirstOrDefaultAsync(p => p.CompanyName != null && p.CompanyName == companyname);

            return publisher;
        }

        public async Task<IEnumerable<Game>> GetGamesByPublisherCompanyName(string companyname)
        {
            return await _dbContext.Games
                .Include(g => g.Publisher)
                .Where(g => g.Publisher!.CompanyName == companyname)
                .ToListAsync();
        }

        public async Task<Publisher?> UpdateAsync(Publisher entity)
        {
            var existingEntity = await _dbContext.Publishers.SingleOrDefaultAsync(e => e.Id == entity.Id);

            if (existingEntity == null)
            {
                throw new ArgumentException("Publisher not found.");
            }

            existingEntity.CompanyName = entity.CompanyName;
            existingEntity.Description = entity.Description;
            existingEntity.HomePage = entity.HomePage;

            try
            {
                await _dbContext.SaveChangesAsync();
                return existingEntity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
    }
}
