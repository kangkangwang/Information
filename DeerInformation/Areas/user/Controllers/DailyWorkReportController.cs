using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Areas.user.Models;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Text;
using System.Xml.Xsl;
using Newtonsoft.Json;
using System.Xml;
using System.Data.Entity.Infrastructure;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.user.Controllers
{
    [DirectController(AreaName = "user")]
    public class DailyWorkReportController : Controller
    {
        //
        // GET: /person/DailyWorkReport/

        Entities entities = new Entities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fiter(StoreRequestParameters parameters, string name = "", string date = "")
        {
            var list = new DailyWorkReportModel().Select(name, date);

            return this.Store(list.GetPage(parameters));
        }

        public ActionResult Read(string id)
        {
            if (id=="null")
            {
                X.Msg.Alert("页面消息","请选择一条记录！").Show();
            }
            else
            {
                WindowModule window = new WindowModule {Width =900,Height = 700,Loader = { Url = Url.Action("Detail", new { id = id }) } };
                window.Render(RenderMode.Auto);
            }

            return this.Direct();
        }

        public ActionResult Detail(string id)
        {
            var result = new DailyWorkReportModel().GetDetailReport(id);
            if (result == null)
            {
                return View("Error");
            }
            return View(result);
        }

        [VisitAuthorize(Create = true)]
        public ActionResult Add()
        {
            WindowModule window = new WindowModule { Width = 700, Height = 450, Loader = { Url = Url.Action("Create") } };
            window.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult Create()
        {
            return View(new DailyWorkReportModel());
        }

        public string Chongfu(List<WorkHours> whs)
        {
            string idstr = "",flag="-11111";
            string[] ids;
            foreach(var item in whs)
            {
                idstr += item.EmployeeID+",";
            }
            idstr = idstr.Substring(0, idstr.Length - 1);
            ids=idstr.Split(',');
            Array.Sort(ids);
            for (int i = 0; i < ids.Length-1; i++)
            {
                if(ids[i]==ids[i+1])
                {
                    flag = ids[i];
                    break;
                }
            }
            return flag;
        }

        public string ChongfuComDB(DailyWorkReportModel model)
        {
            string flag = "-11111";
            decimal dhs,ehs;
            string q;
            foreach (var item in model.WorkHourses)
            {
                dhs = 0;
                ehs = 0;
                //q = entities.V_US_WorkReportWithHours.Where(o => o.EmployeeID == item.EmployeeID).Where(o => o.Date == model.Date).Sum(o => o.DutyHours).ToString();
                //if (String.IsNullOrEmpty(q))
                //    dhs = 0;
                var list = from o in entities.V_US_WorkReportWithHours
                           where o.EmployeeID == item.EmployeeID && o.Date == model.Date
                           select o;
                if(list.Any())
                {
                    foreach(var li in list)
                    {
                        dhs += li.DutyHours;
                        ehs += li.ExtraHours;
                    }
                }

                if(dhs+item.DutyHours>8)
                {
                    flag = "具有员工该天正班工时将超过8小时，禁止添加！<br />工号：" + item.EmployeeID + "，姓名：" + item.EmployeeName + "，已有正班工时：" + dhs.ToString() + "计划添加工时：" + item.DutyHours.ToString();
                    break;
                }
                if (ehs + item.ExtraHours > 16)
                {
                    flag = "具有员工该天加班工时将超过16小时，禁止添加！<br />工号：" + item.EmployeeID + "，姓名：" + item.EmployeeName + "，已有加班工时：" + ehs.ToString() + "计划添加工时：" + item.ExtraHours.ToString();
                    break;
                }
            }
            return flag;
        }

        public ActionResult Submit(DailyWorkReportModel model)
        {
            string chongfu=Chongfu(model.WorkHourses);
            if (chongfu != "-11111")
            {
                var staff = from o in entities.T_HR_Staff
                            where o.StaffID == chongfu
                            select o;
                chongfu = "工号：" + chongfu + "，姓名：" + staff.First().Name;
                X.Msg.Alert("警告", "具有重复员工！<\br>" + chongfu).Show();
                return this.Direct();
            }

            string chongfudb = ChongfuComDB(model);
            if(chongfudb!="-11111")
            {
                X.Msg.Alert("警告", chongfudb).Show();
                return this.Direct();
            }

            FileUtility attachFile = new FileUtility();

            FileUploadField upload = this.GetCmp<FileUploadField>("AnnetPath");

            attachFile.File = upload.PostedFile;

            if (upload.HasFile)
            {
                model.AttachFile = string.Format("~/AttachFile/DailyWorkReport/{0}/{1}.{2}", DateTime.Now.Date.ToString("yyyy-MM-dd"), Guid.NewGuid(),
                    Path.GetExtension(attachFile.File.FileName));
                attachFile.FilePath = model.AttachFile;
                attachFile.SavePath = Server.MapPath(attachFile.FilePath);
                attachFile.FileType = attachFile.File.ContentType;
            }

            bool flag = model.Save(this, attachFile);
            X.Msg.Alert("页面消息", flag ? "工作日报提交成功！" : "工作日报提交失败！", flag?"parent.App.win.close();":null).Show();
            if (flag)
            {
                X.AddScript("parent.App.storedata.reload();");
            }
            return this.Direct();
        }

        public ActionResult GetEmployee(string query)
        {

            return new StoreResult(new DailyWorkReportModel().GetEmployee(query));
        }

        public ActionResult RemoveEmployee(string num)
        {
            int nums = Convert.ToInt32(num);
            if(nums>0)
            {
                var els = X.GetCmp<Container>("Employees");
                var el = X.GetCmp<Container>("empl" + num);
                els.Remove(el);
                this.GetCmp<TextField>("EmployeeNum").SetValue(nums - 1);
            }
            return this.Direct();
        }

        public ActionResult AddEmployee(string num)
        {
            int nums = Convert.ToInt32(num)+1;
            var employ = new Container()
            {
                ID="empl"+nums.ToString(),
                MarginSpec = "5 0 0 0",
                ColumnWidth = 1,
                Layout = LayoutType.Column.ToString(),
                Items =
                {
                    new ComboBox()
                    {
                        ID=string.Format("EmployeeId{0}",nums),FieldLabel = "工号",Name = string.Format("WorkHourses[{0}].EmployeeID",nums),ColumnWidth = 0.25,TriggerAction = TriggerAction.Query,DisplayField = "ID",ValueField = "ID",
                        AllowBlank = false,TypeAhead = true,MinChars =0,
                        ListConfig = new BoundList(){LoadingText ="查找中...",ItemTpl = new XTemplate(){Html = @"<text>
                            <div class='search-item'>
                                <h5><span>姓名：{Name}</span> 工号：{ID}</h5>
                            </div>
                        </text>"}},StoreID = "CheckerStore",Listeners = { Select = { Fn = "selectId"}}
                    },
                    new TextField(){ID = string.Format("EmployeeName{0}",nums),FieldLabel = "姓名",Name = string.Format("WorkHourses[{0}].EmployeeName",nums),ColumnWidth = 0.25,Editable=false,AllowBlank=false} ,
                    new NumberField(){FieldLabel ="正班工时",AllowBlank = false,Name = string.Format("WorkHourses[{0}].DutyHours",nums),ColumnWidth = 0.25},
                    new NumberField(){FieldLabel ="加班工时",AllowBlank = false,Name = string.Format("WorkHourses[{0}].ExtraHours",nums),ColumnWidth = 0.25}
                
                }
            };
            this.GetCmp<TextField>("EmployeeNum").SetValue(nums);
            employ.AddTo("Employees");
            return this.Direct();
        }

        public ActionResult Check(string id)
        {
            var checkObject = new DailyWorkReportModel().GetChechReport(id);
            if (checkObject==null)
            {
                return View("Expire");
            }
            ViewBag.id = checkObject.OperationID;
            string staffid = "";
            foreach(var item in checkObject.WorkHourses)
            {
                staffid += item.EmployeeID+",";
            }
            staffid = staffid.Substring(0, staffid.Length - 1);
            Session["xgw_dailyreport_staffid"] = staffid;
            Session["xgw_dailyreport_date"] = checkObject.Date;
            return View(checkObject );
        }

        public ActionResult SubmitCheck(DailyWorkReportModel model,string id)
        {
            //model.OperationID = id;
            //bool flag = model.SubmitCheck();
            //X.Msg.Alert("页面消息", flag ? "审核操作成功！" : "审核操作失败！", flag ? "parent.App.win.close();" : null).Show();
            //if (flag)
            //{
            //    X.AddScript("parent.App.storedata.reload();");
            //}
            //return this.Direct();

            try
            {
                model.OperationID = id;
                bool flag = model.SubmitCheck();
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

        public ActionResult GetProject()
        {
            var list = (from o in entities.T_GW_FieldInfoManagement
                        select new { o.ID, o.ProjectNo }).ToList();
            return this.Store(list);
        }

        [DirectMethod(Namespace = "dwr")]
        public ActionResult _SetPoInfo(string id)
        {
            T_GW_FieldInfoManagement data = entities.T_GW_FieldInfoManagement.Find(Convert.ToDecimal(id));
            if(data!=null)
            {
                var t1 = X.GetCmp<TextField>("ProjectName");
                var t2 = X.GetCmp<TextField>("ProjectSite");
                var t3 = X.GetCmp<TextField>("ProjectNo");
                t1.Text = data.ProjectName;
                t2.Text = data.FieldName;
                t3.Text = data.ProjectNo;
            }
            return this.Direct();
        }


        public ActionResult SearchStaff()
        {
            string staffid = (string)Session["xgw_dailyreport_staffid"];
            DateTime dt = (DateTime)Session["xgw_dailyreport_date"];
            Window win = new Window
            {

                ID = "windowStaffVacation",
                Title = "员工请假详细信息",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("StaffVacationAndHour", "DailyWorkReport", new { id = staffid ,date=dt.ToString()}),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                }
            };

            win.Render(RenderMode.Auto);

            return this.Direct();
        }

        public ActionResult StaffVacationAndHour(string id,string date)
        {
            try
            {
                string[] st = id.Split(',');
                DateTime dt = Convert.ToDateTime(date);
                string s = dt.Date.ToString().Substring(0,9);
                DateTime dt1 = Convert.ToDateTime(s + " 00:00:00");
                DateTime dt2 = Convert.ToDateTime(s + " 23:59:59");
                var vacationlist = (from o in entities.V_HR_VacationWithDepName
                                    where o.Valid == true && st.Contains(o.StaffID) && o.StartTime>=dt1 && o.StartTime<=dt2
                                    select o).ToList();

                return View(vacationlist);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }




        public ActionResult GetPlants(int start, int limit, int page, string query)
        {
            Paging<Plant> plants = Plant.PlantsPaging(start, limit, "", "", query);

            return this.Store(plants.Data, plants.TotalRecords);
        }


    }
}

