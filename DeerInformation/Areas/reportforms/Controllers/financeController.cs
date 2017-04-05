using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using DeerInformation.Areas.finance.Models;
using DeerInformation.Areas.reportforms.Models;
using Ext.Net.MVC;
using Ext.Net;

namespace DeerInformation.Areas.reportforms.Controllers
{
    public class financeController : Controller
    {
        //
        // GET: /reportforms/finance/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Salary()
		{
			return View();
		}
		
		//查询直接员工工资
		public ActionResult UpdateSalary(string date)
		{
			Image contentImage=this.GetCmp<Image>("reportform");
			contentImage.ImageUrl = Url.Action("SalaryResult", new { date = date, format = "Image" });
			contentImage.ReRender();
			return this.Direct();
		}

		//返回付款申请文件
		public ActionResult FaninceResult(string format = "")
        {
            LocalReport localReport = new LocalReport
            {
                ReportPath = Server.MapPath(@"~\Areas\reportforms\Report\financereport.rdlc")
            };
            var dt = new PaymentApply().Select("","", "","");

            ReportDataSource reportDataSource = new ReportDataSource("payment", dt);
            localReport.DataSources.Add(reportDataSource);

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

		//返回直接员工工资文件
		public ActionResult SalaryResult(string format = "",string date="")
		{
			DateTime dateTime;
			if (!DateTime.TryParse(date,out dateTime))
			{
				dateTime = DateTime.Now;
			}

			LocalReport localReport = new LocalReport
			{
				ReportPath = Server.MapPath(@"~\Areas\reportforms\Report\financeReport\DirectLaborSalary.rdlc")
			};
			var dt = new ReportForm().GetSalaryWithTimes(dateTime);

			ReportDataSource reportDataSource = new ReportDataSource("Salary", dt);
			localReport.DataSources.Add(reportDataSource);
			localReport.SetParameters(new ReportParameter("Month", dateTime.ToString(CultureInfo.InvariantCulture)));

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
