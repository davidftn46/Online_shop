using Addition.DTO;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Addition;
using Addition.Mutual;
using Addition.Constants;

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
                    ModelState.AddModelError(String.Empty, response.Message);
                    return Problem("Login");
                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, "Invalid login atempt.");
                return Problem("Login");
            }
        }

        [HttpPost("registerCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] UserDTO UserDTO)
        {
            if (ModelState.IsValid)
            {
                var task = await _userService.RegisterUser(UserDTO, Roles.RolesType.Customer);
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

        [HttpPost("registerSeller")]
        public async Task<IActionResult> RegisterSeller([FromBody] UserDTO UserDTO)
        {
            if (ModelState.IsValid)
            {
                var task = await _userService.RegisterUser(UserDTO, Roles.RolesType.Seller);
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

        [HttpPost("registerAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(AdministratorDTO UserDTO)
        {
            if (ModelState.IsValid)
            {
                var task = await _userService.RegisterAdmin(UserDTO);
                if (task.Status == Feedback.OK)
                    return RedirectToAction("Index", "Home");
                else if (task.Status == Feedback.InvalidEmail)
                    ModelState.AddModelError("email", task.Message);
                else
                    ModelState.AddModelError(String.Empty, task.Message);
            }
            return Ok();

        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var task = await _userService.ForgotPassword(email);
            if (task.Status == Feedback.OK)
                return RedirectToAction("Index", "Home");
            else if (task.Status == Feedback.InvalidEmail)
                ModelState.AddModelError(String.Empty, task.Message);
            else if (task.Status == Feedback.AccountNotActivated)
                ModelState.AddModelError(String.Empty, task.Message);
            else
                ModelState.AddModelError(String.Empty, task.Message);

            return Ok();

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

        [HttpGet("profile")]
        public IActionResult Profil(string? email)
        {
            if (email == null)
                return RedirectToAction("Login", "Account");
            ProfileDTO profileDTO = _userService.GetProfile(email).Data;

            return Ok(profileDTO);
        }

        [HttpPost]
        [ActionName("Profil")]
        public IActionResult Profil(ProfileDTO profileDTO, IFormFile? file)
        {
            if (ModelState.IsValid)
            {

                Answer<bool> response = _userService.UpdateProfile(profileDTO);
                if (response.Status == Feedback.OK)
                {

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return NotFound();
                }
            }
            return Ok();
        }

    }
}