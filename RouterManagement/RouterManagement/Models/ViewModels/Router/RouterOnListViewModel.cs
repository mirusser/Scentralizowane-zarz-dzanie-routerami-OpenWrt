using RouterManagement.Logic.Connections;

namespace RouterManagement.Models.ViewModels.Router
{
    public class RouterOnListViewModel
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public SshConnection SshConnection { get; set; }
    }
}