using System;
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
            sshclient = new SshClient(adresIp.ToString(), port, username, password);
            connect();
        }

        public SshConnection(IPAddress adresIp, string username, string password)
        {
            sshclient = new SshClient(adresIp.ToString(), username, password);
            connect();
        }

        public SshConnection(string adresIp, string username, string password)
        {
            sshclient = new SshClient(adresIp, username, password);
            connect();
        }

        public SshConnection(string adresIp, int port, string username, string password)
        {
            sshclient = new SshClient(adresIp, port, username, password);
            connect();
        }

        public SshConnection(RouterAccesData routerAccesData)
        {
            if (routerAccesData.Port == null || routerAccesData.Port == 0)
            {
                sshclient = new SshClient(routerAccesData.RouterIp.ToString(),
                    routerAccesData.Login,
                    routerAccesData.Password);
            }
            else
            {
                sshclient = new SshClient(routerAccesData.RouterIp.ToString(), 
                    Convert.ToInt32(routerAccesData.Port),
                    routerAccesData.Login,
                    routerAccesData.Password);
            }
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

            stream = sshclient.CreateShellStream("cmd", 80, 24, 800, 600, 1024);
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