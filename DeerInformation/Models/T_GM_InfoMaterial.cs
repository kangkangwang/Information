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
    
    public partial class T_GM_InfoMaterial
    {
        public decimal UID { get; set; }
        public string MaterialID { get; set; }
        public string MaterialName { get; set; }
        public Nullable<bool> IsEnable { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Extra { get; set; }
        public string InputPerson { get; set; }
        public Nullable<System.DateTime> InputTime { get; set; }
        public string PicturePath { get; set; }
        public string Brand { get; set; }
        public string PurchaseCycle { get; set; }
        public string MinPurchase { get; set; }
        public string CostType { get; set; }
    }
}