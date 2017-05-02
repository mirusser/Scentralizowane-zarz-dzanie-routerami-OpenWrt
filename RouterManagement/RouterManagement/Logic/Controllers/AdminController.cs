using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RouterManagement.Logic.Connections;
using RouterManagement.Models.ViewModels;
using RouterManagement.Models;
using RouterManagement.Models.ViewModels.Firewall;
using RouterManagement.Models.ViewModels.Router;
using RouterManagement.Models.ViewModels.Wireless;

namespace RouterManagement.Logic.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region full configuration

        public ActionResult FullConfiguration(string name = null)
        {
            name = name ?? RoutersConnections.GetFirstRouterName();
            if (name == null) return View("~/Views/Admin/NoRoutersError.cshtml");
            var sshConnection = RoutersConnections.GetConnectionByName(name);
            if (sshConnection == null) return null;

            var currentConfiguratrion = sshConnection.Get_FullConfiguration();

            return View(currentConfiguratrion);
        }

        #endregion

        #region router acces data configuration

        public ActionResult AllRouters()
        {
            var allRouters = RoutersConnections.GetRoutersAsRouterAccesDataViewModel();

            return View(allRouters);
        }

        [HttpPost]
        public ActionResult ReconnectAllRouter()
        {
            RoutersConnections.ReconnectAllRouters();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddRouterPartial()
        {
            var allRoutersNames = new AddRouterPartialViewModel { AllRoutersNames = RoutersConnections.GetAllRoutersNames() };
            return PartialView("~/Views/Admin/RouterPartialViews/_AddRouter.cshtml", allRoutersNames);
        }

        [HttpPost]
        public ActionResult AddRouter(AddRouterViewModel router)
        {
            if (!ModelState.IsValid) return Json(new { status = "false" }, JsonRequestBehavior.AllowGet);

            var routerAccesData = new RouterAccesData
            {
                Name = router.Name,
                RouterIp = router.RouterIp,
                Port = router.Port,
                Login = router.Login,
                Password = router.Password
            };

            RoutersConnections.CreateNewConnection(routerAccesData);

            var isConnected = RoutersConnections.CheckIfRouterIsConnected(router.Name);

            return Json(new { status = "true", isConnected = isConnected.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModifyRouterPartial(string router)
        {
            var routerToModify = RoutersConnections.GetRouterAccesDataByName(router);
            var routerToModifyModel = new ModifyRouterPartialViewModel
            {
                Name = routerToModify.Name,
                RouterIp = routerToModify.RouterIp,
                Port = routerToModify.Port,
                Login = routerToModify.Login,
                Password = routerToModify.Password,
                AllRoutersNames = RoutersConnections.GetAllRoutersNames().Except(new List<string> { router })
            };
            return PartialView("~/Views/Admin/RouterPartialViews/_ModifyRouter.cshtml", routerToModifyModel);
        }

        [HttpPost]
        public ActionResult ModifyRouter(ModifyRouterDataViewModel router)
        {
            if (!ModelState.IsValid) return Json(new { status = "false" }, JsonRequestBehavior.AllowGet);

            var routerAccesData = new RouterAccesData
            {
                Name = router.Name,
                RouterIp = router.RouterIp,
                Port = router.Port,
                Login = router.Login,
                Password = router.Password
            };
            RoutersConnections.DeleteConnectionByName(router.OldName);
            RoutersConnections.CreateNewConnection(routerAccesData);

            var isConnected = RoutersConnections.CheckIfRouterIsConnected(router.Name);

            return Json(new { status = "true", isConnected = isConnected.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveRouter(string name)
        {
            if (string.IsNullOrEmpty(name)) Json(false, JsonRequestBehavior.AllowGet);
            try
            {
                RoutersConnections.DeleteConnectionByName(name);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wireless

        public ActionResult Wireless(string name = null)
        {
            name = name ?? RoutersConnections.GetFirstRouterName();
            if (name == null) return View("~/Views/Admin/NoRoutersError.cshtml");
            var sshConnection = RoutersConnections.GetConnectionByName(name);
            if (sshConnection == null) return null;
            var currentConfiguratrion = sshConnection.Get_Wireless();

            var configurationTrimmed = new WirelessViewModel
            {
                Disabled = (currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".disabled")).Value == "1"),
                Channel = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".channel")).Value,
                Ssid = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".ssid")).Value,
                Encryption = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".encryption")).Value,
                Key = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".key")).Value,
                Mode = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".mode")).Value,
                Network = currentConfiguratrion.FirstOrDefault(c => c.Key.Contains(".network")).Value,
                RouterName = string.IsNullOrEmpty(name) ? RoutersConnections.GetFirstRouterName() : name
            };

            return View(configurationTrimmed);
        }

        [HttpPost]
        public ActionResult EditWirelessPartial(WirelessViewModel config)
        {
            return PartialView("~/Views/Admin/WirelessPartialViews/_EditWireless.cshtml", config);
        }

        [HttpPost]
        public ActionResult SaveWirelessPartial(WirelessViewModel config)
        {
            if (!ModelState.IsValid) return Json(false, JsonRequestBehavior.AllowGet);

            try
            {
                var sshConnection = RoutersConnections.GetConnectionByName(config.RouterName);
                if (sshConnection == null) throw new Exception();

                sshConnection.Send_SaveWireless(config);
                RoutersConnections.ReconnectAllRouters();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region firewall

        public ActionResult Firewall(string name = null)
        {
            name = name ?? RoutersConnections.GetFirstRouterName();
            if (name == null) return View("~/Views/Admin/NoRoutersError.cshtml");
            var sshConnection = RoutersConnections.GetConnectionByName(name);
            if (sshConnection == null) throw new Exception($"Router {name} is offline");

            var model = new FirewallViewModel
            {
                FirewallRestrictionRules = sshConnection.Get_AllFirewallRestrictionRules().ToList(),
                RouterName = name
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult RemoveRule(FirewallRulePartialViewModel rule)
        {
            try
            {
                var sshConnection = RoutersConnections.GetConnectionByName(rule.RouterName);
                if (sshConnection == null) throw new Exception();

                sshConnection.Send_DeleteFirewallRule(rule.RuleName);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddRulePartial(string routerName)
        {
            return PartialView("~/Views/Admin/FirewallPartialViews/_AddRule.cshtml", routerName);
        }

        [HttpPost]
        public ActionResult AddRule(AddFirewallRuleViewModel rule)
        {
            if (!ModelState.IsValid) return Json(false, JsonRequestBehavior.AllowGet);

            try
            {
                var sshConnection = RoutersConnections.GetConnectionByName(rule.RouterName);
                if (sshConnection == null) throw new Exception($"Router {rule.RouterName} is offline");
                var ruleName = sshConnection.Send_SaveFirewallRule(rule);
                return Json(new { ruleName = ruleName, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifyRulePartial(FirewallRulePartialViewModel rule)
        {
            if (!ModelState.IsValid) return Json(false, JsonRequestBehavior.AllowGet);

            var sshConnection = RoutersConnections.GetConnectionByName(rule.RouterName);
            if (sshConnection == null) throw new Exception($"Router {rule.RouterName} is offline");

            var ruleToModify = sshConnection.Get_FirewallRuleByName(rule.RuleName);

            var model = new ModifyFirewallRuleViewModel
            {
                RouterName = rule.RouterName,
                RuleName = rule.RuleName,
                FriendlyName = ruleToModify.FriendlyName,
                Src_mac = (ruleToModify.Src_mac != null)
                    ? string.Join(Environment.NewLine, ruleToModify.Src_mac)
                    : string.Empty,
                Src_ip = (ruleToModify.Src_ip != null)
                    ? string.Join(Environment.NewLine, ruleToModify.Src_ip)
                    : string.Empty,
                Src_port = (ruleToModify.Src_port != null)
                    ? string.Join(Environment.NewLine, ruleToModify.Src_port)
                    : string.Empty,
                Dest_ip = (ruleToModify.Dest_ip != null)
                    ? string.Join(Environment.NewLine, ruleToModify.Dest_ip)
                    : string.Empty,
                Dest_port = (ruleToModify.Dest_port != null)
                    ? string.Join(Environment.NewLine, ruleToModify.Dest_port)
                    : string.Empty,
                Enabled = ruleToModify.Enabled
            };

            return PartialView("~/Views/Admin/FirewallPartialViews/_ModifyRule.cshtml", model);
        }

        [HttpPost]
        public ActionResult ModifyRule(ModifyFirewallRuleViewModel rule)
        {
            if (!ModelState.IsValid) return Json(false, JsonRequestBehavior.AllowGet);

            var sshConnection = RoutersConnections.GetConnectionByName(rule.RouterName);
            if (sshConnection == null) throw new Exception($"Router {rule.RouterName} is offline");

            sshConnection.Send_DeleteFirewallRule(rule.RuleName);
            var ruleName = sshConnection.Send_SaveFirewallRule(rule);

            return Json(new { status = true, ruleName = ruleName }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}