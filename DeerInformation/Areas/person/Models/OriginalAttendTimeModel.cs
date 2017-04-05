using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using DeerInformation.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace DeerInformation.Areas.person.Models
{
    public class OriginalAttendTimeModel
    {
        public List<V_HR_AttendTimeOriginalWithName> Select(string date, string name)
        {
            DateTime? keyDate = null;
            DateTime dateTime;
            if (DateTime.TryParse(date,out dateTime))
            {
                keyDate = dateTime;
            }
            using (Entities db = new Entities())
            {
                string fitformat = string.Format("%{0}%", name == null ? "" : name.Trim());
                return db.V_HR_AttendTimeOriginalWithName.Where(l => (EntityFunctions.DiffDays(keyDate, l.AttendTime) ?? 0) == 0
                    && (SqlFunctions.PatIndex(fitformat, l.Name) > 0 || SqlFunctions.PatIndex(fitformat, l.UserId) > 0)).ToList();
            }
        }
    }
}