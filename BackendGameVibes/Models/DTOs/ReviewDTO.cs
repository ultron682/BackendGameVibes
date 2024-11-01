using System.ComponentModel;

namespace BackendGameVibes.Models.DTOs {
    public class ReviewDTO {
        //[DefaultValue("long required userID")]
        //public string? UserGameVibesId { get; set; }
        [DefaultValue("1")]
        public int? GameId { get; set; }
        public double GeneralScore { get; set; }
        [DefaultValue("9")]
        public double GraphicsScore { get; set; }
        [DefaultValue("9")]
        public double AudioScore { get; set; }
        [DefaultValue("9")]
        public double GameplayScore { get; set; }
        [DefaultValue("Empty comment")]
        public string Comment { get; set; }
    }
}
