using System.Web.Mvc;

namespace DeerInformation.Areas.workyard
{
    public class workyardAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "workyard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "workyard_default",
                "workyard/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
