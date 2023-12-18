using ServiceLayer.Models.Game;

namespace ServiceLayer.Interfaces
{
    public interface IGameService : ICrud<GameModel>
    {
        Task<GameModel?> GetModelModelDescriptionAsync(string key);
        Task<GameModel?> GetModelModelDescriptionByIdAsync(string id);
        Task<GameResponseForDownloadDto> DownloadGameAsync(GameDownloadDto gameDownloadDto);
        //Task<int> GetGamesCountAsync();
        Task<GameResponseDto> CreateGameAsync(GameCreateDto gameDto);
        Task<GameUpdateDto> UpdateGameAsync(GameUpdateDto gameUpdateDto);
        Task<bool> DeleteGameAsync(string id);

        Task<IEnumerable<GetPlatformNameByGameDto>> GetPlatformsNameByGame(string gameId);
    }
}
