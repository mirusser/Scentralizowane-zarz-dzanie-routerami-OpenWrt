namespace RouterManagement.Logic.Connections.Interfaces
{
    public interface ISshConnection
    {
        string Send_CustomCommand(string customCmd);
    }
}
