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
    public class RecommendController : Controller
    {
        //
        // GET: /person/Recommend/

        Entities entities = new Entities();

        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_RecommendWithName
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
            var list = new List<V_HR_RecommendWithName>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    string[] staff = Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_RecommendWithName
                            where staff.Contains(o.StaffID)
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_RecommendWithName
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

        private List<V_HR_RecommendWithName> SearchData(string staffid, string name)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_RecommendWithName>();

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
        public ActionResult RecommendReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_RecommendWithName
                            select o).ToList();

                var store = X.GetCmp<Store>("RecommendStore");
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

                ID = "windowRecommend",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddRecommend", "Recommend", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.RecommendReload()",
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

                ID = "windowRecommend",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddRecommend", "Recommend", new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.RecommendReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);

            return this.Direct();


        }

        public ActionResult AddRecommend(string id)//在修改时传递的为contractid
        {
            if (id == "-1")//-1为添加
            {
                return View();
            }
            else//否则为修改
            {
                V_HR_RecommendWithName item = (from o in entities.V_HR_RecommendWithName
                                               where o.ID == id
                                               select o).First();


                return View(item);
            }
        }

        public ActionResult AddOrEditRecommend(V_HR_RecommendWithName re)
        {
            DirectResult r = new DirectResult();
            T_HR_Recommend reupdate = entities.T_HR_Recommend.Find(re.ID);

            if (reupdate == null)//为空为添加
            {
                T_HR_Recommend readd = new T_HR_Recommend();
                readd.ID = Guid.NewGuid().ToString();
                readd.StaffID = re.StaffID;
                readd.ReStaffID = re.ReStaffID;
                readd.Relation = re.Relation;
                readd.Money = re.Money;
                readd.YearMonth = Convert.ToDateTime(re.YearMonth.Value.Year.ToString() + "-" + re.YearMonth.Value.Month.ToString() + "-01");
                readd.YearMonth1 = Convert.ToDateTime(re.YearMonth1.Value.Year.ToString() + "-" + re.YearMonth1.Value.Month.ToString() + "-01");
                readd.IsDe = re.IsDe;
                readd.Valid = true;
                readd.Remark = re.Remark;
                readd.CreaterName = new LoginUser().EmployeeId;
                readd.CreateTime = DateTime.Now;

                entities.T_HR_Recommend.Add(readd);
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
                reupdate.StaffID = re.StaffID;
                reupdate.ReStaffID = re.ReStaffID;
                reupdate.Relation = re.Relation;
                reupdate.Money = re.Money;
                reupdate.YearMonth = Convert.ToDateTime(re.YearMonth.Value.Year.ToString() + "-" + re.YearMonth.Value.Month.ToString() + "-01");
                reupdate.YearMonth1 = Convert.ToDateTime(re.YearMonth1.Value.Year.ToString() + "-" + re.YearMonth1.Value.Month.ToString() + "-01");
                reupdate.IsDe = re.IsDe;
                reupdate.Valid = true;
                reupdate.Remark = re.Remark;
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
        public ActionResult JSDeleteRecommend(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("RecommendStore");
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["ID"];
                    T_HR_Recommend de = entities.T_HR_Recommend.Find(id);
                    if (de != null)
                    {
                        de.Valid = false;
                        //entities.T_HR_Recommend.Remove(de);
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

        public ActionResult SelectStaff(string type)
        {
            string handle="";
            if(type=="1")
            {
                handle = "App.direct.person.DealGetperson()";
            }
            if(type=="2")
            {
                handle = "App.direct.person._DealGetReperson()";
            }
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
                        Handler = handle,
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult SelectReStaff()
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
                        Handler = "App.direct.person._DealGetReperson()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        //[DirectMethod]
        //public ActionResult _DealGetReperson()
        //{
        //    if (TempData["SelectedStaff"] != null)
        //    {
        //        List<V_HR_IDNameWithDepName> s = (List<V_HR_IDNameWithDepName>)TempData["SelectedStaffList"];
        //        if (s.Count > 0)
        //        {
        //            X.GetCmp<TextField>("ReStaffID").SetValue(s.First().StaffID);
        //            X.GetCmp<TextField>("ReName").SetValue(s.First().Name);
        //        }
        //    }

        //    return this.Direct();
        //}



    }
}
