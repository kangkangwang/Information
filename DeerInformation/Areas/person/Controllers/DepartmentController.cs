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
    public class DepartmentController : Controller
    {
        //
        // GET: /person/Department/
        Entities entities = new Entities();

        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = from o in entities.V_HR_Dep
                           where o.Valid==true
                           orderby o.No, o.DOrder
                            select o;
                
                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

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

        private List<V_HR_Dep> SearchData(string id, string name)//查询时根据ID和Name进行模糊查询
        {
            var list = entities.V_HR_Dep.ToList(); ;

            if (!String.IsNullOrEmpty(name))
            {
                list = (from o in list
                        where o.Name.Contains(name)
                        select o).ToList();
            }

            if (!String.IsNullOrEmpty(id))
            {
                list = (from o in list
                        where o.No.Contains(id)
                        select o).ToList();
            }

            return list;
        }

        [DirectMethod]
        public ActionResult DepartmentReload()//刷新gridpanel的store
        {
            try
            {
                var list = from o in entities.V_HR_Dep
                           where o.Valid == true
                           orderby o.No, o.DOrder
                           select o;

                var store = X.GetCmp<Store>("DepartmentStore");
                store.LoadData(list);
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult GetID1()
        {
            var list = (from o in entities.T_HR_Department1
                        select new { o.ID1, o.Department1Name }).ToList();
            return this.Store(list);
        }

        public ActionResult GetID2(string id1)
        {
            if (!String.IsNullOrEmpty(id1))
            {
                var list = (from o in entities.T_HR_Department2
                            where o.ID1 == id1
                            select new { o.ID2, o.Department2Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department2
                            //where o.ID1 == null
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
                            select new { o.ID3, o.Department3Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department3
                            //where o.ID2 == null
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
                            select new { o.ID4, o.Department4Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department4
                            //where o.ID3 == null
                            select new { o.ID4, o.Department4Name }).ToList();
                return this.Store(list);
            }
        }

        [VisitAuthorize(Create = true)]
        public ActionResult Add()//添加响应
        {
            Window win = new Window
            {

                ID = "windowDepartment",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddDepartment", "Department", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.DepartmentReload()",
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

                ID = "windowDepartment",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddDepartment", "Department", new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.DepartmentReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);

            return this.Direct();


        }

        public ActionResult AddDepartment(string id)//在修改时传递的为contractid
        {
            if (id == "-1")//-1为添加
            {
                return View(new Department());
            }
            else//否则为修改
            {
                V_HR_Dep item = (from o in entities.V_HR_Dep
                                         where o.ID == id
                                         select o).First();

                Department de = new Department();

                de.ID = item.ID;
                de.PreID = item.PreID;
                de.No = item.No;
                de.Name = item.Name;
                de.Level = Convert.ToInt32(item.Level);
                de.DOrder = Convert.ToInt32(item.DOrder);
                de.Remark = item.Remark;
                de.CreaterName = item.CreaterName;
                de.CreateTime = Convert.ToDateTime(item.CreateTime);
                de.EditorName = item.EditorName;
                de.EditeTime = Convert.ToDateTime(item.EditeTime);
                de.Valid = Convert.ToBoolean(item.Valid);

                if(de.Level==2)
                {
                    T_HR_Department1 d1 = (from o in entities.T_HR_Department1
                                           where o.ID1 == de.PreID
                                           select o).First();
                    de.ID1 = d1.ID1;
                }

                if (de.Level == 3)
                {
                    T_HR_Department2 d2 = (from o in entities.T_HR_Department2
                                           where o.ID2 == de.PreID
                                           select o).First();
                    de.ID2 = d2.ID2;
                    de.ID1 = d2.ID1;
                }

                if (de.Level == 4)
                {
                    T_HR_Department3 d3 = (from o in entities.T_HR_Department3
                                           where o.ID3 == de.PreID
                                           select o).First();
                    de.ID3 = d3.ID3;
                    de.ID2 = d3.ID2;

                    T_HR_Department2 d2 = (from o in entities.T_HR_Department2
                                           where o.ID2 == de.ID2
                                           select o).First();
                    de.ID1 = d2.ID1;
                }

                if (de.Level == 5)
                {
                    T_HR_Department4 d4 = (from o in entities.T_HR_Department4
                                           where o.ID4 == de.PreID
                                           select o).First();
                    de.ID4 = d4.ID4;
                    de.ID3 = d4.ID3;

                    T_HR_Department3 d3 = (from o in entities.T_HR_Department3
                                           where o.ID3 == de.ID3
                                           select o).First();
                    de.ID2 = d3.ID2;

                    T_HR_Department2 d2 = (from o in entities.T_HR_Department2
                                           where o.ID2 == de.ID2
                                           select o).First();
                    de.ID1 = d2.ID1;
                }

                var x = X.GetCmp<FieldSet>("BaseList");
                x.Hidden = true;

                return View(de);
            }
        }

        public int GetLevel(string id1,string id2,string id3,string id4)
        {
            int level=0;
            if(!String.IsNullOrEmpty(id1) && !String.IsNullOrEmpty(id2) && !String.IsNullOrEmpty(id3) && !String.IsNullOrEmpty(id4))
            {
                level = 5;
            }
            if (!String.IsNullOrEmpty(id1) && !String.IsNullOrEmpty(id2) && !String.IsNullOrEmpty(id3) && String.IsNullOrEmpty(id4))
            {
                level = 4;
            }
            if (!String.IsNullOrEmpty(id1) && !String.IsNullOrEmpty(id2) && String.IsNullOrEmpty(id3) && String.IsNullOrEmpty(id4))
            {
                level = 3;
            }
            if (!String.IsNullOrEmpty(id1) && String.IsNullOrEmpty(id2) && String.IsNullOrEmpty(id3) && String.IsNullOrEmpty(id4))
            {
                level = 2;
            }
            if (String.IsNullOrEmpty(id1) && String.IsNullOrEmpty(id2) && String.IsNullOrEmpty(id3) && String.IsNullOrEmpty(id4))
            {
                level = 1;
            }
            return level;
        }

        public string GetPreID(int level,string id1,string id2,string id3,string id4)
        {
            string preid = "-1";
            if(level==2)
            {
                preid = id1;
            }
            if (level == 3)
            {
                preid = id2;
            }
            if (level == 4)
            {
                preid = id3;
            }
            if (level == 5)
            {
                preid = id4;
            }
            return preid;
        }

        public string GetDepartment(int level, string id1, string id2, string id3, string id4,string dname)
        {
            string department = "";

            T_HR_Department1 dep1 = entities.T_HR_Department1.Find(id1);
            T_HR_Department2 dep2 = entities.T_HR_Department2.Find(id2);
            T_HR_Department3 dep3 = entities.T_HR_Department3.Find(id3);
            T_HR_Department4 dep4 = entities.T_HR_Department4.Find(id4);

            if (level == 1)
            {
                department = dname;
            }
            if (level == 2)
            {
                department = dep1.Department1Name + dname;
            }
            if (level == 3)
            {
                department = dep1.Department1Name+dep2.Department2Name + dname;
            }
            if (level == 4)
            {
                department = dep1.Department1Name + dep2.Department2Name+dep3.Department3Name + dname;
            }
            if (level == 5)
            {
                department = dep1.Department1Name + dep2.Department2Name + dep3.Department3Name+dep4.Department4Name + dname;
            }
            return department;
        }


        public ActionResult AddOrEditDepartment(Department dep)//AddContract保存相应
        {
            V_HR_Dep testdep = entities.V_HR_Dep.Find(dep.ID);
            dep.Level = GetLevel(dep.ID1, dep.ID2, dep.ID3, dep.ID4);
            if (dep.Level!=0)
            {
                DirectResult r = new DirectResult();
                if (testdep == null)//为空为添加
                {
                    
                    dep.ID = Tool.ProduceSed64(); ;
                    dep.Valid = true;
                    dep.CreaterName = new LoginUser().EmployeeId;
                    dep.CreateTime = DateTime.Now;
                    dep.PreID = GetPreID(dep.Level, dep.ID1, dep.ID2, dep.ID3, dep.ID4);

                    if (dep.Level == 1)
                    {
                        entities.T_HR_Department1.Add(dep.ToDB1(1));
                    }
                    if (dep.Level == 2)
                    {
                        entities.T_HR_Department2.Add(dep.ToDB2(1));
                    }
                    if (dep.Level == 3)
                    {
                        entities.T_HR_Department3.Add(dep.ToDB3(1));
                    }
                    if (dep.Level == 4)
                    {
                        entities.T_HR_Department4.Add(dep.ToDB4(1));
                    }
                    if (dep.Level == 5)
                    {
                        entities.T_HR_Department5.Add(dep.ToDB5(1));
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
                else//否则为修改
                {
                    dep.EditorName = new LoginUser().EmployeeId;
                    dep.EditeTime = DateTime.Now;
                    dep.PreID = GetPreID(dep.Level, dep.ID1, dep.ID2, dep.ID3, dep.ID4);

                    #region
                    if (testdep.Level == "1")
                    {
                        T_HR_Department1 dep1 = entities.T_HR_Department1.Find(dep.ID);
                        dep1.Department1No = dep.No;
                        dep1.Department1Name = dep.Name;
                        dep1.Remark = dep.Remark;
                        dep1.EditorName = dep.EditorName;
                        dep1.EditeTime = dep.EditeTime;
                        dep1.DOrder = dep.DOrder;
                    }
                    if (testdep.Level == "2")
                    {
                        T_HR_Department2 dep2 = entities.T_HR_Department2.Find(dep.ID);
                        dep2.Department2No = dep.No;
                        dep2.Department2Name = dep.Name;
                        dep2.Remark = dep.Remark;
                        dep2.EditorName = dep.EditorName;
                        dep2.EditeTime = dep.EditeTime;
                        dep2.DOrder = dep.DOrder;
                    }
                    if (testdep.Level == "3")
                    {
                        T_HR_Department3 dep3 = entities.T_HR_Department3.Find(dep.ID);
                        dep3.Department3No = dep.No;
                        dep3.Department3Name = dep.Name;
                        dep3.Remark = dep.Remark;
                        dep3.EditorName = dep.EditorName;
                        dep3.EditeTime = dep.EditeTime;
                        dep3.DOrder = dep.DOrder;
                    }
                    if (testdep.Level == "4")
                    {
                        T_HR_Department4 dep4 = entities.T_HR_Department4.Find(dep.ID);
                        dep4.Department4No = dep.No;
                        dep4.Department4Name = dep.Name;
                        dep4.Remark = dep.Remark;
                        dep4.EditorName = dep.EditorName;
                        dep4.EditeTime = dep.EditeTime;
                        dep4.DOrder = dep.DOrder;
                    }
                    if (testdep.Level == "5")
                    {
                        T_HR_Department5 dep5 = entities.T_HR_Department5.Find(dep.ID);
                        dep5.Department5No = dep.No;
                        dep5.Department5Name = dep.Name;
                        dep5.Remark = dep.Remark;
                        dep5.EditorName = dep.EditorName;
                        dep5.EditeTime = dep.EditeTime;
                        dep5.DOrder = dep.DOrder;
                    }
                    #endregion

                    string depname = GetDepartment(dep.Level, dep.ID1, dep.ID2, dep.ID3, dep.ID4, dep.Name);
                    var staffs = from o in entities.T_HR_Staff
                                where o.ID1 == dep.ID || o.ID2 == dep.ID || o.ID3 == dep.ID || o.ID4 == dep.ID || o.ID5 == dep.ID
                                select o;
                    foreach(var item in staffs)
                    {
                        T_HR_Staff staff = entities.T_HR_Staff.Find(item.StaffID);
                        staff.Department = depname;
                    }

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
            else
            {
                X.Msg.Alert("警告", "上级部门不能跳过为空！").Show();
                return this.Direct();
            }

        }

        [VisitAuthorize(Delete = true)]
        public ActionResult JSDeleteDepartment(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("DepartmentStore");
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                if(IsDelete(values))
                {
                    foreach (Dictionary<string, string> row in values)
                    {
                        id = row["ID"];
                        var dep = (from o in entities.V_HR_Dep
                                   where o.ID == id
                                   select o).First();

                        #region
                        if (dep.Level == "1")
                        {
                            T_HR_Department1 dep1 = entities.T_HR_Department1.Find(id);
                            if (dep1 != null)
                            {
                                //entities.T_HR_Contract.Remove(de);
                                dep1.Valid = false;
                            }
                        }
                        if (dep.Level == "2")
                        {
                            T_HR_Department2 dep2 = entities.T_HR_Department2.Find(id);
                            if (dep2 != null)
                            {
                                //entities.T_HR_Contract.Remove(de);
                                dep2.Valid = false;
                            }
                        }
                        if (dep.Level == "3")
                        {
                            T_HR_Department3 dep3 = entities.T_HR_Department3.Find(id);
                            if (dep3 != null)
                            {
                                //entities.T_HR_Contract.Remove(de);
                                dep3.Valid = false;
                            }
                        }
                        if (dep.Level == "4")
                        {
                            T_HR_Department4 dep4 = entities.T_HR_Department4.Find(id);
                            if (dep4 != null)
                            {
                                //entities.T_HR_Contract.Remove(de);
                                dep4.Valid = false;
                            }
                        }
                        if (dep.Level == "5")
                        {
                            T_HR_Department5 dep5 = entities.T_HR_Department5.Find(id);
                            if (dep5 != null)
                            {
                                //entities.T_HR_Contract.Remove(de);
                                dep5.Valid = false;
                            }
                        }
                        #endregion

                        var staffs = from o in entities.T_HR_Staff
                                     where o.ID1 == id || o.ID2 == id || o.ID3 == id || o.ID4 == id || o.ID5 == id
                                     select o;
                        foreach (var item in staffs)
                        {
                            T_HR_Staff staff = entities.T_HR_Staff.Find(item.StaffID);
                            staff.ID1 = null;
                            staff.ID2 = null;
                            staff.ID3 = null;
                            staff.ID4 = null;
                            staff.ID5 = null;
                            staff.Department = null;
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
                else
                {
                    X.Msg.Alert("警告", "不可删除存在下级部门的部门！").Show();
                }

            }
            else
            {
                X.Msg.Alert("提示", "未选择任何列！").Show();
            }

            return this.Direct();
        }

        public bool IsDelete(Dictionary<string, string>[] values)
        {
            bool flag = true;
            string id;
            foreach (Dictionary<string, string> row in values)
            {
                id = row["ID"];
                var list = from o in entities.V_HR_Dep
                           where o.PreID == id
                           select o;
                if (list.Any())
                {
                    flag = false;
                }
            }

            return flag;
        }

    }
}
