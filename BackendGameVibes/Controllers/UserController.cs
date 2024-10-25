using BackendGameVibes.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BackendGameVibes.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly IAccountService _accountService;

        public UserController(IAccountService accountService) {
            _accountService = accountService;
        }

        [HttpGet(":id")]
        public async Task<ActionResult<object>> GetUserAsync(string id) {
            var user = await _accountService.GetUserByIdAsync(id);
            if (user == null) {
                return NotFound();
            }
            else {
                var accountInfo = await _accountService.GetAccountInfoAsync(user.Id, user);
                return Ok(accountInfo);
            }
        }

        [HttpGet("search-user:nick")]
        [SwaggerOperation("wyszukiwanie mozliwych uzytkownikow do dodania znajomych po nicku")]
        [Authorize]
        public async Task<ActionResult<object>> SearchUserAsync(string nick) {
            Console.WriteLine(User.Identity!.Name!);
            string myNickname = User.Identity!.Name!;

            var users = await _accountService.FindUserByNickname(myNickname, nick);
            if (users == null) {
                return NotFound();
            }
            else {
                return Ok(users);
            }
        }

        [HttpPost("send-friend-request:id")]
        [Authorize()]
        [SwaggerOperation("todo: do znajomego wysylane jest powiadomienie. na swoim koncie będzie mogl zaakceptować lub odrzucić bez powiadamienia osoby ktora wyslala zapro. jak git to odpala endpoint confirm-friend-request:id")]
        public async Task<ActionResult<object>> SendFriendRequstAsync([FromQuery] string id) {
            var user = await _accountService.GetUserByIdAsync(id);
            if (user == null) {
                return NotFound();
            }
            else {

                return Ok("ToDo");
            }
        }

        [HttpPost("confirm-friend-request:id")]
        [SwaggerOperation("todo: token bedzie generowany przez wewnętrzny mechanizm i wysylany na email. uzytkownik klikajac na link w emailu odpala ten link")]
        public async Task<ActionResult<object>> ConfirmFriendRequstAsync(string token) {
            //var user = await _accountService.GetUserByIdAsync(id);
            //if (user == null) {
            //    return NotFound();
            //}
            //else {

            //    return Ok("ToDo");
            //}

            return Ok("Todo");
        }
    }
}
