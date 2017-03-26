using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Renci.SshNet;
using RouterManagement.Logic.Connections.Interfaces;
using RouterManagement.Models;

namespace RouterManagement.Logic.Connections
{
    public class SshConnection : ISshConnection
    {
        #region properties

        private readonly SshClient sshclient;
        private ShellStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        #endregion

        #region constructors

        public SshConnection(IPAddress adresIp, int port, string username, string password)
        {
            var host = string.Concat(adresIp.ToString(), port);
            sshclient = new SshClient(host, username, password);
            connect();
        }

        public SshConnection(string host, string username, string password)
        {
            sshclient = new SshClient(host, username, password);
            connect();
        }

        public SshConnection(RouterAccesData routerAccesData)
        {
            var host = routerAccesData.Port == 0 ?
                routerAccesData.RouterIp.ToString() :
                string.Concat(routerAccesData.RouterIp.ToString(), routerAccesData.Port);

            sshclient = new SshClient(host, routerAccesData.Login, routerAccesData.Password);
            connect();
        }

        #endregion

        public bool IsConnected()
        {
            return sshclient.IsConnected;
        }

        public string SendCommand(string customCmd)
        {
            var strAnswer = new StringBuilder();

            writeStream(customCmd);

            strAnswer.AppendLine(readStream());

            var answer = strAnswer.ToString();
            return answer.Trim();
        }

        #region private methods

        private void connect()
        {
            sshclient.Connect();

            stream = sshclient.CreateShellStream("customCommand", 80, 24, 800, 600, 1024);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };
        }

        private void writeStream(string cmd)
        {
            writer.WriteLine(cmd);
            while (stream.Length == 0)
            {
                Thread.Sleep(500);
            }
        }

        private string readStream()
        {
            var result = new StringBuilder();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                result.AppendLine(line);
            }

            return result.ToString();
        }

        #endregion
    }
}