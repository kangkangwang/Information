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
using System.IO;
using DeerInformation.BaseType;

namespace DeerInformation.Areas.person.Controllers
{
    [DirectController(AreaName = "person")]
    public class BasicController : Controller
    {
        Entities entities = new Entities();

        //public ActionResult Index()
        //{
        //    TempData["image"] = "~/Images/aa.jpg";
        //    return View();

        //}
        [HttpPost]
        public ActionResult Save()
        {
            FileUploadField upload = this.GetCmp<FileUploadField>("Userimage");
            BasicModel data = new BasicModel();
            data.UserImageFile = upload.PostedFile;

            if (upload.HasFile)
            {
                data.UserImage = string.Format("~/Images/{0}/{1}{2}", "HR", Guid.NewGuid().ToString(),
                    Path.GetExtension(data.UserImageFile.FileName));
                data.UserImageSavePath =
                    Server.MapPath(data.UserImage);
                data.UserImageFileType = data.UserImageFile.ContentType;

            }

            if (data.UserImageSavePath != null && Path.GetDirectoryName(data.UserImageSavePath) != null)
            {
                if (Directory.Exists(Path.GetDirectoryName(data.UserImageSavePath)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(data.UserImageSavePath));
                }

                data.UserImageFile.SaveAs(data.UserImageSavePath);
            }

            var s = X.GetCmp<Image>("UserImage");
            s.ImageUrl = data.UserImage;
            s.ReRender();
           
            var s1 = X.GetCmp<TextField>("p");
            s1.Text = data.UserImage;
            
            return this.Direct();
        }


        public ActionResult GetData()
        {
            var list = (from o in entities.V_HR_StaffWithDepName
                        where o.HireState == "在职"
                        select o).ToList();
            return this.Store(list);
        }


        public ActionResult SetIFrameLoadProperty()
        {
            string path = Server.MapPath("~/AttachFile/HR/Certificate/1/7e535201-2d64-4b59-9070-cf0023801aaf.jpg");
            if (System.IO.File.Exists(path))//Url.Content("~/AttachFile/HR/Certificate/1/88d2d300-b2ee-424a-9209-e82dcd2b023b.jpg")
            {
                var p = X.GetCmp<TextField>("p");
                p.Text = "zai";
                System.IO.File.Delete(path);
            }
            else
            {
                var p = X.GetCmp<TextField>("p");
                p.Text = "buzai";
            }
            return this.Direct();
        }

        public ActionResult Index()
        {
            return View(BasicModel.GetAllCompanies());
        }

        //public ActionResult Read()
        //{
        //    return this.Store(BasicModel.GetAllCompanies());
        //}

        public ActionResult Submit(SubmitHandler handler)
        {
            return this.File(new System.Text.UTF8Encoding().GetBytes(handler.Xml.OuterXml), "application/xml", "submittedData.xml");
        }

        



    }
}