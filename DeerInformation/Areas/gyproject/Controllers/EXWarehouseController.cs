using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Areas.gyproject.Models;
using DeerInformation.BaseType;
using DeerInformation.Extensions;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using Newtonsoft.Json.Linq;

namespace DeerInformation.Areas.gyproject.Controllers
{
    public class EXWarehouseController : Controller
    {
        //
        // GET: /gyproject/EXWarehouse/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EXFilter(StoreRequestParameters parameters,string EXNO="", string EXType="", string CheckState="")
        {
            return this.Store(new EXMaterial().EXLst(EXNO,EXType,CheckState).GetPage(parameters));
        }

        public ActionResult Opearation(string action)
        {
            string loderUrl = Url.Action(action);
            WindowModule window = new WindowModule()
            {
                Title = action,
                Height = 745,
                Width = 1092,
                Loader = { Url = loderUrl }
            };
            window.Render(RenderMode.Auto);
            return this.Direct();
        }

        #region 领用出库
        public ActionResult Requisition()
        {
            return View(new EXMaterial() { CheckFunName = "领用出库" });
        }

        public ActionResult AddNewMaterial(string materials, string desGridId)
        {
            JArray src = JArray.Parse(materials);
            List<dynamic> materialsLst = new List<dynamic>();
            foreach (var item in src)
            {
                materialsLst.Add(
                JSON.Deserialize<dynamic>(item.ToString()));
            }

            X.GetCmp<Store>(desGridId).Add(new EXMaterial().GetNewMaterialLst(materialsLst));
            return this.Direct();
        } 
        #endregion

        public ActionResult Allotment()
        {
            return View(new EXMaterial() {CheckFunName = "调拨出库"});
        }

        public ActionResult Stocktaking()
        {
            return View(new EXMaterial() { CheckFunName = "盘存出库" });
        }

        public ActionResult ReadLst(string id)
        {
            PrintWindow window = new PrintWindow()
            {
                Title = "详细清单",
                Loader = {Url = Url.Action("ReadApplymentLst", new {id = id})}
            };
            window.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult ReadApplymentLst(string id)
        {
            var obj = new EXMaterial().GetApplymentLst(id);
            if (obj == null) return View("Error");
            var lst= obj.result as V_GM_EXWithStateDsp;
            if (lst == null ) return View("Expire");
            ViewBag.No = lst.EXID;
            ViewBag.date = lst.EXDate.ToString("u");
            ViewBag.EIType = lst.EXType;
            ViewBag.ProjectNo = lst.ProjectID;
            ViewBag.EXWarehouse = lst.EXWarehouse;
            ViewBag.IMWarehouse = lst.IMWarehouse;
            ViewBag.Operator = lst.Operator;
            ViewBag.OperateTime = lst.OperationTime.ToString("u");
            ViewBag.CheckState = lst.CheckStateDsp;
            ViewBag.Remark = obj.remark;
            ViewBag.data=new EXMaterial().GetApplymentMatLst(id);
            ViewBag.CheckStateItems = new EXMaterial().CheckStateItems;
            return View("Detail");
        }
        public ActionResult SubmitApply(FormCollection requstParamsCollection)
        {
            if (!ModelState.IsValid) return this.Direct(false);
            string checkFlowId = requstParamsCollection["CheckFlowId"];
            if (requstParamsCollection["action"] == "requisition")
            {
                string projectId = requstParamsCollection["projectID"];
                string store = requstParamsCollection["store"];
				string eXWarehouse = requstParamsCollection["EXWarehouse"];
                DateTime date = Convert.ToDateTime(requstParamsCollection["EXDate"]);
                JArray src = JArray.Parse(store);
                List<dynamic> materiaList = new List<dynamic>();
                foreach (var item in src)
                {
                    materiaList.Add(
                    JSON.Deserialize<dynamic>(item.ToString()));
                }
                if (!ValidateTool.ValidNumCheck(materiaList, "VirtualAmount", "ApplyNumber"))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
                if (!new EXMaterial() {CheckFlowId = checkFlowId }.SubmitRequisition(this,projectId, date, materiaList, eXWarehouse))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
            }
            else if (requstParamsCollection["action"] == "allotment")//调拨出库
            {
                string allotmentNo = requstParamsCollection["AllotmentNo"];
                string EXWarehouseId = requstParamsCollection["EXWarehouse"];
                string IMWarehouseId = requstParamsCollection["IMWarehouse"];
                string store = requstParamsCollection["store"];
                DateTime date = Convert.ToDateTime(requstParamsCollection["EXDate"]);
                JArray src = JArray.Parse(store);
                List<dynamic> materiaList = new List<dynamic>();
                foreach (var item in src)
                {
                    materiaList.Add(
                    JSON.Deserialize<dynamic>(item.ToString()));
                }
				if (!ValidateTool.ValidNumCheck(materiaList, "PurchaseAmount", "ApplyNumber"))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
                if (!new EXMaterial() { CheckFlowId = checkFlowId }.SubmitAllotment(this,allotmentNo, EXWarehouseId, IMWarehouseId, date, materiaList))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
            }
            else if (requstParamsCollection["action"] == "stocktaking")//盘存出库
            {
                string EXId = requstParamsCollection["EXID"];
                string EXWarehouseId = requstParamsCollection["EXWarehouse"];
                string store = requstParamsCollection["store"];
                DateTime date = Convert.ToDateTime(requstParamsCollection["EXDate"]);
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
                if (!new EXMaterial() { CheckFlowId = checkFlowId }.SubmitStocktaking(this,EXId, EXWarehouseId, date, materiaList))
                {
                    X.Msg.Alert("页面消息","请确认申请数量信息输入正确").Show();
                    return this.Direct();
                }
            }
            X.AddScript("parent.App.datastore.reload();");
            X.AddScript("parent.App.win.close();");
            return this.Direct();
        }

        public ActionResult CheckEXAction(string id)
        {
            var obj = new EXMaterial().GetApplymentLst(id);
            if (obj == null) return View("Expire");
            var lst = obj.result as V_GM_EXWithStateDsp;
            ViewBag.No = lst.EXID;
            ViewBag.date = lst.EXDate.ToString("u");
            ViewBag.EIType = lst.EXType;
            ViewBag.ProjectNo = lst.ProjectID;
            ViewBag.EXWarehouse = lst.EXWarehouse;
            ViewBag.IMWarehouse = lst.IMWarehouse;
            ViewBag.Operator = lst.Operator;
            ViewBag.OperateTime = lst.OperationTime.ToString("u");
            ViewBag.CheckState = lst.CheckStateDsp;
            ViewBag.Remark = obj.remark;
            ViewBag.data = new EXMaterial().GetApplymentMatLst(id);
            ViewBag.CheckStateItems = new EXMaterial().CheckStateItems;
            ViewBag.OperationId = lst.OperationListID;
            return View("CheckEX");
        }

        public ActionResult SubmitCheckApply(string id,FormCollection requstParamsCollection)
        {
            string state = requstParamsCollection["CheckState"];
            string remark = requstParamsCollection["Remark"];
            if (new EXMaterial().SubmitCheck(id,state,remark))
            {
                X.Msg.Alert("页面消息","操作成功!", "history.go(-1);parent.App.win.close();").Show();
            }
            else
            {
                X.Msg.Alert("页面消息","操作失败！").Show();
            }
            return this.Direct();
        }
    }
}
