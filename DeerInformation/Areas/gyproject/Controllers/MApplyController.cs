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
    [DirectController(AreaName="gyproject")]
    public class MApplyController : Controller
    {
        // GET: /gyproject/MApply/
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();      
        private string checkname = "材料申请";

        [DirectMethod]
        public ActionResult MApplyReload()//重新加载申请单管理页面
        {
            var list = DB.V_GM_MApply.ToList();
            cw.Reload(list, "MApplyStore");
            return this.Direct();
        }
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View(DB.V_GM_MApply.ToList());
        }
        public ActionResult MApplyInquire(string addno, string addfield)//精确查询
        {
            var selres1 = DB.V_GM_MApply.Where(w => w.ProjectNo.Contains(addno)).Where(w => w.ApplyMaterialNo.Contains(addfield)).ToList();
            var selres2 = DB.V_GM_MApply.Where(w => w.ProjectNo.Contains(addno)).ToList();
            if (addno.Length > 0 && addno != "")
            {
                if (addfield != "" && addfield.Length > 0)
                {
                    return this.Store(selres1);
                }
                else
                {
                    return this.Store(selres2);
                }
            }
            else
            {
                if (addfield != "" && addfield.Length > 0)
                {
                    return this.Store(DB.V_GM_MApply.Where(w => w.ApplyMaterialNo.Contains(addfield)).ToList());
                }
                else
                {
                    return this.Store(DB.V_GM_MApply.ToList());
                }
            }
        }
        [DirectMethod]
        [VisitAuthorize(Delete = true)]
        public ActionResult DeleteApplyMaterial(string id)//删除申请单操作
        {
            var record = DB.T_GM_ApplyMaterial.Find(id);
            DB.T_GM_ApplyMaterial.Remove(record);
            var list = DB.T_GM_DM.Where(w => w.Remark == id).ToList();
            foreach (var item in list)
            {
                DB.T_GM_DM.Remove(item);
            }
            DB.SaveChanges();
            return this.Direct();
        }
        
        #region 添加特殊申请单
        [VisitAuthorize(Create = true)]
        public ActionResult MApplyAddButton()
        {
            WinModule win = new WinModule();
            win.Title="添加申请单";
            win.Loader.Url=Url.Action("MApplyAddView", "MApply");
            win.Listeners.Close.Handler="App.direct.gyproject.MApplyReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult MApplyAddView()
        {
            GridModule gs = new GridModule();
            GridPanel gm = gs.gf;
            gm.AddTo(this.GetCmp<FormPanel>("storeform"));
            this.GetCmp<FormPanel>("storeform").SetActive(true);
            ViewBag.audit = cw.SetCombox(checkname);
            ViewBag.prono = cw.SetCombox_PNo();
            return View();
        }
        public ActionResult MApplyAdd(string list, string record, string pri, T_GM_ApplyMaterial am)
        {
            List<string> a = cw.JsontoList(list);
            List<string> b = cw.JsontoList(pri);
            List<string> c = cw.JsontoList(record);
            List<decimal> num = cw.StrtoDec(c);
            List<decimal> price = cw.StrtoDec(b);
            if (!cw.IsPositiveNumArr(num))
            {
                return this.Direct(false, "数量不能为负数");
            }
            if (a.Count > 0 && a[0] != "" && a[0] != "null" && a.Count == num.Count && a.Count == price.Count)
            {
                T_CH_Operation_list auditprocess = new T_CH_Operation_list();
                var cf = DB.V_CH_Checkfuncflow.Where(w => w.ID == am.CheckProcess).ToList().FirstOrDefault();
                am.GID = Guid.NewGuid().ToString();
                am.CheckProcess = cf.Name;
                am.ApplyMan = user.EmployeeId;
                //am.ApplyMState = "deploying";
                am.ApplyTime = DateTime.Now;
                DB.T_GM_ApplyMaterial.Add(am);

                auditprocess.ID = am.GID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
                auditprocess.Url = Url.Action("MApplyAudit", "Share", new { gid = am.GID });
                auditprocess.Creator = user.EmployeeId;
                DB.T_CH_Operation_list.Add(auditprocess);

                int i = 0;
                foreach (var item in a)
                {
                    var detail = new T_GM_DM();
                    detail.Remark = am.GID;
                    detail.MFlID = item;
                    
                    detail.Num = num[i];
                    detail.Price = price[i];
                    detail.NO = am.ApplyMaterialNo;
                    detail.Type = "SQM";
                    i++;
                    DB.T_GM_DM.Add(detail);
                }
            }
            else
            {
                return this.Direct();
            }
            DB.SaveChanges();
            return this.Direct();
        }

        #endregion

        #region 修改申请单,提交操作
        public ActionResult ModifySubmit(string list, string record, string price, string gid)
        {
            List<string> a = cw.JsontoList(list);
            List<string> b = cw.JsontoList(price);
            List<string> c = cw.JsontoList(record);
            T_CH_Operation_list auditprocess = new T_CH_Operation_list();
            var am_old = DB.T_GM_ApplyMaterial.Find(gid);
            var am = new T_GM_ApplyMaterial();
            var cf = DB.V_CH_Checkfuncflow.Where(w => w.Name == am_old.CheckProcess).ToList().FirstOrDefault();
            if (a.Count > 0 && a[0] != "")
            {
                am_old.ApplyMState = "modified once";
                DB.T_GM_ApplyMaterial.Attach(am_old);
                DB.Entry(am_old).State = EntityState.Modified;

                am.GID = Guid.NewGuid().ToString();
                am.PrepaidDay = am_old.PrepaidDay;
                am.Tel = am_old.Tel;

                am.ApplyTime = DateTime.Now;
                am.ApplyMan = am_old.ApplyMan;
                am.FPMGID = am_old.FPMGID;
                am.ApplyMaterialNo = am_old.ApplyMaterialNo;
                am.CheckProcess = am_old.CheckProcess;
                am.ProjectNo = am_old.ProjectNo;
                am.Remark = am_old.Remark;
                DB.T_GM_ApplyMaterial.Add(am);
                int i = 0;
                decimal t, f;

                auditprocess.ID = am.GID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
                auditprocess.Url = Url.Action("MApplyAudit", "Share", new { gid = am.GID });
                auditprocess.Creator = user.EmployeeId;
                DB.T_CH_Operation_list.Add(auditprocess);

                foreach (var item in a)
                {
                    if (c[i] != "" && b[i] != "" && c[i] != "null" && b[i] != "null" )
                    {
                        var detail = new T_GM_DM();
                        detail.Remark = am.GID;
                        detail.MFlID = item;
                        decimal.TryParse(c[i], out t);
                        decimal.TryParse(b[i], out f);
                        detail.Num = t;
                        detail.Price = f;
                        detail.NO = am.ApplyMaterialNo;
                        detail.Type = "SQM";
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
                //X.MessageBox.Alert("消息", "操作成功！", "history.go(-1);parent.location.reload();").Show();
                return this.Direct();
            }
            else
            {
                X.Msg.Alert("警告", "您尚未添加任何物料！！！").Show();
                return this.Direct();
            }
        }

        #endregion

        #region 添加一般申请单
        [VisitAuthorize(Update = true)]
        public ActionResult OrdinaryMApplyAddButton()
        {
            WinModule win = new WinModule();
            win.Title = "普通申请单";
            win.Loader.Url = Url.Action("OrdinaryMApplyAddView", "MApply");
            win.Listeners.Close.Handler = "App.direct.gyproject.MApplyReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult OrdinaryMApplyAddView()
        {
            ViewBag.prono = cw.SetCombox_PNo();
            ViewBag.audit = cw.SetCombox(checkname);
            return View();
        }
        public ActionResult OrdinaryMApplyAdd(string list, string record, string pri, T_GM_ApplyMaterial am)
        {
            var re = DB.V_GM_DetailProject.Where(w => w.ProjectNo == am.ProjectNo).ToList().FirstOrDefault();
            am.FPMGID = re.UID;
            string wareid = re.WarehouseID;
            #region 添加物料
            List<string> a = cw.JsontoList(list);//编号
            List<string> b = cw.JsontoList(pri);//价格
            List<string> c = cw.JsontoList(record);//数量
            List<decimal> num = cw.StrtoDec(c);
            List<decimal> price = cw.StrtoDec(b);
            var te = DB.T_GM_TempDetailMaterial.Where(m => m.Remark == am.FPMGID);
            if (!cw.IsPositiveNumArr(num))
            {
                return this.Direct(false, "数量不能为负数");
            }
            if (a.Count > 0 && a[0] != "" && a[0] != "null" && a.Count == num.Count && a.Count == price.Count)
            {
                T_CH_Operation_list auditprocess = new T_CH_Operation_list();
                var cf = DB.V_CH_Checkfuncflow.Where(w => w.ID == am.CheckProcess).ToList().FirstOrDefault();
                am.GID = Guid.NewGuid().ToString();
                am.CheckProcess = cf.Name;
                am.ApplyMan = user.EmployeeId;
                //am.ApplyMState = "deploying";
                am.ApplyTime = DateTime.Now;
                DB.T_GM_ApplyMaterial.Add(am);

                auditprocess.ID = am.GID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
                auditprocess.Url = Url.Action("MApplyAudit", "Share", new { gid = am.GID });
                auditprocess.Creator = user.EmployeeId;
                DB.T_CH_Operation_list.Add(auditprocess);

                int i = 0;
                foreach (var item in a)
                {
                    var detail = new T_GM_DM();
                    detail.Remark = am.GID;
                    detail.MFlID = item;
                    var test = te.Where(m => m.MFlID == item).ToList().First();
                    test.Num -= num[i];
					//var storage = DB.T_GM_MaterialStock.Where(m => m.MaterialID == item).Where(m => m.WarehouseID == wareid).ToList().FirstOrDefault();
                    decimal flag = 1;//--修改 物料申请不需要考虑库存
					//if (storage!=null)
					//{
					//	flag = (test.Num ?? 0) - storage.CurAmount;
					//}
					//else
					//{
                        flag = test.Num ?? 0;
					//}
                    if (flag < 0)
                    {
                        return this.Direct(false, "申请的数量超过项目所需物资数量！！！");
                    }
                    DB.T_GM_TempDetailMaterial.Attach(test);
                    DB.Entry(test).State = EntityState.Modified;
                    detail.Num = num[i];
                    detail.Price = price[i];
                    detail.NO = am.ApplyMaterialNo;
                    detail.Type = "SQM";
                    i++;
                    DB.T_GM_DM.Add(detail);
                }
            }
            else
            {
                return this.Direct();
            }
            #endregion
            DB.SaveChanges();
            return this.Direct();
        }

        #endregion
    }
}
