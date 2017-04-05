using DeerInformation.Models;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using System.IO;


namespace DeerInformation.Extensions
{
    delegate S Test<S>(S n);
    /// <summary>
    /// 
    /// </summary>
    public class Mydb:Controller
    {
        Entities DB = new Entities();
        public void updateworkyard<T>(T model)where T:class
        {
            DB.Entry<T>(model).State = EntityState.Added;

            DB.SaveChanges();
        }

        public bool HasFile(string sFileName)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(sFileName);
            return file.Exists;
        }
        public bool DownloadFile(string FileName, string DownloadName)//石老师的方法
        {
            if (HasFile(FileName))
            {
                FileInfo DownloadFile = new FileInfo(FileName);
                FileStream myFile = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader _BinaryReader = new BinaryReader(myFile);

                if (DownloadName == "")
                    DownloadName = System.Web.HttpUtility.UrlEncode(DownloadFile.FullName, System.Text.Encoding.UTF8);
                else
                    DownloadName = System.Web.HttpUtility.UrlEncode(DownloadName, System.Text.Encoding.UTF8);
                try
                {
                    long startBytes = 0;
                    string lastUpdateTiemStamp = System.IO.File.GetLastWriteTimeUtc(DownloadFile.Directory.ToString()).ToString("r");
                    string _EncodedData = HttpUtility.UrlEncode(DownloadFile.Name, System.Text.Encoding.UTF8) + lastUpdateTiemStamp;


                    System.Web.HttpContext.Current.Response.Clear();
                    System.Web.HttpContext.Current.Response.Buffer = false;
                    System.Web.HttpContext.Current.Response.AddHeader("Accept-Ranges", "bytes");
                    System.Web.HttpContext.Current.Response.AppendHeader("ETag", "\"" + _EncodedData + "\"");
                    System.Web.HttpContext.Current.Response.AppendHeader("Last-Modified", lastUpdateTiemStamp);
                    System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + DownloadName);
                    System.Web.HttpContext.Current.Response.AppendHeader("Content-Length", (DownloadFile.Length - startBytes).ToString());
                    System.Web.HttpContext.Current.Response.AddHeader("Connection", "Keep-Alive");
                    System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                    System.Web.HttpContext.Current.Response.WriteFile(DownloadFile.FullName);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    _BinaryReader.Close();
                    myFile.Close();
                }
            }
            else
                return false;
        }
        /// <summary>
        /// 将前台传回的Json数组转换成字符数组
        /// Converts the Json array returned from the foreground to an array of string
        /// </summary>
        /// <param name="list">Json array returned from the foreground is type of string</param>
        /// <returns></returns>
        public List<string> JsontoList(string list)
        {
            List<string> a = new List<string>();
            list = list.Replace("\"", "").Replace("[", "").Replace("]", "");
            a = list.Split(',').ToList();
            a = a as List<string>;
            return a;
        }
        /// <summary>
        /// 重新加载当前界面的基础方法
        /// Reloads the underlying interface's basic method
        /// </summary>
        /// <param name="data">a list of modelvalue</param>
        /// <param name="storename"></param>
        public void Reload(object data, string storename)
        {
            try
            {
                var store = X.GetCmp<Store>(storename);
                store.LoadData(data);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
            }
        }

        /// <summary>
        /// 判断时间查询输入是否符合规范
        /// judging user's input is legal
        /// </summary>
        /// <param name="start">start time</param>
        /// <param name="end">end time</param>
        /// <returns>bool type</returns>
        public bool QueryTime(string start, string end)
        {
            if (start != "null" && end != "null")
            {
                DateTime dts = Convert.ToDateTime(start.Replace("\"", ""));
                DateTime dte = Convert.ToDateTime(end.Replace("\"", ""));
                if (dts > dte)
                {
                    X.MessageBox.Alert("警告", "结束日期小于开始日期！！！").Show();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (start == "null" && end == "null")
                {
                    X.MessageBox.Alert("警告", "您尚未选择任何查询条件！！！").Show();
                }
                else
                {
                    if (start == "null")
                    {
                        X.MessageBox.Alert("警告", "开始日期不能为空！！！").Show();
                    }
                    else
                    {
                        X.MessageBox.Alert("警告", "结束日期不能为空！！！").Show();
                    }    
                } 
                return false;
            }

            //方法2
            //object[] li = new object[5];//初始化，规模没有影响
            //string sql = string.Format("select * from [Deer_MIS].[dbo].[T_GM_StorageFixedAsset] where StorageDate between '{0}' and '{1}'", dts, dte);
            //var te = DB.T_GM_StorageFixedAsset.SqlQuery(sql, li).ToList();
            //return this.Store(te);
        }
        /// <summary>
        /// the function is as the same as QueryTime,judging user's input is legal,but it is the main criteria for judgment
        /// </summary>
        /// <param name="addno"></param>
        /// <param name="addfield"></param>
        /// <returns></returns>
        public int QueryFirstTwo(string addno, string addfield)
        {
            if (addno.Length > 0 && addno != "")
            {
                if (addfield != "" && addfield.Length > 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (addfield != "" && addfield.Length > 0)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
        }

        //public Panel pp(string id, string title)
        //{        
        //    var p = new Panel()
        //    {
        //        ID = "audit",
        //        Loader = new ComponentLoader()
        //        {
        //            Url = Url.Action("Audit", "Share", new { area = "gyproject", mainid = id, tit = title }),
        //            DisableCaching = true,
        //            Mode = LoadMode.Frame
        //        },
        //    };
        //    return p;
        //}

        
        /// <summary>
        /// 设置审核流程下拉框
        /// set auditprocess combox value
        /// </summary>
        /// <param name="checkname">check flow</param>
        /// <returns></returns>
        public List<ListItem> SetCombox(string checkname)
        {
            List<ListItem> li = new List<ListItem>();
            var list = DB.V_CH_Checkfuncflow.Where(w => w.CheckfuncName == checkname).Where(w => w.Disable == false).ToList();
            foreach (var item in list)
            {
                ListItem listitem = new ListItem();
                listitem.Value = item.ID;
                listitem.Text = item.Name;
                li.Add(listitem);
            }
            return li;
        }
        /// <summary>
        /// set CGM combox value
        /// </summary>
        /// <returns></returns>
        public List<ListItem> SetCombox_CGM(bool type=true)
        {
            DateTime t1=DateTime.Now.AddDays(-3);
            DateTime t2=DateTime.Now.AddDays(7);
            List<ListItem> li = new List<ListItem>();
            if (type)
            {
                var list = DB.V_GM_MPurchase.Where(m => m.PrepaidDay < t2).Where(m => m.PrepaidDay > t1).Where(m => m.IsEnableNo == "审核通过").Where(m => m.OrderStatu ==null).Take(10).ToList();
                foreach (var item in list)
                {
                    ListItem listitem = new ListItem();
                    listitem.Value = item.GID;
                    listitem.Text = item.PurchaseMNo;
                    li.Add(listitem);
                }
            }
            else
            {
                var list = DB.V_GM_FAPurchase.Where(m => m.No_Date < t2).Where(m => m.No_Date > t1).Where(m => m.Description == "审核通过").Where(m => m.Status == null).ToList();//.Where(m => m.Status != "delivered").Where(m => m.No_Date < t2).Where(m => m.No_Date > t1).Where(m => m.Description == "审核通过")
                foreach (var item in list)
                {
                    ListItem listitem = new ListItem();
                    listitem.Value = item.GID;
                    listitem.Text = item.PurchaseFNo;
                    li.Add(listitem);
                }
            }    
            return li;
        }

    }
}