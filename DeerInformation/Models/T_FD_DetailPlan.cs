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
    
    public partial class T_FD_DetailPlan
    {
        public string ID { get; set; }
        public string CollectionId { get; set; }
        public string CollectionType { get; set; }
        public string ProjectSchedule { get; set; }
        public string CollectionRatio { get; set; }
        public Nullable<decimal> CollectionAmount { get; set; }
        public string CollectionState { get; set; }
        public Nullable<System.DateTime> CollectionDate { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> Sent { get; set; }
    }
}
