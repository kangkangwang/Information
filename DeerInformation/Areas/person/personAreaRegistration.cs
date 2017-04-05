using System.Web.Mvc;

namespace DeerInformation.Areas.person
{
    public class personAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "person";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "person_default",
                "person/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
