using System;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Data;
using System.IO;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.workyard.Controllers
{
    [DirectController(AreaName="workyard")]
    public class FieldInfoManagementController : Controller
    {
        //
        // GET: /workyard/FieldInfoManagement/
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();    
        private static string pronoadd = "";
        private static string filednameadd = "";
        private static float xaxes;
        private static float yaxes;

        [DirectMethod]
        public ActionResult FieldInfoReload()//重新加载现场信息管理页面
        {
            var list = DB.T_GW_FieldInfoManagement.ToList();
            cw.Reload(list, "FieldInfoManagement");
            return this.Direct();
        }

        [DirectMethod]
        public ActionResult FieldCheckAddReload()//重新加载异常标记页面
        {
            var list = DB.T_GW_MarkInfo.Where(w => w.ProjectNo == pronoadd).ToList();
            cw.Reload(list, "MarkTable");
            return this.Direct(); 
        }

        [DirectMethod]
        public ActionResult FieldCheckHandleReload(decimal id)//重新加载异常处理页面
        {
            var tmp = DB.T_GW_MarkInfo.Find(id).ProjectNo;
            var list = DB.T_GW_MarkInfo.Where(w => w.ProjectNo == tmp).Where(w => w.AuditResult != "审核通过").ToList();
            var store = X.GetCmp<Store>("MarkHandleTable");
            var storepanel = X.GetCmp<Store>("FieldMarkTable");
            store.LoadData(list);
            storepanel.LoadData(list);
            return this.Direct();
            //return RedirectToAction("FieldCheckAddView");
        }

        [DirectMethod]
        public ActionResult FieldCheckAuditReload(decimal id)//重新加载异常审核页面
        {
            var tmp = DB.T_GW_MarkInfo.Find(id).ProjectNo;
            var list = DB.T_GW_MarkInfo.Where(w => w.ProjectNo == tmp).Where(w => w.IsHandled == "已处理").Where(w => w.AuditResult != "审核通过").ToList().ToList();
            var chartstore = X.GetCmp<Store>("AuditMarkChart");
            chartstore.LoadData(list);
            var store = X.GetCmp<Store>("AuditMarkTable");
            store.LoadData(list);
            return this.Direct();
            //return RedirectToAction("FieldCheckAddView");
        }

        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
	        var result =
		        DB.T_GW_FieldInfoManagement.Join(DB.V_GM_DetailProject, l => l.ProjectNo, s => s.ProjectNo, (a, b) =>
			        new
			        {
				        a.ID,
				        a.ProjectNo,
				        a.ProjectName,
				        a.FieldName,
				        a.WarehouseID,
				        a.FieldPrincipal,
				        a.FieldMapName,
				        a.FieldMapPath,
				        a.InputMan,
				        a.InputTime,
				        a.Remark,
				        a.CustomRating,
				        b.CusEnd
			        }).ToList();
			return View(result);
        }

        public ActionResult FieldInfoInquire(string addno, string addfield)//精确查询,根据用户的查询习惯进行修改
        {
            var selres1 = DB.T_GW_FieldInfoManagement.Where(w => w.ProjectNo.Contains(addno)).Where(w => w.FieldName.Contains(addfield)).ToList();
            var selres2 = DB.T_GW_FieldInfoManagement.Where(w => w.ProjectNo.Contains(addno)).ToList();
            if (addno.Length > 0 && addno != "")
            {
                if (addfield != "" && addfield.Length > 0)
                {
                    this.GetCmp<Store>("FieldInfoManagement").LoadData(selres1);
                    return this.Direct();
                }
                else
                {
                    return this.Store(selres2);
                }
            }
            else
            {
                if (addfield != "" && addfield.Length > 0)
                {
                    return this.Store(DB.T_GW_FieldInfoManagement.Where(w => w.FieldName.Contains(addfield)).ToList());
                }
                else
                {
                    return this.Store(DB.T_GW_FieldInfoManagement.ToList());
                }
            }
        }

        #region 现场信息添加
        public ActionResult FieldInfoAddView()
        {
            ViewBag.warehouse = cw.SetCombox_WarehouseID();
            ViewBag.prono = cw.SetCombox_PNo("未开工", false);
            return View();
        }

        [VisitAuthorize(Create = true)]
        public ActionResult FieldInfoAddButton()
        {
            WinModule win = new WinModule();
            win.ID = "window1";
            win.Title = "现场信息添加";
            win.Height = 280;
            win.Width = 380;
            win.Loader.Url = Url.Action("FieldInfoAddView", "FieldInfoManagement");
            win.Listeners.Close.Handler = "App.direct.workyard.FieldInfoReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult FieldInfoAdd(T_GW_FieldInfoManagement fim)
        {
            var list = DB.T_GM_Warehouse.Where(w => w.WarehouseID == fim.WarehouseID).ToList().FirstOrDefault();
            if (list==null)
            {
                X.Msg.Alert("提示", "输入的仓库有误").Show();
                return this.Direct();
            }
            fim.InputMan = user.EmployeeName;
            fim.InputTime = DateTime.Now;

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
                string logicpath = "~/AttachFile/WorkYard/FieldInfo/" + filenewname;
                string filepath = Server.MapPath(logicpath);
                fim.FieldMapPath = logicpath;
                uploadfile.PostedFile.SaveAs(filepath);
            }
            DB.T_GW_FieldInfoManagement.Add(fim);

            T_GW_FieldSchedule fs = new T_GW_FieldSchedule();
            fs.ProjectNo = fim.ProjectNo; 
            DB.T_GW_FieldSchedule.Add(fs);

            var project = DB.V_GM_DetailProject.FirstOrDefault(w => w.ProjectNo == fim.ProjectNo);
            fim.ProjectName = project.ProjectName;
            DB.SaveChanges();
            DirectResult result = new DirectResult();
            result.IsUpload = true;
            return result;
        }

        #endregion
        
        #region 现场进度计划表添加
        public ActionResult FieldScheduleAddView(string id)
        {
            X.GetCmp<Panel>("").BaseCls = "";
            return View(DB.T_GW_FieldSchedule.Find(id));
        }

        [VisitAuthorize(Create = true)]
        public ActionResult FieldScheduleAddButton(string id)
        {
            Window win = new Window
            {
                ID = "window2",
                Title = "现场进度计划表",
                Height = 320,
                Width = 680,
                Modal = true,
                Constrain = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("FieldScheduleAddView", "FieldInfoManagement", new { ID=id}),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
            };
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult FieldScheduleAdd(T_GW_FieldSchedule fs)
        {
            fs.InputMan = user.EmployeeName;
            fs.InputTime = DateTime.Now;
            DB.T_GW_FieldSchedule.Attach(fs);
            DB.Entry(fs).State = EntityState.Modified;
            DB.SaveChanges();
            return this.Direct();
        }

        #endregion

        #region 现场进度计划表查看
        public ActionResult FieldScheduleSeeView(string id)
        {
            return View(DB.T_GW_FieldSchedule.Find(id));
        }

        [VisitAuthorize(Read = true)]
        public ActionResult FieldScheduleSee(string id)
        {
            Window win = new Window
            {
                ID = "window3",
                Title = "现场进度计划表",
                Height = 320,
                Width = 680,
                Constrain = true,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("FieldScheduleSeeView", "FieldInfoManagement", new { ID = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
            };
            win.Render(RenderMode.Auto);
            return this.Direct();
        }    

        #endregion

        #region 现场异常标记

        [DirectMethod]
        [VisitAuthorize(Create = true)]
        public ActionResult RenderWin(float x, float y)
        {
            xaxes = x;
            yaxes = y;      
            return new Ext.Net.MVC.PartialViewResult { ViewName = "FieldCheckWindow" };
        } 
        public ActionResult FieldCheckAddView(string id,string id2)
        {
            ViewBag.prono = id;
            ViewBag.fieldname = id2;
            string path = DB.T_GW_FieldInfoManagement.Where(w => w.ProjectNo == id).ToList().FirstOrDefault().FieldMapPath;
            ViewBag.cls1 = Url.Content(path);
            pronoadd = id;
            filednameadd = id2;
            return View(DB.T_GW_MarkInfo.Where(w => w.ProjectNo == id).ToList());
        }
        [VisitAuthorize(Create = true)]
        public ActionResult FieldCheckAdd(string id,string id2)
        {
            Window win = new Window
            {
                ID = "window4",
                Title = "现场异常标记表",
                Height = 650,
                Width = 680,
                Modal = true,
                Constrain=true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("FieldCheckAddView", "FieldInfoManagement", new { ID = id ,ID2=id2}),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
            };           
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        [DirectMethod]
        public ActionResult FCAddSubmit(string wl1, string ma1,string ed1,string es1,string rm1)
        {
            var markinfo = new T_GW_MarkInfo();
            var uploadfile = this.GetCmp<FileUploadField>("fileup");
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
                string logicpath = "~/AttachFile/WorkYard/ExceptionMark/" + filenewname;
                string filepath = Server.MapPath(logicpath);
                markinfo.ExcPicPath = logicpath;
                uploadfile.PostedFile.SaveAs(filepath);
            }
            
            markinfo.IsHandled = "未处理";
            markinfo.AuditResult = "未审核";
            markinfo.ReportMan = user.EmployeeName;
            markinfo.ReportTime = DateTime.Now;
            markinfo.ProjectNo = pronoadd;
            markinfo.WorkPlace = filednameadd;
            markinfo.Xaxes = xaxes;
            markinfo.Yaxes = yaxes;
            markinfo.WarnLevel = wl1;
            markinfo.ExceptionDes = ed1;
            markinfo.ExceptionHandleSug = es1;
            markinfo.Mark = ma1;
            markinfo.Remark = rm1 ;  
            DB.T_GW_MarkInfo.Add(markinfo);
            DB.SaveChanges();
            return this.Direct();
        }

        #endregion

        #region 现场异常处理
        //如果需要添加不同颜色,形状的点，可以在数据库中重新添加变量(x1,y1),(x2,y2)等，再在前台用ScatterSeries即可
        public ActionResult FieldCheckHandleView(string id)
        {
            string path = DB.T_GW_FieldInfoManagement.Where(w => w.ProjectNo == id).ToList().FirstOrDefault().FieldMapPath;
            ViewBag.cls = Url.Content(path);
            return View(DB.T_GW_MarkInfo.Where(w => w.ProjectNo == id).Where(w => w.AuditResult != "审核通过" ).ToList());       
        }

        [VisitAuthorize(Update = true)]
        public ActionResult FieldCheckHandleBtn(string id)
        {
            Window win = new Window
            {
                ID = "window5",
                Title = "现场异常处理",
                Height = 800,
                Width = 680,
                Constrain = true,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("FieldCheckHandleView", "FieldInfoManagement", new { ID = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
            };
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        [VisitAuthorize(Update = true)]
        public ActionResult FieldCheckHandle(decimal id)
        {
            Window win = new Window
            {
                ID = "window7",
                Title = "现场异常处理",
                Height = 450,
                Width = 350,
                Modal = true,
                Constrain = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("FieldCheckHandleWin", "FieldInfoManagement", new { ID = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.workyard.FieldCheckHandleReload(" + id + ")",
                    }
                }
            };
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult FieldCheckHandleWin(decimal id)
        {        
            return View(DB.T_GW_MarkInfo.Find(id));
        }
        public ActionResult FCHandleSubmit(T_GW_MarkInfo tgm)//数据库
        {
            var uploadfile = this.GetCmp<FileUploadField>("filehandleup");
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
                string logicpath = "~/AttachFile/WorkYard/ExceptionHandle/" + filenewname;
                string filepath = Server.MapPath(logicpath);
                tgm.HandlePicPath = logicpath;
                uploadfile.PostedFile.SaveAs(filepath);
            }

            tgm.IsHandled = "已处理";
            tgm.HandleMan = user.EmployeeName;
            tgm.AuditResult = "未审核";
            tgm.HandleTime = DateTime.Now;
            DB.T_GW_MarkInfo.Attach(tgm);
            DB.Entry(tgm).State = EntityState.Modified;
            DB.SaveChanges();
            return this.Direct();
        }
        #endregion

        #region 现场异常处理审核
        public ActionResult FieldCheckAuditView(string id)
        {
            string path = DB.T_GW_FieldInfoManagement.Where(w => w.ProjectNo == id).ToList().FirstOrDefault().FieldMapPath;
            ViewBag.cls = Url.Content(path);
            return View(DB.T_GW_MarkInfo.Where(w => w.ProjectNo == id).Where(w => w.IsHandled == "已处理").Where(w => w.AuditResult != "审核通过").ToList());
        }

        [VisitAuthorize(Delete = true)]
        public ActionResult FieldCheckAuditBtn(string id)
        {
            Window win = new Window
            {
                ID = "window6",
                Title = "现场异常审核",
                Height = 800,
                Width = 680,
                Modal = true,
                Constrain = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("FieldCheckAuditView", "FieldInfoManagement", new { ID = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
            };
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        [VisitAuthorize(Delete = true)]
        public ActionResult FieldCheckAudit(decimal id)
        {
            Window win = new Window
            {
                ID = "window8",
                Title = "现场异常审核",
                Height = 450,
                Width = 350,
                Modal = true,
                Constrain = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("FieldCheckAuditWin", "FieldInfoManagement", new { ID = id}),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.workyard.FieldCheckAuditReload(" + id + ")",
                    }
                }
            };
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult FieldCheckAuditWin(decimal id)
        {
            return View(DB.T_GW_MarkInfo.Find(id));
        }

        public ActionResult FCAuditSubmit(T_GW_MarkInfo tgma,string asu)//数据库
        {
            tgma.AuditResult = asu;
            if (asu == "审核驳回")
            {
                tgma.IsHandled = "未处理";
            }
            tgma.AuditMan = user.EmployeeName;
            tgma.AuditTime = DateTime.Now;
            DB.T_GW_MarkInfo.Attach(tgma);
            DB.Entry(tgma).State = EntityState.Modified;
            DB.SaveChanges();
            return this.Direct();
        }
        #endregion


    }
}
