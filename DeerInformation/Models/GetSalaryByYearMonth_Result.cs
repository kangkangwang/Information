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
    
    public partial class GetSalaryByYearMonth_Result
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public int SalaryItemID { get; set; }
        public decimal SalaryValue { get; set; }
        public string Creator { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
