using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.Requests {
    public class ValueModel {
        [Required]
        public string? Value { get; set; }
    }
}
