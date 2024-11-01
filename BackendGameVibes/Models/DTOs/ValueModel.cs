using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.DTOs {
    public class ValueModel {
        [Required]
        public string? Value { get; set; }
    }
}
