namespace BackendGameVibes.Models
{
    public class GameImage
    {
        public int Id
        {
            get; set;
        }
        public string? ImagePath
        {
            get; set;
        }
        public int? GameId
        {
            get; set;
        }

        public Game? Game
        {
            get; set;
        }
    }
}
