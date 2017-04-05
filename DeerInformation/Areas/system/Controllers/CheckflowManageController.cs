using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Areas.system.Models;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Data.Objects.SqlClient;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.system.Controllers
{
    /// <summary>
    /// editor：WKK
    /// date：2016-8-20
    /// </summary>
    [DirectController(AreaName = "system")]
    public class CheckflowManageController : Controller
    {
        private Entities db = new Entities();
        //
        // GET: /system/CheckManage/
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            var result = new CheckflowManage();
            return View(result);
        }

        /// <summary>
        /// 返回查询结果
        /// </summary>
        /// <param name="checkfunc"></param>
        /// <returns></returns>
        public ActionResult Select(string checkfunc)
        {
            var result = new CheckflowManage();
            result.checkfunc = checkfunc == "null" ? "" : checkfunc;
            this.GetCmp<Store>("storedata").LoadData(result.checkflows);
            return this.Direct();
        }
        
        [VisitAuthorize(Create = true)]
        public ActionResult Create(string checkfunc)
        {
            if (db.T_CH_Checkfunc.Find(checkfunc) != null)
            {
                WindowModule win = new WindowModule();
                win.Loader.Url = Url.Action("CreateCheckFlow", "CheckflowManage", new { checkfunc = checkfunc });
                win.Render();
            }

            return this.Direct();
        }

        public ActionResult CreateCheckFlow(string checkfunc)
        {
            ViewBag.winname = "win";
            TempData["checkfunc"] = checkfunc;
            TempData["count"] = 0;
            LoginUser user = new LoginUser(); ;
            CheckFlow newFlow = new CheckFlow();
            newFlow.Creator = user.UserName;
            newFlow.CreatTime = DateTime.Now;
            newFlow.TimeLimit = 24;
            return View(newFlow);
        }
        /// <summary>
        /// 编辑表单
        /// </summary>
        /// <returns></returns>
        public ActionResult PlusCheckPerson()
        {

            if (TempData["count"] != null)
            {
                int count = (int)TempData["count"] + 1;
                var item = new Container() { Layout = LayoutType.HBox.ToString(), MarginSpec = "5 0 0 0" };
                ComboBox ni = new ComboBox()
                {
                    Name = string.Format("[{0}].ID", count),
                    Flex = 1,
                    AllowBlank = true,
                    TypeAhead = true,
                    MinChars = 0,
                    HideTrigger = true,
                    TriggerAction = TriggerAction.Query,
                    DisplayField = "Name",
                    ValueField = "ID",
                    ListConfig = new BoundList()
                    {
                        LoadingText = "查找中...",
                        ItemTpl = new XTemplate()
                        {
                            Html = @"<text>
                            <div class='search-item'>
                                <h3><span>姓名：{Name}</span> 工号：{ID}</h3>
                            </div>
                                    </text>"
                        }
                    },
                    StoreID = "CheckerStore"
                };

                item.Items.Add(ni);
                item.Items.Add(new NumberField() { Flex = 1, AllowBlank = false, MinValue = 0, Name = string.Format("[{0}].lvl1", count) });
                item.Items.Add(new NumberField() { Flex = 1, AllowBlank = false, MinValue = 0, Name = string.Format("[{0}].lvl2", count) });
                item.MarginSpec = "5 0 5 0";
                item.AddTo("form");
                TempData["count"] = count;
            }
            return this.Direct();
        }
        public ActionResult GetCheckers(string query,string checkers)
        {
            var data = db.T_HR_Staff;
            string fitformat = string.Format("%{0}%", query);
            var result = data.Where(se => SqlFunctions.PatIndex(fitformat, se.Name) > 0)
                .Select(m => new { Name = m.Name, ID = m.StaffID })
                .Take(5).ToList();
            var filled = checkers.JsonToList<dynamic>().Select(m => new { Name = m.Name, ID = m.ID });
            foreach (var item in filled)
            {
                result.Add(new {Name = (string)item.Name, ID = (string)item.ID});
            }
            return this.Store(result.Distinct());
        }
        /// <summary>
        /// 处理表单提交
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult Submit(CheckFlow Flow, Checker[] checkers)
        {
            if (checkers.Any(item => db.T_HR_Staff.FirstOrDefault(l=>l.StaffID==item.ID)==null))
            {
                return this.Direct(false);
            }
            if (TempData["checkfunc"] != null)
            {
                if (TempData["checkflow"] != null)
                {
                    string flowID = TempData["checkflow"].ToString();
                    T_CH_Checkflow orginflow = db.T_CH_Checkflow.Find(flowID);
                    if (orginflow == null)
                    {
                        return HttpNotFound();
                    }
                    orginflow.ID = flowID;
                    orginflow.Name = Flow.Name;
                    orginflow.Time_limit = Flow.TimeLimit;
                    orginflow.Creator = Flow.Creator;
                    orginflow.CreatorTime = Flow.CreatTime;
                    orginflow.Description = Flow.Description;
                    orginflow.Disable = Flow.Disable;
                    var orgincheckers = db.T_CH_Checker.Where(l => l.CheckFlowID == flowID).ToList();
                    foreach (var item in orgincheckers)
                    {
                        db.T_CH_Checker.Remove(item);
                    }

                    for (int i = 0; i < checkers.Length; i++)
                    {
                        T_CH_Checker item = new T_CH_Checker();
                        item.ID = Guid.NewGuid().ToString();
                        item.CheckFlowID = flowID;
                        item.CheckerID = checkers[i].ID;
                        item.lvl1 = checkers[i].lvl1;
                        item.lvl2 = checkers[i].lvl2;
                        db.T_CH_Checker.Add(item);
                    }

                }
                else
                {
                    T_CH_Checkflow newflow = new T_CH_Checkflow()
                    {
                        ID = Guid.NewGuid().ToString(),
                        Name = Flow.Name,
                        Time_limit = Flow.TimeLimit,
                        Creator = Flow.Creator,
                        CreatorTime = Flow.CreatTime,
                        Description = Flow.Description,
                        Disable = Flow.Disable
                    };

                    T_CH_Checkfunc_Checkflow_relation newr = new T_CH_Checkfunc_Checkflow_relation()
                    {
                        Check_funcID = (string)TempData["checkfunc"],
                        ID = Guid.NewGuid().ToString(),
                        Check_flowID = newflow.ID
                    };
                    db.T_CH_Checkflow.Add(newflow);
                    db.T_CH_Checkfunc_Checkflow_relation.Add(newr);
                    for (int i = 0; i < checkers.Length; i++)
                    {
                        T_CH_Checker item = new T_CH_Checker();
                        item.ID = Guid.NewGuid().ToString();
                        item.CheckFlowID = newr.Check_flowID;
                        item.CheckerID = checkers[i].ID;
                        item.lvl1 = checkers[i].lvl1;
                        item.lvl2 = checkers[i].lvl2;
                        db.T_CH_Checker.Add(item);
                    }
                }

                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return this.Direct(false);
                }
            }
            else
            {
                return this.Direct(false );
            }
            return this.Direct();
        }

        public ActionResult UpdateCheckFlow(string id, string winname)
        {
            if (id != null)
            {
                ViewBag.winname = winname;

                T_CH_Checkflow orgin = db.T_CH_Checkflow.FirstOrDefault(l => l.ID == id);
                if (orgin != default(T_CH_Checkflow))
                {
                    CheckFlow orginFlow = new CheckFlow();
                    orginFlow.CheckFlowID = orgin.ID;
                    orginFlow.Name = orgin.Name;
                    orginFlow.TimeLimit = orgin.Time_limit;
                    orginFlow.Creator = orgin.Creator;
                    orginFlow.CreatTime = orgin.CreatorTime.datetimedata();
                    orginFlow.Disable = orgin.Disable.booldata();
                    TempData["checkfunc"] = id;//没有任何意义（仅仅是填个值使他不为空）
                    TempData["checkflow"] = id;
                    TempData["count"] = orginFlow.Checkers.Count-1;
                    return View(orginFlow);
                }
            }
            return HttpNotFound();
        }
        //修改
        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id)
        {
            WindowModule newWindowModule = new WindowModule();
            newWindowModule.Loader.Url = Url.Action("UpdateCheckFlow", new { id = id, winname = newWindowModule.ID });
            newWindowModule.Render(RenderMode.Auto);

            return this.Direct();
        }

        public ActionResult Cancel()
        {
            return this.Direct(true);
        }
        //删除所选
        [VisitAuthorize(Delete = true)]
        public ActionResult Delete(string selection)
        {
            CheckflowManage manage=new CheckflowManage();
            JArray src = JArray.Parse(selection);
            V_CH_Checkfuncflow[] roles = new V_CH_Checkfuncflow[src.Count];
            foreach (var item in src)
            {
                roles[src.IndexOf(item)] = JSON.Deserialize<V_CH_Checkfuncflow>(src[src.IndexOf(item)].ToString());
            }
            for (int i = 0; i < roles.Count(); i++)
            {
                db.P_CH_DeleteCheckFlows(roles[i].ID);
                manage.checkfunc=roles[i].CheckfuncID;
            }
            //更新前台数据
            this.GetCmp<Store>("storedata").LoadData(manage.checkflows.ToList());
            return this.Direct();
        }
    }
}
