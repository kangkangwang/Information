using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using  DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;

namespace DeerInformation.Areas.gyproject.Models
{
    public class Warehouse
    {
        public string UID { get; set; }
        [Field(FieldLabel = "仓库编号", AllowBlank = false)]
        public string WarehouseID { get; set; }
        [Field(FieldLabel = "仓库名称", AllowBlank = false)]
        public string WarehouseName { get; set; }
        [Field(FieldLabel = "仓库地址", AllowBlank = false)]
        public string Location { get; set; }
        [Field(FieldLabel = "管理员", AllowBlank = false)]
        public string Manager { get; set; }

        private Entities _entities;

        public Warehouse()
        {
            _entities=new Entities();
        }

        public List<dynamic> WarehouseLst(string key)
        {
            key = key ?? string.Empty;
           return _entities.V_GM_Warehouse.Where(l => l.WarehouseName.Contains(key)).ToList<dynamic>();
        }

        public bool UpdateWarehouse(string action)
        {
            try
            {
                if (action == "create")
                {
                    if (null!=_entities.T_GM_Warehouse.FirstOrDefault(l=>l.WarehouseID==WarehouseID))
                    {
                        return false;
                    }
                    _entities.T_GM_Warehouse.Add(new T_GM_Warehouse()
                    {
                        UID = UID,
                        WarehouseID = WarehouseID,
                        WarehouseName = WarehouseName,
                        Location = Location,
                        Manager = Manager
                    });
                }
                else
                {
                    var item = _entities.T_GM_Warehouse.Find(UID);
                    item.UID = UID;
                    item.WarehouseID = WarehouseID;
                    item.WarehouseName = WarehouseName;
                    item.Location = Location;
                    item.Manager = Manager;
                }
                _entities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                // ignored
            }
            return false;
        }

        public bool ForbidWarehouse()
        {
            return false;
        }

        public Warehouse Init(string action, string id)
        {
            if (action=="create")
            {
                UID = Guid.NewGuid().ToString();
            }else if (action =="edit")
            {
                var item = _entities.T_GM_Warehouse.Find(id);
                if (item == null) return null;
                UID = item.UID;
                WarehouseID = item.WarehouseID;
                WarehouseName = item.WarehouseName;
                Location = item.Location;
                Manager = item.Manager;
            }
            return this;
        }

        public IEnumerable GetEmployee(object query)
        {
            using (Entities db = new Entities())
            {
                string fitformat = string.Format("%{0}%", query);
                var result = db.T_HR_Staff.Where(se => SqlFunctions.PatIndex(fitformat, se.Name) > 0 || SqlFunctions.PatIndex(fitformat, se.StaffID) > 0)
                    .Select(m => new { Name = m.Name, Manager = m.StaffID })
                    .ToList();
                return result;
            }
        }

        public bool IsValid()
        {
            WarehouseID = WarehouseID.Trim();
            if (Manager==null||WarehouseID.Equals(string.Empty))
            {
                return false;
            }
            return true;
        }
    }
}