using DeerInformation.Models;
using Ext.Net.MVC;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Xsl;
using System.Xml;
using Newtonsoft.Json;
using System.Text;

namespace DeerInformation.Areas.project.Controllers
{
    [DirectController(AreaName = "project")]
    public class StorageApplyController : Controller
    {
        //
        // GET: /project/StorageApply/

        Entities DB = new Entities();

        public ActionResult Index()
        {
            return View(DB.T_PM_StorageApply.ToList());
        }


        public ActionResult StorageApplyCheckView()
        {
            return View(DB.T_PM_StorageApply.ToList());
        }


        #region 材料入库信息查询

        public ActionResult Select(string status)
        {
            var aa = status;
            if (status.Length > 0)
            {
                return this.Store(DB.T_PM_StorageApply.Where(w => w.CheckStatus == status).ToList());
            }
            
            else
            {
                return this.Store(DB.T_PM_StorageApply.ToList());
            }
        }

        #endregion


        #region 入库申请查看按钮

        public ActionResult ClickSee(string id)
        {
            Window win = new Window
            {

                ID = "window1",
                Title = "材料入库申请详细信息",
                Height = 500,
                Width = 1000,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("StorageApplySeeView", "StorageApplyEdit", new { ID = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        #endregion


        #region 入库申请审核按钮

        public ActionResult ClickEdit(string id)
        {
            Window win = new Window
            {

                ID = "window2",
                Title = "材料入库申请信息审核",
                Height = 500,
                Width = 1000,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("StorageApplyEditView", "StorageApplyEdit", new { ID = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.project.StorageApplyCheckReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        [DirectMethod]
        public ActionResult StorageApplyCheckReload()
        {
            return RedirectToAction("StorageApplyCheckView");
        }
        #endregion


        #region 材料入库申请添加按钮

        public ActionResult StorageApplyAddView()
        {
            return View();
        }
        public ActionResult StorageApplyAddButton()
        {
            Window win = new Window
            {
                ID = "window4",
                Title = "材料入库申请添加",
                Height = 550,
                Width = 1000,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("StorageApplyAddView"),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.project.StorageApplyReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }


        [DirectMethod]
        public ActionResult StorageApplyReload()
        {
            return RedirectToAction("Index");
        }
        #endregion


        #region 选择材料按钮

        public ActionResult SelectMaterial()
        {
            Window win = new Window
            {

                ID = "window3",
                Title = "选择材料",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("Index", "AddApplyStorageMaterial"),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.project.GetMaterial()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }


        [DirectMethod]
        public ActionResult GetMaterial()
        {
            if (TempData["id"] == null)
            {
                return this.Direct();
            }
            else
            {
                List<string> b = new List<string>();
                b = TempData["id"] as List<string>;
                List<T_PM_MaterialInfo> data = new List<T_PM_MaterialInfo>();
                foreach (var pp in b)
                {
                    var ddd = DB.T_PM_MaterialInfo.Where(w => w.MaterialID == pp).ToList().First();
                    if (ddd != null)
                        data.Add(ddd);
                }
                var panel = X.GetCmp<Store>("MaterialSelected");
                panel.LoadData(data);

                return this.Direct();
            }
        }

        #endregion


        #region 入库信息提交按钮

        public ActionResult AddMaterialStorageApply(string list, string record, T_PM_StorageApply StorageApply, string applyno, string applyman, string applydate, string extra1, string warehouse)
        {
            List<string> a = new List<string>();
            list = list.Replace("\"", "").Replace("[", "").Replace("]", "");
            a = list.Split(',').ToList();
            a = a as List<string>;

            List<string> b = new List<string>();
            record = record.Replace("\"", "").Replace("[", "").Replace("]", "");
            b = record.Split(',').ToList();
            b = b as List<string>;

            var i = 1;

            List<T_PM_MaterialInfo> data = new List<T_PM_MaterialInfo>();

            foreach (var aa in a)
            {
                var ApplyMaterial = new T_PM_ApplyMaterial();
                ApplyMaterial.ApplyID = applyno;
                ApplyMaterial.ApplyMaterialNo = applyno + i ;//材料申请单中每一个材料的申请编号ID
                ApplyMaterial.MaterialNo = DB.T_PM_MaterialInfo.Where(w => w.MaterialID == aa).ToList().First().MaterialID;
                ApplyMaterial.MaterialName = DB.T_PM_MaterialInfo.Where(w => w.MaterialID == aa).ToList().First().MaterialName;
                ApplyMaterial.Size = DB.T_PM_MaterialInfo.Where(w => w.MaterialID == aa).ToList().First().Size;
                ApplyMaterial.Unit = DB.T_PM_MaterialInfo.Where(w => w.MaterialID == aa).ToList().First().Unit;
                ApplyMaterial.Price = DB.T_PM_MaterialInfo.Where(w => w.MaterialID == aa).ToList().First().Price;
                ApplyMaterial.Extra = DB.T_PM_MaterialInfo.Where(w => w.MaterialID == aa).ToList().First().Extra;
                ApplyMaterial.Warehouse = warehouse;
                ApplyMaterial.ApplyNumber = decimal.Parse(b[0+i-1]);
                DB.T_PM_ApplyMaterial.Add(ApplyMaterial);
                i = i + 1;
            }
            DB.SaveChanges();
            StorageApply.ApplyNo = applyno;
            StorageApply.ApplyMan = applyman;
            StorageApply.ApplyDate = DateTime.Now;
            StorageApply.Extra = extra1;
            StorageApply.Warehouse = warehouse;
            StorageApply.CheckStatus = "未审核";
            DB.T_PM_StorageApply.Add(StorageApply);
            DB.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion
    }
}
