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
    public class FAPurchaseController : Controller
    {
        //
        // GET: /gyproject/FAPurchase/
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();
        private string checkname2 = "固定资产采购";
        [DirectMethod]
        public ActionResult FAPurchaseReload()//重新加载采购单管理页面
        {
            var list = DB.V_GM_FAPurchase.ToList();
            cw.Reload(list, "PurchaseFixedAsset");
            return this.Direct();
        }
        [DirectMethod]
        [VisitAuthorize(Delete = true)]
        public ActionResult DeleteFAPurchase(string gid)//删除申请单操作
        {
            var record = DB.T_GM_PurchaseFixedAsset.Find(gid);
            DB.T_GM_PurchaseFixedAsset.Remove(record);
            var list = DB.T_GM_DM.Where(w => w.Remark == gid).ToList();
            foreach (var item in list)
            {
                DB.T_GM_DM.Remove(item);
            }
            DB.SaveChanges();
            return this.Direct();
        }
        public ActionResult Index()
        {
            return View(DB.V_GM_FAPurchase.ToList());
        }

        #region 查询
        [VisitAuthorize(Read = true)]
        public ActionResult Select(string start)
        {
            if (start.Length > 0)
            {
                return this.Store(DB.V_GM_FAPurchase.Where(w => w.PurchaseFNo.Contains(start)).ToList());
            }

            else
            {
                return this.Store(DB.V_GM_FAPurchase.ToList());
            }
        }

        #endregion

        #region 配置供应商

        public ActionResult FADeploy()
        {
            WinModule win = new WinModule();
            win.ID = "window3";
            win.Loader.Url = Url.Action("FADeployView");
            win.Listeners.Close.Handler = "App.direct.gyproject.FAPurchaseReload()";
            win.Render(RenderMode.Auto);       
            return this.Direct();
        }
        public ActionResult FADeployView()
        {
            ViewBag.SQM = cw.SetCombox_SQM(false);
            ViewBag.audit = cw.SetCombox(checkname2);
            return View();
        }
        public ActionResult FAPurchaseSave(string list, string record, string price, string spid, T_GM_PurchaseFixedAsset am)
        {
            List<string> a = cw.JsontoList(list);
            List<string> c = cw.JsontoList(record);
            List<string> b = cw.JsontoList(price);
            T_CH_Operation_list auditprocess = new T_CH_Operation_list();
            var cf = DB.V_CH_Checkfuncflow.Where(w => w.ID == am.AuditProcess).ToList().FirstOrDefault();
            var de = DB.T_GM_ApplyFixedAsset.Find(am.ApplyNo);
            var te = DB.T_GM_TempDetailMaterial.Where(m => m.Remark == am.ApplyNo);
            if (a.Count > 0 && cf != null)
            {
                am.GID = Guid.NewGuid().ToString();
                am.AuditProcess = cf.Name;
                am.Operator = user.EmployeeId;
                am.OperateTime = DateTime.Now;
	            am.SupplierID = spid;
                DB.T_GM_PurchaseFixedAsset.Add(am);

                auditprocess.ID = am.GID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
                auditprocess.Url = Url.Action("FAPurchaseAudit", "Share", new { gid = am.GID });
                auditprocess.Creator = user.EmployeeId;
                DB.T_CH_Operation_list.Add(auditprocess);
            }
            else
            {
                X.Msg.Alert("警告", "您尚未添加任何物料").Show();
                return this.Direct();
            }

            int i = 0;
            decimal t, f;
            foreach (var item in a)
            {
                if (c[i] != "" && b[i] != "" && c[i] != "null" && b[i] != "null")
                {
                    var detail = new T_GM_DM();
                    detail.Remark = am.GID;
                    detail.MFlID = item;
                    decimal.TryParse(c[i], out t);
                    decimal.TryParse(b[i], out f);
                    var test = te.Where(m => m.MFlID == item).ToList().First();
                    test.Num -= t;
                    if (test.Num < 0)
                    {
                        //X.Msg.Alert("警告", "采购的数量超过申请单里剩余物料的数量！！！").Show();
                        return this.Direct(false, "采购的数量超过申请单里剩余物料的数量！！！");
                    }
                    DB.T_GM_TempDetailMaterial.Attach(test);
                    DB.Entry(test).State = EntityState.Modified;
                    detail.Type = "CGF";
                    detail.Num = t;
                    detail.Price = f;
                    detail.NO = am.PurchaseFNo;
                    DB.T_GM_DM.Add(detail);
                    i++;
                }
                else
                {
                    X.Msg.Alert("警告", "物料数量不能为空").Show();
                    return this.Direct();
                }
            }
            DB.SaveChanges();

            if (cw.check(am.ApplyNo))
            {
                de.Status = "deploying";
                DB.T_GM_ApplyFixedAsset.Attach(de);
                DB.Entry(de).State = EntityState.Modified;
                DB.SaveChanges();
                X.Msg.Alert("提示", "订单已配置完成,请耐心等待审核", "parent.App.window3.close();").Show();
                return this.Direct();
            }
            return this.Direct();
        }

        #endregion

        #region 修改采购单,提交操作
        public ActionResult ModifySubmit(string list, string record, string price, string gid)
        {
            List<string> a = cw.JsontoList(list);
            List<string> b = cw.JsontoList(price);
            List<string> c = cw.JsontoList(record);
            T_CH_Operation_list auditprocess = new T_CH_Operation_list();
            var am_old = DB.T_GM_PurchaseFixedAsset.Find(gid);
            var de = DB.T_GM_ApplyFixedAsset.Find(am_old.ApplyNo);
            var te = DB.T_GM_TempDetailMaterial.Where(m => m.Remark == am_old.ApplyNo);
            var am = new T_GM_PurchaseFixedAsset();
            var cf = DB.V_CH_Checkfuncflow.Where(w => w.Name == am_old.AuditProcess).ToList().FirstOrDefault();
            if (a.Count > 0 && a[0] != "")
            {
                am_old.Status = "modified once";
                DB.T_GM_PurchaseFixedAsset.Attach(am_old);
                DB.Entry(am_old).State = EntityState.Modified;

                am.GID = Guid.NewGuid().ToString();
                am.PurchaseFNo = am_old.PurchaseFNo;
                am.ApplyNo = am_old.ApplyNo;
                am.SupplierID = am_old.SupplierID;
                am.OrderType = am_old.OrderType;
                am.No_Date = am_old.No_Date;
                am.OperateTime = DateTime.Now;
                am.Operator = am_old.Operator;
                am.AuditProcess = am_old.AuditProcess;
                am.Remark = am_old.Remark;
                DB.T_GM_PurchaseFixedAsset.Add(am);
                int i = 0;
                decimal t, f;

                auditprocess.ID = am.GID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
                auditprocess.Url = Url.Action("FAPurchaseAudit", "Share", new { gid = am.GID });
                auditprocess.Creator = user.EmployeeId;
                DB.T_CH_Operation_list.Add(auditprocess);

                foreach (var item in a)
                {
                    if (c[i] != "" && b[i] != "" && c[i] != "null" && b[i] != "null")
                    {
                        var detail = new T_GM_DM();
                        detail.Remark = am.GID;
                        detail.MFlID = item;
                        decimal.TryParse(c[i], out t);
                        decimal.TryParse(b[i], out f);
                        var test = te.Where(m => m.MFlID == item).ToList().First();
                        test.Num -= t;
                        if (test.Num < 0)
                        {
                            //X.Msg.Alert("警告", "采购的数量超过申请单里剩余物料的数量！！！").Show();
                            return this.Direct(false, "采购的数量超过申请单里剩余物料的数量！！！");
                        }
                        DB.T_GM_TempDetailMaterial.Attach(test);
                        DB.Entry(test).State = EntityState.Modified;
                        detail.Num = t;
                        detail.Price = f;
                        detail.NO = am.PurchaseFNo;
                        detail.Type = "CGF";
                        i++;
                        DB.T_GM_DM.Add(detail);
                    }
                    else
                    {
                        X.Msg.Alert("警告", "您输入数量或价格为空！！！").Show();
                        return this.Direct();
                    }

                }
                DB.SaveChanges();
                if (cw.check(am.ApplyNo))
                {
                    de.Status = "deploying";
                    DB.T_GM_ApplyFixedAsset.Attach(de);
                    DB.Entry(de).State = EntityState.Modified;
                    DB.SaveChanges();
                    X.Msg.Alert("提示", "订单已配置完成，请等待审核", "parent.App.window3.close();").Show();
                    return this.Direct();
                }
                return this.Direct();
            }
            else
            {
                X.Msg.Alert("警告", "您尚未添加任何物料！！！").Show();
                return this.Direct();
            }
        }

        #endregion

    }
}
