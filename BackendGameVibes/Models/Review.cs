namespace BackendGameVibes.Models
{
    public class Review
    {
        public int Id
        {
            get; set;
        }
        public int UserGameVibesId
        {
            get; set;
        }
        public int GameId
        {
            get; set;
        }
        public double GeneralScore
        {
            get; set;
        }
        public double GraphicsScore
        {
            get; set;
        }
        public double AudioScore
        {
            get; set;
        }
        public double GameplayScore
        {
            get; set;
        }
        public string Comment
        {
            get; set;
        }
        public DateTime CreatedAt
        {
            get; set;
        }

        public UserGameVibes UserGameVibes
        {
            get; set;
        }
        public Game Game
        {
            get; set;
        }
    }
}
