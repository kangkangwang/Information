//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeerInformation.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_GM_Project
    {
        public string UID { get; set; }
        public string BudgetGID { get; set; }
        public string CustomerNo { get; set; }
        public string OfferStatus { get; set; }
        public string Extra { get; set; }
        public string AttachmentPath { get; set; }
        public string AgentMan { get; set; }
        public Nullable<System.DateTime> AgentDate { get; set; }
        public Nullable<System.DateTime> CusStart { get; set; }
        public Nullable<System.DateTime> CusEnd { get; set; }
        public Nullable<System.DateTime> ReciPODate { get; set; }
        public Nullable<decimal> PurchaseAmount { get; set; }
        public Nullable<System.DateTime> CusCmfDate { get; set; }
    }
}
