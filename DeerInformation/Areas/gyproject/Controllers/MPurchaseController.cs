using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Data;
using DeerInformation.Areas.gyproject.Models;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.BaseType;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.gyproject.Controllers
{
	[DirectController(AreaName = "gyproject")]
	public class MPurchaseController : Controller
	{
		//
		// GET: /gyproject/MPurchase/
		Entities DB = new Entities();
		LoginUser user = new LoginUser();
		CommonWay cw = new CommonWay();
		private string checkname = "调拨出库";
		private string checkname2 = "材料采购";
		private string MasterWarehouseID = "JabilWX001";
		[DirectMethod]
		public ActionResult MPurchaseReload()//重新加载采购单管理页面
		{
			var list = DB.V_GM_MPurchase.ToList();
			cw.Reload(list, "MPurchaseStore");
			return this.Direct();
		}

		[DirectMethod]
        [VisitAuthorize(Delete = true)]
		public ActionResult DeleteMPurchase(string gid)//删除申请单操作
		{
			var record = DB.T_GM_PurchaseMaterial.Find(gid);
			DB.T_GM_PurchaseMaterial.Remove(record);
			var list = DB.T_GM_DM.Where(w => w.Remark == gid).ToList();
			foreach (var item in list)
			{
				DB.T_GM_DM.Remove(item);
			}
			DB.SaveChanges();
			return this.Direct();
		}
        [VisitAuthorize(Read = true)]
		public ActionResult Index()
		{
			return View(DB.V_GM_MPurchase.ToList());
		}
		public ActionResult MPurchaseQuery(string addno, string addfield, string operatorname, string start, string end)//获取查询信息
		{
			List<V_GM_MPurchase> result =DB.V_GM_MPurchase.ToList();
			if (!string.IsNullOrEmpty(operatorname)&&!operatorname.Equals("null"))
			{
				result = result.Where(l => l.Name != null && l.Name.Contains(operatorname)).ToList();
			}
			if (!string.IsNullOrEmpty(addno) && !addno.Equals("null"))
			{
				result = result.Where(l => l.ProjectNo != null && l.ProjectNo.Contains(addno)).ToList();
			}
			if (!string.IsNullOrEmpty(addfield) && !addfield.Equals("null"))
			{
				result = result.Where(l => l.PurchaseMNo != null && l.PurchaseMNo.Contains(addfield)).ToList();
			}
			if (start != "null")
			{
				DateTime dts = Convert.ToDateTime(start.Replace("\"", ""));
				result = result.Where(l => l.OperateTime >= dts).ToList();
			}
			if (end != "null")
			{
				DateTime dte = Convert.ToDateTime(end.Replace("\"", ""));
				result = result.Where(l => l.OperateTime <= dte).ToList();
			}
			return this.Store(result);
			//int res = cw.QueryFirstTwo(addno, addfield);
			//bool flag = cw.QueryTime(start, end);
			//switch (res)
			//{
			//	case 0:
			//		var sel0 = result.Where(w => w.ProjectNo == addno).Where(w => w.PurchaseMNo == addfield).ToList();
			//		return this.Store(sel0);
			//	case 1:
			//		var sel1 = result.Where(w => w.ProjectNo == addno).ToList();
			//		return this.Store(sel1);
			//	case 2:
			//		var sel2 = result.Where(w => w.PurchaseMNo == addfield).ToList();
			//		return this.Store(sel2);
			//	case 3:
			//		if (flag)
			//		{
			//			DateTime dts = Convert.ToDateTime(start.Replace("\"", ""));
			//			DateTime dte = Convert.ToDateTime(end.Replace("\"", ""));
			//			var seldatediff = result.Where(w => w.PrepaidDay <= dte).Where(w => w.PrepaidDay >= dts).ToList();
			//			return this.Store(seldatediff);
			//		}
			//		else
			//		{
			//			cw.QueryTime(start, end);
			//			return this.Direct();
			//		}
			//	default:
			//		return this.Direct();
			//}
		}

		#region 修改采购单,提交操作
		public ActionResult ModifySubmit(string list, string record, string price, string gid)
		{
			List<string> a = cw.JsontoList(list);
			List<string> b = cw.JsontoList(price);
			List<string> c = cw.JsontoList(record);
			T_CH_Operation_list auditprocess = new T_CH_Operation_list();
			var am_old = DB.T_GM_PurchaseMaterial.Find(gid);
			var de = DB.T_GM_ApplyMaterial.Find(am_old.AMGID);
			var te = DB.T_GM_TempDetailMaterial.Where(m => m.Remark == am_old.AMGID);
			var am = new T_GM_PurchaseMaterial();
			var cf = DB.V_CH_Checkfuncflow.Where(w => w.Name == am_old.CheckProcess).ToList().FirstOrDefault();
			if (a.Count > 0 && a[0] != "")
			{
				am_old.OrderStatu = "modified once";
				DB.T_GM_PurchaseMaterial.Attach(am_old);
				DB.Entry(am_old).State = EntityState.Modified;

				am.GID = Guid.NewGuid().ToString();
				am.PurchaseMNo = am_old.PurchaseMNo;
				am.SupplierID = am_old.SupplierID;
				am.OrderType = am_old.OrderType;
				am.OperateTime = DateTime.Now;
				am.Operator = am_old.Operator;
				am.AMGID = am_old.AMGID;
				am.ProjectNo = am_old.ProjectNo;
				am.CheckProcess = am_old.CheckProcess;
				am.Remark = am_old.Remark;
				am.ReceiptMethod = am_old.ReceiptMethod;
				DB.T_GM_PurchaseMaterial.Add(am);
				int i = 0;
				decimal t, f;

				auditprocess.ID = am.GID;
				auditprocess.Check_funcID = cf.CheckfuncID;
				auditprocess.Check_flowID = cf.ID;
				auditprocess.CreateTime = DateTime.Now;
				auditprocess.State = 1;
				auditprocess.Url = Url.Action("MPurchaseAudit", "Share", new { gid = am.GID });
				auditprocess.Creator = user.EmployeeId;
				DB.T_CH_Operation_list.Add(auditprocess);

				foreach (var item in a)
				{
					if (c[i] != "" && b[i] != "" && c[i] != "null" && b[i] != "null")
					{
						var detail = new T_GM_DM();
						detail.Remark = am.GID;
						detail.MFlID = item;
						decimal.TryParse(c[i], out t);
						decimal.TryParse(b[i], out f);
						var test = te.Where(m => m.MFlID == item).ToList().First();
						test.Num -= t;
						if (test.Num < 0)
						{
							//X.Msg.Alert("警告", "采购的数量超过申请单里剩余物料的数量！！！").Show();
							return this.Direct(false, "采购的数量超过申请单里剩余物料的数量！！！");
						}
						DB.T_GM_TempDetailMaterial.Attach(test);
						DB.Entry(test).State = EntityState.Modified;

						detail.Num = t;
						detail.Price = f;
						detail.NO = am.PurchaseMNo;
						detail.Type = "CGM";
						i++;
						DB.T_GM_DM.Add(detail);
					}
					else
					{
						X.Msg.Alert("警告", "您输入数量或价格为空！！！").Show();
						return this.Direct();
					}

				}
				DB.SaveChanges();
				if (cw.check(am.AMGID))
				{
					de.ApplyMState = "deploying";
					DB.T_GM_ApplyMaterial.Attach(de);
					DB.Entry(de).State = EntityState.Modified;
					DB.SaveChanges();
					X.Msg.Alert("提示", "订单已配置完成，请等待审核", "parent.App.window3.close();").Show();
					return this.Direct();
				}
				return this.Direct();
			}
			else
			{
				X.Msg.Alert("警告", "您尚未添加任何物料！！！").Show();
				return this.Direct();
			}
		}

		public ActionResult DeletePurchase(string id)
		{
			if (!TempData.ContainsKey("delPurId"))
			{
				TempData.Add("delPurId", id);
			}
			TempData["delPurId"] = id;
			return this.Direct();
		}
		/// <summary>
		/// 删除（取消）采购单
		/// </summary>
		/// <param name="back"></param>
		/// <returns></returns>
		public ActionResult DelPurOpr(string back)
		{
			object tempObj;

			if (TempData.TryGetValue("delPurId", out tempObj))
			{
				string purId = tempObj.ToString();
				DB.P_GM_DeletePurchaseOrder(purId, back == "yes"); //退回到请购
				X.AddScript("App.direct.gyproject.MPurchaseReload()");
			}
			return this.Direct();
		}

		[DirectMethod]
		public ActionResult MPurchaseAckDate(string mpNo)
		{
			var mPurchase = new MPurchase();
			if (!mPurchase.Refresh(mpNo)) return this.Direct();
			this.GetCmp<TextField>("SupplierName").SetValue(mPurchase.SupplierName);
			this.GetCmp<DateField>("SupplierAckDate").SetValue(mPurchase.SupplierAckDate);
			return this.Direct();
		}
		public ActionResult MPurSupAckDate()
		{
			WinModule window = new WinModule {Title = "供应商回复时间",Width = 400,Height = 250,Loader = {Url = Url.Action("AckDate")}};
			window.Render(RenderMode.Auto);
			return this.Direct();
		}
		public ActionResult AckDate()
		{
			return View("SupplierAckDate",new MPurchase());
		}

		public ActionResult SubmitSupplierAckDate(MPurchase mPurchase)
		{
			bool flag = mPurchase.Save();
			X.Msg.Alert("页面信息", flag ? "保存成功" : "保存失败！", flag?"parent.App.win.close()":"").Show();
			return this.Direct();
		}
		#endregion

		#region 配置供应商,新增采购单
        [VisitAuthorize(Create = true)]
		public ActionResult MDeploy()
		{
			WinModule win = new WinModule();
			win.Title = "配置供应商";
			win.Loader.Url = Url.Action("MDeployView", "MPurchase");
			win.Listeners.Close.Handler = "App.direct.gyproject.MPurchaseReload()";
			win.Render(RenderMode.Auto);
			return this.Direct();
		}
		public ActionResult MDeployView()
		{
			ViewBag.supply = cw.SetCombox_SupplierNo();
			ViewBag.SQM = cw.SetCombox_SQM();
			ViewBag.audit = cw.SetCombox(checkname2);
			return View();
		}
		//don't worry about modify the Apply_No,though modify opreation would use the trigger and modify the num,
		//they can't use modify without audit,audit Apply_No isn't pass,they can't use Purchase_No
		//只有通过审核的申请单才可以进行配置，尽管申请单号有多个，但通过审核的申请单号只有一个
		public ActionResult MPurchaseSave(string list, string record, string pri, T_GM_PurchaseMaterial am)
		{
			DecResualt dr = new DecResualt();
			dr = cw.Judge(list, pri, record);
			switch (dr.JresualtName)
			{
				case JudgeResualt.IsNumNegative:
					return this.Direct(false, "数量不能为负数");
				case JudgeResualt.IsPriceNegative:
					return this.Direct(false, "价格不能为负数");
				case JudgeResualt.IsIllegal:
					return this.Direct();
				case JudgeResualt.IsLegal:
					break;
				default:
					return this.Direct(false, "非法操作");
			}

			T_CH_Operation_list auditprocess = new T_CH_Operation_list();
			var cf = DB.V_CH_Checkfuncflow.Where(w => w.ID == am.CheckProcess).ToList().FirstOrDefault();
			var de = DB.T_GM_ApplyMaterial.Find(am.AMGID);
			var te = DB.T_GM_TempDetailMaterial.Where(m => m.Remark == am.AMGID);

			am.GID = Guid.NewGuid().ToString();
			am.Operator = user.EmployeeId;
			am.OperateTime = DateTime.Now;
			am.CheckProcess = cf.Name;
			DB.T_GM_PurchaseMaterial.Add(am);

			auditprocess.ID = am.GID;
			auditprocess.Check_funcID = cf.CheckfuncID;
			auditprocess.Check_flowID = cf.ID;
			auditprocess.CreateTime = DateTime.Now;
			auditprocess.State = 1;
			auditprocess.Url = Url.Action("MPurchaseAudit", "Share", new { gid = am.GID });
			auditprocess.Creator = user.EmployeeId;
			DB.T_CH_Operation_list.Add(auditprocess);

			int i = 0;
			foreach (var item in dr.Material)
			{
				var detail = new T_GM_DM();
				detail.Remark = am.GID;
				detail.MFlID = item;
				var test = te.Where(m => m.MFlID == item).ToList().First();
				test.Num -= dr.Num[i];
				if (test.Num < 0)
				{
					//X.Msg.Alert("警告", "采购的数量超过申请单里剩余物料的数量！！！").Show();
					return this.Direct(false, "采购的数量超过申请单里剩余物料的数量！！！");
				}
				DB.T_GM_TempDetailMaterial.Attach(test);
				DB.Entry(test).State = EntityState.Modified;
				detail.Type = "CGM";
				detail.Num = dr.Num[i];
				detail.Price = dr.Price[i];
				detail.NO = am.PurchaseMNo;
				DB.T_GM_DM.Add(detail);
				i++;
			}
			DB.SaveChanges();

			if (cw.check(am.AMGID))
			{
				de.ApplyMState = "deploying";
				DB.T_GM_ApplyMaterial.Attach(de);
				DB.Entry(de).State = EntityState.Modified;
				DB.SaveChanges();
				X.Msg.Alert("提示", "订单已配置完成，请等待审核", "parent.App.win.close();").Show();
				return this.Direct();
			}
			return this.Direct();
		}

		#endregion

		#region 申请仓库调用
        [VisitAuthorize(Create = true)]
		public ActionResult MEXWarehouse()
		{
			WinModule win = new WinModule();
			win.Title = "仓库调拨";
			win.Loader.Url = Url.Action("MEXWarehouseView", "MPurchase");
			//win.Listeners.Close.Handler = "App.direct.gyproject.MPurchaseReload()";
			win.Render(RenderMode.Auto);
			return this.Direct();
		}
		public ActionResult MEXWarehouseView()
		{
			ViewBag.SQM = cw.SetCombox_SQM();
			ViewBag.audit = cw.SetCombox(checkname);
			return View();
		}
		public ActionResult MEXWarehouseSave(string list, string record, string pri, string check, T_GM_EXWarehouse am)
		{
			DecResualt dr = new DecResualt();
			dr = cw.Judge(list, pri, record);
			switch (dr.JresualtName)
			{
				case JudgeResualt.IsNumNegative:
					return this.Direct(false, "数量不能为负数");
				case JudgeResualt.IsPriceNegative:
					return this.Direct(false, "价格不能为负数");
				case JudgeResualt.IsIllegal:
					return this.Direct();
				case JudgeResualt.IsLegal:
					break;
				default:
					return this.Direct(false, "非法操作");
			}
			T_CH_Operation_list auditprocess = new T_CH_Operation_list();
			//var cf = DB.V_CH_Checkfuncflow.Where(w => w.ID == check).ToList().FirstOrDefault();
			var de = DB.T_GM_ApplyMaterial.Find(am.RefrenceNo);
			var te = DB.T_GM_TempDetailMaterial.Where(m => m.Remark == am.RefrenceNo);
			var stock = DB.T_GM_MaterialStock.Where(m => m.WarehouseID == am.EXWarehouse);

			am.IMWarehouse = DB.V_GM_DetailProject.First(l => l.ProjectNo == de.ProjectNo).WarehouseID;
			am.OperationListID = Guid.NewGuid().ToString();
			am.Operator = user.EmployeeId;
			am.OperationTime = DateTime.Now;
			//am.EXTypeID=2;
			DB.T_GM_EXWarehouse.Add(am);

			auditprocess.ID = am.OperationListID;
			auditprocess.Check_funcID = "-1";//cf.CheckfuncID;
			auditprocess.Check_flowID = "-1";//cf.ID;
			auditprocess.CreateTime = DateTime.Now;
			auditprocess.State = (int)CheckState.Approved;
			auditprocess.Url = Url.Action("MEXWarehouseAudit", "Share", new { gid = am.OperationListID });
			auditprocess.Creator = user.EmployeeId;
			DB.T_CH_Operation_list.Add(auditprocess);

			int i = 0;
			foreach (var item in dr.Material)
			{
				var detail = new T_GM_DM();
				detail.Remark = am.OperationListID;
				detail.MFlID = item;

				var test = te.Where(m => m.MFlID == item).ToList().First();
				test.Num -= dr.Num[i];
				if (test.Num < 0)
				{
					return this.Direct(false, "调库的数量超过申请单里剩余物料的数量！！！");
				}
				DB.T_GM_TempDetailMaterial.Attach(test);
				DB.Entry(test).State = EntityState.Modified;

				var stocknum = stock.FirstOrDefault(m => m.MaterialID == item);
				if (stocknum == null) return this.Direct(false, "物料" + item + "的数量超过仓库里剩余物料的数量！！！");
				stocknum.PurchaseAmount -= dr.Num[i];
				if (stocknum.PurchaseAmount < 0) return this.Direct(false, "物料" + item + "的数量超过仓库里剩余物料的数量！！！");

				DB.T_GM_MaterialStock.Attach(stocknum);
				DB.Entry(stocknum).State = EntityState.Modified;

				detail.Type = "CGM";
				detail.Num = dr.Num[i];
				detail.Price = dr.Price[i];
				detail.NO = am.EXID;
				DB.T_GM_DM.Add(detail);
				i++;
			}
			DB.SaveChanges();
			if (cw.check(am.RefrenceNo))
			{
				de.ApplyMState = "deploying";
				DB.T_GM_ApplyMaterial.Attach(de);
				DB.Entry(de).State = EntityState.Modified;
				DB.SaveChanges();
				X.Msg.Alert("提示", "订单已配置完成，请等待审核", "parent.App.win.close();").Show();
				return this.Direct();
			}
			else
			{
				X.Msg.Alert("提示", "调拨单已配置成功！", "parent.App.win.close();").Show();
			}
			return this.Direct();
		}

		#endregion


	}
}
