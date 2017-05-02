using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
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
        public bool IsConnected => sshclient != null && sshclient.IsConnected;

        private ShellStream shellStream;

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

            shellStream = sshclient.CreateShellStream("cmd", 80, 24, 800, 600, 1024);

            SendCommand("reset");
        }

        public string SendCommand(string customCmd)
        {
            var reader = new StreamReader(shellStream);
            reader.ReadToEnd(); //clear stream from old data
            writeStream(customCmd);

            var strAnswer = new StringBuilder();
            strAnswer.AppendLine(reader.ReadToEnd());
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
            SendCommand($"wifi");
            Thread.Sleep(5000);
        }

        public List<FirewallRuleViewModel> Get_AllFirewallRestrictionRules()
        {
            var rulesNames = getRestrictionRulesNames();

            var rulesList = new List<FirewallRuleViewModel>();

            foreach (var ruleName in rulesNames)
            {
                var routerConfig = SendCommand($"uci show firewall.{ruleName}");
                var currentConfiguratrion = parseAnswerToDictionary(routerConfig);

                var itemToAdd = new FirewallRuleViewModel();
                itemToAdd.RuleName = ruleName;

                if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.name"))
                {
                    itemToAdd.FriendlyName = currentConfiguratrion[$"firewall.{ruleName}.name"].Trim('\'');
                }

                if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.src_mac"))
                {
                    itemToAdd.Src_mac = currentConfiguratrion[$"firewall.{ruleName}.src_mac"].Trim().Trim('\'').Split(',');
                }

                if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.src_ip"))
                {
                    itemToAdd.Src_ip = currentConfiguratrion[$"firewall.{ruleName}.src_ip"].Trim().Trim('\'').Split(',');
                }

                if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.src_port"))
                {
                    itemToAdd.Src_port = currentConfiguratrion[$"firewall.{ruleName}.src_port"].Trim().Trim('\'').Split(',');
                }

                if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.dest_ip"))
                {
                    itemToAdd.Dest_ip = currentConfiguratrion[$"firewall.{ruleName}.dest_ip"].Trim().Trim('\'').Split(',');
                }

                if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.dest_port"))
                {
                    itemToAdd.Dest_port = currentConfiguratrion[$"firewall.{ruleName}.dest_port"].Trim().Trim('\'').Split(',');
                }

                if (currentConfiguratrion.ContainsKey($"firewall.{ruleName}.enabled"))
                {
                    itemToAdd.Enabled = currentConfiguratrion[$"firewall.{ruleName}.enabled"].Trim('\'');
                }

                rulesList.Add(itemToAdd);
            }

            return rulesList;
        }

        public void Send_DeleteFirewallRestrictionRule(string ruleName)
        {
            writeStream($"uci delete firewall.{ruleName}");
            writeStream($"uci commit");
        }

        public string Send_SaveFirewallRestrictionRule(AddFirewallRuleViewModel rule)
        {
            var ruleName = getNewRestrictionRuleName();

            writeStream($"uci set firewall.{ruleName}=rule");
            writeStream($"uci set firewall.{ruleName}.src='*'");
            writeStream($"uci set firewall.{ruleName}.dest='*'");
            writeStream($"uci set firewall.{ruleName}.name='{rule.FriendlyName}'");
            if (!string.IsNullOrEmpty(rule.Src_mac)) writeStream($"uci set firewall.{ruleName}.src_mac='{rule.Src_mac}'");
            if (!string.IsNullOrEmpty(rule.Src_ip)) writeStream($"uci set firewall.{ruleName}.src_ip='{rule.Src_ip}'");
            if (!string.IsNullOrEmpty(rule.Src_port)) writeStream($"uci set firewall.{ruleName}.src_port='{rule.Src_port}'");
            if (!string.IsNullOrEmpty(rule.Dest_ip)) writeStream($"uci set firewall.{ruleName}.dest_ip='{rule.Dest_ip}'");
            if (!string.IsNullOrEmpty(rule.Dest_port)) writeStream($"uci set firewall.{ruleName}.dest_port='{rule.Dest_port}'");
            writeStream($"uci set firewall.{ruleName}.target='DROP'");
            writeStream($"uci set firewall.{ruleName}.enabled='{rule.Enabled}'");

            writeStream($"uci commit firewall");
            SendCommand($"/etc/init.d/firewall restart");

            Thread.Sleep(1500);

            return ruleName;
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
            var writer = new StreamWriter(shellStream) {AutoFlush = true};
            writer.WriteLine(cmd);
            while (shellStream.Length == 0)
            {
                Thread.Sleep(500);
            }
        }

        private string getNewRestrictionRuleName()
        {
            var howMuchToDelete = "RouterManagementRule_".Length;
            var rulesNumbers = getRestrictionRulesNames().Select(r => Convert.ToInt32(r.Remove(0, howMuchToDelete))).OrderBy(r => r).ToList();

            if (!rulesNumbers.Any())
            {
                return "RouterManagementRule_1";
            }

            var numbersBetween = Enumerable.Range(1, rulesNumbers.Last()).Except(rulesNumbers).ToList();
            return numbersBetween.Any() ? $"RouterManagementRule_{numbersBetween.First()}" : $"RouterManagementRule_{rulesNumbers.Last() + 1}";
        }

        private IEnumerable<string> getRestrictionRulesNames()
        {
            var answer = SendCommand("grep RouterManagementRule /etc/config/firewall");

            return from Match match in Regex.Matches(answer, "\'([^\']*)\'")
                select match.ToString().Trim('\'');
        }

        #endregion
    }
}