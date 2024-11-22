using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;

namespace BackendGameVibes.Middlewares {
    public class UpdateLastActivityMiddleware {
        private readonly RequestDelegate _next;

        public UpdateLastActivityMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserManager<UserGameVibes> userManager) {
            if (context.User.Identity is not null && context.User.Identity.IsAuthenticated) {
                var user = await userManager.GetUserAsync(context.User);
                if (user != null) {
                    user.LastActivityDate = DateTime.Now;
                    await userManager.UpdateAsync(user);
                }
            }

            await _next(context);
        }
    }

}
