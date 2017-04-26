using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

        public SshClient sshclient { get; }
        private ShellStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        #endregion

        #region constructors

        public SshConnection(IPAddress adresIp, int port, string username, string password)
        {
            if (!tryPingIp(adresIp.ToString())) return;
            sshclient = new SshClient(adresIp.ToString(), port, username, password);
            Connect();
        }

        public SshConnection(IPAddress adresIp, string username, string password)
        {
            if (!tryPingIp(adresIp.ToString())) return;
            sshclient = new SshClient(adresIp.ToString(), username, password);
            Connect();
        }

        public SshConnection(string adresIp, string username, string password)
        {
            if (!tryPingIp(adresIp.ToString())) return;
            sshclient = new SshClient(adresIp, username, password);
            Connect();
        }

        public SshConnection(string adresIp, int port, string username, string password)
        {
            if (!tryPingIp(adresIp.ToString())) return;
            sshclient = new SshClient(adresIp, port, username, password);
            Connect();
        }

        public SshConnection(RouterAccesData routerAccesData)
        {
            if (!tryPingIp(routerAccesData.RouterIp)) return;


            if (routerAccesData.Port == null || routerAccesData.Port == 0)
            {
                sshclient = new SshClient(routerAccesData.RouterIp,
                    routerAccesData.Login,
                    routerAccesData.Password);
            }
            else
            {
                sshclient = new SshClient(routerAccesData.RouterIp,
                    Convert.ToInt32(routerAccesData.Port),
                    routerAccesData.Login,
                    routerAccesData.Password);
            }
            Connect();
        }

        #endregion

        public void Connect()
        {
            try
            {
                sshclient.Connect();
            }
            catch
            {
                return;
            }

            stream = sshclient.CreateShellStream("cmd", 80, 24, 800, 600, 1024);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };

            SendCommand("reset");
        }

        public bool IsConnected()
        {
            return sshclient != null && sshclient.IsConnected;
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
            writeStream($"wifi");
        }

        public Dictionary<string, string> Send_UciShowFirewall()
        {
            var answer = SendCommand("uci show firewall");
            return parseAnswerToDictionary(answer);
        }

        public void Send_DeleteFirewallRule(int ruleId)
        {
            writeStream($"uci delete firewall.rule_{ruleId}");
            writeStream($"uci commit");
        }

        public int Send_SaveFirewallRule(AddFirewallRuleViewModel rule)
        {
            var id = getNewId();

            writeStream($"uci set firewall.rule_{id}={rule.Type}");
            writeStream($"uci set firewall.rule_{id}.is_ingress={Convert.ToInt32(rule.Is_Ingreee)}");
            writeStream($"uci set firewall.rule_{id}.description={rule.Description}");
            writeStream($"uci set firewall.rule_{id}.local_addr={rule.Local_addr}");
            if (!string.IsNullOrEmpty(rule.Active_hours))
            {
                writeStream($"uci set firewall.rule_{id}.active_hours={rule.Active_hours}");
            }
            writeStream($"uci set firewall.rule_{id}.enabled={Convert.ToInt32(rule.Enabled)}");

            writeStream($"uci commit firewall");

            return id;
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

        #region fake methods

        public Dictionary<string, string> SendFake_UciShow()
        {
            using (var client = new WebClient())
            {
                var answer = client.DownloadString("http://wklej.org/id/3085238/txt/");

                var entriesTable = answer.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var entriesAsDictionary = entriesTable
                    .Select(part => part.Split('='))
                    .ToDictionary(split => split[0], split => split[1]);

                return entriesAsDictionary;
            }
        }

        public Dictionary<string, string> SendFake_UciShowWireless()
        {
            using (WebClient client = new WebClient())
            {
                var answer = client.DownloadString("http://wklej.org/id/3085279/txt/");

                var entriesTable = answer.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var entriesAsDictionary = entriesTable
                    .Select(part => part.Split('='))
                    .ToDictionary(split => split[0], split => split[1]);

                return entriesAsDictionary;
            }
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

        #endregion

        #region private methods

        private static bool tryPingIp(string ipAdress)
        {
            if (string.IsNullOrEmpty(ipAdress)) return false;

            using (var ping = new Ping())
            {
                var pingReply = ping.Send(ipAdress);
                if (pingReply == null || pingReply.Status != IPStatus.Success)
                {
                    return false;
                }
            }

            return true;
        }

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

        private int getNewId()
        {
            var answer = Send_UciShowFirewall();
            var rules = answer.Select(it => it.Key).Where(k => k.Contains("firewall.rule_"));
            var ids = new List<int>();
            foreach (var r in rules)
            {
                try
                {
                    ids.Add((int)char.GetNumericValue(r[14]));
                }
                catch { }
            }


            return ids.Any() ? ids.Max() + 1 : 1;
        }

        #endregion
    }
}