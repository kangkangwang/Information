using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using Ext.Net.MVC;
using System.Web.Mvc;
using DeerInformation.Models;
using System.Collections;

namespace DeerInformation.Extensions
{
    public class GridModule:Panel
    {
        //public GridModule()
        //{
        //    Title = "test";
        //    ID = "gp1";
        //    Height = 250;
        //    AutoScroll = true;

        //    StoreCollection <Store> Store= new StoreCollection<Store>() 
        //    {
        //        new Store
        //        {
        //            ID="MaterialSelected1",
        //            Model=
        //            {
        //                new Model
        //                {
        //                    IDProperty="MaterialID",
        //                    Fields=
        //                    {
        //                            new ModelField("MaterialID", ModelFieldType.String),
        //                            new ModelField("MaterialName", ModelFieldType.String),
        //                            new ModelField("Num", ModelFieldType.String),
        //                            new ModelField("Size", ModelFieldType.String),
        //                            new ModelField("Unit", ModelFieldType.String),
        //                            new ModelField("Price", ModelFieldType.String)
        //                    }
        //                }
        //            },
        //            AutoLoad=false,
        //            PageSize=10  
        //            }
        //    };
            
        //    GridHeaderContainer ColumnModel = new GridHeaderContainer()
        //    {
        //        Columns =
        //        {
        //            new Column { Text = "物料编号", DataIndex = "MaterialID", Flex=1, ID="column1" },
        //            new Column { Text = "物料名称", DataIndex = "MaterialName", Flex=1 , ID="column2"},
        //            new ComponentColumn
        //            {
        //                Text="数量",DataIndex="Num",Editor=true,Flex=1, ID="column3",
        //                Component=
        //                {
        //                    new TextField{AllowBlank=false,EmptyText="数量不能为空"}
        //                }
        //            },
        //            new Column { Text = "型号", DataIndex = "Size", Flex=1, ID="column4" },
        //            new Column { Text = "单位", DataIndex = "Unit", Flex=1, ID="column5" },
        //            new ComponentColumn 
        //            { 
        //                Text = "价格", DataIndex = "Price", Flex=1,Editor=true, ID="column6",
        //                Component=
        //                {
        //                    new TextField{AllowBlank=false,EmptyText="价格不能为空"}
        //                }
        //            },
        //        }
        //    };
        //}
        public GridModule()
        {

        }

        #region attention this code's different from GridModule()'s way
        //Store and ColumnModel are onlyread's way ,so we change use that
        public GridPanel gf = new GridPanel()
        {
            Height = 220,
            //AutoScroll = true,
            //Title = "已选材料",
            Collapsible=true,
            Store =
                {
                    new Store
                    {
                        ID="MaterialSelected",
                        Model=
                        {
                            new Model{
                                IDProperty="MaterialID",
                                Fields=
                                {
                                        new ModelField("MaterialID", ModelFieldType.String),
                                        new ModelField("MaterialName", ModelFieldType.String),
                                        new ModelField("Num", ModelFieldType.String),
                                        new ModelField("Size", ModelFieldType.String),
                                        new ModelField("Unit", ModelFieldType.String),
										new ModelField("MinPurchase", ModelFieldType.String),
                                        new ModelField("Price", ModelFieldType.String)
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
                        new Column { Text = "物料编号", DataIndex = "MaterialID", Flex=1, ID="column1" },
                        new Column { Text = "物料名称", DataIndex = "MaterialName", Flex=1 , ID="column2"},
                        new ComponentColumn
                        {
                            Text="数量",DataIndex="Num",Editor=true,Flex=1, ID="column3",
                            Component=
                            {
                                new NumberField{AllowBlank=false,EmptyText="数量不能为空",Listeners = { FocusLeave = { Fn = "setValidValue"}}}
                            }
                        },
                        new Column { Text = "型号", DataIndex = "Size", Flex=1, ID="column4" },
                        new Column { Text = "最小采购量", DataIndex = "MinPurchase", Flex=1, ID="column7" },
                        new Column { Text = "单位", DataIndex = "Unit", Flex=1, ID="column5" },
                        new Column { Text = "预算价", DataIndex = "Price", Flex=1, ID="column6"},
                    }
            }
        };
        #endregion

        //#region Material's detail of different OrderType

        //public GridPanel GridofMDetail = new GridPanel()
        //{
        //    Height = 220,
        //    AutoScroll = true,
        //    //Title = "已选材料",
        //    //Collapsible=true,
        //    Store =
        //        {
        //            new Store
        //            {
        //                ID="MaterialSelected",
        //                Model=
        //                {
        //                    new Model{
        //                        IDProperty="MaterialID",
        //                        Fields=
        //                        {
        //                                new ModelField("MaterialID", ModelFieldType.String),
        //                                new ModelField("MaterialName", ModelFieldType.String),
        //                                new ModelField("Num", ModelFieldType.String),
        //                                new ModelField("Size", ModelFieldType.String),
        //                                new ModelField("Unit", ModelFieldType.String),
        //                                new ModelField("Price", ModelFieldType.String)
        //                        }
        //                    }
        //                },
        //                PageSize=10    
        //            }
        //        },

        //    ColumnModel =
        //    {
        //        Columns =
        //            {
        //                new Column { Text = "物料编号", DataIndex = "MaterialID", Flex=1, ID="column1" },
        //                new Column { Text = "物料名称", DataIndex = "MaterialName", Flex=1 , ID="column2"},
        //                new Column { Text="数量",DataIndex="Num",Flex=1, ID="column3"},
        //                new Column { Text = "型号", DataIndex = "Size", Flex=1, ID="column4" },
        //                new Column { Text = "单位", DataIndex = "Unit", Flex=1, ID="column5" },
        //                new Column { Text = "价格", DataIndex = "Price", Flex=1, ID="column6"},
        //            }
        //    }
        //};

        //#endregion   

        //public static IEnumerable GetDatasource
        //{
        //    get
        //    {
        //        return null;
        //    }

        //}
    }




}