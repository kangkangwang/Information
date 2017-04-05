using Ext.Net;
using System;
using System.IO;
using System.Web.Mvc;
using Ext.Net.MVC;
using System.Web;

namespace DeerInformation.Areas.gyproject.ShareMethod
{
    public class BaseUseController : Controller
    {
        //
        // GET: /gyproject/BaseUse/

        /// <summary>
        /// 保存附件
        /// </summary>
        /// <param name="fileuploadfieldname"></param>
        /// <returns></returns>
        public string UploadAttach(string fileuploadfieldname)
        {
            var uploadfile = this.GetCmp<FileUploadField>(fileuploadfieldname);
            string fileoldname = uploadfile.FileName;
            if (uploadfile.HasFile)
            {
                string filenewname = Guid.NewGuid().ToString() + Path.GetExtension(fileoldname);
                string logicpath = "~/AttachFile/WorkYard/FieldData/" + filenewname;
                string filepath = Server.MapPath(logicpath);
                uploadfile.PostedFile.SaveAs(filepath);
                return logicpath;
            }
            return "-1";
        }
        /// <summary>
        /// 下载文件的通用方法
        /// </summary>
        /// <param name="filePath"></param>
        public void download(string filePath)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            string houzhui = filePath.Substring(filePath.IndexOf("."));
            if (fileInfo.Exists == true)
            {
                const long ChunkSize = 102400;//100K 每次读取文件，只读取100K，这样可以缓解服务器的压力
                byte[] buffer = new byte[ChunkSize];
                Response.Clear();
                System.IO.FileStream iStream = System.IO.File.OpenRead(filePath);
                long dataLengthToRead = iStream.Length;//获取下载的文件总大小
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=download" + houzhui);
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

    }
}
