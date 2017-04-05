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
    public class VacationController : Controller
    {
        //
        // GET: /person/Vacation/

        Entities entities = new Entities();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_VacationWithDepName
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
            var list = new List<V_HR_VacationWithDepName>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    string[] staff = Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_VacationWithDepName
                            where staff.Contains(o.StaffID) && o.Valid == true
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_VacationWithDepName
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

        private List<V_HR_VacationWithDepName> SearchData(string staffid, string name)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_VacationWithDepName>();

            list = (from o in entities.V_HR_VacationWithDepName
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
        public ActionResult VacationReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_VacationWithDepName
                            where o.Valid == true
                            select o).ToList();

                var store = X.GetCmp<Store>("VacationStore");
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

                ID = "windowVacation",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddVacation", "Vacation", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.VacationReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id, string opid)//修改相应
        {
            if (VacationApply.GetState(opid) == CheckState.Rejected)
            {
                Window win = new Window
                {

                    ID = "windowVacation",
                    Title = "修改",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("AddVacation", "Vacation", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.VacationReload()",
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

        public ActionResult AddVacation(string id)//在修改时传递的为contractid
        {
            if (id == "-1")//-1为添加
            {
                VacationApply va = new VacationApply();
                va.CreaterName = new LoginUser().EmployeeName;
                va.CreateTime = DateTime.Now;
                return View(va);
            }
            else//否则为修改
            {
                V_HR_VacationWithDepName va = entities.V_HR_VacationWithDepName.Find(id);
                if (va == null) return HttpNotFound();
                VacationApply vaa = new VacationApply();
                vaa.ID = va.ID;
                vaa.StaffID = va.StaffID;
                vaa.Name = va.Name;
                vaa.Department = va.Department;
                vaa.VPType = va.VPType;
                vaa.StartTime = Convert.ToDateTime(va.StartTime);
                vaa.EndTime = Convert.ToDateTime(va.EndTime);
                vaa.StartTimeStr = vaa.StartTime.ToString("yyyy-MM-dd HH:mm");
                vaa.EndTimeStr = vaa.EndTime.ToString("yyyy-MM-dd HH:mm");
                vaa.TimeSpan = Convert.ToDecimal(va.TimeSpan);
                vaa.VPReason = va.VPReason;
                vaa.Valid = Convert.ToBoolean(va.Valid);
                vaa.Remark = va.Remark;
                vaa.CreaterName = va.CreaterName;
                vaa.CreateTime = Convert.ToDateTime(va.CreateTime);
                vaa.EditorName = va.EditorName;
                vaa.EditeTime = Convert.ToDateTime(va.EditeTime);
                vaa.OperationListID = va.OperationListID;
                if (vaa.OperationListID != null)
                    vaa.CheckFlowId = vaa.GetCheckFlowId;
                vaa.LastID = va.LastID;
                vaa.VPEdit = Convert.ToBoolean(va.VPEdit);
                vaa.VPDelete = Convert.ToBoolean(va.VPDelete);
                vaa.EditOrDelete = va.EditOrDelete;


                return View(vaa);
            }
        }

        public ActionResult CheckFlowItems()
        {
            var list = (from o in entities.V_CH_Checkfuncflow
                        where o.CheckfuncName == "请假申请"
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

        public ActionResult AddOrEditVacation(VacationApply vacation)
        {
            try
            {
                vacation.StartTime = Convert.ToDateTime(vacation.StartTimeStr);//计算TimeSpan
                vacation.EndTime = Convert.ToDateTime(vacation.EndTimeStr);
            }
            catch (FormatException fe)
            {
                X.Msg.Alert("警告", "日期不正确，请检查！").Show();
                return this.Direct();
            }

            if (vacation.StartTime.Year == vacation.EndTime.Year && vacation.StartTime.Month == vacation.EndTime.Month && vacation.StartTime <= vacation.EndTime)
            {
                DirectResult r = new DirectResult();
                T_HR_Vacation vacationupdate = entities.T_HR_Vacation.Find(vacation.ID);

                if (vacationupdate == null)//为空为添加
                {
                    vacation.ID = Guid.NewGuid().ToString();
                    vacation.OperationListID = Guid.NewGuid().ToString();
                    vacation.Valid = true;
                    vacation.CreaterName = new LoginUser().EmployeeId;
                    vacation.CreateTime = DateTime.Now;
                    vacation.VPEdit = false;
                    vacation.VPDelete = false;

                    T_CH_Operation_list newList = new T_CH_Operation_list();
                    newList.ID = vacation.OperationListID;
                    newList.State = (int)CheckState.Checking;//审核中
                    newList.Check_flowID = vacation.CheckFlowId;
                    newList.Check_funcID = vacation.FuncId;
                    newList.CreateTime = DateTime.Now;
                    newList.Creator = new LoginUser().EmployeeId;
                    newList.Url = Url.Action("CheckVacation", "Vacation", new { id = vacation.ID });

                    entities.T_HR_Vacation.Add(vacation.ToDB(1));
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
                    vacation.EditorName = new LoginUser().EmployeeId;
                    vacation.EditeTime = DateTime.Now;

                    vacationupdate.Valid = false;

                    vacation.ID = Guid.NewGuid().ToString();
                    vacation.OperationListID = Guid.NewGuid().ToString();
                    vacation.Valid = true;
                    vacation.VPEdit = false;
                    vacation.VPDelete = false;

                    T_CH_Operation_list newList = new T_CH_Operation_list();
                    newList.ID = vacation.OperationListID;
                    newList.State = (int)CheckState.Checking;//审核中
                    newList.Check_flowID = vacation.CheckFlowId;
                    newList.Check_funcID = vacation.FuncId;
                    newList.CreateTime = DateTime.Now;
                    newList.Creator = new LoginUser().EmployeeId;
                    newList.Url = Url.Action("CheckVacation", "Vacation", new { id = vacation.ID });

                    entities.T_HR_Vacation.Add(vacation.ToDB(2));
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
                X.Msg.Alert("警告", "请假不可跨月！").Show();
                return this.Direct();
            }

        }

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
        public ActionResult EditVacation(string id, string opid)
        {
            if (VacationApply.GetState(opid) == CheckState.Approved && GetPreState(id))
            {
                Window win = new Window
                {

                    ID = "windowVacation",
                    Title = "销假",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("VacationEdit", "Vacation", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.VacationReload()",
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
            T_HR_Vacation last = entities.T_HR_Vacation.Find(id);
            bool flag = true;
            string opid;

            if (Convert.ToBoolean(last.VPEdit))
            {
                var pre = (from o in entities.T_HR_Vacation
                           where o.LastID == id && o.VPEdit == true
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

            if (Convert.ToBoolean(last.VPDelete))
            {
                var pre = (from o in entities.T_HR_Vacation
                           where o.LastID == id && o.VPDelete == true
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
        [VisitAuthorize(Delete = true)]
        public ActionResult DeleteVacation(string id, string opid)
        {
            if (VacationApply.GetState(opid) == CheckState.Approved && GetPreState(id))
            {
                Window win = new Window
                {

                    ID = "windowVacation",
                    Title = "销假",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("VacationRemove", "Vacation", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.VacationReload()",
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

        public ActionResult VacationEdit(string id)
        {
            V_HR_VacationWithDepName va = entities.V_HR_VacationWithDepName.Find(id);
            if (va != null)
            {
                VacationApply vaa = new VacationApply();
                vaa.ID = va.ID;
                vaa.StaffID = va.StaffID;
                vaa.Name = va.Name;
                vaa.Department = va.Department;
                vaa.VPType = va.VPType;
                vaa.StartTime = Convert.ToDateTime(va.StartTime);
                vaa.EndTime = Convert.ToDateTime(va.EndTime);
                vaa.StartTimeStr = vaa.StartTime.ToString("yyyy-MM-dd HH:mm");
                vaa.EndTimeStr = vaa.EndTime.ToString("yyyy-MM-dd HH:mm");
                vaa.TimeSpan = Convert.ToDecimal(va.TimeSpan);
                vaa.VPReason = va.VPReason;
                vaa.Valid = Convert.ToBoolean(va.Valid);
                vaa.Remark = va.Remark;
                vaa.CreaterName = va.CreaterName;
                vaa.CreateTime = Convert.ToDateTime(va.CreateTime);
                vaa.EditorName = va.EditorName;
                vaa.EditeTime = Convert.ToDateTime(va.EditeTime);
                vaa.OperationListID = va.OperationListID;
                //if (vaa.OperationListID != null)
                //    vaa.CheckFlowId = vaa.GetCheckFlowId;
                vaa.LastID = va.LastID;
                vaa.VPEdit = Convert.ToBoolean(va.VPEdit);
                vaa.VPDelete = Convert.ToBoolean(va.VPDelete);
                vaa.EditOrDelete = va.EditOrDelete;

                return View(vaa);
            }
            else
            {
                return HttpNotFound();
            }



        }

        public ActionResult VacationRemove(string id)
        {
            V_HR_VacationWithDepName va = entities.V_HR_VacationWithDepName.Find(id);
            if (va != null)
            {
                VacationApply vaa = new VacationApply();
                vaa.ID = va.ID;
                vaa.StaffID = va.StaffID;
                vaa.Name = va.Name;
                vaa.Department = va.Department;
                vaa.VPType = va.VPType;
                vaa.StartTime = Convert.ToDateTime(va.StartTime);
                vaa.EndTime = Convert.ToDateTime(va.EndTime);
                vaa.StartTimeStr = vaa.StartTime.ToString("yyyy-MM-dd HH:mm");
                vaa.EndTimeStr = vaa.EndTime.ToString("yyyy-MM-dd HH:mm");
                vaa.TimeSpan = Convert.ToDecimal(va.TimeSpan);
                vaa.VPReason = va.VPReason;
                vaa.Valid = Convert.ToBoolean(va.Valid);
                vaa.Remark = va.Remark;
                vaa.CreaterName = va.CreaterName;
                vaa.CreateTime = Convert.ToDateTime(va.CreateTime);
                vaa.EditorName = va.EditorName;
                vaa.EditeTime = Convert.ToDateTime(va.EditeTime);
                vaa.OperationListID = va.OperationListID;
                //if (vaa.OperationListID != null)
                //    vaa.CheckFlowId = vaa.GetCheckFlowId;
                vaa.LastID = va.LastID;
                vaa.VPEdit = Convert.ToBoolean(va.VPEdit);
                vaa.VPDelete = Convert.ToBoolean(va.VPDelete);
                vaa.EditOrDelete = va.EditOrDelete;

                return View(vaa);
            }
            else
            {
                return HttpNotFound();
            }
        }

        public ActionResult VESubmit(VacationApply vacation)
        {
            try
            {
                vacation.StartTime = Convert.ToDateTime(vacation.StartTimeStr);//计算TimeSpan
                vacation.EndTime = Convert.ToDateTime(vacation.EndTimeStr);
            }
            catch (FormatException fe)
            {
                X.Msg.Alert("警告", "日期不正确，请检查！").Show();
                return this.Direct();
            }

            if (vacation.StartTime.Year == vacation.EndTime.Year && vacation.StartTime.Month == vacation.EndTime.Month && vacation.StartTime <= vacation.EndTime)
            {
                DirectResult r = new DirectResult();
                string orid = vacation.ID;

                var prelists = from o in entities.T_HR_Vacation
                               where o.VPEdit == true && o.LastID == orid
                               select o;
                foreach (T_HR_Vacation item in prelists)
                {
                    T_HR_Vacation prelist = entities.T_HR_Vacation.Find(item.ID);
                    prelist.VPEdit = false;
                }

                T_HR_Vacation orlist = entities.T_HR_Vacation.Find(orid);
                orlist.VPEdit = true;
                orlist.EditOrDelete = "Edit";

                vacation.ID = Guid.NewGuid().ToString();
                vacation.OperationListID = Guid.NewGuid().ToString();
                vacation.Valid = false;
                vacation.CreaterName = new LoginUser().EmployeeId;
                vacation.CreateTime = DateTime.Now;
                vacation.VPEdit = true;
                vacation.VPDelete = false;
                vacation.EditOrDelete = "";
                vacation.LastID = orid;

                T_CH_Operation_list newList = new T_CH_Operation_list();
                newList.ID = vacation.OperationListID;
                newList.State = (int)CheckState.Checking;//审核中
                newList.Check_flowID = vacation.CheckFlowId;
                newList.Check_funcID = vacation.FuncId;
                newList.CreateTime = DateTime.Now;
                newList.Creator = new LoginUser().EmployeeId;
                newList.Url = Url.Action("CheckVacation", "Vacation", new { id = vacation.ID });

                entities.T_HR_Vacation.Add(vacation.ToDB(1));
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

        public ActionResult VRSubmit(VacationApply vacation)
        {
            DirectResult r = new DirectResult();
            string orid = vacation.ID;

            var prelists = from o in entities.T_HR_Vacation
                           where o.VPDelete == true && o.LastID == orid
                           select o;
            foreach (T_HR_Vacation item in prelists)
            {
                T_HR_Vacation prelist = entities.T_HR_Vacation.Find(item.ID);
                prelist.VPDelete = false;
            }

            T_HR_Vacation orlist = entities.T_HR_Vacation.Find(orid);
            orlist.VPDelete = true;
            orlist.EditOrDelete = "Delete";

            vacation.ID = Guid.NewGuid().ToString();
            vacation.OperationListID = Guid.NewGuid().ToString();
            vacation.Valid = false;
            vacation.CreaterName = new LoginUser().EmployeeId;
            vacation.CreateTime = DateTime.Now;
            vacation.VPEdit = false;
            vacation.VPDelete = true;
            vacation.EditOrDelete = "";
            vacation.LastID = orid;

            T_CH_Operation_list newList = new T_CH_Operation_list();
            newList.ID = vacation.OperationListID;
            newList.State = (int)CheckState.Checking;//审核中
            newList.Check_flowID = vacation.CheckFlowId;
            newList.Check_funcID = vacation.FuncId;
            newList.CreateTime = DateTime.Now;
            newList.Creator = new LoginUser().EmployeeId;
            newList.Url = Url.Action("CheckVacation", "Vacation", new { id = vacation.ID });

            entities.T_HR_Vacation.Add(vacation.ToDB(1));
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

                ID = "windowVacation",
                Title = "详细信息",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("VacationDetail", "Vacation", new { id = id }),
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

        public ActionResult VacationDetail(string id)
        {
            VacationApply oritem = new VacationApply();
            if (null != oritem.GetVacationDetail(id))
            {
                if (oritem.VPEdit || oritem.VPDelete)
                {
                    if (oritem.EditOrDelete == "Edit")
                    {
                        var prelist = from o in entities.T_HR_Vacation
                                      where o.LastID == oritem.ID && o.VPEdit == true
                                      select o;

                        if (prelist.Any())
                        {
                            //oritem.preitem = oritem.GetVacationDetail(prelist.First().ID);
                            oritem.GetPreVacationDetail(prelist.First().ID);
                        }

                        //调整View
                    }
                    else if (oritem.EditOrDelete == "Delete")
                    {
                        var prelist = from o in entities.T_HR_Vacation
                                      where o.LastID == oritem.ID && o.VPDelete == true
                                      select o;

                        if (prelist.Any())
                        {
                            //oritem.preitem = oritem.GetVacationDetail(prelist.First().ID);
                            oritem.GetPreVacationDetail(prelist.First().ID);
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
            //int[] id = new int[]{2,3,4};
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

                    ID = "windowVacation",
                    Title = "审核",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("CheckVacation", "Vacation", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.VacationReload()",
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

        public ActionResult CheckVacation(string id)
        {
            VacationApply preitem = new VacationApply();
            if (null != preitem.GetPreVacationDetail(id))
            {
                if (preitem.PreVPEdit || preitem.PreVPDelete)
                {
                    var orelist = (from o in entities.T_HR_Vacation
                                  where o.ID == preitem.PreLastID
                                  select o).First();
                    preitem.GetVacationDetail(orelist.ID);
                    if (orelist.EditOrDelete == "Edit")
                    {
                        //调整View
                    }
                    else if (orelist.EditOrDelete == "Delete")
                    {
                        //调整View
                        var x1 = X.GetCmp<FieldSet>("PreList");
                        x1.Title = "假期取消";
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
                    x1.Title="请假申请";
                }
                return View(preitem);
            }
            else
            {
                return View();
            }


        }

        public ActionResult CheckSubmit(VacationApply va)
        {
            try
            {
                //if (va.SubmitCheckVacation())
                //{
                //    //X.Msg.Alert("消息", "操作成功！", new JFunction { Fn = "closewindow" }).Show();"parent.App.win.close();"
                //    X.MessageBox.Alert("消息", "操作成功！", "history.go(-1);").Show();
                //    return this.Direct();
                //}
                //else
                //{
                //    return View("Expire");
                //}

                bool flag = va.SubmitCheckVacation();
                bool newwindow=false;
                if (Session["NewWindow"] != null)
                    newwindow = Convert.ToBoolean(Session["NewWindow"]);
                X.Msg.Alert("页面消息", flag ? "审核操作成功！" : "审核操作失败！", flag ? (newwindow ? "parent.App.win.close();" : "history.go(-1);" ): null).Show();
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

        public ActionResult Search(string staffid,string preid)
        {
            Window win = new Window
            {

                ID = "windowStaffVacation",
                Title = "员工请假详细信息",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("StaffVacationAndHour", "Vacation", new { id = staffid,preid1=preid }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                }
            };

            win.Render(RenderMode.Auto);

            return this.Direct();
        }

        public ActionResult StaffVacationAndHour(string id,string preid1)
        {
            try
            {
                var vacationlist = (from o in entities.V_HR_VacationWithDepName
                            where o.Valid == true && o.StaffID==id && o.ID!=preid1
                            select o).ToList();

                var list = (from o in entities.V_HR_WorkOverTime
                                    where o.Valid == true && o.StaffID == id
                                    select o).ToList();
                if(list.Any())
                {
                    var si = X.GetCmp<TextField>("StaffID");
                    si.Text = list.First().StaffID;
                    var n = X.GetCmp<TextField>("Name");
                    n.Text = list.First().Name;
                    var ly = X.GetCmp<TextField>("LY");
                    ly.Text = list.First().LastYear.ToString();
                    var cy = X.GetCmp<TextField>("CY");
                    cy.Text = list.First().CurrentYear.ToString();
                    var cys = X.GetCmp<TextField>("CYS");
                    cys.Text = list.First().CurrentYearSum.ToString();
                    var p = X.GetCmp<TextField>("P");
                    p.Text = list.First().Personal.ToString();
                    var c = X.GetCmp<TextField>("C");
                    c.Text = list.First().Company.ToString();
                    var cyl = X.GetCmp<TextField>("CYL");
                    cyl.Text = list.First().CurrentYearLast.ToString();
                }

                return View(vacationlist);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }




    }
}
