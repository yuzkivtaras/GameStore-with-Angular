using DataAccessLayer.Entities;
using ServiceLayer.Models.Publisher;

namespace ServiceLayer.Interfaces
{
    public interface IPublisherService : ICrud<PublisherModel>
    {
        Task<PublisherModel?> GetPublisherModelDescriptionAsync(string companyname);
        Task<PublisherResponseDto> CreatePublisherAsync(PublisherCreateDto publisherDto);
        Task<bool> DeletePublisherAsync(string id);

        Task<Publisher?> GetPublisherByIdAsync(string id);

        Task<IEnumerable<GetGameNameByPublisherDto>> GetGamesNameByPublisherCompanyName(string companyname);

        Task<PublisherResponseForUpdateDto?> UpdatePublisherAsync(PublisherUpdateDto publisherUpdateDto);
    }
}
