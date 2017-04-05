using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeerInformation.Models;
using Ext.Net;

namespace DeerInformation.Areas.gyproject.Models
{
	public class MPurchase
	{
		public string MPurchaseNo { get; set; }

		public string SupplierName { get; set; }

		public IEnumerable<ListItem> MPurchaseNoLst
		{
			get
			{
				using (Entities entities=new Entities())
				{
					var item = entities.V_GM_MPurchase.Where(l => l.IsEnableNo == "审核通过").ToList();
					return item.Select(l => new ListItem(l.PurchaseMNo));
				}
			}
		}

		public DateTime SupplierAckDate { get; set; }

		public bool Refresh(string mpNo)
		{
			using (Entities entities=new Entities())
			{
				var item = entities.V_GM_MPurchase.FirstOrDefault(l => l.PurchaseMNo == mpNo);
				if (item == null) return false;
				MPurchaseNo = item.PurchaseMNo;
				SupplierName = item.SupplierName;
				DateTime? dateTime = entities.T_GM_PurchaseMaterial.First(l => l.PurchaseMNo == mpNo).SupplierAckDate;
				if (dateTime != null)
				{
					SupplierAckDate = dateTime.Value;
				}
				return true;
			}
		}

		public bool Save()
		{
			using (Entities db=new Entities())
			{
				try
				{
					var item = db.T_GM_PurchaseMaterial.FirstOrDefault(l => l.PurchaseMNo == MPurchaseNo);
					if (item == null) return false;
					item.SupplierAckDate = SupplierAckDate;
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
}