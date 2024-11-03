using BackendGameVibes.Models.Forum;
using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.DTOs.Forum {
    public class ForumPostUpdateDTO {
        [Required]
        public string? Content {
            get; set;
        }
    }
}
