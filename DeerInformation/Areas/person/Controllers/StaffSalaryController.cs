using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net;
using DeerInformation.Models;
using System.Text;
using System.Xml.Xsl;
using Newtonsoft.Json;
using System.Xml;
using System.Data.Entity.Infrastructure;
using DeerInformation.Areas.person.Models;
using System.Data;
using DeerInformation.BaseType;

namespace DeerInformation.Areas.person.Controllers
{
    [DirectController(AreaName = "person")]
    public class StaffSalaryController : Controller
    {
        //
        // GET: /person/StaffSalary/

        Entities entities = new Entities();

        public ActionResult Index()
        {
            try
            {
                DateTime dt=DateTime.Now;
                var list = (from o in entities.V_HR_StaffSalaryWithTime
                            //where o.Year==dt.Year && o.Month==dt.Month
                            select o).ToList();
                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }

        }

        public ActionResult Getalldata(string month)//查询按钮响应
        {
            try
            {
                var list = SearchData(month);

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        private List<V_HR_StaffSalaryWithTime> SearchData(string month)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_StaffSalaryWithTime>();

            if (!String.IsNullOrEmpty(month) && month.Length == 6)
            {
                int year = Convert.ToInt32(month.Substring(0, 4));
                int m = Convert.ToInt32(month.Substring(4, 2));
                list = (from o in entities.V_HR_StaffSalaryWithTime
                        where o.Year == year && o.Month == m
                        select o).ToList();
            }

            return list;
        }


    }
}
