using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Song_Single;

namespace Song_Web.Areas.domain.Controllers
{
    public class TestController : BaseSingle
    {
        // GET: domain/Test
        public ActionResult Index()
        {
            ITest te = new Testimpl();
            var model= te.Test();
            return View(model);
        }
    }
}