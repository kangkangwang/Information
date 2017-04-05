using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.BaseType;

namespace DeerInformation.Areas.person.Models
{
    public class VacationApply
    {
        public string ID
        {
            get;
            set;
        }

        public string StaffID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Department
        {
            get;
            set;
        }

        public string VPType
        {
            get;
            set;
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        public DateTime EndTime
        {
            get;
            set;
        }

        public string StartTimeStr
        {
            get;
            set;
        }

        public string EndTimeStr
        {
            get;
            set;
        }

        public decimal TimeSpan
        {
            get;
            set;
        }

        public string VPReason
        {
            get;
            set;
        }

        public bool Valid
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }

        public string CreaterName
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public string EditorName
        {
            get;
            set;
        }

        public DateTime EditeTime
        {
            get;
            set;
        }

        public string OperationListID
        {
            get;
            set;
        }

        public string LastID
        {
            get;
            set;
        }

        public bool VPEdit
        {
            get;
            set;
        }

        public bool VPDelete
        {
            get;
            set;
        }

        public string EditOrDelete
        {
            get;
            set;
        }

        public string FuncName
        {
            get { return "请假申请"; }
        }

        public string FuncId
        {
            get
            {
                using (Entities db = new Entities())
                {
                    var result = db.T_CH_Checkfunc.Where(l => l.Name == FuncName).ToList();
                    if (result.Any())
                    {
                        return result[0].ID;
                    }
                    return null;
                }
            }

        }

        public string GetOperationListID
        {
            get
            {
                using (Entities db = new Entities())
                {
                    return db.T_HR_Vacation.Find(ID).OperationListID;
                }
            }
        }

        public int State
        {
            get;
            set;
        }

        public string CheckFlowId
        {
            get;
            set;
        }

        public string DescriptionForCheck
        {
            get;
            set;
        }

        public string GetCheckFlowId
        {
            get
            {
                using (Entities db = new Entities())
                {
                    return db.T_CH_Operation_list.Find(GetOperationListID).Check_flowID;
                }
            }
        }

        public VacationApply()
        {

        }

        /// <summary>
        /// VacationApply对象转为数据库对象
        /// </summary>
        /// <param name="flag">标志，1为添加，2为修改需要复制修改人信息</param>
        /// <returns>T_HR_Dimission数据库对象</returns>

        public T_HR_Vacation ToDB(int flag)
        {        
            T_HR_Vacation va = new T_HR_Vacation();
            va.ID = ID;
            va.StaffID = StaffID;
            va.VPType = VPType;
            va.StartTime = StartTime;
            va.EndTime = EndTime;
            va.TimeSpan = TimeSpan;
            va.VPReason = VPReason;
            va.Valid = Valid;
            va.Remark = Remark;
            va.CreaterName = CreaterName;
            va.CreateTime = CreateTime;
            va.OperationListID = OperationListID;
            if (flag != 1)
            {
                va.EditorName = EditorName;
                va.EditeTime = EditeTime;
            }
            va.LastID = LastID;
            va.VPEdit = VPEdit;
            va.VPDelete = VPDelete;
            va.EditOrDelete = EditOrDelete;

            return va;
        }

        /// <summary>
        /// 获取给定ID对应OperationList的状态
        /// </summary>
        /// <param name="flag">ID</param>
        /// <returns>审核状态</returns>
        public static CheckState GetState(string id)
        {
            using (Entities db = new Entities())
            {
                var list = from o in db.T_CH_Operation_list
                           where o.ID == id
                           select o;
                if (list.Any())
                {
                    return (CheckState)list.First().State;
                }
                else
                {
                    return CheckState.IsNull;
                }
            }

        }

        /// <summary>
        /// 获取离职详细信息，包括审核状态和审核备注
        /// </summary>
        /// <param name="id">离职ID</param>
        /// <returns>异常返回null</returns>

        public VacationApply GetVacationDetail(string id)
        {
            V_HR_VacationWithDepName va;
            ID = id;
            using (Entities db = new Entities())
            {
                va = db.V_HR_VacationWithDepName.Find(id);
                T_CH_Operation_list op = db.T_CH_Operation_list.Find(GetOperationListID);
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == GetOperationListID
                           select o;

                if (va != null && task.Any())
                {
                    ID = va.ID;
                    StaffID = va.StaffID;
                    Name = va.Name;
                    Department = va.Department;
                    VPType = va.VPType;
                    StartTime = Convert.ToDateTime(va.StartTime);
                    EndTime = Convert.ToDateTime(va.EndTime);
                    StartTimeStr = StartTime.ToString("yyyy-MM-dd HH:mm");
                    EndTimeStr = EndTime.ToString("yyyy-MM-dd HH:mm");
                    TimeSpan = Convert.ToDecimal(va.TimeSpan);
                    VPReason = va.VPReason;
                    Valid = Convert.ToBoolean(va.Valid);
                    Remark = va.Remark;
                    CreaterName = va.CreaterName;
                    CreateTime = Convert.ToDateTime(va.CreateTime);
                    EditorName = va.EditorName;
                    EditeTime = Convert.ToDateTime(va.EditeTime);
                    OperationListID = va.OperationListID;
                    LastID = va.LastID;
                    VPEdit = Convert.ToBoolean(va.VPEdit);
                    VPDelete = Convert.ToBoolean(va.VPDelete);
                    EditOrDelete = va.EditOrDelete;

                    State = op.State;
                    DescriptionForCheck = "";
                    foreach (V_CH_TaskOperation items in task)
                    {
                        DescriptionForCheck += items.Description;
                    }
                    CheckFlowId = op.Check_flowID;
                }
                else
                {
                    return null;
                }
                return this;

            }
        }

        /// <summary>
        /// 获取附属离职详细信息，包括审核状态和审核备注
        /// </summary>
        /// <param name="id">离职ID</param>
        /// <returns>异常返回null</returns>

        public VacationApply GetPreVacationDetail(string id)
        {
            V_HR_VacationWithDepName va;
            PreID = id;
            using (Entities db = new Entities())
            {
                va = db.V_HR_VacationWithDepName.Find(id);
                T_CH_Operation_list op = db.T_CH_Operation_list.Find(GetPreOperationListID);
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == GetPreOperationListID
                           select o;

                if (va != null && task.Any())
                {
                    PreID = va.ID;
                    PreStaffID = va.StaffID;
                    PreName = va.Name;
                    PreDepartment = va.Department;
                    PreVPType = va.VPType;
                    PreStartTime = Convert.ToDateTime(va.StartTime);
                    PreEndTime = Convert.ToDateTime(va.EndTime);
                    PreStartTimeStr = PreStartTime.ToString("yyyy-MM-dd HH:mm");
                    PreEndTimeStr = PreEndTime.ToString("yyyy-MM-dd HH:mm");
                    PreTimeSpan = Convert.ToDecimal(va.TimeSpan);
                    PreVPReason = va.VPReason;
                    PreValid = Convert.ToBoolean(va.Valid);
                    PreRemark = va.Remark;
                    PreCreaterName = va.CreaterName;
                    PreCreateTime = Convert.ToDateTime(va.CreateTime);
                    PreEditorName = va.EditorName;
                    PreEditeTime = Convert.ToDateTime(va.EditeTime);
                    PreOperationListID = va.OperationListID;
                    PreLastID = va.LastID;
                    PreVPEdit = Convert.ToBoolean(va.VPEdit);
                    PreVPDelete = Convert.ToBoolean(va.VPDelete);
                    PreEditOrDelete = va.EditOrDelete;

                    PreState = op.State;
                    PreDescriptionForCheck = "";
                    foreach (V_CH_TaskOperation items in task)
                    {
                        PreDescriptionForCheck += items.Description;
                    }
                    PreCheckFlowId = op.Check_flowID;
                }
                else
                {
                    return null;
                }
                return this;

            }
        }

        #region 
        public string PreID
        {
            get;
            set;
        }

        public string PreStaffID
        {
            get;
            set;
        }

        public string PreName
        {
            get;
            set;
        }

        public string PreDepartment
        {
            get;
            set;
        }

        public string PreVPType
        {
            get;
            set;
        }

        public DateTime PreStartTime
        {
            get;
            set;
        }

        public DateTime PreEndTime
        {
            get;
            set;
        }

        public string PreStartTimeStr
        {
            get;
            set;
        }

        public string PreEndTimeStr
        {
            get;
            set;
        }

        public decimal PreTimeSpan
        {
            get;
            set;
        }

        public string PreVPReason
        {
            get;
            set;
        }

        public bool PreValid
        {
            get;
            set;
        }

        public string PreRemark
        {
            get;
            set;
        }

        public string PreCreaterName
        {
            get;
            set;
        }

        public DateTime PreCreateTime
        {
            get;
            set;
        }

        public string PreEditorName
        {
            get;
            set;
        }

        public DateTime PreEditeTime
        {
            get;
            set;
        }

        public string PreOperationListID
        {
            get;
            set;
        }

        public string GetPreOperationListID
        {
            get
            {
                using (Entities db = new Entities())
                {
                    return db.T_HR_Vacation.Find(PreID).OperationListID;
                }
            }
        }

        public string PreLastID
        {
            get;
            set;
        }

        public bool PreVPEdit
        {
            get;
            set;
        }

        public bool PreVPDelete
        {
            get;
            set;
        }

        public string PreEditOrDelete
        {
            get;
            set;
        }

        public int PreState
        {
            get;
            set;
        }

        public string PreCheckFlowId
        {
            get;
            set;
        }

        public string PreDescriptionForCheck
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 获取审核对象
        /// </summary>
        /// <param name="id">V_HR_VacationWithDepName的ID</param>
        /// <returns>异常返回null</returns>

        public VacationApply GetCheckVacation(string id)
        {
            V_HR_VacationWithDepName va;
            //V_CH_TaskOperation task;
            LoginUser user = new LoginUser();
            ID = id;
            using (Entities db = new Entities())
            {
                va = db.V_HR_VacationWithDepName.Find(id);
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == GetOperationListID && o.StaffID == user.EmployeeId && o.Expire == false
                           select o;
                //task = db.V_CH_TaskOperation.Where(l => l.OperationID == GetOperationListID && l.StaffID == user.EmployeeID && l.Expire == false).ToList().FirstOrDefault();

                if (va != null && task.Any())
                {
                    ID = va.ID;
                    StaffID = va.StaffID;
                    Name = va.Name;
                    Department = va.Department;
                    VPType = va.VPType;
                    StartTime = Convert.ToDateTime(va.StartTime);
                    EndTime = Convert.ToDateTime(va.EndTime);
                    StartTimeStr = StartTime.ToString("yyyy-MM-dd HH:mm");
                    EndTimeStr = EndTime.ToString("yyyy-MM-dd HH:mm");
                    TimeSpan = Convert.ToDecimal(va.TimeSpan);
                    VPReason = va.VPReason;
                    Valid = Convert.ToBoolean(va.Valid);
                    Remark = va.Remark;
                    CreaterName = va.CreaterName;
                    CreateTime = Convert.ToDateTime(va.CreateTime);
                    EditorName = va.EditorName;
                    EditeTime = Convert.ToDateTime(va.EditeTime);
                    OperationListID = va.OperationListID;
                    LastID = va.LastID;
                    VPEdit = Convert.ToBoolean(va.VPEdit);
                    VPDelete = Convert.ToBoolean(va.VPDelete);
                    EditOrDelete = va.EditOrDelete;

                    State = task.First().State;
                    DescriptionForCheck = task.First().Description;
                    CheckFlowId = task.First().Check_flowID;
                }
                else
                {
                    return null;
                }
                return this;

            }
        }

        /// <summary>
        /// 获取审核对象
        /// </summary>
        /// <param name="id">V_HR_VacationWithDepName的ID</param>
        /// <returns>异常返回null</returns>

        public VacationApply GetCheckPreVacation(string id)
        {
            V_HR_VacationWithDepName va;
            //V_CH_TaskOperation task;
            LoginUser user = new LoginUser();
            PreID = id;
            using (Entities db = new Entities())
            {
                va = db.V_HR_VacationWithDepName.Find(id);
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == GetPreOperationListID && o.StaffID == user.EmployeeId && o.Expire == false
                           select o;
                //task = db.V_CH_TaskOperation.Where(l => l.OperationID == GetOperationListID && l.StaffID == user.EmployeeID && l.Expire == false).ToList().FirstOrDefault();

                if (va != null && task.Any())
                {
                    PreID = va.ID;
                    PreStaffID = va.StaffID;
                    PreName = va.Name;
                    PreDepartment = va.Department;
                    PreVPType = va.VPType;
                    PreStartTime = Convert.ToDateTime(va.StartTime);
                    PreEndTime = Convert.ToDateTime(va.EndTime);
                    PreStartTimeStr = PreStartTime.ToString("yyyy-MM-dd HH:mm");
                    PreEndTimeStr = PreEndTime.ToString("yyyy-MM-dd HH:mm");
                    PreTimeSpan = Convert.ToDecimal(va.TimeSpan);
                    PreVPReason = va.VPReason;
                    PreValid = Convert.ToBoolean(va.Valid);
                    PreRemark = va.Remark;
                    PreCreaterName = va.CreaterName;
                    PreCreateTime = Convert.ToDateTime(va.CreateTime);
                    PreEditorName = va.EditorName;
                    PreEditeTime = Convert.ToDateTime(va.EditeTime);
                    PreOperationListID = va.OperationListID;
                    PreLastID = va.LastID;
                    PreVPEdit = Convert.ToBoolean(va.VPEdit);
                    PreVPDelete = Convert.ToBoolean(va.VPDelete);
                    PreEditOrDelete = va.EditOrDelete;

                    State = task.First().State;
                    DescriptionForCheck = task.First().Description;
                    CheckFlowId = task.First().Check_flowID;
                }
                else
                {
                    return null;
                }
                return this;

            }
        }

        /// <summary>
        /// 提交审核结果
        /// </summary>
        /// <returns>返回true表示成功</returns>
        public bool SubmitCheckVacation()
        {
            using (Entities db = new Entities())
            {
                LoginUser user = new LoginUser();
                string operationId = GetPreOperationListID;
                string checkerId = user.EmployeeId;
                int? state = PreState;
                string description = PreDescriptionForCheck;

                var result = db.P_CH_ExecuteCheck(operationId, checkerId, state, description).ToList();
                if (result[0] != 1)//0表失败 1表成功
                {
                    return false;
                }
                return true;
            }

        }



    }
}