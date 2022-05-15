using Evry.Evaluation.Manager;
using Evry.Evaluation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Evry.Evaluation.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var model = new List<EventViewModel>();
            var manager = new EventManager();
            model = manager.GetEventList();
            return View(model);
        }

        public ActionResult Test()
        {
            var model = new List<EventViewModel>();
            var manager = new EventManager();
            model = manager.GetEventList();
            return View(model);
        }
    }
}