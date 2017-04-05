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
    public class EducationFundController : Controller
    {
        //by:xgw
        // GET: /person/EducationFund/

        Entities entities = new Entities();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_EducationFundWithName
                                where o.Valid==true
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
            var list = new List<V_HR_EducationFundWithName>();
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
                    list = (from o in entities.V_HR_EducationFundWithName
                            where staff.Contains(o.StaffID) && o.Valid==true
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_EducationFundWithName
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

                ID = "windowEducationFund",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddEducationFund", "EducationFund", new { educationFundid = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.EducationFundReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddOrEditEducationFund(V_HR_EducationFundWithName educationFund)//AddEducationFund保存相应
        {
            DirectResult r = new DirectResult();
            T_HR_EducationFund educationFundupdate = entities.T_HR_EducationFund.Find(educationFund.ID);

            if (educationFundupdate == null)//为空为添加
            {
                T_HR_EducationFund educationFundadd = new T_HR_EducationFund();
                educationFundadd.ID = Tool.ProduceSed64();
                educationFundadd.StaffID = educationFund.StaffID;
                educationFundadd.Style = educationFund.Style;
                educationFundadd.Money = educationFund.Money;
                educationFundadd.StartTime = educationFund.StartTime;
                educationFundadd.EndTime = educationFund.EndTime;
                educationFundadd.Valid = true;
                educationFundadd.Remark = educationFund.Remark;
                educationFundadd.CreaterName = new LoginUser().EmployeeId;//后期改为用户名
                educationFundadd.CreateTime = DateTime.Now;
                entities.T_HR_EducationFund.Add(educationFundadd);
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
                educationFundupdate.StaffID = educationFund.StaffID;
                educationFundupdate.Style = educationFund.Style;
                educationFundupdate.Money = educationFund.Money;
                educationFundupdate.StartTime = educationFund.StartTime;
                educationFundupdate.EndTime = educationFund.EndTime;
                educationFundupdate.Valid = true;
                educationFundupdate.Remark = educationFund.Remark;
                educationFundupdate.EditorName = new LoginUser().EmployeeId;//后期改为用户名
                educationFundupdate.EditeTime = DateTime.Now;
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

        public ActionResult AddEducationFund(string educationFundid)//在修改时传递的为contractid
        {
            if (educationFundid == "-1")//-1为添加
            {
                return View();
            }
            else//否则为修改
            {
                V_HR_EducationFundWithName item = (from o in entities.V_HR_EducationFundWithName
                                                   where o.ID == educationFundid
                                                   select o).First();

                return View(item);
            }
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id)//修改相应
        {
            Window win = new Window
            {

                ID = "windowEducationFund",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddEducationFund", "EducationFund", new { educationFundid = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.EducationFundReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();


        }

        private List<V_HR_EducationFundWithName> SearchData(string id,string name)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_EducationFundWithName>();

           list = (from o in entities.V_HR_EducationFundWithName
                           where o.Valid == true
                           select o).ToList();

            if (!String.IsNullOrEmpty(id))
            {
                list = (from o in list
                        where o.StaffID.Contains(id)
                        select o).ToList();
            }

            if (!String.IsNullOrEmpty(name))
            {
                list = (from o in list
                        where o.Name.Contains(name)
                        select o).ToList();
            }

            return list;
        }


        [DirectMethod]
        public ActionResult EducationFundReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_EducationFundWithName
                            where o.Valid == true
                            select o).ToList();

                var store = X.GetCmp<Store>("EducationFundStore");
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
        public ActionResult JSDeleteEducationFund(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("EducationFundStore");
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["ID"];
                    T_HR_EducationFund de = entities.T_HR_EducationFund.Find(id);
                    if (de != null)
                    {
                        //entities.T_HR_EducationFund.Remove(de);
                        de.Valid = false;
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




    }
}
