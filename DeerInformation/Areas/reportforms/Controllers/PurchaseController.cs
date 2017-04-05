using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Areas.reportforms.Models;
using DeerInformation.Extensions;
using Ext.Net.MVC;
using Ext.Net;
using Microsoft.Reporting.WebForms;
using DeerInformation.Models;

namespace DeerInformation.Areas.reportforms.Controllers
{
    public class PurchaseController : Controller
    {
        //
        // GET: /reportforms/MaterialApply/
        Entities db = new Entities();
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult MaterialApply()
		{
			return View();
		}
		public ActionResult MaterialSelect(string date)
		{
			Image contentImage = this.GetCmp<Image>("reportform");
			contentImage.ImageUrl = Url.Action("MaterialResult", new { id = "", format = "Image" });
			contentImage.ReRender();
			return this.Direct();
		}
		//返回采购申请单
		public ActionResult MaterialResult(string format = "", string id = "")
        {
            var applyhead = db.V_GM_MApply.Where(w => w.GID == id).ToList().FirstOrDefault();
            if (applyhead.IsEnableNo != "审核通过")
            {
                X.Msg.Alert("提示", "未通过审核的请购单无法导出").Show();
                return this.Direct();
            }
			LocalReport localReport = new LocalReport
			{
				ReportPath = Server.MapPath(@"~\Areas\reportforms\Report\PurchaseReport\MaterialApply.rdlc")
			};
            var dt = new MaterialApply().GetApplyMaterialsList(id);
			ReportDataSource reportDataSource = new ReportDataSource("MaterialApply", dt);
			localReport.DataSources.Add(reportDataSource);

            var project = db.V_GM_DetailProject.Where(w => w.ProjectNo == applyhead.ProjectNo).ToList().FirstOrDefault();
			localReport.SetParameters(new ReportParameter("LocationName", applyhead.FieldName));
            localReport.SetParameters(new ReportParameter("ApplierName", applyhead.Name));
            localReport.SetParameters(new ReportParameter("Tel", applyhead.Tel));
            localReport.SetParameters(new ReportParameter("ProjectName", project.ProjectName));
            localReport.SetParameters(new ReportParameter("ProjectNO", project.ProjectNo));
            localReport.SetParameters(new ReportParameter("OrderNO", project.CustomerNo));
            localReport.SetParameters(new ReportParameter("ApplyTime", applyhead.ApplyTime.Value.ToString()));

			Warning[] warnings;
			string[] streams;

			string reportType;
			string mimeType;
			string encoding;
			string fileNameExtension;

			switch (format)
			{
				case "Image":
					reportType = "png";
					break;
				case "Excel":
					reportType = "xls";
					break;
				case "PDF":
					reportType = "pdf";
					break;
				case "Word":
					reportType = "doc";
					break;
				default:
					reportType = "pdf";
					break;
			}
			string deviceInfo =
				"<DeviceInfo>" +
				"  <OutputFormat>" + reportType + "</OutputFormat>" +
				"</DeviceInfo>";

			var renderedBytes = localReport.Render(
				format,
				deviceInfo,
				out mimeType,
				out encoding,
				out fileNameExtension,
				out streams,
				out warnings);

			return File(renderedBytes, mimeType);
		}

        public ActionResult PurchaseM(string format = "", string id = "")
        {
            
            var purchase = db.V_GM_MPurchase.Where(w => w.GID == id).ToList().FirstOrDefault();
            if (purchase.IsEnableNo!="审核通过")
            {
                X.Msg.Alert("提示", "未通过审核的采购单无法导出").Show();
                return this.Direct();
            }
            var dt = db.V_GM_DM.Where(w => w.Remark == id).ToList();
            var dt2 = db.V_GM_MApply.Where(w => w.GID == purchase.AMGID).ToList().FirstOrDefault();

            LocalReport localReport = new LocalReport
            {
                ReportPath = Server.MapPath(@"~\Areas\reportforms\Report\PurchaseReport\PurchaseList.rdlc")
            };
            
            ReportDataSource reportDataSource = new ReportDataSource("PurchaseM", dt);
            localReport.DataSources.Add(reportDataSource);

            var project = db.V_GM_DetailProject.Where(w => w.ProjectNo == purchase.ProjectNo).ToList().FirstOrDefault();

	        string warehouseid = project.WarehouseID;
	        var warehouse = db.T_GM_Warehouse.First(l => l.WarehouseID == warehouseid);
	        string managerEmployeeId = warehouse.Manager;
			var warehousemanager = db.T_HR_Staff.First(l => l.StaffID == managerEmployeeId);
			localReport.SetParameters(new ReportParameter("FieldName", warehouse.Location));
            localReport.SetParameters(new ReportParameter("Name", warehousemanager.Name));
            localReport.SetParameters(new ReportParameter("ReceiveTel",warehousemanager.TelNum ));

            localReport.SetParameters(new ReportParameter("ProjectName", project.ProjectName));
            localReport.SetParameters(new ReportParameter("DeliverWay", purchase.ReceiptMethod));
            localReport.SetParameters(new ReportParameter("ProjectNO", project.ProjectNo));
            localReport.SetParameters(new ReportParameter("OrderNO", purchase.PurchaseMNo));

	        string supplierid = purchase.SupplierID;
	        var supplier = db.T_GM_SupplierInfo.First(l => l.SupplierID == supplierid);
			localReport.SetParameters(new ReportParameter("Supplier", purchase.SupplierName));
			localReport.SetParameters(new ReportParameter("SupplierAddress", supplier.Address));
			localReport.SetParameters(new ReportParameter("SupplierContacts",supplier.Contact ));
			localReport.SetParameters(new ReportParameter("Tel", supplier.Tel));
			localReport.SetParameters(new ReportParameter("Tax", supplier.Tax));

            localReport.SetParameters(new ReportParameter("PayWay", supplier.PaymentCycle));
            localReport.SetParameters(new ReportParameter("ApplyTime", purchase.PrepaidDay.Value.ToShortDateString()));
			localReport.SetParameters(new ReportParameter("PurchaseStaff", purchase.Name));

	        float sumAmount = 0;
			dt.ForEach(
				delegate (V_GM_DM item )
				{
					sumAmount +=Convert.ToSingle(item.Price * item.Num);
				});
			localReport.SetParameters(new ReportParameter("SumAmount", sumAmount.ToString("N")));
			localReport.SetParameters(new ReportParameter("SumAmountChinese",MoneyConvertChinese.MoneyToChinese(sumAmount.ToString("N")) ));


            Warning[] warnings;
            string[] streams;

            string reportType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            switch (format)
            {
                case "Image":
                    reportType = "png";
                    break;
                case "Excel":
                    reportType = "xls";
                    break;
                case "PDF":
                    reportType = "pdf";
                    break;
                case "Word":
                    reportType = "doc";
                    break;
                default:
                    reportType = "pdf";
                    break;
            }
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>" + reportType + "</OutputFormat>" +
                "</DeviceInfo>";



            var renderedBytes = localReport.Render(
                format,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderedBytes, mimeType);
        }

        public ActionResult GatherM(string format = "", string id = "")
        {
            var dt = new MaterialGather().GetMaterialItems(id);
            LocalReport localReport = new LocalReport
            {
                ReportPath = Server.MapPath(@"~\Areas\reportforms\Report\PurchaseReport\MaterialGather.rdlc")
            };

            ReportDataSource reportDataSource = new ReportDataSource("MaterialGather", dt);
            localReport.DataSources.Add(reportDataSource);

            var project = db.V_GM_DetailProject.Where(w => w.ProjectNo == id).ToList().FirstOrDefault();
            if (project!=null)
            {
                localReport.SetParameters(new ReportParameter("ProjectName", project.ProjectName));
                localReport.SetParameters(new ReportParameter("ProjectNo", project.ProjectNo));
                localReport.SetParameters(new ReportParameter("PONO", project.CustomerNo));
                localReport.SetParameters(new ReportParameter("LocationName", project.FieldName));    
            }
            
            Warning[] warnings;
            string[] streams;

            string reportType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            switch (format)
            {
                case "Image":
                    reportType = "png";
                    break;
                case "Excel":
                    reportType = "xls";
                    break;
                case "PDF":
                    reportType = "pdf";
                    break;
                case "Word":
                    reportType = "doc";
                    break;
                default:
                    reportType = "pdf";
                    break;
            }
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>" + reportType + "</OutputFormat>" +
                "</DeviceInfo>";



            var renderedBytes = localReport.Render(
                format,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderedBytes, mimeType);
        }
    }
}
