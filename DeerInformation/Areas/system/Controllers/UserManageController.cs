using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Areas.system.Models;
using DeerInformation.Extensions;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Newtonsoft.Json.Linq;

namespace DeerInformation.Areas.system.Controllers
{
    /// <summary>
    /// editor：WKK
    /// date：2016-9-11
    /// </summary>
    public class UserManageController : Controller
    {
        //
        // GET: /system/UserManage/
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View(new UserManage());
        }

        public ActionResult CreateUser()
        {
            UserManage userManage = new UserManage(); ;
            return View(userManage);
        }

        [VisitAuthorize(Create = true)]
        public ActionResult Create()
        {
            WindowModule nwin = new WindowModule();
            nwin.Loader.Url = Url.Action("CreateUser");
            nwin.Render(RenderMode.Auto);
            return this.Direct();
        }

        [VisitAuthorize(Read = true)]
		public ActionResult Deatil(string id)
        {
            WindowModule nwin = new WindowModule();
			nwin.Loader.Url = Url.Action("DeatilUser",new {id=id});
            nwin.Render(RenderMode.Auto);
            return this.Direct();
        }


		public ActionResult DeatilUser(string id)
		{
			UserManage userManage = new UserManage();
			userManage.DetailUser(id);
			return View("Deatil",userManage);
		}

        [VisitAuthorize(Delete = true)]
        public ActionResult Forbid(string select)
        {
            var list = select.JsonToList<V_PE_UserRole>();
            if (new UserManage().ForbidUsers(list))
            {
                X.Msg.Alert("消息", "操作成功！", "window.location.reload()").Show();
            }
            else
            {
                X.Msg.Alert("消息", "操作失败，请重试！", "window.location.reload()").Show();
            }
            return this.Direct();
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Enable(string select)
        {
            var list = select.JsonToList<V_PE_UserRole>();
            if (new UserManage().EnableUsers(list))
            {
                X.Msg.Alert("消息", "操作成功！", "window.location.reload()").Show();
            }
            else
            {
                X.Msg.Alert("消息", "操作失败，请重试！", "window.location.reload()").Show();
            }
            return this.Direct();
        }

        public ActionResult Select(UserManage newuser)
        {
            this.GetCmp<Store>("storedata").LoadData(newuser.Users);
            return this.Direct();
        }

        public ActionResult Submit(UserManage newuser)
        {
            bool result = newuser.PasswordConfirm.Trim()==newuser.Password.Trim();
            if (ModelState.IsValid == true && result)
            {
                T_PE_Users user = new T_PE_Users
                {
                    UserID = Guid.NewGuid().ToString(),
                    UserName = newuser.UserName.Trim(),
                    Password = newuser.Password.Trim(),
                    EmployeeID = newuser.EmployeeID.Trim(),
                    CreateTime = DateTime.Now
                };
                T_PE_UserRoles userRoles = new T_PE_UserRoles { RoleID = newuser.RoleID, UserID = user.UserID };
                return this.Direct(new UserManage().CreateUser(user, userRoles));
            }
            return this.Direct(false );
        }

    }
}
