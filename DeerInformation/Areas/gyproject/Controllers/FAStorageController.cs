using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Extensions;
using System.Data;
using DeerInformation.Areas.gyproject.ShareMethod;

namespace DeerInformation.Areas.gyproject.Controllers
{
    [DirectController(AreaName = "gyproject")]
    public class FAStorageController : Controller
    {
        //
        // GET: /gyproject/FAStorage/
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();
        private string checkname = "固定资产入库";
        [DirectMethod]     
        public ActionResult FAStorageReload()//重新加载固定资产入库管理页面
        {
            var list = DB.V_GM_FAStorage.ToList();
            cw.Reload(list, "FAStorageStore");
            return this.Direct();
            //return RedirectToAction("Index"); 
        }
        [DirectMethod]
        [VisitAuthorize(Delete = true)]
        public ActionResult DeleteFAStorage(string id)//删除入库单操作
        {
            var record = DB.T_GM_StorageFixedAsset.Find(id);
            DB.T_GM_StorageFixedAsset.Remove(record);
            var list = DB.T_GM_DM.Where(w => w.NO == id).ToList();
            foreach (var item in list)
            {
                DB.T_GM_DM.Remove(item);
            }
            DB.SaveChanges();
            return this.Direct();
        }
        //[VisitAuthorize(Read=true,Create =true,Update=true,Delete=true)]
        /// <summary>
        /// 前两个是主要查询条件，串联查询（两个条件同时满足才行）。时间查询只有在前两个查询条件为空时才起作用。
        /// </summary>
        /// <param name="addno"></param>
        /// <param name="addfield"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
       [VisitAuthorize(Read = true)]
        public ActionResult FAStorageQuery(string addno, string addfield, string start, string end)//获取查询信息
        {
            int res = cw.QueryFirstTwo(addno, addfield);
            bool flag = cw.QueryTime(start, end);
            switch (res)
            {
                case 0:
                    var sel0 = DB.V_GM_FAStorage.Where(w => w.StorageNo == addno).Where(w => w.StorageType == addfield).ToList();
                    return this.Store(sel0);                 
                case 1:
                    var sel1 = DB.V_GM_FAStorage.Where(w => w.StorageNo == addno).ToList();
                    return this.Store(sel1);
                case 2:
                    var sel2 = DB.V_GM_FAStorage.Where(w => w.StorageType == addfield).ToList();
                    return this.Store(sel2);
                case 3:
                    if (flag)
                    {
                        DateTime dts = Convert.ToDateTime(start.Replace("\"", ""));
                        DateTime dte = Convert.ToDateTime(end.Replace("\"", ""));
                        var seldatediff = DB.V_GM_FAStorage.Where(w => w.No_Date <= dte).Where(w => w.No_Date >= dts).ToList();
                        return this.Store(seldatediff);
                    }
                    else
                    {
                        cw.QueryTime(start, end);
                        return this.Direct();
                    }
                default:
                    return this.Direct();
            }
            
            
        }
        public ActionResult Index()
        {
            return View(DB.V_GM_FAStorage.ToList());
        }

        #region 增加固定资产入库单
        [VisitAuthorize(Create = true)]
        public ActionResult FAStorageAddbtn()
        {
            Window win = new Window
            {
                ID = "window2",
                Title = "固定资产入库单",
                Height = 600,
                Width = 500,
                Modal = true,
                Constrain = true,
                Frame = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("FAStorageAddView", "FAStorage"),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.gyproject.FAStorageReload()",
                    }
                }
            };
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult FAStorageAddView()
        {
            ViewBag.audit = cw.SetCombox(checkname); ;
            ViewBag.cgm = cw.SetCombox_CGM(false);
            return View();
        }
        //[VisitAuthorize(Read = false, Delete = false, Create = true, Update = true)]
        public ActionResult FAStorageSubmit(string list, string record, string pri, string wid, T_GM_StorageFixedAsset rm)
        {
            List<string> a = cw.JsontoList(list);
            List<string> c = cw.JsontoList(record);
            List<string> b = cw.JsontoList(pri);
            var te = DB.T_GM_TempDetailMaterial.Where(w => w.Remark == rm.PFAGID);
            var de = DB.T_GM_PurchaseFixedAsset.Find(rm.PFAGID);

            T_CH_Operation_list auditprocess = new T_CH_Operation_list();
            var cf = DB.V_CH_Checkfuncflow.Where(w => w.CheckfuncName == checkname).Where(w => w.ID == rm.AuditProcess).ToList().FirstOrDefault();

            int i = 0;
            decimal t, f;
            if (a.Count > 0 && a[0] != "")
            {
                rm.GID = Guid.NewGuid().ToString();
                rm.AuditProcess = cf.Name;
                rm.WarehouseID = wid;
                rm.Operator = user.EmployeeId;
                rm.OperateTime = DateTime.Now;
                rm.No_Date = DateTime.Now;
                DB.T_GM_StorageFixedAsset.Add(rm);

                auditprocess.ID = rm.GID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
                auditprocess.Url = Url.Action("Variable", "Share", new { id = rm.GID });//"/gyproject/Share/Variable?id=" + rm.StorageNo + "&title=固定资产入库审核";
                auditprocess.Creator = user.EmployeeId;
                DB.T_CH_Operation_list.Add(auditprocess);

                foreach (var item in a)
                {
                    if (c[i] != "" && b[i] != "" && c[i] != "null" && b[i] != "null")
                    {
                        T_GM_DM dm = new T_GM_DM();
                        decimal.TryParse(c[i], out t);
                        decimal.TryParse(b[i], out f);

                        var test = te.Where(m => m.MFlID == item).ToList().First();
                        test.Num -= t;
                        if (test.Num < 0)
                        {
                            //X.Msg.Alert("警告", "收货实际数量超过采购数量！！！").Show();
                            return this.Direct(false, "收货实际数量超过采购数量！！！");
                        }
                        DB.T_GM_TempDetailMaterial.Attach(test);
                        DB.Entry(test).State = EntityState.Modified;

                        dm.NO = rm.StorageNo;
                        dm.Type = "SHF";
                        dm.Num = t;
                        dm.Price = f;
                        dm.MFlID = a[i];
                        dm.Remark = rm.GID;
                        i++;
                        DB.T_GM_DM.Add(dm);
                    }
                    else
                    {
                        X.Msg.Alert("警告", "物料数量不能为空").Show();
                        return this.Direct();
                    }
                }
                DB.SaveChanges();

                bool fl = true;
                foreach (var item in DB.T_GM_TempDetailMaterial.Where(m => m.Remark == rm.PFAGID).ToList())
                {
                    if (item.Num != 0)
                    {
                        fl = false;
                        break;
                    }
                }
                if (fl)
                {
                    de.Status = "delivering";
                    DB.T_GM_PurchaseFixedAsset.Attach(de);
                    DB.Entry(de).State = EntityState.Modified;
                    DB.SaveChanges();
                    X.Msg.Alert("提示", "此采购单货物已完全送达，等待审核中", "parent.App.window2.close();").Show();
                    return this.Direct();
                }
            }
            else
            {
                X.Msg.Alert("警告", "物料数量不能为空").Show();
            }
            return this.Direct();
        }

        #endregion

        #region 修改入库单,提交操作
        public ActionResult ModifySubmit(string list, string record, string price, string gid)
        {
            List<string> a = cw.JsontoList(list);
            List<string> b = cw.JsontoList(price);
            List<string> c = cw.JsontoList(record);
            T_CH_Operation_list auditprocess = new T_CH_Operation_list();
            var am_old = DB.T_GM_StorageFixedAsset.Find(gid);
            var de = DB.T_GM_PurchaseFixedAsset.Find(am_old.PFAGID);
            var te = DB.T_GM_TempDetailMaterial.Where(m => m.Remark == am_old.PFAGID);
            var am = new T_GM_StorageFixedAsset();
            var cf = DB.V_CH_Checkfuncflow.Where(w => w.Name == am_old.AuditProcess).ToList().FirstOrDefault();
            if (a.Count > 0 && a[0] != "")
            {
                am_old.Status = "modified once";
                DB.T_GM_StorageFixedAsset.Attach(am_old);
                DB.Entry(am_old).State = EntityState.Modified;

                am.GID = Guid.NewGuid().ToString();
                am.StorageNo = am_old.StorageNo;
                am.WarehouseID = am_old.WarehouseID;
                am.StorageType = am_old.StorageType;
                am.No_Date = am_old.No_Date;
                am.OperateTime = DateTime.Now;
                am.Operator = am_old.Operator;
                am.PFAGID = am_old.PFAGID;
                am.AuditProcess = am_old.AuditProcess;
                am.Remark = am_old.Remark;
                DB.T_GM_StorageFixedAsset.Add(am);
                int i = 0;
                decimal t, f;

                auditprocess.ID = am.GID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
                auditprocess.Url = Url.Action("Variable", "Share", new { id = am.GID });
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
                        detail.NO = am.StorageNo;
                        detail.Type = "SHF";
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
                if (cw.check(am.PFAGID))
                {
                    de.Status = "delivering";
                    DB.T_GM_PurchaseFixedAsset.Attach(de);
                    DB.Entry(de).State = EntityState.Modified;
                    DB.SaveChanges();
                    X.Msg.Alert("提示", "此采购单货物已完全送达，等待审核中", "parent.App.window2.close();").Show();
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
