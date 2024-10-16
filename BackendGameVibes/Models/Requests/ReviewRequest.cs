using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.Requests {
    /*
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
    public class ReviewRequest {
        public string UserGameVibesId { get; set; }
        public int GameId { get; set; }
        public double GeneralScore { get; set; }
        public double GraphicsScore { get; set; }
        public double AudioScore { get; set; }
        public double GameplayScore { get; set; }
        public string Comment { get; set; }
    }
}
