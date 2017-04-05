using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Areas.user.Models;
using DeerInformation.Models;
using Ext.Net.MVC;
using Ext.Net;

namespace DeerInformation.Areas.user.Controllers
{
	[DirectController(AreaName = "user")]
	public class WorkPlatformController : Controller
	{
		//
		// GET: /user/WorkPlatform/

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult GetCalendarData()
		{
            return this.Direct(WorkPlatform.GetCalendarData());
		}

        public ActionResult GetUnreadMessage()
		{
            return this.Direct(WorkPlatform.GetUnreadMessage());
		}

		public ActionResult GetCheck()
		{
			return this.Direct(WorkPlatform.GetCheck());
		}

		public ActionResult GetWorkreport()
		{
			return this.Direct(WorkPlatform.GetWorkreport());
		}

		public ActionResult TaskCheck()
		{
			return View();
		}

        public ActionResult GetTask(StoreRequestParameters parameters)
		{
            return this.Store(WorkPlatform.GetTask().GetPage(parameters));
		}

        public ActionResult Message()
        {
            return View();
        }

        public ActionResult GetMessage(StoreRequestParameters parameters)
        {
            return this.Store(WorkPlatform.GetMessage().GetPage(parameters));
        }

	    public ActionResult CcMessage()
	    {
            return View();
        }

        public ActionResult GetCcMessage(StoreRequestParameters parameters)
        {
            return this.Store(WorkPlatform.GetCcMessage().GetPage(parameters));
        }

        [DirectMethod(Namespace = "workplatform")]
		public ActionResult SetRead(string id)
		{
			T_US_Message message = WorkPlatform.ReadMessage(id);
	        if (message == null || string.IsNullOrEmpty(message.Url)) return this.Direct();
	        if (message.NewWindow != null && message.NewWindow == true)
	        {
		        X.AddScript(string.Format("parent.Home.NewWindow('{0}','{1}')", message.Url, message.Title));
		        Session.Add("NewWindow",true);
	        }
	        else
	        {
		        X.AddScript(string.Format("location.href='{0}'", message.Url));
	        }
	        return this.Direct();
		}

	}
}
