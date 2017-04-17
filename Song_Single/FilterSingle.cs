using Song_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Song_Single
{
    /// <summary>
    /// 选择器实现权限及其他验证
    /// </summary>
    public class FilterSingle : ActionFilterAttribute
    {
        public class AdminPower : ActionFilterAttribute
        {
            /// <summary>
            /// 需要登录
            /// </summary>
            public bool IsNeedLogin { get; set; }
            /// <summary>
            /// 重写OnActionExecuting  todo: 需要修改 
            /// </summary>
            /// <param name="filterContext"></param>
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var controllerName = filterContext.RouteData.Values["controller"].ToString().ToLower();
                var actionName = filterContext.RouteData.Values["action"].ToString().ToLower();

                if (!IsNeedLogin)//不需要
                {
                    return;
                }
                var cookieId = Convert.ToInt32(CookieHelper.Get("adminuserid", HttpContext.Current.Request));
                var loginUser = GetModel.DB.AdminUser.FirstOrDefault(a => a.Id == cookieId);    //取缓存登录信息 取cookie
                if (loginUser == null)
                {
                    //判断是否是ajax请求
                    var request = filterContext.HttpContext.Request;
                    if ((request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest")))
                    {
                        return;
                    }
                    filterContext.Result = new RedirectResult("/login/index"); //RedirectToRoute("Default", new { Controller = "Login", Action = "Index" });
                    base.OnActionExecuting(filterContext);
                    return;
                }

                var userEntity = loginUser;

                if (userEntity.IsSuperMan)//超级管理员则不需要判断拥有所有权限
                {
                    return;
                }
            }
        }

    }
}
