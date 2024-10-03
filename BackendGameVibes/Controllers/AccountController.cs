﻿using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace BackendGameVibes.Controllers
{
    [ApiController]
    [Route("account")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestGameVibes model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.RegisterUser(model);

            if (result.Succeeded)
            {
                var user = await _accountService.GetUserByEmailAsync(model.Email);
                await _accountService.SendConfirmationEmail(model.Email, user);
                return Ok("UserRegisteredSuccessfully");
            }

            foreach (var error in result.Errors)
            {
                return error.Code switch
                {
                    "DuplicateUserName" => StatusCode(450, "Username already taken"),
                    "DuplicateEmail" => StatusCode(452, "Email already taken"),
                    _ => StatusCode(454, error.Code)
                };
            }

            return BadRequest(ModelState);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestGameVibes model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _accountService.GetUserByEmailAsync(model.Email);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    return StatusCode(470, "Email unconfirmed");
                }

                var loginResult = await _accountService.LoginUser(user, model.Password);
                if (loginResult != null && loginResult.Succeeded)
                {
                    var token = await _accountService.GenerateJwtToken(user);

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
        public async Task<IActionResult> GetAccountInfo()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return Unauthorized("User not authenticated");

            var user = await _accountService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            var accountInfo = await _accountService.GetAccountInfoAsync(user.Id, user);
            return Ok(accountInfo);
        }

        [HttpPatch("nickname")]
        [Authorize]
        public async Task<IActionResult> ChangeNickname([FromForm] string newUsername)
        {
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

        [AllowAnonymous]
        [HttpPost("send-confirmation-email")]
        public async Task<IActionResult> SendConfirmationEmail([FromForm] string email)
        {
            var user = await _accountService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            var isSent = await _accountService.SendConfirmationEmail(email, user);
            return isSent ? Ok("Mail sent") : BadRequest("Failed to send confirmation email");
        }


        [AllowAnonymous]
        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (token == null)
            {
                return BadRequest("token == null");
            }

            var user = await _accountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Not found user");
            }

            var result = await _accountService.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
            {
                return Redirect("http://localhost:3000/account/confirmedEmail/");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }

                return BadRequest("Error");
            }
        }
    }
}
