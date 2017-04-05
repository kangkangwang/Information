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
using System.Collections.Generic;
using DeerInformation.Extensions;
namespace DeerInformation.Areas.workyard.Controllers
{
    [DirectController(AreaName = "workyard")]
    public class FieldAttachController : BaseUseController
    {
        //
        // GET: /workyard/FieldAttach/

        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();

        [DirectMethod]
        public ActionResult FieldAttachReload()//重新加载物资信息管理页面
        {
            var list = DB.V_GW_FieldImportantAttach.ToList();
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
        [VisitAuthorize(Read= true)]
        public ActionResult Index()
        {
            var list = DB.V_GW_FieldImportantAttach.ToList();
            foreach (var item in list)
            {
                if (item.AnnetPath != null)
                {
                    item.AnnetPath = Url.Content(item.AnnetPath);
                }
            }
            return View(list);
        }
        [VisitAuthorize(Read = true)]
        public ActionResult FieldInquire(string addno)//精确查询
        {

            if (addno.Length > 0 && addno != "")
            {
                return this.Store(DB.V_GW_FieldImportantAttach.Where(w => w.ProjectNo.Contains(addno)).ToList());
            }
            else
            {
                return this.Store(DB.V_GW_FieldImportantAttach.ToList());
            }
        }

        #region 添加附件信息
        public ActionResult FieldAttachAddView()
        {
            var list = DB.T_GM_TypeNo.ToList();
            List<T_GM_TypeNo> lis = new List<T_GM_TypeNo>();
            foreach (var item in list)
            {
                if (item.TypeID.Contains("FD_"))
                {
                    lis.Add(item);
                }
            }
            foreach (var item in lis)
            {
                FileUploadField fdn = new FileUploadField()
                {
                    ID = item.TypeID.Trim(),
                    LabelWidth = 80,
                    Icon = Icon.Attach,
                    FieldLabel = item.TypeName,
                    ButtonText = "选择文件",
                };
                fdn.AddTo(this.GetCmp<Container>("cc"));
            }
            
            this.GetCmp<Container>("cc").SetActive(true);
            ViewBag.prono = cw.SetCombox_PNo();
            return View();
        }
        [VisitAuthorize(Create = true)]
        public ActionResult FieldDataAddButton()
        {
            WinModule win = new WinModule();
            win.ID = "window2";
            win.Title = "现场资料添加";
            win.Height = 500;
            win.Width = 400;
            win.Loader.Url = Url.Action("FieldAttachAddView", "FieldAttach");
            win.Listeners.Close.Handler = "App.direct.workyard.FieldAttachReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult FieldDataAdd(T_GW_FieldImportantAttach fdm)
        {
            DirectResult result = new DirectResult();
            var list = DB.T_GM_TypeNo.ToList();
            foreach (var item in list)
            {
                if (item.TypeID.Contains("FD_"))
                {
                    fdm.UploadMan = user.EmployeeName;
                    fdm.UploadTime = DateTime.Now;
                    fdm.AnnetType = item.TypeID;
                    string str = UploadAttach(item.TypeID.Trim());
                    if (str != "-1")
                    {
                        fdm.AnnetPath = str;
                        DB.T_GW_FieldImportantAttach.Add(fdm);
                        DB.SaveChanges();
                        result.IsUpload = true;
                    }  
                }     
            }
            return result;
        }
        #endregion

        #region 下载附件
        [VisitAuthorize(Read = true)]
        public ActionResult FileDownload(string filePath)
        {
            download(Server.MapPath(filePath));
            return this.Direct(true);
        }  
        #endregion

    }
}
