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
    public class BusinessTripController : Controller
    {
        //
        // GET: /person/BusinessTrip/

        Entities entities = new Entities();

        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_BTWithDepName
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
            var list = new List<V_HR_BTWithDepName>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    //string[] sdep = Tool.DepStr(dep);
                    //var staff = from o in entities.T_HR_Staff
                    //            where sdep.Contains(o.SDepartMentId)
                    //            select o.StaffID;
                    string[] staff = Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_BTWithDepName
                            where staff.Contains(o.StaffID) && o.Valid == true
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_BTWithDepName
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

        private List<V_HR_BTWithDepName> SearchData(string staffid, string name)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_BTWithDepName>();

            list = (from o in entities.V_HR_BTWithDepName
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
        public ActionResult BusinessTripReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_BTWithDepName
                            where o.Valid == true
                            select o).ToList();

                var store = X.GetCmp<Store>("BusinessTripStore");
                store.LoadData(list);
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        [VisitAuthorize(Create=true)]
        public ActionResult Add()//添加响应
        {
            Window win = new Window
            {

                ID = "windowBusinessTrip",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddBusinessTrip", "BusinessTrip", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.BusinessTripReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id, string opid)//修改相应
        {
            if (BTApply.GetState(opid) == CheckState.Rejected)
            {
                Window win = new Window
                {

                    ID = "windowBusinessTrip",
                    Title = "修改",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("AddBusinessTrip", "BusinessTrip", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.BusinessTripReload()",
                        }
                    }
                };

                win.Render(RenderMode.Auto);
            }
            else
            {
                X.Msg.Alert("警告", "不可修改！").Show();
            }

            return this.Direct();


        }

        public ActionResult AddBusinessTrip(string id)//在修改时传递的为contractid
        {
            if (id == "-1")//-1为添加
            {
                return View(new BTApply());
            }
            else//否则为修改
            {
                V_HR_BTWithDepName bt = entities.V_HR_BTWithDepName.Find(id);
                if (bt == null) return HttpNotFound();
                BTApply btt = new BTApply();
                btt.ID = bt.ID;
                btt.StaffID = bt.StaffID;
                btt.Name = bt.Name;
                btt.Department = bt.Department;
                btt.BTPlace = bt.BTPlace;
                btt.StartTime = Convert.ToDateTime(bt.StartTime);
                btt.EndTime = Convert.ToDateTime(bt.EndTime);
                btt.StartTimeStr = btt.StartTime.ToString("yyyy-MM-dd HH:mm");
                btt.EndTimeStr = btt.EndTime.ToString("yyyy-MM-dd HH:mm");
                btt.TimeSpan = bt.TimeSpan;
                btt.BTReason = bt.BTReason;
                btt.Valid = Convert.ToBoolean(bt.Valid);
                btt.Remark = bt.Remark;
                btt.CreaterName = bt.CreaterName;
                btt.CreateTime = Convert.ToDateTime(bt.CreateTime);
                btt.EditorName = bt.EditorName;
                btt.EditeTime = Convert.ToDateTime(bt.EditeTime);
                btt.OperationListID = bt.OperationListID;
                if (btt.OperationListID != null)
                    btt.CheckFlowId = btt.GetCheckFlowId;
                btt.LastID = bt.LastID;
                btt.BTEdit = Convert.ToBoolean(bt.BTEdit);
                btt.BTDelete = Convert.ToBoolean(bt.BTDelete);
                btt.EditOrDelete = bt.EditOrDelete;


                return View(btt);
            }
        }

        public ActionResult CheckFlowItems()
        {
            var list = (from o in entities.V_CH_Checkfuncflow
                        where o.CheckfuncName == "出差申请"
                        select new { o.ID, o.Name }).ToList();
            return this.Store(list);
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

        public ActionResult AddOrEditBusinessTrip(BTApply businesstrip)
        {
            try
            {
                businesstrip.StartTime = Convert.ToDateTime(businesstrip.StartTimeStr);//计算TimeSpan
                businesstrip.EndTime = Convert.ToDateTime(businesstrip.EndTimeStr);
            }
            catch(FormatException fe)
            {
                X.Msg.Alert("警告", "日期不正确，请检查！").Show();
                return this.Direct();
            }
            
            if (businesstrip.StartTime.Year == businesstrip.EndTime.Year && businesstrip.StartTime.Month == businesstrip.EndTime.Month && businesstrip.StartTime <= businesstrip.EndTime)
            {
                DirectResult r = new DirectResult();
                T_HR_BusinessTrip businesstripupdate = entities.T_HR_BusinessTrip.Find(businesstrip.ID);

                if (businesstripupdate == null)//为空为添加
                {
                    //businesstrip.StartTime = Convert.ToDateTime(businesstrip.StartTimeStr);//计算TimeSpan
                    //businesstrip.EndTime = Convert.ToDateTime(businesstrip.EndTimeStr);
                    businesstrip.ID = Guid.NewGuid().ToString();
                    businesstrip.OperationListID = Guid.NewGuid().ToString();
                    businesstrip.Valid = true;
                    businesstrip.CreaterName = new LoginUser().EmployeeId;
                    businesstrip.CreateTime = DateTime.Now;
                    businesstrip.BTEdit = false;
                    businesstrip.BTDelete = false;
                    //businesstrip.TimeSpan = GetTimeSpan(businesstrip.StartTime, businesstrip.EndTime);

                    T_CH_Operation_list newList = new T_CH_Operation_list();
                    newList.ID = businesstrip.OperationListID;
                    newList.State = (int)CheckState.Checking;//审核中
                    newList.Check_flowID = businesstrip.CheckFlowId;
                    newList.Check_funcID = businesstrip.FuncId;
                    newList.CreateTime = DateTime.Now;
                    newList.Creator = new LoginUser().EmployeeId;
                    newList.Url = Url.Action("CheckBusinessTrip", "BusinessTrip", new { id = businesstrip.ID });

                    entities.T_HR_BusinessTrip.Add(businesstrip.ToDB(1));
                    entities.T_CH_Operation_list.Add(newList);
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
                    //businesstrip.StartTime = Convert.ToDateTime(businesstrip.StartTimeStr);//计算TimeSpan
                    //businesstrip.EndTime = Convert.ToDateTime(businesstrip.EndTimeStr);
                    businesstrip.EditorName = new LoginUser().EmployeeId;
                    businesstrip.EditeTime = DateTime.Now;

                    businesstripupdate.Valid = false;

                    businesstrip.ID = Guid.NewGuid().ToString();
                    businesstrip.OperationListID = Guid.NewGuid().ToString();
                    businesstrip.Valid = true;
                    businesstrip.BTEdit = false;
                    businesstrip.BTDelete = false;
                    //businesstrip.TimeSpan = GetTimeSpan(businesstrip.StartTime, businesstrip.EndTime);

                    T_CH_Operation_list newList = new T_CH_Operation_list();
                    newList.ID = businesstrip.OperationListID;
                    newList.State = (int)CheckState.Checking;//审核中
                    newList.Check_flowID = businesstrip.CheckFlowId;
                    newList.Check_funcID = businesstrip.FuncId;
                    newList.CreateTime = DateTime.Now;
                    newList.Creator = new LoginUser().EmployeeId;
                    newList.Url = Url.Action("CheckBusinessTrip", "BusinessTrip", new { id = businesstrip.ID });

                    entities.T_HR_BusinessTrip.Add(businesstrip.ToDB(2));
                    entities.T_CH_Operation_list.Add(newList);
                    try
                    {
                        entities.SaveChanges();
                        r.Success = true;
                        X.Msg.Alert("提示", "修改成功！", new JFunction { Fn = "closewindow" }).Show();
                    }
                    catch (Exception e)
                    {
                        X.Msg.Alert("警告", "数据保存失败！<br /> note:" + e.Message, new JFunction { Fn = "closewindow" }).Show();
                        r.Success = false;
                    }
                }
                return r;
            }
            else
            {
                X.Msg.Alert("警告", "出差申请不可跨月！").Show();
                return this.Direct();
            }

        }

        public decimal GetTH(DateTime st, DateTime et)
        {
            decimal th=0;

            return th;
        }//如何计算出差占用多少休息日,不算了，放到界面上让用户自己填，待做

        public string GetTimeSpan(DateTime st, DateTime et)
        {
            DateTime dt = Convert.ToDateTime(st.Year.ToString() + "-" + st.Month.ToString() + "-01");
            var wt = (from o in entities.T_HR_WorkTime
                      where o.Month == dt
                      select o).ToList();
            double noonbreak = 0;
            bool flag;//是否跨中午休息时间
            if (wt.Any())
            {
                int ah, am, ph, pm;
                ah = Convert.ToInt32(wt.First().AMClassOff.Substring(0, 2));
                am = Convert.ToInt32(wt.First().AMClassOff.Substring(3, 2));
                ph = Convert.ToInt32(wt.First().PMClassGoOn.Substring(0, 2));
                pm = Convert.ToInt32(wt.First().PMClassGoOn.Substring(3, 2));
                noonbreak = ph - ah + (pm - ph) / 60;

                int sth, stm, eth, etm;
                sth = st.Hour;
                stm = st.Minute;
                eth = et.Hour;
                etm = et.Minute;

                if ((sth <= ah && stm <= am && eth >= ph && etm >= pm) || (eth <= ah && etm <= am && sth >= ph && stm >= pm))//是否跨休息时间
                    flag = true;
                else
                    flag = false;
            }
            else
            {
                noonbreak = 1.5;
                flag = true;
            }

            double timespan = 0;
            int dayspan = st.Day - et.Day;
            DateTime st1 = st.AddDays(dayspan);
            TimeSpan ts = et.Subtract(st);
            double hourstemp = ts.TotalHours;

            if (flag)
            {
                if (hourstemp > 0)
                {
                    timespan = 8 * dayspan + hourstemp - noonbreak;
                }
                if (hourstemp < 0)
                {
                    timespan = 8 * dayspan + hourstemp + noonbreak;
                }
                if (hourstemp == 0)
                {
                    timespan = 8 * dayspan + hourstemp;
                }
            }
            else
            {
                timespan = 8 * dayspan + hourstemp;
            }

            timespan = Math.Round(timespan, 2);

            return timespan.ToString();
        }

        [VisitAuthorize(Update = true)]
        public ActionResult EditBusinessTrip(string id, string opid)
        {
            if (BTApply.GetState(opid) == CheckState.Approved && GetPreState(id))
            {
                Window win = new Window
                {

                    ID = "windowBusinessTrip",
                    Title = "销假",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("BusinessTripEdit", "BusinessTrip", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.BusinessTripReload()",
                        }
                    }
                };

                win.Render(RenderMode.Auto);
            }
            else
            {
                X.Msg.Alert("警告", "不可修改！").Show();
            }

            return this.Direct();


        }

        public bool GetPreState(string id)
        {
            T_HR_BusinessTrip last = entities.T_HR_BusinessTrip.Find(id);
            bool flag = true;
            string opid;

            if (Convert.ToBoolean(last.BTEdit))
            {
                var pre = (from o in entities.T_HR_BusinessTrip
                           where o.LastID == id && o.BTEdit == true
                           select o).ToList();
                if (pre.Any())
                {
                    opid = pre.First().OperationListID;
                    var list = from o in entities.T_CH_Operation_list
                               where o.ID == opid
                               select o;
                    if (list.Any())
                    {
                        if (list.First().State == 1)
                            flag = false;
                    }
                }
            }

            if (Convert.ToBoolean(last.BTDelete))
            {
                var pre = (from o in entities.T_HR_BusinessTrip
                           where o.LastID == id && o.BTDelete == true
                           select o).ToList();
                if (pre.Any())
                {
                    opid = pre.First().OperationListID;
                    var list = from o in entities.T_CH_Operation_list
                               where o.ID == opid
                               select o;
                    if (list.Any())
                    {
                        if (list.First().State == 1)
                            flag = false;
                    }
                }
            }

            return flag;

        }

        [VisitAuthorize(Update = true)]
        public ActionResult DeleteBusinessTrip(string id, string opid)
        {
            if (BTApply.GetState(opid) == CheckState.Approved && GetPreState(id))
            {
                Window win = new Window
                {

                    ID = "windowBusinessTrip",
                    Title = "销差",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("BusinessTripRemove", "BusinessTrip", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.BusinessTripReload()",
                        }
                    }
                };

                win.Render(RenderMode.Auto);
            }
            else
            {
                X.Msg.Alert("警告", "不可修改！").Show();
            }

            return this.Direct();


        }

        public ActionResult BusinessTripEdit(string id)
        {
            V_HR_BTWithDepName bt = entities.V_HR_BTWithDepName.Find(id);
            if (bt != null)
            {
                BTApply btt = new BTApply();
                btt.ID = bt.ID;
                btt.StaffID = bt.StaffID;
                btt.Name = bt.Name;
                btt.Department = bt.Department;
                btt.BTPlace = bt.BTPlace;
                btt.StartTime = Convert.ToDateTime(bt.StartTime);
                btt.EndTime = Convert.ToDateTime(bt.EndTime);
                btt.StartTimeStr = btt.StartTime.ToString("yyyy-MM-dd HH:mm");
                btt.EndTimeStr = btt.EndTime.ToString("yyyy-MM-dd HH:mm");
                btt.TimeSpan = bt.TimeSpan;
                btt.BTReason = bt.BTReason;
                btt.Valid = Convert.ToBoolean(bt.Valid);
                btt.Remark = bt.Remark;
                btt.CreaterName = bt.CreaterName;
                btt.CreateTime = Convert.ToDateTime(bt.CreateTime);
                btt.EditorName = bt.EditorName;
                btt.EditeTime = Convert.ToDateTime(bt.EditeTime);
                btt.OperationListID = bt.OperationListID;
                //if (btt.OperationListID != null)
                //    btt.CheckFlowId = btt.GetCheckFlowId;
                btt.LastID = bt.LastID;
                btt.BTEdit = Convert.ToBoolean(bt.BTEdit);
                btt.BTDelete = Convert.ToBoolean(bt.BTDelete);
                btt.EditOrDelete = bt.EditOrDelete;

                return View(btt);
            }
            else
            {
                return HttpNotFound();
            }



        }

        public ActionResult BusinessTripRemove(string id)
        {
            V_HR_BTWithDepName bt = entities.V_HR_BTWithDepName.Find(id);
            if (bt != null)
            {
                BTApply btt = new BTApply();
                btt.ID = bt.ID;
                btt.StaffID = bt.StaffID;
                btt.Name = bt.Name;
                btt.Department = bt.Department;
                btt.BTPlace = bt.BTPlace;
                btt.StartTime = Convert.ToDateTime(bt.StartTime);
                btt.EndTime = Convert.ToDateTime(bt.EndTime);
                btt.StartTimeStr = btt.StartTime.ToString("yyyy-MM-dd HH:mm");
                btt.EndTimeStr = btt.EndTime.ToString("yyyy-MM-dd HH:mm");
                btt.TimeSpan = bt.TimeSpan;
                btt.BTReason = bt.BTReason;
                btt.Valid = Convert.ToBoolean(bt.Valid);
                btt.Remark = bt.Remark;
                btt.CreaterName = bt.CreaterName;
                btt.CreateTime = Convert.ToDateTime(bt.CreateTime);
                btt.EditorName = bt.EditorName;
                btt.EditeTime = Convert.ToDateTime(bt.EditeTime);
                btt.OperationListID = bt.OperationListID;
                //if (btt.OperationListID != null)
                //    btt.CheckFlowId = btt.GetCheckFlowId;
                btt.LastID = bt.LastID;
                btt.BTEdit = Convert.ToBoolean(bt.BTEdit);
                btt.BTDelete = Convert.ToBoolean(bt.BTDelete);
                btt.EditOrDelete = bt.EditOrDelete;

                return View(btt);
            }
            else
            {
                return HttpNotFound();
            }
        }

        public ActionResult BESubmit(BTApply businesstrip)
        {
            try
            {
                businesstrip.StartTime = Convert.ToDateTime(businesstrip.StartTimeStr);//计算TimeSpan
                businesstrip.EndTime = Convert.ToDateTime(businesstrip.EndTimeStr);
            }
            catch (FormatException fe)
            {
                X.Msg.Alert("警告", "日期不正确，请检查！").Show();
                return this.Direct();
            }

            if (businesstrip.StartTime.Year == businesstrip.EndTime.Year && businesstrip.StartTime.Month == businesstrip.EndTime.Month && businesstrip.StartTime <= businesstrip.EndTime)
            {
                DirectResult r = new DirectResult();
                string orid = businesstrip.ID;

                var prelists = from o in entities.T_HR_BusinessTrip
                               where o.BTEdit == true && o.LastID == orid
                               select o;
                foreach (T_HR_BusinessTrip item in prelists)
                {
                    T_HR_BusinessTrip prelist = entities.T_HR_BusinessTrip.Find(item.ID);
                    prelist.BTEdit = false;
                }

                T_HR_BusinessTrip orlist = entities.T_HR_BusinessTrip.Find(orid);
                orlist.BTEdit = true;
                orlist.EditOrDelete = "Edit";

                businesstrip.ID = Guid.NewGuid().ToString();
                businesstrip.OperationListID = Guid.NewGuid().ToString();
                businesstrip.Valid = false;
                businesstrip.CreaterName = new LoginUser().EmployeeId;
                businesstrip.CreateTime = DateTime.Now;
                businesstrip.BTEdit = true;
                businesstrip.BTDelete = false;
                businesstrip.EditOrDelete = "";
                businesstrip.LastID = orid;
                //businesstrip.TimeSpan = GetTimeSpan(businesstrip.StartTime, businesstrip.EndTime);
                //businesstrip.TiaoxiuHours = GetTH(businesstrip.StartTime, businesstrip.EndTime);

                T_CH_Operation_list newList = new T_CH_Operation_list();
                newList.ID = businesstrip.OperationListID;
                newList.State = (int)CheckState.Checking;//审核中
                newList.Check_flowID = businesstrip.CheckFlowId;
                newList.Check_funcID = businesstrip.FuncId;
                newList.CreateTime = DateTime.Now;
                newList.Creator = new LoginUser().EmployeeId;
                newList.Url = Url.Action("CheckBusinessTrip", "BusinessTrip", new { id = businesstrip.ID });

                entities.T_HR_BusinessTrip.Add(businesstrip.ToDB(1));
                entities.T_CH_Operation_list.Add(newList);
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
            else
            {
                X.Msg.Alert("警告", "请假不可跨月！").Show();
                return this.Direct();
            }
        }

        public ActionResult BRSubmit(BTApply businesstrip)
        {
            DirectResult r = new DirectResult();
            string orid = businesstrip.ID;

            var prelists = from o in entities.T_HR_BusinessTrip
                           where o.BTDelete == true && o.LastID == orid
                           select o;
            foreach (T_HR_BusinessTrip item in prelists)
            {
                T_HR_BusinessTrip prelist = entities.T_HR_BusinessTrip.Find(item.ID);
                prelist.BTDelete = false;
            }

            T_HR_BusinessTrip orlist = entities.T_HR_BusinessTrip.Find(orid);
            orlist.BTDelete = true;
            orlist.EditOrDelete = "Delete";

            businesstrip.ID = Guid.NewGuid().ToString();
            businesstrip.OperationListID = Guid.NewGuid().ToString();
            businesstrip.Valid = false;
            businesstrip.CreaterName = new LoginUser().EmployeeId;
            businesstrip.CreateTime = DateTime.Now;
            businesstrip.BTEdit = false;
            businesstrip.BTDelete = true;
            businesstrip.EditOrDelete = "";
            businesstrip.LastID = orid;

            T_CH_Operation_list newList = new T_CH_Operation_list();
            newList.ID = businesstrip.OperationListID;
            newList.State = (int)CheckState.Checking;//审核中
            newList.Check_flowID = businesstrip.CheckFlowId;
            newList.Check_funcID = businesstrip.FuncId;
            newList.CreateTime = DateTime.Now;
            newList.Creator = new LoginUser().EmployeeId;
            newList.Url = Url.Action("CheckBusinessTrip", "BusinessTrip", new { id = businesstrip.ID });

            entities.T_HR_BusinessTrip.Add(businesstrip.ToDB(1));
            entities.T_CH_Operation_list.Add(newList);
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

        [VisitAuthorize(Read = true)]
        public ActionResult Detail(string id)
        {
            Window win = new Window
            {

                ID = "windowBusinessTrip",
                Title = "详细信息",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("BusinessTripDetail", "BusinessTrip", new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        //Handler = "toggleRowSelect()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult BusinessTripDetail(string id)
        {
            BTApply oritem = new BTApply();
            if (null != oritem.GetBusinessTripDetail(id))
            {
                if (oritem.BTEdit || oritem.BTDelete)
                {
                    if (oritem.EditOrDelete == "Edit")
                    {
                        var prelist = from o in entities.T_HR_BusinessTrip
                                      where o.LastID == oritem.ID && o.BTEdit == true
                                      select o;

                        if (prelist.Any())
                        {
                            //oritem.preitem = oritem.GetVacationDetail(prelist.First().ID);
                            oritem.GetPreBusinessTripDetail(prelist.First().ID);
                        }

                        //调整View
                    }
                    else if (oritem.EditOrDelete == "Delete")
                    {
                        var prelist = from o in entities.T_HR_BusinessTrip
                                      where o.LastID == oritem.ID && o.BTDelete == true
                                      select o;

                        if (prelist.Any())
                        {
                            //oritem.preitem = oritem.GetVacationDetail(prelist.First().ID);
                            oritem.GetPreBusinessTripDetail(prelist.First().ID);
                        }

                        //调整View
                        var x = X.GetCmp<Panel>("PrePanel");
                        x.Hidden = true;
                    }
                    else
                    {
                        //调整View
                        var x = X.GetCmp<FieldSet>("PreList");
                        x.Hidden = true;
                    }
                }
                else
                {
                    //调整View
                    var x = X.GetCmp<FieldSet>("PreList");
                    x.Hidden = true;
                }
                return View(oritem);
            }
            else
            {
                return View();
            }
        }

        public ActionResult CheckStateItems()
        {
            //int[] id = new int[] { 2, 3, 4 };
            var list = (from o in entities.T_CH_Check_state
                        //where id.Contains(o.ID)
                        select new { o.ID, o.Description }).ToList();
            return this.Store(list);
        }

        public ActionResult Check(string id)//审核相应
        {
            //if (DimissionApply.GetExpire(id))
            //{
            Window win = new Window
            {

                ID = "windowBusinessTrip",
                Title = "审核",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("CheckBusinessTrip", "BusinessTrip", new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.BusinessTripReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            //}
            //else
            //{
            //    X.Msg.Alert("警告", "该审核任务已过期或不存在！").Show();
            //}

            return this.Direct();


        }

        public ActionResult CheckBusinessTrip(string id)
        {
            BTApply preitem = new BTApply();
            if (null != preitem.GetPreBusinessTripDetail(id))
            {
                if (preitem.PreBTEdit || preitem.PreBTDelete)
                {
                    var orelist = (from o in entities.T_HR_BusinessTrip
                                   where o.ID == preitem.PreLastID
                                   select o).First();
                    preitem.GetBusinessTripDetail(orelist.ID);
                    if (orelist.EditOrDelete == "Edit")
                    {
                        //调整View
                    }
                    else if (orelist.EditOrDelete == "Delete")
                    {
                        //调整View
                        var x1 = X.GetCmp<FieldSet>("PreList");
                        x1.Title = "出差取消";
                        var x = X.GetCmp<Panel>("PrePanel");
                        x.Hidden = true;
                    }
                }
                else
                {
                    //调整View
                    var x = X.GetCmp<FieldSet>("OreList");
                    x.Hidden = true;
                    var x1 = X.GetCmp<FieldSet>("PreList");
                    x1.Title = "出差申请";
                }
                return View(preitem);
            }
            else
            {
                return View();
            }


        }

        public ActionResult CheckSubmit(BTApply bt)
        {
            try
            {
                bool flag = bt.SubmitCheckBusinessTrip();
                bool newwindow = false;
                if (Session["NewWindow"] != null)
                    newwindow = Convert.ToBoolean(Session["NewWindow"]);
                X.Msg.Alert("页面消息", flag ? "审核操作成功！" : "审核操作失败！", flag ? (newwindow ? "parent.App.win.close();" : "history.go(-1);") : null).Show();
                if (flag)
                {
                    X.AddScript("parent.App.storedata.reload();");
                }
                return this.Direct();
            }
            catch (Exception e)
            {
                return this.Direct(false, e.Message);
            }
        }




    }
}
