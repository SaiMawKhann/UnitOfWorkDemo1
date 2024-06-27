using kzy_entities.Entities;
using kzy_entities.Models.Response.Product;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnitOfWorkDemo1.MapperProfiles
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductListResponseModel>()
                 .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name));
        }
    }
}
