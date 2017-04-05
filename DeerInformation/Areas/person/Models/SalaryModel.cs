using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeerInformation.Models;
using Ext.Net.MVC;

namespace DeerInformation.Areas.person.Models
{
    public class SalaryModel
    {
        [Field(FieldLabel = "员工")]
        public string EmployeeId { get; set; }
        [Field(FieldLabel = "年份")]
        public int Year { get; set; }
        [Field(FieldLabel = "月份")]
        public int Month { get; set; }

        private List<Salary> _salarys;

        public List<Salary> Salarys
        {
            get
            {
                if (_salarys == null)
                {
                    using (Entities db = new Entities())
                    {
                        _salarys =
                            db.GetSalaryByYearMonth(EmployeeId,Year,Month)
                                .Select(l => new Salary() { SalayId = l.SalaryItemID, SalaryValue = l.SalaryValue })
                                .ToList();
                    }
                }
                return _salarys;
            }
            set { _salarys = value; }
        }

        public List<SalaryItem> SalaryItems
        {
            get
            {
                using (Entities db = new Entities())
                {
                   var allitems=  db.T_HR_SalaryItems.Where(l => l.IsValid == true).Select(all =>new SalaryItem()
                            {Id = all.ID, Description = all.Description, Name = all.Name }
                        ).ToList();
                    foreach (SalaryItem t in allitems)
                    {
                        var findResult = Salarys.FirstOrDefault(l => l.SalayId == t.Id);
                        if (findResult!=null)
                        {
                            t.Value = findResult.SalaryValue;
                        }
                    }
                    return allitems;
                }
            }
        }

        public bool UpdateSalarys()
        {
            using (Entities db=new Entities())
            {
                try
                {
                    var data =
                        db.T_HR_Salary.Where(l => l.EmployeeID == EmployeeId && l.Year == Year && l.Month == Month)
                            .ToList();
                    foreach (var item in Salarys)
                    {
                        var item1 = item;
                        var current = data.FirstOrDefault(l => l.SalaryItemID == item1.SalayId);
                        if (current!=null)
                        {
                            current.SalaryValue = item1.SalaryValue;
                        }
                        else
                        {
                            db.T_HR_Salary.Add(new T_HR_Salary()
                            {
                                EmployeeID = EmployeeId,
                                SalaryItemID = item1.SalayId,
                                SalaryValue = item1.SalaryValue,
                                Creator = new LoginUser().EmployeeId,
                                CreateTime = DateTime.Now,
                                Year = Year,
                                Month = Month
                            });
                        }

                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception )
                {
                    return false;
                }
                
            }
        }
    }

    public class SalaryItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }

        public string Description { get; set; }
    }

    public class Salary
    {
        public int SalayId { get; set; }

        public decimal SalaryValue { get; set; }
    }

}