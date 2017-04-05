using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Areas.user.Models;
using Ext.Net;
using Ext.Net.MVC;
using  DeerInformation.Extensions;

namespace DeerInformation.Areas.user.Controllers
{
    [UserAuthorize]
    [VisitAuthorize]
    public class ModifyDataController : Controller
    {
        //
        // GET: /user/ModifyDate/
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View(new UserData().GetCurrentData());
        }

        //
        // POST: /user/ModifyDate/Edit/5

        [HttpPost]
        public ActionResult Save(UserData data)
        {
            try
            {
                FileUploadField upload = this.GetCmp<FileUploadField>("Userimage");

                data.UserImageFile = upload.PostedFile;

                if (upload.HasFile)
                {
                    data.UserImage = string.Format("~/Images/{0}/{1}.{2}", data.UserName, Guid.NewGuid().ToString(),
                        Path.GetExtension(data.UserImageFile.FileName));
                    data.UserImageSavePath =
                        Server.MapPath(data.UserImage);
                    data.UserImageFileType = data.UserImageFile.ContentType;

                }
                if (data.SavaData()) X.MessageBox.Notify("消息", "信息保存成功！").Show();
                else X.MessageBox.Notify("消息", "信息保存失败！").Show();

            }
            catch
            {
                X.MessageBox.Notify("消息", "信息保存失败！").Show();
            }
            return this.Direct();
        }

        public ActionResult Password()
        {
            return View(new UserData());
        }

        public ActionResult SavePassword(UserData data)
        {
            try
            {
                if (data.SavePassword()) X.MessageBox.Notify("消息", "信息保存成功！").Show();
                else X.MessageBox.Notify("消息", "信息保存失败！").Show();

            }
            catch
            {
                X.MessageBox.Notify("消息", "信息保存失败！").Show();
            }
            return this.Direct();
        }
    }
}
