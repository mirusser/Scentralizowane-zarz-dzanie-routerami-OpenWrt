using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RouterManagement.Logic.Connections;
using RouterManagement.Models.ViewModels;
using RouterManagement.Models;

namespace RouterManagement.Logic.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendUciShow(string name = null)
        {
            name = name ?? RoutersConnections.GetFirstRouterName();
            if (name == null) return View("~/Views/Admin/NoRoutersError.cshtml");
            var sshConnection = RoutersConnections.GetConnectionByName(name);
            if (sshConnection == null) return null;

            var currentConfiguratrion = sshConnection.Send_UciShow();
            
            return View(currentConfiguratrion);
        }

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
            AddRouterDataPartialViewModel allRoutersNames = new AddRouterDataPartialViewModel {AllRoutersNames = RoutersConnections.GetAllRoutersNames()};
            return PartialView("~/Views/Admin/PartialViews/_AddRouter.cshtml", allRoutersNames);
        }

        [HttpPost]
        public ActionResult AddRouter(AddRouterDataViewModel router)
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
            var routerToModifyModel = new ModifyRouterDataPartialViewModel
            {
                Name = routerToModify.Name,
                RouterIp = routerToModify.RouterIp,
                Port = routerToModify.Port,
                Login = routerToModify.Login,
                Password = routerToModify.Password,
                AllRoutersNames = RoutersConnections.GetAllRoutersNames().Except(new List<string>{router})
            };
            return PartialView("~/Views/Admin/PartialViews/_ModifyRouter.cshtml", routerToModifyModel);
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
            if(string.IsNullOrEmpty(name)) Json(false, JsonRequestBehavior.AllowGet);
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
            //var wlan = new WlanClient();  /play with this later

            name = name ?? RoutersConnections.GetFirstRouterName();
            if (name == null) return View("~/Views/Admin/NoRoutersError.cshtml");
            var sshConnection = RoutersConnections.GetConnectionByName(name);
            if (sshConnection == null) return null;
            var currentConfiguratrion = sshConnection.Send_UciShowWireless();

            var configurationTrimmed = new SendUciShowWirelessViewModel
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
        public ActionResult EditWirelessPartial(SendUciShowWirelessViewModel config)
        {
            return PartialView("~/Views/Admin/PartialViews/_EditWirelessConfiguration.cshtml", config);
        }

        [HttpPost]
        public ActionResult SaveWirelessPartial(SendUciShowWirelessViewModel config)
        {
            if (!ModelState.IsValid) return Json(false, JsonRequestBehavior.AllowGet);

            try
            {
                var sshConnection = RoutersConnections.GetConnectionByName(config.RouterName);
                if (sshConnection == null) throw new Exception();

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

        public ActionResult Firewall(string name = null)
        {
            name = name ?? RoutersConnections.GetFirstRouterName();
            if (name == null) return View("~/Views/Admin/NoRoutersError.cshtml");
            var sshConnection = RoutersConnections.GetConnectionByName(name);
            if (sshConnection == null) return null;

            var model = new FirewallViewModel
            {
                FirewallRestrictionRules = sshConnection.Get_AllFirewallRestrictionRules(),
                RouterName = name
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult RemoveRule(string ruleName, string routerName)
        {
            try
            {
                var sshConnection = RoutersConnections.GetConnectionByName(routerName);
                if (sshConnection == null) throw new Exception();

                sshConnection.Send_DeleteFirewallRestrictionRule(ruleName);
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
            return PartialView("~/Views/Admin/PartialViews/_AddRule.cshtml", routerName);
        }

        [HttpPost]
        public ActionResult SaveRule(AddFirewallRuleViewModel restrictionRule, string routerName)
        {
            if (!ModelState.IsValid) return Json(false, JsonRequestBehavior.AllowGet);

            try
            {
                if (routerName == null) routerName = RoutersConnections.GetFirstRouterName();
                var sshConnection = RoutersConnections.GetConnectionByName(routerName);
                var ruleName = sshConnection.Send_SaveFirewallRestrictionRule(restrictionRule);
                return Json(new { ruleName = ruleName, status = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}