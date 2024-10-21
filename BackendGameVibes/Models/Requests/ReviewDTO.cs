using System.ComponentModel;

namespace BackendGameVibes.Models.Requests {
    /* swagger
{
  "userGameVibesId": "08f442f0-980e-4030-8241-23e2afdf83b3",
  "gameId": 3,
  "generalScore": 6,
  "graphicsScore": 7.5,
  "audioScore": 4,
  "gameplayScore": 9,
  "comment": "Bardzo fajna gierka. Polecam Magda Gessler"
}
    */
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
