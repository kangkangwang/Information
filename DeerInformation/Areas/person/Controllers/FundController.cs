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
    public class FundController : Controller
    {
        //
        // GET: /person/Fund/

        Entities entities = new Entities();

        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_Fund
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
            var list = new List<V_HR_Fund>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    string[] staff = Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_Fund
                            where staff.Contains(o.StaffID) && o.Valid == true
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_Fund
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

        private List<V_HR_Fund> SearchData(string staffid, string name)//查询时根据ID和Name进行模糊查询
        {
            var list = (from o in entities.V_HR_Fund
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
        public ActionResult FundReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_Fund
                            where o.Valid == true
                            select o).ToList();

                var store = X.GetCmp<Store>("FundStore");
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

                ID = "windowFund",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddFund", "Fund", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.FundReload()",
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

                ID = "windowFund",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddFund", "Fund", new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.FundReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);

            return this.Direct();


        }

        public ActionResult AddFund(string id)//在修改时传递的为contractid
        {
            if (id == "-1")//-1为添加
            {
                return View();
            }
            else//否则为修改
            {
                V_HR_Fund item = (from o in entities.V_HR_Fund
                                  where o.ID == id
                                  select o).First();


                return View(item);
            }
        }

        public ActionResult AddOrEditFund(V_HR_Fund re)
        {
            DirectResult r = new DirectResult();
            T_HR_Fund reupdate = entities.T_HR_Fund.Find(re.ID);

            if (reupdate == null)//为空为添加
            {
                var ft = (from o in entities.V_HR_Fund
                          where o.StaffID == re.StaffID && o.Valid == true
                          select o).ToList();
                if (!ft.Any())
                {
                    T_HR_Fund readd = new T_HR_Fund();
                    readd.ID = Guid.NewGuid().ToString();
                    readd.StaffID = re.StaffID;
                    readd.StartDate = re.StartDate;
                    if (re.EndDate != null)
                    {
                        readd.EndDate = re.EndDate;
                        readd.Months = GetMonths(Convert.ToDateTime(re.StartDate), Convert.ToDateTime(re.EndDate));
                    }  
                    //readd.Months = re.Months;
                    readd.Valid = true;
                    readd.Remark = re.Remark;
                    readd.CreaterName = new LoginUser().EmployeeId;
                    readd.CreateTime = DateTime.Now;

                    T_HR_Staff s = entities.T_HR_Staff.Find(re.StaffID);
                    s.FundValid = true;

                    entities.T_HR_Fund.Add(readd);
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
                    X.Msg.Alert("警告", "该员工基金记录已存在，不可重复添加！").Show();
                    return this.Direct();
                }
            }
            else//否则为修改
            {
                reupdate.StaffID = re.StaffID;
                reupdate.StartDate = re.StartDate;
                if (re.EndDate != null)
                {
                    reupdate.EndDate = re.EndDate;
                    reupdate.Months = GetMonths(Convert.ToDateTime(re.StartDate), Convert.ToDateTime(re.EndDate));
                } 
                //reupdate.Months = re.Months;
                reupdate.Remark = re.Remark;
                reupdate.Valid = true;
                reupdate.EditorName = new LoginUser().EmployeeId;
                reupdate.EditeTime = DateTime.Now;

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
        public ActionResult JSDeleteFund(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("FundStore");
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["ID"];
                    T_HR_Fund de = entities.T_HR_Fund.Find(id);
                    if (de != null)
                    {
                        //entities.T_HR_Fund.Remove(de);
                        de.Valid = false;
                        T_HR_Staff st = entities.T_HR_Staff.Find(de.StaffID);
                        st.FundValid = false;
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

        public int GetMonths(DateTime dt1,DateTime dt2)
        {
            int months;
            months = (dt2.Year - dt1.Year) * 12 + (dt2.Month - dt1.Month);
            return months;
        }



    }
}
