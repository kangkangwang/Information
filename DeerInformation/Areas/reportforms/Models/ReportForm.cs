using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using  DeerInformation.Models;

namespace DeerInformation.Areas.reportforms.Models
{
	public class ReportForm
	{
		public DateTime Date { get; set; }

		public List<V_HR_StaffSalaryWithTime> GetSalaryWithTimes(DateTime dateTime)
		{
			using (Entities db = new Entities())
			{
				List<V_HR_StaffSalaryWithTime> result =
					db.V_HR_StaffSalaryWithTime.Where(l => l.Year == dateTime.Year && l.Month == dateTime.Month).ToList();
				return result;
			}
		}
	}
}