using DeerInformation.Models;
using Ext.Net.MVC;
using Ext.Net;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.gyproject.Controllers
{
    /// <summary>
    /// 客户信息增删查改
    /// gy
    /// 2016-6-23
    /// </summary>
    [DirectController(AreaName = "gyproject")]
    public class CustomInfoController : Controller
    {       
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();
        
        public ActionResult Index()
        {
            return View(DB.T_GM_CustomerInfo.ToList());
        }

        [DirectMethod]
        public ActionResult CustomerInfoReload()
        {
            var list = DB.T_GM_CustomerInfo.ToList();
            cw.Reload(list, "CustomerInfoSelect");
            return this.Direct();
        }


        #region 客户信息添加
        public ActionResult CustomerAddView()
        {
            return View();
        }
        [VisitAuthorize(Create = true)]
        public ActionResult CustomerAddButton()
        {
            WinModule win = new WinModule();
            win.ID = "window1";
            win.Title = "客户信息添加";
            win.Height = 450;
            win.Loader.Url=Url.Action("CustomerAddView", "CustomInfo");
            win.Listeners.Close.Handler="App.direct.gyproject.CustomerInfoReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult CustomerAdd(T_GM_CustomerInfo CustomerInfo)//客户信息提交
        {
            CustomerInfo.InputTime = DateTime.Now;
            CustomerInfo.InputPerson = user.EmployeeId;
            try
            {
                DB.T_GM_CustomerInfo.Add(CustomerInfo);
                DB.SaveChanges();
            }
            catch (Exception)
            {
                X.Msg.Alert("警告", "操作有误！<br /> note:");
            }
            return this.Direct();
        }
        #endregion

        #region 客户信息修改按钮
        public ActionResult CustomerEditView(string ID)
        {
            decimal t = 0;
            decimal.TryParse(ID, out t);
            return View(DB.T_GM_CustomerInfo.Find(t));
        }
        [VisitAuthorize(Update = true)]
        public ActionResult ClickEdit(string id)
        {
            WinModule win = new WinModule();
            win.ID = "window2";
            win.Title = "客户信息修改";
            win.Height = 450;
            win.Loader.Url = Url.Action("CustomerEditView", "CustomInfo", new { ID = id });
            win.Listeners.Close.Handler = "App.direct.gyproject.CustomerInfoReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult CustomerInfoEdit(T_GM_CustomerInfo CustomerInfo)
        {
            DirectResult r = new DirectResult();
            CustomerInfo.InputTime = DateTime.Now;
            CustomerInfo.InputPerson = user.EmployeeId;
            DB.T_GM_CustomerInfo.Attach(CustomerInfo);
            DB.Entry(CustomerInfo).State = EntityState.Modified;
            DB.SaveChanges();
            return r;
        }
        #endregion

        #region 客户信息查询
        [VisitAuthorize(Read = true)]
        public ActionResult SelectCustomer(string CustomerName)
        {
            if (CustomerName != "")
            {
                return this.Store(DB.T_GM_CustomerInfo.Where(w => w.CustomerName.Contains(CustomerName)));
            }
            else
            {
                return this.Store(DB.T_GM_CustomerInfo.ToList());
            }
        }

        #endregion

        #region 客户信息删除
        [DirectMethod]
        [VisitAuthorize(Delete = true)]
        public ActionResult ClickDelete2(string id)
        {
            decimal t = 0;
            decimal.TryParse(id, out t);
            var record = DB.T_GM_CustomerInfo.Find(t);
            DB.T_GM_CustomerInfo.Remove(record);
            DB.SaveChanges();
            return this.Direct();
        }

        #endregion

    }
}
