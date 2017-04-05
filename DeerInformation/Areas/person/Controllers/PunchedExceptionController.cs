using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Areas.person.Models;
using DeerInformation.BaseType;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.person.Controllers
{
    [DirectController(AreaName = "person")]
    public class PunchedExceptionController : Controller
    {
        //
        // GET: /person/PunchedException/
        Entities entities = new Entities();
        public ActionResult Index()
        {
            return View(new PunchedExceptionModle());
        }

        public ActionResult Fiter(StoreRequestParameters parameters, string date, string state, string solved, string name = "")
        {

            var exception = new PunchedExceptionModle() { Name = name, Date = null, State = PunchedException.Exception, Solved = false };
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
            {
                exception.Date = dateTime;
            }
            int attendState;
            if (int.TryParse(state, out attendState))
            {
                exception.State = (PunchedException)attendState;
            }
            bool attendSolved;
            if (bool.TryParse(solved, out attendSolved))
            {
                exception.Solved = attendSolved;
            }

            return this.Store(exception.Exceptions.GetPage(parameters));
        }

        public ActionResult Deal(string id, string userid)
        {
            if (id == "null" || userid == "null")
            {
                X.Msg.Alert("页面消息", "未选择任何行").Show();
            }
            else
            {
                WindowModule window = new WindowModule
                {
                    Loader = { Url = Url.Action("Attend", new { id = id, userid = userid }) }
                };
                window.Render(RenderMode.Auto);
            }
            return this.Direct();
        }

        public ActionResult Attend(string id, string userid)
        {
            var exception = new PunchedExceptionModle() { Id = id, EmployeeId = userid };
            if (exception.AttendValidRecords != null)
            {
                exception.Date = exception.AttendValidRecords.AttendTime;
                return View(exception);
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult Submit(PunchedExceptionModle form, string id)
        {
            form.Id = id;
            if (form.DealException())
            {
                X.Msg.Alert("页面消息", "操作成功！", "parent.App.win.close();").Show();
            }
            else
            {
                X.Msg.Alert("页面消息", "操作失败！").Show();
            }
            X.AddScript("parent.App.storedata.reload();");
            return this.Direct();
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
                    Url = Url.Action("SelectStaff", "SelectStaff", new { area = "", NUM = 999999 }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.DealGetpersonWithAll()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        [DirectMethod]
        public ActionResult DealGetpersonWithAll()
        {
            if (TempData["SelectedStaffList"] != null)
            {
                List<V_HR_IDNameWithDepName> s = (List<V_HR_IDNameWithDepName>)TempData["SelectedStaffList"];
                if (s.Count > 0)
                {
                    string[] staffid;
                    string staffids = "";

                    if (Session["xgw_attendstaffid"] != null)
                    {
                        staffid = (string[])Session["xgw_attendstaffid"];
                        foreach (string item in staffid)
                        {
                            staffids += item + ",";
                        }
                    }
                    foreach (var item in s)
                    {
                        staffids += item.StaffID + ",";
                    }
                    staffids = staffids.Substring(0, staffids.Length - 1);
                    staffid = staffids.Split(',');

                    //Session["xgw_attendstaffid"] = staffid;
                    var list = from o in entities.V_HR_IDNameWithDepName
                               where staffid.Contains(o.StaffID)
                               select o;

                    staffids = "";
                    foreach (var item in list)
                    {
                        staffids += item.StaffID + ",";
                    }
                    staffids = staffids.Substring(0, staffids.Length - 1);
                    staffid = staffids.Split(',');
                    Session["xgw_attendstaffid"] = staffid;

                    var store = X.GetCmp<Store>("StaffStore");
                    store.LoadData(list);
                }
            }

            return this.Direct();
        }

        [VisitAuthorize(Create = true)]
        public ActionResult Add()//添加响应
        {
            Window win = new Window
            {

                ID = "windowAttend",
                Title = "补签",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddAttend", "PunchedException"),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddAttend(string date)
        {
            var list = from o in entities.V_HR_IDNameWithDepName
                       where o.StaffID == ""
                       select o;
            string[] s = null;
            Session["xgw_attendstaffid"] = s;
            return View(list);
        }

        public ActionResult JSDeleteAttendStaff(SubmitHandler handler)//删除响应
        {
            string id, staffids = "";
            string[] staffid = null, destaffid;

            if (Session["xgw_attendstaffid"] != null)
            {
                staffid = (string[])Session["xgw_attendstaffid"];
            }

            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            foreach (Dictionary<string, string> row in values)
            {
                id = row["StaffID"];
                staffids += id + ",";
            }
            staffids = staffids.Substring(0, staffids.Length - 1);
            destaffid = staffids.Split(',');

            var list = from o in entities.V_HR_IDNameWithDepName
                       where staffid.Contains(o.StaffID)
                       select o;

            var lalist = from o in list
                         where !destaffid.Contains(o.StaffID)
                         select o;

            staffids = "";
            foreach (var item in lalist)
            {
                staffids += item.StaffID + ",";
            }
            if (staffids.Length != 0)
            {
                staffids = staffids.Substring(0, staffids.Length - 1);
            }
            staffid = staffids.Split(',');
            Session["xgw_attendstaffid"] = staffid;

            var store = X.GetCmp<Store>("StaffStore");
            store.LoadData(lalist);

            return this.Direct();
        }

        public ActionResult Test()
        {
            string staffids = "";
            string[] staffid;
            if (Session["xgw_attendstaffid"] != null)
            {
                staffid = (string[])Session["xgw_attendstaffid"];
                foreach (string item in staffid)
                {
                    staffids += item + ",";
                }
                staffids = staffids.Substring(0, staffids.Length - 1);
            }
            X.Msg.Alert("提示", staffids).Show();
            return this.Direct();
        }

        public ActionResult SaveAttend(string date, string order, string isdep, string id1, string id2, string id3, string id4, string id5)
        {
            if(id1=="null")
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

            bool dep = Convert.ToBoolean(isdep);

            DirectResult r = new DirectResult();

            LoginUser lu = new LoginUser();

            DateTime dt = Convert.ToDateTime(date);

            int or=Convert.ToInt32(order);

            string[] staffid = null;

            if(dep)
            {
                if(NoNullDep(id1,id2,id3,id4,id5))
                {
                    staffid = GetStaffIDFromDep(id1, id2, id3, id4, id5);
                }
                else
                {
                    X.Msg.Alert("警告", "部门不可跳过为空！").Show();
                    return this.Direct();
                }
            }
            else
            {
                if (Session["xgw_attendstaffid"] != null)
                {
                    staffid = (string[])Session["xgw_attendstaffid"];
                }
            }    

            #region
            if (staffid != null)
            {
                foreach (string id in staffid)
                {
                    bool four=false;
                    var schstaff = from o in entities.T_HR_SchListWithStaff
                                   where o.StaffID == id
                                   select o;
                    if(schstaff.Any())
                    {
                        string schlistid = schstaff.First().SchListID;
                        var sch = from o in entities.T_HR_SchList
                                  where o.ID == schlistid
                                  select o;
                        if(sch.Any())
                        {
                            four = Convert.ToBoolean(sch.First().Four);
                        }
                    }

                    if(!four && or>2)
                    {
                        continue;
                    }

                    var ats=from o in entities.T_HR_AttendInsert
                            where o.UserId==id && o.PunchedOrder==or
                            select o;
                    if(ats.Any())
                    {
                        T_HR_AttendInsert atup = entities.T_HR_AttendInsert.Find(ats.First().ID);

                        T_HR_AttendanceExceptionHandleRecords aehrup = new T_HR_AttendanceExceptionHandleRecords();
                        aehrup.EmployeeID = id;
                        aehrup.Date = dt.Date;
                        aehrup.BeforeTime = atup.AttendTime;
                        aehrup.FinalTime = dt;
                        aehrup.ExceptionState = 6;
                        aehrup.PunchedOrder = or;
                        aehrup.Editor = new LoginUser().EmployeeId;
                        aehrup.UpdateTime = DateTime.Now;
                        aehrup.Remark = "补签";
                        entities.T_HR_AttendanceExceptionHandleRecords.Add(aehrup);
                        
                        atup.AttendTime = dt;
                        atup.EditUserId = new LoginUser().EmployeeId;
                        atup.EditTime = DateTime.Now;
                        atup.State = 1;
                        atup.Solved = false;
                        atup.PunchedOrder = or;

                        
                    }
                    else
                    {
                        T_HR_AttendanceExceptionHandleRecords aehradd = new T_HR_AttendanceExceptionHandleRecords();
                        aehradd.EmployeeID = id;
                        aehradd.Date = dt.Date;
                        aehradd.FinalTime = dt;
                        aehradd.ExceptionState = 6;
                        aehradd.PunchedOrder = or;
                        aehradd.Editor = new LoginUser().EmployeeId;
                        aehradd.UpdateTime = DateTime.Now;
                        aehradd.Remark = "补签";
                        entities.T_HR_AttendanceExceptionHandleRecords.Add(aehradd);

                        T_HR_AttendInsert at = new T_HR_AttendInsert();
                        at.ID = Tool.ProduceSed64();
                        at.UserId = id;
                        at.AttendTime = dt;
                        at.EditUserId = new LoginUser().EmployeeId;
                        at.EditTime = DateTime.Now;
                        at.State = 1;
                        at.Solved = false;
                        at.PunchedOrder = or;
                        entities.T_HR_AttendInsert.Add(at);
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
            }
            #endregion
            return r;
        }

        private string[] GetStaffIDFromDep(string id1, string id2, string id3, string id4, string id5)
        {
            string[] staffid=null;

            if(String.IsNullOrEmpty(id5) && String.IsNullOrEmpty(id4) && String.IsNullOrEmpty(id3) && String.IsNullOrEmpty(id2) && String.IsNullOrEmpty(id1))
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
                foreach(var item in list)
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
            if(!String.IsNullOrEmpty(id5) && (String.IsNullOrEmpty(id4) || String.IsNullOrEmpty(id3) || String.IsNullOrEmpty(id2) || String.IsNullOrEmpty(id1)))
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


    }
}
