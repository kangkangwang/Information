using System;
using System.Collections;
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
    public class CollectionPlan
    {
        public string ID { get; set; }

        [Field(FieldLabel = "项目编号")]
        public string ProjectID { get; set; }
		[Field(FieldLabel = "客户订单号")]
		public string  CustomerNo { get; set; }
        [Field(FieldLabel = "项目名称")]
        public string ProjectName { get; set; }
        [Field(FieldLabel = "订单编号")]
        public string BudgetID { get; set; }
        [Field(FieldLabel = "客户名称")]
        public string CustomerName { get; set; }
        [Field(FieldLabel = "合同总额")]
        public decimal ContactSum { get; set; }
        [Field(FieldLabel = "备注")]
        public string Remark { get; set; }
        [Field(FieldLabel = "附件")]
        public string Attachment { get; set; }
        [Field(FieldLabel = "创建人")]
        public string Creator { get; set; }
        [Field(FieldLabel = "创建时间")]
        public DateTime CreateTime { get; set; }

	    public List<ListItem> ProjectItems
	    {
		    get
		    {
			    using (Entities db=new Entities())
			    {
				    return db.V_GM_DetailProject.Select(l => new ListItem(){Text = l.ProjectNo}).ToList();
			    }
		    }
	    }

	    public IEnumerable<ListItem> PlanTypeItems
        {
            get
            {
                using (Entities db = new Entities())
                {
                    var items = db.T_FD_CollectionType.ToList();
                    return items.Select(item => new ListItem(item.CollectionTypeName, item.CollectionTypeName)).ToList();
                }
            }
        }

        public List<T_FD_CollectionPlan> Select(string customername, string projectid, string date)
        {
            using (Entities db = new Entities())
            {
                string fitformatCsNa = string.Format("%{0}%", customername.Trim());
                string fitformat = string.Format("%{0}%", projectid.Trim());
                DateTime dt = new DateTime();
                if (DateTime.TryParse(date, out dt))
                {
                    return db.T_FD_CollectionPlan.
                        Where(l =>
                                SqlFunctions.PatIndex(fitformat, l.ProjectID) > 0 &&
								SqlFunctions.PatIndex(fitformatCsNa, l.CustomerName) > 0 &&
                                EntityFunctions.DiffDays(l.CreateTime, dt) == 0)
                                .ToList();
                }
                return db.T_FD_CollectionPlan.
                    Where(l => SqlFunctions.PatIndex(fitformat, l.ProjectID) > 0 &&
							   SqlFunctions.PatIndex(fitformatCsNa, l.CustomerName) > 0)
                    .ToList();
            }
        }

        private List<DetailPlan> _DetailPlans;

        public List<DetailPlan> DetailPlans
        {
            get
            {
                if (_DetailPlans == null)
                {
                    using (Entities db = new Entities())
                    {
                        _DetailPlans = db.V_FD_DetailCollectionPlan.Where(l => l.ID == ID)
                            .Select(l =>new DetailPlan()
                                    {
                                        CollectionId = l.ID,
                                        CollectionType = l.CollectionType,
                                        CollectionRatio = l.CollectionRatio,
                                        CollectionAmount = l.CollectionAmount??0,
                                        ProjectSchedule = l.ProjectSchedule
                                    }).ToList();
                    }
                }
                return _DetailPlans;
            }
            set { _DetailPlans = value; }
        }



	    public bool Save(Controller controller, FileUtility fileUtility)
        {
            if (fileUtility.File != null)
            {
                if (!fileUtility.SavaData()) return false;
            }

            using (Entities db = new Entities())
            {
                try
                {
                    if (db.T_FD_CollectionPlan.FirstOrDefault(l => l.ProjectID == ProjectID) != null) return false;
                    T_FD_CollectionPlan collectionplan = new T_FD_CollectionPlan()
                    {
                        ID = Guid.NewGuid().ToString(),
                        CreateTime = DateTime.Now,
                        Creator = new LoginUser().EmployeeId,
                        ProjectName = ProjectName,
                        ProjectID = ProjectID,
						BudgetID = CustomerNo,
                        CustomerName=CustomerName,
                        ContactSum=ContactSum,
                        Attachment = Attachment,
                        Remark = Remark
                    };
                    db.T_FD_CollectionPlan.Add(collectionplan);
                    foreach (var detailplan in DetailPlans)
                    {
                        T_FD_DetailPlan plan = new T_FD_DetailPlan()
                        {
                            CollectionId = collectionplan.ID,
                            CollectionType = detailplan.CollectionType,
                            CollectionRatio = detailplan.CollectionRatio,
                            CollectionAmount = detailplan.CollectionAmount,
                            ProjectSchedule = detailplan.ProjectSchedule,
                            ID = Guid.NewGuid().ToString()
                        };
                        db.T_FD_DetailPlan.Add(plan);
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
        }

        public CollectionPlan Find(string id)
        {
            using (Entities db=new Entities())
            {
                var item = db.T_FD_CollectionPlan.Find(id);
                if (item == null) return null;
                ID = item.ID;
                ProjectID = item.ProjectID;
                ProjectName = item.ProjectName;
                BudgetID = item.BudgetID;
                CustomerName = item.CustomerName;
                ContactSum = item.ContactSum;
                Remark = item.Remark;
                Attachment = item.Attachment;
                Creator = item.Creator;
                CreateTime = item.CreateTime ?? DateTime.MinValue;
                return this;
            }
        }
    }

    public class DetailPlan
    {
        public string ID { get; set; }
        public string CollectionId { get; set; }
        public string ProjectSchedule { get; set; }
        public string CollectionType { get; set; }
        public string CollectionRatio { get; set; }
        public decimal CollectionAmount { get; set; }
        public string CollectionState { get; set; }
        public DateTime CollectionDate { get; set; }
        public string Remark { get; set; }
    }

    
}