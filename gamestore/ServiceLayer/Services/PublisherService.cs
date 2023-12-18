using AutoMapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using ServiceLayer.Interfaces;
using ServiceLayer.Models.Publisher;

namespace ServiceLayer.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PublisherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PublisherResponseDto> CreatePublisherAsync(PublisherCreateDto publisherDto)
        {
            Publisher publisher = _mapper.Map<Publisher>(publisherDto);
            await _unitOfWork.PublisherRepository.CreateAsync(publisher);
            return _mapper.Map<PublisherResponseDto>(publisher);
        }

        public async Task<bool> DeletePublisherAsync(string id)
        {
            Publisher? deletedPublisher = await _unitOfWork.PublisherRepository.DeletePublisherByIdAsync(id);
            return deletedPublisher != null;
        }

        public async Task<IEnumerable<PublisherModel>> GetAllModelsAsync()
        {
            var publishers = await _unitOfWork.PublisherRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PublisherModel>>(publishers);
        }

        public async Task<Publisher?> GetPublisherByIdAsync(string id)
        {
            return await _unitOfWork.PublisherRepository.GetByIdAsync(id);
        }

        public async Task<PublisherModel?> GetPublisherModelDescriptionAsync(string companyName)
        {
            var publisherEntity = await _unitOfWork.PublisherRepository.GetPublisherDetailsByCompanyNameAsync(companyName);

            if (publisherEntity != null)
            {
                var publisherModel = _mapper.Map<PublisherModel>(publisherEntity);
                return publisherModel;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<GetGameNameByPublisherDto>> GetGamesNameByPublisherCompanyName(string companyname)
        {
            var games = await _unitOfWork.PublisherRepository.GetGamesByPublisherCompanyName(companyname);

            var gameIdNameDtos = games.Select(g => new GetGameNameByPublisherDto
            {
                Name = g.Name
            });

            return gameIdNameDtos;
        }

        public async Task<PublisherResponseForUpdateDto?> UpdatePublisherAsync(PublisherUpdateDto publisherUpdateDto)
        {
            if (publisherUpdateDto == null)
            {
                throw new ArgumentNullException(nameof(publisherUpdateDto));
            }

            if (publisherUpdateDto.Publisher == null)
            {
                throw new ArgumentException(nameof(publisherUpdateDto.Publisher), "Publisher data is required");
            }

            if (string.IsNullOrEmpty(publisherUpdateDto.Publisher.CompanyName)
                    || string.IsNullOrEmpty(publisherUpdateDto.Publisher.Description)
                    || string.IsNullOrEmpty(publisherUpdateDto.Publisher.HomePage)
                    || string.IsNullOrEmpty(publisherUpdateDto.Publisher.Id))
            {
                throw new ArgumentException(nameof(publisherUpdateDto.Publisher), "All Publisher properties must be non-null");
            }

            var publisher = new Publisher
            {
                Id = publisherUpdateDto.Publisher.Id,
                CompanyName = publisherUpdateDto.Publisher.CompanyName,
                Description = publisherUpdateDto.Publisher.Description,
                HomePage = publisherUpdateDto.Publisher.HomePage
            };

            Publisher? updatedPublisher = await _unitOfWork.PublisherRepository.UpdateAsync(publisher);

            if (updatedPublisher == null) return null;

            if (updatedPublisher == null || string.IsNullOrEmpty(updatedPublisher.CompanyName)
                    || string.IsNullOrEmpty(updatedPublisher.Description)
                    || string.IsNullOrEmpty(updatedPublisher.HomePage))
            {
                throw new ArgumentNullException(nameof(updatedPublisher), "All properties of updatedPublisher must be non-null");
            }

            var publisherResponseForUpdateDto = new PublisherResponseForUpdateDto
            {
                CompanyName = updatedPublisher.CompanyName,
                Description = updatedPublisher.Description,
                HomePage = updatedPublisher.HomePage,
            };

            return publisherResponseForUpdateDto;
        }
    }
}
