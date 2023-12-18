using DataAccessLayer.Entities;
using ServiceLayer.Models;
using ServiceLayer.Models.Platform;

namespace ServiceLayer.Interfaces
{
    public interface IPlatformService: ICrud<PlatformModel>
    {
        Task<PlatformModel?> GetPlatformModelDescriptionAsync(string id);
        Task<bool> DeletePlatformAsync(string id);
        Task<PlatformResponseDto> CreatePlatformAsync(PlatformCreateDto platformDto);
        Task<PlatformResponseForUpdateDto?> UpdatePlatformAsync(PlatformUpdateDto platformDto);

        Task<IEnumerable<Platform>> GetPlatformsByIdsAsync(IEnumerable<string> type);
 
        Task<IEnumerable<GetGameNameByPlatformDto>> GetGamesNameByPlatformId(string platformId);
    }
}
