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
    
    public partial class T_HR_AttendanceExceptionHandleRecords
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public System.DateTime Date { get; set; }
        public System.DateTime BeforeTime { get; set; }
        public System.DateTime FinalTime { get; set; }
        public int ExceptionState { get; set; }
        public int PunchedOrder { get; set; }
        public string Editor { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public string Remark { get; set; }
    }
}
