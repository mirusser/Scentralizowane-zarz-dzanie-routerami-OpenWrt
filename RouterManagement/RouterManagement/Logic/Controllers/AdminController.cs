using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RouterManagement.Logic.Connections;

namespace RouterManagement.Logic.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendUciShow()
        {
            //TODO The sshConnection parameters should be "RouterAccessData" taken from repository or something like this
            //var sshConnection = new SshConnection("192.168.2.1", "root", "konopie1");
            //var currentConfiguratrion = sshConnection?.Send_UciShow();
            var currentConfiguratrion = new Dictionary<string, string>
            {
                {"1", "bla bla"},
                {"2", "bla"},
                {"3", "asfgasdfsadf"}
            };
            return View(currentConfiguratrion);
        }
    }
}