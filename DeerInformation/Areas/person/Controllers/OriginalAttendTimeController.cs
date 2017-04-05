using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Areas.person.Models;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;

namespace DeerInformation.Areas.person.Controllers
{
    public class OriginalAttendTimeController : Controller
    {
        //
        // GET: /person/OriginalAttendTime/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fiter(StoreRequestParameters parameters,string date, string name="" )
        {
            return this.Store(new OriginalAttendTimeModel().Select(date,name).GetPage(parameters));
        }
        

    }
}
