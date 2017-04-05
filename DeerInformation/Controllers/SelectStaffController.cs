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

namespace DeerInformation.Controllers
{
    public class SelectStaffController : Controller
    {
        //
        // GET: /SelectStaff/

        #region
        Entities entities = new Entities();
        private List<V_HR_IDNameWithDepName> ReadySelect = new List<V_HR_IDNameWithDepName>();
        private List<V_HR_IDNameWithDepName> Selected = new List<V_HR_IDNameWithDepName>();
        private int num,schfilt;


        public ActionResult SelectStaff(int NUM=1,int SchFilt=0)
        {
            try
            {
                ///树初始化
                ViewData["root"] = DeerInformation.Areas.person.Tool.GetNode();
                ReadySelect = entities.V_HR_IDNameWithDepName.Where(o=>o.HireState=="在职").ToList();
                schfilt = SchFilt;
                if (schfilt == 1)
                {
                    var staffid = from o in entities.T_HR_SchListWithStaff
                                  select o.StaffID;

                    ReadySelect = (from o in ReadySelect
                                  where !staffid.Contains(o.StaffID)
                                  orderby o.StaffID
                                  select o).ToList();
                }
                num = NUM;
                Session["xgw_schfilt"] = schfilt;
                Session["xgw_num"] = num;
                Session["ReadySelect"] = ReadySelect;
                Session["Selected"] = Selected;
                return View(ReadySelect);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult Getalldata(string staffid, string name)//查询按钮响应
        {
            ReadySelect = (List<V_HR_IDNameWithDepName>)Session["ReadySelect"];
            Selected = (List<V_HR_IDNameWithDepName>)Session["Selected"];
            schfilt = (int)Session["xgw_schfilt"];

            try
            {
                var selectid = from o in Selected
                               select o.StaffID;

                ReadySelect = (from o in entities.V_HR_IDNameWithDepName
                               where !selectid.Contains(o.StaffID) && o.HireState=="在职"
                               select o).ToList();

                if (!String.IsNullOrEmpty(staffid))
                {
                    ReadySelect = (from o in ReadySelect
                                   where o.StaffID.Contains(staffid)
                                   select o).ToList();
                }

                if (!String.IsNullOrEmpty(name))
                {
                    ReadySelect = (from o in ReadySelect
                                   where o.Name.Contains(name)
                                   select o).ToList();
                }

                if (schfilt == 1)
                {
                    var staffida = from o in entities.T_HR_SchListWithStaff
                                  select o.StaffID;

                    ReadySelect = (from o in ReadySelect
                                   where !staffida.Contains(o.StaffID)
                                   orderby o.StaffID
                                   select o).ToList();
                }

                var p = X.GetCmp<Store>("ReadySelectStore");
                p.LoadData(ReadySelect);
                var p1 = X.GetCmp<Store>("SelectedStore");
                p1.LoadData(Selected);
                Session["ReadySelect"] = ReadySelect;
                Session["Selected"] = Selected;

                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult DepClick(string dep)
        {
            schfilt = (int)Session["xgw_schfilt"];
            ReadySelect = (List<V_HR_IDNameWithDepName>)Session["ReadySelect"];
            Selected = (List<V_HR_IDNameWithDepName>)Session["Selected"];
            var list = new List<V_HR_IDNameWithDepName>();
            dep = dep.Replace("\"", "");
            var selectedid = from o in Selected
                             select o.StaffID;
            try
            {
                if (dep != "0")
                {
                    string[] staff = DeerInformation.Areas.person.Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_IDNameWithDepName
                            where staff.Contains(o.StaffID) && o.HireState=="在职"
                            select o).ToList();

                    ReadySelect = (from o in list
                                   where !selectedid.Contains(o.StaffID)
                                   select o).ToList();
                }
                else
                {
                    list = entities.V_HR_IDNameWithDepName.Where(o=>o.HireState=="在职").ToList();
                    ReadySelect = (from o in list
                                   where !selectedid.Contains(o.StaffID)
                                   select o).ToList();
                }
                if (schfilt == 1)
                {
                    var staffida = from o in entities.T_HR_SchListWithStaff
                                   select o.StaffID;

                    ReadySelect = (from o in ReadySelect
                                   where !staffida.Contains(o.StaffID)
                                   orderby o.StaffID
                                   select o).ToList();
                }
                Session["ReadySelect"] = ReadySelect;
                Session["Selected"] = Selected;
                return this.Store(ReadySelect);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult Add(string id)
        {
            ReadySelect = (List<V_HR_IDNameWithDepName>)Session["ReadySelect"];
            Selected = (List<V_HR_IDNameWithDepName>)Session["Selected"];
            num = (int)Session["xgw_num"];
            int selectednum = Selected.Count;

            try
            {
                if (selectednum < num)
                {
                    var selected = from o in entities.V_HR_IDNameWithDepName
                                   where o.StaffID == id && o.HireState == "在职"
                                   select o;
                    foreach (var item in selected)
                    {
                        ReadySelect = (from o in ReadySelect
                                       where o.StaffID != item.StaffID
                                       select o).ToList();
                        Selected.Add(item);
                    }
                    var p = X.GetCmp<Store>("ReadySelectStore");
                    p.LoadData(ReadySelect);
                    var p1 = X.GetCmp<Store>("SelectedStore");
                    p1.LoadData(Selected);
                    Session["ReadySelect"] = ReadySelect;
                    Session["Selected"] = Selected;
                }
                else
                {
                    X.Msg.Alert("提示", "只能选择"+num+"条记录，请删除已选择记录，再重新选择！").Show();
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        public ActionResult Delete(string id)
        {
            ReadySelect = (List<V_HR_IDNameWithDepName>)Session["ReadySelect"];
            Selected = (List<V_HR_IDNameWithDepName>)Session["Selected"];

            string selectedid = "";
            if (Selected.Count != 0)
            {
                foreach (var item in Selected)
                {
                    selectedid = item.StaffID;
                }
            }

            try
            {
                if (selectedid != "")
                {
                    var selected = from o in entities.V_HR_IDNameWithDepName
                                   where o.StaffID == id && o.HireState == "在职"
                                   select o;
                    foreach (var item in selected)
                    {
                        if (ReadySelect.Count != 0)
                        {
                            ReadySelect.Add(item);
                            ReadySelect = (from o in ReadySelect
                                           orderby o.StaffID
                                           select o).ToList();
                        }
                        else
                        {
                            ReadySelect = selected.ToList();
                        }
                        Selected = (from o in Selected
                                    where o.StaffID != item.StaffID
                                    select o).ToList();
                    }
                    var p = X.GetCmp<Store>("ReadySelectStore");
                    p.LoadData(ReadySelect);
                    var p1 = X.GetCmp<Store>("SelectedStore");
                    p1.LoadData(Selected);

                    Session["ReadySelect"] = ReadySelect;
                    Session["Selected"] = Selected;
                }
                else
                {
                    X.Msg.Alert("提示", "尚未选择一条记录！").Show();
                }

                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }


        }

        public ActionResult Submit()
        {
            Selected = (List<V_HR_IDNameWithDepName>)Session["Selected"];
            //string staff = "";
            if (Selected.Count != 0)
            {
                //foreach (var item in Selected)
                //{
                //    staff = item.StaffID + "," + item.Name + "," + item.SDepartMentId + "," + item.SDepartMentName;
                //}
                //TempData["SelectedStaff"] = staff;
                TempData["SelectedStaffList"] = Selected;
                Session.Remove("Selected");
                Session.Remove("ReadySelect");
                Session.Remove("xgw_num");
            }
            else
            {
                X.Msg.Alert("提示", "尚未选择任何人员！").Show();
            }

            return this.Direct();
        }

        public ActionResult Reset()
        {
            try
            {
                ReadySelect = entities.V_HR_IDNameWithDepName.Where(o=> o.HireState=="在职").ToList();
                Session["ReadySelect"] = ReadySelect;
                Session["Selected"] = Selected;
                var p = X.GetCmp<Store>("ReadySelectStore");
                p.LoadData(ReadySelect);
                var p1 = X.GetCmp<Store>("SelectedStore");
                p1.LoadData(Selected);
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }
        #endregion

    }
}
