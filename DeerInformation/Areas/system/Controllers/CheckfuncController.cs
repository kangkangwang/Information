using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Areas.system.Models;
using System.Reflection;
using System.Resources;
using System.Web.Routing;
using DeerInformation.Extensions;
using Ext.Net.Utilities;
using Newtonsoft.Json.Linq;

namespace DeerInformation.Areas.system.Controllers
{
    /// <summary>
    /// editor：WKK
    /// date：2016-8-25
    /// </summary>
    [DirectController(AreaName = "system")]
    public class CheckfuncController : Controller
    {
        private Entities db = new Entities();
        //
        // GET: /system/Checkfunc/
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {

            return View(db.T_CH_Checkfunc.ToList());
        }

        //
        // GET: /system/Checkfunc/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /system/Checkfunc/Create
        [VisitAuthorize(Create = true)]
        public ActionResult Add()
        {
            WindowModule newwin = new WindowModule();
            newwin.Loader.Url = Url.Action("part", "Checkfunc", new { id = Guid.NewGuid() });
            newwin.Render();
            return this.Direct();
        }

        //
        // POST: /system/Checkfunc/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {

                T_CH_Checkfunc newitem = new T_CH_Checkfunc();
                TryUpdateModel<T_CH_Checkfunc>(newitem, collection);
                db.T_CH_Checkfunc.Add(newitem);
                db.SaveChanges();
                return this.Direct(true);
            }
            catch
            {
                return this.Direct(false);
            }
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Edits(string id)
        {
            WindowModule newwin = new WindowModule();
            newwin.Loader.Url = Url.Action("Edit", "Checkfunc", new { id = id });
            newwin.Render();
            return this.Direct();
        }

        //
        // GET: /system/Checkfunc/Edit/5

        public ActionResult Edit(string id)
        {

            return View(db.T_CH_Checkfunc.Find(id));
        }

        //
        // POST: /system/Checkfunc/Edit/5

        [HttpPost]
        public ActionResult Edit(T_CH_Checkfunc collection)
        {
            try
            {
                T_CH_Checkfunc obj_or = new T_CH_Checkfunc();
                foreach (var item in typeof(T_CH_Checkfunc).GetProperties())
                {
                    item.SetValue(obj_or, item.GetValue(collection, null), null);
                }
                db.Entry(obj_or).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                return this.Direct(true);
            }
            catch
            {
                return this.Direct(false);
            }
        }

        //
        // GET: /system/Checkfunc/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /system/Checkfunc/Delete/5

        [HttpPost]
        [VisitAuthorize(Delete = true)]
        public ActionResult delete(string selection)
        {
            try
            {
                var list = selection.JsonToList<T_CH_Checkfunc>();

                foreach (var item in list)
                {
                    var obj = db.T_CH_Checkfunc.Find(item.ID);
                    db.T_CH_Checkfunc.Remove(obj);
                }

                db.SaveChanges();
                return this.Direct(true);
            }
            catch
            {
                return this.Direct(false);
            }

        }

        [DirectMethod(Namespace = "system_func")]
        public ActionResult refresh()
        {
            this.GetCmp<Store>("storedata").LoadData(db.T_CH_Checkfunc.ToList());
            return this.Direct();
        }

        public ActionResult part(string id)
        {
            var item = db.T_CH_Checkfunc.Find(id);
            if (item != null)
            {
                return View(db.T_CH_Checkfunc.Find(id));
            }
            else
            {
                return View(new T_CH_Checkfunc() { ID = id });
            }

        }

        public ActionResult Expand(string funId, string name, string command)
        {
            if (command == "汇办")
            {
                WindowModule window = new WindowModule()
                {
                    Title = name,
                    Loader = { Url = Url.Action("Users", new { funId = funId }) }
                };
                window.Render(RenderMode.Auto);
            }
            return this.Direct();
        }

        public ActionResult UsersFilter(string keyWord, string funId)
        {
            X.GetCmp<Store>("storedata").LoadData(new CheckFun().UsersFilter(keyWord, funId));
            return this.Direct();
        }

        public ActionResult Users(string funId)
        {
            ViewBag.funId = funId;
            return View(new CheckFun().UsersBelongFun(funId));
        }

        public ActionResult SaveUsers(string users, string funId)
        {
            if (funId.IsNotEmpty())
            {
                CheckFun checkFun = new CheckFun();
                if (checkFun.AddCcpeopleToRole(users.JsonToList<dynamic>(), funId))
                {
                    return this.Direct();
                }
            }
            return this.Direct(false);
        }

        public ActionResult Cancel()
        {
            return this.Direct();
        }
    }
}
