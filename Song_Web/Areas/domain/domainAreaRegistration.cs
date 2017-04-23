using System.Web.Mvc;

namespace Song_Web.Areas.domain
{
    public class domainAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "domain";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "domain_default",
                "domain/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}