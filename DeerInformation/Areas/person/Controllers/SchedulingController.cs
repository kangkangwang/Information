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
    public class SchedulingController : Controller
    {
        //by:xgw
        // GET: /person/Duty/

        Entities entities = new Entities();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var sclist = entities.T_HR_Scheduling.ToList();
                var wolist = entities.T_HR_SchList.ToList();
                var SchedulingStore = X.GetCmp<Store>("SchedulingStore");
                SchedulingStore.LoadData(sclist);
                List<SchListModel> sm = new List<SchListModel>();
                foreach (var item in wolist)
                {
                    SchListModel slm = new SchListModel();
                    slm.ID = item.ID;
                    slm.Name = item.Name;
                    slm.AMTime = item.AMClassGoOn.ToString().Substring(0, 5) + "-" + item.AMClassOff.ToString().Substring(0, 5);
                    slm.AMValidGoTime = item.ACGST.ToString().Substring(0, 5) + "-" + item.ACGET.ToString().Substring(0, 5);
                    slm.AMValidOffTime = item.ACOST.ToString().Substring(0, 5) + "-" + item.ACOET.ToString().Substring(0, 5);
                    slm.PMTime = item.PMClassGoOn.ToString().Substring(0, 5) + "-" + item.PMClassOff.ToString().Substring(0, 5);
                    slm.PMValidGoTime = item.PCGST.ToString().Substring(0, 5) + "-" + item.PCGET.ToString().Substring(0, 5);
                    slm.PMValidOffTime = item.PCOST.ToString().Substring(0, 5) + "-" + item.PCOET.ToString().Substring(0, 5);
                    slm.Two = Convert.ToBoolean(item.Two);
                    slm.Four = Convert.ToBoolean(item.Four);
                    if (slm.Two)
                    {
                        slm.AttNum = "2次卡";
                    }
                    if (slm.Four)
                    {
                        slm.AttNum = "4次卡";
                    }
                    slm.CreateTime = Convert.ToDateTime(item.CreateTime);
                    slm.CreaterName = item.CreaterName;
                    slm.EditeTime = Convert.ToDateTime(item.EditeTime);
                    slm.EditorName = item.EditorName;
                    sm.Add(slm);
                }
                var WorkTimeStore = X.GetCmp<Store>("WorkTimeStore");
                WorkTimeStore.LoadData(sm);

                return View();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }

        }

        public ActionResult Getalldata(string starttime)//查询按钮响应
        {
            try
            {
                List<T_HR_Scheduling> sclist = new List<T_HR_Scheduling>();
                //List<T_HR_WorkTime> wolist = new List<T_HR_WorkTime>();
                if (!String.IsNullOrEmpty(starttime))
                {

                    int year = Convert.ToInt32(starttime.Substring(0, 4));
                    int month = Convert.ToInt32(starttime.Substring(4, 2));
                    string s1 = year.ToString() + "-" + month.ToString() + "-01";
                    DateTime dt = Convert.ToDateTime(s1);
                    DateTime dt1 = dt.AddMonths(1);
                    sclist = (from o in entities.T_HR_Scheduling
                              where o.StartTime >= dt && o.EndTime <= dt1
                              select o).ToList();
                    //wolist = (from o in entities.T_HR_WorkTime
                    //          where o.Month == dt
                    //          select o).ToList();
                }
                else
                {
                    sclist = entities.T_HR_Scheduling.ToList();
                    //wolist = entities.T_HR_WorkTime.ToList();
                }
                var SchedulingStore = X.GetCmp<Store>("SchedulingStore");
                SchedulingStore.LoadData(sclist);
                //var WorkTimeStore = X.GetCmp<Store>("WorkTimeStore");
                //WorkTimeStore.LoadData(wolist);
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }
        [VisitAuthorize(Create = true)]
        public ActionResult Add(string type)//添加响应
        {
            string actionname, controllername;
            if (type == "1")
            {
                actionname = "AddScheduling";
                controllername = "Scheduling";
            }
            else
            {
                actionname = "AddWorkTime";
                controllername = "Scheduling";
            }
            Window win = new Window
            {

                ID = "window",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action(actionname, controllername, new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.SchReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddOrEditScheduling(T_HR_Scheduling scheduling)//Addscheduling保存相应
        {
            DirectResult r = new DirectResult();
            DateTime dt1 = Convert.ToDateTime(scheduling.StartTime), dt2 = Convert.ToDateTime(scheduling.EndTime);
            //确保新添加的休息日不包含已存在的休息日且不被已存在的休息日包含
            if (dt1.Month != dt2.Month || dt1.Year != dt2.Year)
            {
                X.Msg.Alert("警告", "不允许跨年或跨月设置休息日！").Show();
                r.Success = false;
            }
            else
            {
                if (dt1 > dt2)
                {
                    X.Msg.Alert("警告", "结束时间早于开始时间！").Show();
                    r.Success = false;
                }
                else
                {
                    if (ScDate(dt1, dt2))
                    {
                        X.Msg.Alert("警告", "休息日时段已存在！").Show();
                        r.Success = false;
                    }
                    else
                    {
                        T_HR_Scheduling scupdate = entities.T_HR_Scheduling.Find(scheduling.ID);

                        if (scupdate == null)//为空为添加
                        {
                            T_HR_Scheduling scadd = new T_HR_Scheduling();
                            scadd.ID = Tool.ProduceSed64();
                            scadd.StartTime = scheduling.StartTime;
                            scadd.HolidayType1 = "上午";
                            scadd.EndTime = scheduling.EndTime;
                            scadd.HolidayType2 = "下午";
                            scadd.Remark = scheduling.Remark;
                            scadd.CreaterName = "admin";//后期改为用户名
                            scadd.CreateTime = DateTime.Now;
                            entities.T_HR_Scheduling.Add(scadd);
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
                            scupdate.StartTime = scheduling.StartTime;
                            scupdate.HolidayType1 = "上午";
                            scupdate.EndTime = scheduling.EndTime;
                            scupdate.HolidayType2 = "下午";
                            scupdate.Remark = scheduling.Remark;
                            scupdate.EditorName = "admin";//后期改为用户名
                            scupdate.EditeTime = DateTime.Now;
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

                    }

                }

            }


            return r;
        }

        public ActionResult AddOrEditTime(SchListModel worktime)//Addworktime保存相应
        {
            if (!ValidTime(worktime))
            {
                X.Msg.Alert("警告", "存在开始时间不小于结束时间的情况！").Show();
                return this.Direct();
            }
            DirectResult r = new DirectResult();
            T_HR_SchList wtupdate = entities.T_HR_SchList.Find(worktime.ID);

            if (wtupdate == null)//为空为添加
            {
                worktime.ID = Tool.ProduceSed64();
                worktime.CreaterName = new LoginUser().EmployeeId;//后期改为用户名
                worktime.CreateTime = DateTime.Now;
                entities.T_HR_SchList.Add(worktime.ToDB(1));
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
                wtupdate.Name = worktime.Name;
                wtupdate.AMClassGoOn = worktime.AMTime.Substring(0, 5) + ":00";
                wtupdate.AMClassOff = worktime.AMTime.Substring(6, 5) + ":00";
                wtupdate.ACGST = worktime.AMValidGoTime.Substring(0, 5) + ":00";
                wtupdate.ACGET = worktime.AMValidGoTime.Substring(6, 5) + ":00";
                wtupdate.ACOST = worktime.AMValidOffTime.Substring(0, 5) + ":00";
                wtupdate.ACOET = worktime.AMValidOffTime.Substring(6, 5) + ":00";
                wtupdate.PMClassGoOn = worktime.PMTime.Substring(0, 5) + ":00";
                wtupdate.PMClassOff = worktime.PMTime.Substring(6, 5) + ":00";
                wtupdate.PCGST = worktime.PMValidGoTime.Substring(0, 5) + ":00";
                wtupdate.PCGET = worktime.PMValidGoTime.Substring(6, 5) + ":00";
                wtupdate.PCOST = worktime.PMValidOffTime.Substring(0, 5) + ":00";
                wtupdate.PCOET = worktime.PMValidOffTime.Substring(6, 5) + ":00";
                if (worktime.AttNum == "2次卡")
                {
                    worktime.Two = true;
                    worktime.Four = false;
                }
                else
                {
                    worktime.Two = false;
                    worktime.Four = true;
                }
                wtupdate.Two = worktime.Two;
                wtupdate.Four = worktime.Four;
                wtupdate.EditorName = new LoginUser().EmployeeId;//后期改为用户名
                wtupdate.EditeTime = DateTime.Now;
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

        public bool ValidTime(SchListModel wt)
        {
            bool flag = true;
            string AMClassGoOn = "2012-12-12 " + wt.AMTime.Substring(0, 5) + ":00";
            string AMClassOff = "2012-12-12 " + wt.AMTime.Substring(6, 5) + ":00";
            string ACGST = "2012-12-12 " + wt.AMValidGoTime.Substring(0, 5) + ":00";
            string ACGET = "2012-12-12 " + wt.AMValidGoTime.Substring(6, 5) + ":00";
            string ACOST = "2012-12-12 " + wt.AMValidOffTime.Substring(0, 5) + ":00";
            string ACOET = "2012-12-12 " + wt.AMValidOffTime.Substring(6, 5) + ":00";
            string PMClassGoOn = "2012-12-12 " + wt.PMTime.Substring(0, 5) + ":00";
            string PMClassOff = "2012-12-12 " + wt.PMTime.Substring(6, 5) + ":00";
            string PCGST = "2012-12-12 " + wt.PMValidGoTime.Substring(0, 5) + ":00";
            string PCGET = "2012-12-12 " + wt.PMValidGoTime.Substring(6, 5) + ":00";
            string PCOST = "2012-12-12 " + wt.PMValidOffTime.Substring(0, 5) + ":00";
            string PCOET = "2012-12-12 " + wt.PMValidOffTime.Substring(6, 5) + ":00";

            DateTime dt1, dt2;

            dt1 = Convert.ToDateTime(AMClassGoOn);
            dt2 = Convert.ToDateTime(AMClassOff);
            if (dt1 >= dt2)
            {
                flag = false;
            }

            dt1 = Convert.ToDateTime(ACGST);
            dt2 = Convert.ToDateTime(ACGET);
            if (dt1 >= dt2)
            {
                flag = false;
            }

            dt1 = Convert.ToDateTime(ACOST);
            dt2 = Convert.ToDateTime(ACOET);
            if (dt1 >= dt2)
            {
                flag = false;
            }

            dt1 = Convert.ToDateTime(PMClassGoOn);
            dt2 = Convert.ToDateTime(PMClassOff);
            if (dt1 >= dt2)
            {
                flag = false;
            }

            dt1 = Convert.ToDateTime(PCGST);
            dt2 = Convert.ToDateTime(PCGET);
            if (dt1 >= dt2)
            {
                flag = false;
            }

            dt1 = Convert.ToDateTime(PCOST);
            dt2 = Convert.ToDateTime(PCOET);
            if (dt1 >= dt2)
            {
                flag = false;
            }

            return flag;
        }

        public ActionResult AddScheduling(string id)//在修改时传递的为ID
        {
            if (id == "-1")//-1为添加，自动生成PositionCategoryID
            {
                return View();
            }
            else//否则为修改
            {
                T_HR_Scheduling item = (from o in entities.T_HR_Scheduling
                                        where o.ID == id
                                        select o).First();

                return View(item);
            }
        }

        public ActionResult AddWorkTime(string id)//在修改时传递的为ID
        {
            if (id == "-1")//-1为添加，自动生成PositionCategoryID
            {
                return View();
            }
            else//否则为修改
            {
                T_HR_SchList item = (from o in entities.T_HR_SchList
                                     where o.ID == id
                                     select o).First();

                SchListModel slm = new SchListModel();
                slm.ID = item.ID;
                slm.Name = item.Name;
                slm.AMTime = item.AMClassGoOn.ToString().Substring(0, 5) + "-" + item.AMClassOff.ToString().Substring(0, 5);
                slm.AMValidGoTime = item.ACGST.ToString().Substring(0, 5) + "-" + item.ACGET.ToString().Substring(0, 5);
                slm.AMValidOffTime = item.ACOST.ToString().Substring(0, 5) + "-" + item.ACOET.ToString().Substring(0, 5);
                slm.PMTime = item.PMClassGoOn.ToString().Substring(0, 5) + "-" + item.PMClassOff.ToString().Substring(0, 5);
                slm.PMValidGoTime = item.PCGST.ToString().Substring(0, 5) + "-" + item.PCGET.ToString().Substring(0, 5);
                slm.PMValidOffTime = item.PCOST.ToString().Substring(0, 5) + "-" + item.PCOET.ToString().Substring(0, 5);
                slm.Two = Convert.ToBoolean(item.Two);
                slm.Four = Convert.ToBoolean(item.Four);
                if (slm.Two)
                {
                    slm.AttNum = "2次卡";
                }
                if (slm.Four)
                {
                    slm.AttNum = "4次卡";
                }
                slm.CreateTime = Convert.ToDateTime(item.CreateTime);
                slm.CreaterName = item.CreaterName;
                slm.EditeTime = Convert.ToDateTime(item.EditeTime);
                slm.EditorName = item.EditorName;

                return View(slm);
            }
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id, string type)//修改相应，id为DutyID
        {
            string actionname, controllername;
            if (type == "1")
            {
                actionname = "AddScheduling";
                controllername = "Scheduling";
            }
            else
            {
                actionname = "AddWorkTime";
                controllername = "Scheduling";
            }
            Window win = new Window
            {

                ID = "window",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action(actionname, controllername, new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.SchReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();


        }

        [DirectMethod]
        public ActionResult SchReload()//刷新gridpanel的store
        {
            try
            {
                var sclist = entities.T_HR_Scheduling.ToList();
                var wolist = entities.T_HR_SchList.ToList();
                var SchedulingStore = X.GetCmp<Store>("SchedulingStore");
                SchedulingStore.LoadData(sclist);
                List<SchListModel> sm = new List<SchListModel>();
                foreach (var item in wolist)
                {
                    SchListModel slm = new SchListModel();
                    slm.ID = item.ID;
                    slm.Name = item.Name;
                    slm.AMTime = item.AMClassGoOn.ToString().Substring(0, 5) + "-" + item.AMClassOff.ToString().Substring(0, 5);
                    slm.AMValidGoTime = item.ACGST.ToString().Substring(0, 5) + "-" + item.ACGET.ToString().Substring(0, 5);
                    slm.AMValidOffTime = item.ACOST.ToString().Substring(0, 5) + "-" + item.ACOET.ToString().Substring(0, 5);
                    slm.PMTime = item.PMClassGoOn.ToString().Substring(0, 5) + "-" + item.PMClassOff.ToString().Substring(0, 5);
                    slm.PMValidGoTime = item.PCGST.ToString().Substring(0, 5) + "-" + item.PCGET.ToString().Substring(0, 5);
                    slm.PMValidOffTime = item.PCOST.ToString().Substring(0, 5) + "-" + item.PCOET.ToString().Substring(0, 5);
                    slm.Two = Convert.ToBoolean(item.Two);
                    slm.Four = Convert.ToBoolean(item.Four);
                    if (slm.Two)
                    {
                        slm.AttNum = "2次卡";
                    }
                    if (slm.Four)
                    {
                        slm.AttNum = "4次卡";
                    }
                    slm.CreateTime = Convert.ToDateTime(item.CreateTime);
                    slm.CreaterName = item.CreaterName;
                    slm.EditeTime = Convert.ToDateTime(item.EditeTime);
                    slm.EditorName = item.EditorName;
                    sm.Add(slm);
                }
                var WorkTimeStore = X.GetCmp<Store>("WorkTimeStore");
                WorkTimeStore.LoadData(sm);

                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }
        [VisitAuthorize(Delete = true)]
        public ActionResult JSDeleteSch(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("SchedulingStore");
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            foreach (Dictionary<string, string> row in values)
            {
                id = row["ID"];
                T_HR_Scheduling de = entities.T_HR_Scheduling.Find(id);
                if (de != null)
                {
                    entities.T_HR_Scheduling.Remove(de);
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


            return this.Direct();
        }
        [VisitAuthorize(Delete = true)]
        public ActionResult JSDeleteSchList(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("WorkTimeStore");
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            foreach (Dictionary<string, string> row in values)
            {
                id = row["ID"];
                T_HR_SchList de = entities.T_HR_SchList.Find(id);
                var list = from o in entities.T_HR_SchListWithStaff
                           where o.SchListID == id
                           select o;
                if (de != null)
                {
                    entities.T_HR_SchList.Remove(de);
                    foreach (var item in list)
                    {
                        T_HR_SchListWithStaff sc = entities.T_HR_SchListWithStaff.Find(item.ID);
                        entities.T_HR_SchListWithStaff.Remove(sc);
                    }
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


            return this.Direct();
        }


        private bool ScDate(DateTime dt1, DateTime dt2)
        {
            bool flag = false;
            try
            {
                //确保新添加的休息日不包含已存在的休息日且不被已存在的休息日包含
                var list1 = from o in entities.T_HR_Scheduling
                            where dt1 >= o.StartTime && dt2 <= o.EndTime
                            select o;
                var list2 = from o in entities.T_HR_Scheduling
                            where dt1 <= o.StartTime && dt2 >= o.EndTime
                            select o;
                var list3 = from o in entities.T_HR_Scheduling
                            where dt1 <= o.StartTime && dt2 <= o.EndTime && dt2 >= o.StartTime
                            select o;
                var list4 = from o in entities.T_HR_Scheduling
                            where dt1 >= o.StartTime && dt2 >= o.EndTime && dt1 <= o.EndTime
                            select o;

                if (list1.Count() != 0 || list2.Count() != 0 || list3.Count() != 0 || list4.Count() != 0)
                {
                    flag = true;
                }
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据修改失败！<br /> note:" + e.Message).Show();
                flag = true;
            }
            return flag;
        }
        [VisitAuthorize(Create = true)]
        public ActionResult AddSchStaff(string id)
        {
            Window win = new Window
            {

                ID = "windowStaff",
                Title = "详细信息",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddStaff", "Scheduling", new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.SchReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();


        }

        public ActionResult AddStaff(string id)
        {
            Session["s_schlistid"] = id;

            var s = from o in entities.T_HR_SchList
                    where o.ID == id
                    select o;
            SchListModel slm = new SchListModel();
            if (s.Any())
            {
                T_HR_SchList item = s.First();
                slm.ID = item.ID;
                slm.Name = item.Name;
                slm.AMTime = item.AMClassGoOn.ToString().Substring(0, 5) + "-" + item.AMClassOff.ToString().Substring(0, 5);
                slm.AMValidGoTime = item.ACGST.ToString().Substring(0, 5) + "-" + item.ACGET.ToString().Substring(0, 5);
                slm.AMValidOffTime = item.ACOST.ToString().Substring(0, 5) + "-" + item.ACOET.ToString().Substring(0, 5);
                slm.PMTime = item.PMClassGoOn.ToString().Substring(0, 5) + "-" + item.PMClassOff.ToString().Substring(0, 5);
                slm.PMValidGoTime = item.PCGST.ToString().Substring(0, 5) + "-" + item.PCGET.ToString().Substring(0, 5);
                slm.PMValidOffTime = item.PCOST.ToString().Substring(0, 5) + "-" + item.PCOET.ToString().Substring(0, 5);
                slm.Two = Convert.ToBoolean(item.Two);
                slm.Four = Convert.ToBoolean(item.Four);
                if (slm.Two)
                {
                    slm.AttNum = "2次卡";
                }
                if (slm.Four)
                {
                    slm.AttNum = "4次卡";
                }
                if (item.CreateTime != null)
                {
                    slm.CreateTime = Convert.ToDateTime(item.CreateTime);
                }
                slm.CreaterName = item.CreaterName;
                if (item.EditeTime != null)
                {
                    slm.EditeTime = Convert.ToDateTime(item.EditeTime);
                }
                slm.EditorName = item.EditorName;

                var textid = X.GetCmp<TextField>("ID");
                textid.Text = slm.ID;
                var textName = X.GetCmp<TextField>("Name");
                textName.Text = slm.Name;
                var textAMTime = X.GetCmp<TextField>("AMTime");
                textAMTime.Text = slm.AMTime;
                var textAMValidGoTime = X.GetCmp<TextField>("AMValidGoTime");
                textAMValidGoTime.Text = slm.AMValidGoTime;
                var textAMValidOffTime = X.GetCmp<TextField>("AMValidOffTime");
                textAMValidOffTime.Text = slm.AMValidOffTime;
                var textCreaterName = X.GetCmp<TextField>("CreaterName");
                textCreaterName.Text = slm.CreaterName;
                var textEditorName = X.GetCmp<TextField>("EditorName");
                textEditorName.Text = slm.EditorName;
                var textPMTime = X.GetCmp<TextField>("PMTime");
                textPMTime.Text = slm.PMTime;
                var textPMValidGoTime = X.GetCmp<TextField>("PMValidGoTime");
                textPMValidGoTime.Text = slm.PMValidGoTime;
                var textPMValidOffTime = X.GetCmp<TextField>("PMValidOffTime");
                textPMValidOffTime.Text = slm.PMValidOffTime;
                var textAttNum = X.GetCmp<TextField>("AttNum");
                textAttNum.Text = slm.AttNum;
                if (item.CreateTime != null)
                {
                    var textCreateTime = X.GetCmp<TextField>("CreateTime");
                    textCreateTime.Text = slm.CreateTime.ToString();
                }
                if (item.EditeTime != null)
                {
                    var textEditeTime = X.GetCmp<TextField>("EditeTime");
                    textEditeTime.Text = slm.EditeTime.ToString();
                }

            }
            try
            {
                var list = from o in entities.V_HR_SchListWithStaff
                           where o.SchListID == id
                           select o;
                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
            //return View(slm);
        }

        public ActionResult JSDeleteSchListStaff(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("StaffStore");
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            foreach (Dictionary<string, string> row in values)
            {
                id = row["ID"];
                T_HR_SchListWithStaff de = entities.T_HR_SchListWithStaff.Find(id);
                if (de != null)
                {
                    entities.T_HR_SchListWithStaff.Remove(de);
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


            return this.Direct();
        }

        public ActionResult SelectStaff()
        {
            //TempData["s_schlistid"] = schlistid;
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
                    Url = Url.Action("SelectStaff", "SelectStaff", new { area = "", NUM = 999999, SchFilt = 1 }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.DealGetSchListperson()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        [DirectMethod]
        public ActionResult DealGetSchListperson()
        {
            if (TempData["SelectedStaffList"] != null)
            {
                List<V_HR_IDNameWithDepName> s = (List<V_HR_IDNameWithDepName>)TempData["SelectedStaffList"];
                string schlistid = (string)Session["s_schlistid"];
                if (s.Count > 0)
                {
                    try
                    {
                        foreach (var item in s)
                        {
                            T_HR_SchListWithStaff slws = new T_HR_SchListWithStaff();
                            slws.ID = Tool.ProduceSed64();
                            slws.SchListID = schlistid;
                            slws.StaffID = item.StaffID;
                            slws.CreaterName = new LoginUser().EmployeeId;
                            slws.CreateTime = DateTime.Now;
                            entities.T_HR_SchListWithStaff.Add(slws);
                        }
                        entities.SaveChanges();

                        var list = from o in entities.V_HR_SchListWithStaff
                                   where o.SchListID == schlistid
                                   select o;
                        var store = X.GetCmp<Store>("StaffStore");
                        store.LoadData(list);
                    }
                    catch (Exception e)
                    {
                        X.Msg.Alert("警告", "数据保存失败！<br /> note:" + e.Message).Show();
                        return this.Direct();
                    }
                }
            }

            return this.Direct();
        }


        public ActionResult GetallStaffdata(string staffid, string name, string schlistid)//查询按钮响应
        {
            try
            {
                var list = SearchData(staffid, name, schlistid);

                var store = X.GetCmp<Store>("StaffStore");
                store.LoadData(list);

                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        private List<V_HR_SchListWithStaff> SearchData(string staffid, string name, string schlistid)//查询时根据ID和Name进行模糊查询
        {
            var list = (from o in entities.V_HR_SchListWithStaff
                        where o.SchListID == schlistid
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
                        where o.ID.Contains(staffid)
                        select o).ToList();
            }

            return list;
        }

        public ActionResult GetID1()
        {
            var list = (from o in entities.T_HR_Department1
                        orderby o.DOrder
                        select new { o.ID1, o.Department1Name }).ToList();
            return this.Store(list);
        }

        public ActionResult GetID2(string id1)
        {
            if (!String.IsNullOrEmpty(id1))
            {
                var list = (from o in entities.T_HR_Department2
                            where o.ID1 == id1
                            orderby o.DOrder
                            select new { o.ID2, o.Department2Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department2
                            orderby o.DOrder
                            select new { o.ID2, o.Department2Name }).ToList();
                return this.Store(list);
            }
        }

        public ActionResult GetID3(string id2)
        {
            if (!String.IsNullOrEmpty(id2))
            {
                var list = (from o in entities.T_HR_Department3
                            where o.ID2 == id2
                            orderby o.DOrder
                            select new { o.ID3, o.Department3Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department3
                            orderby o.DOrder
                            select new { o.ID3, o.Department3Name }).ToList();
                return this.Store(list);
            }
        }

        public ActionResult GetID4(string id3)
        {
            if (!String.IsNullOrEmpty(id3))
            {
                var list = (from o in entities.T_HR_Department4
                            where o.ID3 == id3
                            orderby o.DOrder
                            select new { o.ID4, o.Department4Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department4
                            orderby o.DOrder
                            select new { o.ID4, o.Department4Name }).ToList();
                return this.Store(list);
            }
        }

        public ActionResult GetID5(string id4)
        {
            if (!String.IsNullOrEmpty(id4))
            {
                var list = (from o in entities.T_HR_Department5
                            where o.ID4 == id4
                            orderby o.DOrder
                            select new { o.ID5, o.Department5Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department5
                            orderby o.DOrder
                            select new { o.ID5, o.Department5Name }).ToList();
                return this.Store(list);
            }
        }


        public ActionResult AddStaffFromDep()
        {
            Window win = new Window
            {

                ID = "windowDepartmentStaff",
                Title = "按部门添加员工",
                Height = 450,
                Width = 700,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("Department", "Scheduling"),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.SchStaffReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();


        }

        [DirectMethod]
        public ActionResult SchStaffReload()//刷新gridpanel的store
        {
            try
            {
                string schlistid = (string)Session["s_schlistid"];
                var list = from o in entities.V_HR_SchListWithStaff
                           where o.SchListID == schlistid
                           select o;
                var store = X.GetCmp<Store>("StaffStore");
                store.LoadData(list);

                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult Department()
        {
            return View();
        }

        public ActionResult AddSchStaffFromDep(string id1, string id2, string id3, string id4, string id5)
        {
            DirectResult r = new DirectResult();
            r.Success = true;
            if (id1 == "null")
            {
                id1 = null;
            }
            if (id2 == "null")
            {
                id2 = null;
            }
            if (id3 == "null")
            {
                id3 = null;
            }
            if (id4 == "null")
            {
                id4 = null;
            }
            if (id5 == "null")
            {
                id5 = null;
            }

            string[] staffid = null;
            if (NoNullDep(id1, id2, id3, id4, id5))
            {
                staffid = GetStaffIDFromDep(id1, id2, id3, id4, id5);
            }
            else
            {
                X.Msg.Alert("警告", "部门不可跳过为空！").Show();
                return this.Direct();
            }

            if (staffid != null)
            {
                string schlistid = (string)Session["s_schlistid"];
                try
                {
                    foreach (var item in staffid)
                    {
                        var lists = from o in entities.T_HR_SchListWithStaff
                                    where o.StaffID == item
                                    select o;

                        if (!lists.Any())
                        {
                            T_HR_SchListWithStaff slws = new T_HR_SchListWithStaff();
                            slws.ID = Tool.ProduceSed64();
                            slws.SchListID = schlistid;
                            slws.StaffID = item;
                            slws.CreaterName = new LoginUser().EmployeeId;
                            slws.CreateTime = DateTime.Now;
                            entities.T_HR_SchListWithStaff.Add(slws);
                        }
                    }
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

        private string[] GetStaffIDFromDep(string id1, string id2, string id3, string id4, string id5)
        {
            string[] staffid = null;

            if (String.IsNullOrEmpty(id5) && String.IsNullOrEmpty(id4) && String.IsNullOrEmpty(id3) && String.IsNullOrEmpty(id2) && String.IsNullOrEmpty(id1))
            {
                staffid = null;
            }
            else
            {
                var list = entities.T_HR_Staff.Where(o => o.HireState == "在职").ToList();
                if (!String.IsNullOrEmpty(id1))
                {
                    list = (from o in list
                            where o.ID1 == id1
                            select o).ToList();
                }
                if (!String.IsNullOrEmpty(id2))
                {
                    list = (from o in list
                            where o.ID2 == id2
                            select o).ToList();
                }
                if (!String.IsNullOrEmpty(id3))
                {
                    list = (from o in list
                            where o.ID3 == id3
                            select o).ToList();
                }
                if (!String.IsNullOrEmpty(id4))
                {
                    list = (from o in list
                            where o.ID4 == id4
                            select o).ToList();
                }
                if (!String.IsNullOrEmpty(id5))
                {
                    list = (from o in list
                            where o.ID5 == id5
                            select o).ToList();
                }

                string strs = "";
                foreach (var item in list)
                {
                    strs += item.StaffID + ",";
                }
                strs = strs.Substring(0, strs.Length - 1);
                staffid = strs.Split(',');
            }

            return staffid;
        }

        private bool NoNullDep(string id1, string id2, string id3, string id4, string id5)
        {
            bool flag = true;
            if (!String.IsNullOrEmpty(id5) && (String.IsNullOrEmpty(id4) || String.IsNullOrEmpty(id3) || String.IsNullOrEmpty(id2) || String.IsNullOrEmpty(id1)))
            {
                flag = false;
            }
            if (!String.IsNullOrEmpty(id4) && (String.IsNullOrEmpty(id3) || String.IsNullOrEmpty(id2) || String.IsNullOrEmpty(id1)))
            {
                flag = false;
            }
            if (!String.IsNullOrEmpty(id3) && (String.IsNullOrEmpty(id2) || String.IsNullOrEmpty(id1)))
            {
                flag = false;
            }
            if (!String.IsNullOrEmpty(id2) && String.IsNullOrEmpty(id1))
            {
                flag = false;
            }
            return flag;
        }

        public ActionResult AddStaffToAnotherList(string schlistid)
        {
            Window win = new Window
            {

                ID = "windowAnother",
                Title = "迁移员工",
                Height = 450,
                Width = 700,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AnotherList", "Scheduling", new { id = schlistid }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.SchStaffReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();


        }

        public ActionResult AnotherList(string id)
        {
            var x = X.GetCmp<TextField>("FromID");
            x.Text = id;
            return View();
        }

        public ActionResult GetSchList(string fromid)
        {
            var list = (from o in entities.T_HR_SchList
                        where o.ID != fromid
                        select new { o.ID, o.Name }).ToList();
            return this.Store(list);
        }

        [DirectMethod]
        public ActionResult SetSchlistTextBox(string schlist)
        {
            var s = from o in entities.T_HR_SchList
                    where o.ID == schlist
                    select o;
            SchListModel slm = new SchListModel();
            if (s.Any())
            {
                T_HR_SchList item = s.First();
                slm.ID = item.ID;
                slm.Name = item.Name;
                slm.AMTime = item.AMClassGoOn.ToString().Substring(0, 5) + "-" + item.AMClassOff.ToString().Substring(0, 5);
                slm.AMValidGoTime = item.ACGST.ToString().Substring(0, 5) + "-" + item.ACGET.ToString().Substring(0, 5);
                slm.AMValidOffTime = item.ACOST.ToString().Substring(0, 5) + "-" + item.ACOET.ToString().Substring(0, 5);
                slm.PMTime = item.PMClassGoOn.ToString().Substring(0, 5) + "-" + item.PMClassOff.ToString().Substring(0, 5);
                slm.PMValidGoTime = item.PCGST.ToString().Substring(0, 5) + "-" + item.PCGET.ToString().Substring(0, 5);
                slm.PMValidOffTime = item.PCOST.ToString().Substring(0, 5) + "-" + item.PCOET.ToString().Substring(0, 5);
                slm.Two = Convert.ToBoolean(item.Two);
                slm.Four = Convert.ToBoolean(item.Four);
                if (slm.Two)
                {
                    slm.AttNum = "2次卡";
                }
                if (slm.Four)
                {
                    slm.AttNum = "4次卡";
                }
                if (item.CreateTime != null)
                {
                    slm.CreateTime = Convert.ToDateTime(item.CreateTime);
                }
                slm.CreaterName = item.CreaterName;
                if (item.EditeTime != null)
                {
                    slm.EditeTime = Convert.ToDateTime(item.EditeTime);
                }
                slm.EditorName = item.EditorName;
                var textid = X.GetCmp<TextField>("ID");
                textid.Text = slm.ID;
                var textName = X.GetCmp<TextField>("Name");
                textName.Text = slm.Name;
                var textAMTime = X.GetCmp<TextField>("AMTime");
                textAMTime.Text = slm.AMTime;
                var textAMValidGoTime = X.GetCmp<TextField>("AMValidGoTime");
                textAMValidGoTime.Text = slm.AMValidGoTime;
                var textAMValidOffTime = X.GetCmp<TextField>("AMValidOffTime");
                textAMValidOffTime.Text = slm.AMValidOffTime;
                var textCreaterName = X.GetCmp<TextField>("CreaterName");
                textCreaterName.Text = slm.CreaterName;
                var textEditorName = X.GetCmp<TextField>("EditorName");
                textEditorName.Text = slm.EditorName;
                var textPMTime = X.GetCmp<TextField>("PMTime");
                textPMTime.Text = slm.PMTime;
                var textPMValidGoTime = X.GetCmp<TextField>("PMValidGoTime");
                textPMValidGoTime.Text = slm.PMValidGoTime;
                var textPMValidOffTime = X.GetCmp<TextField>("PMValidOffTime");
                textPMValidOffTime.Text = slm.PMValidOffTime;
                var textAttNum = X.GetCmp<TextField>("AttNum");
                textAttNum.Text = slm.AttNum;
                if (item.CreateTime != null)
                {
                    var textCreateTime = X.GetCmp<TextField>("CreateTime");
                    textCreateTime.Text = slm.CreateTime.ToString();
                }
                if (item.EditeTime != null)
                {
                    var textEditeTime = X.GetCmp<TextField>("EditeTime");
                    textEditeTime.Text = slm.EditeTime.ToString();
                }

            }
            return this.Direct();
        }

        public ActionResult AddFromAnother(string id, string fromid)
        {
            DirectResult r = new DirectResult();

            var fromlist = from o in entities.T_HR_SchListWithStaff
                           where o.SchListID == fromid
                           select o;
            if (fromlist.Any())
            {
                foreach (var item in fromlist)
                {
                    T_HR_SchListWithStaff slws = entities.T_HR_SchListWithStaff.Find(item.ID);
                    slws.SchListID = id;
                }
            }

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
            return r;

        }


    }
}
