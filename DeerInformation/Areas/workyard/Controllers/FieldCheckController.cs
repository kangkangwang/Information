using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Extensions;
using DeerInformation.Models;
using Ext.Net.MVC;

namespace DeerInformation.Areas.workyard.Controllers
{
    [DirectController(AreaName = "workyard")]
    public class FieldCheckController : Controller
    {
        //
        // GET: /workyard/FieldCheck/
        Entities DB = new Entities();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            var list = DB.T_GW_MarkInfo.ToList();
            foreach (var item in list)
            {
                if (item.HandlePicPath != null)
                {
                    item.HandlePicPath = Url.Content(item.HandlePicPath);
                }
                if (item.ExcPicPath != null)
                {
                    item.ExcPicPath = Url.Content(item.ExcPicPath);
                }
            }
            return View(list);
        }

        public List<T_GW_MarkInfo> SetUrl(List<T_GW_MarkInfo> list)
        {
            foreach (var item in list)
            {
                if (item.HandlePicPath != null)
                {
                    item.HandlePicPath = Url.Content(item.HandlePicPath);
                }
                if (item.ExcPicPath != null)
                {
                    item.ExcPicPath = Url.Content(item.ExcPicPath);
                }
            }
            return list;
        }

        public ActionResult Search(string materialname, string handle)
        {
            List<T_GW_MarkInfo> list = new List<T_GW_MarkInfo>();
            if (materialname != "" && handle != "null")
            {
                list = DB.T_GW_MarkInfo.Where(w => w.ProjectNo.Contains(materialname)).Where(w => w.IsHandled.Contains(handle)).ToList();
                return this.Store(SetUrl(list));
            }
            else if (materialname == "" && handle != "null")
            {
                list = DB.T_GW_MarkInfo.Where(w => w.IsHandled.Contains(handle)).ToList();
                return this.Store(SetUrl(list));
            }
            else if (materialname == "" && handle == "null")
            {
                list = DB.T_GW_MarkInfo.Where(w => w.ProjectNo.Contains(materialname)).ToList();
                return this.Store(SetUrl(list));
            }
            else
            {
                list = DB.T_GW_MarkInfo.ToList();
                return this.Store(SetUrl(list));
            }
        }

    }
}
