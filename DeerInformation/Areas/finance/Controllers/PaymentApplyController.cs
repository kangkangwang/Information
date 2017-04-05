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
    public class PaymentApplyController : Controller
    {
        //
        // GET: /finance/PaymentApply/
        private Entities DB = new Entities();

        /// <summary>
        /// 产生首页view数据
        /// </summary>
        /// <returns></returns>
        /// 
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fiter(StoreRequestParameters parameters, string customername = "", string projectNo = "", string paymentstate = "", string date = "")
        {
			var applylist = new PaymentApply().Select(customername, projectNo, paymentstate, date);

            return this.Store(applylist.GetPage(parameters));
        }
        // GET: /finance/PaymentApply/Add/

        public ActionResult AddView()
        {
            LoginUser user = new LoginUser();
            return View(new PaymentApply() { Creator = user.EmployeeName });
        }
        //
        // GET: /finance/PaymentApply/Create

        public ActionResult Create(PaymentApply pay)
        {
            FileUtility attachFile = new FileUtility();
            FileUploadField upload = this.GetCmp<FileUploadField>("AnnetPath");
            if (upload.HasFile)
            {
                attachFile.File = upload.PostedFile;
                pay.AnnetPath = string.Format("~/AttachFile/PaymentApply/{0}/{1}.{2}", DateTime.Now.Date.ToString("yyyy-MM-dd"), Guid.NewGuid(),
                Path.GetExtension(attachFile.File.FileName));
                attachFile.FilePath = pay.AnnetPath;
                attachFile.SavePath = Server.MapPath(attachFile.FilePath);
                attachFile.FileType = attachFile.File.ContentType;
            }
            if (pay.CreatePaymentApply(this, attachFile))
            {
                X.Msg.Alert("页面消息", "保存成功！", "parent.App.storedata.reload();parent.App.win.close();").Show();
            }
            else
            {
                X.Msg.Alert("页面消息", "保存失败，请确保输入信息正确！").Show();

            }
            return this.Direct();

         }

        /// <summary>
        /// 编辑视图的submit响应  保存用户修改的数据到数据库
        /// </summary>
        /// <param name="pay"></param>
        /// <returns></returns>
        public ActionResult Edit(string id, PaymentApply pay)
        {
            pay.ID = id;
            return this.Direct( pay.UpdatePaymentApply());
        }

        /// <summary>
        /// 移除某条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Remove(string id)
        {
            var payr = DB.T_FD_AccountReceivable.Find(id);
            if (payr != null)
            {
                DB.T_FD_AccountReceivable.Remove(payr);
                DB.SaveChanges();
            }
            return this.Direct();
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        /// <returns></returns>
        [DirectMethod]
        public ActionResult Reload()
        {
            this.GetCmp<Store>("storedata").LoadData(DB.T_FD_AccountReceivable.ToList());
            return this.Direct();
        }

        /// <summary>
        /// 创建新建窗口 窗口加载AddView
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateAction()
        {
            WindowModule nitem = new WindowModule();
            nitem.Loader.Url = Url.Action("AddView");
            nitem.Width = 550;
            nitem.Height = 600;
            nitem.Title = "应收款单";
            nitem.Render();
            return this.Direct();
        }

        /// <summary>
        /// 创建编辑窗口 窗口加载EditView
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditAction(string id)
        {
            WindowModule nitem = new WindowModule();
            nitem.Loader.Url = Url.Action("EditView", "PaymentApply", new { id = id });
            nitem.Width = 550;
            nitem.Height = 600;
            nitem.Title = "应收款单";
            nitem.Render();
            return this.Direct();
        }

        /// <summary>
        /// 产生编辑视图数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditView(string id)
        {
            PaymentApply payedit = new PaymentApply();
            var res = payedit.GetEditPaymentApply(id);
            if (res != null)
            {
                return View(res);
            }
            else
            {
                return View("Unauthorized");
            }


        }
        [DirectMethod(Namespace = "PaymentApply")]
        public ActionResult PaymentApply(string id,string command)
        {
	        WindowModule nwin = new WindowModule
	        {
		        Width = 550,
		        Height = 600,
		        Title = command == "Invoice" ? "发票信息" : "收款信息",
		        Loader =
		        {
			        Url =
				        command == "Invoice"
							? Url.Action("InvoiceLst", "Invoice", new { referenceId = id })
							: Url.Action("EditView", "PaymentApply", new { id = id })
		        }
	        };
	        nwin.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult Detail(string id)
        {
            PaymentApply applydetail = new PaymentApply();
            var obj = applydetail.GetDetailPaymentApply(id);
            if (obj != null)
            {
                return View(obj);
            }
            else
            {
                return View("Unauthorized");
            }
        }

        [DirectMethod(Namespace = "PaymentApply")]
        public ActionResult GetPaidAmount(string projectno)
        {
            PaymentApply paymentApply = new PaymentApply();

            return this.Direct(paymentApply.GetPaidAmount(projectno));
        }
    }
}
