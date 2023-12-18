using AutoMapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServiceLayer.Interfaces;
using ServiceLayer.Models.Game;
using System.Text;

namespace ServiceLayer.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public GameService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = memoryCache;
        }

        public async Task<GameResponseDto> CreateGameAsync(GameCreateDto gameDto)
        {
            Publisher? publisher = await _unitOfWork.PublisherRepository.GetByIdAsync(gameDto.Publisher);

            var game = new Game
            {
                Name = gameDto.Game?.Name,
                Key = gameDto.Game?.Key,
                Description = gameDto.Game?.Description,
                UnitInStock = gameDto.Game?.UnitInStock,
                Price = gameDto.Game?.Price,
                Discontinued = gameDto.Game?.Discontinued,
                PublisherId = publisher?.Id,              
            };

            var allPlatforms = _unitOfWork.PlatformRepository.GetAll();
            var platformsByTypes = allPlatforms.Where(platform => gameDto.Platforms.Contains(platform.Id)).ToList();

            foreach (Platform platform in platformsByTypes)
            {
                game.GamePlatforms.Add(new GamePlatform
                {
                    GamesKey = game.Key,
                    PlatformsId = platform.Id,
                });
            }

            var allGenres = _unitOfWork.GenreRepository.GetAll();
            var genresByNames = allGenres.Where(genre => gameDto.Genres.Contains(genre.Id)).ToList();

            foreach (Genre genre in genresByNames)
            {
                genre.GameGenres.Add(new GameGenre
                {
                    GamesKey = game.Key,
                    GenresId = genre.Id,
                });
            }

            Game addedGame = await _unitOfWork.GameRepository.CreateAsync(game);
            await _unitOfWork.SaveAsync();

            return new GameResponseDto
            {
                Name = addedGame.Name,
                Key = addedGame.Key,
                Description = addedGame.Description,
                UnitInStock = addedGame.UnitInStock,
                Price = addedGame.Price,
                Discontinued = addedGame.Discontinued,
            };
        }

        public async Task<bool> DeleteGameAsync(string key)
        {
            Game? deletedGame = await _unitOfWork.GameRepository.DeleteGameByIdAsync(key);
            return deletedGame != null;
        }

        public async Task<GameResponseForDownloadDto> DownloadGameAsync(GameDownloadDto gameDownloadDto)
        {
            var gameKey = gameDownloadDto?.Game!.Key;

            if (string.IsNullOrEmpty(gameKey))
            {
                throw new ArgumentException("Game key is missing.");
            }

            var game = await _unitOfWork.GameRepository.GetGameDetailsByKeyAsync(gameKey);

            if (game == null)
            {
                throw new InvalidOperationException($"Game with key '{gameKey}' does not exist");
            }

            var fileName = $"{game.Name}_{DateTime.UtcNow:yyyy-MM-dd-HH:mm}.txt";
            var fileContent = Encoding.UTF8.GetBytes($"This is an auto generated file for a game: {game.Name}");

            return new GameResponseForDownloadDto
            {
                Name = game.Name,
                Key = game.Key,
                Description = game.Description,
                UnitInStock = game.UnitInStock,
                Price = game.Price,
                Discontinued = game.Discontinued,
                FileName = fileName,
                FileContent = fileContent
            };
        }

        public async Task<IEnumerable<GameModel>> GetAllModelsAsync()
        {
            var games = await _unitOfWork.GameRepository.GetAllAsync();

            var gameModels = games.Select(gameEntity => new GameModel()
            {
                Name = gameEntity.Name,
                Key = gameEntity.Key,
                Description = gameEntity.Description,
                UnitInStock = gameEntity.UnitInStock,
                Price = gameEntity.Price,
                Discontinued = gameEntity.Discontinued,

                Genres = gameEntity.GameGenres
                    .Where(gg => gg.Genre!.Name != null)
                    .Select(gg => gg.Genre!.Name!)
                    .ToList(),
                Platforms = gameEntity.GamePlatforms
                    .Where(gp => gp.Platform!.Type != null)
                    .Select(gp => gp.Platform!.Type!)
                    .ToList()
            });

            return gameModels;
        }

        public async Task<GameModel?> GetModelModelDescriptionAsync(string key)
        {
            var gameEntity = await _unitOfWork.GameRepository.GetGameDetailsByKeyAsync(key);

            if (gameEntity == null)
            {
                return null;
            }

            var gameModel = new GameModel()
            {
                Id = gameEntity.Id,
                Name = gameEntity.Name,
                Key = gameEntity.Key,
                Description = gameEntity.Description,
                UnitInStock = gameEntity.UnitInStock,
                Price = gameEntity.Price,
                Discontinued = gameEntity.Discontinued,
            };

            return gameModel;
        }

        public async Task<GameModel?> GetModelModelDescriptionByIdAsync(string id)
        {
            var gameEntity = await _unitOfWork.GameRepository.GetGameDetailsByIdAsync(id);

            if (gameEntity == null)
            {
                return null;
            }

            var gameModel = new GameModel
            {
                Id = gameEntity.Id,
                Name = gameEntity.Name,
                Key = gameEntity.Key,
                Description = gameEntity.Description,
                UnitInStock = gameEntity.UnitInStock,
                Price = gameEntity.Price,
                Discontinued = gameEntity.Discontinued,
            };

            return gameModel;
        }

        //public async Task<int> GetGamesCountAsync()
        //{
        //    const string cacheKey = "GamesCount";

        //    if (!_cache.TryGetValue(cacheKey, out int count))
        //    {
        //        var games = await _unitOfWork.GameRepository.GetAllAsync();
        //        count = games.Count();

        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //            .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

        //        _cache.Set(cacheKey, count, cacheEntryOptions);
        //    }

        //    return count;
        //}

        public async Task<IEnumerable<GetPlatformNameByGameDto>> GetPlatformsNameByGame(string gameId)
        {
            var platform = await _unitOfWork.GameRepository.GetPlatformsByGame(gameId);

            var platformIdNameDtos = platform.Select(p => new GetPlatformNameByGameDto
            {
                Id = p!.Id,
                Type = p.Type
            });

            return platformIdNameDtos;
        }

        public async Task<GameUpdateDto> UpdateGameAsync(GameUpdateDto gameUpdateDto)
        {
            if (gameUpdateDto.Game == null)
            {
                throw new ArgumentException($"GameUpdateDto.Game is null.");
            }

            if (gameUpdateDto.Game.Id == null)
            {
                throw new ArgumentException($"GameUpdateDto.Game.Id is null.");
            }

            var game = await _unitOfWork.GameRepository.GetGameDetailsByIdAsync(gameUpdateDto.Game.Id);
            if (game == null)
            {
                throw new ArgumentException($"Game with ID '{gameUpdateDto.Game.Id}' not found.");
            }

            var gameForUpdate = new GameUpdateDto
            {
                Game = new GameResponseForUpdateDto
                {
                    Name = game.Name,
                    Description = game.Description,
                    UnitInStock = game.UnitInStock,
                    Price = game.Price,
                    Discontinued = game.Discontinued,
                },
            };

            game.Name = gameUpdateDto.Game.Name;
            game.Description = gameUpdateDto.Game.Description;
            game.UnitInStock = gameUpdateDto.Game.UnitInStock;
            game.Price = gameUpdateDto.Game.Price;
            game.Discontinued = gameUpdateDto.Game.Discontinued;

            if (gameUpdateDto.Publisher != null)
            {
                Publisher? publisher = await _unitOfWork.PublisherRepository.GetByIdAsync(gameUpdateDto.Publisher);
                if (publisher == null)
                {
                    throw new ArgumentException($"Publisher with ID '{gameUpdateDto.Publisher}' not found.");
                }
                game.PublisherId = publisher.Id;
            }

            if (gameUpdateDto.Platforms != null)
            {
                game.GamePlatforms = game.GamePlatforms.Where(gp => gp.PlatformsId != null && gameUpdateDto.Platforms.Contains(gp.PlatformsId)).ToList();

                foreach (var platformId in gameUpdateDto.Platforms)
                {
                    if (!game.GamePlatforms.Any(gp => gp.PlatformsId == platformId))
                    {
                        game.GamePlatforms.Add(new GamePlatform { GamesKey = game.Key, PlatformsId = platformId });
                    }
                }
            }

            if (gameUpdateDto.Genres != null)
            {
                game.GameGenres = game.GameGenres.Where(gg => gg.GenresId != null && gameUpdateDto.Genres.Contains(gg.GenresId)).ToList();

                foreach (var genreId in gameUpdateDto.Genres)
                {
                    if (!game.GameGenres.Any(gg => gg.GenresId == genreId))
                    {
                        game.GameGenres.Add(new GameGenre { GamesKey = game.Key, GenresId = genreId });
                    }
                }
            }

            await _unitOfWork.GameRepository.UpdateAsync(game);

            await _unitOfWork.SaveAsync();

            return gameForUpdate;
        }
    }
}
