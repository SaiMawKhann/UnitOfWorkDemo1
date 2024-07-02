using AutoMapper;
using kzy_entities.Entities;
using kzy_entities.Models.Response.Onboarding;
using kzy_entities.Models.Response.Product;
using System.Runtime.ConstrainedExecution;

namespace UnitOfWorkDemo1.MapperProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductListResponseModel>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedOn));

            CreateMap<Customer, ProfileResponseModel>();

            CreateMap<Product, ProductDetailResponseModel>()
                 .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                 .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedOn)); 

        }
    }
}
