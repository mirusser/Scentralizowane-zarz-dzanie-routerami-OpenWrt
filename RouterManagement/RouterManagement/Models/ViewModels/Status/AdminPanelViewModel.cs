using System.Collections.Generic;

namespace RouterManagement.Models.ViewModels.Status
{
    public class AdminPanelViewModel
    {
        public int NumbersOfSavedClients { get; set; }
        public int NumbersOfConnectedClients { get; set; }

        public IEnumerable<RoutersStatusViewModel> RoutersStatusViewModel { get; set; }
    }
}