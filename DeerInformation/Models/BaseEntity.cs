using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace DeerInformation.BaseType
{
    public enum PermissionType
    {
        Read = 1,//查看
        Create,//创建
        Update,//编辑
        Delete,//删除
        ShowPart//查看部分
    }

    public enum CheckState
    {
        IsNull=0,
        Checking = 1,//1	审核中
        Approved = 2,//2	审核通过
        Rejected = 3,//3	审核驳回
        Refused = 4, //4	审核不通过
        Expried = 5  //5	审核过期

    }

    public enum PunchedException
    {
        Normal=1,     //正常
        Exception=2,  //异常
        Late=3,       //迟到
        Absent=4,     //旷工
        LeftEarly=5   //早退
    }

    public enum WarehouseType
    {

        Requisitions=1, //领用出库
        AllotmentEX =2,// 调拨出库
        Purchase =3, //采购入库
        AllotmentIM =4,// 调拨入库
        Outsourse =5, //外包入库
        StocktakingIM =6, //盘点入库
        StocktakingEX =7 //盘点出库

    }

    internal static class GlobalVariate
    {
       public static string TotalWarehouseId = "ATLND001";
    }


    public class MainMenuItem
    {
        //菜单id
        public string id { get; set; }
        //菜单名称
        public string name { get; set; }
        //菜单链接
        public string url { get; set; }
        //菜单顺序
        public int index { get; set; }
        //菜单上级
        public string pid { get; set; }

        public bool DefaultURL  { get; set; }
        private List<PermissionType> m_permission;
        //权限Read,Update,Create,Delete,Showpart
        public List<PermissionType> permission
        {
            get
            {
                if (m_permission == null)
                {
                    m_permission = new List<PermissionType>();
                }
                return m_permission;
            }
            set
            {
                if (m_permission != null)
                {
                    m_permission.Clear();
                    m_permission = value;
                }
                else
                {
                    m_permission = value;
                }
            }
        }
    }
}