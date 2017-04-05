using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net;
using DeerInformation.Models;
using System.Text;
using System.Xml.Xsl;
using Newtonsoft.Json;
using System.Xml;
using System.Data.Entity.Infrastructure;
using DeerInformation.Areas.person.Models;

namespace DeerInformation.Areas.person
{
    public class Tool
    {

      private static Entities entities = new Entities();
        #region MyRegion
       /// <summary>
        /// 产生种子
       /// </summary>
       /// <param name="n"></param>
       /// <returns></returns>
       /// 
        public static string ProduceSed(int n)
        {
            return System.Guid.NewGuid().ToString().Substring(0, n);
        }
        public static string ProduceSed64()
        {
            return System.Guid.NewGuid().ToString();
        }
        public static string ProduceSed20()
        {
            return System.Guid.NewGuid().ToString().Substring(0, 20);
        }
        #endregion

        public static string[] DepStr(string dep)
        {
            string level="",deps="";

            var deplevel = from o in entities.V_ComJiaGou
                           where o.ID == dep
                           select new { o.Level };
            foreach (var item in deplevel)
            {
                level = item.Level.ToString();
            }

            if (level == "1")
            {
                var Cfdepid = from o in entities.V_ComJiaGou
                             where o.PID == dep
                             select o.ID;
                var Csdepid = from o in entities.V_ComJiaGou
                             where Cfdepid.Contains(o.PID)
                             select o;
                foreach(var item in Csdepid)
                {
                    deps = deps + item.ID.ToString() + ",";
                }
                if(deps.Length!=0)
                {
                    deps = deps.Substring(0, deps.Length - 1);
                }  
            }

            if (level == "2")
            {
                var Fsdepid = from o in entities.V_ComJiaGou
                              where o.PID==dep
                              select o;
                foreach (var item in Fsdepid)
                {
                    deps = deps + item.ID.ToString() + ",";
                }
                if (deps.Length != 0)
                {
                    deps = deps.Substring(0, deps.Length - 1);
                }  
            }

            if(level=="3")
            {
                deps = dep;
            }

            string[] sdep = deps.Split(',');

            return sdep;
        }

        public static string[] StaffStr(string dep)
        {
            string level="",staffs="";
            var depl = from o in entities.V_HR_Dep
                      where o.ID == dep
                      select o;
            if(depl.Any())
            {
                level = depl.First().Level;
            }

            if(level=="1")
            {
                var list = from o in entities.T_HR_Staff
                           where o.ID1 == dep
                           select o;
                foreach(var item in list)
                {
                    staffs = staffs + item.StaffID + ",";
                }
            }

            if (level == "2")
            {
                var list = from o in entities.T_HR_Staff
                           where o.ID2 == dep
                           select o;
                foreach (var item in list)
                {
                    staffs = staffs + item.StaffID + ",";
                }
            }

            if (level == "3")
            {
                var list = from o in entities.T_HR_Staff
                           where o.ID3 == dep
                           select o;
                foreach (var item in list)
                {
                    staffs = staffs + item.StaffID + ",";
                }
            }

            if (level == "4")
            {
                var list = from o in entities.T_HR_Staff
                           where o.ID4 == dep
                           select o;
                foreach (var item in list)
                {
                    staffs = staffs + item.StaffID + ",";
                }
            }

            if (level == "5")
            {
                var list = from o in entities.T_HR_Staff
                           where o.ID5 == dep
                           select o;
                foreach (var item in list)
                {
                    staffs = staffs + item.StaffID + ",";
                }
            }

            if(staffs.Length!=0)
            {
                staffs = staffs.Substring(0, staffs.Length - 1);
            }

            string[] staff = staffs.Split(',');

            return staff;

        }

        public static Node GetNode()
        {
            List<NodeWithPreID> node1 = new List<NodeWithPreID>();
            List<NodeWithPreID> node2 = new List<NodeWithPreID>();
            List<NodeWithPreID> node3 = new List<NodeWithPreID>();
            List<NodeWithPreID> node4 = new List<NodeWithPreID>();
            List<NodeWithPreID> node5 = new List<NodeWithPreID>();

            Node root = new Node();
            root.NodeID = "0";
            root.Text = "德尔集团";
            root.Expanded = true;
            root.Icon = Icon.World;

            try
            {
                var dep1 = from o in entities.V_HR_Dep
                          where o.Level=="1"
                          orderby o.DOrder
                          select o;
                var dep2 = from o in entities.V_HR_Dep
                           where o.Level == "2"
                           orderby o.DOrder
                           select o;
                var dep3 = from o in entities.V_HR_Dep
                           where o.Level == "3"
                           orderby o.DOrder
                           select o;
                var dep4 = from o in entities.V_HR_Dep
                           where o.Level == "4"
                           orderby o.DOrder
                           select o;
                var dep5 = from o in entities.V_HR_Dep
                           where o.Level == "5"
                           orderby o.DOrder
                           select o;

                foreach(var item in dep5)
                {
                    NodeWithPreID nwp = new NodeWithPreID();
                    Node node = new Node();
                    node.NodeID = item.ID;
                    node.Expanded = false;
                    node.Text = item.Name;
                    node.Icon = Icon.UserB;
                    nwp.node = node;
                    nwp.preid = item.PreID;
                    node5.Add(nwp);
                }

                foreach(var item in dep4)
                {
                    NodeWithPreID nwp = new NodeWithPreID();
                    Node node = new Node();
                    node.NodeID = item.ID;
                    node.Expanded = false;
                    node.Text = item.Name;
                    node.Icon = Icon.UserB;
                    nwp.node = node;
                    nwp.preid = item.PreID;
                    foreach(var n in node5)
                    {
                        if(n.preid==item.ID)
                        {
                            node.Children.Add(n.node);
                            //node5.Remove(n);
                        }
                    }
                    node4.Add(nwp);
                }

                foreach (var item in dep3)
                {
                    NodeWithPreID nwp = new NodeWithPreID();
                    Node node = new Node();
                    node.NodeID = item.ID;
                    node.Expanded = false;
                    node.Text = item.Name;
                    node.Icon = Icon.UserB;
                    nwp.node = node;
                    nwp.preid = item.PreID;
                    foreach (var n in node4)
                    {
                        if (n.preid == item.ID)
                        {
                            node.Children.Add(n.node);
                            //node4.Remove(n);
                        }
                    }
                    node3.Add(nwp);
                }

                foreach (var item in dep2)
                {
                    NodeWithPreID nwp = new NodeWithPreID();
                    Node node = new Node();
                    node.NodeID = item.ID;
                    node.Expanded = false;
                    node.Text = item.Name;
                    node.Icon = Icon.UserB;
                    nwp.node = node;
                    nwp.preid = item.PreID;
                    foreach (var n in node3)
                    {
                        if (n.preid == item.ID)
                        {
                            node.Children.Add(n.node);
                            //node3.Remove(n);
                        }
                    }
                    node2.Add(nwp);
                }

                foreach (var item in dep1)
                {
                    NodeWithPreID nwp = new NodeWithPreID();
                    Node node = new Node();
                    node.NodeID = item.ID;
                    node.Expanded = true;
                    node.Text = item.Name;
                    node.Icon = Icon.UserEarth;
                    nwp.node = node;
                    nwp.preid = item.PreID;
                    foreach (var n in node2)
                    {
                        if (n.preid == item.ID)
                        {
                            node.Children.Add(n.node);
                            //node2.Remove(n);
                        }
                    }
                    node1.Add(nwp);
                }

                foreach(var n in node1)
                {
                    root.Children.Add(n.node);
                    //node1.Remove(n);
                }

            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
            }
            return root;

        }


    }
}