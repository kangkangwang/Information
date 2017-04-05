using DeerInformation.Models;
using Ext.Net.MVC;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Areas.gyproject.ShareModule;
using System.IO;
using DeerInformation.Areas.gyproject.Models;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.gyproject.Controllers
{

	[DirectController(AreaName = "gyproject")]

	public class MaterialController : Controller
	{
		Entities DB = new Entities();
		LoginUser user = new LoginUser();
		CommonWay cw = new CommonWay();

		/// <summary>
		/// 将图片的网络地址转成实际物理地址
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		private List<T_GM_InfoMaterial> SetVirtualPath(List<T_GM_InfoMaterial> list)
		{
			foreach (var item in list)
			{
				if (item.PicturePath != null)
				{
					item.PicturePath = Url.Content(item.PicturePath);
				}
			}
			return list;
		}
		[VisitAuthorize(Read = true)]
		public ActionResult Index()//显示所有T_GM_InfoMaterial表里的数据在panel
		{
			var list = DB.T_GM_InfoMaterial.ToList();
			return View(SetVirtualPath(list));
		}

		[DirectMethod]
		public ActionResult MaterialInfoReload()//重新加载物资信息管理页面
		{
			var list = DB.T_GM_InfoMaterial.ToList();
			cw.Reload(SetVirtualPath(list), "MaterialInfoSelect");
			return this.Direct();
		}

		#region 物资信息修改
		public ActionResult MaterialEditView(string id)
		{
			List<ListItem> list = new List<ListItem>();
			foreach (var item in DB.T_PM_MeasuringUnitID.Select(m => m.MeasuringUnit))
			{
				ListItem li = new ListItem();
				li.Value = item;
				list.Add(li);
			}
			ViewBag.unit = list;
			decimal t;
			decimal.TryParse(id, out t);
			return View(DB.T_GM_InfoMaterial.Find(t));
		}
		[VisitAuthorize(Update = true)]
		public ActionResult ClickEdit(string id)
		{
			WinModule win = new WinModule();
			win.ID = "window1";
			win.Title = "物资信息修改";
			win.Height = 450;
			win.Loader.Url = Url.Action("MaterialEditView", "Material", new { ID = id });
			win.Listeners.Close.Handler = "App.direct.gyproject.MaterialInfoReload()";
			win.Render(RenderMode.Auto);
			return this.Direct();
		}
		/// <summary>
		/// 物资修改信息提交
		/// update a record
		/// created by gy
		/// </summary>
		/// <param name="MaterialInfo"></param>
		/// <returns></returns>
		public ActionResult MaterialInfoEdit(T_GM_InfoMaterial MaterialInfo)
		{
			MaterialInfo.InputTime = DateTime.Now;
			MaterialInfo.InputPerson = user.EmployeeId;
			//-------------------------------
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
				string logicpath = "~/AttachFile/MaterialInfo/" + filenewname;
				string filepath = Server.MapPath(logicpath);
				MaterialInfo.PicturePath = logicpath;
				uploadfile.PostedFile.SaveAs(filepath);
			}
			//--------------------------------
			DB.T_GM_InfoMaterial.Attach(MaterialInfo);
			DB.Entry(MaterialInfo).State = EntityState.Modified;
			DB.SaveChanges();
			return this.Direct(true);
		}
		#endregion

		#region 物资信息添加
		public ActionResult MaterialAddView()
		{
			List<ListItem> list = new List<ListItem>();
			foreach (var item in DB.T_PM_MeasuringUnitID.Select(m => m.MeasuringUnit))
			{
				ListItem li = new ListItem();
				li.Value = item;
				list.Add(li);
			}
			ViewBag.unit = list;
			return View();
		}
		[VisitAuthorize(Create = true)]
		public ActionResult MaterialAddButton()
		{
			WinModule win = new WinModule();
			win.ID = "window2";
			win.Title = "物资信息添加";
			win.Height = 450;
			win.Loader.Url = Url.Action("MaterialAddView", "Material");
			win.Listeners.Close.Handler = "App.direct.gyproject.MaterialInfoReload()";
			win.Render(RenderMode.Auto);
			return this.Direct();
		}
		public ActionResult MaterialInfoAdd(T_GM_InfoMaterial MaterialInfo)
		{
			MaterialInfo.InputTime = DateTime.Now;
			MaterialInfo.InputPerson = user.EmployeeId;
			//decimal uid=DB.T_GM_InfoMaterial.ToList().LastOrDefault().UID;
			//MaterialInfo.MaterialID = MaterialInfo.Type + (uid + 1).ToString();

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
				string logicpath = "~/AttachFile/MaterialInfo/" + filenewname;
				string filepath = Server.MapPath(logicpath);
				MaterialInfo.PicturePath = logicpath;
				uploadfile.PostedFile.SaveAs(filepath);
			}
			DB.T_GM_InfoMaterial.Add(MaterialInfo);
			DB.SaveChanges();
			DirectResult r = new DirectResult();
			r.IsUpload = true;
			return r;
		}
		#endregion

		#region 物资信息删除按钮
		[DirectMethod]
		[VisitAuthorize(Delete = true)]
		public ActionResult ClickDelete(string id)
		{
			#region wkk's way
			//X.Msg.Confirm("消息", "确认删除该项?", new MessageBoxButtonsConfig
			//{
			//    Yes = new MessageBoxButtonConfig
			//    {
			//        Handler = string.Format("App.direct.gyproject.Remove('{0}');", id),
			//        Text = "确认"
			//    },
			//    No = new MessageBoxButtonConfig
			//    {
			//        Handler = "",
			//        Text = "取消"
			//    }
			//}).Show();
			//return this.Direct();
			#endregion

			#region my way
			decimal t = 0;
			decimal.TryParse(id, out t);
			var record = DB.T_GM_InfoMaterial.Find(t);
			DB.T_GM_InfoMaterial.Remove(record);
			DB.SaveChanges();
			return this.Direct();
			#endregion

			#region xgw's way
			//string pcid;
			//Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(id);

			//if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
			//{
			//    foreach (Dictionary<string, string> row in values)
			//    {
			//        pcid = row["MaterialID"];
			//        T_GM_InfoMaterial de = DB.T_GM_InfoMaterial.Find(pcid);
			//        if (de != null)
			//        {
			//            DB.T_GM_InfoMaterial.Remove(de);
			//            try
			//            {
			//                DB.SaveChanges();
			//            }
			//            catch (Exception e)
			//            {
			//                X.Msg.Alert("警告", "数据删除失败！<br /> note:" + e.Message).Show();
			//            }
			//        }
			//    }
			//}
			//else
			//{
			//    X.Msg.Alert("提示", "未选择任何列！").Show();
			//}
			//return this.Direct();
			#endregion
		}

		#endregion

		#region 物资查询按钮
		public ActionResult SelectMaterial(string materialname, string brand, string size)
		{
			var result = DB.T_GM_InfoMaterial.ToList();
			if (!string.IsNullOrEmpty(materialname)&&!materialname.Equals("null"))
			{
				result = result.Where(l => l.MaterialName != null&& l.MaterialName.Contains(materialname)).ToList();
			}
			if (!string.IsNullOrEmpty(brand) && !brand.Equals("null"))
			{
				result = result.Where(l =>l.Brand!=null&& l.Brand.Contains(brand)).ToList();
			}
			if (!string.IsNullOrEmpty(size) && !size.Equals("null"))
			{
				result = result.Where(l =>l.Size!=null&& l.Size.Contains(size)).ToList();
			}
			return this.Store(result);

		}

		#endregion

		[DirectMethod]
		public ActionResult MaterialStockReload()//刷新库存信息panel
		{
			try
			{
				var list = DB.T_PM_MaterialStock.ToList();
				var store = X.GetCmp<Store>("MaterialStock");
				store.LoadData(list);
				return this.Direct();
			}
			catch (Exception e)
			{
				X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
				return this.Direct();
			}
			//return RedirectToAction("StockView");
		}

		public ActionResult StockView()
		{
			return View(new Stock());
		}

		public ActionResult StockFilter(StoreRequestParameters parameters, string warehouse, string materialName, string materialSize)
		{
			var originalRes = new Stock().GetStocksByParams(warehouse, materialName, materialSize);
			originalRes.ForEach(l => l.PicturePath = (l.PicturePath == null) ? "" : Url.Content(l.PicturePath));
			return this.Store(originalRes.GetPage(parameters));
		}

		public ActionResult ProjetcFilter(StoreRequestParameters parameters, string keyWord)
		{
			var originalRes = new Stock().GetProjectByParams(keyWord);
			return this.Store(originalRes.GetPage(parameters));
		}

		#region 库存信息查询
		public ActionResult SelectMaterialStock(string materialname, string size, string warehouse)
		{

			if (warehouse.Length > 0)
			{
				if (materialname.Length > 0 && size != "" && size.Length > 0)
				{
					return this.Store(DB.T_PM_MaterialStock.Where(w => w.MaterialName.Contains(materialname)).Where(w => w.Size.Contains(size)).Where(w => w.Warehouse.Contains(warehouse)).ToList());
				}
				else if (materialname.Length <= 0 && size.Length > 0 && size != "")
				{
					return this.Store(DB.T_PM_MaterialStock.Where(w => w.Size.Contains(size)).Where(w => w.Warehouse.Contains(warehouse)).ToList());
				}
				else if (materialname.Length > 0 && size == "")
				{
					return this.Store(DB.T_PM_MaterialStock.Where(w => w.MaterialName.Contains(materialname)).Where(w => w.Warehouse.Contains(warehouse)).ToList());
				}
				else
				{
					return this.Store(DB.T_PM_MaterialStock.Where(w => w.Warehouse.Contains(warehouse)).ToList());
				}
			}
			else
			{
				if (materialname.Length > 0 && size != "" && size.Length > 0)
				{
					return this.Store(DB.T_PM_MaterialStock.Where(w => w.MaterialName.Contains(materialname)).Where(w => w.Size.Contains(size)).ToList());
				}
				else if (materialname.Length <= 0 && size.Length > 0 && size != "")
				{
					return this.Store(DB.T_PM_MaterialStock.Where(w => w.Size.Contains(size)).ToList());
				}
				else if (materialname.Length > 0 && size == "")
				{
					return this.Store(DB.T_PM_MaterialStock.Where(w => w.MaterialName.Contains(materialname)).ToList());
				}
				else
				{
					return this.Store(DB.T_PM_MaterialStock.ToList());
				}
			}
		}

		#endregion

		#region 物资库存信息修改按钮

		public ActionResult MaterialStockEditView(string id)
		{
			return View(DB.T_PM_MaterialStock.Find(id));
		}

		public ActionResult ClickEditStock(string id)
		{
			Window win = new Window
			{
				ID = "window3",
				Title = "库存信息修改",
				Height = 450,
				Width = 700,
				Modal = true,
				CloseAction = CloseAction.Destroy,
				Loader = new ComponentLoader
				{
					Url = Url.Action("MaterialStockEditView", "Material", new { ID = id }),
					DisableCaching = true,
					Mode = LoadMode.Frame
				},
				Listeners =
				{
					Close =
					{
						Handler = "App.direct.gyproject.MaterialStockReload()",
					}
				}
			};
			win.Render(RenderMode.Auto);
			return this.Direct();
		}

		public ActionResult MaterialStockEditMain(T_PM_MaterialStock MaterialStock)
		{
			DB.T_PM_MaterialStock.Attach(MaterialStock);
			DB.Entry(MaterialStock).State = EntityState.Modified;
			DB.SaveChanges();
			return this.Direct(true);
		}

		#endregion

		public ActionResult WarehouseLst()
		{
			return View("Warehouse");
		}
		public ActionResult WarehouseFilter(string key, StoreRequestParameters parameters)
		{
			return this.Store((new Warehouse().WarehouseLst(key).GetPage(parameters)));
		}
		public ActionResult WarehouseOperation(string action, string title, string id)
		{
			WindowModule window = new WindowModule() { Title = title, Loader = { Url = Url.Action("EditWarehouse", new { actiontype = action, id = id }) } };
			window.Render();
			return this.Direct();
		}
		public ActionResult EditWarehouse(string actiontype, string id)
		{
			ViewBag.action = actiontype;
			return View("WarehouseEdit", new Warehouse().Init(actiontype, id));
		}

		public ActionResult GetManager(string query)
		{
			return new StoreResult(new Warehouse().GetEmployee(query));
		}

		public ActionResult WarehouseSubmit(string actiontype, Warehouse warehouse)
		{
			if (!warehouse.IsValid())
			{
				X.Msg.Alert("页面消息", "请正确填写表单！").Show();
				return this.Direct();
			}

			if (warehouse.UpdateWarehouse(actiontype))
			{
				X.Msg.Alert("页面消息", "保存成功！", "parent.select(parent.App.WarehouseStore);parent.App.win.close();").Show();
			}
			else
			{
				X.Msg.Alert("页面消息", "保存失败，可能是输入的仓库ID已被占用！").Show();
			}
			return this.Direct();
		}
	}
}
