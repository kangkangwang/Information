using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeerInformation.Models
{
    /// <summary>
    /// 已登录信息类 读取已登录用户信息
    /// </summary>
    public class LoginUser
    {
        public LoginUser()
        {
            var httpContext = HttpContext.Current;
            if (httpContext.Request.Cookies["User"] != null)
            {
                HttpCookie cookie = httpContext.Request.Cookies["User"];
                UserId = HttpUtility.UrlDecode(cookie["UserID"]);
                UserName = HttpUtility.UrlDecode(cookie["UserName"]);
                Password = HttpUtility.UrlDecode(cookie["Password"]);
            }
        }
        /// <summary>
        ///用户id
        /// </summary>
        public string UserId
        {
            get;
            set;

        }

        /// <summary>
        /// 用户名（用户登录名）
        /// </summary>
        public string UserName
        {
            get;
            set;
        }
        /// <summary>
        /// 散列加密后密码
        /// </summary>
        public string Password { get; set; }

        //用户工号
        public string EmployeeId
        {
            get
            {
                using (Entities db = new Entities())
                {
                    return db.T_PE_Users.Find(UserId).EmployeeID;
                }
            }
        }

        //员工姓名
        public string EmployeeName
        {
            get
            {
                using (Entities db = new Entities())
                {
                    return db.T_HR_Staff.Find(EmployeeId).Name;
                }
            }
        }
    }
}