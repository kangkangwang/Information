using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace DeerInformation.Areas.system.Models
{

    public class RolesViewModle
    {
        public Entities db = new Entities();

        public RolesViewModle()
        {
            pageindex = 1;
            pagenum = 34;
        }

        #region view的显示逻辑
        //返回用于显示的数据集
        public IEnumerable<T_PE_Roles> data
        {
            get
            {
                var result = (from p in db.T_PE_Roles.AsNoTracking() orderby p.CreateTime select p);
                return result;

            }
        }

        public SelectedRowCollection InitiallySelectedRows
        {
            get
            {
                SelectedRowCollection result = new SelectedRowCollection();
                if (this.data.Any())
                {
                    result.Add(new SelectedRow(this.data.First().RoleID.ToString()));
                }
                return result;
            }
        }

        //返回单页数据条数
        public int pagenum { get; set; }
        //页号
        public int pageindex { get; set; }

        //加载菜单数据
        public Node LoadNodes(string roleid)
        {

            Node root = new Node();

            root.CustomAttributes.Add(new ConfigItem("ID", -1));

            root.Expanded = true;

            var menu = from li in db.P_PE_GetRolePermission(roleid).ToList()
                       select new RolePermission
                       {
                           ID = li.MenuID,
                           ParentID = li.ParentID,
                           TextEN = li.TextEN,
                           TextCN = li.TextCN,
                           Read = li.Read.intdata().IntToBool(),
                           Create = li.Create.intdata().IntToBool(),
                           Update = li.Update.intdata().IntToBool(),
                           Delete = li.Delete.intdata().IntToBool()
                       };

            root.Children.Clear();

            FillMenu(menu, "-1", root);

            return root;
        }

        public List<dynamic > UsersBelongRole(string roleid)
        {
            var belongs = db.V_PE_UserRole.Where(l => l.RoleID == roleid).Distinct().ToList();
            var notbelongs = db.V_PE_UserRole.Where(l => l.RoleID != roleid || l.RoleID == null).Distinct().ToList();

            return belongs.Union(notbelongs).Select(l => new { UserID = l.UserID, UserName = l.UserName, Activity = l.Activity, Grant = l.RoleID == roleid, EmployeeID = l.EmployeeID }).ToList<dynamic>();
        }

        public object UsersFilter(string keyWord, string roleid)
        {
            var contianer = UsersBelongRole(roleid);
            return contianer.Where(l => l.UserID.Contains(keyWord) || l.UserName.Contains(keyWord)).ToList();
        }

        //生成树形结构
        public void FillMenu(IEnumerable<RolePermission> menu, string pid, Node nodec)
        {

            var li = menu.Where(la =>
            {
                return la.ParentID == pid;
            });

            if (!li.Any())
            {
                nodec.Leaf = true;
                return;
            }

            int index = 0;
            foreach (var item in li)
            {
                Node i = new Node();
                i.Text = item.TextCN;
                i.CustomAttributes.Add(new ConfigItem("ID", item.ID));
                i.CustomAttributes.Add(new ConfigItem("TextEN", item.TextEN));
                i.CustomAttributes.Add(new ConfigItem("TextCN", item.TextCN));
                i.CustomAttributes.Add(new ConfigItem("URL", item.URL));
                i.CustomAttributes.Add(new ConfigItem("ImageURL", item.ImageURL));
                i.CustomAttributes.Add(new ConfigItem("Visible", item.Visible.booldata()));
                i.CustomAttributes.Add(new ConfigItem("Creator", item.Creator));
                i.CustomAttributes.Add(new ConfigItem("CreateTime", item.CreateTime.datetimedata()));
                i.CustomAttributes.Add(new ConfigItem("Read", item.Read));
                i.CustomAttributes.Add(new ConfigItem("Create", item.Create));
                i.CustomAttributes.Add(new ConfigItem("Update", item.Update));
                i.CustomAttributes.Add(new ConfigItem("Delete", item.Delete));
                nodec.Children.Add(i);
                nodec.Expanded = true;
                FillMenu(menu, item.ID, nodec.Children[index]);
                index++;
            }

            return;
        }
        #endregion

        #region 角色及权限修改逻辑
        public bool insert(T_PE_Roles obj)
        {
            try
            {
                db.T_PE_Roles.Add(obj);
                db.SaveChanges();
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        public bool update(T_PE_Roles obj)
        {
            try
            {
                db.Entry(obj).State = System.Data.EntityState.Detached;
                db.T_PE_Roles.Attach(obj);
                db.Entry(obj).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        public bool delete(T_PE_Roles[] obj)
        {
            try
            {
                for (int i = 0; i < obj.Count(); i++)
                {
                    var item = db.T_PE_Roles.Find(obj[i].RoleID);
                    db.T_PE_Roles.Attach(item);
                    db.T_PE_Roles.Remove(item);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        public bool AddUsersToRole(List<dynamic> users, string roleid)
        {

            try
            {
                foreach (var user in users)
                {
                    string userid = user.UserID;
                    var current = db.T_PE_UserRoles.Where(l => l.UserID == userid && l.RoleID == roleid).ToList();
                    if ((bool)user.Grant && !current.Any())
                    {
                        db.T_PE_UserRoles.Add(new T_PE_UserRoles()
                        {
                            UserID = userid,
                            RoleID = roleid,
                            CreatorID = new LoginUser().EmployeeId,
                            Creator = new LoginUser().EmployeeName,
                            CreateTime = DateTime.Now
                        });
                    }
                    if (!(bool)user.Grant && current.Any())
                    {
                        db.T_PE_UserRoles.Remove(current.First());
                    }

                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool GrandAuthority(SubmittedNode userNode, string roleid)
        {
            try
            {
                TravelTreeNode(userNode, roleid);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public int TravelTreeNode(SubmittedNode userNode, string roleid)
        {
            if (userNode != null)
            {
                foreach (var node in userNode.Children)
                {
                    Grand(node, roleid);
                    TravelTreeNode(node, roleid);
                }

            }
            return 0;
        }

        private void Grand(SubmittedNode node, string roleid)
        {
            WriteAuthorityToDatabase(roleid, "Read", node.Attributes["ID"].ToString(), (bool)node.Attributes["Read"]);
            WriteAuthorityToDatabase(roleid, "Create", node.Attributes["ID"].ToString(), (bool)node.Attributes["Create"]);
            WriteAuthorityToDatabase(roleid, "Update", node.Attributes["ID"].ToString(), (bool)node.Attributes["Update"]);
            WriteAuthorityToDatabase(roleid, "Delete", node.Attributes["ID"].ToString(), (bool)node.Attributes["Delete"]);

        }

        private void WriteAuthorityToDatabase(string roleid, string action, string menuid, bool value)
        {
            var currentstate = db.V_PE_RolePermission.Where(
                li => li.RoleID == roleid && li.MenuID == menuid && li.Action == action).ToList();
            //删除权限
            if (!value && currentstate.Any())
            {

                var permission = db.T_PE_Permission.Where(l => l.Action == action && l.MenuID == menuid).ToList();
                List<string> permissionList = new List<string>();
                permission.ForEach(l =>
                {
                    permissionList.Add(l.PermissionID);
                });
                var rolePermission =
                    db.T_PE_RolePermissions.Where(
                        l => l.RoleID == roleid && permissionList.Contains(l.PermissionID)).ToList();
                rolePermission.ForEach(l =>
                {
                    db.T_PE_RolePermissions.Remove(l);
                });

            }
            //添加权限
            if (value && !currentstate.Any())
            {
                string permissionid = Guid.NewGuid().ToString();
                var permissionBefore = db.T_PE_Permission.FirstOrDefault(l => l.Action == action && l.MenuID == menuid);
                if (permissionBefore == null)
                {
                    T_PE_Permission permission = new T_PE_Permission()
                    {
                        Action = action,
                        MenuID = menuid,
                        CreateTime = DateTime.Now,
                        PermissionID = permissionid
                    };
                    db.T_PE_Permission.Add(permission);
                }
                else
                {
                    permissionid = permissionBefore.PermissionID;
                }

                T_PE_RolePermissions rolePermission = new T_PE_RolePermissions()
                {
                    RoleID = roleid,
                    PermissionID = permissionid,
                    CreateTime = DateTime.Now
                };

                db.T_PE_RolePermissions.Add(rolePermission);
            }
            db.SaveChanges();
        }
        #endregion


    }
}