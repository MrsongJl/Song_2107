using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Song_Single.FilterSingle;

namespace Song_Web.Controllers
{
    public class HomeController : Song_Single.BaseSingle
    {
        // GET: Home
        //[AdminPower(IsNeedLogin = true)]
        [Description("后台首页")]
        public ActionResult Index()
        {
            // Log4netProvider.Logger.Error("出错了");
            //t
            return View();
        }
    }
}