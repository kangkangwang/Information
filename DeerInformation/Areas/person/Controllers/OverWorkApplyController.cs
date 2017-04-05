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
    public class OverWorkApplyController : Controller
    {
        //
        // GET: /person/OverWorkApply/

        Entities entities = new Entities();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_OWApplyWithDepName
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
            var list = new List<V_HR_OWApplyWithDepName>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    string[] staff = Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_OWApplyWithDepName
                            where staff.Contains(o.StaffID) && o.Valid == true
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_OWApplyWithDepName
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

        private List<V_HR_OWApplyWithDepName> SearchData(string staffid, string name)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_OWApplyWithDepName>();

            list = (from o in entities.V_HR_OWApplyWithDepName
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
        public ActionResult OWApplyReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_OWApplyWithDepName
                            where o.Valid == true
                            select o).ToList();

                var store = X.GetCmp<Store>("OverWorkApplyStore");
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

                ID = "windowOWApply",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddOverWorkApply", "OverWorkApply", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.OWApplyReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id, string opid)//修改相应
        {
            if (OWApply.GetState(opid) == CheckState.Rejected)
            {
                Window win = new Window
                {

                    ID = "windowOWApply",
                    Title = "修改",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("AddOverWorkApply", "OverWorkApply", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.OWApplyReload()",
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

        public ActionResult AddOverWorkApply(string id)//在修改时传递的为contractid
        {
            if (id == "-1")//-1为添加
            {
                OWApply va = new OWApply();
                va.CreaterName = new LoginUser().EmployeeName;
                va.CreateTime = DateTime.Now;
                return View(va);
            }
            else//否则为修改
            {
                V_HR_OWApplyWithDepName di = entities.V_HR_OWApplyWithDepName.Find(id);
                if (di == null) return HttpNotFound();
                OWApply dia = new OWApply();
                dia.ID = di.ID;
                dia.StaffID = di.StaffID;
                dia.Name = di.Name;
                dia.Department = di.Department;
                dia.StartTime = Convert.ToDateTime(di.StartTime);
                dia.EndTime = Convert.ToDateTime(di.EndTime);
                dia.StartTimeStr = dia.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                dia.EndTimeStr = dia.EndTime.ToString("yyyy-MM-dd HH:mm:ss");
                dia.TimeSpan = Convert.ToDecimal(di.TimeSpan);
                dia.CheckTimeSpan = Convert.ToDecimal(di.CheckTimeSpan);
                dia.Valid = Convert.ToBoolean(di.Valid);
                dia.Remark = di.Remark;
                dia.CreaterName = di.CreaterName;
                dia.CreateTime = Convert.ToDateTime(di.CreateTime);
                dia.EditorName = di.EditorName;
                dia.EditeTime = Convert.ToDateTime(di.EditeTime);
                dia.OperationListID = di.OperationListID;
                if (dia.OperationListID != null)
                    dia.CheckFlowId = dia.GetCheckFlowId;


                return View(dia);
            }
        }

        public ActionResult CheckFlowItems()
        {
            var list = (from o in entities.V_CH_Checkfuncflow
                        where o.CheckfuncName == "可调休加班时数申请"
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

        public ActionResult AddOrEditOWApply(OWApply owapply)
        {
            owapply.StartTime = Convert.ToDateTime(owapply.StartTimeStr);
            owapply.EndTime = Convert.ToDateTime(owapply.EndTimeStr);
            if(owapply.StartTime<owapply.EndTime)
            {
                DirectResult r = new DirectResult();
                T_HR_OverWorkApply owapplyupdate = entities.T_HR_OverWorkApply.Find(owapply.ID);

                if (owapplyupdate == null)//为空为添加
                {
                    owapply.ID = Guid.NewGuid().ToString();
                    owapply.OperationListID = Guid.NewGuid().ToString();
                    owapply.Valid = true;
                    owapply.CreaterName = new LoginUser().EmployeeId;
                    owapply.CreateTime = DateTime.Now;

                    T_CH_Operation_list newList = new T_CH_Operation_list();
                    newList.ID = owapply.OperationListID;
                    newList.State = (int)CheckState.Checking;//审核中
                    newList.Check_flowID = owapply.CheckFlowId;
                    newList.Check_funcID = owapply.FuncId;
                    newList.CreateTime = DateTime.Now;
                    newList.Creator = new LoginUser().EmployeeId;
                    newList.Url = Url.Action("CheckOverWorkApply", "OverWorkApply", new { id = owapply.ID });

                    entities.T_HR_OverWorkApply.Add(owapply.ToDB(1));
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
                    owapply.EditorName = new LoginUser().EmployeeId;
                    owapply.EditeTime = DateTime.Now;

                    owapplyupdate.Valid = false;

                    owapply.ID = Guid.NewGuid().ToString();
                    owapply.OperationListID = Guid.NewGuid().ToString();
                    owapply.Valid = true;

                    T_CH_Operation_list newList = new T_CH_Operation_list();
                    newList.ID = owapply.OperationListID;
                    newList.State = (int)CheckState.Checking; ;//审核中
                    newList.Check_flowID = owapply.CheckFlowId;
                    newList.Check_funcID = owapply.FuncId;
                    newList.CreateTime = DateTime.Now;
                    newList.Creator = new LoginUser().EmployeeId;
                    newList.Url = Url.Action("CheckOverWorkApply", "OverWorkApply", new { id = owapply.ID });

                    entities.T_HR_OverWorkApply.Add(owapply.ToDB(2));
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
                return r;
            }
            else
            {
                X.Msg.Alert("警告", "调休加班时段错误！").Show();
                return this.Direct();
            }
            
        }

        public ActionResult Check(string id)//审核相应
        {
            if (OWApply.GetExpire(id))
            {
                Window win = new Window
                {

                    ID = "windowOWApply",
                    Title = "审核",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("CheckOverWorkApply", "OverWorkApply", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.OWApplyReload()",
                        }
                    }
                };

                win.Render(RenderMode.Auto);
            }
            else
            {
                X.Msg.Alert("警告", "该审核任务已过期或不存在！").Show();
            }

            return this.Direct();


        }

        public ActionResult CheckOverWorkApply(string id)
        {
            OWApply item = new OWApply();
            if (null != item.GetCheckOW(id))
            {
                return View(item);
            }
            else
            {
                return View();
            }


        }

        public ActionResult CheckSubmit(OWApply di)
        {
            try
            {
                var list = entities.T_HR_OverWorkApply.Find(di.ID);
                list.CheckTimeSpan = di.CheckTimeSpan;
                entities.SaveChanges();

                bool flag = di.SubmitCheckOWApply();
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

        public ActionResult CheckStateItems()
        {
          //  int[] id = new int[] { 2, 3, 4 };
            var list = (from o in entities.T_CH_Check_state
                      //  where id.Contains(o.ID)
                        select new { o.ID, o.Description }).ToList();
            return this.Store(list);
        }
        [VisitAuthorize(Read = true)]
        public ActionResult Detail(string id)
        {
            Window win = new Window
            {

                ID = "windowOWApply",
                Title = "详细信息",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("OverWorkApplyDetail", "OverWorkApply", new { id = id }),
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

        public ActionResult OverWorkApplyDetail(string id)
        {
            OWApply item = new OWApply();
            if (null != item.GetOWDetail(id))
            {
                return View(item);
            }
            else
            {
                return View();
            }
        }



    }
}
