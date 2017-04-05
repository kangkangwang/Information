using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;
using DeerInformation.BaseType;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;

namespace DeerInformation.Areas.person.Models
{
    public class PunchedExceptionModle
    {
        public List<V_HR_PuchedException> Exceptions
        {
            get
            {
                using (Entities db=new Entities() )
                {
                    string fitformat = string.Format("%{0}%", Name==null?"":Name.Trim());
                    return db.V_HR_PuchedException.Where(l => (EntityFunctions.DiffDays(Date, l.AttendTime)??0) == 0 
                        && (SqlFunctions.PatIndex(fitformat, l.Name) > 0 || SqlFunctions.PatIndex(fitformat, l.UserId) > 0)
                        &&l.Solved==Solved&&l.State==(int)State).ToList();
                }
            }
        }

        public List<ListItem> ExceptionTypesItems
        {
            get
            {
                using (Entities db = new Entities())
                {
                    var result = db.T_HR_ExceptionType.ToList();
                    return result.Select(l => new ListItem(l.Name,l.ID.ToString() )).ToList();
                }
            }
        } 
        public string Name { get; set; }

        public DateTime? Date { get; set; }

        public string Id { get; set; }

        public string EmployeeId { get; set; }

        public bool Solved { get; set; }

        public PunchedException State { get; set; }

        [Field(AllowBlank = false)]
        public string AttendTime { get; set; }

        [Field(FieldLabel = "备注")]
        public string Remark { get; set; }

        public T_HR_Staff Employee
        {
            get
            {
                using (Entities db = new Entities())
                {

                    return db.T_HR_Staff.FirstOrDefault(l => l.StaffID == EmployeeId);
                }
            }
        }

        private List<T_HR_Attendance> _attendRecords;

        public List<T_HR_Attendance> AttendRecords
        {
            get
            {
                if (_attendRecords == null)
                {
                    using (Entities db = new Entities())
                    {
                        _attendRecords = db.T_HR_Attendance.Where(l => l.UserId == EmployeeId && (EntityFunctions.DiffDays(Date, l.AttendTime) ?? -1) == 0).ToList();
                    }
                }
                return _attendRecords;

            }
            set { _attendRecords = value; }
        }

        public List<ListItem> AttendTimesItems
        {
            get { return  AttendRecords.Select(l => new ListItem(l.AttendTime.ToLongTimeString())).ToList(); }
        }
        private T_HR_AttendInsert _attendValidRecords;

        public T_HR_AttendInsert AttendValidRecords
        {
            get
            {
                if (_attendValidRecords == null)
                {
                    using (Entities db = new Entities())
                    {
                        _attendValidRecords = db.T_HR_AttendInsert.FirstOrDefault(l => l.ID == Id );
                    }
                }
                return _attendValidRecords;

            }
            set { _attendValidRecords = value; }
        }

        public bool DealException()
        {
            DateTime dateTime;
            if (!DateTime.TryParse(AttendTime, out dateTime)) return false;
            using (Entities db=new Entities())
            {
                try
                {
                    var user = new LoginUser();
                    var obj = db.T_HR_AttendInsert.Find(Id);

                    T_HR_AttendanceExceptionHandleRecords record=new T_HR_AttendanceExceptionHandleRecords()
                    {
                        BeforeTime = obj.AttendTime,
                        Date = obj.AttendTime.Date,
                        Editor = user.EmployeeId,
                        EmployeeID = obj.UserId,
                        ExceptionState = obj.State,
                        FinalTime = dateTime,
                        PunchedOrder = obj.PunchedOrder,
                        UpdateTime = DateTime.Now,
                        Remark = Remark
                    };
                    obj.AttendTime =obj.AttendTime.Date+ dateTime.TimeOfDay;
                    obj.EditUserId = user.EmployeeId;
                    obj.EditTime = DateTime.Now;
                    obj.Solved = true;
                    db.T_HR_AttendanceExceptionHandleRecords.Add(record);
                    db.SaveChanges();
                    return true;

                }
                catch (Exception )
                {
                    return false;
                }
            }

        }

    }
}