using Addition;
using Addition.Constants;
using Addition.DTO;
using Addition.Mutual;
using AutoMapper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccesLayer.Model;
using DataAccesLayer.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
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

        public async Task<Answer<bool>> RegisterUser(UserDTO userDTO, Roles.RolesType Role, string file)
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
            newUser.ProfileUrl = file;
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

            try
            {
                var success = await _emailService.SendMailAsync(new EmailData()
                {
                    Towho = newUser.Email,
                    Content = emailContent,
                    HtmlContent = true,
                    Subject = "Aktivacija naloga"
                });

                if (success)
                {
                    _uow.User.Add(newUser);
                    _uow.Save();
                    return new Answer<bool>(true, Feedback.OK, "User registered succesfully");
                }
                else
                    return new Answer<bool>(false, Feedback.InternalServerError, "There was an error while registering new user");
            }
            catch (Exception ex)
            {
                return new Answer<bool>(false, Feedback.InternalServerError, ex.Message);
            }
        }
        public Answer<bool> RegisterAdmin(UserDTO userDTO)
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
            newUser.Role = Roles.RolesType.Administrator;
            newUser.IsVerified = true;

            newUser.ProfileUrl = Path.Combine(Directory.GetCurrentDirectory(), "Avatars");
            newUser.ProfileUrl = Path.Combine(newUser.ProfileUrl, "avatar.svg");
            try
            {
                _uow.User.Add(newUser);
                _uow.Save();
                return new Answer<bool>(true, Feedback.OK, "User registered succesfully");
            }
            catch (Exception ex)
            {
                return new Answer<bool>(false, Feedback.InternalServerError, ex.Message);
            }
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
                        issuer: "http://localhost:4646", 
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(20),
                        signingCredentials: signinCredentials
                    );
                    string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                    ProfileDTO p = _mapper.Map<ProfileDTO>(u);
                    p.Token = tokenString;
                    p.Role = u.Role;

                    byte[] imageBytes = System.IO.File.ReadAllBytes(u.ProfileUrl);
                    p.Avatar = Convert.ToBase64String(imageBytes);

                    return new Answer<ProfileDTO>(p, Feedback.OK, "Login successful");
                }
                else
                    return new  Answer<ProfileDTO>(null, Feedback.NotFound, "Wrong password");
            }
            else
                return new Answer<ProfileDTO>(null, Feedback.NotFound, "This user does not exist");
        }

        public Answer<List<ProfileDTO>> GetVerified()
        {
            List<User> notVerified = _uow.User.GetAll(u => !u.IsVerified).ToList();
            if (notVerified.Count == 0)
                return new Answer<List<ProfileDTO>>(null, Feedback.AllUsersVerified, "All users are verified");
            else
            {
                List<ProfileDTO> response = new List<ProfileDTO>();
                foreach (var elem in notVerified)
                {
                    ProfileDTO retUser = _mapper.Map<ProfileDTO>(elem);
                    byte[] imageBytes = System.IO.File.ReadAllBytes(elem.ProfileUrl);
                    retUser.Avatar = Convert.ToBase64String(imageBytes);
                    response.Add(retUser);
                }
                return new Answer<List<ProfileDTO>>(response, Feedback.OK);
            }
        }

        public async Task<Answer<bool>> VerifyUser(VerificationDTO verificationDTO)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.UserName == verificationDTO.UserName);
            if (u == null)
            {
                return new Answer<bool>(false, Feedback.NotFound);
            }
            u.IsVerified = true;
            string emailContent = $"<p>Zdravo {u.FirstName} {u.LastName},</p>";
            emailContent += $"<p>Vas nalog je verifikovan. Zahvaljujemo vam se na strpljenju.</p>";

            try
            {
                _uow.User.Update(u);
                _uow.Save();

                var success = await _emailService.SendMailAsync(new EmailData()
                {
                    Towho = u.Email,
                    Content = emailContent,
                    HtmlContent = true,
                    Subject = "Verifikacija naloga"
                });

                if (success)
                    return new Answer<bool>(true, Feedback.OK, "User verified succesfully");
                else
                    return new Answer<bool>(false, Feedback.InternalServerError, "There was an error while verifying user");
            }
            catch (Exception ex)
            {
                return new Answer<bool>(false, Feedback.InternalServerError, ex.Message);
            }
        }

        public async Task<Answer<bool>> DenyUser(VerificationDTO verificationDTO)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.UserName == verificationDTO.UserName);
            if (u == null)
            {
                return new Answer<bool>(false, Feedback.NotFound);
            }

            string emailContent = $"<p>Zdravo {u.FirstName} {u.LastName},</p>";
            emailContent += $"<p>Vas nalog je odbijen iz razloga {verificationDTO.Reason}. </p>";

            try
            {
                _uow.User.Remove(u);
                _uow.Save();

                var success = await _emailService.SendMailAsync(new EmailData()
                {
                    Towho = u.Email,
                    Content = emailContent,
                    HtmlContent = true,
                    Subject = "Verifikacija naloga"
                });

                if (success)
                    return new Answer<bool>(true, Feedback.OK, "User denied");
                else
                    return new Answer<bool>(false, Feedback.InternalServerError, "There was an error");
            }
            catch (Exception ex)
            {
                return new Answer<bool>(false, Feedback.InternalServerError, ex.Message);
            }
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

        public Answer<ProfileDTO> UpdateProfile(UserDTO userDTO, string file)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.Email == userDTO.Email);
            string avatar = String.Empty;
            if (file != String.Empty)
            {
                avatar = u.ProfileUrl;
                u.ProfileUrl = file;

            }
            u.FirstName = userDTO.FirstName;
            u.LastName = userDTO.LastName;
            u.BirthDate = userDTO.BirthDate;
            u.Address = userDTO.Address;

            try
            {
                _uow.User.Update(u);
                _uow.Save();

                ProfileDTO p = _mapper.Map<ProfileDTO>(u);
                p.Role = u.Role;

                byte[] imageBytes = System.IO.File.ReadAllBytes(u.ProfileUrl);
                p.Avatar = Convert.ToBase64String(imageBytes);
                if (avatar != String.Empty)
                    if (avatar.Split('\\')[1] != "avatar.svg")
                        System.IO.File.Delete(avatar);
                return new Answer<ProfileDTO>(p, Feedback.OK, "Profile changed");
            }
            catch (Exception ex)
            {
                return new Answer<ProfileDTO>(null, Feedback.InternalServerError, "There was an error");
            }
        }

        public async Task<Answer<ProfileDTO>> GoogleLogin(string accessToken)
        {
            var httpClient = new HttpClient();
            var requestUrl = $"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={accessToken}";

            try
            {
                var response = await httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Google login verification failed.");
                }
                var tokenInfo = await response.Content.ReadFromJsonAsync<GoogleTokenInfo>();

                User u = _uow.User.GetFirstOrDefault(u => u.Email == tokenInfo.Email);

                if (u != null)
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
                        issuer: "http://localhost:4646", 
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(20),
                        signingCredentials: signinCredentials
                    );
                    string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                    ProfileDTO p = _mapper.Map<ProfileDTO>(u);
                    p.Token = tokenString;
                    p.Role = u.Role;

                    byte[] imageBytes = System.IO.File.ReadAllBytes(u.ProfileUrl);
                    p.Avatar = Convert.ToBase64String(imageBytes);

                    return new Answer<ProfileDTO>(p, Feedback.OK, "Login successful");
                }
                else
                    throw new KeyNotFoundException("This user does not exist");

            }
            catch (KeyNotFoundException ex)
            {
                return new Answer<ProfileDTO>(null, Feedback.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                return new Answer<ProfileDTO>(null, Feedback.InternalServerError, ex.Message);
            }
        }

        public async Task<Answer<bool>> GoogleRegister(string accessToken, Roles.RolesType Role)
        {
            
            var httpClient = new HttpClient();
            var requestUrl = $"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={accessToken}";

            try
            {
                var response = await httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Google login verification failed.");
                }
                var tokenInfo = await response.Content.ReadFromJsonAsync<GoogleTokenInfo>();

                if (MailExists(tokenInfo.Email))
                {
                    return new Answer<bool>(false, Feedback.InvalidEmail, "Email already exists");
                }
                User newUser = new User
                {
                    Address = " ",
                    BirthDate = DateTime.UtcNow.AddYears(-18),
                    UserName = "user" + Guid.NewGuid(),
                    Email = tokenInfo.Email,
                    FirstName = tokenInfo.GivenName,
                    LastName = tokenInfo.FamilyName,
                };
                if ((tokenInfo.GivenName == null || tokenInfo.FamilyName == null) && tokenInfo.Name.Contains(' '))
                {
                    newUser.FirstName = tokenInfo.Name.Split(' ')[0];
                    newUser.LastName = tokenInfo.Name.Split(' ')[1];
                }
                else
                {
                    newUser.FirstName = tokenInfo.Name;
                    newUser.LastName = tokenInfo.Name;
                }

                byte[] salt = Hash.GenerateSalt();
                newUser.Salt = salt;
                newUser.Password = Hash.GenerateSaltedHash(Encoding.ASCII.GetBytes(Guid.NewGuid().ToString()), salt);

                string pictureUrl = tokenInfo.Picture;

                
                var pictureResponse = await httpClient.GetAsync(pictureUrl);

                if (pictureResponse.IsSuccessStatusCode)
                {
                    var pictureBytes = await pictureResponse.Content.ReadAsByteArrayAsync();
                    string extension = GetFileExtensionFromContentType(pictureResponse.Content.Headers.ContentType?.MediaType);

                    string fileName = Guid.NewGuid().ToString() + extension;
                    string filePath = Path.Combine("Avatars\\", fileName);

                    
                    File.WriteAllBytes(filePath, pictureBytes);

                    newUser.ProfileUrl = filePath;
                }
                else
                {
                    newUser.ProfileUrl = "\\Avatars\\avatar.svg";
                }

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

                try
                {
                    var success = await _emailService.SendMailAsync(new EmailData()
                    {
                        Towho = newUser.Email,
                        Content = emailContent,
                        HtmlContent = true,
                        Subject = "Aktivacija naloga"
                    });

                    if (success)
                    {
                        _uow.User.Add(newUser);
                        _uow.Save();
                        return new Answer<bool>(true, Feedback.OK, "User registered succesfully");
                    }
                    else
                        return new Answer<bool>(false, Feedback.InternalServerError, "There was an error while registering new user");
                }
                catch (Exception ex)
                {
                    return new Answer<bool>(false, Feedback.InternalServerError, ex.Message);
                }
            }
            catch (Exception ex)
            {
                return new Answer<bool>(false, Feedback.InternalServerError, ex.Message);
            }
        }

        private string GetFileExtensionFromContentType(string contentType)
        {
            switch (contentType)
            {
                case "image/jpeg":
                    return ".jpg";
                case "image/png":
                    return ".png";
                case "image/gif":
                    return ".gif";
                default:
                    return ".jpg"; 
            }
        }
    }
}
