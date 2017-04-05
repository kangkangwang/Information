using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using DeerInformation.Models;
using Ext.Net;

namespace DeerInformation.Areas.gyproject.Models
{
    public class Stock
    {
        private readonly Entities _entities = new Entities();

        internal List<V_GM_DetailMStock> GetStocksByParams(string warehouseId, string materialName = "", string materialSize = "")
        {
            if (warehouseId==null||materialName==null||materialSize==null)
            {
                return _entities.V_GM_DetailMStock.ToList();
            }
            return _entities.V_GM_DetailMStock.Where(l => l.WarehouseID == warehouseId && l.MaterialName.Contains(materialName) && l.Size.Contains(materialSize)).ToList();
        }

        public List<ListItem> WarehouseItems
        {
            get
            {
                List<ListItem> list = new List<ListItem>();

                foreach (var item in _entities.T_GM_Warehouse)
                {
                    ListItem li = new ListItem { Value = item.WarehouseID,Text = item.WarehouseName};
                    list.Add(li);
                }
                return list;
            }
        }

        public List<V_GM_DetailProject> GetProjectByParams(string keyWord)
        {
            string fitformat = string.Format("%{0}%", keyWord ?? "");
            return _entities.V_GM_DetailProject.Where(l => SqlFunctions.PatIndex(fitformat, l.ClientName) > 0 || SqlFunctions.PatIndex(fitformat, l.ProjectName) > 0).ToList();
        }
    }
}