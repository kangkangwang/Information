using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Data.Objects.SqlClient;
using System;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.BaseType;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.gyproject.Controllers
{
    [DirectController(AreaName = "gyproject")]
    public class ShareController : Controller
    {
        //
        // GET: /gyproject/Share/
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();
        private string MasterWarehouseID = "JabilWX001";
        private List<T_GM_InfoMaterial> SetVirtualPath(List<T_GM_InfoMaterial> list)
        {
            foreach (var item in list)
            {
                if (item.PicturePath != null)
                {
                    item.PicturePath = Url.Content(item.PicturePath);
                }
            }
            return list;
        }

        #region old and abandon way
        public ActionResult GetMID(string query)
        {
            var data = DB.T_GM_InfoMaterial;
            string fitformat = string.Format("%{0}%", query);
            var result = data.Where(se => SqlFunctions.PatIndex(fitformat, se.MaterialName) > 0)
                .Select(m => new { Name = m.MaterialName, ID = m.MaterialID })
                .Take(5);
            return this.Store(result.ToList());
        }
        public ActionResult GetWarehouseID(string query)
        {
            var data = DB.T_GM_Warehouse;
            string fitformat = string.Format("%{0}%", query);
            var result = data.Where(se => SqlFunctions.PatIndex(fitformat, se.WarehouseName) > 0)
                .Select(m => new { Name = m.WarehouseName, ID = m.WarehouseID })
                .Take(5);
            return this.Store(result.ToList());
        }
        public ActionResult GetSupplierID(string query)
        {
            var data = DB.T_GM_SupplierInfo;
            string fitformat = string.Format("%{0}%", query);
            var result = data.Where(se => SqlFunctions.PatIndex(fitformat, se.SupplierName) > 0)
                .Select(m => new { Name = m.SupplierName, ID = m.SupplierID })
                .Take(5);
            return this.Store(result.ToList());
        }
        #endregion

        #region See the detail material of last layer
        /// <summary>
        /// 查看要交货的采购单
        /// 可根据gid和no查询
        /// </summary>
        /// <param name="purchasegid"></param>
        /// <returns></returns>
        public ActionResult TempPurchase(string purchasegid, string MorFA = "M")
        {
            if (MorFA == "M")
            {
                if (purchasegid.Length < 32)
                {
                    var record = DB.V_GM_MPurchase.Where(m => m.PurchaseMNo == purchasegid).Where(m => m.IsEnableNo == "审核通过").ToList().FirstOrDefault();
                    if (record == null)
                    {
                        X.Msg.Alert("提示", "您输入的采购单号有误", "parent.App.win.close();").Show();
                        return this.Direct();
                    }
                    else
                    {
                        purchasegid = record.GID;
                    }
                }                
                var project = DB.V_GM_MPurchase.Where(w => w.GID == purchasegid).ToList().FirstOrDefault();
                string place = DB.T_GW_FieldInfoManagement.Where(w => w.ProjectNo == project.ProjectNo).ToList().FirstOrDefault().FieldName;
                X.GetCmp<TextField>("tel").SetValue(project.Tel);
                X.GetCmp<TextField>("prono").SetValue(project.ProjectNo);
                X.GetCmp<TextField>("place").SetValue(place);
                X.GetCmp<ComboBox>("purchasegid").ReadOnly = true;
            }
            else
            {
                purchasegid = DB.V_GM_FAPurchase.Where(m => m.PurchaseFNo == purchasegid).Where(m => m.Description == "审核通过").ToList().FirstOrDefault().GID;
            }
            SharePanel(purchasegid, "采购单剩余物料", "receiptm");
            return this.Direct();
        }
        public ActionResult TempApply(string smgid, bool flag = true)
        {
            if (flag)
            {
                var applym = DB.T_GM_ApplyMaterial.Find(smgid);
                X.GetCmp<DateField>("prepaidday").SetValue(applym.PrepaidDay);
                X.GetCmp<TextField>("prono").SetValue(applym.ProjectNo);
                X.GetCmp<TextField>("tel").SetValue(applym.Tel);
                X.GetCmp<ComboBox>("smgid").ReadOnly = true;
                SharePanel(smgid, "申请单剩余物料", "purchasem");
            }
            return this.Direct();
        }

        public ActionResult TempApply2(string smgid, string warehouseid)
        {
            var applym = DB.T_GM_ApplyMaterial.Find(smgid);
            X.GetCmp<DateField>("prepaidday").SetValue(applym.PrepaidDay);
            X.GetCmp<TextField>("prono").SetValue(applym.ProjectNo);
            X.GetCmp<TextField>("tel").SetValue(applym.Tel);
            X.GetCmp<ComboBox>("smgid").ReadOnly = true;
            SharePanel(smgid + "," + warehouseid, "申请单剩余物料", "purchasem");
            return this.Direct();
        }
        public ActionResult SeeMaterial(string prono)//1、普通请购单查看报价单详细物料
        {
	        var list = DB.V_GM_DetailProject.FirstOrDefault(w => w.ProjectNo == prono);
            if (list == null)
            {
                X.Msg.Alert("提示", "您输入的项目编号有误", "parent.App.win.close();").Show();
                return this.Direct();
            }
            SharePanel(list.UID, "项目可申请物料");
            X.GetCmp<ComboBox>("prono").ReadOnly = true;
            return this.Direct();
        }
        #endregion

        #region Choose Material in the Doom
        private void SharePanel(string gid,string title, string condition = "projectm", string containername = "projectm")
        {
            FormPanel p = new FormPanel()
            {
                MinHeight = 10,
                Loader = new ComponentLoader()
                {
                    Url = Url.Action("ChooseM", "Share", new { con = condition, id = gid, tit=title }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                }
            };
            p.AddTo(this.GetCmp<Container>(containername));
            this.GetCmp<Container>(containername).SetActive(true);
        }
        public void DifferentLayer(object list, bool num = false ,bool search = true,bool confirm = true)
        {
            cw.Reload(list, "AppliedMaterial");      
            X.GetCmp<Column>("NumChange").Hidden = num;
            X.GetCmp<Container>("Ser_contaniner").Hidden = search;
            X.GetCmp<CommandColumn>("Confirm").Hidden=confirm;           
        }
        public ActionResult ChooseM(string con,string id,string tit)
        {
            switch (con)
            {
                case "projectbudget":
                    List<T_GM_InfoMaterial> list = DB.T_GM_InfoMaterial.Where(w => w.IsEnable == true).Where(w => w.Type == "M").ToList();
                    DifferentLayer(SetVirtualPath(list), true, false, false);
                    break;
                case "projectbudgetsee":
                    var list_see = DB.V_GM_DM.Where(w => w.Remark == id).ToList();
                    //X.GetCmp<HyperlinkColumn>("path").Hidden = true;
                    DifferentLayer(list_see);
                    break;
                case "projectm":
                    var warehouseid = DB.V_GM_DetailProject.Where(w => w.UID == id).ToList().FirstOrDefault().WarehouseID;
					List<object> store_budget = cw.ViewStorageAndBudgt(id, warehouseid);
					X.GetCmp<Column>("NumStock").SetHidden(false);
                    DifferentLayer(store_budget, false, true, false);
                    break;
                case "purchasem":
                    List<object> mix = cw.ViewStorageAndNo(id, MasterWarehouseID);
                    X.GetCmp<Column>("NumStock").SetHidden(false);
                    DifferentLayer(mix, false, true, false);
                    break;  
                case "receiptm":
                    var list_receiptm = DB.V_GM_TempDM.Where(w => w.Remark == id).ToList<dynamic>();
                    var BalanceAcountLst = DB.T_GM_InfoMaterial.Where(l => l.Type == "BA").Select(l=>new {
                        Price = l.Price,MaterialName = l.MaterialName,Size = l.Size,Unit = l.Unit,MaterialID = l.MaterialID,UID = l.UID,Brand = l.Brand,PicturePath = l.PicturePath,IsEnable = l.IsEnable,PurchaseCycle = l.PurchaseCycle,MinPurchase = l.MinPurchase,MFlID = l.MaterialID
                    }).ToList();
                    list_receiptm = list_receiptm.Union(BalanceAcountLst).ToList();
                    DifferentLayer(list_receiptm, false, true, false);
                    break;
                default:
                    break;
            }
            ViewBag.tit = tit;
            return View();
        }

        #endregion
        
        #region 物资查询
        public ActionResult SearchMaterial(string size)
        {
            if (size != "")
            {
                return this.Store(DB.T_GM_InfoMaterial.Where(w => w.IsEnable == true).Where(w => w.MaterialName.Contains(size)).ToList());
            }
            else
            {
                return this.Store(DB.T_GM_InfoMaterial.Where(w => w.IsEnable == true).ToList());
            }
        }

        #endregion

        #region 选择材料
         public ActionResult MS(string list)
        {
            TempData["ID"] = cw.JsontoList(list);
            return this.Direct();
        }
        public ActionResult SM(string type="M")
        {
            WinModule win = new WinModule();
            win.Constrain = false;
            win.ID="window12";
            win.Title="选择材料";
            win.Height = 550;
            win.Width = 550;
            win.Loader.Url = Url.Action("SelMInfo", "Share", new { type = type });
            win.Listeners.Close.Handler = "App.direct.gyproject.GetM('MaterialSelected')";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult SelMInfo(string type)
        {
            var list = DB.T_GM_InfoMaterial.Where(w => w.IsEnable == true).Where(w => w.Type == type).ToList();
            return View(SetVirtualPath(list));
        }
        /// <summary>
        /// 其实这里用T_GM_InfoMaterial中的UID更好
        /// </summary>
        /// <param name="storename"></param>
        /// <returns></returns>
        [DirectMethod]
        public ActionResult GetM(string storename)
        {
            if (TempData["ID"] == null)
            {
                return this.Direct();
            }
            else
            {
                List<string> b = new List<string>();
                b = TempData["ID"] as List<string>;

                List<T_GM_InfoMaterial> data = new List<T_GM_InfoMaterial>();
                foreach (var pp in b)
                {
                    var ddd = DB.T_GM_InfoMaterial.Where(w => w.MaterialID == pp).ToList().FirstOrDefault();
                    if (ddd != null)
                        data.Add(ddd);
                    else
                    {
                        X.Msg.Alert("警告", "您尚未选择任何物料！！！").Show();
                        return this.Direct();
                    }
                }
                cw.Reload(data, storename);
                return this.Direct();
            }
        }

        #endregion

        #region 查看物料明细
        public ActionResult MDetail(string gid, string title)
        {
            Window win = new Window
            {
                ID = "window3",
                Title = "明细",
                Height = 450,
                Width = 500,
                Modal = true,
                Constrain = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("MDetailSeeView", "Share", new { ID = gid, tit = title }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                }
            };
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult MDetailSeeView(string id, string tit)
        {
            ViewBag.tit = tit;
            return View(DB.V_GM_DM.Where(w => w.Remark == id).ToList());
        }

        #endregion

        #region the base interface of audit 审核基础界面
       
        /// <summary>
        /// panel shared by all audit interface
        /// 所有审核界面共享的panel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        public void ShareAuditPanel(string id, string title)
        {

            var p = new Panel()
            {
                Height=400,
                ID = "audit",
                Loader = new ComponentLoader()
                {
                    Url =  Url.Action("Audit", "Share", new { mainid = id, tit = title }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
            };
            p.AddTo(this.GetCmp<Panel>("panel"));
            this.GetCmp<Panel>("panel").SetActive(true);
        }
        public ActionResult Audit(string mainid, string tit)
        {
            ViewBag.opreationid = mainid;
            ViewBag.tit = tit;
			bool flag = DB.T_CH_Operation_list.First(l => l.ID == mainid).State != (int)CheckState.Checking;
            return flag ? View("Expire") :View(DB.V_GM_DM.Where(w => w.Remark == mainid).ToList());
        }
        public ActionResult AuditSubmit(string state, string des, string opreationlistid)
        {
            string id = user.EmployeeId;
            var result = DB.P_CH_ExecuteCheck(opreationlistid, id, Convert.ToInt32(state), des).ToList();
            if (result[0] == 1)
            {
                X.MessageBox.Alert("消息", "操作成功！", "history.go(-1);").Show();
            }
            else
            {
                X.Msg.Alert("警告", "审核失败", "history.go(-1);").Show();
            }
            return this.Direct();
        }

        //#region two way's of using particalview
        //public Ext.Net.MVC.PartialViewResult Variable(string containerId)
        //{
        //    var storage = DB.T_GM_StorageFixedAsset.Find("SHF20160928111304");
        //    return new Ext.Net.MVC.PartialViewResult
        //    {
        //        ViewName = "Variable",
        //        ContainerId = containerId,
        //        RenderMode = RenderMode.AddTo,
        //        Model = storage,
        //    };
        //}
        //public ActionResult addparticalview2(string containerId, string id)
        //{
        //    var storage = DB.T_GM_StorageFixedAsset.Find(id);
        //    var result = new Ext.Net.MVC.PartialViewResult
        //    {
        //        ViewName = "Variable",
        //        ContainerId = containerId,
        //        RenderMode = RenderMode.AddTo,
        //        Model = storage,
        //    };
        //    this.GetCmp<Panel>(containerId).SetActive(true);
        //    return result;
        //}

        //#endregion

        #endregion

        #region The detail of every Audit
        public ActionResult Variable(string id)
        {
            string title = "固定资产入库审核";
            ShareAuditPanel(id, title);
            return View(DB.V_GM_FAStorage.Where(w => w.GID == id).ToList().FirstOrDefault());
        }
        public ActionResult MResidualAudit(string id)
        {
            string tit = "材料退回单物料明细";
            ShareAuditPanel(id, tit);
            return View(DB.V_GM_MResidual.Where(w => w.GID == id).ToList().FirstOrDefault());
        }
        public ActionResult MApplyAudit(string gid)
        {
            string tit = "材料申请单物料明细";
            ShareAuditPanel(gid, tit);
            return View(DB.V_GM_MApply.Where(w => w.GID == gid).ToList().FirstOrDefault());
        }
        public ActionResult MPurchaseAudit(string gid)
        {
            string tit = "材料采购单物料明细";
            ShareAuditPanel(gid, tit);
            return View(DB.V_GM_MPurchase.Where(w => w.GID == gid).ToList().FirstOrDefault());
        }
        public ActionResult FAApplyAudit(string gid)
        {
            string tit = "固定资产申请单物料明细";
            ShareAuditPanel(gid, tit);
            return View(DB.V_GM_FAApply.Where(w => w.GID == gid).ToList().FirstOrDefault());
        }
        public ActionResult FAPurchaseAudit(string gid)
        {
            string tit = "固定资产采购单物料明细";
            ShareAuditPanel(gid, tit);
            return View(DB.V_GM_FAPurchase.Where(w => w.GID == gid).ToList().FirstOrDefault());
        }

        public ActionResult ProjectBudgetAudit(string gid)
        {
            string tit = "报价单物料明细";
            ShareAuditPanel(gid, tit);
            return View(DB.V_GM_DetailBudget.Where(w => w.GID == gid).ToList().FirstOrDefault());
        }

        #endregion

        #region modify

        public bool PassCondition(string gid, string controllername)
        {
            bool condition=false;
            switch (controllername)
            {
                case "MApply":
                    var pass_condition = DB.V_GM_MApply.Where(w => w.GID == gid).ToList().FirstOrDefault();
                    condition = pass_condition.IsEnableNo == "审核驳回" && pass_condition.ApplyMState != "modified once";
                    break;
                case "MPurchase":
                    var pass_condition2 = DB.V_GM_MPurchase.Where(w => w.GID == gid).ToList().FirstOrDefault();
                    condition = pass_condition2.IsEnableNo == "审核驳回" && pass_condition2.OrderStatu != "modified once";
                    break;
                case "MResidual":
                    var pass_condition3 = DB.V_GM_MResidual.Where(w => w.GID == gid).ToList().FirstOrDefault();
                    condition = pass_condition3.Description == "审核驳回" && pass_condition3.Remark != "modified once";
                    break;
                case "FAApply":
                    var pass_condition4 = DB.V_GM_FAApply.Where(w => w.GID == gid).ToList().FirstOrDefault();
                    condition = pass_condition4.Description == "审核驳回" && pass_condition4.Status != "modified once";
                    break;
                case "FAPurchase":
                    var pass_condition5 = DB.V_GM_FAPurchase.Where(w => w.GID == gid).ToList().FirstOrDefault();
                    condition = pass_condition5.Description == "审核驳回" && pass_condition5.Status != "modified once";
                    break;
                case "FAStorage":
                    var pass_condition6 = DB.V_GM_FAStorage.Where(w => w.GID == gid).ToList().FirstOrDefault();
                    condition = pass_condition6.Description == "审核驳回" && pass_condition6.Status != "modified once";
                    break;

                default: 
                    break;
            }
            return condition;
        }
        [VisitAuthorize(Update = true)]
        public ActionResult ShareModify(string gid, string controllername)
        {
            if (PassCondition(gid,controllername))
            {
                Window win = new Window
                {
                    ID = "winmodify",
                    Title = "修改",
                    Height = 400,
                    Width = 600,
                    Modal = true,
                    AutoScroll = true,
                    Constrain = true,
                    CloseAction = CloseAction.Destroy,
                    Items =
                    {
                        new Panel()
                        {
                            Height = 380,
                            ID="modify",
                             Loader=new ComponentLoader()
                            {
                                Url = Url.Action("Modify", "Share", new { gid = gid,controllername=controllername}),
                                DisableCaching = true,
                                Mode = LoadMode.Frame
                            }
                        },        
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.gyproject."+controllername+"Reload()",
                        }
                    }
                };
                win.Render(RenderMode.Auto);      
            }
            else
            {
                X.Msg.Alert("提示","只有审核驳回的订单或第一次修改才有权限执行该操作！！！").Show();//最近驳回的订单，可以在index里将旧的隐藏掉。
               
            }
            //return RedirectToAction("Modify", new { gid = gid });
            return this.Direct();
        }
        public ActionResult Modify(string gid, string controllername)
        {
            string amgid;
            ViewBag.name = controllername;
            ViewBag.gid = gid;
            switch (controllername)
            {
                case "MPurchase":
                    ViewBag.title = "可修改最大物料数量";
                    amgid = DB.T_GM_PurchaseMaterial.Find(gid).AMGID;
                    return View(DB.V_GM_TempMaterial.Where(m => m.Remark == amgid).ToList());
                case "Mapply":
                    var appno = DB.T_GM_ApplyMaterial.Find(gid);
                    string acname = appno.ApplyMaterialNo;
                    if (acname.Contains("SQMO"))
                    {
                        ViewBag.title = "可修改最大物料数量";
                        amgid = appno.FPMGID;
                        return View(DB.V_GM_TempMaterial.Where(m => m.Remark == amgid).ToList());
                    }
                    else
                    {
                        ViewBag.title = "原订单物料";
                        return View(DB.V_GM_DM.Where(w => w.Remark == gid).ToList()); 
                    }  
                case "FAPurchase":
                    ViewBag.title = "可修改最大物料数量";
                    amgid = DB.V_GM_FAApply.Where(m => m.ApplyNo == (DB.T_GM_PurchaseFixedAsset.Find(gid).ApplyNo)).Where(m => m.Status == "审核通过").ToList().FirstOrDefault().GID;
                    return View(DB.V_GM_TempMaterial.Where(m => m.Remark == amgid).ToList());
                case "FAStorage":
                    ViewBag.title = "可修改最大物料数量";
                    amgid = DB.T_GM_StorageFixedAsset.Find(gid).PFAGID;
                    return View(DB.V_GM_TempMaterial.Where(m => m.Remark == amgid).ToList());
                default:
                   ViewBag.title = "原订单物料";
                   return View(DB.V_GM_DM.Where(w => w.Remark == gid).ToList()); 
            }
            
        }
        
        #endregion

    }
}
