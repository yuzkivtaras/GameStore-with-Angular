using AutoMapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Interfaces;
using ServiceLayer.Models;
using ServiceLayer.Models.Platform;

namespace ServiceLayer.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPlatformRepository _platformRepository;

        public PlatformService(IUnitOfWork unitOfWork, IMapper mapper, IPlatformRepository platformRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _platformRepository = platformRepository;
        }

        public async Task<PlatformResponseDto> CreatePlatformAsync(PlatformCreateDto platformDto)
        {
            Platform platform = _mapper.Map<Platform>(platformDto);
            await _unitOfWork.PlatformRepository.CreateAsync(platform);
            return _mapper.Map<PlatformResponseDto>(platform);
        }

        public async Task<bool> DeletePlatformAsync(string id)
        {
            Platform? deletedPlatform = await _unitOfWork.PlatformRepository.DeletePlatformByIdAsync(id);
            return deletedPlatform != null;
        }

        public async Task<IEnumerable<PlatformModel>> GetAllModelsAsync()
        {
            var platforms = await _unitOfWork.PlatformRepository.GetAllAsync();
            return _mapper.Map < IEnumerable<PlatformModel>> (platforms);
        }

        public async Task<PlatformModel?> GetPlatformModelDescriptionAsync(string id)
        {
            var platformEntity = await _unitOfWork.PlatformRepository.GetPlatformDetailsByIdAsync(id);
            return _mapper.Map<PlatformModel>(platformEntity);
        }

        public async Task<IEnumerable<Platform>> GetPlatformsByIdsAsync(IEnumerable<string> type)
        {
            IQueryable<Platform> platformsQueryable = _platformRepository.GetAll();

            var filteredPlatforms = platformsQueryable.Where(platform => type.Contains(platform.Type));

            return await filteredPlatforms.ToListAsync();
        }

        public async Task<PlatformResponseForUpdateDto?> UpdatePlatformAsync(PlatformUpdateDto platformDto)
        {
            if (platformDto.Platform == null)
            {
                throw new ArgumentNullException(nameof(platformDto.Platform), "Platform data and ID are required.");
            }

            string platformId = platformDto.Platform.Id ?? string.Empty;

            var platform = new Platform
            {
                Id = platformId,
                Type = platformDto.Platform.Type
            };

            Platform? updatedPlatform = await _unitOfWork.PlatformRepository.UpdateAsync(platform);

            if (updatedPlatform == null)
            {
                throw new InvalidOperationException("Failed to update the platform.");
            }

            string updatedPlatformType = updatedPlatform?.Type ?? string.Empty;

            return new PlatformResponseForUpdateDto
            {
                Type = updatedPlatformType,
            };
        }

        public async Task<IEnumerable<GetGameNameByPlatformDto>> GetGamesNameByPlatformId(string platformId)
        {
            var games = await _unitOfWork.PlatformRepository.GetGamesByPlatformId(platformId);

            var gameIdNameDtos = games.Select(g => new GetGameNameByPlatformDto
            {
                Id = g.Id,
                Name = g.Name
            });

            return gameIdNameDtos;
        }
    }
}
