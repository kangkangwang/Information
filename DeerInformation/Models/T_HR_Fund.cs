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
    
    public partial class T_HR_Fund
    {
        public string ID { get; set; }
        public string StaffID { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> Months { get; set; }
        public string Remark { get; set; }
        public string CreaterName { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string EditorName { get; set; }
        public Nullable<System.DateTime> EditeTime { get; set; }
        public Nullable<bool> Valid { get; set; }
    }
}
