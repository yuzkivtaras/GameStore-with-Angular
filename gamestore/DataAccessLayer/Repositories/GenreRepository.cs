using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly GameStoreDbContext _dbContext;

        public GenreRepository(GameStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Genre> GetAll()
        {
            return _dbContext.Genres;
        }

        public async Task<List<Genre>> GetByIdsAsync(List<string> ids)
        {
            return await _dbContext.Genres.Where(g => ids.Contains(g.Id)).ToListAsync();
        }

        public async Task<Genre> CreateAsync(Genre genre)
        {
            if (genre.ParentGenreId != null)
            {
                var parentGenre = await _dbContext.Genres.FindAsync(genre.ParentGenreId);
                if (parentGenre == null)
                {
                    throw new ArgumentException("The provided ParentGenreId does not exist");
                }
            }

            var createdGenre = _dbContext.Genres.Add(genre);
            await _dbContext.SaveChangesAsync();

            return createdGenre.Entity;
        }

        public async Task<Genre?> DeleteGenreByIdAsync(string id)
        {
            var genre = await _dbContext.Genres.FirstOrDefaultAsync(p => p.Id == id);

            if (genre == null) return null;

            _dbContext.Genres.Remove(genre);
            await _dbContext.SaveChangesAsync();

            return genre;
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _dbContext.Genres.ToListAsync();
        }

        public async Task<Genre?> GetGenreDetailsByIdAsync(string id)
        {
            return await _dbContext.Genres.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Game>> GetGamesByGenreId(string genreId)
        {
            return await _dbContext.Games.Include(g => g.GameGenres)
                .Where(g => g.GameGenres.Any(gp => gp.GenresId == genreId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetGamesByParentId(string parentId)
        {
            return await _dbContext.GameGenres
                .Include(x => x.Game)
                .Where(g => g.Genre!.ParentGenreId == parentId)
                .Select(g => g.Game)
                .OfType<Game>()
                .ToListAsync();
        }

        public async Task<Genre?> UpdateAsync(Genre entity)
        {
            var existingEntity = await _dbContext.Genres.SingleOrDefaultAsync(g => g.Id == entity.Id);

            if (existingEntity == null)
            {
                throw new ArgumentException("Genre not found.");
            }

            existingEntity.Name = entity.Name;

            if (!string.IsNullOrEmpty(entity.ParentGenreId))
            {
                existingEntity.ParentGenreId = entity.ParentGenreId;
            }
            else
            {
                existingEntity.ParentGenreId = null;
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
