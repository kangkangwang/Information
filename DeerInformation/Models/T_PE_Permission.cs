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
    
    public partial class T_PE_Permission
    {
        public string PermissionID { get; set; }
        public string MenuID { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string Expand { get; set; }
        public string Creator { get; set; }
        public string CreatorID { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string Editor { get; set; }
        public string EditorID { get; set; }
        public Nullable<System.DateTime> EditeTime { get; set; }
    }
}