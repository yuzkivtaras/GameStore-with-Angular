using AutoMapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using ServiceLayer.Interfaces;
using ServiceLayer.Models.Genre;

namespace ServiceLayer.Services
{
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GenreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GenreResponseDto> CreateGenreAsync(GenreCreateDto genreDto)
        {
            var genre = new Genre();

            if (!string.IsNullOrEmpty(genreDto.Genre?.Name))
            {
                genre.Name = genreDto.Genre.Name;
            }

            if (!string.IsNullOrEmpty(genreDto.Genre?.ParentGenreId))
            {
                genre.ParentGenreId = genreDto.Genre.ParentGenreId;
            }

            var createdGenre = await _unitOfWork.GenreRepository.CreateAsync(genre);

            var result = new GenreResponseDto
            {
                Name = createdGenre.Name,
                ParentGenreId = createdGenre.ParentGenreId
            };

            return result;
        }

        public async Task<GenreModel?> DeleteGenreModelAsync(string id)
        {
            var deletedGenre = await _unitOfWork.GenreRepository.DeleteGenreByIdAsync(id);

            if (deletedGenre == null) return null;

            await _unitOfWork.SaveAsync();

            return _mapper.Map<GenreModel>(deletedGenre);
        }

        public async Task<IEnumerable<GenreModel>> GetAllModelsAsync()
        {
            var genres = await _unitOfWork.GenreRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<GenreModel>>(genres);
        }

        public async Task<GenreModel?> GetGenreModelDescriptionAsync(string id)
        {
            var genreEntity = await _unitOfWork.GenreRepository.GetGenreDetailsByIdAsync(id);

            return _mapper.Map<GenreModel>(genreEntity);
        }

        public async Task<List<Genre>> GetGenresByIdsAsync(List<string> ids)
        {
            return await _unitOfWork.GenreRepository.GetByIdsAsync(ids);
        }

        public async Task<IEnumerable<GetGameNameByGenreDto>> GetGamesNameByGenreId(string genreId)
        {
            var games = await _unitOfWork.GenreRepository.GetGamesByGenreId(genreId);

            var gameIdNameDtos = games.Select(g => new GetGameNameByGenreDto
            {
                Id = g.Id,
                Name = g.Name
            });

            return gameIdNameDtos;
        }

        public async Task<IEnumerable<GetGameNameByGenreParentDto>> GetGamesNameByParentId(string parentId)
        {
            var games = await _unitOfWork.GenreRepository.GetGamesByParentId(parentId);

            var gameIdNameDtos = games.Select(g => new GetGameNameByGenreParentDto
            {
                Id = g.Id,
                Name = g.Name
            });

            return gameIdNameDtos;
        }

        public async Task<GenreResponseForUpdateDto> UpdateGenreAsync(GenreUpdateDto genreUpdateDto)
        {
            if (genreUpdateDto.Genre == null || genreUpdateDto.Genre.Name == null)
            {
                throw new ArgumentNullException(nameof(genreUpdateDto.Genre), "Genre data and ID are required.");
            }

            string genreId = genreUpdateDto.Genre.Id ?? throw new ArgumentNullException(nameof(genreUpdateDto.Genre.Id), "Genre ID is required.");
            string genreName = genreUpdateDto.Genre.Name;
            string? parentGenreId = genreUpdateDto.Genre.ParentGenreId;

            var genre = new Genre
            {
                Id = genreId,
                Name = genreName,
                ParentGenreId = parentGenreId
            };

            Genre? updatedGenre = await _unitOfWork.GenreRepository.UpdateAsync(genre);

            return new GenreResponseForUpdateDto
            {
                Name = updatedGenre!.Name,
                ParentGenreId = updatedGenre.ParentGenreId
            };
        }
    }
}
