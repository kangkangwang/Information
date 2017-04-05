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
    public class BTApply
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

        public string TimeSpan
        {
            get;
            set;
        }

        public string BTPlace
        {
            get;
            set;
        }

        public string BTReason
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

        public bool BTEdit
        {
            get;
            set;
        }

        public bool BTDelete
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
            get { return "出差申请"; }
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
                    return db.T_HR_BusinessTrip.Find(ID).OperationListID;
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

        public BTApply()
        {

        }

        /// <summary>
        /// BTApply对象转为数据库对象
        /// </summary>
        /// <param name="flag">标志，1为添加，2为修改需要复制修改人信息</param>
        /// <returns>T_HR_BusinessTrip数据库对象</returns>

        public T_HR_BusinessTrip ToDB(int flag)
        {
            T_HR_BusinessTrip bt = new T_HR_BusinessTrip();
            bt.ID = ID;
            bt.StaffID = StaffID;
            bt.BTPlace = BTPlace;
            bt.StartTime = StartTime;
            bt.EndTime = EndTime;
            bt.TimeSpan = TimeSpan;
            bt.BTReason = BTReason;
            bt.Valid = Valid;
            bt.Remark = Remark;
            bt.CreaterName = CreaterName;
            bt.CreateTime = CreateTime;
            bt.OperationListID = OperationListID;
            if (flag != 1)
            {
                bt.EditorName = EditorName;
                bt.EditeTime = EditeTime;
            }
            bt.LastID = LastID;
            bt.BTEdit = BTEdit;
            bt.BTDelete = BTDelete;
            bt.EditOrDelete = EditOrDelete;

            return bt;
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
        /// 获取详细信息，包括审核状态和审核备注
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>异常返回null</returns>

        public BTApply GetBusinessTripDetail(string id)
        {
            V_HR_BTWithDepName bt;
            ID = id;
            using (Entities db = new Entities())
            {
                bt = db.V_HR_BTWithDepName.Find(id);
                T_CH_Operation_list op = db.T_CH_Operation_list.Find(GetOperationListID);
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == GetOperationListID
                           select o;

                if (bt != null && task.Any())
                {
                    ID = bt.ID;
                    StaffID = bt.StaffID;
                    Name = bt.Name;
                    Department = bt.Department;
                    BTPlace = bt.BTPlace;
                    StartTime = Convert.ToDateTime(bt.StartTime);
                    EndTime = Convert.ToDateTime(bt.EndTime);
                    StartTimeStr = StartTime.ToString("yyyy-MM-dd HH:mm");
                    EndTimeStr = EndTime.ToString("yyyy-MM-dd HH:mm");
                    TimeSpan = bt.TimeSpan;
                    BTReason = bt.BTReason;
                    Valid = Convert.ToBoolean(bt.Valid);
                    Remark = bt.Remark;
                    CreaterName = bt.CreaterName;
                    CreateTime = Convert.ToDateTime(bt.CreateTime);
                    EditorName = bt.EditorName;
                    EditeTime = Convert.ToDateTime(bt.EditeTime);
                    OperationListID = bt.OperationListID;
                    LastID = bt.LastID;
                    BTEdit = Convert.ToBoolean(bt.BTEdit);
                    BTDelete = Convert.ToBoolean(bt.BTDelete);
                    EditOrDelete = bt.EditOrDelete;

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
        /// 获取附属详细信息，包括审核状态和审核备注
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>异常返回null</returns>

        public BTApply GetPreBusinessTripDetail(string id)
        {
            V_HR_BTWithDepName bt;
            PreID = id;
            using (Entities db = new Entities())
            {
                bt = db.V_HR_BTWithDepName.Find(id);
                T_CH_Operation_list op = db.T_CH_Operation_list.Find(GetPreOperationListID);
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == GetPreOperationListID
                           select o;

                if (bt != null && task.Any())
                {
                    PreID = bt.ID;
                    PreStaffID = bt.StaffID;
                    PreName = bt.Name;
                    PreDepartment = bt.Department;
                    PreBTPlace = bt.BTPlace;
                    PreStartTime = Convert.ToDateTime(bt.StartTime);
                    PreEndTime = Convert.ToDateTime(bt.EndTime);
                    PreStartTimeStr = PreStartTime.ToString("yyyy-MM-dd HH:mm");
                    PreEndTimeStr = PreEndTime.ToString("yyyy-MM-dd HH:mm");
                    PreTimeSpan = bt.TimeSpan;
                    PreBTReason = bt.BTReason;
                    PreValid = Convert.ToBoolean(bt.Valid);
                    PreRemark = bt.Remark;
                    PreCreaterName = bt.CreaterName;
                    PreCreateTime = Convert.ToDateTime(bt.CreateTime);
                    PreEditorName = bt.EditorName;
                    PreEditeTime = Convert.ToDateTime(bt.EditeTime);
                    PreOperationListID = bt.OperationListID;
                    PreLastID = bt.LastID;
                    PreBTEdit = Convert.ToBoolean(bt.BTEdit);
                    PreBTDelete = Convert.ToBoolean(bt.BTDelete);
                    PreEditOrDelete = bt.EditOrDelete;

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

        public string PreBTPlace
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

        public string PreTimeSpan
        {
            get;
            set;
        }

        public string PreBTReason
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
                    return db.T_HR_BusinessTrip.Find(PreID).OperationListID;
                }
            }
        }

        public string PreLastID
        {
            get;
            set;
        }

        public bool PreBTEdit
        {
            get;
            set;
        }

        public bool PreBTDelete
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
        /// <param name="id">V_HR_BTWithDepName的ID</param>
        /// <returns>异常返回null</returns>

        public BTApply GetCheckBusinessTrip(string id)
        {
            V_HR_BTWithDepName bt;
            //V_CH_TaskOperation task;
            LoginUser user = new LoginUser();
            ID = id;
            using (Entities db = new Entities())
            {
                bt = db.V_HR_BTWithDepName.Find(id);
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == GetOperationListID && o.StaffID == user.EmployeeId && o.Expire == false
                           select o;
                //task = db.V_CH_TaskOperation.Where(l => l.OperationID == GetOperationListID && l.StaffID == user.EmployeeID && l.Expire == false).ToList().FirstOrDefault();

                if (bt != null && task.Any())
                {
                    ID = bt.ID;
                    StaffID = bt.StaffID;
                    Name = bt.Name;
                    Department = bt.Department;
                    BTPlace = bt.BTPlace;
                    StartTime = Convert.ToDateTime(bt.StartTime);
                    EndTime = Convert.ToDateTime(bt.EndTime);
                    StartTimeStr = StartTime.ToString("yyyy-MM-dd HH:mm");
                    EndTimeStr = EndTime.ToString("yyyy-MM-dd HH:mm");
                    TimeSpan = bt.TimeSpan;
                    BTReason = bt.BTReason;
                    Valid = Convert.ToBoolean(bt.Valid);
                    Remark = bt.Remark;
                    CreaterName = bt.CreaterName;
                    CreateTime = Convert.ToDateTime(bt.CreateTime);
                    EditorName = bt.EditorName;
                    EditeTime = Convert.ToDateTime(bt.EditeTime);
                    OperationListID = bt.OperationListID;
                    LastID = bt.LastID;
                    BTEdit = Convert.ToBoolean(bt.BTEdit);
                    BTDelete = Convert.ToBoolean(bt.BTDelete);
                    EditOrDelete = bt.EditOrDelete;

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
        /// <param name="id">V_HR_BTWithDepName的ID</param>
        /// <returns>异常返回null</returns>

        public BTApply GetCheckPreBusinessTrip(string id)
        {
            V_HR_BTWithDepName bt;
            //V_CH_TaskOperation task;
            LoginUser user = new LoginUser();
            PreID = id;
            using (Entities db = new Entities())
            {
                bt = db.V_HR_BTWithDepName.Find(id);
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == GetPreOperationListID && o.StaffID == user.EmployeeId && o.Expire == false
                           select o;
                //task = db.V_CH_TaskOperation.Where(l => l.OperationID == GetOperationListID && l.StaffID == user.EmployeeID && l.Expire == false).ToList().FirstOrDefault();

                if (bt != null && task.Any())
                {
                    PreID = bt.ID;
                    PreStaffID = bt.StaffID;
                    PreName = bt.Name;
                    PreDepartment = bt.Department;
                    PreBTPlace = bt.BTPlace;
                    PreStartTime = Convert.ToDateTime(bt.StartTime);
                    PreEndTime = Convert.ToDateTime(bt.EndTime);
                    PreStartTimeStr = PreStartTime.ToString("yyyy-MM-dd HH:mm");
                    PreEndTimeStr = PreEndTime.ToString("yyyy-MM-dd HH:mm");
                    PreTimeSpan = bt.TimeSpan;
                    PreBTReason = bt.BTReason;
                    PreValid = Convert.ToBoolean(bt.Valid);
                    PreRemark = bt.Remark;
                    PreCreaterName = bt.CreaterName;
                    PreCreateTime = Convert.ToDateTime(bt.CreateTime);
                    PreEditorName = bt.EditorName;
                    PreEditeTime = Convert.ToDateTime(bt.EditeTime);
                    PreOperationListID = bt.OperationListID;
                    PreLastID = bt.LastID;
                    PreBTEdit = Convert.ToBoolean(bt.BTEdit);
                    PreBTDelete = Convert.ToBoolean(bt.BTDelete);
                    PreEditOrDelete = bt.EditOrDelete;

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
        public bool SubmitCheckBusinessTrip()
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