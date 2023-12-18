using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameStoreDbContext _dbContext;

        public GameRepository(GameStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Platform> GetAll()
        {
            return _dbContext.Platforms;
        }

        public async Task<Game?> DeleteGameByIdAsync(string key)
        {
            var game = await _dbContext.Games.FirstOrDefaultAsync(p => p.Key == key);

            if (game == null) return null;

            _dbContext.Games.Remove(game);
            await _dbContext.SaveChangesAsync();

            return game;
        }

        public async Task<Game> CreateAsync(Game game)
        {
            _dbContext.Games.Add(game);
            await _dbContext.SaveChangesAsync();
            return game;
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _dbContext.Games.ToListAsync();
        }

        public async Task<Game?> GetGameDetailsByKeyAsync(string key)
        {
            return await _dbContext.Games
                .Include(g => g.GameGenres)
                .ThenInclude(o => o.Genre)
                .Include(g => g.GamePlatforms)
                .FirstOrDefaultAsync(p => p.Key == key);
        }

        public async Task<Game?> GetGameDetailsByIdAsync(string id)
        {
            return await _dbContext.Games
               .Include(g => g.Publisher)
               .Include(g => g.GameGenres)
                   .ThenInclude(gg => gg.Genre)
               .Include(g => g.GamePlatforms)
                   .ThenInclude(gp => gp.Platform)
               .SingleOrDefaultAsync(p => p.Id == id);

        }

        public async Task<IEnumerable<Platform?>> GetPlatformsByGame(string gameId)
        {
            return await _dbContext.GamePlatforms
                .Include(gp => gp.Platform)
                .Where(gp => gp.GamesKey == gameId)
                .Select(gp => gp.Platform)
                .ToListAsync();
        }

        public async Task<Game?> UpdateAsync(Game game)
        {
            var existingEntity = await _dbContext.Games.SingleOrDefaultAsync(g => g.Id == game.Id);
            if (existingEntity == null)
            {
                throw new ArgumentException("Game not found.");
            }

            existingEntity.Name = game.Name;
            existingEntity.Description = game.Description;
            existingEntity.UnitInStock = game.UnitInStock;
            existingEntity.Price = game.Price;
            existingEntity.Discontinued = game.Discontinued;

            if (existingEntity.PublisherId != null)
            {
                existingEntity.PublisherId = game.PublisherId;
            }

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
