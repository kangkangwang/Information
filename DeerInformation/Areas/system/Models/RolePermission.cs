using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeerInformation.Areas.system.Models
{
    public class RolePermission : funcmenu
    {
        public bool Read { get; set; }

        public bool Update { get; set; }

        public bool Create
        {
            get;
            set;
        }

        public bool Delete
        {
            get;
            set;
        }
    }
}