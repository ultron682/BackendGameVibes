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
        public string CpuRequirement
        {
            get; set;
        }
        public string GpuRequirement
        {
            get; set;
        }
        public string RamRequirement
        {
            get; set;
        }
        public string DiskRequirement
        {
            get; set;
        }
        public string OperatingSystemRequirement
        {
            get; set;
        }

        public Game Game
        {
            get; set;
        }
    }
}
