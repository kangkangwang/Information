using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Areas.system.Models;
using DeerInformation.Extensions;
using Ext.Net.Utilities;
using Newtonsoft.Json.Linq;

namespace DeerInformation.Areas.system.Controllers
{
    /// <summary>
    /// editor：WKK
    /// date：2016-9-10
    /// </summary>
    [DirectController(AreaName = "system")]
    public class RolesController : Controller
    {
        //private Entities db = new Entities();

        //
        // GET: /system/Roles/
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View(new RolesViewModle());
        }

        [DirectMethod]
        public ActionResult GetData()
        {
            var viewmodle = new RolesViewModle();
            this.GetCmp<Store>("storedata").LoadPage(viewmodle.pageindex);
            this.GetCmp<Store>("storedata").LoadData(viewmodle.data);
            return this.Direct();
        }

        #region 客户端响应按钮
        [VisitAuthorize(Create = true)]
        public ActionResult add(string selection)
        {
            WindowModule win = new WindowModule();
            win.Title = "增加";
            win.Loader = new ComponentLoader() { Url = Url.Action("CreatePage", new { name = win.ID }), Mode = LoadMode.Frame };
            win.Render(true);
            return this.Direct();
        }

        [VisitAuthorize(Update = true)]
        public ActionResult edit(string selection)
        {
            WindowModule win = new WindowModule();
            win.Title = "编辑";
            win.Loader = new ComponentLoader() { Url = Url.Action("EditPage", new { id = selection, name = win.ID }), Mode = LoadMode.Frame };
            win.Render(true);
            return this.Direct();
        }
        [VisitAuthorize(Delete = true)]
        public ActionResult delete(string selection)
        {
            JArray src = JArray.Parse(selection);
            T_PE_Roles[] roles = new T_PE_Roles[src.Count];
            foreach (var item in src)
            {
                roles[src.IndexOf(item)] = JSON.Deserialize<T_PE_Roles>(src[src.IndexOf(item)].ToString());
            }
            return this.Direct(new RolesViewModle().delete(roles));
        }
        #endregion

        public ActionResult CreatePage(string name)
        {
            T_PE_Roles item = new T_PE_Roles();
            item.RoleID = Guid.NewGuid().ToString();
            ViewBag.winname = name;
            return View(item);
        }

        public ActionResult Create(T_PE_Roles obj)
        {
            return this.Direct(new RolesViewModle().insert(obj));
        }

        public ActionResult Detail(T_PE_Roles obj)
        {
            T_PE_Roles obj_or = new RolesViewModle().db.T_PE_Roles.Find(obj.RoleID);
            foreach (var item in typeof(T_PE_Roles).GetProperties())
            {
                item.SetValue(obj_or, item.GetValue(obj, null), null);
            }
            return this.Direct(new RolesViewModle().update(obj_or));
        }

        public ActionResult EditPage(string id, string name)
        {
            ViewBag.winname = name;
            return View(new RolesViewModle().db.T_PE_Roles.Find(id));
        }

        public ActionResult GetRoot()
        {
            object roleid = TempData.Peek("roleid");
            if (roleid == null)
            {
                return this.Direct(false, "找不到角色，无法进行授权操作！");
            }
            this.GetCmp<TreePanel>("treepanel").SetRootNode(new RolesViewModle().LoadNodes(roleid.ToString()));
            return this.Direct();
        }

        public ActionResult PageBack( )
        {
            return this.RedirectToAction("Index");
        }

        public ActionResult Expand(string roleid, string command, string description)
        {
            if (command == "查看")
            {
                return this.RedirectToAction("authority", new { roleid = roleid }); 
            }

            if (command == "更改组成员")
            {
                WindowModule window = new WindowModule { Loader = { Url = Url.Action("Users", new { roleid = roleid }) }, Title = description };
                window.Render(RenderMode.Auto);
            } 
            return this.Direct();
        }

        public ActionResult UsersFilter(string keyWord ,string roleid )
        {
            X.GetCmp<Store>("storedata").LoadData(new RolesViewModle().UsersFilter(keyWord,roleid));
            return this.Direct();
        }

        public ActionResult Users(string roleid, string description)
        {
            ViewBag.roleid = roleid;
            return View(new RolesViewModle().UsersBelongRole(roleid));
        }

        public ActionResult SaveUsers(string users,string roleid)
        {
            if (roleid.IsNotEmpty())
            {
                RolesViewModle roles = new RolesViewModle();
                if (roles.AddUsersToRole(users.JsonToList<dynamic>(), roleid))
                {
                    return this.Direct();
                }
            }
            return this.Direct(false);

        }
        #region 权限配置
        public ActionResult authority(string roleid)
        {
            string name = new RolesViewModle().db.T_PE_Roles.Find(roleid).Description;
            ViewBag.name = string.Format("授权给:{0}", name);
            TempData["roleid"] = roleid;
            return View(new RolesViewModle().LoadNodes(roleid));
        }

        public ActionResult SubmitNodes(SubmittedNode data)
        {
            object roleid = TempData.Peek("roleid");

            if (roleid == null)
            {
                return this.Direct(false, "找不到角色，无法进行授权操作！");
            }
            else
            {
                try
                {
                    RolesViewModle modle = new RolesViewModle();
                    modle.GrandAuthority(data, roleid.ToString());
                    this.GetCmp<TreePanel>("treepanel").SetRootNode(new RolesViewModle().LoadNodes(roleid.ToString()));
                    X.Msg.Alert("页面消息","权限设置已保存！").Show();
                }
                catch (Exception e)
                {
                    return this.Direct(false, e.Message);

                }
                return this.Direct();
            }
        }

        #endregion
    }
}