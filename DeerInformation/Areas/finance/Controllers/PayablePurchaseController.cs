using System;
using System.Collections.Generic;
using DeerInformation.Areas.finance.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Models;
using DeerInformation.Areas.system.Models;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.finance.Controllers
{
     [DirectController(AreaName = "finance")]
    public class PayablePurchaseController : Controller
    {
        private Entities DB = new Entities();
        //
        // GET: /finance/PayablePurchase/
         [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View(new PurchasePay().PurchaseList);
        }

        //
        // GET: /finance/PayablePurchase/Select/

        public ActionResult Select(string name)
        {
            this.GetCmp<Store>("storedata").LoadData(new PurchasePay() { Keyword = name }.PurchaseList);
            return this.Direct();
        }

        //
        // GET: /finance/PayablePurchase/Fiter/
        //筛选操作
        public ActionResult Fiter(StoreRequestParameters parameters, string suppliername = "", string projectno = "", string paymentstate = "", string invoicestate = "", string confirmtime = "")
        {
            var PurchaseList = new PurchasePay().Select(suppliername, projectno, confirmtime, invoicestate, paymentstate);

            return this.Store(PurchaseList.GetPage(parameters));
        }

        //
        // GET: /finance/PayablePurchase/Remove/
        //移除操作
         [VisitAuthorize(Delete = true)]
        public ActionResult Remove(string id)
        {
            var apay = DB.T_FD_AccountPayable.Find(id);
            if (apay != null)
            {
                DB.T_FD_AccountPayable.Remove(apay);
                DB.SaveChanges();
            }
            return this.Direct();
        }
       

        //
        // GET: /finance/PayablePurchase/ReLoad/
        //重新加载
        [DirectMethod]
        public ActionResult Reload()
        {
            this.GetCmp<Store>("storedata").LoadData(DB.T_FD_AccountPayable.ToList());
            return this.Direct();
        }

        //GET: /finance/PayablePurchase/Edit/
        //编辑
         [VisitAuthorize(Update = true)]
        public ActionResult Edit(string id, PurchasePay ppurpay, string operatortype)
        {
            ppurpay.ID = id;
            X.AddScript("parent.App.storedata.reload();");                                                      
            return this.Direct(ppurpay.UpdatePurchasePay(operatortype));
        }
        //产生编辑视图数据 
        public ActionResult EditView(string receivePmNo, string viewname)
        {
            PurchasePay ppay = new PurchasePay();
            var pp = ppay.GetEditPurchasePay(receivePmNo);
            if (pp != null)
            {
                return View(viewname,pp);
            }
            else return View("Unauthorized");
        }

        //创建编辑窗口 窗口加载EditView
        public ActionResult EditAction(string id)
        {
            WindowModule nitem = new WindowModule();
            nitem.Loader.Url = Url.Action("EditView", new { ReceivePMNo = id });
            nitem.Width = 550;
            nitem.Height = 600;
            nitem.Title = "应付款单";
            nitem.Render();
            return this.Direct();
        }

        public ActionResult EditOperation(string command, string id)
        {

            WindowModule window = new WindowModule
            {
                Width = 550,
                Height = 600
            };
            if (command== "invoiceinfoAdd")
            {
				window.Loader.Url = Url.Action("InvoiceLst", "Invoice", new { referenceId =id});
                window.Title = "发票信息";
            }
            else if (command == "PaymentinfoAdd")
            {
                window.Loader.Url = Url.Action("EditView", new {receivePmNo = id, viewname = "PaymentView"});
                window.Title = "付款信息";
            }
            else
            {
                return this.Direct();
            }
            window.Render(RenderMode.Auto);
            return this.Direct();
        }

    }
}
