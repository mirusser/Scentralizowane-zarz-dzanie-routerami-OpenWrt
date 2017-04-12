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
using RouterManagement.Models.ViewModels;

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

        public Dictionary<string, string> SendFake_UciShow()
        {
            using (WebClient client = new WebClient())
            {
                string answer = client.DownloadString("http://wklej.org/id/3085238/txt/");

                var entriesTable = answer.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var entriesAsDictionary = entriesTable
                    .Select(part => part.Split('='))
                    .ToDictionary(split => split[0], split => split[1]);

                return entriesAsDictionary;
            }
        }

        public Dictionary<string, string> Send_UciShowWireless()
        {
            var answer = SendCommand("uci show wireless");

            return parseAnswerToDictionary(answer);
        }

        public Dictionary<string, string> SendFake_UciShowWireless()
        {
            using (WebClient client = new WebClient())
            {
                string answer = client.DownloadString("http://wklej.org/id/3085279/txt/");

                var entriesTable = answer.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var entriesAsDictionary = entriesTable
                    .Select(part => part.Split('='))
                    .ToDictionary(split => split[0], split => split[1]);

                return entriesAsDictionary;
            }
        }

        public void Send_UciSetWireless(SendUciShowWirelessViewModel wireless)
        {
            writeStream($"uci set wireless.@wifi-device[0].disabled={Convert.ToInt32(wireless.Disabled)}");
            writeStream($"uci set wireless.@wifi-device[0].channel={wireless.Channel}");
            writeStream($"uci set wireless.@wifi-iface[0].ssid={wireless.Ssid}");
            writeStream($"uci set wireless.@wifi-iface[0].encryption={wireless.Encryption}");
            writeStream($"uci set wireless.@wifi-iface[0].key={wireless.Key}");
            writeStream($"uci set wireless.@wifi-iface[0].mode={wireless.Mode}");
            writeStream($"uci set wireless.@wifi-iface[0].network={wireless.Network}");
            if (wireless.Network == "wan")
            {
                //writeStream($"uci del network.wan");
                writeStream($"uci set network.wan=interface");
                writeStream($"uci set network.wan.proto=dhcp");
            }

            writeStream($"uci commit");
        }

        public Dictionary<string, string> Send_UciShowFirewall()
        {
            var answer = SendCommand("uci show firewall");
            return parseAnswerToDictionary(answer);
        }

        public Dictionary<string, string> SendFake_UciShowFirewall()
        {
            using (WebClient client = new WebClient())
            {
                string answer = client.DownloadString("http://wklej.org/id/3085278/txt/");

                var entriesTable = answer.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var entriesAsDictionary = entriesTable
                    .Select(part => part.Split('='))
                    .ToDictionary(split => split[0], split => split[1]);

                return entriesAsDictionary;
            }
        }

        public void Send_DeleteFirewallRule(int ruleId)
        {
            SendCommand($"uci delete firewall.rule_{ruleId}");
            SendCommand($"uci commit firewall");
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
            try
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
            catch (Exception ex)
            {
            }
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