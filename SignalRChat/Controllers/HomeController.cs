using SignalRChat_Common.Tool;
using SignalRChat_Model;
using System;
using System.Web;
using System.Web.Mvc;

namespace SignalRChat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}