﻿using System.Web.Mvc;
using RouterManagement.Logic.Connections;

namespace RouterManagement.Logic.Controllers
{
    public class ApiController : Controller
    {
        [HttpPost]
        public ActionResult GetRouterNames()
        {
            var names = Routers.GetOnlineRoutersNames();
            return Json(names, JsonRequestBehavior.AllowGet);
        }
    }
}