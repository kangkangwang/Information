using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using DeerInformation.Models;

namespace DeerInformation.Areas.finance.Models
{
    public class ProjectInfo
    {
        public IEnumerable<V_GM_DetailProject> ProjectList
        {
            get
            {
                using (Entities db=new Entities())
                {
                    string fitformat = string.Format("%{0}%", Keyword ?? "");
                    return db.V_GM_DetailProject.Where(l => SqlFunctions.PatIndex(fitformat, l.ClientName) > 0 || SqlFunctions.PatIndex(fitformat, l.ProjectName) > 0).ToList();
                }
            }
        }

        public string Keyword { get; set; }



    }
}