using BackendGameVibes.IServices;
using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.DTOs.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;


namespace BackendGameVibes.Controllers {
    [ApiController]
    [Route("account")]
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
                else if (loginResult!.IsLockedOut) {
                    var timeToEndLockout = user.LockoutEnd!.Value.Subtract(DateTime.Now);

                    await _accountService.SendLockedOutAccountEmailAsync(model.Email, user!);
                    return StatusCode(471, timeToEndLockout.TotalMinutes);
                }
            }

            return Unauthorized("Invalid login attempt");
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation("Zwraca informacje o uzytkowniku")]
        public async Task<IActionResult> GetAccountInfo() {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            var accountInfo = await _accountService.GetAccountInfoAsync(userId);
            if (accountInfo != null)
                return Ok(accountInfo);
            else
                return BadRequest("User not found");
        }

        [HttpPatch("change-username")]
        [Authorize]
        [SwaggerOperation("Require authorization")]
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

        [HttpPost("change-password")]
        [Authorize]
        [SwaggerOperation("Require authorization")]
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

        [HttpPost("change-profile-picture")]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile profilePicture) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated");

            if (profilePicture == null || profilePicture.Length == 0)
                return BadRequest("InvalidProfilePicture");

            if (profilePicture.FileName.EndsWith("png") || profilePicture.FileName.EndsWith("jpg")) {
                using (var ms = new MemoryStream()) {
                    await profilePicture.CopyToAsync(ms);
                    var imageData = ms.ToArray();

                    var result = await _accountService.UpdateProfilePictureAsync(userId, imageData);
                    return result ? Ok("ProfilePictureUpdated") : BadRequest("FailedToUpdateProfilePicture");
                }
            }
            else {
                return BadRequest("InvalidProfilePictureType");
            }
        }

        [HttpPost("change-profile-desc")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileDescription([FromBody] ValueModel valueModel) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated");

            if (string.IsNullOrWhiteSpace(valueModel.Value))
                return BadRequest("InvalidNewDescription");

            var result = await _accountService.UpdateProfileDescriptionAsync(userId, valueModel.Value);
            return result ? Ok("ProfileDescriptionUpdated") : BadRequest("FailedToUpdateProfileDescription");

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

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation("publiczne dane o profilu gracza")]
        public async Task<ActionResult<object>> GetUserAsync(string userId) {
            var user = await _accountService.GetUserByIdAsync(userId);
            if (user == null) {
                return NotFound();
            }
            else {
                var accountInfo = await _accountService.GetPublicAccountInfoAsync(user.Id);
                return Ok(accountInfo);
            }
        }

        [HttpGet("user/search:nick")]
        [SwaggerOperation("Require authorization >=user. Wyszukiwanie mozliwych uzytkownikow do dodania znajomych po nicku, (bez modów i adminów)")]
        [Authorize]
        public async Task<ActionResult<object>> SearchUserAsync(string nick) {
            string myNickname = User.Identity!.Name!;
            string myUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var users = await _accountService.FindUsersNickAndIdsByNickname(myUserId, myNickname, nick);
            if (users == null) {
                return NotFound();
            }
            else {
                return Ok(users);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("user/friends")]
        [SwaggerOperation("Require authorization >=user. Zwraca liste znajomych uzytkownika")]
        public async Task<ActionResult<object>> GetAllFriendsOfUser() {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            var friends = await _accountService.GetAllFriendsOfUser(userId);
            return Ok(friends);
        }

        [HttpGet]
        [Authorize]
        [Route("user/friend-requests")]
        [SwaggerOperation("Require authorization >=user. Zwraca liste zaproszen do znajomych")]
        public async Task<ActionResult<object>> GetFriendRequests() {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            var friendRequests = await _accountService.GetFriendRequestsForUser(userId);
            return Ok(friendRequests);
        }

        [HttpPost("user/send-friend-request:{receiverUserId}")]
        [Authorize]
        [SwaggerOperation("autoryzowany uzytkownik wysyla zaproszenie. 200 - wyslano, 201 - zaproszenie wyslane wczesniej i oczekuje na odpowiedz, 202 - Juz sa znajomymi")]
        public async Task<IActionResult> SendFriendRequestAsync(string receiverUserId) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            (bool success, bool? isExistingFriend, FriendRequest? friendRequest) = await _accountService.SendFriendRequestAsync(userId, receiverUserId);
            if (success && isExistingFriend == false)
                return StatusCode(200, "Zaproszenie wysłane");
            else if (success == false && isExistingFriend == false && friendRequest != null && friendRequest!.IsAccepted == null)
                return StatusCode(201, "Zaproszenie pomiedzy uzytkonwikami wyslane juz wczesniej. nie sa aktualnie znajomymi");
            else if (success == false && isExistingFriend == true)
                return StatusCode(202, "Juz sa znajomymi");
            else if (success == false && friendRequest != null && friendRequest!.IsAccepted == false)
                return StatusCode(203, "Zaproszony znajomy odrzucil zaproszenie. uwaga: uzytkownik nadal moze zaakceptowac zaproszenie");
            else
                return BadRequest("error");
        }

        [HttpPost("user/confirm-friend-request:{receiverUserId}")]
        [SwaggerOperation("autoryzowany uzytkownik akceptuje zaproszenie")]
        [Authorize]
        public async Task<IActionResult> ConfirmFriendRequestAsync(string receiverUserId) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            bool success = await _accountService.ConfirmFriendRequestAsync(userId, receiverUserId);
            if (success)
                return Ok("Zaproszenie zaakceptowane");
            else
                return BadRequest("Brak aktualnego zaproszenia");
        }

        [HttpPost("user/revoke-friend-request:{receiverUserId}")]
        [SwaggerOperation("autoryzowany uzytkownik odrzuca zaproszenie")]
        [Authorize]
        public async Task<IActionResult> RevokeFriendRequestAsync(string receiverUserId) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            bool success = await _accountService.RevokeFriendRequestAsync(userId, receiverUserId);
            if (success)
                return Ok("Zaproszenie odrzucone");
            else
                return BadRequest("Brak aktualnego zaproszenia");
        }

        [HttpDelete("user/remove-friend:{friendId}")]
        [SwaggerOperation("autoryzowany uzytkownik usuwa znajomego")]
        [Authorize]
        public async Task<IActionResult> RemoveFriendAsync(string friendId) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            bool success = await _accountService.RemoveFriendAsync(userId, friendId);
            if (success)
                return Ok("Usunięto znajomego");
            else
                return BadRequest("Brak znajomego");
        }
    }
}
