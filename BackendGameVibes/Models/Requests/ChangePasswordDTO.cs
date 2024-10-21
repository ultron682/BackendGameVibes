using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.Requests {
    public class ChangePasswordDTO {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmNewPassword { get; set; }

    }
}
