using DeerInformation.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;

namespace DeerInformation.Areas.reportforms.Models
{
	public class MaterialGather
	{
		//获取项目订单的所以采购材料的汇总表
		public List<MaterialItem> GetMaterialItems(string projectNo)
		{
            using (Entities db = new Entities())
            {
                var result = db.P_GM_MaterialGather(projectNo).ToList();
                List<MaterialItem> li = new List<MaterialItem>();
                foreach (var item in result)
                {
                    MaterialItem mi = new MaterialItem();
                    mi.Name = item.MaterialName;
                    mi.Format = item.Size;
                    mi.Brand = item.Brand;
                    mi.Unit = item.Unit;
                    mi.Productor = item.Productor;
                    mi.Amount = item.Amount.Value;
                    mi.Price = item.Price.Value;
                    mi.Sum = mi.Amount * mi.Price;
                    li.Add(mi);
                }
                return li;

            } 
		}
	}

	public class MaterialItem
	{
		public string Name { get; set; }

		public string Format { get; set; }

		public string Brand { get; set; }

		public string Unit { get; set; }

		public Decimal Amount { get; set; }

		public Decimal Price { get; set; }

		public Decimal Sum { get; set; }

		public string Productor { get; set; }

		public string Remark { get; set; }
	}
}