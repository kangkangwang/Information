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
    [DirectController(AreaName = "person")]
    public class CertificateController : Controller
    {
        //
        // GET: /person/Certificate/

        Entities entities = new Entities();

        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_Certificate
                            where o.Valid==true
                            select o).ToList();
                foreach (var item in list)
                {
                    if (item.CerImage != null)
                        item.CerImage = Url.Content(item.CerImage);
                }
                ViewData["root"] = Tool.GetNode();
                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }

        }

        public ActionResult DepClick(string dep,string state)
        {
            var list = new List<V_HR_Certificate>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    string[] staff = Tool.StaffStr(dep);
                    list = (from o in entities.V_HR_Certificate
                            where staff.Contains(o.StaffID) && o.Valid==true
                            select o).ToList();
                }
                else
                {
                    list = (from o in entities.V_HR_Certificate
                            where o.Valid == true
                            select o).ToList();
                }

                if (state != "全部")
                {
                    list = (from o in list
                            where o.State == state
                            select o).ToList();
                }

                foreach (var item in list)
                {
                    if (item.CerImage != null)
                        item.CerImage = Url.Content(item.CerImage);
                }

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }//处理树形菜单响应

        public ActionResult Getalldata(string staffid, string name,string state)//查询按钮响应
        {
            try
            {
                var list = SearchData(staffid, name,state);

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        private List<V_HR_Certificate> SearchData(string staffid, string name,string state)//查询时根据ID和Name进行模糊查询
        {
            var list = (from o in entities.V_HR_Certificate
                       where o.Valid == true
                       select o).ToList();

            if (state != "全部")
            {
                list = (from o in list
                        where o.State == state
                        select o).ToList();
            }

            if (!String.IsNullOrEmpty(name))
            {
                list = (from o in list
                        where o.Name.Contains(name)
                        select o).ToList();
            }

            if (!String.IsNullOrEmpty(staffid))
            {
                list = (from o in list
                        where o.StaffID.Contains(staffid)
                        select o).ToList();
            }

            foreach (var item in list)
            {
                if (item.CerImage != null)
                    item.CerImage = Url.Content(item.CerImage);
            }

            return list;
        }

        [DirectMethod]
        public ActionResult CertificateReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_Certificate
                            where o.Valid==true
                            select o).ToList();
                foreach (var item in list)
                {
                    if (item.CerImage != null)
                        item.CerImage = Url.Content(item.CerImage);
                }

                var store = X.GetCmp<Store>("CertificateStore");
                store.LoadData(list);
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        [VisitAuthorize(Create = true)]
        public ActionResult Add()//添加响应
        {
            Window win = new Window
            {

                ID = "windowCertificate",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddCertificate", "Certificate", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.CertificateReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id)//修改相应
        {
            Window win = new Window
            {

                ID = "windowCertificate",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddCertificate", "Certificate", new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.CertificateReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);

            return this.Direct();


        }

        public ActionResult AddCertificate(string id)//在修改时传递的为contractid
        {
            if (id == "-1")//-1为添加
            {
                return View();
            }
            else//否则为修改
            {
                V_HR_Certificate item = (from o in entities.V_HR_Certificate
                                         where o.ID == id
                                         select o).First();


                return View(item);
            }
        }

        [HttpPost]
        public ActionResult AddOrEditCertificate(V_HR_Certificate re)
        {
            FileUploadField upload = this.GetCmp<FileUploadField>("Userimage");
            HttpPostedFile UserImageFile;
            string UserImage = "", UserImageSavePath = "", UserImageFileType,path="";
            UserImageFile = upload.PostedFile;

            if (UserImageFile.ContentLength <= 20971520)
            {
                DirectResult r = new DirectResult();
                T_HR_Certificate reupdate = entities.T_HR_Certificate.Find(re.ID);

                if (upload.HasFile)
                {
                    UserImage = string.Format("~/AttachFile/HR/Certificate/{0}/{1}{2}", re.StaffID, Guid.NewGuid().ToString(),
                        Path.GetExtension(UserImageFile.FileName));
                    UserImageSavePath =
                        Server.MapPath(UserImage);
                    UserImageFileType = UserImageFile.ContentType;

                }

                if (UserImageSavePath != "" && Path.GetDirectoryName(UserImageSavePath) != null)
                {
                    if (Directory.Exists(Path.GetDirectoryName(UserImageSavePath)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(UserImageSavePath));
                    }

                    UserImageFile.SaveAs(UserImageSavePath);
                }

                if (reupdate == null)//为空为添加
                {
                    T_HR_Certificate readd = new T_HR_Certificate();
                    readd.ID = Guid.NewGuid().ToString();
                    readd.StaffID = re.StaffID;
                    readd.CertificateName = re.CertificateName;
                    readd.CertificateNum = re.CertificateNum;
                    readd.Startdate = re.Startdate;
                    readd.Deadline = re.Deadline;
                    readd.State = re.State;
                    if (UserImage != "")
                        readd.CerImage = UserImage;
                    readd.Remark = re.Remark;
                    readd.Valid = true;
                    readd.CreaterName = new LoginUser().EmployeeId;
                    readd.CreateTime = DateTime.Now;

                    entities.T_HR_Certificate.Add(readd);
                    try
                    {
                        entities.SaveChanges();
                        r.Success = true;
                        X.Msg.Alert("提示", "保存成功！", new JFunction { Fn = "closewindow" }).Show();
                    }
                    catch (Exception e)
                    {
                        X.Msg.Alert("警告", "数据保存失败！<br /> note:" + e.Message, new JFunction { Fn = "closewindow" }).Show();
                        r.Success = false;
                    }
                }
                else//否则为修改
                {
                    reupdate.StaffID = re.StaffID;
                    reupdate.CertificateName = re.CertificateName;
                    reupdate.CertificateNum = re.CertificateNum;
                    reupdate.Startdate = re.Startdate;
                    reupdate.Deadline = re.Deadline;
                    reupdate.State = re.State;
                    if (UserImage != "")
                    {
                        if (reupdate.CerImage != null)
                            path = Server.MapPath(reupdate.CerImage);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        reupdate.CerImage = UserImage;
                    }   
                    reupdate.Remark = re.Remark;
                    reupdate.Valid = true;
                    reupdate.EditorName = new LoginUser().EmployeeId;
                    reupdate.EditeTime = DateTime.Now;

                    try
                    {
                        entities.SaveChanges();
                        r.Success = true;
                        X.Msg.Alert("提示", "保存成功！", new JFunction { Fn = "closewindow" }).Show();
                    }
                    catch (Exception e)
                    {
                        X.Msg.Alert("警告", "数据保存失败！<br /> note:" + e.Message, new JFunction { Fn = "closewindow" }).Show();
                        r.Success = false;
                    }
                }

                return r;
            }
            else
            {
                X.Msg.Alert("警告", "图片需要小于20M！").Show();
                return this.Direct();
            }
        }

        [VisitAuthorize(Delete = true)]
        public ActionResult JSDeleteCertificate(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("CertificateStore");
            string id, path="";
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["ID"];
                    T_HR_Certificate de = entities.T_HR_Certificate.Find(id);
                    if (de != null)
                    {
                        if (de.CerImage != null)
                            path = Server.MapPath(de.CerImage);
                        de.Valid = false;
                        //entities.T_HR_Certificate.Remove(de);
                        try
                        {
                            entities.SaveChanges();
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            s.Remove(id);
                        }
                        catch (Exception e)
                        {
                            X.Msg.Alert("警告", "数据删除失败！<br /> note:" + e.Message).Show();
                        }
                    }
                }
            }
            else
            {
                X.Msg.Alert("提示", "未选择任何列！").Show();
            }
            return this.Direct();
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
                        Handler = "App.direct.person.DealGetperson()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }





    }
}
