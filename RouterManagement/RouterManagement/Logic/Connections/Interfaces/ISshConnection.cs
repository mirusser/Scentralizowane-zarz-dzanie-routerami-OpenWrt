namespace RouterManagement.Logic.Connections.Interfaces
{
    public interface ISshConnection
    {
        string SendCommand(string customCmd);
    }
}
