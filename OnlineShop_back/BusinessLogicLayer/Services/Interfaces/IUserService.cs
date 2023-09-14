using Addition;
using Addition.DTO;
using Addition.Mutual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IUserService
    {
        Task<Answer<bool>> RegisterAdmin(AdministratorDTO userDTO);
        Task<Answer<bool>> RegisterUser(UserDTO userDTO, Roles.RolesType Role);
        Answer<ProfileDTO> LoginUser(LoginDTO loginDTO);
        Answer<bool> VerifyUser(long id);
        Task<Answer<bool>> ForgotPassword(string email);
        Answer<bool> ResetPassword(ResetPasswordDTO passwordResetDTO);
        Answer<ProfileDTO> GetProfile(string email);
        Answer<bool> UpdateProfile(ProfileDTO profileDTO);
    }
}
