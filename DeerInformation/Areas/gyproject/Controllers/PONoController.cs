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
using System.IO;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.gyproject.Controllers
{
    [DirectController(AreaName = "gyproject")]
    public class PONoController : Controller
    {
        //
        // GET: /gyproject/PONo/

        Entities DB = new Entities();
        CommonWay cw = new CommonWay();
        LoginUser user = new LoginUser();

        [DirectMethod]
        public ActionResult PONoReload()//重新加载客户订单页面
        {
            var list = DB.V_GM_DetailProject.ToList();
            cw.Reload(list, "PONoSelect");
            return this.Direct();
        }
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View(DB.V_GM_DetailProject.ToList());
        }

        #region 添加客户订单

        public ActionResult BudgetSee(string prono)
        {
            var list = DB.V_GM_DetailBudget.Where(w => w.ProjectNo == prono).Where(w => w.Description == "审核通过").Where(w => w.BudgetStatus == null).ToList().FirstOrDefault();
            if (list == null)
            {
                X.Msg.Alert("提示", "您输入的项目编号有误","parent.App.window1.close();").Show();
                return this.Direct(); 
            }
            string id = list.GID;
            var p = new Panel()
            {
                Height = 330,
                Layout = "Form",
                Loader = new ComponentLoader()
                {
                    Url = Url.Action("BudgetSeeView", "ProjectBudget", new { ID = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
            };
            p.AddTo(this.GetCmp<Panel>("con"));
            this.GetCmp<Panel>("con").SetActive(true);
            X.GetCmp<ComboBox>("prono").ReadOnly = true;
            return this.Direct();
        }
        public ActionResult POAddView()
        {
            List<ListItem> list = new List<ListItem>();
            var projectno = DB.V_GM_DetailBudget.Where(w => w.Description == "审核通过").Where(w => w.BudgetStatus == null).ToList();
            foreach (var item in projectno)
            {
                ListItem li = new ListItem();
                li.Value = item.ProjectNo;
                list.Add(li);
            }
            ViewBag.prono = list;
            return View();
        }
        [VisitAuthorize(Create = true)]
        public ActionResult PONoAddButton()
        {
            WinModule win = new WinModule();
            win.ID = "window1";
	        win.Height = 715;
            win.Title = "新增客户订单";
            win.Loader.Url = Url.Action("POAddView");
            win.Listeners.Close.Handler = "App.direct.gyproject.PONoReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        public ActionResult PONoAdd(string prono, T_GM_Project am)
        {
            am.UID = Guid.NewGuid().ToString();
            am.BudgetGID = DB.V_GM_DetailBudget.Where(w => w.ProjectNo == prono).Where(w => w.Description == "审核通过").Where(w => w.BudgetStatus == null).ToList().FirstOrDefault().GID;
            am.AgentMan = user.EmployeeId;
            am.AgentDate = DateTime.Now;
			am.OfferStatus= string.IsNullOrEmpty(am.CustomerNo) ? "否" : "是";
            var budget = DB.T_GM_Budget.Find(am.BudgetGID);
            budget.BudgetStatus = "未开工";
            DB.T_GM_Budget.Attach(budget);
            DB.Entry(budget).State= EntityState.Modified;
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
                string logicpath = "~/AttachFile/ProjectPONo/" + filenewname;
                string filepath = Server.MapPath(logicpath);
                am.AttachmentPath = logicpath;
                uploadfile.PostedFile.SaveAs(filepath);
            }

            DB.T_GM_Project.Add(am);

            var material_list = DB.T_GM_DM.Where(w => w.Remark == am.BudgetGID).ToList();
            foreach (var item in material_list)
            {
                var detail = new T_GM_DM();
                detail.Remark = am.UID;
                detail.MFlID = item.MFlID;
                detail.NO = item.NO;
                detail.Num = item.Num;
                detail.Price = item.Price;
                detail.Type = "FPM";
                DB.T_GM_DM.Add(detail);  
            }
            DB.SaveChanges();
            DirectResult result = new DirectResult();
            result.IsUpload = true;
            return result;
        }

        #endregion

        #region 客户订单查询按钮

        public ActionResult SelectPONo(string clientname, string projectname)
        {
            if (clientname.Length > 0 && projectname != "" && projectname.Length > 0)
            {
                return this.Store(DB.V_GM_DetailProject.Where(w => w.ClientName.Contains(clientname)).Where(w => w.ProjectName.Contains(projectname)).ToList());
            }
            else if (clientname.Length <= 0 && projectname.Length > 0 && projectname != "")
            {
                return this.Store(DB.V_GM_DetailProject.Where(w => w.ProjectName.Contains(projectname)).ToList());
            }
            else if (clientname.Length > 0 && projectname == "")
            {
                return this.Store(DB.V_GM_DetailProject.Where(w => w.ClientName.Contains(clientname)).ToList());
            }
            else
            {
                return this.Store(DB.V_GM_DetailProject.ToList());
            }
        }

        #endregion

        #region 客户订单查看按钮
        public ActionResult PONoSeeView(string id)
        {
            return View(DB.T_GM_Project.Find(id));
        }
        public ActionResult ClickSee(string id)
        {
            string bgid = DB.T_GM_Project.Find(id).BudgetGID;
            Window win = new Window
            {
                ID = "win",
                Title = "客户订单明细",
                Height = 800,
                Width = 600,
				Maximizable = true,
                Modal = true,
                Constrain = true,
                CloseAction = CloseAction.Destroy,
                Items =
                {
                    new Panel()
                    {
                        Height = 290,
                        Layout = "Form",
                        Loader=new ComponentLoader()
                        {
                            Url = Url.Action("PONoSeeView", "PONo", new { ID = id }),
                            DisableCaching = true,
                            Mode = LoadMode.Frame
                        }
                    }, 
                    new Panel()
                    {
						Title = "报价详情",
                        Layout = "Form",
                        Height = 510,
                        Loader = new ComponentLoader()
                        {
                            Url = Url.Action("BudgetSeeView", "ProjectBudget", new { ID = bgid }),
                            DisableCaching = true,
                            Mode = LoadMode.Frame
                        },
                    },
                },
            };
            win.Render(RenderMode.Auto);     
            return this.Direct();
        }

        #endregion    

        #region 客户订单修改按钮

        public ActionResult PONoUpdateView(string id)
        {
            return View(DB.T_GM_Project.Find(id));
        }
        [VisitAuthorize(Update = true)]
        public ActionResult PONoUpdate(string id)
        {
            string statu = DB.T_GM_Project.Find(id).OfferStatus;
            if (statu=="异常订单")
            {
                WinModule win = new WinModule();
                win.Title = "修改客户订单";
                win.Height = 400;
                win.Loader.Url = Url.Action("PONoUpdateView", new { ID = id });
                win.Listeners.Close.Handler = "App.direct.gyproject.PONoReload()";
                win.Render(RenderMode.Auto);
            }
            else
            {
                X.Msg.Alert("提示", "只有没有填写客户订单号的异常订单才能修改！！！").Show();
            }  
            return this.Direct();
        }

        public ActionResult PONoModify(T_GM_Project am)
        {
            am.AgentMan = user.EmployeeId;
            am.OfferStatus =string.IsNullOrEmpty( am.CustomerNo)?"否": "是";
            DB.T_GM_Project.Attach(am);
            DB.Entry(am).State = EntityState.Modified;
            DB.SaveChanges();
            return this.Direct();
        }

        #endregion    

    }
}
