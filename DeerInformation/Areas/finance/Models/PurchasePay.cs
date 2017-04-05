using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using DeerInformation.Models;
using System.Data.Objects;
using Ext.Net.MVC;

namespace DeerInformation.Areas.finance.Models
{
    public class PurchasePay
    {

        public string ID { get; set; }
        [Field(FieldLabel = "采购单号")]
        public string PurchaseMaterialNo { get; set; }
        [Field(FieldLabel = "项目编号")]
        public string ProjectID { get; set; }
        [Field(FieldLabel = "项目名称")]
        public string ProjectName { get; set; }
        [Field(FieldLabel = "供应商编号")]
        public string SupplierID { get; set; }
        [Field(FieldLabel = "供应商名称")]
        public string SupplierName { get; set; }
        [Field(FieldLabel = "付款状态")]
        public string PayState { get; set; }
        [Field(FieldLabel = "应付总额")]
        public decimal PayAmount { get; set; }
        [Field(FieldLabel = "经办人")]
        public string Creator { get; set; }
        [Field(FieldLabel = "日期")]
        public DateTime CreateDate { get; set; }
        [Field(FieldLabel = "备注")]
        public string Remark { get; set; }
        [Field(FieldLabel = "附件")]
        public string Attachment { get; set; }

        [Field(FieldLabel = "发票状态")]
        public bool InvoiceState { get; set; }
        [Field(FieldLabel = "发票总额")]
        public decimal InvoiceAmount { get; set; }
        [Field(FieldLabel = "发票单号")]
        public string InvoiceNo { get; set; }
		[Field(FieldLabel = "实付金额")]
		public decimal? RealPayAmount { get; set; }
        public IEnumerable<V_FD_PurchasePayable> PurchaseList
        {
            get
            {
                using (Entities db = new Entities())
                {
                    string purchase = string.Format("%{0}%", Keyword ?? "");
                    return db.V_FD_PurchasePayable.Where(l => SqlFunctions.PatIndex(purchase, l.PurchaseMaterialNo) > 0 || SqlFunctions.PatIndex(purchase, l.SupplierID) > 0).ToList();
                }
            }
        }
        public string Keyword
        {
            get;
            set;
        }



	    public List<V_FD_PurchasePayable> Select(string suppliername, string projectno, string confirmtime, string invoicestate, string paymentstate )
        {
            using (Entities db = new Entities())
            {
                string fitformats = string.Format("%{0}%", suppliername.Trim());
                string fitformatp = string.Format("%{0}%", projectno.Trim());
                DateTime dt = new DateTime();
                if (DateTime.TryParse(confirmtime, out dt))
                {
                    return db.V_FD_PurchasePayable.
                        Where(l => SqlFunctions.PatIndex(fitformats, l.SupplierName) > 0 &&
                                  SqlFunctions.PatIndex(fitformatp, l.ProjectID) > 0 &&
                                  l.PayState == paymentstate && l.InvoiceState == (invoicestate=="已收发票"?true :false)
                                  &&EntityFunctions.DiffDays(l.ConfirmTime, dt) == 0)
                        .ToList();
                }
                return db.V_FD_PurchasePayable.
                    Where(l => SqlFunctions.PatIndex(fitformats, l.SupplierName) > 0 
                        && l.PayState == paymentstate && l.InvoiceState == (invoicestate == "已收发票" ? true : false)
                                  &&SqlFunctions.PatIndex(fitformatp, l.ProjectID) > 0)
                    .ToList();
            }
        }

        public bool UpdatePurchasePay(string type)
        {
            using (Entities db = new Entities())
            {
                try
                {
                    var OldPurchasepay = db.T_FD_AccountPayable.Find(ID);
                    if (OldPurchasepay != null)
                    {
                        OldPurchasepay.ID = ID;
                        OldPurchasepay.ProjectID = ProjectID;

                        OldPurchasepay.SupplierID = SupplierID;

                        OldPurchasepay.PayAmount = PayAmount;
                        OldPurchasepay.CreateDate = CreateDate;
                        OldPurchasepay.Creator = Creator;
                        OldPurchasepay.Remark = Remark;
                        OldPurchasepay.Attachment = Attachment;
                        if (type == "invoice")
                        {
                            OldPurchasepay.InvoiceState = InvoiceState;
                            OldPurchasepay.InvoiceNo = InvoiceNo;
                            OldPurchasepay.InvoiceAmount = InvoiceAmount;
                        }
                        else if (type == "payment")
                        {
                            OldPurchasepay.PayState = PayState;

                        }
	                    OldPurchasepay.RealPayAmount = RealPayAmount;
                        db.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }

        }

        public PurchasePay GetEditPurchasePay(string id)
        {
            T_FD_AccountPayable apayable;
            ID = id;
            using (Entities db = new Entities())
            {
                apayable = db.T_FD_AccountPayable.FirstOrDefault(l => l.ReceivePMNo == id);
                var expandPro = db.V_FD_PurchasePayable.FirstOrDefault(l => l.ReceivePMNo == id);
                if (apayable != null &&expandPro!=null)
                {
                    ID = apayable.ID;
                    PurchaseMaterialNo = expandPro.PurchaseMaterialNo;
                    SupplierID = apayable.SupplierID;
                    SupplierName = expandPro.SupplierName;
                    ProjectID = apayable.ProjectID;
                    ProjectName = expandPro.ProjectName;
                    PayAmount = apayable.PayAmount.decimaldata();
                    PayState = apayable.PayState;
                    CreateDate = DateTime.Now;
                    Creator = new LoginUser().EmployeeId;
                    Remark = apayable.Remark;
                    Attachment = apayable.Attachment;
                    InvoiceNo = apayable.InvoiceNo ?? "";
                    InvoiceState = apayable.InvoiceState ?? false;
                    InvoiceAmount = apayable.InvoiceAmount ?? 0;
	                RealPayAmount = apayable.RealPayAmount;
                }
                else { return null; }
                return this;
            }
        }

        public decimal GetPayAmount(string receiptno)
        {
            using (Entities db = new Entities())
            {
                var amount = db.V_FD_PurchasePayable.Where(l => l.PurchaseMaterialNo == receiptno).ToList();
                return amount.Sum(l => l.PayAmount).decimaldata();
            }

        }
    }
}