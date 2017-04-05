using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Data;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.gyproject.Controllers
{
    [DirectController(AreaName = "gyproject")]
    public class ProjectController : Controller
    {
        //
        // GET: /gyproject/Project/
        Entities DB = new Entities();
        CommonWay cw = new CommonWay();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View(DB.V_GM_DetailProject.ToList());//.Where(m=>m.Description=="审核通过")
        }

		public ActionResult SelectProject(string clientname, string projectname)
		{
			var reslut =
				DB.V_GM_DetailProject.Where(
					l => l.ClientName.Contains(clientname ?? "") && l.ProjectName.Contains(projectname ?? "")).ToList();
			return this.Store(reslut);
	    }
        #region 查看项目进度
        public ActionResult SeeFieldSchedule(string id)
        {
            WinModule win = new WinModule();
            win.ID = "window3";
            win.Loader.Url = Url.Action("FieldScheduleSeeView", new { id = id });
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult FieldScheduleSeeView(string id)
        {
            string prono = DB.T_GM_Budget.Find(DB.T_GM_Project.Find(id).BudgetGID).ProjectNo;
            return View(DB.T_GW_FieldSchedule.Find(prono));
        }

        #endregion

        #region 查看已付金额
        //已收货单为准
        public ActionResult SeePaid(string id)
        {
            WinModule win = new WinModule();
            win.ID = "window1";
            win.Loader.Url = Url.Action("PaidSeeView", new { id = id });
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult PaidSeeView(string id)
        {
            string prono = DB.T_GM_Budget.Find(DB.T_GM_Project.Find(id).BudgetGID).ProjectNo;

            decimal total = 0;
            var list = DB.V_GM_DetailRecieve.Where(w => w.ProjectNo == prono).ToList();
            foreach (var item in list)
            {
                var price = DB.V_GM_DM.Where(w => w.Remark == item.ReceivePMNo).ToList();
                foreach (var material in price)
                {
                    total += (material.Num ?? 0) * (material.Price ?? 0);
                }
            }
            ViewBag.total = total.ToString();
            return View(list);
        }
        
        #endregion

        #region 查看退回材料金额
        public ActionResult SeeBack(string id)
        {
            WinModule win = new WinModule();
            win.ID = "window2";
            win.Loader.Url = Url.Action("BackSeeView", new { id = id });
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult BackSeeView(string id)
        {
            string prono = DB.T_GM_Budget.Find(DB.T_GM_Project.Find(id).BudgetGID).ProjectNo;
            decimal total = 0;
            var list = DB.V_GM_MResidual.Where(w => w.ProjectNo == prono).Where(w=>w.Description=="审核通过").ToList();
            foreach (var item in list)
            {
                var price = DB.V_GM_DM.Where(w => w.Remark == item.GID).ToList();
                foreach (var material in price)
                {
                    total += (material.Num ?? 0) * (material.Price ?? 0);
                }
            }
            ViewBag.total2 = total.ToString();
            return View(list);
        }
        #endregion


    }
}
