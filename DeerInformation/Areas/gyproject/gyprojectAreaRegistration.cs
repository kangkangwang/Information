using System.Web.Mvc;

namespace DeerInformation.Areas.gyproject
{
    public class gyprojectAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "gyproject";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "gyproject_default",
                "gyproject/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
