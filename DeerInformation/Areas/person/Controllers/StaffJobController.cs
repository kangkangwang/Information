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
    public class StaffJobController : Controller
    {
        //by:xgw
        // GET: /person/StaffJob/

        Entities entities = new Entities();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_StaffJobWithName
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

        public Node Getnode()
        {
            Node root = new Node();
            root.NodeID = "0";
            root.Text = "德尔集团";
            root.Expanded = true;
            root.Icon = Icon.World;

            try
            {
                var listjiagou = entities.V_ComJiaGou.ToList();

                foreach (var item in listjiagou)
                {
                    if (item.PID == "-1")
                    {
                        Node e = new Node();
                        e.NodeID = item.ID;
                        e.Expanded = true;
                        e.Text = item.Name;
                        e.Icon = Icon.UserEarth;
                        root.Children.Add(e);
                    }
                    else
                    {
                        var allnode = root.Children;
                        if (item.Level == "2")
                        {
                            foreach (var node in allnode)
                            {
                                if (item.PID == node.NodeID)
                                {
                                    Node e = new Node();
                                    e.NodeID = item.ID;
                                    e.Expanded = true;
                                    e.Text = item.Name;
                                    e.Icon = Icon.UserEarth;
                                    node.Children.Add(e);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            bool successed = false;
                            foreach (var node in allnode)
                            {
                                var allfnode = node.Children;
                                foreach (var fnode in allfnode)
                                {
                                    if (item.PID == fnode.NodeID)
                                    {
                                        Node e = new Node();
                                        e.NodeID = item.ID;
                                        e.Expanded = true;
                                        e.Text = item.Name;
                                        e.Icon = Icon.UserB;
                                        fnode.Children.Add(e);
                                        successed = true;
                                        break;
                                    }
                                }
                                if (successed) break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
            }

            return root;
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
        [VisitAuthorize(Create = true)]
        public ActionResult Add()//添加响应
        {
            Window win = new Window
            {

                ID = "windowStaffJob",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddStaffJob", "StaffJob", new { staffJobid = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.StaffJobReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddOrEditStaffJob(V_HR_StaffJobWithName staffJob)//AddstaffJob保存相应
        {
            DirectResult r = new DirectResult();
            T_HR_StaffJob staffJobupdate = entities.T_HR_StaffJob.Find(staffJob.StaffJobID);

            if (staffJobupdate == null)//为空为添加
            {
                T_HR_StaffJob staffJobadd = new T_HR_StaffJob();
                staffJobadd.StaffJobID = Tool.ProduceSed64();
                staffJobadd.StaffID = staffJob.StaffID;
                staffJobadd.JobID = staffJob.JobID;
                if (staffJob.ValidTime <= DateTime.Now.Date)//添加保存时可能存在两个有效职位,但是没有用到添加，员工职位的添加是在入职时设置的，那时还不存在该员工职位记录
                {
                    staffJobadd.IsValid = true;
                }
                else
                {
                    staffJobadd.IsValid = false;
                }
                staffJobadd.ValidTime = staffJob.ValidTime;
                staffJobadd.Remark = staffJob.Remark;
                staffJobadd.CreaterName = "admin";//后期改为用户名
                staffJobadd.CreateTime = DateTime.Now;
                entities.T_HR_StaffJob.Add(staffJobadd);
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
                T_HR_Staff s = entities.T_HR_Staff.Find(staffJob.StaffID);
                s.StaffType = staffJob.StaffType;
                T_HR_StaffJob staffJobadd = new T_HR_StaffJob();
                staffJobadd.StaffJobID = Tool.ProduceSed64();
                staffJobadd.StaffID = staffJob.StaffID;
                staffJobadd.JobID = staffJob.JobID;
                if (staffJob.ValidTime <= DateTime.Now.Date)
                {
                    staffJobupdate.IsValid = false;
                    staffJobadd.IsValid = true;
                }
                else
                {
                    staffJobupdate.IsValid = true;
                    staffJobadd.IsValid = false;
                }
                staffJobadd.ValidTime = staffJob.ValidTime;
                staffJobadd.ChangeType = staffJob.ChangeType;
                staffJobadd.Remark = staffJob.Remark;
                staffJobadd.CreaterName = "admin";//后期改为用户名
                staffJobadd.CreateTime = DateTime.Now;
                entities.T_HR_StaffJob.Add(staffJobadd);
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

        public ActionResult AddStaffJob(string staffJobid)//在修改时传递的为contractid
        {
            if (staffJobid == "-1")//-1为添加
            {
                //var s = X.GetCmp<TextField>("StaffID");
                //s.Text = "qwerqwreq";
                return View();
            }
            else//否则为修改
            {
                V_HR_StaffJobWithName item = (from o in entities.V_HR_StaffJobWithName
                                                   where o.StaffJobID == staffJobid
                                                   select o).First();

                return View(item);
            }
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id)//修改相应
        {
            Window win = new Window
            {

                ID = "windowStaffJob",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddStaffJob", "StaffJob", new { staffJobid = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.StaffJobReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();


        }

        private List<V_HR_StaffJobWithName> SearchData(string id, string name)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_StaffJobWithName>();

            list = (from o in entities.V_HR_StaffJobWithName
                        where o.IsValid == true
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

        //public ActionResult GetPositionCategory()
        //{
        //    var list = (from o in entities.T_HR_PositionCategory
        //                select new { o.PositionCategoryID, o.PositionCategoryName }).ToList();
        //    return this.Store(list);
        //}


        [DirectMethod]
        public ActionResult StaffJobReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_StaffJobWithName
                            where o.IsValid == true
                            select o).ToList();

                var store = X.GetCmp<Store>("StaffJobStore");
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
        [DirectMethod]
        public ActionResult JSDeleteStaffJob(string selectedData)//删除响应
        {
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(selectedData);

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["StaffJobID"];
                    T_HR_StaffJob de = entities.T_HR_StaffJob.Find(id);
                    if (de != null)
                    {
                        entities.T_HR_StaffJob.Remove(de);
                        try
                        {
                            entities.SaveChanges();
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

        //[DirectMethod]
        //public ActionResult SetPCid(string id)
        //{
        //    var idcom = X.GetCmp<TextField>("PCID");
        //    idcom.Text = id;
        //    return this.Direct();
        //}

        public ActionResult DepClick(string dep)
        {
            var list = new List<V_HR_StaffJobWithName>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    string[] staff = Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_StaffJobWithName
                            where staff.Contains(o.StaffID) && o.IsValid==true
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_StaffJobWithName
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

        //[DirectMethod]
        //public ActionResult DealGetperson()
        //{
        //    string[] staff = TempData["SelectedStaff"].ToString().Split(',');
        //    if (staff.Length == 2)
        //    {
        //        X.GetCmp<TextField>("StaffID").SetValue(staff[0]);
        //        X.GetCmp<TextField>("Name").SetValue(staff[1]);
        //    }

        //    return this.Direct();
        //}

        public ActionResult GetJob()
        {
            var list = (from o in entities.V_HR_Job
                        select new { o.JobID,o.JobName }).ToList();
            return this.Store(list);
        }

        [DirectMethod]
        public ActionResult SetJobid(string id)
        {
            var idcom = X.GetCmp<TextField>("JobID");
            var dname = X.GetCmp<TextField>("DutyName");
            var dtype = X.GetCmp<TextField>("DutyType");
            var pcname = X.GetCmp<TextField>("PCName");
            idcom.Text = id;
            try
            {
                V_HR_Job job = (from o in entities.V_HR_Job
                                where o.JobID == id
                                select o).First();
                dname.Text = job.DutyName;
                dtype.Text = job.DutyType;
                pcname.Text = job.PositionCategoryName;
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }         

            return this.Direct();
        }

        public ActionResult Detail(string id)//查看详细
        {
            Window win = new Window
            {

                ID = "windowStaffJobDetail",
                Title = "职位异动详细信息",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("StaffJobDetail", "StaffJob", new { staffid = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                }
                //Listeners =
                //{
                //    Close =
                //    {
                //        Handler = "App.direct.person.StaffJobReload()",
                //    }
                //}
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult StaffJobDetail(string staffid)
        {
            try
            {
                V_HR_StaffJobWithName sj = (from o in entities.V_HR_StaffJobWithName
                                            where o.StaffID == staffid
                                            select o).First();
                var textstaffid = X.GetCmp<TextField>("StaffID");
                textstaffid.Text = sj.StaffID;
                //var textdep = X.GetCmp<TextField>("SDepartMentName");
                //textdep.Text = sj.SDepartMentName;
                var textstafftype = X.GetCmp<TextField>("StaffType");
                textstafftype.Text = sj.StaffType;
                var textname = X.GetCmp<TextField>("Name");
                textname.Text = sj.Name;
                var textentrytime = X.GetCmp<TextField>("EntryTime");
                textentrytime.Text = Convert.ToDateTime(sj.EntryTime.ToString()).ToString("yyyy-MM-dd");

                var list = (from o in entities.V_HR_StaffJobWithName
                            where o.StaffID == staffid
                            select o).ToList();
                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }



    }
}
