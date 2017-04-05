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
    public class DutyController : Controller
    {
        //by:xgw
        // GET: /person/Duty/

        Entities entities = new Entities();

        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = entities.V_HR_DutyWithPCName.ToList();
                //var store = X.GetCmp<Store>("DutyStore");
                //store.LoadData(list);
                
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
        [VisitAuthorize(Create = true)]
        public ActionResult Add()//添加响应
        {
            Window win = new Window
            {

                ID = "windowDuty",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddDuty", "Duty", new { dutyid = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.DutyReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddOrEditDuty(V_HR_DutyWithPCName duty)//AddPositionCategory保存相应
        {
            DirectResult r = new DirectResult();
            T_HR_Duty dutyupdate = entities.T_HR_Duty.Find(duty.DutyID);

            if (dutyupdate == null)//为空为添加
            {
                T_HR_Duty dutyadd = new T_HR_Duty();
                dutyadd.DutyID = duty.DutyID;
                dutyadd.DutyName = duty.DutyName;
                dutyadd.DutyLevel = duty.DutyLevel;
                dutyadd.DutyRank = duty.DutyRank;
                dutyadd.FunctionList = duty.FunctionList;
                dutyadd.DutyType = duty.DutyType;
                dutyadd.Remark = duty.Remark;
                dutyadd.PositionCategoryID = duty.PositionCategoryID;
                dutyadd.CreaterName = "admin";//后期改为用户名
                dutyadd.CreateTime = DateTime.Now;
                entities.T_HR_Duty.Add(dutyadd);
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
                dutyupdate.DutyName = duty.DutyName;
                dutyupdate.DutyLevel = duty.DutyLevel;
                dutyupdate.DutyRank = duty.DutyRank;
                dutyupdate.FunctionList = duty.FunctionList;
                dutyupdate.DutyType = duty.DutyType;
                dutyupdate.Remark = duty.Remark;
                dutyupdate.PositionCategoryID = duty.PositionCategoryID;
                dutyupdate.EditorName = "admin";//后期改为用户名
                dutyupdate.EditorTime = DateTime.Now;
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

        public ActionResult AddDuty(string dutyid)//在修改时传递的为dutyID
        {
            if (dutyid == "-1")//-1为添加，自动生成PositionCategoryID
            {
                string id, idnum;
                int num, n;
                V_HR_DutyWithPCName list = (from o in entities.V_HR_DutyWithPCName
                                              orderby o.DutyID descending
                                              select o).First();
                id = list.DutyID.ToString();
                num = int.Parse(id.Substring(2, 4)) + 1;
                idnum = num.ToString();
                n = idnum.Length;
                for (int i = 0; i < 4 - n; i++)
                {
                    idnum = "0" + idnum;
                }
                id = "ZW" + idnum;
                ViewData["AutoID"] = id;
                return View();
            }
            else//否则为修改
            {
                V_HR_DutyWithPCName item = (from o in entities.V_HR_DutyWithPCName
                                              where o.DutyID == dutyid
                                              select o).First();
                ViewData["AutoID"] = dutyid;      

                return View(item);
            }
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id)//修改相应，id为DutyID
        {
            Window win = new Window
            {

                ID = "windowDuty",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddDuty", "Duty", new { dutyid = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.DutyReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();


        }

        private List<V_HR_DutyWithPCName> SearchData(string id, string name)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_DutyWithPCName>();

            list = entities.V_HR_DutyWithPCName.ToList();

            if (!String.IsNullOrEmpty(id))
            {
                list = (from o in list
                        where o.DutyID.Contains(id)
                        select o).ToList();
            }

            if (!String.IsNullOrEmpty(name))
            {
                list = (from o in list
                        where o.DutyName.Contains(name)
                        select o).ToList();
            }

            return list;
        }

        public ActionResult GetPositionCategory()
        {
            var list = (from o in entities.T_HR_PositionCategory
                       select new { o.PositionCategoryID, o.PositionCategoryName }).ToList();
            return this.Store(list);
        }


        [DirectMethod]
        public ActionResult DutyReload()//刷新gridpanel的store
        {
            try
            {
                var list = entities.V_HR_DutyWithPCName.ToList();

                var store = X.GetCmp<Store>("DutyStore");
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
        public ActionResult JSDeleteDuty(string selectedData)//删除响应
        {
            string id;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(selectedData);

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["DutyID"];
                    T_HR_Duty de = entities.T_HR_Duty.Find(id);
                    if (de != null)
                    {
                        entities.T_HR_Duty.Remove(de);
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

        [DirectMethod]
        public ActionResult SetPCid(string id)
        {
            var idcom = X.GetCmp<TextField>("PCID");
            idcom.Text = id;
            return this.Direct();
        }



    }
}
