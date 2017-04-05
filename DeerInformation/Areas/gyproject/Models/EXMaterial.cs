using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web.Mvc;
using DeerInformation.BaseType;
using DeerInformation.Extensions;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;

namespace DeerInformation.Areas.gyproject.Models
{
    public class EXMaterial
    {

        [Field(FieldLabel = "出库单编号", AllowBlank = false)]
        public string EXID { get; set; }

        [Field(FieldLabel = "出库日期", AllowBlank = false)]
        public System.DateTime EXDate { get; set; }

        public int EXTypeID { get; set; }

        [Field(FieldLabel = "项目编号", AllowBlank = false)]
        public string ProjectID { get; set; }

        [Field(FieldLabel = "调拨单号", AllowBlank = false)]
        public string AllotmentNo { get; set; }

        public string Operator { get; set; }
        public System.DateTime OperationTime { get; set; }
        public string OperationListID { get; set; }

        [Field(FieldLabel = "入库仓库", AllowBlank = false)]
        public string IMWarehouse { get; set; }

        [Field(FieldLabel = "出库仓库", AllowBlank = false)]
        public string EXWarehouse { get; set; }

        [Field(FieldLabel = "审核流程", AllowBlank = false)]
        public string CheckFlowId { get; set; }

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

        public string CheckFunName;

        public List<ListItem> CheckFlowItems
        {
            get
            {
                return
                    _entities.V_CH_Checkfuncflow.Where(l => l.CheckfuncName == CheckFunName)
                        .Select(l => new ListItem() { Text = l.Name, Value = l.ID })
                        .ToList()
                    ;
            }
        }

        public List<ListItem> CheckStateItems
        {
            get
            {
                return
                     _entities.T_CH_Check_state.Where(l => l.ID == (int)CheckState.Checking || l.ID == (int)CheckState.Approved || l.ID == (int)CheckState.Rejected).Select(l => new ListItem() { Text = l.Description, Value = SqlFunctions.StringConvert((double)l.ID) })
                        .ToList()
                    ;
            }
        }

        private readonly Entities _entities = new Entities();

        public EXMaterial()
        {
            EXID = "EXW" + DateTime.Now.ToString("yyyyMMdd") + SerialNum.NewSerialNum();
            AllotmentNo = "ALM" + DateTime.Now.ToString("yyyyMMdd") + SerialNum.NewSerialNum();
        }

        internal List<dynamic> EXLst(string NO,string EXtype, string checkState)
        {
            return _entities.V_GM_EXWithStateDsp.Where(l => l.EXID.Contains(NO) && l.EXType.Contains(EXtype) && l.CheckStateDsp.Contains(checkState)).ToList<dynamic>();
        }

        internal List<V_GM_DetailMStock> GetNewMaterialLst(List<dynamic> selectList)
        {
            List<string> idLst = new List<string>();
            foreach (var item in selectList)
            {
                idLst.Add(item.ID.ToString());
            }
            var result = _entities.V_GM_DetailMStock.Where(l => idLst.Contains(l.ID)).ToList();
            return result;
        }

        internal bool SubmitRequisition(Controller controller, string projectId, DateTime date, List<dynamic> store, string eXWarehouse)
        {
            try
            {
                string EXId = EXID;
                string opeLstId = Guid.NewGuid().ToString();
                _entities.T_GM_EXWarehouse.Add(new T_GM_EXWarehouse()
                {
                    EXID = EXId,
                    EXDate = date,
                    EXTypeID = (int)WarehouseType.Requisitions,
                    ProjectID = projectId,
                    OperationListID = opeLstId,
                    Operator = new LoginUser().EmployeeId,
                    OperationTime = DateTime.Now,
                    EXWarehouse = eXWarehouse
                });

                if (!WriteStockNum(store, EXId, opeLstId, eXWarehouse, "REQEX")) return false;
                if (!WriteCheckOperation(controller, EXId, opeLstId, CheckFlowId)) return false;
                _entities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal bool SubmitAllotment(Controller controller, string allotmentNo, string eXWarehouseId, string iMWarehouseId, DateTime date,
            List<dynamic> materiaList)
        {
            try
            {
                LoginUser user = new LoginUser();
                string opeLstId = Guid.NewGuid().ToString();
                _entities.T_GM_EXWarehouse.Add(new T_GM_EXWarehouse()
                {
                    EXID = allotmentNo,
                    EXDate = date,
                    EXTypeID = (int)WarehouseType.AllotmentEX,
                    ProjectID = string.Empty,
                    OperationListID = opeLstId,
                    Operator = user.EmployeeId,
                    OperationTime = DateTime.Now,
                    IMWarehouse = iMWarehouseId,
                    EXWarehouse = eXWarehouseId
                });

                if (!WriteStockNum(materiaList, allotmentNo, opeLstId, eXWarehouseId, "ALMEX")) return false;
                if (!WriteCheckOperation(controller, allotmentNo, opeLstId, CheckFlowId)) return false;
                _entities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal bool SubmitStocktaking(Controller controller, string exId, string exWarehouseId, DateTime date, List<dynamic> materiaList)
        {
            try
            {
                LoginUser user = new LoginUser();
                string opeLstId = Guid.NewGuid().ToString();
                _entities.T_GM_EXWarehouse.Add(new T_GM_EXWarehouse()
                {
                    EXID = exId,
                    EXDate = date,
                    EXTypeID = (int)WarehouseType.StocktakingEX,
                    ProjectID = string.Empty,
                    OperationListID = opeLstId,
                    Operator = user.EmployeeId,
                    OperationTime = DateTime.Now,
                    EXWarehouse = exWarehouseId
                });

                if (!WriteStockNum(materiaList, exId, opeLstId, exWarehouseId, "STKEX"))return false;
                if (!WriteCheckOperation(controller, exId, opeLstId, CheckFlowId)) return false;
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
            return _entities.V_GM_EXDetail.Where(l => l.EXID == id).ToList();
        }

        public dynamic GetApplymentLst(string id)
        {
            var result = _entities.V_GM_EXWithStateDsp.FirstOrDefault(l => l.EXID == id);
            if (result == null) return null;
            StringBuilder remark = new StringBuilder();
            var checkLst = _entities.V_CH_TaskOperation.Where(l => l.OperationID == result.OperationListID).ToList();
            foreach (var item in checkLst)
            {
                remark.AppendLine(item.State_description + "审核人:" + item.Name + "联系电话:" + item.TelNum + "备注：" +
                                  item.Description);
            }
            return new { result, remark = remark.ToString() };
        }

        public bool WriteCheckOperation(Controller controller, string EXID, string operationId, string checkFlowId)
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
                Url = controller.Url.Action("CheckEXAction", new { id = EXID })
            });
            return true;
        }

        public bool WriteStockNum(List<dynamic> materiaList, string exId, string opeLstId, string exWarehouseId, string typeStr)
        {
            foreach (var item in materiaList)
            {
                string matId = item.MaterialID.Value;
                decimal matNum = Convert.ToDecimal(item.ApplyNumber.Value);
                _entities.T_GM_DM.Add(new T_GM_DM()
                {
                    NO = exId,
                    Type = typeStr,
                    MFlID = item.MaterialID.Value,
                    Num = Convert.ToDecimal(item.ApplyNumber.Value),
                    Remark = opeLstId
                });
                var matItem =
                    _entities.T_GM_MaterialStock.FirstOrDefault(
                        l => l.MaterialID == matId && l.WarehouseID == exWarehouseId);
                if (matItem == null) return false;
	            switch (typeStr)
	            {
					case "REQEX":
						matItem.VirtualAmount = matItem.VirtualAmount - matNum;
			            break;
					case "ALMEX":
						matItem.VirtualAmount = matItem.VirtualAmount - matNum;
			            matItem.PurchaseAmount = matItem.PurchaseAmount - matNum;
						break;
					case "STKEX":
						matItem.VirtualAmount = matItem.VirtualAmount - matNum;
			            matItem.PurchaseAmount = matItem.PurchaseAmount - matNum;
						break;
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