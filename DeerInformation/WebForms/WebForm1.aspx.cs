using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DeerInformation.Areas.finance.Models;
using Microsoft.Reporting.WebForms;

namespace DeerInformation.WebForms
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~\Areas\reportforms\Report\financereport.rdlc");
            var dt = new PaymentApply().Select("", "", "", "");

            ReportDataSource reportDataSource = new ReportDataSource("payment", dt);

            ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
            ReportViewer1.LocalReport.Refresh();
        }
    }
}