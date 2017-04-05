using DeerInformation.Models;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeerInformation.Areas.gyproject.ShareMethod
{
    public enum JudgeResualt
    {
        HaveNull, IsNumNegative, IsPriceNegative, IsIllegal, IsLegal
    }
    public class CommonWay
    {
        Entities DB = new Entities();
        public CommonWay()
        {

        }
        /// <summary>
        /// 检查缓存表里物料数量是否都为零
        /// Check whether the number of materials in the Temp_table is zero
        /// </summary>
        /// <param name="no">单号主键</param>
        /// <returns>true订单配置完成，无需配置</returns>
        public bool check(string no)
        {
            bool flag = true;
            var de = DB.T_GM_TempDetailMaterial.Where(m => m.Remark == no).ToList();
            foreach (var item in de)
            {
                if (item.Num != 0)
                {
                    flag = false;
                    break;
                }
            }
            return flag;
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
        /// Converts the string list to decimal list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<decimal> StrtoDec(List<string> list)
        {
            List<decimal> result = new List<decimal>();
            foreach (var item in list)
            {
                decimal t;
                if (decimal.TryParse(item, out t) && t != 0)
                {
                    result.Add(t);   
                }
                else
                {
                    break;
                }         
            }
            return result;
        }
        /// <summary>
        /// Judge a decimal list whether has negative
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool IsPositiveNumArr(List<decimal> list)
        {
            foreach (var item in list)
            {
                if (item<0)
                {
                    return false;
                }
            }
            return true;
        }
        public DecResualt Judge(string list, string pri, string record)
        {
            DecResualt dr = new DecResualt();
            List<string> a = JsontoList(list);
            List<string> b = JsontoList(pri);
            List<string> c = JsontoList(record);
            List<decimal> num = StrtoDec(c);
            List<decimal> price = StrtoDec(b);
            dr.Material = a;
            dr.Num = num;
            dr.Price = price;
            if (!IsPositiveNumArr(num))
            {
                dr.JresualtName = JudgeResualt.IsNumNegative;
            }
            if (!IsPositiveNumArr(price))
            {
                dr.JresualtName = JudgeResualt.IsPriceNegative;
            }
            if (a.Count > 0 && a[0] != "" && a[0] != "null" && a.Count == num.Count && a.Count == price.Count)
            {
                dr.JresualtName = JudgeResualt.IsLegal;
            }
            else
            {
                dr.JresualtName = JudgeResualt.IsIllegal;
            }
            return dr;
        }
        /// <summary>
        /// view storage and No
        /// </summary>
        /// <param name="id"></param>
        /// <param name="MasterWarehouseID"></param>
        /// <returns></returns>
        public List<object> ViewStorageAndNo(string id, string WarehouseID)
        {
			//string gid = id.Substring(0, id.IndexOf(","));
			//string warehouseid = id.Substring(id.IndexOf(",") + 1);
			string gid = id, warehouseid=WarehouseID;
            var list = DB.V_GM_TempMaterial.Where(w => w.Remark == gid).ToList();
            var mwid = DB.T_GM_MaterialStock.Where(w => w.WarehouseID == warehouseid);
            List<object> mix = new List<object>();
            foreach (var item in list)
            {
                var m = mwid.Where(w => w.MaterialID == item.MaterialID).ToList().FirstOrDefault();
                if (m != null)
                {
                    mix.Add(new
                    {
                        Brand = item.Brand,
                        MaterialID = item.MaterialID,
                        MaterialName = item.MaterialName,
                        Size = item.Size,
                        Unit = item.Unit,
                        PicturePath = item.PicturePath,
                        PurchaseCycle = item.PurchaseCycle,
                        MinPurchase = item.MinPurchase,
                        Price = item.Price,
                        Num = item.Num,
                        CurAmount = m.CurAmount
                    });
                }
                else
                {
                    mix.Add(new
                    {
                        Brand = item.Brand,
                        MaterialID = item.MaterialID,
                        MaterialName = item.MaterialName,
                        Size = item.Size,
                        Unit = item.Unit,
                        PicturePath = item.PicturePath,
                        PurchaseCycle = item.PurchaseCycle,
                        MinPurchase = item.MinPurchase,
                        Price = item.Price,
                        Num = item.Num,
                        CurAmount = 0
                    });
                }
            }
            return mix;
        }
		/// <summary>
		/// 申请请购单时查看报价单物料和库存情况
		/// </summary>
		/// <param name="id"></param>
		/// <param name="warehouseId"></param>
		/// <returns></returns>
		public List<object> ViewStorageAndBudgt(string id, string warehouseId)
		{
			string gid = id, warehouseid = warehouseId;
			var list = DB.V_GM_TempMaterial.Where(w => w.Remark == gid).Where(l=>l.Num>0).ToList();
			var mwid = DB.T_GM_MaterialStock.Where(w => w.WarehouseID == warehouseid);
			List<object> mix = new List<object>();
			foreach (var item in list)
			{
				var m = mwid.Where(w => w.MaterialID == item.MaterialID).ToList().FirstOrDefault();
				if (m != null)
				{
					mix.Add(new
					{
						Brand = item.Brand,
						MaterialID = item.MaterialID,
						MaterialName = item.MaterialName,
						Size = item.Size,
						Unit = item.Unit,
						PicturePath = item.PicturePath,
						PurchaseCycle = item.PurchaseCycle,
						MinPurchase = item.MinPurchase,
						Price = item.Price,
						Num = item.Num,
						CurAmount = m.CurAmount
					});
				}
				else
				{
					mix.Add(new
					{
						Brand = item.Brand,
						MaterialID = item.MaterialID,
						MaterialName = item.MaterialName,
						Size = item.Size,
						Unit = item.Unit,
						PicturePath = item.PicturePath,
						PurchaseCycle = item.PurchaseCycle,
						MinPurchase = item.MinPurchase,
						Price = item.Price,
						Num = item.Num,
						CurAmount = 0
					});
				}
			}
			return mix;
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
        public List<ListItem> SetCombox_CGM(bool type = true)
        {
            //DateTime t1 = DateTime.Now.AddDays(-3);
            //DateTime t2 = DateTime.Now.AddDays(7);
            List<ListItem> li = new List<ListItem>();
            if (type)
            {
                var list = DB.V_GM_MPurchase.Where(m => m.IsEnableNo == "审核通过").Where(m => m.OrderStatu == null).ToList();//Where(m => m.PrepaidDay < t2).Where(m => m.PrepaidDay > t1).
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
                var list = DB.V_GM_FAPurchase.Where(m => m.Description == "审核通过").Where(m => m.Status == null).ToList();//.Where(m => m.Status != "delivered").Where(m => m.No_Date < t2).Where(m => m.No_Date > t1).Where(m => m.Description == "审核通过")
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
        /// <summary>
        /// set SQM combox value
        /// </summary>
        /// <param name="type">true为物料，false为固定资产</param>
        /// <returns></returns>
        public List<ListItem> SetCombox_SQM(bool type = true)
        {
            List<ListItem> li = new List<ListItem>();

            if (type)
            {
                var list = DB.V_GM_MApply.Where(m => m.IsEnableNo == "审核通过").Where(m => m.ApplyMState == null).ToList();
                foreach (var item in list)
                {
                    ListItem listitem = new ListItem();
                    listitem.Value = item.GID;
                    listitem.Text = item.ApplyMaterialNo;
                    li.Add(listitem);
                }
            }
            else
            {
                var list = DB.V_GM_FAApply.Where(m => m.Description == "审核通过").Where(m => m.Status == null).ToList();
                foreach (var item in list)
                {
                    ListItem listitem = new ListItem();
                    listitem.Value = item.GID;
                    listitem.Text = item.ApplyNo;
                    li.Add(listitem);
                }
            }
            return li;
        }
        /// <summary>
        /// 设置项目编号下拉框
        /// set project combox value
        /// </summary>
        /// <param name="statu"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<ListItem> SetCombox_PNo(string statu = "未开工",bool flag=true)
        {
            List<ListItem> li = new List<ListItem>();
            List<T_GM_Budget> list = new List<T_GM_Budget>();
            if (flag)
            {
                list = DB.T_GM_Budget.Where(w => w.BudgetStatus == "施工中").ToList();
            }
            else
            {
                list = DB.T_GM_Budget.Where(w => w.BudgetStatus == statu ).ToList();    
            }
            foreach (var item in list)
            {
                ListItem listitem = new ListItem();
                listitem.Value = item.ProjectNo;
                //listitem.Text = item.ProjectName;
                li.Add(listitem);
            }
            return li;
        }
        /// <summary>
        /// 设置供应商下拉框
        /// set supplier combox
        /// </summary>
        /// <returns></returns>
        public List<ListItem> SetCombox_SupplierNo()
        {
            List<ListItem> li = new List<ListItem>();
            foreach (var item in DB.T_GM_SupplierInfo.ToList())
            {
                ListItem list = new ListItem();
                list.Value = item.SupplierID;
                list.Text = item.SupplierName;
                li.Add(list);
            }
            return li;
        }
        public List<ListItem> SetCombox_WarehouseID()
        {
            List<ListItem> li = new List<ListItem>();
            foreach (var item in DB.T_GM_Warehouse.ToList())
            {
                ListItem list = new ListItem();
                list.Value = item.WarehouseID;
                list.Text = item.WarehouseName;
                li.Add(list);
            }
            return li;
        }
    }
    public class DecResualt
    {
        public JudgeResualt JresualtName
        {
            get;
            set;
        }
        public List<decimal> Num
        {
            get;
            set;
        }
        public List<decimal> Price
        {
            get;
            set;
        }
        public List<string> Material
        {
            get;
            set;
        }
    }
}