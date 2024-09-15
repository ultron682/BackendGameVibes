using Microsoft.AspNetCore.Identity;

namespace CodeShareBackend.Models
{
    public class UserGameVibes: IdentityUser
    {
        public List<CodeSnippet> CodeSnippets { get; set; } = new List<CodeSnippet>();
    }
}
