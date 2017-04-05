using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Routing;
using DeerInformation.Models;

namespace DeerInformation.Extensions
{
    /// <summary>
    /// editor:WKK
    /// date:2016-6-13
    /// </summary>
    public class Authentication
    {
        private Entities db;

        public Authentication()
        {
            db = new Entities();
        }

        /// <summary>
        /// 用户登录验证
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsUser(string userid, string username, string password)
        {

            if (db.T_PE_Users.Count(li => li.Activity == true && li.UserName == username && li.UserID == userid) > 0)
            {
                var users = db.T_PE_Users.Where(li => li.UserName == username).ToList();
                //用户名重复 返回错误
                if (users.Count() > 1)
                {
                    return false;
                }
                var employeeId = users.First().EmployeeID;
                //未注册员工基本信息 返回错误
                if (db.T_HR_Staff.FirstOrDefault(l => l.StaffID == employeeId) == null)
                {
                    return false;
                }
                //密码错误 返回错误
                if (EncryptionCommon.Sha256(users.First().Password) == password)
                {
                    return true;
                }
                
            }

            return false;
        }
        //验证权限
        public bool IsAuthorised(HttpContextBase httpContext, VisitAuthorizeAttribute visitAuthorizeAttribute)
        {
            string controller = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            string action = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();
            LoginUser user = new LoginUser();
            string stringPattern = string.Format("/%/{0}%", controller);
            using (Entities db = new Entities())
            {
                var permission = db.V_PE_UserPermission.Where(
                    l =>
                        l.UserID == user.UserId && l.Activity == true && SqlFunctions.PatIndex(stringPattern, l.URL) > 0)
                    .ToList();
                if (visitAuthorizeAttribute.Read && !permission.Where(l => l.Action == "Read").ToList().Any())
                {
                    return false;
                }
                if (visitAuthorizeAttribute.Update && !permission.Where(l => l.Action == "Update").ToList().Any())
                {
                    return false;
                }
                if (visitAuthorizeAttribute.Create && !permission.Where(l => l.Action == "Create").ToList().Any())
                {
                    return false;
                }
                if (visitAuthorizeAttribute.Delete && !permission.Where(l => l.Action == "Delete").ToList().Any())
                {
                    return false;
                }
            }
            return true;
        }

    }
}