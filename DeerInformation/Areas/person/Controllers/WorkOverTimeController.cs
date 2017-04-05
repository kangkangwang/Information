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
using DeerInformation.Extensions;

namespace DeerInformation.Areas.person.Controllers
{
    [DirectController(AreaName = "person")]
    public class WorkOverTimeController : Controller
    {
        //
        // GET: /person/WorkOverTime/

        Entities entities = new Entities();

        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_WorkOverTime
                            where o.Valid == true
                            select o).ToList();
                ViewData["root"] = Tool.GetNode();
                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }

        }

        public ActionResult DepClick(string dep)
        {
            var list = new List<V_HR_WorkOverTime>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    string[] staff = Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_WorkOverTime
                            where staff.Contains(o.StaffID) && o.Valid == true
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_WorkOverTime
                            where o.Valid == true
                            select o).ToList();
                }

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }//处理树形菜单响应

        public ActionResult Getalldata(string staffid, string name)//查询按钮响应
        {
            try
            {
                var list = SearchData(staffid, name);

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        private List<V_HR_WorkOverTime> SearchData(string staffid, string name)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_WorkOverTime>();

            list = (from o in entities.V_HR_WorkOverTime
                    where o.Valid == true
                    select o).ToList();

            if (!String.IsNullOrEmpty(name))
            {
                list = (from o in list
                        where o.Name.Contains(name)
                        select o).ToList();
            }

            if (!String.IsNullOrEmpty(staffid))
            {
                list = (from o in list
                        where o.StaffID.Contains(staffid)
                        select o).ToList();
            }

            return list;
        }

        [DirectMethod]
        public ActionResult WorkOverTimeReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_WorkOverTime
                            where o.Valid == true
                            select o).ToList();

                var store = X.GetCmp<Store>("WorkOverTimeStore");
                store.LoadData(list);
                return this.Direct();
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

                ID = "windowWorkOverTime",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddWorkOverTime", "WorkOverTime", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.WorkOverTimeReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id)//修改相应
        {
            Window win = new Window
            {

                ID = "windowWorkOverTime",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddWorkOverTime", "WorkOverTime", new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.WorkOverTimeReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);

            return this.Direct();


        }

        public ActionResult AddWorkOverTime(string id)//在修改时传递的为contractid
        {
            if (id == "-1")//-1为添加
            {
                int year = DateTime.Now.Year;
                var y = X.GetCmp<NumberField>("Year");
                y.Value = year;
                return View();
            }
            else//否则为修改
            {
                V_HR_WorkOverTime item = (from o in entities.V_HR_WorkOverTime
                                                     where o.ID == id
                                                     select o).First();

                var p = X.GetCmp<Panel>("Select");
                p.Hidden = true;


                return View(item);
            }
        }

        public ActionResult SelectStaff()
        {
            Window win = new Window
            {

                ID = "window1",
                Title = "",
                Height = 450,
                Width = 700,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("SelectStaff", "SelectStaff", new { area = "", NUM = 1 }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.DealGetpersonWithSDep()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddOrEditWorkOverTime(V_HR_WorkOverTime wot)
        {
            DirectResult r = new DirectResult();
            T_HR_WorkOverTime wotupdate = entities.T_HR_WorkOverTime.Find(wot.ID);

            if (wotupdate == null)//为空为添加
            {
                var list=from o in entities.T_HR_WorkOverTime
                         where o.StaffID==wot.StaffID && o.Year ==wot.Year
                         select o;
                if(!list.Any())
                {
                    T_HR_WorkOverTime wotadd = new T_HR_WorkOverTime();
                    wotadd.ID = Guid.NewGuid().ToString();
                    wotadd.StaffID = wot.StaffID;
                    wotadd.Year = wot.Year;
                    wotadd.LastYear = wot.LastYear;
                    wotadd.CurrentYear = wot.CurrentYear;
                    wotadd.CurrentYearSum = wot.LastYear + wot.CurrentYear;
                    wotadd.Personal = wot.Personal;
                    wotadd.Company = wot.Company;
                    wotadd.CurrentYearLast = wot.LastYear + wot.CurrentYear - wot.Personal - wot.Company;
                    wotadd.Valid = true;
                    wotadd.CreaterName = new LoginUser().EmployeeId;
                    wotadd.CreateTime = DateTime.Now;
                    entities.T_HR_WorkOverTime.Add(wotadd);
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
                else
                {
                    X.Msg.Alert("警告", "此员工本年可调休加班时数记录已存在，不可重复添加！").Show();
                    r.Success = false;
                }
            }
            else//否则为修改
            {
                wotupdate.Year = wot.Year;
                wotupdate.LastYear = wot.LastYear;
                wotupdate.CurrentYear = wot.CurrentYear;
                wotupdate.CurrentYearSum = wot.LastYear + wot.CurrentYear;
                wotupdate.Personal = wot.Personal;
                wotupdate.Company = wot.Company;
                wotupdate.CurrentYearLast = wot.LastYear + wot.CurrentYear - wot.Personal - wot.Company;
                wotupdate.EditorName = new LoginUser().EmployeeId;
                wotupdate.EditeTime = DateTime.Now;
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
            return r;
        }

        [VisitAuthorize(Delete = true)]
        public ActionResult JSDeleteWorkOverTime(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("WorkOverTimeStore");
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["ID"];
                    T_HR_WorkOverTime de = entities.T_HR_WorkOverTime.Find(id);
                    if (de != null)
                    {
                        de.Valid = false;
                        //entities.T_HR_WorkOverTime.Remove(de);
                        try
                        {
                            entities.SaveChanges();
                            s.Remove(id);
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
