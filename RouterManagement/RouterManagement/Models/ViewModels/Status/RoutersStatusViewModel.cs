using System.Collections.Generic;

namespace RouterManagement.Models.ViewModels.Status
{
    public class RoutersStatusViewModel
    {
        public string RouterName { get; set; }
        public CurrentMemoryUsageViewModel CurrentMemoryUsage { get; set; }
    }
}