using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net;
using DeerInformation.Models;
using System.Text;
using System.Xml.Xsl;
using Newtonsoft.Json;
using System.Xml;
using System.Data.Entity.Infrastructure;
using DeerInformation.Areas.person.Models;
using System.Data;
using DeerInformation.BaseType;
using System.IO;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.person.Controllers
{
    public class TipsPeopleController : Controller
    {
        //
        // GET: /person/TipsPeople/
        Entities entities = new Entities();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            var item = from o in entities.V_HR_TipsPeople
                                    where o.Valid == true
                                    select o;
            V_HR_TipsPeople tp = new V_HR_TipsPeople();
            if(item.Any())
            {
                tp = item.First();
            }
            return View(tp);
        }

        public ActionResult SelectStaff()
        {
            Window win = new Window
            {

                ID = "window1",
                Title = "",
                Height = 450,
                Width = 700,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("SelectStaff", "SelectStaff", new { area = "", NUM = 1 }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.DealGetpersonWithSDep()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }
        [VisitAuthorize(Create = true)]
        public ActionResult EditPeople(V_HR_TipsPeople tp)
        {
            DirectResult r = new DirectResult();
            var last = from o in entities.V_HR_TipsPeople
                       where o.ID == tp.ID
                       select o;
            if(last.Any())
            {
                T_HR_TipsPeople lasttp = entities.T_HR_TipsPeople.Find(tp.ID);
                lasttp.Valid = false;
            }

            T_HR_TipsPeople newtp = new T_HR_TipsPeople();
            newtp.ID = Tool.ProduceSed64();
            newtp.StaffID = tp.StaffID;
            newtp.Valid = true;
            newtp.Remark = tp.Remark;
            newtp.CreaterName = new LoginUser().EmployeeId;
            newtp.CreateTime = DateTime.Now;

            entities.T_HR_TipsPeople.Add(newtp);

            try
            {
                entities.SaveChanges();
                r.Success = true;
                X.Msg.Alert("提示", "保存成功！").Show();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据保存失败！<br /> note:" + e.Message).Show();
                r.Success = false;
            }
            return r;

        }

    }
}
