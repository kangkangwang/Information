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
    public class OWApply
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

        public decimal TimeSpan
        {
            get;
            set;
        }

        public decimal CheckTimeSpan
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

        public string FuncName
        {
            get { return "可调休加班时数申请"; }
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
                    return db.T_HR_OverWorkApply.Find(ID).OperationListID;
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

        public OWApply()
        {
            TimeSpan = 0;
            CheckTimeSpan = 0;
        }

        /// <summary>
        /// OWApply对象转为数据库对象
        /// </summary>
        /// <param name="flag">标志，1为添加，2为修改需要复制修改人信息</param>
        /// <returns>数据库对象</returns>

        public T_HR_OverWorkApply ToDB(int flag)
        {
            T_HR_OverWorkApply di = new T_HR_OverWorkApply();
            di.ID = ID;
            di.StaffID = StaffID;
            di.StartTime = StartTime;
            di.EndTime = EndTime;
            di.TimeSpan = TimeSpan;
            di.CheckTimeSpan = CheckTimeSpan;
            di.Valid = Valid;
            di.Remark = Remark;
            di.CreaterName = CreaterName;
            di.CreateTime = CreateTime;
            di.OperationListID = OperationListID;
            if (flag != 1)
            {
                di.EditorName = EditorName;
                di.EditeTime = EditeTime;
            }

            return di;
        }

        /// <summary>
        /// 获取审核对象
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>异常返回null</returns>

        public OWApply GetCheckOW(string id)
        {
            V_HR_OWApplyWithDepName di;
            //V_CH_TaskOperation task;
            LoginUser user = new LoginUser();
            ID = id;
            using (Entities db = new Entities())
            {
                di = db.V_HR_OWApplyWithDepName.Find(id);
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == GetOperationListID && o.StaffID == user.EmployeeId && o.Expire == false
                           select o;
                //task = db.V_CH_TaskOperation.Where(l => l.OperationID == GetOperationListID && l.StaffID == user.EmployeeID && l.Expire == false).ToList().FirstOrDefault();

                if (di != null && task.Any())
                {
                    ID = di.ID;
                    StaffID = di.StaffID;
                    Name = di.Name;
                    Department = di.Department;
                    StartTime = Convert.ToDateTime(di.StartTime);
                    EndTime = Convert.ToDateTime(di.EndTime);
                    StartTimeStr = StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                    EndTimeStr = EndTime.ToString("yyyy-MM-dd HH:mm:ss");
                    TimeSpan = Convert.ToDecimal(di.TimeSpan);
                    CheckTimeSpan = Convert.ToDecimal(di.TimeSpan);
                    Valid = Convert.ToBoolean(di.Valid);
                    Remark = di.Remark;
                    CreaterName = di.CreaterName;
                    CreateTime = Convert.ToDateTime(di.CreateTime);
                    EditorName = di.EditorName;
                    EditeTime = Convert.ToDateTime(di.EditeTime);
                    OperationListID = di.OperationListID;

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
        public bool SubmitCheckOWApply()
        {
            using (Entities db = new Entities())
            {
                LoginUser user = new LoginUser();
                string operationId = GetOperationListID;
                string checkerId = user.EmployeeId;
                int? state = State;
                string description = DescriptionForCheck;

                var result = db.P_CH_ExecuteCheck(operationId, checkerId, state, description).ToList();
                if (result[0] != 1)//0表失败 1表成功
                {
                    return false;
                }
                return true;
            }

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

        public OWApply GetOWDetail(string id)
        {
            V_HR_OWApplyWithDepName di;
            ID = id;
            using (Entities db = new Entities())
            {
                di = db.V_HR_OWApplyWithDepName.Find(id);
                T_CH_Operation_list op = db.T_CH_Operation_list.Find(GetOperationListID);
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == GetOperationListID
                           select o;

                if (di != null && task.Any())
                {
                    ID = di.ID;
                    StaffID = di.StaffID;
                    Name = di.Name;
                    Department = di.Department;
                    StartTime = Convert.ToDateTime(di.StartTime);
                    EndTime = Convert.ToDateTime(di.EndTime);
                    StartTimeStr = StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                    EndTimeStr = EndTime.ToString("yyyy-MM-dd HH:mm:ss");
                    TimeSpan = Convert.ToDecimal(di.TimeSpan);
                    CheckTimeSpan = Convert.ToDecimal(di.CheckTimeSpan);
                    Valid = Convert.ToBoolean(di.Valid);
                    Remark = di.Remark;
                    CreaterName = di.CreaterName;
                    CreateTime = Convert.ToDateTime(di.CreateTime);
                    EditorName = di.EditorName;
                    EditeTime = Convert.ToDateTime(di.EditeTime);
                    OperationListID = di.OperationListID;

                    State = op.State;
                    DescriptionForCheck = "";
                    foreach (V_CH_TaskOperation item in task)
                    {
                        DescriptionForCheck += item.Description;
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
        /// 获取是否可以审核，即Task的Expire属性不为true
        /// </summary>
        /// <param name="flag">ID</param>
        /// <returns>返回true表示可以审核</returns>
        public static bool GetExpire(string id)
        {
            bool flag = true;
            LoginUser user = new LoginUser();
            using (Entities db = new Entities())
            {
                var op = from o in db.T_HR_OverWorkApply
                         where o.ID == id
                         select o;
                string opid = op.First().OperationListID;
                var task = from o in db.V_CH_TaskOperation
                           where o.OperationID == opid && o.StaffID == user.EmployeeId
                           select o;
                if (task.Any())
                {
                    foreach (V_CH_TaskOperation item in task)
                    {
                        if (item.Expire)
                        {
                            flag = false;
                            break;
                        }
                    }
                    return flag;
                }
                else
                {
                    return flag;
                }
            }

        }



    }
}