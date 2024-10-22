using BackendGameVibes.IServices;
using BackendGameVibes.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;


namespace BackendGameVibes.Controllers {
    [ApiController]
    [Route("account")]
    [Authorize]
    public class AccountController : ControllerBase {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService) {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.RegisterUserAsync(model);

            if (result.Succeeded) {
                var user = await _accountService.GetUserByEmailAsync(model.Email);
                if (user != null) {
                    await _accountService.SendConfirmationEmailAsync(model.Email, user!);
                    return Ok("UserRegisteredSuccessfully");
                }
                else {
                    return Ok("UserRegisteredFailed user not created");
                }
            }

            foreach (var error in result.Errors) {
                return error.Code switch {
                    "DuplicateUserName" => StatusCode(450, "Username already taken"),
                    "DuplicateEmail" => StatusCode(452, "Email already taken"),
                    _ => StatusCode(454, error.Code)
                };
            }

            return BadRequest(ModelState);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _accountService.GetUserByEmailAsync(model.Email);

            if (user != null) {
                if (!user.EmailConfirmed) {
                    return StatusCode(470, "Email unconfirmed");
                }

                var loginResult = await _accountService.LoginUserAsync(user, model.Password);
                if (loginResult != null && loginResult.Succeeded) {
                    var token = await _accountService.GenerateJwtTokenAsync(user);

                    //var userToken = new IdentityUserToken<string> {
                    //    UserId = user.Id,
                    //    LoginProvider = "Default",
                    //    Name = "LoginToken",
                    //    Value = token
                    //};

                    //await _accountService.SaveTokenToDB(userToken); // todo: ERROR

                    return Ok(new { accessToken = token });
                }
            }

            return Unauthorized("Invalid login attempt");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAccountInfo() {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return Unauthorized("User not authenticated");

            var user = await _accountService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            var accountInfo = await _accountService.GetAccountInfoAsync(user.Id, user);
            return Ok(accountInfo);
        }

        [HttpPatch("change-username")]
        [Authorize]
        public async Task<IActionResult> ChangeNickname([FromBody] ValueModel valueModel) {
            string newUsername = valueModel.Value!;
            if (string.IsNullOrWhiteSpace(newUsername))
                return BadRequest("Invalid username");

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return Unauthorized("User not authenticated");

            var user = await _accountService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            var isUpdated = await _accountService.UpdateUserNameAsync(user.Id, newUsername);
            return isUpdated ? Ok() : BadRequest("Failed to update username");
        }

        [HttpPost("send-confirmation-email")]
        [AllowAnonymous]
        public async Task<IActionResult> SendConfirmationEmail([FromForm] string email) {
            var user = await _accountService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            var isSent = await _accountService.SendConfirmationEmailAsync(email, user);
            return isSent ? Ok("Mail sent") : BadRequest("Failed to send confirmation email");
        }

        [HttpGet("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token) {
            if (token == null) {
                return BadRequest("token == null");
            }

            var user = await _accountService.GetUserByIdAsync(userId);
            if (user == null) {
                return NotFound("Not found user");
            }

            var result = await _accountService.ConfirmEmailAsync(userId, token);

            if (result.Succeeded) {
                return Redirect("http://localhost:3000/account/confirmedEmail/");
            }
            else {
                foreach (var error in result.Errors) {
                    Console.WriteLine(error.Description);
                }

                return BadRequest("Error");
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.NewPassword != model.ConfirmNewPassword)
                return BadRequest("New password and confirmation do not match");

            //Console.WriteLine(JsonDocument.Parse(User).ToString());

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound("User not found");

            var (succeeded, errors) = await _accountService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (!succeeded) {
                return BadRequest(new { Errors = errors });
            }

            return Ok("Password changed successfully");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> StartResetPassword([FromBody] ValueModel valueModel) {
            var email = valueModel.Value!;
            var (success, message) = await _accountService.StartResetPasswordAsync(email);
            return success ? Ok("Reset password email sent") : BadRequest(message);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("confirm-reset-password")]
        public async Task<IActionResult> ConfirmResetPassword(string email, string token, string newPassword) {
            var result = await _accountService.ConfirmResetPasswordAsync(email, token, newPassword);
            return result.Succeeded ? Ok("Password reset successfully") : BadRequest("Failed to reset password");
        }
    }
}
