using DataAccessLayer.Entities;
using ServiceLayer.Models.Genre;

namespace ServiceLayer.Interfaces
{
    public interface IGenreService : ICrud<GenreModel>
    {
        Task<GenreModel?> DeleteGenreModelAsync(string id);
        Task<GenreModel?> GetGenreModelDescriptionAsync(string id);
        Task<GenreResponseDto> CreateGenreAsync(GenreCreateDto genreDto);
        Task<GenreResponseForUpdateDto> UpdateGenreAsync(GenreUpdateDto genreUpdateDto);

        Task<List<Genre>> GetGenresByIdsAsync(List<string> ids);

        Task<IEnumerable<GetGameNameByGenreDto>> GetGamesNameByGenreId(string genreId);
        Task<IEnumerable<GetGameNameByGenreParentDto>> GetGamesNameByParentId(string parentId);
    }
}
