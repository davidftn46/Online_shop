using Addition.DTO;
using DataAccesLayer.Model;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Map
{
    public class MapProfile: AutoMapper.Profile
    {
        public MapProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, ProfileDTO>().ForMember(x => x.Token, opt => opt.Ignore()).ForMember(x => x.Role, opt => opt.Ignore()).ForMember(x => x.Avatar, opt => opt.Ignore()).ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, NewProductDTO>().ReverseMap();
            CreateMap<OrderProduct, OrderProductDTO>();
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts));
        }
    }
}
