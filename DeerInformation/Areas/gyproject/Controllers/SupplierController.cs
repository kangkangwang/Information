using DeerInformation.Models;
using Ext.Net.MVC;
using Ext.Net;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.gyproject.Controllers
{
    /// <summary>
    /// 供货商信息增删查改
    /// gy
    /// 2016-6-22
    /// </summary>
    [DirectController(AreaName = "gyproject")]
   
    public class SupplierController : Controller
    {
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View(DB.T_GM_SupplierInfo.ToList());
        }

        [DirectMethod]
        public ActionResult SupplierInfoReload()
        {
            var list = DB.T_GM_SupplierInfo.ToList();
            cw.Reload(list, "SupplierInfoSelect");
            return this.Direct();
        }

        #region 供应商添加
        public ActionResult SupplierAddView()
        {
            return View();
        }
        [VisitAuthorize(Create = true)]
        public ActionResult SupplierAddButton()
        {
            WinModule win = new WinModule();
            win.ID = "window1";
            win.Title = "供应商信息添加";
            win.Height = 400;
            win.Loader.Url = Url.Action("SupplierAddView", "Supplier");
            win.Listeners.Close.Handler = "App.direct.gyproject.SupplierInfoReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult SupplierAdd(T_GM_SupplierInfo SupplierInfo)//供应商信息提交
        {
            SupplierInfo.InputTime = DateTime.Now;
            SupplierInfo.InputPerson = user.EmployeeId;
            try
            {
                DB.T_GM_SupplierInfo.Add(SupplierInfo);
                DB.SaveChanges();  
            }
            catch (Exception)
            {
                //X.Msg.Alert("警告", "操作有误！", new JFunction { Fn = "showResult" }).Show();
            }
           
            return this.Direct();
        }
        #endregion
             
        #region 供应商修改按钮
        public ActionResult SupplierEditView(string ID)
        {
            decimal t;
            decimal.TryParse(ID, out t);
            return View(DB.T_GM_SupplierInfo.Find(t));
        }
        [VisitAuthorize(Update = true)]
        public ActionResult ClickEdit(string id)
        {
            WinModule win = new WinModule();
            win.ID = "window2";
            win.Title = "供应商信息修改";
            win.Height = 400;
            win.Loader.Url = Url.Action("SupplierEditView", "Supplier", new { ID = id });
            win.Listeners.Close.Handler = "App.direct.gyproject.SupplierInfoReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult SupplierInfoEdit(T_GM_SupplierInfo SupplierInfo)
        {
            DirectResult r = new DirectResult();
            SupplierInfo.InputTime = DateTime.Now;
            SupplierInfo.InputPerson = "admin";
            DB.T_GM_SupplierInfo.Attach(SupplierInfo);
            DB.Entry(SupplierInfo).State = EntityState.Modified; 
            DB.SaveChanges();
            return r;
        }
        #endregion

        #region 供应商查询
       
        public ActionResult SelectSupplier(string suppliername)
        {
            
            if (suppliername!="")
            {
                return this.Store(DB.T_GM_SupplierInfo.Where(w => w.SupplierName.Contains(suppliername)));      
            }
            else
            {
                return this.Store(DB.T_GM_SupplierInfo.ToList());
            }
        }

        #endregion

        #region 供应商信息删除
        [DirectMethod]
        [VisitAuthorize(Delete = true)]
        public ActionResult DeleteSupply(string id)
        {
            decimal t;
            decimal.TryParse(id, out t);
            var record = DB.T_GM_SupplierInfo.Find(t);
            DB.T_GM_SupplierInfo.Remove(record);
            DB.SaveChanges();
            return this.Direct();
        }

        #endregion

    }
}
