using System.Web.Mvc;

namespace RouterManagement.Logic.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}