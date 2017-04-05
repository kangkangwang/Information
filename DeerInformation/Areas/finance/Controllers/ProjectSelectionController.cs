using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Areas.finance.Models;
using Ext.Net;
using Ext.Net.MVC;

namespace DeerInformation.Areas.finance.Controllers
{
    public class ProjectSelectionController : Controller
    {
        //
        // GET: /finance/ProjectSelection/

        public ActionResult Index()
        {
            return View(new ProjectInfo().ProjectList);
        }

        public ActionResult Select(string name)
        {
            this.GetCmp<Store>("storedata").LoadData(new ProjectInfo() { Keyword = name }.ProjectList);
            return this.Direct();
        }


    }
}
