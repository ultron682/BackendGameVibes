namespace BackendGameVibes.Models
{
    public class SystemRequirement
    {
        public int Id
        {
            get; set;
        }
        public int GameId
        {
            get; set;
        }
        public required string CpuRequirement
        {
            get; set;
        }
        public required string GpuRequirement
        {
            get; set;
        }
        public required string RamRequirement
        {
            get; set;
        }
        public required string DiskRequirement
        {
            get; set;
        }
        public required string OperatingSystemRequirement
        {
            get; set;
        }

        public Game? Game
        {
            get; set;
        }
    }
}
