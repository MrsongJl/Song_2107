using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Song_Single;
using Song_Public;

namespace Song_Web.Controllers
{
    public class testController : Controller
    {
        // GET: test
        public ActionResult Index()
        {
            TestHelper t = new Song_Single.TestHelper();
            t.start();
            return View();
        }

    }

   
}