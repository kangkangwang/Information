using DeerInformation.Areas.finance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Models;
using DeerInformation.Areas.system.Models;
using System.Data;
using System.Data.Objects;
using System.Text;
using System.Data.Objects.SqlClient;
using System.EnterpriseServices.Internal;
using System.Globalization;
using System.IO;
using System.Web.Routing;
using System.Web.UI.WebControls;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.finance.Controllers
{
    [DirectController(AreaName = "finance")]
    public class CollectionPlanController : Controller
    {
        private Entities DB = new Entities();
        //
        // GET: /finance/CollectionPlan/
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View();
        }
        
        //
        // GET: /finance/CollectionPlan/Fiter

        public ActionResult Fiter(StoreRequestParameters parameters, string customername = "", string projectid = "",  string date = "")
        {
            var collectionlist = new CollectionPlan().Select(customername, projectid, date);
            return this.Store(collectionlist.GetPage(parameters));
        }

        [VisitAuthorize(Create = true)]
        public ActionResult Add()
        {
            WindowModule window = new WindowModule { Width = 700, Height = 450, Loader = { Url = Url.Action("Create") } };
            window.Render(RenderMode.Auto);
            return this.Direct();
        }

        [VisitAuthorize(Read = true)]
        public ActionResult Read(string id)
        {
            WindowModule window = new WindowModule { Width = 700, Height = 450, Loader = { Url = Url.Action("Detail",new {id=id}) } };
            window.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult Detail(string id)
        {
            return View(new CollectionPlan().Find(id));
        }

        public ActionResult Create()
        {
            return View("CreateView", new CollectionPlan());
        }
        public ActionResult Submit(CollectionPlan cplan)
        {
            FileUtility attachFile = new FileUtility();

            FileUploadField upload = this.GetCmp<FileUploadField>("AnnetPath");

            if (upload.HasFile)
            {
                attachFile.File = upload.PostedFile;
                cplan.Attachment = string.Format("~/AttachFile/CollectionPlan/{0}/{1}.{2}", DateTime.Now.Date.ToString("yyyy-MM-dd"), Guid.NewGuid(),
                    Path.GetExtension(attachFile.File.FileName));
                attachFile.FilePath = cplan.Attachment;
                attachFile.SavePath = Server.MapPath(attachFile.FilePath);
                attachFile.FileType = attachFile.File.ContentType;
            }

            bool flag = cplan.Save(this, attachFile);
            X.Msg.Alert("页面消息", flag ? "提交成功！" : "提交失败！", flag ? "parent.App.win.close();" : null).Show();
            if (flag)
            {
                X.AddScript("parent.App.storedata.reload();");
            }
            return this.Direct();
        }

        public ActionResult GetProject()
        {
            var list = (from o in DB.V_GM_DetailProject
                        select new { o.UID, o.ProjectNo}).ToList();
            return this.Store(list);
        }

        [DirectMethod(Namespace = "CPlan")]
        public ActionResult _SetPoInfo(string id)
        {
            V_GM_DetailProject data = DB.V_GM_DetailProject.FirstOrDefault(l => l.ProjectNo == id);
            if (data != null)
            {
                var t1 = X.GetCmp<TextField>("ProjectName");
                var t2 = X.GetCmp<TextField>("ClientName");
                var t3 = X.GetCmp<TextField>("OfferMoney");
				var t4 = X.GetCmp<TextField>("CustomerNo");

                t1.Text = data.ProjectName;
                t2.Text = data.ClientName;
				t3.Text = data.PurchaseAmount.ToString();
	            t4.Text = data.CustomerNo;
            }
            return this.Direct();
        }

        public ActionResult AddPlan(string num)
        {
            int nums = Convert.ToInt32(num) + 1;
            var collectionType =
                new ComboBox()
                {
                    FieldLabel = "收款类型",
                    Name = string.Format("DetailPlans[{0}].CollectionType", nums),
                    ColumnWidth = 0.25
                };
            collectionType.Items.AddRange(new CollectionPlan().PlanTypeItems);
            var plan = new Container()
            {
                MarginSpec = "5 0 0 0",
                ColumnWidth = 1,
                Layout = LayoutType.Column.ToString(),
                Items =
                {
                    collectionType,
                    new TextField(){FieldLabel = "项目进度",Regex = "^(\\d|[1-9]\\d|100)$",Name = string.Format("DetailPlans[{0}].ProjectSchedule",nums),ColumnWidth = 0.25} ,
                    new NumberField(){FieldLabel ="收款比例",Regex = "^(\\d|[1-9]\\d|100)$",AllowBlank = false,Name = string.Format("DetailPlans[{0}].CollectionRatio",nums),ColumnWidth = 0.25},
                    new NumberField(){FieldLabel ="金额",AllowBlank = false,Name = string.Format("DetailPlans[{0}].CollectionAmount",nums),ColumnWidth = 0.25}
                
                }
            };
            this.GetCmp<TextField>("collectionplan").SetValue(nums);
            plan.AddTo("plans");
            return this.Direct();
        }
        

        
    }
}
