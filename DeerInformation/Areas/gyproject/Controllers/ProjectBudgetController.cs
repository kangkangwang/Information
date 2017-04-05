using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Extensions;
using System.Data;
using System.IO;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.Areas.gyproject.ShareMethod;
using System.Data.Objects.SqlClient;

namespace DeerInformation.Areas.gyproject.Controllers
{
     [DirectController(AreaName = "gyproject")]
    public class ProjectBudgetController : Controller
    {
         Entities DB = new Entities();
         CommonWay cw = new CommonWay();
         LoginUser user = new LoginUser();
         private string checkname = "报价单";
         private void SharePanel(string title,string condition = "projectbudget",string gid=null)
         {
			 Container p = new Container()
             {
                 MinHeight=10,
                 Loader = new ComponentLoader()
                 {
                     Url = Url.Action("ChooseM", "Share", new { con = condition ,id=gid,tit=title}),
                     DisableCaching = true,
                     Mode = LoadMode.Frame
                 }
             };
	         p.Layout = "Form";
             p.AddTo(this.GetCmp<FormPanel>("storeform"));
             this.GetCmp<FormPanel>("storeform").SetActive(true);
         }

         [DirectMethod]

         public ActionResult BudgetReload()//重新加载报价单页面
        {
            var list = DB.V_GM_DetailBudget.Where(w => w.BudgetStatus != "modified once" || w.BudgetStatus == null).ToList();
            cw.Reload(list, "BudgetSelect");
            return this.Direct();
        }
         [VisitAuthorize(Read = true)]
         public ActionResult Index()
        {
            return View(DB.V_GM_DetailBudget.Where(w => w.BudgetStatus != "modified once" || w.BudgetStatus == null).ToList());
        }
         public ActionResult SetProjectNo(string company,string client)
         {
             var cli = DB.T_GM_CustomerInfo.Where(w => w.CustomerName == client).ToList().FirstOrDefault();
             string customer = cli.CustomerAbbreviation.ToString().Trim() + cli.SubsidiaryNumber.ToString().Trim() + company;
             string fit = string.Format("{0}%", customer);
             var historyno = DB.T_GM_Budget.Where(se => SqlFunctions.PatIndex(fit, se.ProjectNo) > 0).ToList();    
             if (historyno.Count()>0)
             {
                 List<int> projectno = new List<int>();
                 foreach (var item in historyno)
                 {
                     int i;
                     int.TryParse(item.ProjectNo.Substring(5, 4), out i);
                     projectno.Add(i);
                 }
                 customer += (projectno.Max() + 1).ToString("d4");
             }
             else
             {
                 customer += "0001";   
             }           
             X.GetCmp<TextField>("prono").SetValue(customer);
             
             return this.Direct();
         }

         #region 查看历史订单
         public ActionResult BudgetSeeReload(string version)
         {
             WinModule win = new WinModule();
             win.ID = "win_his";
             win.Title = "历史报价单详细信息";
             win.Loader.Url = Url.Action("History", "ProjectBudget", new { id=version });
             win.Render(RenderMode.Auto);
             return this.Direct();
         }
         public ActionResult History(string id)
         {
             Entities DB = new Entities();
             SharePanel("订单物料","projectbudgetsee", id);
             return View(DB.V_GM_DetailBudget.Where(w => w.GID == id).ToList().FirstOrDefault());
         }
         private System.Collections.IEnumerable GetVersionData(string projectno)
         {
             List<object> data = new List<object>();
             var list = DB.T_GM_Budget.Where(w => w.ProjectNo == projectno).ToList();
             foreach (var item in list)
             {
                 if (item.Version!=null)
                 {
                     data.Add(new { ver = item.Version, gid = item.GID });
                 }    
             }
             return data;
         }
         public ActionResult GetVersion(string projectno)
         {
             return this.Store(GetVersionData(projectno));
         }

         #endregion

         #region 添加报价单
         [VisitAuthorize(Create =true)]
         public ActionResult BudgetAddButton()
         {
             WinModule win = new WinModule();
             win.Title = "新增报价单";
             win.Loader.Url = Url.Action("BudgetAddView", "ProjectBudget");
             win.Listeners.Close.Handler = "App.direct.gyproject.BudgetReload()";
             win.Render(RenderMode.Auto);
             return this.Direct();
         }
         public ActionResult BudgetAddView()
         {
             SharePanel("待选择物料"); 
             List<ListItem> li = new List<ListItem>();
             var list = DB.T_GM_CustomerInfo.ToList();
             foreach (var item in list)
             {
                 ListItem listitem = new ListItem();
                 listitem.Value = item.CustomerName;
                 li.Add(listitem);
             }
             ViewBag.client = li;
             ViewBag.audit = cw.SetCombox(checkname);
             return View();
        }
         public ActionResult BudgetAdd(string list, string record, string pri, T_GM_Budget am)
         {     
             T_CH_Operation_list auditprocess = new T_CH_Operation_list();
             var cf = DB.V_CH_Checkfuncflow.Where(w => w.ID == am.AuditProgress).ToList().FirstOrDefault();
             auditprocess.ID = Guid.NewGuid().ToString();
             auditprocess.Check_funcID = cf.CheckfuncID;
             auditprocess.Check_flowID = cf.ID;
             auditprocess.CreateTime = DateTime.Now;
             auditprocess.State = 1;
             auditprocess.Url = Url.Action("ProjectBudgetAudit", "Share", new { gid = auditprocess.ID });
             auditprocess.Creator = user.EmployeeId;
             DB.T_CH_Operation_list.Add(auditprocess);

             am.GID = auditprocess.ID;
             am.AuditProgress = cf.Name;
             am.AgentMan = user.EmployeeId;
             am.AgentDate = DateTime.Now;
             am.Version = 1;

             var uploadfile = this.GetCmp<FileUploadField>("AttachmentPath");
             int filesize = uploadfile.PostedFile.ContentLength;
             string fileoldname = uploadfile.FileName;
             if (filesize > 20 * 1024 * 1024)
             {
                 X.Msg.Alert("提示", "上传文件过大，大小必须低于20M").Show();
                 return this.Direct();
             }      
             if (uploadfile.HasFile)
             {
                 string filenewname = Guid.NewGuid().ToString() + Path.GetExtension(fileoldname);
                 string logicpath = "~/AttachFile/ProjectBudget/" + filenewname;
                 string filepath = Server.MapPath(logicpath);
                 am.AttachmentPath = logicpath;
                 uploadfile.PostedFile.SaveAs(filepath);
             }
         
             DB.T_GM_Budget.Add(am);
             #region 添加物料
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
                 int i = 0;
                 foreach (var item in a)
                 {
                     var detail = new T_GM_DM();
                     detail.Remark = am.GID;
                     detail.MFlID = item;
                     detail.Num = num[i];
                     detail.Price = price[i];
                     detail.NO = am.ProjectNo;
                     detail.Type = "BJD";
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
             DirectResult re = new DirectResult();
             re.IsUpload = true; 
             return re;
         }

         #endregion

         #region 报价单查询按钮
         [VisitAuthorize(Read = true)]
         public ActionResult SelectBudget(string clientname, string projectname)
         {
             if (clientname.Length > 0 && projectname != "" && projectname.Length > 0)
             {
                 return this.Store(DB.V_GM_DetailBudget.Where(w => w.BudgetStatus != "modified once" || w.BudgetStatus == null).Where(w => w.ClientName.Contains(clientname)).Where(w => w.ProjectName.Contains(projectname)).ToList());
             }
             else if (clientname.Length <= 0 && projectname.Length > 0 && projectname != "")
             {
                 return this.Store(DB.V_GM_DetailBudget.Where(w => w.BudgetStatus != "modified once" || w.BudgetStatus == null).Where(w => w.ProjectName.Contains(projectname)).ToList());
             }
             else if (clientname.Length > 0 && projectname == "")
             {
                 return this.Store(DB.V_GM_DetailBudget.Where(w => w.BudgetStatus != "modified once" || w.BudgetStatus == null).Where(w => w.ClientName.Contains(clientname)).ToList());
             }
             else
             {
                 return this.Store(DB.V_GM_DetailBudget.Where(w => w.BudgetStatus != "modified once" || w.BudgetStatus == null).ToList());
             }
         }

         #endregion

         #region 报价单查看按钮

         public ActionResult BudgetSeeView(string id)
         {
             SharePanel("订单物料","projectbudgetsee",id);
             return View(DB.V_GM_DetailBudget.Where(w => w.GID == id).ToList().FirstOrDefault());
         }
         [VisitAuthorize(Read = true)]
         public ActionResult ClickSee(string id)
         {
             WinModule win = new WinModule();
             win.Title = "报价详细信息";
             win.Loader.Url = Url.Action("BudgetSeeView", "ProjectBudget", new { ID = id });
             win.Render(RenderMode.Auto);
             return this.Direct();
         }

         #endregion

         #region 报价单删除
         [VisitAuthorize(Delete = true)]
         [DirectMethod]
         public ActionResult DeleteBudget(string id)
         {
             var record = DB.T_GM_Budget.Find(id);
             DB.T_GM_Budget.Remove(record);
             DB.SaveChanges();
             return this.Direct();
         }

         #endregion

         #region 报价单修改按钮
         public ActionResult BudgetEditView(string ID)
         {
             SharePanel("待选择物料");
             var list = DB.V_GM_DM.Where(w => w.Remark == ID).ToList();
             cw.Reload(list, "DeployedMaterial");
             return View(DB.T_GM_Budget.Find(ID));
         }
         [VisitAuthorize(Update = true)]
         public ActionResult ClickEdit(string id)
         {
             var pass_condition = DB.V_GM_DetailBudget.Where(w => w.GID == id).ToList().FirstOrDefault();
             if (pass_condition.Description == "审核驳回" )//禁止修改审核通过的报价单
             //if (pass_condition.Description=="审核驳回" || pass_condition.Description=="审核通过")
             {
                 WinModule win = new WinModule();
                 win.Title = "修改报价单";
                 win.Loader.Url = Url.Action("BudgetEditView", "ProjectBudget", new { ID = id });
                 win.Listeners.Close.Handler = "App.direct.gyproject.BudgetReload()";
                 win.Render(RenderMode.Auto);
             }
             else
             {
                 X.Msg.Alert("提示", "您没有权限执行该操作！！！").Show();
             }
             
             return this.Direct();
         }
         public ActionResult BudgetEdit(T_GM_Budget am, string list, string record, string pri)
         {
             T_CH_Operation_list auditprocess = new T_CH_Operation_list();
             var cf = DB.V_CH_Checkfuncflow.Where(w => w.Name == am.AuditProgress).ToList().FirstOrDefault();
             auditprocess.ID = Guid.NewGuid().ToString();
             auditprocess.Check_funcID = cf.CheckfuncID;
             auditprocess.Check_flowID = cf.ID;
             auditprocess.CreateTime = DateTime.Now;
             auditprocess.State = 1;
             auditprocess.Url = Url.Action("ProjectBudgetAudit", "Share", new { gid = auditprocess.ID });
             auditprocess.Creator = user.EmployeeId;
             DB.T_CH_Operation_list.Add(auditprocess);

             var am_new = new T_GM_Budget();
             am_new.GID = auditprocess.ID;
             am_new.AgentDate = DateTime.Now;
             am_new.AgentMan = user.EmployeeId;
             am_new.AuditProgress = cf.Name;
             am_new.ClientName = am.ClientName;
             am_new.CostMoney = am.CostMoney;
             am_new.Extra = am.Extra;
             am_new.LaborCost = am.LaborCost;
             am_new.LaborOffer = am.LaborOffer;
             am_new.MaterialCost = am.MaterialCost;
             am_new.MaterialOffer = am.MaterialOffer;
             am_new.OfferDate = am.OfferDate;
             am_new.OfferMoney = am.OfferMoney;
             am_new.OtherFeeCost = am.OtherFeeCost;
             am_new.OtherFeeOffer = am.OtherFeeOffer;
             am_new.ProjectName = am.ProjectName;
             am_new.ProjectNo = am.ProjectNo;
             am_new.QuotationNo = am.QuotationNo;
             am_new.TaxRate = am.TaxRate;
             am_new.Version = am.Version + 1;

             am_new.RateMargin = am.RateMargin;
             am_new.NetMargin = am.NetMargin;

             var uploadfile = this.GetCmp<FileUploadField>("ModifyAttachmentPath");
             int filesize = uploadfile.PostedFile.ContentLength;
             string fileoldname = uploadfile.FileName;
             if (filesize > 20 * 1024 * 1024)
             {
                 X.Msg.Alert("提示", "上传文件过大，大小必须低于20M").Show();
                 return this.Direct();
             }
             if (uploadfile.HasFile)
             {
                 string filenewname = Guid.NewGuid().ToString() + Path.GetExtension(fileoldname);
                 string logicpath = "~/AttachFile/ProjectBudget/"  + filenewname;
                 string filepath = Server.MapPath(logicpath);
                 am_new.AttachmentPath = logicpath;
                 uploadfile.PostedFile.SaveAs(filepath);
             }

             DB.T_GM_Budget.Add(am_new);

             #region 添加物料
             List<string> a = cw.JsontoList(list);
             List<string> b = cw.JsontoList(pri);
             List<string> c = cw.JsontoList(record);
             List<decimal> num = cw.StrtoDec(c);
             List<decimal> price = cw.StrtoDec(b);
             if (!cw.IsPositiveNumArr(num))
             {
                 return this.Direct(false, "数量不能为负数");
             }
             if (a.Count > 0 && a[0] != "" && a[0] !="null" && a.Count==num.Count && a.Count== price.Count)
             {
                 int i = 0;
                 foreach (var item in a)
                 {
                     var detail = new T_GM_DM();
                     detail.Remark = am_new.GID;
                     detail.MFlID = item;
                     detail.Num = num[i];
                     detail.Price = price[i];
                     detail.NO = am.ProjectNo;
                     detail.Type = "BJD";
                     i++;
                     DB.T_GM_DM.Add(detail); 
                 } 
             }
             else
             {
                 return this.Direct();
             }
             #endregion

             am = DB.T_GM_Budget.Find(am.GID);
             am.BudgetStatus = "modified once";
             DB.T_GM_Budget.Attach(am);
             DB.Entry(am).State = EntityState.Modified;
             DB.SaveChanges();

             DirectResult re = new DirectResult();
             re.IsUpload = true;
             return re;
         }

         #endregion
         
    }
}
