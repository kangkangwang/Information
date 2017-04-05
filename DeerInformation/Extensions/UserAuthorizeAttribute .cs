using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.BaseType;


namespace DeerInformation.Extensions
{
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //检查Cookies["User"]是否存在
            if (httpContext.Request.Cookies["User"] == null) return false;
            //验证用户名密码是否正确
            HttpCookie cookie = httpContext.Request.Cookies["User"];
            string userid = HttpUtility.UrlDecode(cookie["UserID"]);
            string userName = HttpUtility.UrlDecode(cookie["UserName"]);
            string password = HttpUtility.UrlDecode(cookie["Password"]);

            if (userName == "" || password == "") return false;

            Authentication userAc = new Authentication();
            if (userAc.IsUser(userid, userName, password) == true) return true;
            else return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            //跳转的错误页
            filterContext.HttpContext.Response.Redirect("~/Home/Login");
        }


    }


}