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
        Answer<bool> RegisterAdmin(UserDTO userDTO);
        Task<Answer<bool>> RegisterUser(UserDTO userDTO, Roles.RolesType Role, string file);
        Task<Answer<bool>> GoogleRegister(string accessToken, Roles.RolesType Role);
        Answer<ProfileDTO> LoginUser(LoginDTO loginDTO);
        Task<Answer<ProfileDTO>> GoogleLogin(string accessToken);
        Task<Answer<bool>> VerifyUser(VerificationDTO verificationDTO);
        Task<Answer<bool>> DenyUser(VerificationDTO verificationDTO);
        Answer<bool> ResetPassword(ResetPasswordDTO passwordResetDTO);
        Answer<ProfileDTO> GetProfile(string email);
        Answer<ProfileDTO> UpdateProfile(UserDTO userDTO, string file);
        Answer<List<ProfileDTO>> GetVerified();

    }
}
