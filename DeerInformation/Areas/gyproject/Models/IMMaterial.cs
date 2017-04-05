using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Models;
using DeerInformation.BaseType;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.gyproject.Models
{
    public class IMMaterial
    {

        [Field(FieldLabel = "出库单编号", AllowBlank = false)]
        public string IMID { get; set; }
        [Field(FieldLabel = "入库日期", AllowBlank = false)]
        public System.DateTime IMDate { get; set; }
        public int IMTypeID { get; set; }
        [Field(FieldLabel = "项目编号", AllowBlank = false)]
        public string ProjectID { get; set; }
        [Field(FieldLabel = "采购单编号", AllowBlank = false)]
        public string PurchaseMNo { get; set; }
        [Field(FieldLabel = "调拨单编号", AllowBlank = false)]
        public string AllotmentNo { get; set; }
        public string Operator { get; set; }
        public System.DateTime OperationTime { get; set; }
        public string OperationListID { get; set; }

        [Field(FieldLabel = "审核流程", AllowBlank = false)]
        public string CheckFlowId { get; set; }

        public string CheckFunName;

        public List<ListItem> CheckFlowItems
        {
            get
            {
                return
                    _entities.V_CH_Checkfuncflow.Where(l => l.CheckfuncName == CheckFunName).Select(l => new ListItem() { Text = l.Name, Value = l.ID })
                        .ToList()
                    ;
            }
        }
        public List<ListItem> Warehouse
        {
            get
            {
                return
                    _entities.T_GM_Warehouse.Select(l => new ListItem() { Text = l.WarehouseName, Value = l.WarehouseID })
                        .ToList()
                    ;
            }
        }

        public List<ListItem> CheckStateItems
        {
            get
            {
                return
                     _entities.T_CH_Check_state.Where(l => l.ID == (int)CheckState.Checking || l.ID == (int)CheckState.Approved || l.ID == (int)CheckState.Rejected).Select(l => new ListItem() {Text = l.Description,Value = SqlFunctions.StringConvert((double)l.ID)})
                        .ToList()
                    ;
            }
        }

        private readonly Entities _entities = new Entities();

        public IMMaterial()
        {
            IMID = "IMW" + DateTime.Now.ToString("yyyyMMdd") + SerialNum.NewSerialNum();
        }

        internal List<dynamic> IMLst(string IMNO,string IMType,string CheckState)
        {
            return _entities.V_GM_IMWithStateDsp.Where(l => l.IMID.Contains(IMNO) && l.IMType.Contains(IMType) && l.CheckStateDsp.Contains(CheckState)).ToList<dynamic>();
        }

        private IMMaterial ToIMMaterial(T_GM_IMWarehouse src)
        {
            foreach (var item in src.GetType().GetProperties())
            {
                item.SetValue(this, item.GetValue(src, null));
            }
            return this;
        }

        public dynamic GetDetailByPurchaseNo(string pId)
        {
            var pItem =
                _entities.V_GM_MPurchase.FirstOrDefault(l => l.PurchaseMNo == pId && l.IsEnableNo == "审核通过");
            if (pItem == null) return null;
            var proitem = _entities.V_GM_DetailProject.FirstOrDefault(l => l.ProjectNo == pItem.ProjectNo);
            if (proitem == null) return null;
            var matLst = _entities.V_GM_DM.Where(l => l.Remark == pItem.GID).ToList();
            var processlst =
                _entities.V_GM_IMDetail.Where(
                    l => l.RefrenceNo == pId && (l.State == null || l.State == (int)CheckState.Checking || l.State == (int)CheckState.Approved)).GroupBy(l => l.MaterialID)
                    .Select(s => new { MaterialID = s.Key, ProcessedNum = s.Sum(p => p.Num) }).ToList();
            var matLstNum =
                matLst.GroupJoin(processlst, a => a.MaterialID, b => b.MaterialID,
                (a, b) => new { a.MaterialID, a.MaterialName, a.Size, a.UID, a.Unit, a.Price, a.PicturePath, a.Num, IMNum = b.Sum(l => l.ProcessedNum.Value), UIMNum = a.Num - b.Sum(l => l.ProcessedNum.Value) })
                    .ToList();
            return new { ProjectNo = pItem.ProjectNo, Warehouse = proitem.WarehouseID, matLst = matLstNum };
        }

        internal bool SubmitPurchase(Controller controller, string purchaseMNo, DateTime date, List<dynamic> store, string imWarehouse)
        {
            try
            {
                var validItem =
                    _entities.V_GM_MPurchase.FirstOrDefault(l => l.PurchaseMNo == purchaseMNo && l.IsEnableNo == "审核通过");
                if (validItem == null)
                {
                    X.Msg.Alert("页面消息", "采购单号不合法或对应的采购单未审核！").Show();
                    return false;
                }
                string IMId = IMID;
                string opeLstId = Guid.NewGuid().ToString();
                _entities.T_GM_IMWarehouse.Add(new T_GM_IMWarehouse()
                {
                    IMID = IMId,
                    IMDate = date,
                    IMTypeID = (int)WarehouseType.Purchase,
                    ProjectID = validItem.ProjectNo,
                    RefrenceNo = purchaseMNo,
                    OperationListID = opeLstId,
                    Operator = new LoginUser().EmployeeId,
                    OperationTime = DateTime.Now,
                    IMWarehouseID = imWarehouse
                });

                if (!WriteStockNum(store, IMID, opeLstId, imWarehouse, "PURIM")) return false;
                if (!WriteCheckOperation(controller, IMId, opeLstId, CheckFlowId)) return false;
                _entities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object GetDetailByAllotmentNo(string aId)
        {
            var aItem =
                _entities.V_GM_EXWithStateDsp.FirstOrDefault(l => l.EXID == aId && (l.State == 2 || l.State == null));
            if (aItem == null) return null;
            var matLst = _entities.V_GM_DM.Where(l => l.Remark == aItem.OperationListID).ToList();
            var processlst =
                _entities.V_GM_IMDetail.Where(
                    l => l.RefrenceNo == aId && (l.State == null || l.State == (int)CheckState.Checking || l.State == (int)CheckState.Approved)).GroupBy(l => l.MaterialID)
                    .Select(s => new { MaterialID = s.Key, ProcessedNum = s.Sum(p => p.Num) }).ToList();
            var matLstNum =
                matLst.GroupJoin(processlst, a => a.MaterialID, b => b.MaterialID,
                (a, b) => new { a.MaterialID, a.MaterialName, a.Size, a.UID, a.Unit, a.Price, a.PicturePath, a.Num, IMNum = b.Sum(l => l.ProcessedNum.Value), UIMNum = a.Num - b.Sum(l => l.ProcessedNum.Value) })
                    .ToList();
            return new { aItem.IMWarehouse, aItem.EXWarehouse, aItem.EXDate, matLst = matLstNum };
        }

        internal bool SubmitAllotment(Controller controller, string allotmentNo, string exWarehouseId, string imWarehouseId, DateTime date, List<dynamic> materiaList)
        {
            try
            {
                var validItem =
                    _entities.V_GM_EXWithStateDsp.FirstOrDefault(l => l.EXID == allotmentNo && (l.State == 2 || l.State == null));
                if (validItem == null)
                {
                    X.Msg.Alert("页面消息", "调拨单号不合法或对应的调拨单未审核！").Show();
                    return false;
                }
                string IMId = IMID;
                string opeLstId = Guid.NewGuid().ToString();
                _entities.T_GM_IMWarehouse.Add(new T_GM_IMWarehouse()
                {
                    IMID = IMId,
                    IMDate = date,
                    IMTypeID = (int)WarehouseType.AllotmentIM,
                    ProjectID = string.Empty,
                    RefrenceNo = allotmentNo,
                    OperationListID = opeLstId,
                    Operator = new LoginUser().EmployeeId,
                    OperationTime = DateTime.Now,
                    EXWarehouseID = exWarehouseId,
                    IMWarehouseID = imWarehouseId
                });

                if (!WriteStockNum(materiaList, IMID, opeLstId, imWarehouseId, "ALMIM")) return false;
                if (!WriteCheckOperation(controller, IMId, opeLstId, CheckFlowId)) return false;
                _entities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal bool SubmitStocktaking(Controller controller, string imId, string imWarehouseId, DateTime date, List<dynamic> materiaList)
        {
            try
            {
                string IMId = imId;
                string opeLstId = Guid.NewGuid().ToString();
                _entities.T_GM_IMWarehouse.Add(new T_GM_IMWarehouse()
                {
                    IMID = IMId,
                    IMDate = date,
                    IMTypeID = (int)WarehouseType.StocktakingIM,
                    ProjectID = string.Empty,
                    OperationListID = opeLstId,
                    Operator = new LoginUser().EmployeeId,
                    OperationTime = DateTime.Now,
                    IMWarehouseID = imWarehouseId
                });
                if(!WriteStockNum(materiaList, IMID, opeLstId, imWarehouseId, "STKIM"))return false;
                if (!WriteCheckOperation(controller, IMId, opeLstId, CheckFlowId)) return false;
                _entities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object GetApplymentMatLst(string id)
        {
            return _entities.V_GM_IMDetail.Where(l => l.IMID == id).ToList();
        }

        public dynamic GetApplymentLst(string id)
        {
            var result = _entities.V_GM_IMWithStateDsp.FirstOrDefault(l => l.IMID == id);
            if (result == null) return null;
            StringBuilder remark = new StringBuilder();
            var checkLst = _entities.V_CH_TaskOperation.Where(l => l.OperationID == result.OperationListID).ToList();
            foreach (var item in checkLst)
            {
                remark.AppendLine(item.State_description + "审核人:" + item.Name + "联系电话:" + item.TelNum + "备注：" + item.Description);
            }
            return new { result, remark = remark.ToString() };
        }

        public bool WriteCheckOperation(Controller controller, string IMID, string operationId, string checkFlowId)
        {
            var lst = _entities.V_CH_Checkfuncflow.FirstOrDefault(l => l.ID == checkFlowId);
            if (lst == null) return false;
            _entities.T_CH_Operation_list.Add(new T_CH_Operation_list()
            {
                ID = operationId,
                Check_flowID = lst.ID,
                Check_funcID = lst.CheckfuncID,
                CreateTime = DateTime.Now,
                Creator = new LoginUser().EmployeeId,
                State = (int)CheckState.Checking,
                Url = controller.Url.Action("CheckIMAction", new { id = IMID })
            });
            return true;
        }

        public bool WriteStockNum(List<dynamic> materiaList, string IMId, string opeLstId,string imWarehouseId, string typeStr)
        {
            foreach (var item in materiaList)
            {
                string matId = item.MaterialID.Value;
                decimal matNum = Convert.ToDecimal(item.ApplyNumber.Value);
                _entities.T_GM_DM.Add(new T_GM_DM()
                {
                    NO = IMId,
                    Type = typeStr,
                    MFlID = item.MaterialID.Value,
                    Num = Convert.ToDecimal(item.ApplyNumber.Value),
                    Remark = opeLstId
                });
                var matItem =
                    _entities.T_GM_MaterialStock.FirstOrDefault(
                        l => l.MaterialID == matId && l.WarehouseID == imWarehouseId);
                if (matItem != null)
                {
					switch (typeStr)
					{
						case "PURIM":
							break;
						case "ALMIM":
							break;
						case "STKIM":
							break;
					}
                }
                else
                {
                    _entities.T_GM_MaterialStock.Add(new T_GM_MaterialStock()
                    {
                        ID = Guid.NewGuid().ToString(),
                        CurAmount = 0,
                        PurchaseAmount = 0,
                        MaterialID = matId,
                        MaxAmount = 0,
                        MinAmount = 0,
                        VirtualAmount = 0,
                        WarehouseID = imWarehouseId
                    });
                }
            }
            return true;
        }

        public bool SubmitCheck(string id, string state, string remark)
        {
            var result =
                _entities.P_CH_ExecuteCheck(id, new LoginUser().EmployeeId, Convert.ToInt32(state), remark).ToList();
            if (result[0] == 0) return false;
            return true;
        }
    }
}