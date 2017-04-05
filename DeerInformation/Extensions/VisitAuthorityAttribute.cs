using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DeerInformation.Models;

namespace DeerInformation.Extensions
{
    public class VisitAuthorizeAttribute : AuthorizeAttribute
    {
        public bool Read { get; set; }
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            LoginUser user = new LoginUser();
            Users = user.UserId;
            Authentication userAc = new Authentication();
            bool flag = userAc.IsAuthorised(httpContext, this);

            if (flag == true) return true;
            else return false;

        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            //跳转的错误页
            filterContext.HttpContext.Response.Redirect("~/Home/Unauthorized");
        }
    }
}