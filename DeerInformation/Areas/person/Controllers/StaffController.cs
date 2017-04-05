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
    public class StaffController : Controller
    {
        //
        // GET: /person/Staff/

        Entities entities = new Entities();
        [VisitAuthorize(Read=true)]
        public ActionResult Index()
        {
            try
            {
                var list = (from o in entities.V_HR_StaffWithDepName
                            where o.HireState == "在职"
                            select o).ToList();
                ViewData["root"] = Tool.GetNode();
                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }

        }

        public ActionResult DepClick(string dep, string hirestate)
        {
            var list = new List<V_HR_StaffWithDepName>();
            dep = dep.Replace("\"", "");
            try
            {
                if (dep != "0")
                {
                    string[] staff = Tool.StaffStr(dep);
                    if (hirestate == "全部")
                    {
                        list = (from o in entities.V_HR_StaffWithDepName
                                where staff.Contains(o.ID)
                                select o).ToList();
                    }
                    else
                    {
                        list = (from o in entities.V_HR_StaffWithDepName
                                where staff.Contains(o.ID) && o.HireState == hirestate
                                select o).ToList();
                    }
                }
                else
                {
                    if (hirestate == "全部")
                    {
                        list = (from o in entities.V_HR_StaffWithDepName
                                select o).ToList();
                    }
                    else
                    {
                        list = (from o in entities.V_HR_StaffWithDepName
                                where o.HireState == hirestate
                                select o).ToList();
                    }
                }

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }//处理树形菜单响应

        public ActionResult Getalldata(string staffid, string name, string hirestate)//查询按钮响应
        {
            try
            {
                var list = SearchData(staffid, name, hirestate);

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }

        private List<V_HR_StaffWithDepName> SearchData(string staffid, string name, string hirestate)//查询时根据ID和Name进行模糊查询
        {
            var list = new List<V_HR_StaffWithDepName>();

            if (hirestate == "全部")
            {
                list = (from o in entities.V_HR_StaffWithDepName
                        select o).ToList();
            }
            else
            {
                list = (from o in entities.V_HR_StaffWithDepName
                        where o.HireState == hirestate
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
                        where o.ID.Contains(staffid)
                        select o).ToList();
            }

            return list;
        }

        [DirectMethod]
        public ActionResult StaffReload()//刷新gridpanel的store
        {
            try
            {
                var list = (from o in entities.V_HR_StaffWithDepName
                            where o.HireState == "在职"
                            select o).ToList();

                var store = X.GetCmp<Store>("StaffStore");
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

                ID = "windowStaff",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddStaff", "Staff", new { id = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.StaffReload()",
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

                ID = "windowStaff",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddStaff", "Staff", new { id = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.StaffReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);

            return this.Direct();


        }

        public int GetMaxID()
        {
            System.Text.RegularExpressions.Regex rex=
        new System.Text.RegularExpressions.Regex(@"^\d+$");
            try
            {
                int maxid = 0,temp=0;
                var ids = from o in entities.T_HR_Staff
                          select new { o.StaffID };

                if(ids.Any())
                {
                    foreach(var id in ids)
                    {
                        if (rex.IsMatch(id.StaffID))
                        {
                            temp=Convert.ToInt32(id.StaffID);
                            if(temp>maxid)
                            {
                                maxid = temp;
                            }
                        }
                    }
                }

                return maxid;
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return -1;
            }

        }

        public ActionResult AddStaff(string id="-1")//在修改时传递的为contractid
        {
            if (id == "-1")//-1为添加
            {
                StaffDetail s = new StaffDetail();
                int sid = GetMaxID();
                if(sid!=-1)
                {
                    s.ID = (sid + 1).ToString();
                }
                s.HireState = "在职";
                s.NoAttend = "打卡";
                var combo = X.GetCmp<ComboBox>("HireState");
                combo.Selectable = false;
                return View(s);
            }
            else//否则为修改
            {
                V_HR_StaffWithDepName item = (from o in entities.V_HR_StaffWithDepName
                                              where o.ID == id
                                              select o).First();
                var ep = from o in entities.T_HR_EntryPosition
                                         where o.StaffID == id && o.IsValid == true
                                         select o;
                StaffDetail sd = new StaffDetail();
                sd.ID = item.ID;
                sd.Name = item.Name;
                sd.TelNum = item.TelNum;
                sd.Urgencyperson = item.Urgencyperson;
                sd.UPTelNum = item.UPTelNum;
                sd.Email = item.Email;
                sd.Nation = item.Nation;
                sd.HireState = item.HireState;
                sd.CertificateType = item.CertificateType;
                sd.CertificateNum = item.CertificateNum;
                sd.IdCardAddress = item.IdCardAddress;
                sd.Nativeplace = item.Nativeplace;
                sd.Birthday = Convert.ToDateTime(item.Birthday);
                sd.Sex = item.Sex;
                sd.MaritalStatus = item.MaritalStatus;
                sd.StaffType = item.StaffType;
                sd.WorkAddress = item.WorkAddress;
                sd.Area = item.Area;
                sd.BankAccount = item.BankAccount;
                sd.BankType = item.BankType;
                sd.Craft = item.Craft;
                sd.Education = item.Education;
                sd.GraduationSchool = item.GraduationSchool;
                sd.Major = item.Major;
                sd.Remark = item.Remark;
                sd.ID1 = item.ID1;
                sd.Department1No = item.Department1No;
                sd.Department1Name = item.Department1Name;
                sd.ID2 = item.ID2;
                sd.Department2No = item.Department2No;
                sd.Department2Name = item.Department2Name;
                sd.ID3 = item.ID3;
                sd.Department3No = item.Department3No;
                sd.Department3Name = item.Department3Name;
                sd.ID4 = item.ID4;
                sd.Department4No = item.Department4No;
                sd.Department4Name = item.Department4Name;
                sd.ID5 = item.ID5;
                sd.Department5No = item.Department5No;
                sd.Department5Name = item.Department5Name;
                sd.Department = item.Department;
                sd.NoAttend = item.NoAttend;
                sd.ImagePath = item.ImagePath;
                sd.SocialInsuranceType = item.SocialInsuranceType;
                sd.SSStartdate = Convert.ToDateTime(item.SSStartdate);
                sd.SSEnddate = Convert.ToDateTime(item.SSEnddate);
                sd.Probation = Convert.ToInt32(item.Probation);
                sd.ProbationDate = Convert.ToDateTime(item.ProbationDate);
                sd.ProbationValid = Convert.ToBoolean(item.ProbationValid);
                sd.PreDimissionDate = Convert.ToDateTime(item.PreDimissionDate);
                sd.DimissionType = item.DimissionType;
                sd.DimissionReason = item.DimissionReason;
                sd.CreaterName = item.CreaterName;
                sd.CreateTime = Convert.ToDateTime(item.CreateTime);
                sd.EditorName = item.EditorName;
                sd.EditeTime = Convert.ToDateTime(item.EditeTime);
                sd.FundValid = Convert.ToBoolean(item.FundValid);
                if(ep.Any())
                {
                    sd.EntryPositionID = ep.First().EntryPositionID;
                    sd.EntryTime = Convert.ToDateTime(ep.First().EntryTime);
                }       

                var p = X.GetCmp<FieldSet>("JobList");
                p.Hidden = true;

                var staffid = X.GetCmp<TextField>("ID");
                staffid.Editable = false;


                return View(sd);
            }
        }

        public ActionResult GetID1()
        {
            var list = (from o in entities.T_HR_Department1
                        orderby o.DOrder
                        select new { o.ID1, o.Department1Name }).ToList();
            return this.Store(list);
        }

        public ActionResult GetID2(string id1)
        {
            if (!String.IsNullOrEmpty(id1))
            {
                var list = (from o in entities.T_HR_Department2
                            where o.ID1 == id1
                            orderby o.DOrder
                            select new { o.ID2, o.Department2Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department2
                            orderby o.DOrder
                            select new { o.ID2, o.Department2Name }).ToList();
                return this.Store(list);
            }
        }

        public ActionResult GetID3(string id2)
        {
            if (!String.IsNullOrEmpty(id2))
            {
                var list = (from o in entities.T_HR_Department3
                            where o.ID2 == id2
                            orderby o.DOrder
                            select new { o.ID3, o.Department3Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department3
                            orderby o.DOrder
                            select new { o.ID3, o.Department3Name }).ToList();
                return this.Store(list);
            }
        }

        public ActionResult GetID4(string id3)
        {
            if (!String.IsNullOrEmpty(id3))
            {
                var list = (from o in entities.T_HR_Department4
                            where o.ID3 == id3
                            orderby o.DOrder
                            select new { o.ID4, o.Department4Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department4
                            orderby o.DOrder
                            select new { o.ID4, o.Department4Name }).ToList();
                return this.Store(list);
            }
        }

        public ActionResult GetID5(string id4)
        {
            if (!String.IsNullOrEmpty(id4))
            {
                var list = (from o in entities.T_HR_Department5
                            where o.ID4 == id4
                            orderby o.DOrder
                            select new { o.ID5, o.Department5Name }).ToList();
                return this.Store(list);
            }
            else
            {
                var list = (from o in entities.T_HR_Department5
                            orderby o.DOrder
                            select new { o.ID5, o.Department5Name }).ToList();
                return this.Store(list);
            }
        }

        public ActionResult GetPC()
        {
            var list = (from o in entities.T_HR_PositionCategory
                        select new { o.PositionCategoryID, o.PositionCategoryName }).ToList();
            return this.Store(list);
        }

        public ActionResult GetDuty(string pcid)
        {
            if (String.IsNullOrEmpty(pcid))
            {
                var list = (from o in entities.T_HR_Duty
                            select new { o.DutyID, o.DutyName }).ToList();
                return this.Store(list);

            }
            else
            {
                var list = (from o in entities.T_HR_Duty
                            where o.PositionCategoryID==pcid
                            select new { o.DutyID, o.DutyName }).ToList();
                return this.Store(list);
            }    
        }

        public ActionResult GetJob(string dutyid)
        {
            if (String.IsNullOrEmpty(dutyid))
            {
                var list = (from o in entities.T_HR_Job
                            select new { o.JobID, o.JobName }).ToList();
                return this.Store(list);

            }
            else
            {
                var list = (from o in entities.T_HR_Job
                            where o.DutyID == dutyid
                            select new { o.JobID, o.JobName }).ToList();
                return this.Store(list);
            }    
        }

        [DirectMethod]
        public ActionResult SetDT(string dutyid)
        {
            T_HR_Duty d = entities.T_HR_Duty.Find(dutyid);
            var c = X.GetCmp<ComboBox>("DutyType");
            c.SetValue(d.DutyType);
            return this.Direct();
        }

        [HttpPost]
        public ActionResult AddOrEditStaff(StaffDetail sd)
        {
            //int staffidint = Convert.ToInt32(sd.ID);
            //sd.ID = staffidint.ToString();

            DirectResult r = new DirectResult();
            T_HR_Staff sdupdate = entities.T_HR_Staff.Find(sd.ID);
            string department="";

            if(!String.IsNullOrEmpty(sd.ID1))
            {
                var d1=from o in entities.T_HR_Department1
                       where o.ID1==sd.ID1
                       select o;
                department+=d1.First().Department1Name;
            }
            if(!String.IsNullOrEmpty(sd.ID2))
            {
                var d1=from o in entities.T_HR_Department2
                       where o.ID2==sd.ID2
                       select o;
                department+=d1.First().Department2Name;
            }
            if(!String.IsNullOrEmpty(sd.ID3))
            {
                var d1=from o in entities.T_HR_Department3
                       where o.ID3==sd.ID3
                       select o;
                department+=d1.First().Department3Name;
            }
            if(!String.IsNullOrEmpty(sd.ID4))
            {
                var d1=from o in entities.T_HR_Department4
                       where o.ID4==sd.ID4
                       select o;
                department+=d1.First().Department4Name;
            }
            if(!String.IsNullOrEmpty(sd.ID5))
            {
                var d1=from o in entities.T_HR_Department5
                       where o.ID5==sd.ID5
                       select o;
                department+=d1.First().Department5Name;
            }

            FileUploadField upload = this.GetCmp<FileUploadField>("Userimage");
            HttpPostedFile UserImageFile;
            string UserImage = "", UserImageSavePath = "", UserImageFileType;
            UserImageFile = upload.PostedFile;

            if (upload.HasFile)
            {
                UserImage = string.Format("~/AttachFile/HR/Certificate/{0}/{1}{2}", sd.ID, Guid.NewGuid().ToString(),
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

            if (sdupdate == null)//为空为添加
            {
                var list = from o in entities.T_HR_Staff
                           where o.StaffID == sd.ID
                           select o;
                if (!list.Any())
                {
                    T_HR_Staff sdadd = new T_HR_Staff();
                    sdadd.StaffID = sd.ID;
                    sdadd.Name = sd.Name;
                    sdadd.TelNum = sd.TelNum;
                    sdadd.Urgencyperson = sd.Urgencyperson;
                    sdadd.UPTelNum = sd.UPTelNum;
                    sdadd.Email = sd.Email;
                    sdadd.Nation = sd.Nation;
                    sdadd.HireState = sd.HireState;
                    sdadd.CertificateType = sd.CertificateType;
                    sdadd.CertificateNum = sd.CertificateNum;
                    sdadd.IdCardAddress = sd.IdCardAddress;
                    sdadd.Nativeplace = sd.Nativeplace;
                    sdadd.Birthday = sd.Birthday;
                    sdadd.Sex = sd.Sex;
                    sdadd.MaritalStatus = sd.MaritalStatus;
                    sdadd.StaffType = sd.StaffType;
                    sdadd.WorkAddress = sd.WorkAddress;
                    sdadd.Area = sd.Area;
                    sdadd.BankAccount = sd.BankAccount;
                    sdadd.BankType = sd.BankType;
                    sdadd.Craft = sd.Craft;
                    sdadd.Education = sd.Education;
                    sdadd.GraduationSchool = sd.GraduationSchool;
                    sdadd.Major = sd.Major;
                    sdadd.Remark = sd.Remark;
                    sdadd.ID1 = sd.ID1;
                    sdadd.ID2 = sd.ID2;
                    sdadd.ID3 = sd.ID3;
                    sdadd.ID4 = sd.ID4;
                    sdadd.ID5 = sd.ID5;
                    if (department != "")
                        sdadd.Department = department;
                    sdadd.NoAttend = sd.NoAttend;
                    if(UserImage!="")
                    {
                        sdadd.ImagePath = UserImage;
                    }
                    sdadd.SocialInsuranceType = sd.SocialInsuranceType;
                    sdadd.SSStartdate = sd.SSStartdate;
                    sdadd.SSEnddate = sd.SSEnddate;
                    sdadd.Probation = sd.Probation;
                    sdadd.ProbationDate = sd.ProbationDate;
                    sdadd.ProbationValid = sd.ProbationValid;
                    sdadd.PreDimissionDate = sd.PreDimissionDate;
                    sdadd.DimissionType = sd.DimissionType;
                    sdadd.DimissionReason = sd.DimissionReason;
                    sdadd.CreaterName = new LoginUser().EmployeeId;
                    sdadd.CreateTime = DateTime.Now;

                    var eplist = from o in entities.T_HR_EntryPosition
                                 where o.StaffID == sd.ID
                                 select o;
                    foreach (var epitem in eplist)
                    {
                        T_HR_EntryPosition e = entities.T_HR_EntryPosition.Find(epitem.EntryPositionID);
                        e.IsValid = false;
                    }

                    T_HR_EntryPosition ep = new T_HR_EntryPosition();
                    ep.EntryPositionID = Guid.NewGuid().ToString();
                    ep.StaffID = sd.ID;
                    ep.EntryTime = sd.EntryTime;
                    ep.Num = GetEPNum(sd.ID);
                    ep.IsValid = true;
                    ep.CreaterName = new LoginUser().EmployeeId;
                    ep.CreateTime = DateTime.Now;

                    var jolist = from o in entities.T_HR_StaffJob
                                 where o.StaffID == sd.ID
                                 select o;
                    foreach (var joitem in jolist)
                    {
                        T_HR_StaffJob e = entities.T_HR_StaffJob.Find(joitem.StaffJobID);
                        e.IsValid = false;
                    }

                    T_HR_StaffJob sj = new T_HR_StaffJob();
                    sj.StaffJobID = Guid.NewGuid().ToString();
                    sj.StaffID = sd.ID;
                    sj.JobID = sd.JobID;
                    sj.IsValid = true;
                    sj.ValidTime = sd.EntryTime;
                    sj.ChangeType = "入职";
                    sj.CreaterName = new LoginUser().EmployeeId;
                    sj.CreateTime = DateTime.Now;

                    var wotlist = from o in entities.T_HR_WorkOverTime
                                  where o.StaffID == sd.ID
                                  select o;
                    foreach (var wotitem in wotlist)
                    {
                        T_HR_WorkOverTime e = entities.T_HR_WorkOverTime.Find(wotitem.ID);
                        e.Valid = false;
                    }

                    T_HR_WorkOverTime wot = new T_HR_WorkOverTime();
                    wot.ID = Guid.NewGuid().ToString();
                    wot.StaffID = sd.ID;
                    wot.Year = DateTime.Now.Year;
                    wot.LastYear = 0;
                    wot.CurrentYear = 0;
                    wot.CurrentYearSum = 0;
                    wot.Personal = 0;
                    wot.Company = 0;
                    wot.CurrentYearLast = 0;
                    wot.Valid = true;
                    wot.CreaterName = new LoginUser().EmployeeId;
                    wot.CreateTime = DateTime.Now;

                    entities.T_HR_Staff.Add(sdadd);
                    entities.T_HR_EntryPosition.Add(ep);
                    entities.T_HR_StaffJob.Add(sj);
                    entities.T_HR_WorkOverTime.Add(wot);
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
                else
                {
                    X.Msg.Alert("警告", "此员工工号已存在，不可重复添加！").Show();
                    r.Success = false;
                }
            }
            else//否则为修改
            {
                sdupdate.StaffID = sd.ID;
                sdupdate.Name = sd.Name;
                sdupdate.TelNum = sd.TelNum;
                sdupdate.Urgencyperson = sd.Urgencyperson;
                sdupdate.UPTelNum = sd.UPTelNum;
                sdupdate.Email = sd.Email;
                sdupdate.Nation = sd.Nation;
                sdupdate.HireState = sd.HireState;
                sdupdate.CertificateType = sd.CertificateType;
                sdupdate.CertificateNum = sd.CertificateNum;
                sdupdate.IdCardAddress = sd.IdCardAddress;
                sdupdate.Nativeplace = sd.Nativeplace;
                sdupdate.Birthday = sd.Birthday;
                sdupdate.Sex = sd.Sex;
                sdupdate.MaritalStatus = sd.MaritalStatus;
                sdupdate.StaffType = sd.StaffType;
                sdupdate.WorkAddress = sd.WorkAddress;
                sdupdate.Area = sd.Area;
                sdupdate.BankAccount = sd.BankAccount;
                sdupdate.BankType = sd.BankType;
                sdupdate.Craft = sd.Craft;
                sdupdate.Education = sd.Education;
                sdupdate.GraduationSchool = sd.GraduationSchool;
                sdupdate.Major = sd.Major;
                sdupdate.Remark = sd.Remark;
                sdupdate.ID1 = sd.ID1;
                sdupdate.ID2 = sd.ID2;
                sdupdate.ID3 = sd.ID3;
                sdupdate.ID4 = sd.ID4;
                sdupdate.ID5 = sd.ID5;
                if (department != "")
                    sdupdate.Department = department;
                sdupdate.NoAttend = sd.NoAttend;
                if(UserImage!="")
                {
                    string path = "";
                    if (sdupdate.ImagePath != null)
                        path = Server.MapPath(sdupdate.ImagePath);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    sdupdate.ImagePath = UserImage;
                }
                sdupdate.SocialInsuranceType = sd.SocialInsuranceType;
                sdupdate.SSStartdate = sd.SSStartdate;
                sdupdate.SSEnddate = sd.SSEnddate;
                sdupdate.Probation = sd.Probation;
                sdupdate.ProbationDate = sd.ProbationDate;
                sdupdate.ProbationValid = sd.ProbationValid;
                sdupdate.PreDimissionDate = sd.PreDimissionDate;
                sdupdate.DimissionType = sd.DimissionType;
                sdupdate.DimissionReason = sd.DimissionReason;
                sdupdate.EditorName = new LoginUser().EmployeeId;
                sdupdate.EditeTime = DateTime.Now;

                T_HR_EntryPosition epupdate = entities.T_HR_EntryPosition.Find(sd.EntryPositionID);
                if (epupdate != null)
                {
                    epupdate.StaffID = sd.ID;
                    epupdate.EntryTime = sd.EntryTime;
                    epupdate.EditorName = new LoginUser().EmployeeId;
                    epupdate.EditeTime = DateTime.Now;
                }
                else
                {
                    var eplist = from o in entities.T_HR_EntryPosition
                                 where o.StaffID == sd.ID
                                 select o;
                    foreach (var epitem in eplist)
                    {
                        T_HR_EntryPosition e = entities.T_HR_EntryPosition.Find(epitem.EntryPositionID);
                        e.IsValid = false;
                    }

                    T_HR_EntryPosition ep = new T_HR_EntryPosition();
                    ep.EntryPositionID = Guid.NewGuid().ToString();
                    ep.StaffID = sd.ID;
                    ep.EntryTime = sd.EntryTime;
                    ep.Num = GetEPNum(sd.ID);
                    ep.IsValid = true;
                    ep.CreaterName = new LoginUser().EmployeeId;
                    ep.CreateTime = DateTime.Now;
                    entities.T_HR_EntryPosition.Add(ep);
                }

                try
                {
                    if (sd.HireState == "离职")
                    {
                        DeleteStaff(sd.ID);
                    }
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

        public int GetEPNum(string id)
        {
            int num;
            var list = from o in entities.T_HR_EntryPosition
                       where o.StaffID == id
                       orderby o.Num descending
                       select o;
            if (list.Any())
            {
                num = Convert.ToInt32(list.First().Num) + 1;
            }
            else
            {
                num = 0;
            }
            return num;
        }
        [VisitAuthorize(Delete = true)]
        public ActionResult JSDeleteStaff(SubmitHandler handler)//删除响应
        {
            var s = X.GetCmp<Store>("StaffStore");
            string id,path="";
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(handler.Json.ToString());

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    id = row["ID"];
                    T_HR_Staff de = entities.T_HR_Staff.Find(id);
                    if (de != null)
                    {
                        entities.T_HR_Staff.Remove(de);
                        try
                        {
                            if (de.ImagePath != null)
                                path = Server.MapPath(de.ImagePath);
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
        [VisitAuthorize(Create = true)]
        public ActionResult Reinstatement(string id)//修改相应
        {
            if(GetHireState(id)=="离职")
            {
                Window win = new Window
                {

                    ID = "windowStaff",
                    Title = "修改",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("ReinstatementStaff", "Staff", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    },
                    Listeners =
                    {
                        Close =
                        {
                            Handler = "App.direct.person.StaffReload()",
                        }
                    }
                };

                win.Render(RenderMode.Auto);
            }
            else
            {
                X.Msg.Alert("警告", "非离职员工不可复职！", new JFunction { Fn = "toggleRowSelect" }).Show();
            }      
            return this.Direct();


        }

        public ActionResult ReinstatementStaff(string id)//在修改时传递的为contractid
        {
            V_HR_StaffWithDepName item = (from o in entities.V_HR_StaffWithDepName
                                          where o.ID == id
                                          select o).First();
            var ep = from o in entities.T_HR_EntryPosition
                     where o.StaffID == id && o.IsValid == true
                     select o;
            StaffDetail sd = new StaffDetail();
            sd.ID = item.ID;
            sd.Name = item.Name;
            sd.TelNum = item.TelNum;
            sd.Urgencyperson = item.Urgencyperson;
            sd.UPTelNum = item.UPTelNum;
            sd.Email = item.Email;
            sd.Nation = item.Nation;
            sd.HireState = "在职";
            sd.CertificateType = item.CertificateType;
            sd.CertificateNum = item.CertificateNum;
            sd.IdCardAddress = item.IdCardAddress;
            sd.Nativeplace = item.Nativeplace;
            sd.Birthday = Convert.ToDateTime(item.Birthday);
            sd.Sex = item.Sex;
            sd.MaritalStatus = item.MaritalStatus;
            sd.StaffType = item.StaffType;
            sd.WorkAddress = item.WorkAddress;
            sd.Area = item.Area;
            sd.BankAccount = item.BankAccount;
            sd.BankType = item.BankType;
            sd.Craft = item.Craft;
            sd.Education = item.Education;
            sd.GraduationSchool = item.GraduationSchool;
            sd.Major = item.Major;
            sd.Remark = item.Remark;
            sd.ID1 = item.ID1;
            sd.Department1No = item.Department1No;
            sd.Department1Name = item.Department1Name;
            sd.ID2 = item.ID2;
            sd.Department2No = item.Department2No;
            sd.Department2Name = item.Department2Name;
            sd.ID3 = item.ID3;
            sd.Department3No = item.Department3No;
            sd.Department3Name = item.Department3Name;
            sd.ID4 = item.ID4;
            sd.Department4No = item.Department4No;
            sd.Department4Name = item.Department4Name;
            sd.ID5 = item.ID5;
            sd.Department5No = item.Department5No;
            sd.Department5Name = item.Department5Name;
            sd.Department = item.Department;
            sd.NoAttend = item.NoAttend;
            sd.ImagePath = item.ImagePath;
            sd.SocialInsuranceType = item.SocialInsuranceType;
            sd.SSStartdate = Convert.ToDateTime(item.SSStartdate);
            sd.SSEnddate = Convert.ToDateTime(item.SSEnddate);
            sd.Probation = Convert.ToInt32(item.Probation);
            sd.ProbationDate = Convert.ToDateTime(item.ProbationDate);
            sd.ProbationValid = Convert.ToBoolean(item.ProbationValid);
            sd.PreDimissionDate = Convert.ToDateTime(item.PreDimissionDate);
            sd.DimissionType = item.DimissionType;
            sd.DimissionReason = item.DimissionReason;
            sd.CreaterName = item.CreaterName;
            sd.CreateTime = Convert.ToDateTime(item.CreateTime);
            sd.EditorName = item.EditorName;
            sd.EditeTime = Convert.ToDateTime(item.EditeTime);
            sd.FundValid = Convert.ToBoolean(item.FundValid);
            if (ep.Any())
            {
                sd.EntryPositionID = ep.First().EntryPositionID;
                sd.EntryTime = Convert.ToDateTime(ep.First().EntryTime);
            }  

            return View(sd);
        }

        [HttpPost]
        public ActionResult SetStaff(StaffDetail sd)
        {
            //int staffidint = Convert.ToInt32(sd.ID);
            //sd.ID = staffidint.ToString();

            DirectResult r = new DirectResult();
            T_HR_Staff sdupdate = entities.T_HR_Staff.Find(sd.ID);

            string department = "";

            if (!String.IsNullOrEmpty(sd.ID1))
            {
                var d1 = from o in entities.T_HR_Department1
                         where o.ID1 == sd.ID1
                         select o;
                department += d1.First().Department1Name;
            }
            if (!String.IsNullOrEmpty(sd.ID2))
            {
                var d1 = from o in entities.T_HR_Department2
                         where o.ID2 == sd.ID2
                         select o;
                department += d1.First().Department2Name;
            }
            if (!String.IsNullOrEmpty(sd.ID3))
            {
                var d1 = from o in entities.T_HR_Department3
                         where o.ID3 == sd.ID3
                         select o;
                department += d1.First().Department3Name;
            }
            if (!String.IsNullOrEmpty(sd.ID4))
            {
                var d1 = from o in entities.T_HR_Department4
                         where o.ID4 == sd.ID4
                         select o;
                department += d1.First().Department4Name;
            }
            if (!String.IsNullOrEmpty(sd.ID5))
            {
                var d1 = from o in entities.T_HR_Department5
                         where o.ID5 == sd.ID5
                         select o;
                department += d1.First().Department5Name;
            }

            FileUploadField upload = this.GetCmp<FileUploadField>("Userimage");
            HttpPostedFile UserImageFile;
            string UserImage = "", UserImageSavePath = "", UserImageFileType;
            UserImageFile = upload.PostedFile;

            if (upload.HasFile)
            {
                UserImage = string.Format("~/AttachFile/HR/Certificate/{0}/{1}{2}", sd.ID, Guid.NewGuid().ToString(),
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

            //sdupdate.StaffID = sd.StaffID;
            sdupdate.Name = sd.Name;
            sdupdate.TelNum = sd.TelNum;
            sdupdate.Urgencyperson = sd.Urgencyperson;
            sdupdate.UPTelNum = sd.UPTelNum;
            sdupdate.Email = sd.Email;
            sdupdate.Nation = sd.Nation;
            sdupdate.HireState = "在职";
            sdupdate.CertificateType = sd.CertificateType;
            sdupdate.CertificateNum = sd.CertificateNum;
            sdupdate.IdCardAddress = sd.IdCardAddress;
            sdupdate.Nativeplace = sd.Nativeplace;
            sdupdate.Birthday = sd.Birthday;
            sdupdate.Sex = sd.Sex;
            sdupdate.MaritalStatus = sd.MaritalStatus;
            sdupdate.StaffType = sd.StaffType;
            sdupdate.WorkAddress = sd.WorkAddress;
            sdupdate.Area = sd.Area;
            sdupdate.BankAccount = sd.BankAccount;
            sdupdate.BankType = sd.BankType;
            sdupdate.Craft = sd.Craft;
            sdupdate.Education = sd.Education;
            sdupdate.GraduationSchool = sd.GraduationSchool;
            sdupdate.Major = sd.Major;
            sdupdate.Remark = sd.Remark;
            sdupdate.ID1 = sd.ID1;
            sdupdate.ID2 = sd.ID2;
            sdupdate.ID3 = sd.ID3;
            sdupdate.ID4 = sd.ID4;
            sdupdate.ID5 = sd.ID5;
            if (department != "")
                sdupdate.Department = department;
            sdupdate.NoAttend = sd.NoAttend;
            if (UserImage != "")
            {
                string path = "";
                if (sdupdate.ImagePath != null)
                    path = Server.MapPath(sdupdate.ImagePath);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                sdupdate.ImagePath = UserImage;
            }
            sdupdate.SocialInsuranceType = sd.SocialInsuranceType;
            sdupdate.SSStartdate = sd.SSStartdate;
            sdupdate.SSEnddate = sd.SSEnddate;
            sdupdate.Probation = sd.Probation;
            sdupdate.ProbationDate = sd.ProbationDate;
            sdupdate.ProbationValid = sd.ProbationValid;
            sdupdate.PreDimissionDate = sd.PreDimissionDate;
            sdupdate.DimissionType = sd.DimissionType;
            sdupdate.DimissionReason = sd.DimissionReason;
            sdupdate.EditorName = new LoginUser().EmployeeId;
            sdupdate.EditeTime = DateTime.Now;

            var eplist = from o in entities.T_HR_EntryPosition
                         where o.StaffID == sd.ID
                         select o;
            foreach (var epitem in eplist)
            {
                T_HR_EntryPosition e = entities.T_HR_EntryPosition.Find(epitem.EntryPositionID);
                e.IsValid = false;
            }

            T_HR_EntryPosition ep = new T_HR_EntryPosition();
            ep.EntryPositionID = Guid.NewGuid().ToString();
            ep.StaffID = sd.ID;
            ep.EntryTime = sd.EntryTime;
            ep.Num = GetEPNum(sd.ID);
            ep.IsValid = true;
            ep.CreaterName = new LoginUser().EmployeeId;
            ep.CreateTime = DateTime.Now;

            var jolist = from o in entities.T_HR_StaffJob
                         where o.StaffID == sd.ID
                         select o;
            foreach (var joitem in jolist)
            {
                T_HR_StaffJob e = entities.T_HR_StaffJob.Find(joitem.StaffJobID);
                e.IsValid = false;
            }

            T_HR_StaffJob sj = new T_HR_StaffJob();
            sj.StaffJobID = Guid.NewGuid().ToString();
            sj.StaffID = sd.ID;
            sj.JobID = sd.JobID;
            sj.IsValid = true;
            sj.ValidTime = sd.EntryTime;
            sj.ChangeType = "入职";
            sj.CreaterName = new LoginUser().EmployeeId;
            sj.CreateTime = DateTime.Now;

            var wotlist = from o in entities.T_HR_WorkOverTime
                          where o.StaffID == sd.ID
                          select o;
            foreach (var wotitem in wotlist)
            {
                T_HR_WorkOverTime e = entities.T_HR_WorkOverTime.Find(wotitem.ID);
                e.Valid = false;
            }

            T_HR_WorkOverTime wot = new T_HR_WorkOverTime();
            wot.ID = Guid.NewGuid().ToString();
            wot.StaffID = sd.ID;
            wot.Year = DateTime.Now.Year;
            wot.LastYear = 0;
            wot.CurrentYear = 0;
            wot.CurrentYearSum = 0;
            wot.Personal = 0;
            wot.Company = 0;
            wot.CurrentYearLast = 0;
            wot.Valid = true;
            wot.CreaterName = new LoginUser().EmployeeId;
            wot.CreateTime = DateTime.Now;

            entities.T_HR_EntryPosition.Add(ep);
            entities.T_HR_StaffJob.Add(sj);
            entities.T_HR_WorkOverTime.Add(wot);
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
            return r;
        }

        public string GetHireState(string id)
        {
            string state="";
            var list = (from o in entities.T_HR_Staff
                        where o.StaffID == id
                        select o).ToList();
            if(list.Any())
            {
                state = list.First().HireState;
            }
            return state;
        }
        [VisitAuthorize(Read = true)]
        public ActionResult Detail(string id)
        {
                Window win = new Window
                {
                    ID = "windowStaff",
                    Title = "详细信息",
                    Height = 500,
                    Width = 800,
                    BodyPadding = 5,
                    Modal = true,
                    CloseAction = CloseAction.Destroy,
                    Loader = new ComponentLoader
                    {
                        Url = Url.Action("StaffDetail", "Staff", new { id = id }),
                        DisableCaching = true,
                        Mode = LoadMode.Frame
                    }
                };

                win.Render(RenderMode.Auto);

            return this.Direct();


        }

        public ActionResult StaffDetail(string id = "T_Employee")
        {
            V_HR_StaffWithDepName item = (from o in entities.V_HR_StaffWithDepName
                                          where o.ID == id
                                          select o).First();
            var ep = from o in entities.T_HR_EntryPosition
                     where o.StaffID == id && o.IsValid == true
                     select o;
            StaffDetail sd = new StaffDetail();
            sd.ID = item.ID;
            sd.Name = item.Name;
            sd.TelNum = item.TelNum;
            sd.Urgencyperson = item.Urgencyperson;
            sd.UPTelNum = item.UPTelNum;
            sd.Email = item.Email;
            sd.Nation = item.Nation;
            sd.HireState = item.HireState;
            sd.CertificateType = item.CertificateType;
            sd.CertificateNum = item.CertificateNum;
            sd.IdCardAddress = item.IdCardAddress;
            sd.Nativeplace = item.Nativeplace;
            sd.Birthday = Convert.ToDateTime(item.Birthday);
            sd.Sex = item.Sex;
            sd.MaritalStatus = item.MaritalStatus;
            sd.StaffType = item.StaffType;
            sd.WorkAddress = item.WorkAddress;
            sd.Area = item.Area;
            sd.BankAccount = item.BankAccount;
            sd.BankType = item.BankType;
            sd.Craft = item.Craft;
            sd.Education = item.Education;
            sd.GraduationSchool = item.GraduationSchool;
            sd.Major = item.Major;
            sd.Remark = item.Remark;
            sd.ID1 = item.ID1;
            sd.Department1No = item.Department1No;
            sd.Department1Name = item.Department1Name;
            sd.ID2 = item.ID2;
            sd.Department2No = item.Department2No;
            sd.Department2Name = item.Department2Name;
            sd.ID3 = item.ID3;
            sd.Department3No = item.Department3No;
            sd.Department3Name = item.Department3Name;
            sd.ID4 = item.ID4;
            sd.Department4No = item.Department4No;
            sd.Department4Name = item.Department4Name;
            sd.ID5 = item.ID5;
            sd.Department5No = item.Department5No;
            sd.Department5Name = item.Department5Name;
            sd.Department = item.Department;
            sd.NoAttend = item.NoAttend;
            sd.ImagePath = item.ImagePath;
            sd.SocialInsuranceType = item.SocialInsuranceType;
            sd.SSStartdate = Convert.ToDateTime(item.SSStartdate);
            sd.SSEnddate = Convert.ToDateTime(item.SSEnddate);
            sd.Probation = Convert.ToInt32(item.Probation);
            sd.ProbationDate = Convert.ToDateTime(item.ProbationDate);
            sd.ProbationValid = Convert.ToBoolean(item.ProbationValid);
            sd.PreDimissionDate = Convert.ToDateTime(item.PreDimissionDate);
            sd.DimissionType = item.DimissionType;
            sd.DimissionReason = item.DimissionReason;
            sd.CreaterName = item.CreaterName;
            sd.CreateTime = Convert.ToDateTime(item.CreateTime);
            sd.EditorName = item.EditorName;
            sd.EditeTime = Convert.ToDateTime(item.EditeTime);
            sd.FundValid = Convert.ToBoolean(item.FundValid);
            if (ep.Any())
            {
                sd.EntryPositionID = ep.First().EntryPositionID;
                sd.EntryTime = Convert.ToDateTime(ep.First().EntryTime);
            }

            if (item.ImagePath != null)
            {
                TempData["s_image"] = item.ImagePath;
            }
            else
            {
                TempData["s_image"] = "";
            }

            //TempData["s_image"] = "~/AttachFile/DailyWorkReport/2016-10-19/26ff7752-0602-4f95-b072-a6a9530be848..png";

            TempData["s_staffid"] = id;

            var job = from o in entities.V_HR_StaffJobWithName
                      where o.StaffID == id && o.IsValid == true
                      select o;
            if(job.Any())
            {
                sd.StaffJobID = job.First().StaffJobID;
                sd.JobID = job.First().JobID;
                sd.JobName = job.First().JobName;
                sd.DutyID = job.First().DutyID;
                sd.DutyName = job.First().DutyName;
                sd.DutyType = job.First().DutyType;
                sd.PositionCategoryID = job.First().PositionCategoryID;
                sd.PositionCategoryName = job.First().PositionCategoryName;
            }

            var contract = from o in entities.V_HR_ContractWithStaffName
                           where o.StaffID == id && o.Valid == true
                           select o;
            if(contract.Any())
            {
                sd.ContractID = contract.First().ID;
                sd.Num = Convert.ToInt32(contract.First().Num);
                sd.StartTime = Convert.ToDateTime(contract.First().StartTime);
                sd.EndTime = Convert.ToDateTime(contract.First().EndTime);
                sd.Years = Convert.ToDecimal(contract.First().Years);
            }

            var fund = from o in entities.V_HR_Fund
                       where o.StaffID == id && o.Valid == true
                       select o;
            if(fund.Any())
            {
                sd.FundID = fund.First().ID;
                sd.StartDate = Convert.ToDateTime(fund.First().StartDate);
                sd.EndDate = Convert.ToDateTime(fund.First().EndDate);
                sd.Months = Convert.ToInt32(fund.First().Months);
            }

            var recommend = from o in entities.V_HR_RecommendWithName
                            where o.ReStaffID == id && o.Valid == true
                            select o;
            if(recommend.Any())
            {
                sd.ReID = recommend.First().ID;
                sd.ReStaffID = recommend.First().StaffID;
                sd.ReName = recommend.First().Name;
                sd.Relation = recommend.First().Relation;
                sd.Money = Convert.ToDecimal(recommend.First().Money);
                sd.YearMonth = Convert.ToDateTime(recommend.First().YearMonth);
                sd.IsDe = Convert.ToBoolean(recommend.First().IsDe);
            }

            return View(sd);
        }

        public ActionResult CertificateInfo(string id="")
        {
            try
            {
                var list = (from o in entities.V_HR_Certificate
                            where o.StaffID==id
                            select o).ToList();
                foreach (var item in list)
                {
                    if (item.CerImage != null)
                        item.CerImage = Url.Content(item.CerImage);
                }
                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }


        public void DeleteStaff(string staffid)
        {
            var wots = from o in entities.T_HR_WorkOverTime
                      where o.StaffID == staffid && o.Valid == true
                      select o;
            if(wots.Any())
            {
                foreach(var item in wots)
                {
                    T_HR_WorkOverTime wot = entities.T_HR_WorkOverTime.Find(item.ID);
                    //entities.T_HR_WorkOverTime.Remove(wot);
                    wot.Valid = false;
                }
            }

            var fus = from o in entities.T_HR_Fund
                       where o.StaffID == staffid && o.Valid == true
                       select o;
            if (fus.Any())
            {
                foreach (var item in fus)
                {
                    T_HR_Fund fu = entities.T_HR_Fund.Find(item.ID);
                    fu.Valid = false;
                   // entities.T_HR_Fund.Remove(fu);
                }
            }

            var schs = from o in entities.T_HR_SchListWithStaff
                      where o.StaffID == staffid
                      select o;
            if (schs.Any())
            {
                foreach (var item in schs)
                {
                    T_HR_SchListWithStaff sch = entities.T_HR_SchListWithStaff.Find(item.ID);
                    entities.T_HR_SchListWithStaff.Remove(sch);
                }
            }

            var res = from o in entities.T_HR_Recommend
                      where o.StaffID == staffid && o.Valid == true
                      select o;
            if (res.Any())
            {
                foreach (var item in res)
                {
                    T_HR_Recommend re = entities.T_HR_Recommend.Find(item.ID);
                    re.Valid = false;
                    // entities.T_HR_Fund.Remove(fu);
                }
            }

            var efs = from o in entities.T_HR_EducationFund
                      where o.StaffID == staffid && o.Valid == true
                      select o;
            if (efs.Any())
            {
                foreach (var item in efs)
                {
                    T_HR_EducationFund ef = entities.T_HR_EducationFund.Find(item.ID);
                    ef.Valid = false;
                    // entities.T_HR_Fund.Remove(fu);
                }
            }

            var cos = from o in entities.T_HR_Contract
                      where o.StaffID == staffid && o.Valid == true
                      select o;
            if (cos.Any())
            {
                foreach (var item in cos)
                {
                    T_HR_Contract co = entities.T_HR_Contract.Find(item.ID);
                    co.Valid = false;
                    // entities.T_HR_Fund.Remove(fu);
                }
            }

            var users = from o in entities.T_PE_Users
                      where o.EmployeeID == staffid && o.Activity == true
                      select o;
            if (users.Any())
            {
                foreach (var item in users)
                {
                    T_PE_Users us = entities.T_PE_Users.Find(item.UserID);
                    us.Activity = false;
                    // entities.T_HR_Fund.Remove(fu);
                }
            }

                entities.SaveChanges();

        }



    }
}
