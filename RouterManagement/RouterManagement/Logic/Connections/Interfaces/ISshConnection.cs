namespace RouterManagement.Logic.Connections.Interfaces
{
    public interface ISshConnection
    {
        bool IsConnected();
        string SendCommand(string customCmd);
    }
}
