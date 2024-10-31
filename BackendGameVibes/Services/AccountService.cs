using BackendGameVibes.Data;
using BackendGameVibes.Helpers;
using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.Points;
using BackendGameVibes.Models.Requests.Account;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;


namespace BackendGameVibes.Services {
    public class AccountService : IAccountService {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserGameVibes> _userManager;
        private readonly SignInManager<UserGameVibes> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly MailService _mail_Service;
        private readonly HtmlTemplateService _htmlTemplateService;
        private readonly PointsSettings _pointsSettings;

        public AccountService(ApplicationDbContext context, UserManager<UserGameVibes> userManager,
            SignInManager<UserGameVibes> signInManager, IConfiguration configuration, MailService mail_Service,
            HtmlTemplateService htmlTemplateService, RoleManager<IdentityRole> roleManager,
            IOptions<PointsSettings> pointsSettings) {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mail_Service = mail_Service;
            _htmlTemplateService = htmlTemplateService;
            _roleManager = roleManager;
            _pointsSettings = pointsSettings.Value;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDTO model) {
            var user = new UserGameVibes { UserName = model.UserName, Email = model.Email };
            if (user.ProfilePicture == null) {
                var defaultImagePath = Path.Combine("wwwroot/Images", "default-profile.jpg");
                var newProfilePicture = new ProfilePicture { ImageData = await File.ReadAllBytesAsync(defaultImagePath) };

                user.ProfilePicture = newProfilePicture;
            }

            IdentityResult userResult = await _userManager.CreateAsync(user, model.Password);

            if (userResult.Succeeded)
                await _userManager.AddToRoleAsync(user, "user");

            return userResult;
        }

        public async Task<UserGameVibes?> GetUserByEmailAsync(string email) {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<string> GenerateJwtTokenAsync(UserGameVibes user) {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var roles = await _userManager.GetRolesAsync(user);
            var rolesString = string.Join(",", roles);
            var token = JwtTokenGenerator.GenerateToken(user.Email!, user.UserName!, user.Id, rolesString, key, _configuration["Jwt:Issuer"]!, _configuration["Jwt:Audience"]!);
            return token;
        }

        public async Task<SignInResult?> LoginUserAsync(UserGameVibes user, string password) {
            return await _signInManager.PasswordSignInAsync(user.UserName!, password, true, true);
        }

        public async Task<UserGameVibes?> GetUserByIdAsync(string userId) {
            return await _userManager.FindByIdAsync(userId);
        }

        // unused method todo: remove?
        public async Task SaveTokenToDbAsync(IdentityUserToken<string> identityUserToken) {
            _context.UserTokens.Add(identityUserToken);
            await _context.SaveChangesAsync();
        }

        public async Task<object?> GetAccountInfoAsync(string userId) {
            var userGameVibes = await GetUserByIdAsync(userId);
            if (userGameVibes == null)
                return null;

            var roles = await _userManager.GetRolesAsync(userGameVibes);

            var accountInfo = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.UserReviews)
                .Include(u => u.ForumRole)
                .Include(u => u.ProfilePicture)
                .Select(u => new {
                    u.Id,
                    u.Email,
                    u.EmailConfirmed,
                    u.UserName,
                    ProfilePictureBlob = u.ProfilePicture != null ? u.ProfilePicture.ImageData : null,
                    ForumRole = new { u.ForumRole!.Id, u.ForumRole.Name, u.ForumRole.Threshold },
                    u.ExperiencePoints,
                    Roles = roles.ToArray(),
                    Reviews = u.UserReviews.Select(r => new {
                        r.Id,
                        r.GameId,
                        r.GeneralScore,
                        r.GameplayScore,
                        r.GraphicsScore,
                        r.AudioScore,
                        r.Comment,
                        r.CreatedAt
                    }).ToArray(),
                    UserReportedReviews = u.UserReportedReviews.Select(rr => new {
                        rr.Id,
                        rr.ReporterUserId,
                        ReporterUserName = rr.ReporterUser!.UserName,
                        rr.ReviewId,
                        rr.Reason
                    }).ToArray(),
                    Friends = u.UserFriends.Select(f => new {
                        f.FriendId,
                        f.FriendUser!.UserName,
                        f.FriendsSince
                    }).ToArray(),
                    FriendRequestsReceived = u.UserFriendRequestsReceived.Select(fr => new {
                        fr.Id,
                        SenderId = fr.SenderUserId,
                        SenderName = fr.SenderUser!.UserName,
                        fr.IsAccepted
                    }).ToArray(),
                    FriendRequestsSent = u.UserFriendRequestsSent.Select(fr => new {
                        fr.Id,
                        ReceiverId = fr.ReceiverUserId,
                        ReceiverName = fr.ReceiverUser!.UserName,
                        fr.IsAccepted
                    }).ToArray(),
                    UserForumPosts = u.UserForumPosts!.Select(p => new {
                        p.Id,
                        p.Content,
                        p.CreatedDateTime,
                        p.LastUpdatedDateTime,
                        p.Likes,
                        p.DisLikes,
                        p.ThreadId
                    }).ToArray(),
                    UserReportedPosts = u.UserReportedPosts!.Select(rp => new {
                        rp.Id,
                        rp.ReporterUserId,
                        ReporterUserName = rp.ReporterUser!.UserName,
                        rp.ForumPostId,
                        rp.Reason
                    }).ToArray(),
                    UserFollowedGames = u.UserFollowedGames!.Select(g => new {
                        g.Id,
                        g.Title
                    }).ToArray()
                })
                .FirstOrDefaultAsync();

            return accountInfo!;
        }

        public async Task<object?> GetPublicAccountInfoAsync(string userId) {
            var userGameVibes = await GetUserByIdAsync(userId);
            if (userGameVibes == null)
                return null;

            var roles = await _userManager.GetRolesAsync(userGameVibes);

            var accountInfo = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.UserReviews)
                .Include(u => u.ForumRole)
                .Include(u => u.ProfilePicture)
                .Select(u => new {
                    u.Id,
                    u.UserName,
                    ProfilePictureBlob = u.ProfilePicture != null ? u.ProfilePicture.ImageData : null,
                    ForumRole = new { u.ForumRole!.Id, u.ForumRole.Name, u.ForumRole.Threshold },
                    u.ExperiencePoints,
                    Reviews = u.UserReviews.Select(r => new {
                        r.Id,
                        r.GameId,
                        r.GeneralScore,
                        r.GameplayScore,
                        r.GraphicsScore,
                        r.AudioScore,
                        r.Comment,
                        r.CreatedAt
                    }).ToArray(),
                    Friends = u.UserFriends.Select(f => new {
                        f.FriendId,
                        f.FriendUser!.UserName,
                        f.FriendsSince
                    }).ToArray(),
                    UserForumPosts = u.UserForumPosts!.Select(p => new {
                        p.Id,
                        p.Content,
                        p.CreatedDateTime,
                        p.LastUpdatedDateTime,
                        p.Likes,
                        p.DisLikes,
                        p.ThreadId
                    }).ToArray(),
                    UserFollowedGames = u.UserFollowedGames!.Select(g => new {
                        g.Id,
                        g.Title
                    }).ToArray()
                })
                .FirstOrDefaultAsync();

            return accountInfo!;
        }

        public async Task<bool> UpdateUserNameAsync(string userId, string newUsername) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return false;
            }

            user.UserName = newUsername;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> SendConfirmationEmailAsync(string email, UserGameVibes user) {
            if (user == null || email == null)
                return false;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);
            var ConfirmationLink = $"http://localhost:5556/account/confirm?userId={user.Id}&token={token}";

            Console.WriteLine($"Please confirm your account by <a href='{ConfirmationLink!}'>clicking here</a>.");

            string emailBody = await _htmlTemplateService.GetEmailTemplateAsync("wwwroot/EmailTemplates/confirm_email_template.html", new Dictionary<string, string>
            {
                { "ConfirmationLink", ConfirmationLink! },
                { "UserName", user.UserName! }
            });

            _mail_Service.SendMail(new MailData() {
                EmailBody = emailBody,
                EmailSubject = "Confirm your account",
                EmailToId = email,
                EmailToName = user.UserName!
            });
            return true;
        }

        public async Task<bool> SendLockedOutAccountEmailAsync(string email, UserGameVibes user) {
            if (user == null || email == null)
                return false;

            string emailBody = await _htmlTemplateService.GetEmailTemplateAsync("wwwroot/EmailTemplates/account_locked_out_template.html", new Dictionary<string, string>
            {
                { "UserName", user.UserName! }
            });

            _mail_Service.SendMail(new MailData() {
                EmailBody = emailBody,
                EmailSubject = "Your account is locked",
                EmailToId = email,
                EmailToName = user.UserName!
            });
            return true;
        }


        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            token = Uri.UnescapeDataString(token);

            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return (false, new[] { "User not found" });
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded) {
                return (false, result.Errors.Select(e => e.Description));
            }

            await _signInManager.RefreshSignInAsync(user);

            return (true, Enumerable.Empty<string>());
        }

        public async Task<(bool, string)> StartResetPasswordAsync(string email) {
            if (email == null || string.IsNullOrWhiteSpace(email))
                return (false, "No email data");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "No user found");

            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(resetToken))
                return (false, "Error while generating reset token");

            resetToken = Uri.EscapeDataString(resetToken);
            var ConfirmationLink = $"http://localhost:3000/account/reset?userId={user.Id}&token={resetToken}";

            Console.WriteLine($"Reset password link: <a href='{ConfirmationLink!}'>clicking here</a>.");

            string emailBody = await _htmlTemplateService.GetEmailTemplateAsync("wwwroot/EmailTemplates/reset_password.html", new Dictionary<string, string>
            {
                { "ConfirmationLink", ConfirmationLink! },
                { "UserName", user.UserName! }
            });

            _mail_Service.SendMail(new MailData() {
                EmailBody = emailBody,
                EmailSubject = "Reset password for account",
                EmailToId = email,
                EmailToName = user.UserName!
            });

            return (true, "Ok");
        }

        public async Task<IdentityResult> ConfirmResetPasswordAsync(string email, string resetToken, string newPassword) {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            resetToken = Uri.UnescapeDataString(resetToken);

            return await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        }

        public async Task<object[]> FindUsersNickAndIdsByNickname(string myUserId, string myNickname, string searchName) {
            var allStandardUsers = await _userManager.GetUsersInRoleAsync("user");

            myNickname = myNickname.ToUpper();
            searchName = searchName.ToUpper();

            var othersUsers = allStandardUsers
                 .Where(u =>
                 u.NormalizedUserName!.Contains(searchName)
                 && u.NormalizedUserName != myNickname)
                 .Select(u => new {
                     u.Id,
                     u.UserName
                 })
                 .ToArray();

            var resultWithIsFriend = othersUsers.Select(u => new {
                u.Id,
                u.UserName,
                IsFriend = _context.Friends.Any(f => f.UserId == myUserId && f.FriendId == u.Id)
            }).ToArray();

            return resultWithIsFriend;
        }

        public async Task<IEnumerable<object>> GetAllFriendsOfUser(string userId) {
            var friends = await _context.Friends
             .Where(f => f.UserId == userId)
             .Select(f => new {
                 f.FriendId,
                 f.FriendUser!.UserName,
                 f.FriendsSince
             })
             .ToArrayAsync();

            return friends;
        }

        private async Task<bool> SendNewFriendRequestEmailAsync(string email, string userSenderNick) {
            if (userSenderNick == null || email == null)
                return false;

            string emailBody = await _htmlTemplateService.GetEmailTemplateAsync("wwwroot/EmailTemplates/new_friend_request.html",
            new Dictionary<string, string>
            {
                { "UserName", userSenderNick }
            });

            _mail_Service.SendMail(new MailData() {
                EmailBody = emailBody,
                EmailSubject = "New friend request",
                EmailToId = email,
                EmailToName = userSenderNick
            });
            return true;
        }

        public async Task<(bool, bool, FriendRequest?)> SendFriendRequestAsync(string senderId, string receiverId) {
            var existingRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => (fr.SenderUserId == senderId && fr.ReceiverUserId == receiverId)
                ||
                (fr.SenderUserId == receiverId && fr.ReceiverUserId == senderId));

            bool isExistingFriends = await _context.Friends
                .AnyAsync(f => f.UserId == senderId && f.FriendId == receiverId);

            if (existingRequest == null && isExistingFriends == false) {
                var friendRequest = new FriendRequest {
                    SenderUserId = senderId,
                    ReceiverUserId = receiverId
                };

                var userSender = await _userManager.FindByIdAsync(senderId);
                var userReceiver = await _userManager.FindByIdAsync(receiverId);

                if (userSender != null && userReceiver != null)
                    await SendNewFriendRequestEmailAsync(userReceiver!.Email!, userSender!.UserName!);

                _context.FriendRequests.Add(friendRequest);
                await _context.SaveChangesAsync();
                return (true, isExistingFriends, existingRequest);
            }
            else {
                return (false, isExistingFriends, existingRequest);
            }
        }

        public async Task<bool> ConfirmFriendRequestAsync(string userId, string friendId) {
            var friendRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderUserId == friendId && fr.ReceiverUserId == userId && (fr.IsAccepted == null || fr.IsAccepted == false));

            if (friendRequest != null) {
                friendRequest.IsAccepted = true;

                var friend1 = new Friend { UserId = userId, FriendId = friendId };
                var friend2 = new Friend { UserId = friendId, FriendId = userId };

                _context.FriendRequests.Update(friendRequest);
                _context.Friends.AddRange(friend1, friend2);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RevokeFriendRequestAsync(string userId, string friendId) {
            var friendRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderUserId == friendId && fr.ReceiverUserId == userId && fr.IsAccepted == null);

            if (friendRequest != null) {
                friendRequest.IsAccepted = false;
                _context.FriendRequests.Update(friendRequest);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveFriendAsync(string userId, string friendId) {
            var friendRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr =>
                (
                (fr.SenderUserId == userId && fr.ReceiverUserId == friendId)
                ||
                (fr.SenderUserId == friendId && fr.ReceiverUserId == userId)
                )
                && fr.IsAccepted == true);

            var friend1 = await _context.Friends
             .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);

            var friend2 = await _context.Friends
                .FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId);

            if (friendRequest != null)
                _context.FriendRequests.Remove(friendRequest);

            if (friend1 != null && friend2 != null) {
                _context.Friends.Remove(friend1);
                _context.Friends.Remove(friend2);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<object>> GetFriendRequestsForUser(string userId) {
            var friendRequests = await _context.FriendRequests
                .Where(fr => fr.ReceiverUserId == userId && (fr.IsAccepted == null || fr.IsAccepted == false))
                .Select(fr => new {
                    fr.Id,
                    SenderId = fr.SenderUserId,
                    SenderName = fr.SenderUser!.UserName,
                    fr.IsAccepted
                })
                .ToArrayAsync();

            return friendRequests;
        }

        public async Task<bool> UpdateProfilePictureAsync(string userId, byte[] imageData) {
            var user = await _context.Users
                .Include(u => u.ProfilePicture)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) {
                return false;
            }
            user.ProfilePicture!.ImageData = imageData;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public void Dispose() {
            _context.Dispose();
            _userManager.Dispose();
            _roleManager.Dispose();
        }
    }
}
