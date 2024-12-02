using BackendGameVibes.Data;
using BackendGameVibes.Helpers;
using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.Points;
using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using System.Data;


namespace BackendGameVibes.Services {
    public class AccountService : IAccountService {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserGameVibes> _userManager;
        private readonly SignInManager<UserGameVibes> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MailService _mail_Service;
        private readonly HtmlTemplateService _htmlTemplateService;
        private readonly IForumExperienceService _forumExperienceService;
        private readonly IActionCodesService _actionCodesService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly HttpClient _httpClient;

        public AccountService(ApplicationDbContext context, UserManager<UserGameVibes> userManager,
            SignInManager<UserGameVibes> signInManager, MailService mail_Service,
            HtmlTemplateService htmlTemplateService, RoleManager<IdentityRole> roleManager,
            IForumExperienceService forumExperienceService, IActionCodesService actionCodesService,
            IJwtTokenService jwtTokenService, HttpClient httpClient) {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _mail_Service = mail_Service;
            _htmlTemplateService = htmlTemplateService;
            _roleManager = roleManager;
            _forumExperienceService = forumExperienceService;
            _actionCodesService = actionCodesService;
            _jwtTokenService = jwtTokenService;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDTO model) {
            var user = new UserGameVibes { UserName = model.UserName, Email = model.Email };
            if (user.ProfilePicture == null) {
                try {
                    string randomColorHex = new Random().Next(0, 16777215).ToString("X");
                    var defaultProfilePictureUrl = $"https://ui-avatars.com/api/?background={randomColorHex}&bold=true&size=128&color=fff&name=" + model.UserName;
                    var defaultProfilePicture = await _httpClient.GetAsync(defaultProfilePictureUrl, CancellationToken.None);
                    var profilePictureBlob = await defaultProfilePicture.Content.ReadAsByteArrayAsync();

                    user.ProfilePicture = new ProfilePicture { ImageData = profilePictureBlob, ImageFormat = "image/png" };
                }
                catch (Exception) {
                    Console.WriteLine("Error while downloading default profile picture");
                    ProfilePicture newProfilePicture = await GetDefaultProfilePicture();

                    user.ProfilePicture = newProfilePicture;
                }
            }

            IdentityResult userResult = await _userManager.CreateAsync(user, model.Password);

            if (userResult.Succeeded)
                await _userManager.AddToRoleAsync(user, "user");

            return userResult;
        }

        private static async Task<ProfilePicture> GetDefaultProfilePicture() {
            var defaultImagePath = Path.Combine("wwwroot/Images", "default-profile.jpg");
            var newProfilePicture = new ProfilePicture { ImageData = await File.ReadAllBytesAsync(defaultImagePath), ImageFormat = "image/blob", UploadedDate = DateTime.Now };
            return newProfilePicture;
        }

        public async Task<UserGameVibes?> GetUserByEmailAsync(string email) {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<UserGameVibes?> GetUserByIdAsync(string userId) {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<(string, string[])> GenerateJwtTokenAsync(UserGameVibes user) {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            var rolesString = string.Join(",", roles);

            var token = _jwtTokenService.GenerateToken(user.Email!,
                user.UserName!,
                user.Id,
                rolesString
            );

            return (token, roles.ToArray());
        }

        public (string? email, string? username, string? userId, string? role) GetDataFromJwtTokenAsync(string accessToken) {
            return _jwtTokenService.GetTokenClaims(accessToken);
        }


        public async Task<SignInResult?> LoginUserAsync(UserGameVibes user, string password) {
            return await _signInManager.PasswordSignInAsync(user.UserName!, password, true, true);
        }

        // unused method todo: remove?
        public async Task SaveTokenToDbAsync(IdentityUserToken<string> identityUserToken) {
            _context.UserTokens.Add(identityUserToken);
            await _context.SaveChangesAsync();
        }

        public async Task<object?> GetBasicAccountInfoAsync(string userId) {
            var userGameVibes = await GetUserByIdAsync(userId);
            if (userGameVibes == null)
                return null;

            var userRoles = await _userManager.GetRolesAsync(userGameVibes);

            var accountInfo = await _context.Users
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
                    u.PhoneNumber,
                    u.PhoneNumberConfirmed,
                    u.TwoFactorEnabled,
                    u.LockoutEnd,
                    u.LockoutEnabled,
                    u.AccessFailedCount,
                    u.Description,
                    u.LastActivityDate,
                    Roles = userRoles.ToArray(),
                })
                .FirstOrDefaultAsync(u => u.Id == userGameVibes.Id);

            return accountInfo!;
        }

        public async Task<object?> GetDetailedAccountInfoAsync(string userId) {
            var userGameVibes = await GetUserByIdAsync(userId);
            if (userGameVibes == null)
                return null;

            var userRoles = await _userManager.GetRolesAsync(userGameVibes);

            var accountInfo = await _context.Users
                .Where(u => u.Id == userGameVibes.Id)
                .Include(u => u.UserReviews)
                .Include(u => u.ForumRole)
                .Include(u => u.UserReportedReviews)
                .Include(u => u.UserFriendRequestsReceived)
                .Include(u => u.UserFriendRequestsSent)
                .Include(u => u.UserForumThreads)
                .Include(u => u.UserForumPosts)
                .Include(u => u.UserReportedPosts)
                .Include(u => u.UserFollowedGames)
                .Select(u => new {
                    u.Id,
                    Reviews = u.UserReviews.Select(r => new {
                        r.Id,
                        r.GeneralScore,
                        r.GameplayScore,
                        r.GraphicsScore,
                        r.AudioScore,
                        r.Comment,
                        r.CreatedAt,
                        r.UpdatedAt,
                        r.GameId,
                        GameTitle = r.Game != null ? r.Game.Title : "NoData",
                        GameCoverImageUrl = r.Game != null ? r.Game.CoverImage : "NoData",
                        UserForumRoleName = r.UserGameVibes != null ? r.UserGameVibes.ForumRole!.Name : "NoRank",
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
                    UserForumThreads = u.UserForumThreads != null ? u.UserForumThreads.Select(t => new {
                        t.Id,
                        t.Title,
                        t.CreatedDateTime,
                        t.LastUpdatedDateTime
                    })
                    .ToArray()
                    : Array.Empty<object>(),
                    UserForumPosts = u.UserForumPosts!
                        .Select(p => new {
                            p.Id,
                            p.Content,
                            p.CreatedDateTime,
                            p.LastUpdatedDateTime,
                            p.LikesCount,
                            p.DisLikesCount,
                            p.ThreadId,
                            threadTitle = p.Thread != null ? p.Thread.Title : "NoData"
                        }).ToArray(),
                    UserReportedPosts = u.UserReportedPosts.Select(rp => new {
                        rp.Id,
                        rp.ReporterUserId,
                        ReporterUserName = rp.ReporterUser!.UserName,
                        rp.ForumPostId,
                        rp.Reason
                    }).ToArray(),
                    UserFollowedGames = u.UserFollowedGames != null ? u.UserFollowedGames.Select(g => new {
                        g.Id,
                        g.Title
                    }).ToArray() : Array.Empty<object>(),
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
                    u.Description,
                    u.LastActivityDate,
                    Reviews = u.UserReviews.Select(r => new {
                        r.Id,
                        r.GeneralScore,
                        r.GameplayScore,
                        r.GraphicsScore,
                        r.AudioScore,
                        r.Comment,
                        r.CreatedAt,
                        r.UpdatedAt,
                        r.GameId,
                        GameTitle = r.Game != null ? r.Game.Title : "NoData",
                        GameCoverImageUrl = r.Game != null ? r.Game.CoverImage : "NoData",
                        UserForumRoleName = r.UserGameVibes != null ? r.UserGameVibes.ForumRole!.Name : "NoRank",
                    }).ToArray(),
                    Friends = u.UserFriends.Select(f => new {
                        f.FriendId,
                        f.FriendUser!.UserName,
                        f.FriendsSince
                    }).ToArray(),
                    UserForumThreads = u.UserForumThreads != null ? u.UserForumThreads.Select(t => new {
                        t.Id,
                        t.Title,
                        t.CreatedDateTime,
                        t.LastUpdatedDateTime
                    }).OrderByDescending(p => p.LastUpdatedDateTime).Take(5).ToArray() : Array.Empty<object>(),
                    UserForumPosts = u.UserForumPosts!
                        .Select(p => new {
                            p.Id,
                            p.Content,
                            p.CreatedDateTime,
                            p.LastUpdatedDateTime,
                            p.LikesCount,
                            p.DisLikesCount,
                            p.ThreadId,
                            threadTitle = p.Thread != null ? p.Thread.Title : "NoData"
                        }).ToArray(),
                    UserFollowedGames = u.UserFollowedGames != null ? u.UserFollowedGames.Select(g => new {
                        g.Id,
                        g.Title
                    }).ToArray() : Array.Empty<object>(),
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

                await _forumExperienceService.AddNewFriendPoints(userId);

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
            user.ProfilePicture!.ImageFormat = "image/blob";
            user.ProfilePicture!.ImageData = imageData;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateProfileDescriptionAsync(string userId, string description) {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) {
                return false;
            }
            user.Description = description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SendGeneralEmailToUserAsync(UserGameVibes user, string subject, string message) {

            if (user == null || subject == null || message == null || user.Email == null)
                return false;

            string emailBody = await _htmlTemplateService.GetEmailTemplateAsync("wwwroot/EmailTemplates/user-message.html",
            new Dictionary<string, string>
            {
                { "message", message },
                 { "subject", subject }
            });

            _mail_Service.SendMail(new MailData() {
                EmailBody = emailBody,
                EmailSubject = "New friend request",
                EmailToId = user.Email!,
                EmailToName = user.UserName!
            });
            return true;
        }

        public async Task<(ActionCode? actionCode, bool isAlreadyExistValidExpiryDate)> SendCloseAccountRequestAsync(string userId) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) {
                return (null, false);
            }

            (ActionCode? actionCode, bool isAlreadyExistValidExpiryDate) = await _actionCodesService.GenerateUniqueActionCode(user.Id);

            if (isAlreadyExistValidExpiryDate == false) {
                string emailBody = await _htmlTemplateService.GetEmailTemplateAsync("wwwroot/EmailTemplates/close_account.html",
                new Dictionary<string, string>
                {
                { "UserName", user.UserName! },
                { "ConfirmationCode", actionCode!.Code! }
                });

                _mail_Service.SendMail(new MailData() {
                    EmailBody = emailBody,
                    EmailSubject = "Close account request",
                    EmailToId = user.Email!,
                    EmailToName = user.UserName!
                });
                return (actionCode, false);
            }
            else {
                return (actionCode, true);
            }
        }

        public async Task<bool> ConfirmCloseAccountRequest(string userId, string confirmationCode) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var actionCode = await _context.ActiveActionCodes.FirstOrDefaultAsync(ac => ac.Code == confirmationCode && ac.UserId == userId);

            if (user == null || actionCode == null) {
                return false;
            }

            var deleteResult = await _userManager.DeleteAsync(user);

            if (deleteResult.Succeeded) {
                await _actionCodesService.RemoveActionCode(actionCode.Code!);
            }

            return deleteResult.Succeeded;
        }

        public async Task<bool> FollowGameAsync(string userId, int gameId) {
            var user = await _context.Users
                .Include(u => u.UserFollowedGames)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);

            if (user != null && game != null) {
                if (user.UserFollowedGames!.FirstOrDefault(g => g.Id == gameId) == null) {
                    user.UserFollowedGames!.Add(game);
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
                return true;
            }

            return false;
        }

        public async Task<bool> UnfollowGameAsync(string userId, int gameId) {
            var user = await _context.Users
                .Include(u => u.UserFollowedGames)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);

            if (user != null && game != null) {
                user.UserFollowedGames!.Remove(game);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<object?> GetUserProfilePicture(string userId) {
            var user = await _context.Users
                .Include(u => u.ProfilePicture)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) {
                return null;
            }

            if (user.ProfilePicture == null) {
                user.ProfilePicture = await GetDefaultProfilePicture();
            }

            return new {
                user.ProfilePicture.ImageFormat,
                user.ProfilePicture.ImageData,
                user.ProfilePicture.UploadedDate
            };
        }

        public void Dispose() {
            _context.Dispose();
            _userManager.Dispose();
            _roleManager.Dispose();
        }
    }
}