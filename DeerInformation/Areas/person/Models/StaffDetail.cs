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
    public class StaffDetail
    {
        #region 基本信息
        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string TelNum
        {
            get;
            set;
        }

        public string Urgencyperson
        {
            get;
            set;
        }

        public string UPTelNum
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Nation
        {
            get;
            set;
        }

        public string HireState
        {
            get;
            set;
        }

        public string CertificateType
        {
            get;
            set;
        }

        public string CertificateNum
        {
            get;
            set;
        }

        public string IdCardAddress
        {
            get;
            set;
        }

        public string Nativeplace
        {
            get;
            set;
        }

        public DateTime Birthday
        {
            get;
            set;
        }

        public string Sex
        {
            get;
            set;
        }

        public string MaritalStatus
        {
            get;
            set;
        }

        public string StaffType
        {
            get;
            set;
        }

        public string WorkAddress
        {
            get;
            set;
        }

        public string Area
        {
            get;
            set;
        }

        public string BankAccount
        {
            get;
            set;
        }

        public string BankType
        {
            get;
            set;
        }

        public string Craft
        {
            get;
            set;
        }

        public string Education
        {
            get;
            set;
        }

        public string GraduationSchool
        {
            get;
            set;
        }

        public string Major
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

        public string ID1
        {
            get;
            set;
        }

        public string Department1No
        {
            get;
            set;
        }

        public string Department1Name
        {
            get;
            set;
        }

        public string ID2
        {
            get;
            set;
        }

        public string Department2No
        {
            get;
            set;
        }

        public string Department2Name
        {
            get;
            set;
        }

        public string ID3
        {
            get;
            set;
        }

        public string Department3No
        {
            get;
            set;
        }

        public string Department3Name
        {
            get;
            set;
        }

        public string ID4
        {
            get;
            set;
        }

        public string Department4No
        {
            get;
            set;
        }

        public string Department4Name
        {
            get;
            set;
        }

        public string ID5
        {
            get;
            set;
        }

        public string Department5No
        {
            get;
            set;
        }

        public string Department5Name
        {
            get;
            set;
        }

        public string Department
        {
            get;
            set;
        }

        public string ImagePath
        {
            get;
            set;
        }

        public string NoAttend
        {
            get;
            set;
        }
        #endregion

        #region  社保
        public string SocialInsuranceType
        {
            get;
            set;
        }

        public DateTime SSStartdate
        {
            get;
            set;
        }

        public DateTime SSEnddate
        {
            get;
            set;
        }
        #endregion

        #region 职位信息
        public string StaffJobID
        {
            get;
            set;
        }

        public string JobID
        {
            get;
            set;
        }

        public string JobName
        {
            get;
            set;
        }

        public string DutyID
        {
            get;
            set;
        }

        public string DutyName
        {
            get;
            set;
        }

        public string DutyType
        {
            get;
            set;
        }

        public string PositionCategoryID
        {
            get;
            set;
        }

        public string PositionCategoryName
        {
            get;
            set;
        }

        public string EntryPositionID
        {
            get;
            set;
        }

        public DateTime EntryTime
        {
            get;
            set;
        }
        #endregion

        #region 离职信息
        public DateTime PreDimissionDate
        {
            get;
            set;
        }

        public string DimissionType
        {
            get;
            set;
        }

        public string DimissionReason
        {
            get;
            set;
        }
        #endregion

        #region 合同信息
        public string ContractID
        {
            get;
            set;
        }

        public int Probation
        {
            get;
            set;
        }

        public DateTime ProbationDate
        {
            get;
            set;
        }

        public bool ProbationValid
        {
            get;
            set;
        }

        public int Num
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

        public decimal Years
        {
            get;
            set;
        }
        #endregion

        #region 基金
        public string FundID
        {
            get;
            set;
        }

        public bool FundValid
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EndDate
        {
            get;
            set;
        }

        public int Months
        {
            get;
            set;
        }
        #endregion

        #region 伯乐奖
        public string ReID
        {
            get;
            set;
        }

        public string ReStaffID
        {
            get;
            set;
        }

        public string ReName
        {
            get;
            set;
        }

        public string Relation
        {
            get;
            set;
        }

        public decimal Money
        {
            get;
            set;
        }

        public DateTime YearMonth
        {
            get;
            set;
        }

        public bool IsDe
        {
            get;
            set;
        }
        #endregion

        #region 证书
        public List<T_HR_Certificate> certificates;
        #endregion 

        public StaffDetail()
        {

        }


    }
}