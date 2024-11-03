using System.ComponentModel;

namespace BackendGameVibes.Models.DTOs {
    public class ReviewUpdateDTO {
        public double? GeneralScore { get; set; }
        public double? GraphicsScore { get; set; }
        public double? AudioScore { get; set; }
        public double? GameplayScore { get; set; }
        public string? Comment { get; set; }
    }
}
