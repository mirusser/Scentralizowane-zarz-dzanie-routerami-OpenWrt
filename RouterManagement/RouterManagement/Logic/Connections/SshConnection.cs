using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public Dictionary<string, string> Send_UciShow()
        {
            var answer = SendCommand("uci show");

            return parseAnswerToDictionary(answer);
        }

        public Dictionary<string, string> Send_UciShowWireless()
        {
            var answer = SendCommand("uci show wireless");

            return parseAnswerToDictionary(answer);
        }

        public string SendCommand(string customCmd)
        {
            var strAnswer = new StringBuilder();

            writeStream(customCmd);

            strAnswer.AppendLine(readStream());

            var answer = strAnswer.ToString().Trim();

            if (answer.Contains(string.Concat("-ash: ", customCmd, ": not found")))
            {
                throw new InvalidOperationException(string.Concat("Unrecognized command: ", customCmd));
            }

            if (answer.Contains("Entry not found"))
            {
                throw new InvalidOperationException(string.Concat("Entry not found for command: ", customCmd));
            }

            return answer;
        }

        #region private methods

        private static Dictionary<string, string> parseAnswerToDictionary(string answer)
        {
            //remove first line - command sent to router
            answer = answer.Substring(answer.IndexOf(Environment.NewLine, StringComparison.Ordinal) + 1);
            //remove two last lines (unrecognized log information)
            answer = answer.Remove(answer.LastIndexOf(Environment.NewLine, StringComparison.Ordinal));
            answer = answer.Remove(answer.LastIndexOf(Environment.NewLine, StringComparison.Ordinal));
            //remove new line mark as first char
            if (answer.FirstOrDefault().Equals('\n'))
            {
                answer = answer.Remove(0,1);
            }

            var entriesTable = answer.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var entriesAsDictionary = entriesTable
                .Select(part => part.Split('='))
                .ToDictionary(split => split[0], split => split[1]);

            return entriesAsDictionary;
        }

        private void connect()
        {
            sshclient.Connect();

            stream = sshclient.CreateShellStream("cmd", 80, 24, 800, 600, 1024);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };

            SendCommand("reset");
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