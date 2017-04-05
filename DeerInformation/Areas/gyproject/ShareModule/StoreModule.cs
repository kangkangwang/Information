using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Data;

namespace DeerInformation.Areas.gyproject.ShareModule
{
    public class StoreModule:Panel
    {
        public GridPanel DeployedMaterial = new GridPanel()
        {
            Title = "已选材料",
            ID="GridPanel2",
            Store =
                {
                    new Store
                    {
                        ID="DeployedMaterial",
                        Model=
                        {
                            new Model{
                                IDProperty="MaterialID",
                                Fields=
                                {
                                        new ModelField("Brand", ModelFieldType.String),
                                        new ModelField("MaterialID", ModelFieldType.String),
                                        new ModelField("MaterialName", ModelFieldType.String),
                                        new ModelField("Size", ModelFieldType.String),
                                        new ModelField("Unit", ModelFieldType.String),
                                        new ModelField("PurchaseCycle", ModelFieldType.String),
                                        new ModelField("MinPurchase", ModelFieldType.String),
                                        new ModelField("Price", ModelFieldType.String),
                                        new ModelField("Num2", ModelFieldType.String),
                                        new ModelField("Pri", ModelFieldType.String)
                                }
                            }
                        },
                        AutoLoad=false,
                        PageSize=10    
                    }
                },

            ColumnModel =
            {
                Columns =
                    {
                        new Column { Text = "物料编号", DataIndex = "MaterialID", Flex=3, ID="column1" },
                        new Column { Text = "物料名称", DataIndex = "MaterialName", Flex=4 , ID="column2"},
                        new ComponentColumn
                        {
                            Text="申请数量",DataIndex="Num2",Editor=true,Flex=3, ID="column3",
                            Component=
                            {
                                new NumberField{AllowBlank=false,EmptyText="数量不能为空"}
                            }
                        },
                        new Column { Text = "品牌", DataIndex = "Brand", Flex=3, ID="column7" },
                        new Column { Text = "采购周期", DataIndex = "PurchaseCycle", Flex=3 , ID="column8"},
                        new Column { Text = "最小采购量", DataIndex = "MinPurchase", Flex=3, ID="column9" },
                        new Column { Text = "型号", DataIndex = "Size", Flex=3, ID="column4" },
                        new Column { Text = "单位", DataIndex = "Unit", Flex=3, ID="column5" },
                        new Column { Text = "预算价", DataIndex = "Price", Flex=3, ID="column6"},
                        new ComponentColumn
                        {
                            Text="实际价格",DataIndex="Pri",Editor=true,Flex=3, ID="PriChange",
                            Component=
                            {
                                new NumberField{AllowBlank=false,EmptyText="价格不能为空"}
                            }
                        },
                    }
            }
        };
    }
}