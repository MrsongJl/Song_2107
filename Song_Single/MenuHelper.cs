using Song_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Song_Single
{
   public class MenuHelper : Controller
    {
        /// <summary>
        /// 更新权限项 未经测试
        /// </summary>
        /// <returns></returns>
        public ActionResult ActionPermissionUpdate()
        {
            var list = GetAllActionByAssembly();
            var aprs = GetModel.DB.ActionPermissionRole.ToList();

            var actionRoles = new List<ActionRole>();
            foreach (var item in aprs)
            {
                var ar = new ActionRole
                {
                    ControllerName = item.ActionPermission.ControllerName,
                    ActionName = item.ActionPermission.ActionName,
                    Description = item.ActionPermission.Description,
                    RoleName = item.Role.Name
                };
                actionRoles.Add(ar);
                GetModel.DB.ActionPermissionRole.Remove(item);
            }
            GetModel.DB.SaveChanges();

            var ls = GetModel.DB.ActionPermission;

            foreach (var item in ls)
            {
                GetModel.DB.ActionPermission.Remove(item);
            }
            GetModel.DB.SaveChanges();

            foreach (var item in list)
            {
                var ac = new ActionPermission
                {
                    ActionName = item.ActionName,
                    ControllerName = item.ControllerName,
                    Description = item.Description,
                    CreateTime = DateTime.Now,
                    IsAbort = false
                };
                GetModel.DB.ActionPermission.Add(ac);
            }
            GetModel.DB.SaveChanges();

            foreach (var item in actionRoles)
            {
                var apr = new ActionPermissionRole();
                var actionp = GetModel.DB.ActionPermission.FirstOrDefault(a => a.ActionName == item.ActionName & a.ControllerName == item.ControllerName & a.Description == item.Description);
                var role = GetModel.DB.Role.FirstOrDefault(a => a.Name == item.RoleName);
                apr.ActionPermission = actionp;
                apr.Role = role;
                GetModel.DB.ActionPermissionRole.Add(apr);
            }
            GetModel.DB.SaveChanges();
            return Content("ok");
        }
        /// <summary>
        /// 取含有标记的所有Action
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ActionPermission> GetAllActionByAssembly()
        {
            var result = new List<ActionPermission>();

            var types = Assembly.GetAssembly(typeof(MenuHelper)).GetTypes();

            foreach (var type in types)
            {
                if (type.BaseType != null && (type.BaseType.Name == "MenuHelper"))
                {
                    var members = type.GetMethods();
                    foreach (var member in members)
                    {
                        if (member.DeclaringType != null && (member.DeclaringType.Name != "MenuHelper" && member.CustomAttributes != null
                                                             && (member.ReturnType.Name == "ActionResult" || member.ReturnType.Name == "JsonResult")))
                        {
                            var ap = new ActionPermission
                            {
                                ActionName = member.Name,
                                ControllerName = member.DeclaringType.Name.Substring(0, member.DeclaringType.Name.Length - 10)
                            };
                            object[] attrs = member.GetCustomAttributes(typeof(DescriptionAttribute), true);
                            if (attrs.Length > 0)
                            {
                                var descriptionAttribute = attrs[0] as DescriptionAttribute;
                                if (descriptionAttribute != null)
                                    ap.Description = descriptionAttribute.Description;
                            }
                            result.Add(ap);
                        }
                    }
                }
            }
            return result;
        }
        public class ActionRole
        {
            public string ControllerName { get; set; }

            public string ActionName { get; set; }

            public string Description { get; set; }

            public string RoleName { get; set; }
        }




    }
}
