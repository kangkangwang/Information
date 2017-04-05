using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using DeerInformation.Areas.gyproject.Models;
using DeerInformation.BaseType;
using DeerInformation.Extensions;
using DeerInformation.Models;
using Ext.Net.MVC;
using Newtonsoft.Json.Linq;

namespace DeerInformation.Areas.gyproject.Controllers
{
    public class IMWarehouseController : Controller
    {
        //
        // GET: /gyproject/IMWarehouse/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IMFilter(StoreRequestParameters parameters,string IMNO="",string IMType="",string CheckState="")
        {
            return this.Store(new IMMaterial().IMLst(IMNO,IMType,CheckState).GetPage(parameters));
        }

        public ActionResult Opearation(string action)
        {
            string loderUrl = Url.Action(action);
            WindowModule window=new WindowModule()
            {
                Title = action,
                Height = 745,
                Width = 1092,
                Loader =  {Url= loderUrl }
            };
            window.Render(RenderMode.Auto);
            return this.Direct();
        }

        #region 采购入库
        public ActionResult Purchase()
        {
            return View(new IMMaterial() { CheckFunName = "采购入库" });
        }

        public ActionResult FillDetailByPurchaseNo(FormCollection paramsCollection)
        {
            string pId = paramsCollection["PurchaseMNo"];
            dynamic result = new IMMaterial().GetDetailByPurchaseNo(pId);
            if (result != null)
            {
                X.GetCmp<TextField>("ProjectName").Text = result.ProjectNo;
                X.GetCmp<TextField>("Warehouse").Text = result.Warehouse;
                X.GetCmp<Store>("ApplyStore").LoadData(result.matLst);
            }
            return this.Direct();
        }

        public ActionResult GetPurchaseMNo(FormCollection paramsCollection)
        {
            string queryStr = paramsCollection["query"];
            using (Entities db = new Entities())
            {
                var res = db.V_GM_MPurchase.Where(l => l.PurchaseMNo.Contains(queryStr) && l.IsEnableNo == "审核通过")
                    .Select(s => new { PurchaseMNo = s.PurchaseMNo })
                    .Distinct().Take(5)
                    .ToList();
                return this.Store(res);
            }
        }
        #endregion

        #region Allotment
        public ActionResult Allotment()
        {
            return View(new IMMaterial() {CheckFunName = "调拨入库"});
        }

        public ActionResult GetAllocationNo(FormCollection paramsCollection)
        {
            string queryStr = paramsCollection["query"];
            using (Entities db = new Entities())
            {
                var res = db.T_GM_EXWarehouse.Where(l => l.EXID.Contains(queryStr) && l.EXTypeID == (int)WarehouseType.AllotmentEX)
                    .Select(s => new { EXID = s.EXID })
                    .Distinct().Take(5)
                    .ToList();
                return this.Store(res);
            }
        }

        public ActionResult FillDetailByAllotmentNo(FormCollection paramsCollection)
        {
            string aId = paramsCollection["AllotmentNo"];
            dynamic result = new IMMaterial().GetDetailByAllotmentNo(aId);
            if (result != null)
            {
                X.GetCmp<TextField>("EXWarehouse").Text = result.EXWarehouse;
                X.GetCmp<TextField>("IMWarehouse").Text = result.IMWarehouse;
                X.GetCmp<DateField>("EXDate").Value = result.EXDate;
                X.GetCmp<Store>("ApplyStore").LoadData(result.matLst);
            }
            return this.Direct();
        } 
        #endregion

        public ActionResult Stocktaking()
        {
            return View(new IMMaterial() { CheckFunName = "盘存入库" });
        }

        public ActionResult ReadLst(string id)
        {
            PrintWindow window = new PrintWindow()
            {
                Title = "详细清单",
                Loader = { Url = Url.Action("ReadApplymentLst", new { id = id }) }
            };
            window.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult ReadApplymentLst(string id)
        {
            var obj= new IMMaterial().GetApplymentLst(id);
            if (obj == null) return View("Error");
            var lst = obj.result as V_GM_IMWithStateDsp;
            ViewBag.No = lst.IMID;
            ViewBag.date = lst.IMDate.ToString("u");
            ViewBag.EIType = lst.IMType;
            ViewBag.ProjectNo = lst.ProjectID;
			ViewBag.EXWarehouse = lst.EXWarehouseID;
            ViewBag.IMWarehouse = lst.IMWarehouseID;
            ViewBag.Operator = lst.Operator;
            ViewBag.OperateTime = lst.OperationTime.ToString("u");
            ViewBag.CheckState = lst.CheckStateDsp;
            ViewBag.Remark = obj.remark;
            ViewBag.data = new IMMaterial().GetApplymentMatLst(id);
            return View("Detail");
        }

        public ActionResult SubmitApply(FormCollection requstParamsCollection)
        {
            if (!ModelState.IsValid) return this.Direct(false);

            string checkFlowId = requstParamsCollection["CheckFlowId"];
            if (requstParamsCollection["action"] == "purchase")//采购入库
            {
                string purchaseMNo = requstParamsCollection["PurchaseMNo"];
                string store = requstParamsCollection["store"];
                string imWarehouse = requstParamsCollection["Warehouse"];
                DateTime date = Convert.ToDateTime(requstParamsCollection["IMDate"]);
                JArray src = JArray.Parse(store);
                List<dynamic> materiaList = new List<dynamic>();
                foreach (var item in src)
                {
                    materiaList.Add(
                    JSON.Deserialize<dynamic>(item.ToString()));
                }
                if (!ValidateTool.ValidNumCheck(materiaList, "UIMNum", "ApplyNumber"))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
                if (!new IMMaterial() {CheckFlowId = checkFlowId }.SubmitPurchase(this,purchaseMNo, date, materiaList, imWarehouse))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
            }
            else if (requstParamsCollection["action"] == "allotment")//调拨入库
            {
                string allotmentNo = requstParamsCollection["AllotmentNo"];
                string EXWarehouseId = requstParamsCollection["EXWarehouse"];
                string IMWarehouseId = requstParamsCollection["IMWarehouse"];
                string store = requstParamsCollection["store"];
                DateTime date = Convert.ToDateTime(requstParamsCollection["IMDate"]);
                JArray src = JArray.Parse(store);
                List<dynamic> materiaList = new List<dynamic>();
                foreach (var item in src)
                {
                    materiaList.Add(
                    JSON.Deserialize<dynamic>(item.ToString()));
                }
                if (!ValidateTool.ValidNumCheck(materiaList, "UIMNum", "ApplyNumber"))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
                if (!new IMMaterial() { CheckFlowId = checkFlowId }.SubmitAllotment(this, allotmentNo, EXWarehouseId, IMWarehouseId, date, materiaList))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
            }
            else if (requstParamsCollection["action"] == "stocktaking")//盘存入库
            {
                string IMId = requstParamsCollection["IMID"];
                string IMWarehouseId = requstParamsCollection["IMWarehouse"];
                string store = requstParamsCollection["store"];
                DateTime date = Convert.ToDateTime(requstParamsCollection["IMDate"]);
                JArray src = JArray.Parse(store);
                List<dynamic> materiaList = new List<dynamic>();
                foreach (var item in src)
                {
                    materiaList.Add(
                    JSON.Deserialize<dynamic>(item.ToString()));
                }
                if (!ValidateTool.ValidNumCheck(materiaList, "CurAmount", "ApplyNumber"))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
                if (!new IMMaterial() { CheckFlowId = checkFlowId }.SubmitStocktaking(this, IMId, IMWarehouseId, date, materiaList))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
            }
            X.AddScript("parent.App.datastore.reload();");
            X.AddScript("parent.App.win.close();"); 
            return this.Direct();
        }

        public ActionResult CheckIMAction(string id)
        {
            var obj = new IMMaterial().GetApplymentLst(id);
            if (obj == null) return View("Expire");
            var lst = obj.result as V_GM_IMWithStateDsp;
            if (lst == null ) return View("Expire");
            ViewBag.No = lst.IMID;
            ViewBag.date = lst.IMDate.ToString("u");
            ViewBag.EIType = lst.IMType;
            ViewBag.ProjectNo = lst.ProjectID;
            ViewBag.EXWarehouse = lst.EXWarehouseID;
            ViewBag.IMWarehouse = lst.IMWarehouseID;
            ViewBag.Operator = lst.Operator;
            ViewBag.OperateTime = lst.OperationTime.ToString("u");
            ViewBag.CheckState = lst.CheckStateDsp;
            ViewBag.Remark = obj.remark;
            ViewBag.data = new IMMaterial().GetApplymentMatLst(id);
            ViewBag.CheckStateItems = new IMMaterial().CheckStateItems;
            ViewBag.OperationId = lst.OperationListID;
            return View("CheckIM");
        }

        public ActionResult SubmitCheckApply(string id, FormCollection requstParamsCollection)
        {
            string state = requstParamsCollection["CheckState"];
            string remark = requstParamsCollection["Remark"];
            if (new IMMaterial().SubmitCheck(id, state, remark))
            {
                X.Msg.Alert("页面消息", "操作成功!", "history.go(-1);parent.App.win.close();").Show();
            }
            else
            {
                X.Msg.Alert("页面消息", "操作失败！").Show();
            }
            return this.Direct();
        }
    }
}
