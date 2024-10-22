using BackendGameVibes.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendGameVibes.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly IAccountService _accountService;

        public UserController(IAccountService accountService) {
            _accountService = accountService;
        }

        [HttpGet(":id")]
        public async Task<ActionResult<object>> GetUser(string id) {
            var user = await _accountService.GetUserByIdAsync(id);
            if (user == null) {
                return NotFound();
            }
            else {
                var accountInfo = await _accountService.GetAccountInfoAsync(user.Id, user);
                return Ok(accountInfo);
            }
        }
    }
}
