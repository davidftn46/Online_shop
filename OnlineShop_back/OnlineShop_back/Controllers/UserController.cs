using Addition.DTO;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Addition;
using Addition.Mutual;
using Addition.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Projekat_Backend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO LoginDTO)
        {
            if (ModelState.IsValid)
            {
                Answer<ProfileDTO> response = _userService.LoginUser(LoginDTO);

                if (response.Status == Feedback.OK)
                    return Ok(response.Data);
                else
                {
                    return Problem(response.Message, statusCode: (int)response.Status);
                }
            }
            else
            {
                return Problem("Entered values not valid", statusCode: (int)Feedback.BadRequest);
            }
        }


        [HttpPost("googleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] string googleAccessToken = null)
        {
            if (!string.IsNullOrEmpty(googleAccessToken))
            {
                Answer<ProfileDTO> response = await _userService.GoogleLogin(googleAccessToken);
                if (response.Status == Feedback.OK)
                    return Ok(response.Data);
                else
                {
                    return Problem(response.Message, statusCode: (int)response.Status);
                }
            }
            return BadRequest();

        }

        [HttpPost("googleRegister")]
        public async Task<IActionResult> GoogleRegister([FromBody] GoogleRegisterToken googleRegisterDTO)
        {
            if (!string.IsNullOrEmpty(googleRegisterDTO.GoogleAccessToken))
            {
                Answer<bool> response;
                if (googleRegisterDTO.Role == "customer")
                    response = await _userService.GoogleRegister(googleRegisterDTO.GoogleAccessToken, Roles.RolesType.Customer);
                else if (googleRegisterDTO.Role == "seller")
                    response = await _userService.GoogleRegister(googleRegisterDTO.GoogleAccessToken, Roles.RolesType.Seller);
                else
                    return BadRequest();

                if (response.Status == Feedback.OK)
                    return Ok(response.Data);
                else
                {
                    return Problem(response.Message, statusCode: (int)response.Status);
                }
            }
            return BadRequest();

        }

        [HttpPost("registerCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromForm] UserDTO UserDTO, IFormFile? file = null)
        {
            if (ModelState.IsValid)
            {
                string filePath;
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    string uploadPath = "Avatars";
                    Console.WriteLine(uploadPath);

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Avatars");
                    filePath = Path.Combine(filePath, "avatar.svg");
                }

                var task = await _userService.RegisterUser(UserDTO, Roles.RolesType.Customer, filePath);
                if (task.Status == Feedback.OK)
                    return Ok(task.Message);
                else if (task.Status == Feedback.InternalServerError)
                {
                    if (System.IO.File.Exists(filePath) && file != null)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return Problem(task.Message, statusCode: ((int)task.Status));
                }
                else
                    return Problem(task.Message, statusCode: ((int)task.Status));
            }
            return Problem("Entered values not valid", statusCode: (int)Feedback.BadRequest);

        }

        [HttpPost("registerSeller")]
        public async Task<IActionResult> RegisterSeller([FromForm] UserDTO UserDTO, IFormFile? file = null)
        {
            if (ModelState.IsValid)
            {
                string filePath;
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Avatars");
                    Console.WriteLine(uploadPath);

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Avatars");
                    filePath = Path.Combine(filePath, "avatar.svg");
                }

                var task = await _userService.RegisterUser(UserDTO, Roles.RolesType.Seller, filePath);
                if (task.Status == Feedback.OK)
                    return Ok(task.Message);
                else if (task.Status == Feedback.InternalServerError)
                {
                    if (System.IO.File.Exists(filePath) && file != null)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return Problem(task.Message, statusCode: ((int)task.Status));
                }
                else
                    return Problem(task.Message, statusCode: ((int)task.Status));
            }
            return Problem();

        }

        [HttpPost("registerAdmin")]
        public IActionResult RegisterAdmin(UserDTO UserDTO)
        {
            if (ModelState.IsValid)
            {
                var task = _userService.RegisterAdmin(UserDTO);
                if (task.Status == Feedback.OK)
                    return Ok(task.Message);
                else if (task.Status == Feedback.InvalidEmail)
                    ModelState.AddModelError("email", task.Message);
                else if (task.Status == Feedback.InvalidUsername)
                    ModelState.AddModelError("username", task.Message);
                else if (task.Status == Feedback.InternalServerError)
                    ModelState.AddModelError(String.Empty, task.Message);
                else
                    ModelState.AddModelError(String.Empty, task.Message);
            }
            return Problem();

        }

        [HttpGet("notVerified")]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetUnverified()
        {
            var result = _userService.GetVerified();
            if (result.Status == Feedback.AllUsersVerified)
                return Ok(result.Message);
            else if (result.Status == Feedback.OK)
                return Ok(result.Data);
            else
                return Problem(result.Message);

        }

        [HttpPost("verify")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Verify([FromBody] VerificationDTO user)
        {
            var result = await _userService.VerifyUser(user);
            if (result.Status == Feedback.OK)
                return Ok(result.Message);
            else
                return Problem(detail: result.Message, statusCode: (int)result.Status);
        }

        [HttpPost("deny")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Deny([FromBody] VerificationDTO user)
        {
            var result = await _userService.DenyUser(user);
            if (result.Status == Feedback.OK)
                return Ok(result.Message);
            else
                return Problem(detail: result.Message, statusCode: (int)result.Status);
        }

        [HttpGet("resetPassword")]
        public async Task<IActionResult> ResetPassword(Guid guid)
        {
            ResetPasswordDTO resetDTO = new ResetPasswordDTO();
            
            return Ok(resetDTO);

        }
        [HttpPost("resetPassword")]
        public IActionResult ResetPassword(ResetPasswordDTO passwordResetDTO)
        {
            if (ModelState.IsValid)
            {
                Answer<bool> response = _userService.ResetPassword(passwordResetDTO);
                if (response.Status == Feedback.OK)
                {
                    
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, response.Message);
                    return NotFound();
                }
            }
            return Ok();
        }

        [HttpPost("updateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] UserDTO UserDTO, IFormFile? file = null)
        {
            if (ModelState.IsValid)
            {
                string filePath;
                string avatar = String.Empty;
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    string uploadPath = "Avatars";
                    Console.WriteLine(uploadPath);

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }


                }
                else
                    filePath = String.Empty;

                var task = _userService.UpdateProfile(UserDTO, filePath);
                if (task.Status == Feedback.OK)
                {

                    return Ok(task.Data);
                }
                else if (task.Status == Feedback.InternalServerError)
                {
                    if (System.IO.File.Exists(filePath) && file != null)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return Problem(task.Message, statusCode: ((int)task.Status));
                }
                else
                    return Problem(task.Message, statusCode: ((int)task.Status));
            }
            return Problem("Entered values not valid", statusCode: (int)Feedback.BadRequest);

        }

    }
}