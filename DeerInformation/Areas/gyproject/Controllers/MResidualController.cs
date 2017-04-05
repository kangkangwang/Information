using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Data;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.gyproject.Controllers
{
    [DirectController(AreaName = "gyproject")]
    [AllowAnonymous]
    //have done!!!
   //There is no problem temporarily
    public class MResidualController : Controller
    {
        //
        // GET: /gyproject/MResidual/
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();
        private string checkname = "材料退回单";

        [DirectMethod]
        public ActionResult MResidualReload()//重新加载收货单管理页面
        {
            var list = DB.V_GM_MResidual.ToList();
            cw.Reload(list, "MResidualStore");
            return this.Direct();
        }

        [DirectMethod]
        [VisitAuthorize(Delete = true)]
        public ActionResult DeleteMResidual(string id)//删除收货单操作
        {
            var record = DB.T_GM_ResidualM.Find(id);
            DB.T_GM_ResidualM.Remove(record);
            var list = DB.T_GM_DM.Where(w => w.Remark == id).ToList();
            foreach (var item in list)
            {
                DB.T_GM_DM.Remove(item);
            }
            DB.SaveChanges();
            return this.Direct();
        }

        //[VisitAuthorize(Read=true,Create =true,Update=true,Delete=true)]
        public ActionResult MResidualQuery(string addno, string addfield, string start, string end)//获取查询信息
        {
            int res = cw.QueryFirstTwo(addno, addfield);
            bool flag = cw.QueryTime(start, end);
            switch (res)
            {
                case 0:
                    var sel0 = DB.V_GM_MResidual.Where(w => w.ResidualNo == addno).Where(w => w.ProjectNo == addfield).ToList();
                    return this.Store(sel0);
                case 1:
                    var sel1 = DB.V_GM_MResidual.Where(w => w.ResidualNo == addno).ToList();
                    return this.Store(sel1);
                case 2:
                    var sel2 = DB.V_GM_MResidual.Where(w => w.ProjectNo == addfield).ToList();
                    return this.Store(sel2);
                case 3:
                    if (flag)
                    {
                        DateTime dts = Convert.ToDateTime(start.Replace("\"", ""));
                        DateTime dte = Convert.ToDateTime(end.Replace("\"", ""));
                        var seldatediff = DB.V_GM_MResidual.Where(w => w.BackDate <= dte).Where(w => w.BackDate >= dts).ToList();
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
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View(DB.V_GM_MResidual.ToList());
        }

        #region 增加剩余物料退回单
        [VisitAuthorize(Create = true)]
        public ActionResult MResidualAddbtn()
        {
            WinModule win = new WinModule();
            win.ID = "window2";
            win.Title = "余料退回单";
            win.Loader.Url = Url.Action("MResidualAddView", "MResidual");
            win.Listeners.Close.Handler = "App.direct.gyproject.MResidualReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult MResidualAddView()
        {
            ViewBag.spid = cw.SetCombox_SupplierNo();
            ViewBag.prono = cw.SetCombox_PNo();
            ViewBag.audit = cw.SetCombox(checkname);
            return View();
        }

        //[VisitAuthorize(Read = false, Delete = false, Create = true, Update = true)]
        public ActionResult MResidualSubmit(string list, string record, string pri, T_GM_ResidualM rm)
        {
            try
            {
                List<string> a = cw.JsontoList(list);
                List<string> c = cw.JsontoList(record);
                List<string> b = cw.JsontoList(pri);
                T_CH_Operation_list auditprocess = new T_CH_Operation_list();
                var cf = DB.V_CH_Checkfuncflow.Where(w => w.CheckfuncName == checkname).Where(w => w.ID == rm.AuditProcess).ToList().FirstOrDefault();
                int i = 0;
                decimal t, f;
                if (a.Count > 0 && a[0] != "" && cf != null)
                {
                    rm.GID = Guid.NewGuid().ToString();
                    rm.AuditProcess = cf.Name;
                    rm.Operator = user.EmployeeId;
                    rm.OperateTime = DateTime.Now;
                    rm.BackDate = DateTime.Now;
                    DB.T_GM_ResidualM.Add(rm);

                    auditprocess.ID = rm.GID;
                    auditprocess.Check_funcID = cf.CheckfuncID;
                    auditprocess.Check_flowID = cf.ID;
                    auditprocess.CreateTime = DateTime.Now;
                    auditprocess.State = 1;
                    auditprocess.Url = Url.Action("MResidualAudit", "Share", new { id = rm.GID});
                    auditprocess.Creator = user.EmployeeId;
                    DB.T_CH_Operation_list.Add(auditprocess);

                    foreach (var item in a)
                    {
                        if (c[i] != "" && b[i] != "" && c[i] != "null" && b[i] != "null")
                        {
                            T_GM_DM dm = new T_GM_DM();
                            decimal.TryParse(c[i], out t);
                            decimal.TryParse(b[i], out f);
                            dm.NO = rm.ResidualNo;
                            dm.Type = "THM";
                            dm.Num = t;
                            dm.Price = f;
                            dm.MFlID = a[i];
                            dm.Remark = rm.GID;
                            i++;
                            DB.T_GM_DM.Add(dm);
                        }
                        else
                        {
                            X.Msg.Alert("警告", "数量或价格不能为空").Show();
                            return this.Direct();
                        }
                    }
                    DB.SaveChanges();
                    return this.Direct();
                }
                else
                {
                    X.Msg.Alert("提示", "你尚未选择任何材料").Show();
                    return this.Direct();
                }
            }
            catch (Exception e)
            {

                return this.Direct(false, e.Message);
            }
            
        }

        #endregion

        #region 修改物料退回单,提交操作
        public ActionResult ModifySubmit(string list, string record, string price, string gid)
        {
            //X.GetCmp<ComboBox>("");
            List<string> a = cw.JsontoList(list);
            List<string> b = cw.JsontoList(price);
            List<string> c = cw.JsontoList(record);
            T_CH_Operation_list auditprocess = new T_CH_Operation_list();
            var am_old = DB.T_GM_ResidualM.Find(gid);
            var am = new T_GM_ResidualM();
            var cf = DB.V_CH_Checkfuncflow.Where(w => w.Name == am_old.AuditProcess).ToList().FirstOrDefault();
            if (a.Count > 0 && a[0] != "")
            {
                am_old.Remark = "modified once";
                DB.T_GM_ResidualM.Attach(am_old);
                DB.Entry(am_old).State = EntityState.Modified;

                am.GID = Guid.NewGuid().ToString();
                am.SupplierID = am_old.SupplierID;
                am.ResidualType = am_old.ResidualType;
                am.ResidualNo = am_old.ResidualNo;
                am.OperateTime = DateTime.Now;
                am.Operator = am_old.Operator;
                am.BackDate = DateTime.Now;
                am.ProjectNo = am_old.ProjectNo;
                am.AuditProcess = am_old.AuditProcess;
                DB.T_GM_ResidualM.Add(am);
                int i = 0;
                decimal t, f;

                auditprocess.ID = am.GID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
                auditprocess.Url = Url.Action("MResidualAudit", "Share", new { id = am.GID });
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
                        detail.Num = t;
                        detail.Price = f;
                        detail.NO = am.ResidualNo;
                        detail.Type = "THM";
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
            }
            else
            {
                X.Msg.Alert("提示", "您尚未添加任何物料！！！").Show();
            }
            return this.Direct();
        }

        #endregion

        

    }
}
