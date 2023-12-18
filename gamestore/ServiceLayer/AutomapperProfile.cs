using AutoMapper;
using DataAccessLayer.Entities;
using ServiceLayer.Models;
using ServiceLayer.Models.Genre;
using ServiceLayer.Models.Order;
using ServiceLayer.Models.Publisher;

namespace ServiceLayer
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Genre, GenreModel>();
            CreateMap<GenreModel, Genre>();

            CreateMap<Platform, PlatformModel>();
            CreateMap<PlatformModel, Platform>();

            CreateMap<Platform, PlatformResponseDto>();
            CreateMap<PlatformCreateDto, Platform>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Platform.Type));

            CreateMap<Publisher, PublisherModel>();
            CreateMap<PublisherModel, Publisher>();

            CreateMap<Publisher, PublisherResponseDto>();
            CreateMap<PublisherCreateDto, Publisher>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Publisher.CompanyName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Publisher.Description))
                .ForMember(dest => dest.HomePage, opt => opt.MapFrom(src => src.Publisher.HomePage));

            CreateMap<Order, OrderModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate));

            CreateMap<OrderModel, Order>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate));
        }
    }
}
