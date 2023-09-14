using Addition.DTO;
using DataAccesLayer.Model;
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
            CreateMap<User, ProfileDTO>().ForMember(x => x.Token, opt => opt.Ignore()).ForMember(x => x.Role, opt => opt.Ignore()).ReverseMap();
        }
    }
}
