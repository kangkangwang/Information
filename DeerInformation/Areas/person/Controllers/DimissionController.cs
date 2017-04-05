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
    public class DimissionController : Controller
    {
        //
        // GET: /person/Dimission/

        Entities entities = new Entities();

        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_Dimission
                            where o.IsValid == true
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
            var list = new List<V_HR_Dimission>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    string[] staff = Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_Dimission
                            where staff.Contains(o.StaffID) && o.IsValid == true
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_Dimission
                            where o.IsValid == true
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

        public ActionResult Getalldata(string staffid, string time)//查询按钮响应
        {
            try
            {
                var list = SearchData(staffid, time);

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        private List<V_HR_Dimission> SearchData(string staffid, string time)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_Dimission>();

            list = (from o in entities.V_HR_Dimission
                    where o.IsValid == true
                    select o).ToList();

            if (time != "null")
            {
                time = time.Replace("\"", "");
                DateTime dt = Convert.ToDateTime(time).Date;
                list = (from o in list
                        where o.DimissionTime == dt
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
        public ActionResult DimissionReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_Dimission
                            where o.IsValid == true
                            select o).ToList();

                var store = X.GetCmp<Store>("DimissionStore");
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

                ID = "windowDimission",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddDimission", "Dimission", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.DimissionReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id, string opid)//修改相应
        {
            if(DimissionApply.GetState(opid)==CheckState.Rejected)
            {
                Window win = new Window
                {

                    ID = "windowDimission",
                    Title = "修改",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("AddDimission", "Dimission", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.DimissionReload()",
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

        public ActionResult AddDimission(string id)//在修改时传递的为contractid
        {
            if (id == "-1")//-1为添加
            {
                DimissionApply da = new DimissionApply();
                da.StaffID = new LoginUser().EmployeeId;
                da.Name = new LoginUser().EmployeeName;
                return View(da);
            }
            else//否则为修改
            {
                V_HR_Dimission di = entities.V_HR_Dimission.Find(id);
                if (di == null) return HttpNotFound();
                DimissionApply dia = new DimissionApply();
                dia.ID = di.ID;
                dia.StaffID = di.StaffID;
                dia.Name = di.Name;
                dia.Department = di.Department;
                dia.DimissionTime = Convert.ToDateTime(di.DimissionTime);
                dia.DimissionType = di.DimissionType;
                dia.DimissionReason = di.DimissionReason;
                dia.ReasonType = di.ReasonType;
                dia.Num = Convert.ToInt32(di.Num);
                dia.IsValid = Convert.ToBoolean(di.IsValid);
                dia.Remark = di.Remark;
                dia.CreaterName = di.CreaterName;
                dia.CreateTime = Convert.ToDateTime(di.CreateTime);
                dia.CreateTimeStr = dia.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                dia.EditorName = di.EditorName;
                dia.EditeTime = Convert.ToDateTime(di.EditeTime);
                dia.OperationListID = di.OperationListID;
                if(dia.OperationListID!=null)
                    dia.CheckFlowId = dia.GetCheckFlowId;


                return View(dia);
            }
        }

        public ActionResult CheckFlowItems()
        {
            var list = (from o in entities.V_CH_Checkfuncflow
                        where o.CheckfuncName == "离职申请"
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
                        Handler = "App.direct.person.DealGetperson()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddOrEditDimission(DimissionApply dimission)
        {
            var dt = (from o in entities.T_HR_Dimission
                      where o.StaffID == dimission.StaffID && o.IsValid == true
                      select o).ToList();
            if(!dt.Any())
            {
                DirectResult r = new DirectResult();
                T_HR_Dimission dimissionupdate = entities.T_HR_Dimission.Find(dimission.ID);

                if (dimissionupdate == null)//为空为添加
                {
                    dimission.ID = Guid.NewGuid().ToString();
                    dimission.OperationListID = Guid.NewGuid().ToString();
                    dimission.Num = GetDimissionNum(dimission.StaffID);
                    dimission.IsValid = true;
                    dimission.CreaterName = new LoginUser().EmployeeId;
                    dimission.CreateTime = DateTime.Now;

                    T_CH_Operation_list newList = new T_CH_Operation_list();
                    newList.ID = dimission.OperationListID;
                    newList.State = 1;//审核中
                    newList.Check_flowID = dimission.CheckFlowId;
                    newList.Check_funcID = dimission.FuncId;
                    newList.CreateTime = DateTime.Now;
                    newList.Creator = new LoginUser().EmployeeId;
                    newList.Url = Url.Action("CheckDimission", "Dimission", new { id = dimission.ID });

                    entities.T_HR_Dimission.Add(dimission.ToT_HR_Dimission(1));
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
                    dimission.EditorName = new LoginUser().EmployeeId;
                    dimission.EditeTime = DateTime.Now;

                    dimission.ID = Guid.NewGuid().ToString();
                    dimission.OperationListID = Guid.NewGuid().ToString();
                    dimission.IsValid = true;

                    dimissionupdate.IsValid = false;

                    T_CH_Operation_list newList = new T_CH_Operation_list();
                    newList.ID = dimission.OperationListID;
                    newList.State = (int)CheckState.Checking; ;//审核中
                    newList.Check_flowID = dimission.CheckFlowId;
                    newList.Check_funcID = dimission.FuncId;
                    newList.CreateTime = DateTime.Now;
                    newList.Creator = new LoginUser().EmployeeId;
                    newList.Url = Url.Action("CheckDimission", "Dimission", new { id = dimission.ID });

                    entities.T_HR_Dimission.Add(dimission.ToT_HR_Dimission(2));
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
                X.Msg.Alert("警告", "该员工已存在离职记录，不可重复添加！").Show();
                return this.Direct();
            }
            
        }

        public int GetDimissionNum(string staffid)
        {
            int num;
            var list = from o in entities.T_HR_Dimission
                        where o.StaffID == staffid
                        orderby o.Num descending
                        select o;
            if(list.Any())
            {
                num = (int)list.First().Num + 1;
            }
            else
            {
                num = 1;
            }
            return num;
        }

        public ActionResult Check(string id)//审核相应
        {
            if(DimissionApply.GetExpire(id))
            {
                Window win = new Window
                {

                    ID = "windowDimission",
                    Title = "审核",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("CheckDimission", "Dimission", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.DimissionReload()",
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

        public ActionResult CheckDimission(string id)
        {
            DimissionApply item = new DimissionApply();
            if (null != item.GetCheckDimission(id))
            {
                return View(item);
            }
            else
            {
                return View();
            }


        }

        public ActionResult CheckSubmit(DimissionApply di)
        {
            try
            {
                bool flag = di.SubmitCheckDimission();
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
            //int[] id = new int[] { 2, 3, 4 };
            var list = (from o in entities.T_CH_Check_state
                        //where id.Contains(o.ID)
                        select new { o.ID, o.Description }).ToList();
            return this.Store(list);
        }

        public ActionResult Detail(string id)
        {
            Window win = new Window
            {

                ID = "windowDimission",
                Title = "详细信息",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("DimissionDetail", "Dimission", new { id = id }),
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

        public ActionResult DimissionDetail(string id)
        {
            DimissionApply item = new DimissionApply();
            if (null != item.GetDimissionDetail(id))
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
