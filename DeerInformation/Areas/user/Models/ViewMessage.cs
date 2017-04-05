using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using DeerInformation.Models;

namespace DeerInformation.Areas.user.Models
{
    public class ViewUnreadMessage
    {
        public int id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public DateTime createtime { get; set; }
        public string url { get; set; }

    }

    public class ViewTask
    {

        public string type { set; get; }
        public DateTime createtime { set; get; }
        public DateTime endtime { set; get; }
        public string url { set; get; }
    }

    public class ViewMessage
    {
        public int id { get; set; }
        public string type { get; set; }
        public DateTime createtime { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public bool read { get; set; }

    }

    public class ViewCcMessage : ViewMessage
    {
        public new string type = "抄送消息";
    }
}