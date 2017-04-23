using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Song_Web.Areas.domain.Controllers
{
    public class HomeController : Controller
    {
        // GET: domain/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}