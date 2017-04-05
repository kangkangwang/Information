using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.BaseType;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Data.Entity.Infrastructure;

namespace DeerInformation.Areas.user.Models
{
    public class DailyWorkReportModel
    {
        public string ID { get; set; }
        [Field(AllowBlank = false, FieldLabel = "项目编号")]
        public string ProjectID { get; set; }
        [Field(AllowBlank = false, FieldLabel = "项目编号")]
        public string ProjectNo { get; set; }
        [Field(AllowBlank = false, FieldLabel = "项目名称")]
        public string ProjectName { get; set; }
        [Field(AllowBlank = false, FieldLabel = "考勤归属地")]
        public string ProjectSite { get; set; }
        [Field(AllowBlank = false, FieldLabel = "日期")]
        public System.DateTime Date { get; set; }
        public string OperationID { get; set; }
        [Field(AllowBlank = false, FieldLabel = "经办人")]
        public string Creator { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string AttachFile { get; set; }
        [Field(AllowBlank = false, FieldLabel = "项目进度百分比")]
        public int ProgressPercentage { get; set; }
        [Field(AllowBlank = true, FieldLabel = "备注")]
        public string Remark { get; set; }
        [Field(AllowBlank = false, FieldLabel = "审核流程")]
        public string CheckFlowId { get; set; }
        public static string CheckFucName = "工作日报";

        public static string CheckFucId
        {
            get
            {
                using (Entities db = new Entities())
                {
                    var firstOrDefault = db.T_CH_Checkfunc.FirstOrDefault(l => l.Name == CheckFucName);
                    return firstOrDefault != null ? firstOrDefault.ID : null;
                }
            }
        }

        //审核流程候选项
        public IEnumerable<ListItem> CheckFlowItems
        {
            get
            {
                using (Entities db = new Entities())
                {
                    var items = db.V_CH_Checkfuncflow.Where(l => l.CheckfuncName == CheckFucName && l.Disable == false).ToList();
                    return items.Select(item => new ListItem(item.Name, item.ID));
                }
            }
        }

        private List<WorkHours> _WorkHourses;

        public List<WorkHours> WorkHourses
        {
            get
            {
                if (_WorkHourses == null)
                {
                    using (Entities db = new Entities())
                    {
                        _WorkHourses = db.V_US_WorkReportWithHours.Where(l => l.ID == ID)
                            .Select(
                                l =>
                                    new WorkHours()
                                    {
                                        DailyReportID = l.ID,
                                        EmployeeID = l.EmployeeID,
                                        EmployeeName = l.EmployeeName,
                                        DutyHours = l.DutyHours,
                                        ExtraHours = l.ExtraHours
                                    }).ToList();
                    }
                }
                return _WorkHourses;
            }
            set { _WorkHourses = value; }
        }

        public CheckResult CheckResult { set; get; }

        public IEnumerable<ListItem> CheckStateItems
        {
            get
            {
                using (Entities db = new Entities())
                {
                    var items = db.T_CH_Check_state.ToList();
                    return items.Select(item => new ListItem(item.Description, item.ID)).ToList();
                }
            }
        }

        public List<V_US_WorkReportWithState> Select(string name, string date)
        {

            DateTime dateTime;
            LoginUser user=new LoginUser();;
            using (Entities db = new Entities())
            {

                string filtestr = string.Format("%{0}%", name.Trim());
                List<V_US_WorkReportWithState> result = db.V_US_WorkReportWithState.Where(
                    l =>l.Creator==user.EmployeeId&&
                        (SqlFunctions.PatIndex(filtestr, l.ProjectName) > 0 ||
                        SqlFunctions.PatIndex(filtestr, l.ProjectNo) > 0)).ToList();
                if (DateTime.TryParse(date, out dateTime))
                {
                    //result = result.Where(l =>EntityFunctions.DiffDays( l.Date , dateTime)==0).ToList();
                    dateTime = dateTime.Date;
                    result = (from o in result
                              where o.Date == dateTime
                              select o).ToList();
                }
                return result;
            }
        }

        public bool Save(Controller controller, FileUtility fileUtility)
        {
            if (fileUtility.File.ContentLength != 0)
            {
                if (!fileUtility.SavaData()) return false;
            }

            using (Entities db = new Entities())
            {
                try
                {
                    T_US_DailyWorkReport workReport = new T_US_DailyWorkReport()
                    {
                        ID = Guid.NewGuid().ToString(),
                        AttachFile = AttachFile,
                        CreateTime = DateTime.Now,
                        Creator = new LoginUser().EmployeeId,
                        OperationID = Guid.NewGuid().ToString(),
                        Date = Date,
                        ProgressPercentage = ProgressPercentage,
                        ProjectID = ProjectID,
                        ProjectNo = ProjectNo,
                        ProjectName = ProjectName,
                        ProjectSite = ProjectSite,
                        Remark = Remark
                    };
                    db.T_US_DailyWorkReport.Add(workReport);
                    T_CH_Operation_list operation = new T_CH_Operation_list()
                    {
                        Check_flowID = CheckFlowId,
                        Check_funcID = CheckFucId,
                        CreateTime = DateTime.Now,
                        Creator = new LoginUser().EmployeeId,
                        ID = workReport.OperationID,
                        State = (int)CheckState.Checking,
                        Url = controller.Url.Action("Check", "DailyWorkReport", new { id = workReport.ID })
                    };
                    db.T_CH_Operation_list.Add(operation);
                    foreach (var hour in WorkHourses)
                    {
                        T_US_WorkHours workHours = new T_US_WorkHours()
                        {
                            DailyReportID = workReport.ID,
                            DutyHours = hour.DutyHours,
                            EmployeeID = hour.EmployeeID,
                            EmployeeName = hour.EmployeeName,
                            ExtraHours = hour.ExtraHours,
                            ID = Guid.NewGuid().ToString()
                        };
                        db.T_US_WorkHours.Add(workHours);
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
        }

        public IEnumerable GetEmployee(string str)
        {
            using (Entities db = new Entities())
            {
                string fitformat = string.Format("%{0}%", str);
                var result = db.T_HR_Staff.Where(se => SqlFunctions.PatIndex(fitformat, se.Name) > 0 || SqlFunctions.PatIndex(fitformat, se.StaffID) > 0).Where(o=>o.HireState=="在职")
                    .Select(m => new { Name = m.Name, ID = m.StaffID })
                    .Take(5).ToList();
                return result;
            }
        }

        public DailyWorkReportModel GetChechReport(string id)
        {
            using (Entities db = new Entities())
            {
                try
                {
                    LoginUser user = new LoginUser();
                    var checkObject = db.V_US_WorkReportWithHours.FirstOrDefault(l => l.ID == id);
                    if (checkObject == null) return null;
                    ID = checkObject.ID;
                    AttachFile = checkObject.AttachFile;
                    CreateTime = checkObject.CreateTime;
                    Creator = checkObject.Creator;
                    OperationID = checkObject.OperationID;
                    Date = checkObject.Date;
                    ProgressPercentage = checkObject.ProgressPercentage;
                    ProjectID = checkObject.ProjectID;
                    ProjectNo = checkObject.ProjectNo;
                    ProjectName = checkObject.ProjectName;
                    ProjectSite = checkObject.ProjectSite;
                    Remark = checkObject.Remark;
                    var result =
                        db.V_CH_TaskOperation.FirstOrDefault(l => l.StaffID == user.EmployeeId && l.OperationID == OperationID && l.Expire == false);
                    if (result == null) return null;
                    CheckResult checkResult = new CheckResult() {Description = result.Description, State = result.State};
                    CheckFlowId = result.Check_flowID;
                    CheckResult = checkResult;
                    return this;
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }

        public DailyWorkReportModel GetDetailReport(string id)
        {
            using (Entities db = new Entities())
            {
                try
                {
                    var checkObject = db.V_US_WorkReportWithHours.FirstOrDefault(l => l.ID == id);
                    if (checkObject == null) return null;
                    ID = checkObject.ID;
                    AttachFile = checkObject.AttachFile;
                    CreateTime = checkObject.CreateTime;
                    Creator = checkObject.Creator;
                    OperationID = checkObject.OperationID;
                    Date = checkObject.Date;
                    ProgressPercentage = checkObject.ProgressPercentage;
                    ProjectID = checkObject.ProjectID;
                    ProjectNo = checkObject.ProjectNo;
                    ProjectName = checkObject.ProjectName;
                    ProjectSite = checkObject.ProjectSite;
                    Remark = checkObject.Remark;
                    var task =
                        db.V_CH_TaskOperation.Where( l => l.OperationID == OperationID ).ToList();
                    if (!task.Any()) return null;
                    var result = task.First();
                    CheckResult checkResult = new CheckResult() {  State = result.State_list };
                    foreach (var item in task)
                    {
                        checkResult.Description += string.Format("{0}({3}):{1},tel:{2}\r\n", item.Name, item.Description, item.TelNum, item.State_description);
                    }
                    CheckFlowId = result.Check_flowID;
                    CheckResult = checkResult;
                    return this;
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }

        public bool SubmitCheck()
        {
            using (Entities db=new Entities())
            {
               var result= db.P_CH_ExecuteCheck(OperationID, new LoginUser().EmployeeId, CheckResult.State, CheckResult.Description).ToList();
               if (result[0] != 1)//0表失败 1表成功
               {
                   return false;
               }
                return true;
            }
        }
    }

    public class WorkHours
    {
        public string ID { get; set; }
        public string DailyReportID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public Decimal DutyHours { get; set; }
        public Decimal ExtraHours { get; set; }
    }

    public class CheckResult
    {
        [Field(FieldLabel = "审核状态")]
        public int State { get; set; }
        [Field(FieldLabel = "审核备注")]
        public string Description { get; set; }
    }
}