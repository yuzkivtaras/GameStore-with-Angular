using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly GameStoreDbContext _dbContext;

        public PlatformRepository(GameStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Platform> GetAll()
        {
            return _dbContext.Platforms;
        }

        public async Task<List<Platform>> GetByIdsAsync(List<string> ids)
        {
            return await _dbContext.Platforms.Where(p => ids.Contains(p.Id)).ToListAsync();
        }

        public async Task<Platform?> CreateAsync(Platform platform)
        {
            _dbContext.Platforms.Add(platform);
            await _dbContext.SaveChangesAsync();
            return platform;
        }

        public async Task<IEnumerable<Platform>> GetAllAsync()
        {
            return await _dbContext.Platforms.ToListAsync();
        }

        public async Task<Platform?> GetPlatformDetailsByIdAsync(string id)
        {
            return await _dbContext.Platforms.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Platform?> FindPlatformAsync(string id)
        {
            return await _dbContext.Platforms.FindAsync(id);
        }

        public async Task<Platform> UpdateAsync(Platform entity)
        {
            var existingEntity = await _dbContext.Platforms.SingleOrDefaultAsync(e => e.Id == entity.Id);

            if (existingEntity == null)
            {
                throw new ArgumentException("Platform not found.");
            }

            existingEntity.Type = entity.Type;

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

        public async Task<Platform?> DeletePlatformByIdAsync(string id)
        {
            Platform? platform = await _dbContext.Platforms.FindAsync(id);
            if (platform == null)
            {
                return null;
            }

            _dbContext.Platforms.Remove(platform);
            await _dbContext.SaveChangesAsync();

            return platform;
        }

        public async Task<IEnumerable<Game>> GetGamesByPlatformId(string platformId)
        {
            return await _dbContext.Games.Include(g => g.GamePlatforms)
                .Where(g => g.GamePlatforms.Any(gp => gp.PlatformsId == platformId))
                .ToListAsync();
        }
    }
}
