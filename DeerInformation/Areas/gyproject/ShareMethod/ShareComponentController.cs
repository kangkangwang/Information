using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Data.Objects.SqlClient;
using System;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Areas.gyproject.ShareModule;

namespace DeerInformation.Areas.gyproject.ShareMethod
{
    public class ShareComponentController : Controller
    {
        //
        // GET: /gyproject/Component/
        public void ComponentFormPanel(string gid, string title, string condition, string containername = "projectm")
        {
            FormPanel p = new FormPanel()
            {
                Layout="auto",
                MinHeight = 10,
                Loader = new ComponentLoader()
                {
                    Url = Url.Action("ChooseM", "Share", new { con = condition, id = gid, tit = title }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                }
            };
            p.AddTo(this.GetCmp<Container>(containername));
            this.GetCmp<Container>(containername).SetActive(true);
        }

    }
}
