using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.Brand, O => O.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.Category, O => O.MapFrom(s => s.Category.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>());


            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BasketItemDto, BasketItem>();

            CreateMap<AddressDto, Core.Entities.OrderAggregate.Address>();

            CreateMap<Core.Entities.Identity.Address, AddressDto>()
                .ForMember(d => d.FirstName, O => O.MapFrom(s => s.FName))
                .ForMember(d => d.LastName, O => O.MapFrom(s => s.LName)).ReverseMap();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, O => O.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryMethodCost, O => O.MapFrom(s => s.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductName, O => O.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.ProductId, O => O.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.PictureUrl, O => O.MapFrom(s => s.Product.PictureUrl))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());

        }
    }
}
