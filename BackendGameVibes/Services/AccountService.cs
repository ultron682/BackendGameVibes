using BackendGameVibes.Data;
using BackendGameVibes.Helpers;
using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace BackendGameVibes.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserGameVibes> _userManager;
        private readonly SignInManager<UserGameVibes> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mail_Service;
        private readonly HtmlTemplateService _htmlTemplateService;


        public AccountService(ApplicationDbContext context, UserManager<UserGameVibes> userManager,
            SignInManager<UserGameVibes> signInManager, IConfiguration configuration, IMailService mail_Service, HtmlTemplateService htmlTemplateService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mail_Service = mail_Service;
            _htmlTemplateService = htmlTemplateService;
        }

        public async Task<IdentityResult> RegisterUser(RegisterRequestCodeShare model)
        {
            var user = new UserGameVibes { UserName = model.UserName, Email = model.Email };
            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<UserGameVibes?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<string> GenerateJwtToken(UserGameVibes user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var token = JwtTokenGenerator.GenerateToken(user.Email!, user.UserName!, key, _configuration["Jwt:Issuer"]!, _configuration["Jwt:Audience"]!);
            return token;
        }

        public async Task<SignInResult?> LoginUser(UserGameVibes user, string password)
        {
            return await _signInManager.PasswordSignInAsync(user.UserName!, password, true, false);
        }

        public async Task<UserGameVibes?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }



        public async Task<object> GetAccountInfoAsync(string userId)
        {
            var accountInfo = await _context.Users
                .Where(u => u.Id == userId)
                //.Include(u => u.CodeSnippets)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.UserName,
                    u.EmailConfirmed,
                    //CodeSnippets = u.CodeSnippets.Select(cs => new
                    //{
                    //    cs.UniqueId,
                    //    Code = cs.Code.Length > 20 ? cs.Code.Substring(0, 20) : cs.Code
                    //}).ToArray()
                })
                .FirstOrDefaultAsync();

            return accountInfo!;
        }

        public async Task<bool> UpdateUserNameAsync(string userId, string newUsername)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.UserName = newUsername;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> SendConfirmationEmail(string email, UserGameVibes user)
        {
            if (user == null || email == null)
                return false;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);
            var ConfirmationLink = $"http://localhost:5555/account/confirm?userId={user.Id}&token={token}";

            Console.WriteLine($"Please confirm your account by <a href='{ConfirmationLink!}'>clicking here</a>.");

            string emailBody = await _htmlTemplateService.GetEmailTemplateAsync("wwwroot/EmailTemplates/confirm_email_template.html", new Dictionary<string, string>
            {
                { "ConfirmationLink", ConfirmationLink! },
                { "UserName", user.UserName! }
            });

            _mail_Service.SendMail(new MailData()
            {
                EmailBody = emailBody,
                EmailSubject = "Confirm your account",
                EmailToId = email,
                EmailToName = user.UserName!
            });
            return true;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            token = Uri.UnescapeDataString(token);

            return await _userManager.ConfirmEmailAsync(user, token);
        }
    }
}
