using System.Web.Mvc;

namespace DeerInformation.Areas.reportforms
{
    public class reportformsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "reportforms";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "reportforms_default",
                "reportforms/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
