using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Data;
using DeerInformation.Areas.system.Models;
using DeerInformation.Extensions;
using DeerInformation.Areas.gyproject.ShareMethod;

namespace DeerInformation.Areas.gyproject.Controllers
{
    [DirectController(AreaName = "gyproject")]
    public class FAApplyController : Controller
    {
        //
        // GET: /gyproject/FAApply/
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();
        private string checkname = "固定资产申请";
        [DirectMethod]
        public ActionResult FAApplyReload()//重新加载申请单管理页面
        {
            var list = DB.V_GM_FAApply.ToList();
            cw.Reload(list, "ApplyFixedAsset");
            return this.Direct();
        }

        [DirectMethod]
        public ActionResult FADeployedReload(string id)//重新加载配置申请单页面
        {
            var list = DB.V_GM_TempMaterial.Where(m => m.Remark == id).ToList();
            cw.Reload(list, "AppliedMaterial");
            return this.Direct();
        }
        public ActionResult Index()
        {
            return View(DB.V_GM_FAApply.ToList());
        }

        [DirectMethod]
        [VisitAuthorize(Delete = true)]
        public ActionResult DeleteApplyFixedAsset(string id)//删除申请单操作
        {
            var record = DB.T_GM_ApplyFixedAsset.Find(id);
            DB.T_GM_ApplyFixedAsset.Remove(record);
            var list = DB.T_GM_DM.Where(w => w.Remark == id).ToList();
            foreach (var item in list)
            {
                DB.T_GM_DM.Remove(item);
            }
            DB.SaveChanges();
            return this.Direct();
        }

        #region 固定资产申请信息查询
        [VisitAuthorize(Read = true)]
        public ActionResult Select(string start)
        {
            if (start.Length > 0&& start!="")
            {
                return this.Store(DB.V_GM_FAApply.Where(w => w.ApplyNo.Contains(start)).ToList());
            }

            else
            {
                return this.Store(DB.V_GM_FAApply.ToList());
            }
        }

        #endregion

        #region 固定资产申请添加
        public ActionResult ApplyFixedAssetAddView()
        {
            GridModule gs = new GridModule();
            GridPanel gm = gs.gf;
            gm.AddTo(this.GetCmp<FormPanel>("storeform"));
            this.GetCmp<FormPanel>("storeform").SetActive(true);
            ViewBag.audit = cw.SetCombox(checkname);
            return View();
        }
        [VisitAuthorize(Create = true)]
        public ActionResult ApplyFixedAssetAddButton()
        {
            Window win = new Window
            {
                ID = "window2",
                Title = "项目物资申请添加",
                Height = 500,
                Width = 500,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("ApplyFixedAssetAddView"),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.gyproject.FAApplyReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult FAApplyAdd(string list, string record, string pri, T_GM_ApplyFixedAsset am)
        {
            List<string> a = cw.JsontoList(list);
            List<string> b = cw.JsontoList(pri);
            List<string> c = cw.JsontoList(record);
            T_CH_Operation_list auditprocess = new T_CH_Operation_list();
            var cf = DB.V_CH_Checkfuncflow.Where(w => w.ID == am.AuditProcess).ToList().FirstOrDefault();
            if (a.Count > 0 && a[0] != "")
            {
                am.GID = Guid.NewGuid().ToString();
                am.No_Date = DateTime.Now;
                am.OperateTime = DateTime.Now;
                am.Operator = user.EmployeeId;
                am.AuditProcess = cf.Name;
                DB.T_GM_ApplyFixedAsset.Add(am);

                int i = 0;
                decimal t, f;

                auditprocess.ID = am.GID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
                auditprocess.Url = Url.Action("FAApplyAudit", "Share", new { gid = am.GID });
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
                        detail.NO = am.ApplyNo;
                        detail.Type = "SQF";
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
                return this.Direct();
            }
            else
            {
                X.Msg.Alert("警告", "您尚未添加任何物料！！！").Show();
                return this.Direct();
            }
        }

        #endregion

        #region 修改申请单,提交操作
        public ActionResult ModifySubmit(string list, string record, string price, string gid)
        {
            List<string> a = cw.JsontoList(list);
            List<string> b = cw.JsontoList(price);
            List<string> c = cw.JsontoList(record);
            T_CH_Operation_list auditprocess = new T_CH_Operation_list();
            var am_old = DB.T_GM_ApplyFixedAsset.Find(gid);
            var am = new T_GM_ApplyFixedAsset();
            var cf = DB.V_CH_Checkfuncflow.Where(w => w.Name == am_old.AuditProcess).ToList().FirstOrDefault();
            if (a.Count > 0 && a[0] != "")
            {
                am_old.Status = "modified once";
                DB.T_GM_ApplyFixedAsset.Attach(am_old);
                DB.Entry(am_old).State = EntityState.Modified;

                am.GID = Guid.NewGuid().ToString();
                am.No_Date = DateTime.Now;
                am.Operator = am_old.Operator;
                am.OperateTime = DateTime.Now;
                am.ApplyNo = am_old.ApplyNo;
                am.AuditProcess = am_old.AuditProcess;
                am.Applicant = am_old.Applicant;
                am.ApplicantSector = am_old.ApplicantSector;
                am.Remark = am_old.Remark;
                DB.T_GM_ApplyFixedAsset.Add(am);
                int i = 0;
                decimal t, f;

                auditprocess.ID = am.GID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
                auditprocess.Url = Url.Action("FAApplyAudit", "Share", new { gid = am.GID });
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
                        detail.NO = am.ApplyNo;
                        detail.Type = "SQF";
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

        

    }
}
