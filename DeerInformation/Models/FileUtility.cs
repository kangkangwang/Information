using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeerInformation.Models
{
    public class FileUtility
    {
        public HttpPostedFile File { get; set; }

        public string FileType { get; set; }

        /// <summary>
        /// 文件的相对路径（包括文件名及拓展名）
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件的完整存储物理路径（包括文件名及拓展名）
        /// </summary>
        public string SavePath { get; set; }

        public bool SavaData()
        {
            try
            {
                var direcPath = Path.GetDirectoryName(SavePath);
                if (direcPath != null)
                {
                    if (Directory.Exists(direcPath) == false)
                    {
                        Directory.CreateDirectory(direcPath);
                    }

                    File.SaveAs(SavePath);
                    return true;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return false;
        }
    }
}