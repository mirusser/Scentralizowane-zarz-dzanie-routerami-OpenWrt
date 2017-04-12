using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RouterManagement.Logic.Connections;
using RouterManagement.Models.ViewModels;

namespace RouterManagement.Logic.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendUciShow(int? selectedRouter = null)
        {
            var sshConnection = new SshConnection("192.168.2.1", "root", "konopie1");
            var currentConfiguratrion = sshConnection?.SendFake_UciShow();
            
            return View(currentConfiguratrion);
        }

        #region wireless

        public ActionResult Wireless(int? selectedRouter = null)
        {
            var sshConnection = new SshConnection("192.168.1.1", "root", "konopie");
            var currentConfiguratrion = sshConnection?.SendFake_UciShowWireless();

            var configurationTrimmed = new SendUciShowWirelessViewModel
            {
                Disabled = (currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".disabled")).Value == "1"),
                Channel = Convert.ToInt32(currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".channel")).Value),
                Ssid = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".ssid")).Value,
                Encryption = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".encryption")).Value,
                Key = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".key")).Value,
                Mode = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".mode")).Value,
                Network = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".network")).Value
            };

            return View(configurationTrimmed);
        }

        [HttpPost]
        public ActionResult EditWirelessPartial(SendUciShowWirelessViewModel config)
        {
            return PartialView("~/Views/Admin/PartialViews/_EditWirelessConfiguration.cshtml", config);
        }

        [HttpPost]
        public ActionResult SaveWirelessPartial(SendUciShowWirelessViewModel config)
        {
            try
            {
                var sshConnection = new SshConnection("192.168.1.1", "root", "konopie");
                sshConnection.Send_UciSetWireless(config);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region firewall

        public ActionResult Firewall(int? selectedRouter = null)
        {
            var sshConnection = new SshConnection("192.168.1.1", "root", "konopie");
            var currentConfiguratrion = sshConnection?.SendFake_UciShowFirewall();

            currentConfiguratrion = currentConfiguratrion.Where(c => c.Key.Contains("rule_")).ToDictionary(it => it.Key, it => it.Value);
            var firewallList = new List<FirewallViewModel>();

            var n = 1;
            while (currentConfiguratrion.Any(c => c.Key.Contains($"firewall.rule_{n}")))
            {
                var itemToAdd = new FirewallViewModel();

                itemToAdd.Id = n;

                if(currentConfiguratrion.ContainsKey($"firewall.rule_{n}"))
                {
                    itemToAdd.Type = currentConfiguratrion[$"firewall.rule_{n}"];
                }

                if (currentConfiguratrion.ContainsKey($"firewall.rule_{n}.is_ingress"))
                {
                    itemToAdd.Is_Ingreee = (currentConfiguratrion[$"firewall.rule_{n}.is_ingress"] == "1");
                }

                if (currentConfiguratrion.ContainsKey($"firewall.rule_{n}.description"))
                {
                    itemToAdd.Description = currentConfiguratrion[$"firewall.rule_{n}.description"];
                }

                if (currentConfiguratrion.ContainsKey($"firewall.rule_{n}.local_addr"))
                {
                    itemToAdd.Local_addr = currentConfiguratrion[$"firewall.rule_{n}.local_addr"].Trim().Split(',');
                }

                if (currentConfiguratrion.ContainsKey($"firewall.rule_{n}.active_hours"))
                {
                    itemToAdd.Active_hours = currentConfiguratrion[$"firewall.rule_{n}.active_hours"].Trim().Split(',');
                }

                if (currentConfiguratrion.ContainsKey($"firewall.rule_{n}.enabled"))
                {
                    itemToAdd.Enabled = (currentConfiguratrion[$"firewall.rule_{n}.enabled"] == "1");
                }

                firewallList.Add(itemToAdd);

                n++;
            }

            return View(firewallList);
        }

        [HttpPost]
        public ActionResult RemoveRule(int ruleId)
        {
            try
            {
                var sshConnection = new SshConnection("192.168.1.1", "root", "konopie");
                sshConnection.Send_DeleteFirewallRule(ruleId);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}