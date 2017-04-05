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


namespace DeerInformation.Areas.person.Controllers
{
    [DirectController(AreaName = "person")]
    public class SalaryPerHourController : Controller
    {
        //by:xgw
        // GET: /person/SalaryPerHour/

        Entities entities = new Entities();

        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.T_HR_SalaryPerHour
                                where o.IsValid==true
                                select o).ToList();

                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }

        }

        public ActionResult Getalldata(string validtime, string stafftype)//查询按钮响应
        {
            try
            {
                var list = SearchData(validtime, stafftype);

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult Add()//添加响应
        {
            Window win = new Window
            {

                ID = "windowSalaryPerHour",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddSalaryPerHour", "SalaryPerHour", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.SalaryPerHourReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddOrEditJob(T_HR_SalaryPerHour job)//保存相应
        {
            DirectResult r = new DirectResult();
            //T_HR_Job jobupdate = entities.T_HR_Job.Find(job.JobID);

            //if (jobupdate == null)//为空为添加
            //{
            //    T_HR_Job jobadd = new T_HR_Job();
            //    jobadd.JobID = job.JobID;
            //    jobadd.JobName = job.JobName;
            //    jobadd.DutyID = job.DutyID;
            //    jobadd.Remark = job.Remark;
            //    jobadd.CreaterName = "admin";//后期改为用户名
            //    jobadd.CreateTime = DateTime.Now;
            //    entities.T_HR_Job.Add(jobadd);
            //    try
            //    {
            //        entities.SaveChanges();
            //        r.Success = true;
            //        X.Msg.Alert("提示", "保存成功！", new JFunction { Fn = "closewindow" }).Show();
            //    }
            //    catch (Exception e)
            //    {
            //        X.Msg.Alert("警告", "数据保存失败！<br /> note:" + e.Message, new JFunction { Fn = "closewindow" }).Show();
            //        r.Success = false;
            //    }
            //}
            //else//否则为修改
            //{
            //    jobupdate.JobName = job.JobName;
            //    jobupdate.DutyID = job.DutyID;
            //    jobupdate.Remark = job.Remark;
            //    jobupdate.EditorName = "admin";//后期改为用户名
            //    jobupdate.EditorTime = DateTime.Now;
            //    try
            //    {
            //        entities.SaveChanges();
            //        r.Success = true;
            //        X.Msg.Alert("提示", "修改成功！", new JFunction { Fn = "closewindow" }).Show();
            //    }
            //    catch (Exception e)
            //    {
            //        X.Msg.Alert("警告", "数据修改失败！<br /> note:" + e.Message, new JFunction { Fn = "closewindow" }).Show();
            //        r.Success = false;
            //    }
            //}
            return r;
        }

        public ActionResult AddSalaryPerHour(string id)//在修改时传递的为id
        {
            if (id == "-1")
            {
                return View();
            }
            else//否则为修改
            {
                T_HR_SalaryPerHour item = (from o in entities.T_HR_SalaryPerHour
                                             where o.ID == id
                                             select o).First();

                return View(item);
            }
        }


        public ActionResult Update(string id)//修改相应，id为JobID
        {
            Window win = new Window
            {

                ID = "windowSalaryPerHour",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddSalaryPerHour", "SalaryPerHour", new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.SalaryPerHourReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        private List<T_HR_SalaryPerHour> SearchData(string validtime, string stafftype)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<T_HR_SalaryPerHour>();

            list = (from o in entities.T_HR_SalaryPerHour
                        where o.IsValid == true
                        select o).ToList();

            if (validtime!="null")
            {
                DateTime dt = Convert.ToDateTime(validtime);
                list = (from o in list
                        where o.ValidTime==dt
                        select o).ToList();
            }

            if (stafftype != "null")
            {
                list = (from o in list
                        where o.StaffStype.Contains(stafftype)
                        select o).ToList();
            }

            return list;
        }

        [DirectMethod]
        public ActionResult JobReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.T_HR_SalaryPerHour
                            where o.IsValid == true
                            select o).ToList();

                var store = X.GetCmp<Store>("SalaryPerHourStore");
                store.LoadData(list);
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        [DirectMethod]
        public ActionResult JSDeleteSalaryPerHour(string selectedData)//删除响应
        {
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(selectedData);

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["ID"];
                    T_HR_SalaryPerHour de = entities.T_HR_SalaryPerHour.Find(id);
                    if (de != null)
                    {
                        entities.T_HR_SalaryPerHour.Remove(de);
                        try
                        {
                            entities.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            X.Msg.Alert("警告", "数据删除失败！<br /> note:" + e.Message).Show();
                        }
                    }
                }
            }
            else
            {
                X.Msg.Alert("提示", "未选择任何列！").Show();
            }

            return this.Direct();
        }




    }
}
