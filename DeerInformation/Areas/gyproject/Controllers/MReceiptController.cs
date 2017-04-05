using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Data;
using System.Web.Razor.Text;
using DeerInformation.Areas.gyproject.ShareMethod;
using DeerInformation.Areas.gyproject.ShareModule;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.gyproject.Controllers
{
    [DirectController(AreaName = "gyproject")]
    [AllowAnonymous]
    public class MReceiptController : Controller
    {
        //
        // GET: /gyproject/MReceipt/
        Entities DB = new Entities();
        LoginUser user = new LoginUser();
        CommonWay cw = new CommonWay();
        private string checkname = "采购入库";

        [DirectMethod]
        public ActionResult MReceiptReload()//重新加载收货单管理页面
        {
            var list = DB.V_GM_DetailRecieve.ToList();
            cw.Reload(list, "MReceiptStore");
            return this.Direct();
        }

        [DirectMethod]
        [VisitAuthorize(Delete = true)]
        public ActionResult DeleteMReceipt(string id)//删除收货单操作
        {
            var record = DB.T_GM_ReceiptMaterial.Find(id);
            DB.T_GM_ReceiptMaterial.Remove(record);
            var list = DB.T_GM_DM.Where(w => w.Remark == id).ToList();
            foreach (var item in list)
            {
                DB.T_GM_DM.Remove(item);
            }
            DB.SaveChanges();
            return this.Direct();
        }

        //[VisitAuthorize(Read=true,Create =true,Update=true,Delete=true)]
        public ActionResult MReceiptQuery(string addno,string addfield,string start,string end)//获取查询信息
        {
            int res = cw.QueryFirstTwo(addno, addfield);
            bool flag = cw.QueryTime(start, end);
            switch (res)
            {
                case 0:
                    var sel0 = DB.V_GM_DetailRecieve.Where(w => w.ProjectNo == addno).Where(w => w.ReceivePMNo == addfield).ToList();
                    return this.Store(sel0);
                case 1:
                    var sel1 = DB.V_GM_DetailRecieve.Where(w => w.ProjectNo == addno).ToList();
                    return this.Store(sel1);
                case 2:
                    var sel2 = DB.V_GM_DetailRecieve.Where(w => w.ReceivePMNo == addfield).ToList();
                    return this.Store(sel2);
                case 3:
                    if (flag)
                    {
                        DateTime dts = Convert.ToDateTime(start.Replace("\"", ""));
                        DateTime dte = Convert.ToDateTime(end.Replace("\"", ""));
                        var seldatediff = DB.V_GM_DetailRecieve.Where(w => w.ConfirmTime <= dte).Where(w => w.ConfirmTime >= dts).ToList();
                        return this.Store(seldatediff);
                    }
                    else
                    {
                        cw.QueryTime(start, end);
                        return this.Direct();
                    }
                default:
                    return this.Direct();
            }
        }
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            return View(DB.V_GM_DetailRecieve.ToList());
        }

        #region 增加收货单
        [VisitAuthorize(Create = true)]
        public ActionResult MReceiptAddbtn()
        {
            WinModule win = new WinModule();
            win.Title = "新增收货单";
            win.Loader.Url = Url.Action("MReceiptAddView", "MReceipt");
            win.Listeners.Close.Handler = "App.direct.gyproject.MReceiptReload()";
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult MReceiptAddView()
        {
            ViewBag.audit = cw.SetCombox(checkname);
            ViewBag.cgm = cw.SetCombox_CGM();
            return View();
        }

        //[VisitAuthorize(Read=false,Delete=false,Create=true,Update=true)]
        /// <summary>
        /// 注意看注释
        /// </summary>
        /// <param name="list"></param>
        /// <param name="record"></param>
        /// <param name="pri"></param>
        /// <param name="wid"></param>
        /// <param name="rm"></param>
        /// <returns></returns>
        public ActionResult MReceiptSubmit(string list, string record, string pri, T_GM_ReceiptMaterial rm, string AuditProcess)
        {
            List<string> a = cw.JsontoList(list);
            List<string> c = cw.JsontoList(record);
            List<string> b = cw.JsontoList(pri);
            //这里是多张收货单确定一张采购单，与1张申请单配置多张采购单相反，
            //所以数据库中收货表要取采购单的GID才能找到对应记录，而不是订单号
            var te = DB.T_GM_TempDetailMaterial.Where(w => w.Remark == rm.PMNo);
            var de = DB.T_GM_PurchaseMaterial.Find(rm.PMNo);
	        string IMNo = "IMW" + DateTime.Now.ToString("yyyyMMdd") + SerialNum.NewSerialNum();
	        string IMOpId = Guid.NewGuid().ToString();
            int i = 0;
            decimal t,f,totalprice=0;
            if (a.Count>0&&a[0]!="")
            {
                //rm.ReceiptPlace = wid;
                rm.ConfirmMan = user.EmployeeId;
                rm.ConfirmTime = DateTime.Now;
	            rm.OperationLstId = IMOpId;
                
               foreach (var item in a)
                {
                    if (c[i] != "" && b[i] != "" && c[i] != "null" && b[i] != "null")
                    {
						T_GM_DM dm = new T_GM_DM();
						T_GM_DM IMWMat = new T_GM_DM();
                        decimal.TryParse(c[i], out t);
                        decimal.TryParse(b[i], out f);

                        var test = te.FirstOrDefault(m => m.MFlID == item);
                        if (test != null)
                        {
                            test.Num -= t;
                            if (test.Num < 0)
                            {
                                //X.Msg.Alert("警告","" ).Show();
                                return this.Direct(false, "收货实际数量超过采购数量！！！");
                            }
                            DB.T_GM_TempDetailMaterial.Attach(test);
                            DB.Entry(test).State = EntityState.Modified;

                            //入库物料
                            IMWMat.NO = IMNo;
                            IMWMat.Type = "PURIM";
                            IMWMat.Price = f;
                            IMWMat.MFlID = a[i];
                            IMWMat.Num = t;
                            IMWMat.Remark = IMOpId;
                            DB.T_GM_DM.Add(IMWMat);
                        }
                        //收货的物料
                        dm.NO = rm.ReceivePMNo;
                        dm.Type = "SHM";
                        dm.Num = t;
                        dm.Price = f;
                        dm.MFlID = a[i];
                        dm.Remark = rm.ReceivePMNo;
                        DB.T_GM_DM.Add(dm);
	                    
                        i++;
                        totalprice += t * f;
                        
                    }
                    else
                    {
                        X.Msg.Alert("警告", "您输入数量或价格为空！！！").Show();
                        return this.Direct();
                    }
                }
                rm.RMTotalPrice = totalprice;
                DB.T_GM_ReceiptMaterial.Add(rm);

	            var project = DB.V_GM_DetailProject.FirstOrDefault(l => l.ProjectNo == rm.ProjectNo);
	            if (project == null) return this.Direct();
				T_GM_IMWarehouse imw = new T_GM_IMWarehouse();
                imw.IMDate = DateTime.Now;
                imw.IMTypeID = 3;
				imw.IMID = IMNo;
	            imw.IMWarehouseID = project.WarehouseID;
                imw.ProjectID = rm.ProjectNo;
                imw.RefrenceNo = rm.ReceivePMNo;
                imw.Operator = user.EmployeeId;
                imw.OperationTime = DateTime.Now;
				imw.OperationListID = IMOpId;
	            DB.T_GM_IMWarehouse.Add(imw);

                T_CH_Operation_list auditprocess = new T_CH_Operation_list();
                var cf = DB.V_CH_Checkfuncflow.Where(w => w.CheckfuncName == checkname).Where(w => w.ID == AuditProcess).ToList().FirstOrDefault();
                auditprocess.ID = imw.OperationListID;
                auditprocess.Check_funcID = cf.CheckfuncID;
                auditprocess.Check_flowID = cf.ID;
                auditprocess.CreateTime = DateTime.Now;
                auditprocess.State = 1;
				auditprocess.Url = Url.Action("CheckIMAction", "IMWarehouse", new { id = IMNo });
                auditprocess.Creator = user.EmployeeId;
                DB.T_CH_Operation_list.Add(auditprocess);


                DB.SaveChanges();

                if (cw.check(rm.PMNo))
                {
                    de.OrderStatu = "delivered";
                    DB.T_GM_PurchaseMaterial.Attach(de);
                    DB.Entry(de).State = EntityState.Modified;
                    DB.SaveChanges();
                    X.Msg.Alert("提示", "此采购单货物已完全送达", "parent.App.win.close();").Show();
                    return this.Direct();
                }
            }
            else
            {
                X.Msg.Alert("警告", "物料数量不能为空").Show();               
            }
            X.Msg.AddScript("parent.App.win.close();");
            return this.Direct();
        }

        #endregion

    }
}
