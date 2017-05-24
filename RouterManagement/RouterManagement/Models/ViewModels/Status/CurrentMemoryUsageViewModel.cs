namespace RouterManagement.Models.ViewModels.Status
{
    public class CurrentMemoryUsageViewModel
    {
        public int RamTotal { get; set; }
        public int RamUsed { get; set; }
        public int RamFree { get; set; }

        public int SwapTotal { get; set; }
        public int SwapUsed { get; set; }
        public int SwapFree { get; set; }
    }
}