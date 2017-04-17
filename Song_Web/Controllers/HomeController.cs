using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Song_Web.Controllers
{
    public class HomeController : Song_Single.BaseSingle
    {
        // GET: Home
        [Description("后台首页")]
        public ActionResult Index()
        {
            // Log4netProvider.Logger.Error("出错了");
            //t
            return View();
        }
    }
}