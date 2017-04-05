using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeerInformation.Models;

namespace DeerInformation.Areas.reportforms.Models
{
	public class MaterialApply
	{
		public V_GM_MApply GetVgmMApplies(string applyId)
		{
			return null;
		}

		public List<V_GM_DM> GetApplyMaterialsList(string applyId)
		{
			using (Entities db=new Entities())
			{
                var result = db.V_GM_DM.Where(m => m.Remark == applyId).ToList();
				return result;
			}
		}
	}
}