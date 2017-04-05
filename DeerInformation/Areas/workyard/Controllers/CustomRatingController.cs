using System;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Extensions;


namespace DeerInformation.Areas.workyard.Controllers
{
    [DirectController(AreaName = "workyard")]
    public class CustomRatingController : Controller
    {
        //
        // GET: /workyard/CustomRating/
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();

        [VisitAuthorize(Read= true)]
        public ActionResult Index()
        {
            return View(DB.T_GW_CustomRating.ToList());
        }
        [DirectMethod]
        public ActionResult CustomRatingReload()//重新加载物资信息管理页面
        {
            var list = DB.T_GW_CustomRating.ToList();
            cw.Reload(list, "FixedAssetSelected");
            return this.Direct();
        }
        [VisitAuthorize(Create = true)]
        public ActionResult AddButton()
        {
            WinModule win = new WinModule();
            win.ID = "window2";
            win.Title = "客户评价添加";
            win.Height = 320;
            win.Width = 400;
            win.Loader.Url = Url.Action("CustomRatingAddView", "CustomRating");
            win.Listeners.Close.Handler = "App.direct.workyard.CustomRatingReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult CustomRatingAddView()
        {
            return View();
        }

        public ActionResult CRSubmit(T_GW_CustomRating cr)
        {
            cr.Appraiser = user.EmployeeId;
            cr.AppraisTime = DateTime.Now;
            DB.T_GW_CustomRating.Add(cr);
            DB.SaveChanges();
            return this.Direct();
        }

    }
}
