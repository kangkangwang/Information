using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.BaseType;


namespace DeerInformation.Areas.finance.Models
{
    public class PaymentApply
    {
        [Field(FieldLabel = "收款单编号")]
        public string ID
        {
            get;
            set;
        }
        [Field(FieldLabel = "客户名称")]
        public string CustomerName
        {
            get;
            set;
        }
        [Field(FieldLabel = "项目编号")]
        public string ProjectID
        {
            get;
            set;
        }
        [Field(FieldLabel = "项目名称")]
        public string ProjectName
        {
            get;
            set;
        }
        [Field(FieldLabel = "订单号")]
        public string BudgetID
        {
            get;
            set;
        }
        [Field(FieldLabel = "施工进度")]
        public string CompletionPercent
        {
            get;
            set;
        }
        [Field(FieldLabel = "是否开发票")]
        public bool InvoiceOrnot
        {
            get;
            set;
        }
        [Field(FieldLabel = "发票编号")]
        public string InvoiceNum
        {
            get;
            set;
        }
        [Field(FieldLabel = "发票种类")]
        public string InvoiceType
        {
            get;
            set;
        }
        [Field(FieldLabel = "发票总金额")]
        public decimal InvoiceAmount
        {
            get;
            set;
        }
        [Field(FieldLabel = "订单总金额")]
        public decimal BudgetSum
        {
            get;
            set;
        }
        [Field(FieldLabel = "应收金额")]
        public decimal HaveReceive
        {
            get;
            set;
        }
        [Field(FieldLabel = "剩余金额")]
        public decimal? RestAmount
        {
            get;
            set;
        }

        [Field(FieldLabel = "创建人")]
        public string Creator
        {
            get;
            set;
        }
        [Field(FieldLabel = "日期")]
        public DateTime CreateDate
        {
            get;
            set;
        }
        [Field(FieldLabel = "备注")]
        public string Remark
        {
            get;
            set;
        }
        [Field(FieldLabel = "收款状态")]
        public string PaymentState
        {
            get;
            set;
        }
        public string AnnetPath
        {
            get;
            set;
        }

		[Field(FieldLabel = "实收金额")]
	    public decimal? RealReceiveAmount { get; set; }

        public IEnumerable<ListItem> InvoiceTypeItems
        {
            get
            {
                using (Entities db = new Entities())
                {
                    var items = db.T_FD_InvoiceType.ToList();
                    return items.Select(item => new ListItem(item.Name, item.Name)).ToList();
                }
            }
        }



	    //modle转为数据库类型
        public T_FD_AccountReceivable ToT_FD_AccountReceivable()
        {
            T_FD_AccountReceivable res = new T_FD_AccountReceivable();
            res.ID = ID;
            res.CustomerName = CustomerName;
            res.ProjectID = ProjectID;
            res.ProjectName = ProjectName;
            res.BudgetID = BudgetID;
            res.CompletionPercent = CompletionPercent;
            res.InvoiceOrnot = InvoiceOrnot;
            res.InvoiceType = InvoiceType;
            res.InvoiceAmount = InvoiceAmount;
            res.InvoiceNum = InvoiceNum;
            res.BudgetSum = BudgetSum;
            res.HaveReceive = HaveReceive;
            res.RestAmount = RestAmount;
            res.Creator = Creator;
            res.CreateDate = CreateDate;
            res.PaymentState = PaymentState;
            res.Remark = Remark;
            res.AnnetPath = AnnetPath;
	        res.RealReceiveAmount = RealReceiveAmount;
            return res;
        }

		public List<T_FD_AccountReceivable> Select(string customername, string projectNo, string paymentstate, string date)
        {
            using (Entities db = new Entities())
            {
                string fitformatc = string.Format("%{0}%", customername.Trim());
				string fitformat = string.Format("%{0}%", projectNo.Trim());
                string fitformats = string.Format("%{0}%", paymentstate.Trim());
                DateTime dt = new DateTime();
                if (DateTime.TryParse(date, out dt))
                {
                    return db.T_FD_AccountReceivable.
                        Where(l =>
                                SqlFunctions.PatIndex(fitformat, l.ProjectID) > 0 && 
                                SqlFunctions.PatIndex(fitformatc, l.CustomerName) > 0 &&
                                SqlFunctions.PatIndex(fitformats, l.PaymentState) > 0 &&
                                EntityFunctions.DiffDays(l.CreateDate, dt) >= 0)
                        .ToList();
                }
                return db.T_FD_AccountReceivable.
					Where(l => SqlFunctions.PatIndex(fitformat, l.ProjectID) > 0 &&
                               SqlFunctions.PatIndex(fitformats, l.PaymentState) > 0 &&
                               SqlFunctions.PatIndex(fitformatc, l.CustomerName) > 0)
                    .ToList();
            }
        }

        public bool CreatePaymentApply(Controller controller, FileUtility attachfile)
        {
            if (attachfile.File != null)
            {
                if (!attachfile.SavaData())
                    return false;
            }

            using (Entities db = new Entities())
            {
                ID = Guid.NewGuid().ToString();
                try
                {
                    db.T_FD_AccountReceivable.Add(ToT_FD_AccountReceivable());
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool UpdatePaymentApply()
        {

            using (Entities db = new Entities())
            {
                try
                {
                    var paymentBefore = db.T_FD_AccountReceivable.Find(ID);
                    if (paymentBefore != null)
                    {
                        paymentBefore.CustomerName = CustomerName;
                        paymentBefore.ProjectID = ProjectID;
                        paymentBefore.ProjectName = ProjectName;
                        paymentBefore.BudgetID = BudgetID;
                        paymentBefore.CompletionPercent = CompletionPercent;
                        paymentBefore.InvoiceOrnot = InvoiceOrnot;
                        paymentBefore.InvoiceType = InvoiceType;
                        paymentBefore.InvoiceAmount = InvoiceAmount;
                        paymentBefore.InvoiceNum = InvoiceNum;
                        paymentBefore.BudgetSum = BudgetSum;
                        paymentBefore.HaveReceive = HaveReceive;
                        paymentBefore.RestAmount = RestAmount;
                        paymentBefore.Creator = Creator;
                        paymentBefore.CreateDate = CreateDate;
                        paymentBefore.PaymentState = PaymentState;
                        paymentBefore.Remark = Remark;
	                    paymentBefore.RealReceiveAmount = RealReceiveAmount;
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 返回付款申请详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PaymentApply GetDetailPaymentApply(string id)
        {
            T_FD_AccountReceivable pay;
            ID = id;
            using (Entities db = new Entities())
            {
                pay = db.T_FD_AccountReceivable.Find(id);
                if (pay != null)
                {
                    ID = pay.ID;
                    CustomerName = pay.CustomerName;
                    ProjectID = pay.ProjectID;
                    ProjectName = pay.ProjectName;
                    BudgetID = pay.BudgetID;
                    CompletionPercent = pay.CompletionPercent;
                    InvoiceOrnot = pay.InvoiceOrnot;
                    InvoiceType = pay.InvoiceType;
                    InvoiceAmount = pay.InvoiceAmount;
                    BudgetSum = pay.BudgetSum;
                    HaveReceive = pay.HaveReceive;
                    RestAmount = pay.RestAmount;
                    Creator = pay.Creator;
                    CreateDate = pay.CreateDate;
                    PaymentState = pay.PaymentState;
                    Remark = pay.Remark;
                    AnnetPath = pay.AnnetPath;
	                RealReceiveAmount = pay.RealReceiveAmount;
                }
                else
                {
                    return null;
                }

                return this;
            }
        }

        public PaymentApply GetEditPaymentApply(string id)
        {
            LoginUser user = new LoginUser();
            ID = id;
            using (Entities db = new Entities())
            {
                var pay = db.T_FD_AccountReceivable.Find(id);
                if (pay == null) return null;
                ID = pay.ID;
                CustomerName = pay.CustomerName;
                ProjectID = pay.ProjectID;
                ProjectName = pay.ProjectName;
                BudgetID = pay.BudgetID;
                CompletionPercent = pay.CompletionPercent;
                InvoiceOrnot = pay.InvoiceOrnot;
                InvoiceNum = pay.InvoiceNum;
                InvoiceType = pay.InvoiceType;
                InvoiceAmount = pay.InvoiceAmount;
                BudgetSum = pay.BudgetSum;
                HaveReceive = pay.HaveReceive;
                RestAmount = pay.RestAmount;
                Creator = pay.Creator;
                CreateDate = pay.CreateDate;
                PaymentState = pay.PaymentState;
                Remark = pay.Remark;
                AnnetPath = pay.AnnetPath;
	            RealReceiveAmount = pay.RealReceiveAmount;
                return this;
            }
        }

        public decimal GetPaidAmount(string projectno)
        {
            using (Entities db = new Entities())
            {
                var content = db.T_FD_AccountReceivable.Where(l => l.ProjectID == projectno).ToList();
                return content.Sum(l => l.HaveReceive);
            }
        }
    }
}