using Addition;
using Addition.Constants;
using Addition.DTO;
using Addition.Mutual;
using AutoMapper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccesLayer.Model;
using DataAccesLayer.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.SrvcImplementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _secretKey;

        public UserService(IUnitOfWork uow, IEmailService emailService, IConfiguration configuration, IMapper mapper)
        {
            _uow = uow;
            _emailService = emailService;
            _configuration = configuration;
            _mapper = mapper;
            _secretKey = _configuration.GetSection("SecretKey");
        }

        public Answer<bool> VerifyUser(long id)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.Id == id);
            if (u != null)
            {
                u.IsVerified = true;
            }
            else
                return new Answer<bool>(false, Feedback.AccountAlreadyActivated, "Account already verified");
            _uow.User.Update(u);
            _uow.Save();

            return new Answer<bool>(true, Feedback.OK, "Account verified");
        }

        public Answer<ProfileDTO> LoginUser(LoginDTO loginDTO)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.Email == loginDTO.Email);

            if (u != null)
            {
               
                if (u.Password.SequenceEqual(Hash.GenerateSaltedHash(Encoding.ASCII.GetBytes(loginDTO.Password), u.Salt)))
                {
                    List<Claim> claims = new List<Claim>();
                    if (u.Role == Roles.RolesType.Customer)
                        claims.Add(new Claim(ClaimTypes.Role, "Customer"));
                    if (u.Role == Roles.RolesType.Seller)
                        claims.Add(new Claim(ClaimTypes.Role, "Seller"));
                    if (u.Role == Roles.RolesType.Administrator)
                        claims.Add(new Claim(ClaimTypes.Role, "Administrator"));

                    SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey.Value));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var tokeOptions = new JwtSecurityToken(
                        issuer: "http://localhost:44327", 
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(20),
                        signingCredentials: signinCredentials
                    );
                    string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                    ProfileDTO p = _mapper.Map<ProfileDTO>(u);
                    p.Token = tokenString;
                    p.Role = u.Role;
                    return new Answer<ProfileDTO>(p, Feedback.OK, "Login successful");
                }
                else
                    return new Answer<ProfileDTO>(null, Feedback.NotFound, "There was an error with login");
            }
            else
                return new Answer<ProfileDTO>(null, Feedback.NotFound, "This user does not exist");
        }


        private bool MailExists(string email)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.Email == email);
            if (u != null)
                return true;
            else
                return false;
        }
        private bool UsernameExists(string username)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.UserName == username);
            if (u != null)
                return true;
            else
                return false;
        }

        private bool AccountVerified(string email)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.Email == email);
            if (u.IsVerified)
                return true;
            else
                return false;
        }

        public async Task<Answer<bool>> RegisterUser(UserDTO userDTO, Roles.RolesType Role)
        {
            if (MailExists(userDTO.Email))
            {
                return new Answer<bool>(false, Feedback.InvalidEmail, "Email already exists");
            }
            if (UsernameExists(userDTO.UserName))
            {
                return new Answer<bool>(false, Feedback.InvalidUsername, "Username already exists");
            }

            User newUser = _mapper.Map<User>(userDTO);

            byte[] salt = Hash.GenerateSalt();
            newUser.Salt = salt;
            newUser.Password = Hash.GenerateSaltedHash(Encoding.ASCII.GetBytes(userDTO.Password), salt);

            string emailContent;
            if (Role == Roles.RolesType.Customer)
            {
                newUser.IsVerified = true;
                newUser.Role = Roles.RolesType.Customer;
                emailContent = $"<p>Zdravo {newUser.FirstName} {newUser.LastName},</p>";
                emailContent += $"<p>Vas nalog je uspešno napravljen. Zelimo vam srecnu kupovinu.</p>";
            }
            else
            {
                newUser.Role = Roles.RolesType.Seller;
                newUser.IsVerified = false;
                emailContent = $"<p>Zdravo {newUser.FirstName} {newUser.LastName},</p>";
                emailContent += $"<p>Vas nalog je uspešno napravljen. Molimo vas da sačekate da neko od naših administratora pregleda i odobri vaš profil.</p>";
                emailContent += $"<p>Dobićete email obaveštenja kada nalog bude pregledan.</p>";
            }
            newUser.ProfileUrl = @"\img\profilePictures\img_avatar.png";
            try
            {
                _uow.User.Add(newUser);
                _uow.Save();

                var success = await _emailService.SendMailAsync(new EmailData()
                {
                    Towho = newUser.Email,
                    Content = emailContent,
                    HtmlContent = true,
                    Subject = "Aktivacija naloga"
                });

                if (success)
                    return new Answer<bool>(true, Feedback.OK, "User registered succesfully");
                else
                    return new Answer<bool>(false, Feedback.InternalServerError, "There was an error while registering new user");
            }
            catch (Exception ex)
            {
                return new Answer<bool>(false, Feedback.InternalServerError, ex.Message);
            }
        }
        public async Task<Answer<bool>> RegisterAdmin(AdministratorDTO userDTO)
        {

            return new Answer<bool>(false, Feedback.InternalServerError, "aaa");
            

        }

        public async Task<Answer<bool>> ForgotPassword(string email)
        {
            if (MailExists(email))
            {
                if (AccountVerified(email))
                {
                    User u = _uow.User.GetFirstOrDefault(u => u.Email == email);
                    u.PasswordGuid = Guid.NewGuid();
                    _uow.User.Update(u);
                    _uow.Save();

                    var emailContent = $"<p>Zdravo {u.FirstName} {u.LastName},</p>";
                    emailContent += $"<p>Vas nalog je uspešno napravljen. Kliknite na link ispod da biste ga aktivirali.</p>";
                    emailContent += $"<a href='{_configuration["ResetPasswordUrl"]}{u.PasswordGuid}'>Resetuj lozinku</a>";

                    var success = await _emailService.SendMailAsync(new Addition.Mutual.EmailData()
                    {
                        Towho = u.Email,
                        Content = emailContent,
                        HtmlContent = true,
                        Subject = "Reset lozinke"
                    });

                    if (success)
                        return new Answer<bool>(success, Feedback.OK, "Reset mail sent");
                    else
                        return new Answer<bool>(success, Feedback.InternalServerError, "There was an error");
                }
                else return new Answer<bool>(false, Feedback.AccountNotActivated, "Account is not activated");

            }
            else
                return new Answer<bool>(false, Feedback.InvalidEmail, "Mail does not exist");
        }

        public Answer<bool> ResetPassword(ResetPasswordDTO passwordResetDTO)
        {

            return new Answer<bool>(false, Feedback.InvalidPasswordGuid, "Password reset link is not active anymore");
            
        }

        public Answer<ProfileDTO> GetProfile(string email)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.Email == email);
            return new Answer<ProfileDTO>(_mapper.Map<ProfileDTO>(u), Feedback.OK, "Profile");
        }

        public Answer<bool> UpdateProfile(ProfileDTO profileDTO)
        {

            return new Answer<bool>(true, Feedback.OK, "Profile changed");
        }
    }
}
