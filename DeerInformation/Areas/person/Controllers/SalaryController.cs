using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DeerInformation.Areas.person.Models;
using Ext.Net.MVC;
using DeerInformation.Models;
using Ext.Net;

namespace DeerInformation.Areas.person.Controllers
{
    public class SalaryController : Controller
    {
        //
        // GET: /person/Salary/

        public ActionResult Index()
        {
             return View(new SalaryModel(){Year = DateTime.Now.Year,Month = DateTime.Now.Month});
        }

        public ActionResult Select(SalaryModel model)
        {
            model.Salarys = null;
            foreach (var item in model.SalaryItems)
            {
                X.GetCmp<NumberField>("Salaryitem"+item.Id).SetValue(item.Value);
            }
            return this.FormPanel(true);
        }
        public ActionResult Save(SalaryModel model)
        {
            X.Msg.Alert("消息", model.UpdateSalarys() ? "薪资修改成功！" : "薪资修改失败！").Show();
            return this.Direct();
        }

        public ActionResult GetEmployee(string query)
        {
            using (Entities db = new Entities())
            {
                var data = db.T_HR_Staff.Where(o=>o.HireState=="在职");
                string fitformat = string.Format("%{0}%", query);
                var result = data.Where(se => SqlFunctions.PatIndex(fitformat, se.Name) > 0)
                    .Select(m => new { Name = m.Name, ID = m.StaffID })
                    .Take(5).ToList();
                return this.Store(result.Distinct());
            }

        }
    }
}
