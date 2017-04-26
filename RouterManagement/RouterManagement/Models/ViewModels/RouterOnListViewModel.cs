using RouterManagement.Logic.Connections;

namespace RouterManagement.Models.ViewModels
{
    public class RouterOnListViewModel
    {
        public bool IsActive;
        public string Name;
        public SshConnection SshConnection;
    }
}