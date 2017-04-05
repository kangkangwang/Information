using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.IO;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.workyard.Controllers
{
    [DirectController(AreaName = "workyard")]
    public class FieldDataManagementController : Controller
    {
        //
        // GET: /workyard/FieldDataManagement/
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();

        [DirectMethod]
        public ActionResult FieldDataReload()//重新加载物资信息管理页面
        {
            var list = DB.T_GW_FieldDataManagement.ToList();
            foreach (var item in list)
            {
                if (item.AnnetPath != null)
                {
                    item.AnnetPath = Url.Content(item.AnnetPath);
                }
            }
            cw.Reload(list, "FieldDataManagement");
            return this.Direct();
        }
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            var list = DB.T_GW_FieldDataManagement.ToList();
            foreach (var item in list)
            {
                if (item.AnnetPath != null)
                {
                    item.AnnetPath = Url.Content(item.AnnetPath);
                }
            }
            return View(list);
        }

        public ActionResult FieldInquire(string addno, string addfield, string addrecordname)//精确查询
        {

            if (addno.Length > 0 && addno != "")
            {
                if (addfield != "" && addfield.Length > 0)
                {
                    if (addrecordname.Length > 0)
                    {
                        return this.Store(DB.T_GW_FieldDataManagement.Where(w => w.ProjectNo.Contains(addno)).Where(w => w.FieldName.Contains(addfield)).Where(w => w.AnnetName.Contains(addrecordname)).ToList());
                    }
                    else
                    {
                        return this.Store(DB.T_GW_FieldDataManagement.Where(w => w.ProjectNo.Contains(addno)).Where(w => w.FieldName.Contains(addfield)).ToList());
                    }
                }
                else
                {
                    if (addrecordname.Length > 0)
                    {
                        return this.Store(DB.T_GW_FieldDataManagement.Where(w => w.ProjectNo.Contains(addno)).Where(w => w.AnnetName.Contains(addrecordname)).ToList());
                    }
                    else
                    {
                        return this.Store(DB.T_GW_FieldDataManagement.Where(w => w.ProjectNo.Contains(addno)).ToList());
                    }
                }
            }
            else
            {
                if (addfield != "" && addfield.Length > 0)
                {
                    if (addrecordname.Length > 0)
                    {
                        return this.Store(DB.T_GW_FieldDataManagement.Where(w => w.FieldName.Contains(addfield)).Where(w => w.AnnetName.Contains(addrecordname)).ToList());
                    }
                    else
                    {
                        return this.Store(DB.T_GW_FieldDataManagement.Where(w => w.FieldName.Contains(addfield)).ToList());
                    }
                }
                else
                {
                    if (addrecordname.Length > 0)
                    {
                        return this.Store(DB.T_GW_FieldDataManagement.Where(w => w.AnnetName.Contains(addrecordname)).ToList());
                    }
                    else
                    {
                        return this.Store(DB.T_GW_FieldDataManagement.ToList());
                    }
                }
            }
        }

        [DirectMethod]
        [VisitAuthorize(Delete = true)]
        public ActionResult ClickDelete(string id)
        {
            decimal id1 = 0;
            decimal.TryParse(id, out id1);
            var record = DB.T_GW_FieldDataManagement.Find(id1);
            DB.T_GW_FieldDataManagement.Remove(record);
            DB.SaveChanges();
            return this.Direct();
        }

        #region 添加附件信息
        public ActionResult FieldDataAddView()
        {
            ViewBag.prono = cw.SetCombox_PNo();
            return View();
        }

        [VisitAuthorize(Create = true)]
        public ActionResult FieldDataAddButton()
        {
            WinModule win = new WinModule();
            win.ID = "window2";
            win.Title = "现场资料添加";
            win.Height = 200;
            win.Width = 400;
            win.Loader.Url = Url.Action("FieldDataAddView", "FieldDataManagement");
            win.Listeners.Close.Handler = "App.direct.workyard. FieldDataReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult FieldDataAdd(T_GW_FieldDataManagement fdm)
        {
            fdm.UploadMan = user.EmployeeName;
            fdm.UploadTime = DateTime.Now;

            var uploadfile = this.GetCmp<FileUploadField>("AttachmentPath");
            int filesize = uploadfile.PostedFile.ContentLength;
            string fileoldname = uploadfile.FileName;
            if (filesize > 20 * 1024 * 1024)
            {
                X.Msg.Alert("提示", "上传文件过大，大小必须低于20M").Show();
                return this.Direct();
            }
            if (uploadfile.HasFile)
            {
                string filenewname = Guid.NewGuid().ToString() + Path.GetExtension(fileoldname);
                string logicpath = "~/AttachFile/WorkYard/FieldData/" + filenewname;
                string filepath = Server.MapPath(logicpath);
                fdm.AnnetPath = logicpath;
                uploadfile.PostedFile.SaveAs(filepath);
            }
            DB.T_GW_FieldDataManagement.Add(fdm);
            DB.SaveChanges();

            DirectResult result = new DirectResult();
            result.IsUpload = true;
            return result;
        }
        #endregion

        #region 下载附件
        public ActionResult FileDownload(string filePath, string fileName)
        {
            download(Server.MapPath(filePath), fileName);
            return this.Direct(true);
        }
        public void download(string filePath, string fileName)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            if (fileInfo.Exists == true)
            {
                const long ChunkSize = 102400;//100K 每次读取文件，只读取100K，这样可以缓解服务器的压力
                byte[] buffer = new byte[ChunkSize];
                Response.Clear();
                System.IO.FileStream iStream = System.IO.File.OpenRead(filePath);
                long dataLengthToRead = iStream.Length;//获取下载的文件总大小
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName));
                while (dataLengthToRead > 0 && Response.IsClientConnected)
                {
                    int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(ChunkSize));//读取的大小
                    Response.OutputStream.Write(buffer, 0, lengthRead);
                    Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }
                Response.Close();
            }
        }
        #endregion

    }
}
