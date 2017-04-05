using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.BaseType;

namespace DeerInformation.Areas.person.Models
{
    public class SchListStaffModel
    {
        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Department
        {
            get;
            set;
        }
    }
}