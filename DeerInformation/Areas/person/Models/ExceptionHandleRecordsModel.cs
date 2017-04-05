using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using DeerInformation.Models;

namespace DeerInformation.Areas.person.Models
{
    public class ExceptionHandleRecordsModel
    {
        public List<V_HR_ExceptionHandleRecordsWithDetail> Select(string date, string name)
        {
            DateTime? keyDate = null;
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
            {
                keyDate = dateTime;
            }
            using (Entities db = new Entities())
            {
                string fitformat = string.Format("%{0}%", name == null ? "" : name.Trim());
                return db.V_HR_ExceptionHandleRecordsWithDetail.Where(l => (EntityFunctions.DiffDays(keyDate, l.Date) ?? 0) == 0
                    && (SqlFunctions.PatIndex(fitformat, l.EmployeeName) > 0 || SqlFunctions.PatIndex(fitformat, l.EmployeeID) > 0)).ToList();
            }
        }
    }
}