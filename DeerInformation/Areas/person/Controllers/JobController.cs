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
using System.IO;
using DeerInformation.Extensions;


namespace DeerInformation.Areas.person.Controllers
{
    [DirectController(AreaName = "person")]
    public class JobController : Controller
    {
        //by:xgw
        // GET: /person/Job/

        Entities entities = new Entities();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = entities.V_HR_JobWithDutyName.ToList();

                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }

        }

        public ActionResult Getalldata(string id, string name)//查询按钮响应
        {
            try
            {
                var list = SearchData(id, name);

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }
        [VisitAuthorize(Create = true)]
        public ActionResult Add()//添加响应
        {
            Window win = new Window
            {

                ID = "windowJob",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddJob", "Job", new { jobid = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.Job1Reload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddOrEditJob(V_HR_JobWithDutyName job)//保存相应
        {
            DirectResult r = new DirectResult();
            T_HR_Job jobupdate = entities.T_HR_Job.Find(job.JobID);

            if (jobupdate == null)//为空为添加
            {
                T_HR_Job jobadd = new T_HR_Job();
                jobadd.JobID = job.JobID;
                jobadd.JobName = job.JobName;
                jobadd.DutyID = job.DutyID;
                jobadd.Remark = job.Remark;
                jobadd.CreaterName = "admin";//后期改为用户名
                jobadd.CreateTime = DateTime.Now;
                entities.T_HR_Job.Add(jobadd);
                try
                {
                    entities.SaveChanges();
                    r.Success = true;
                    X.Msg.Alert("提示", "保存成功！", new JFunction { Fn = "closewindow" }).Show();
                }
                catch (Exception e)
                {
                    X.Msg.Alert("警告", "数据保存失败！<br /> note:" + e.Message, new JFunction { Fn = "closewindow" }).Show();
                    r.Success = false;
                }
            }
            else//否则为修改
            {
                jobupdate.JobName = job.JobName;
                jobupdate.DutyID = job.DutyID;
                jobupdate.Remark = job.Remark;
                jobupdate.EditorName = "admin";//后期改为用户名
                jobupdate.EditorTime = DateTime.Now;
                try
                {
                    entities.SaveChanges();
                    r.Success = true;
                    X.Msg.Alert("提示", "修改成功！", new JFunction { Fn = "closewindow" }).Show();
                }
                catch (Exception e)
                {
                    X.Msg.Alert("警告", "数据修改失败！<br /> note:" + e.Message, new JFunction { Fn = "closewindow" }).Show();
                    r.Success = false;
                }
            }
            return r;
        }

        public ActionResult AddJob(string jobid)//在修改时传递的为jobid
        {
            if (jobid == "-1")//-1为添加，自动生成PositionCategoryID
            {
                string id, idnum;
                int num, n;
                V_HR_JobWithDutyName list = (from o in entities.V_HR_JobWithDutyName
                                            orderby o.JobID descending
                                            select o).First();
                id = list.JobID.ToString();
                num = int.Parse(id.Substring(2, 4)) + 1;
                idnum = num.ToString();
                n = idnum.Length;
                for (int i = 0; i < 4 - n; i++)
                {
                    idnum = "0" + idnum;
                }
                id = "JO" + idnum;
                ViewData["AutoID"] = id;
                return View();
            }
            else//否则为修改
            {
                V_HR_JobWithDutyName item = (from o in entities.V_HR_JobWithDutyName
                                             where o.JobID == jobid
                                            select o).First();
                ViewData["AutoID"] = jobid;

                return View(item);
            }
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id)//修改相应，id为JobID
        {
            Window win = new Window
            {

                ID = "windowJob",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddJob", "Job", new { jobid = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.Job1Reload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        private List<V_HR_JobWithDutyName> SearchData(string id, string name)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_JobWithDutyName>();

            list = entities.V_HR_JobWithDutyName.ToList();

            if (!String.IsNullOrEmpty(id))
            {
                list = (from o in list
                        where o.JobID.Contains(id)
                        select o).ToList();
            }

            if (!String.IsNullOrEmpty(name))
            {
                list = (from o in list
                        where o.JobName.Contains(name)
                        select o).ToList();
            }

            return list;
        }

        public ActionResult GetDuty()
        {
            var list = (from o in entities.T_HR_Duty
                        select new { o.DutyID, o.DutyName }).ToList();
            return this.Store(list);
        }


        [DirectMethod]
        public ActionResult Job1Reload()//刷新gridpanel的store
        {
            try
            {
                var list = entities.V_HR_JobWithDutyName.ToList();

                var store = X.GetCmp<Store>("JobStore");
                store.LoadData(list);
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        [VisitAuthorize(Delete = true)]
        [DirectMethod]
        public ActionResult JSDeleteJob(string selectedData)//删除响应
        {
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(selectedData);

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["JobID"];
                    T_HR_Job de = entities.T_HR_Job.Find(id);
                    if (de != null)
                    {
                        entities.T_HR_Job.Remove(de);
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

        [DirectMethod]
        public ActionResult SetDutyid(string id)
        {
            var idcom = X.GetCmp<TextField>("DutyID");
            idcom.Text = id;
            return this.Direct();
        }



    }
}
