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
    public class ContractController : Controller
    {
        //by:xgw
        // GET: /person/Contract/

        Entities entities = new Entities();

        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_ContractWithStaffName
                            //where o.Valid == true
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

        public ActionResult Getalldata(string name, string id)//查询按钮响应
        {
            try
            {
                var list = SearchData(name, id);

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

                ID = "windowContract",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddContract", "Contract", new { contractid = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.ContractReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddOrEditContract(V_HR_ContractWithStaffName contract)//AddContract保存相应
        {
            T_HR_Contract contractupdate = entities.T_HR_Contract.Find(contract.ID);

            DirectResult r = new DirectResult();
            if (contractupdate == null)//为空为添加
            {
                var ca = (from o in entities.V_HR_ContractWithStaffName
                          where o.StaffID == contract.StaffID && o.Valid == true
                          select o).ToList();
                if (!ca.Any())
                {
                    T_HR_Contract contractadd = new T_HR_Contract();
                    contractadd.ID = Tool.ProduceSed64();
                    contractadd.StaffID = contract.StaffID;
                    contractadd.Num = contract.Num;
                    contractadd.ContractType = contract.ContractType;
                    contractadd.StartTime = contract.StartTime;
                    contractadd.EndTime = contract.EndTime;
                    contractadd.Years = contract.Years;
                    contractadd.Company = contract.Company;
                    contractadd.Valid = true;
                    contractadd.Remark = contract.Remark;
                    contractadd.CreaterName = new LoginUser().EmployeeId;//后期改为用户名
                    contractadd.CreateTime = DateTime.Now;
                    entities.T_HR_Contract.Add(contractadd);
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
                    X.Msg.Alert("警告", "该员工合同已存在，不可重复添加！").Show();
                    return this.Direct();
                }
            }
            else//否则为修改
            {
                //contractupdate.StaffID = contract.StaffID;
                contractupdate.Num = contract.Num;
                contractupdate.ContractType = contract.ContractType;
                contractupdate.StartTime = contract.StartTime;
                contractupdate.EndTime = contract.EndTime;
                contractupdate.Years = contract.Years;
                contractupdate.Company = contract.Company;
                contractupdate.Valid = true;
                contractupdate.Remark = contract.Remark;
                contractupdate.EditorName = new LoginUser().EmployeeId;//后期改为用户名
                contractupdate.EditeTime = DateTime.Now;
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

        public ActionResult AddContract(string contractid)//在修改时传递的为contractid
        {
            if (contractid == "-1")//-1为添加
            {
                return View();
            }
            else//否则为修改
            {
                V_HR_ContractWithStaffName item = (from o in entities.V_HR_ContractWithStaffName
                                                   where o.ID == contractid
                                                   select o).First();

                var b = X.GetCmp<Button>("select");
                b.Disabled = true;
                //b.Hidden = false;

                var panel = X.GetCmp<Panel>("selectpanel");
                panel.Hidden = true;

                var com = X.GetCmp<ComboBox>("Company");
                com.Value = item.Company;

                return View(item);
            }
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id)//修改相应
        {
            Window win = new Window
            {

                ID = "windowContract",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddContract", "Contract", new { contractid = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.ContractReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();


        }

        private List<V_HR_ContractWithStaffName> SearchData(string name, string id)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_ContractWithStaffName>();

            list = (from o in entities.V_HR_ContractWithStaffName
                    //where o.Valid == true
                    select o).ToList();

            if (!String.IsNullOrEmpty(name))
            {
                list = (from o in list
                        where o.Name.Contains(name)
                        select o).ToList();
            }

            if (!String.IsNullOrEmpty(id))
            {
                list = (from o in list
                        where o.StaffID.Contains(id)
                        select o).ToList();
            }

            return list;
        }


        [DirectMethod]
        public ActionResult ContractReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_ContractWithStaffName
                            //where o.Valid == true
                            select o).ToList();

                var store = X.GetCmp<Store>("ContractStore");
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
        public ActionResult JSDeleteContract(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("ContractStore");
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["ID"];
                    T_HR_Contract de = entities.T_HR_Contract.Find(id);
                    if (de != null)
                    {
                        //entities.T_HR_Contract.Remove(de);
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

        public ActionResult DepClick(string dep)
        {
            var list = new List<V_HR_ContractWithStaffName>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    string[] staff = Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_ContractWithStaffName
                            where staff.Contains(o.StaffID) //&& o.Valid==true
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_ContractWithStaffName
                            //where o.Valid == true
                            select o).ToList();
                }

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
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

        #region DealSelectStaff
        [DirectMethod]
        public ActionResult DealGetperson()
        {
            //if(TempData["SelectedStaff"]!=null)
            //{
            //    string[] staff = TempData["SelectedStaff"].ToString().Split(',');
            //    if (staff.Length == 4)
            //    {
            //        X.GetCmp<TextField>("StaffID").SetValue(staff[0]);
            //        X.GetCmp<TextField>("Name").SetValue(staff[1]);
            //    }
            //}
            if (TempData["SelectedStaffList"] != null)
            {
                List<V_HR_IDNameWithDepName> s = (List<V_HR_IDNameWithDepName>)TempData["SelectedStaffList"];
                if (s.Count > 0)
                {
                    X.GetCmp<TextField>("StaffID").SetValue(s.First().StaffID);
                    X.GetCmp<TextField>("Name").SetValue(s.First().Name);
                }
            }

            return this.Direct();
        }

        [DirectMethod]
        public ActionResult DealGetpersonWithSDep()
        {
            //if (TempData["SelectedStaff"] != null)
            //{
            //    string[] staff = TempData["SelectedStaff"].ToString().Split(',');
            //    if (staff.Length == 4)
            //    {
            //        X.GetCmp<TextField>("StaffID").SetValue(staff[0]);
            //        X.GetCmp<TextField>("Name").SetValue(staff[1]);
            //        X.GetCmp<TextField>("SDepartMentId").SetValue(staff[2]);
            //        X.GetCmp<TextField>("SDepartMentName").SetValue(staff[3]);
            //    }
            //}
            if (TempData["SelectedStaffList"] != null)
            {
                List<V_HR_IDNameWithDepName> s = (List<V_HR_IDNameWithDepName>)TempData["SelectedStaffList"];
                if (s.Count > 0)
                {
                    X.GetCmp<TextField>("StaffID").SetValue(s.First().StaffID);
                    X.GetCmp<TextField>("Name").SetValue(s.First().Name);
                    X.GetCmp<TextField>("Department").SetValue(s.First().Department);
                    //X.GetCmp<TextField>("SDepartMentId").SetValue(s.First().SDepartMentId);
                    //X.GetCmp<TextField>("SDepartMentName").SetValue(s.First().SDepartMentName);
                }
            }

            return this.Direct();
        }

        [DirectMethod]
        public ActionResult _DealGetReperson()
        {
            if (TempData["SelectedStaffList"] != null)
            {
                List<V_HR_IDNameWithDepName> s = (List<V_HR_IDNameWithDepName>)TempData["SelectedStaffList"];
                if (s.Count > 0)
                {
                    X.GetCmp<TextField>("ReStaffID").SetValue(s.First().StaffID);
                    X.GetCmp<TextField>("ReName").SetValue(s.First().Name);
                }
            }

            return this.Direct();
        }

        #endregion



    }
}
