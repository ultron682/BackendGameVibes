using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.DTOs {
    public class EmailSendDTO {
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}
