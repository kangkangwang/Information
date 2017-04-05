using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Data;
using Newtonsoft.Json;
using System.Xml.Xsl;
using System.Xml;
using System.Text;
using System.IO;

namespace DeerInformation.Areas.workyard.Controllers
{
    [DirectController(AreaName="workyard")]
    public class TestController : Controller
    {
        //
        // GET: /workyard/Test/
        Entities DB = new Entities();
        public ActionResult Index()
        {
            return View(DB.T_GW_MarkInfo.ToList());
        }

        public ActionResult TEST()
        {
            return View();
        }
        [DirectMethod]
        public ActionResult ClickSubmit(string mark, string extra, float x, float y)
        {
            var markinfo = new T_GW_MarkInfo();
            markinfo.Mark = mark;
            markinfo.Remark = extra;
            markinfo.Xaxes = x;
            markinfo.Yaxes = y;
            DB.T_GW_MarkInfo.Add(markinfo);
            DB.SaveChanges();
            return this.Direct();
        }

        public ActionResult UploadClick()
        { 
            var uploadfile = this.GetCmp<FileUploadField>("FileUploadField1").PostedFile;
            int filesize = Int32.Parse(uploadfile.ContentLength.ToString());
            string fileoldname = uploadfile.FileName;
            string filenewname = Path.GetFileNameWithoutExtension(fileoldname) + DateTime.Now.ToString(@"yyyyMMddHHmmss") + new Random().Next(1, 100).ToString()+Path.GetExtension(fileoldname);
            if (filesize>5*1024*1024)
            {
                X.Msg.Alert("提示", "上传文件过大，大小必须低于5M").Show();
            }
            string filepath = Server.MapPath("~/App_Data/" + filenewname);

            if (this.GetCmp<FileUploadField>("FileUploadField1").HasFile)
            {
                uploadfile.SaveAs(filepath);
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "Success",
                    Message = "文件上传成功"
                });
                
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "Fail",
                    Message = "No file uploaded"
                });
            }
            DirectResult result = new DirectResult();
            result.IsUpload = true;
            return result;
        }

    }
}
