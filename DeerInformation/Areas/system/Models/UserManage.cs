using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;

namespace DeerInformation.Areas.system.Models
{
    public class UserManage
    {
        [Field(FieldLabel = "用户名或工号")]
        public string SearchWord { get; set; }

        [Field(FieldLabel = "角色",AllowBlank = false)]
        public string RoleID { get; set; }

        [Field(FieldLabel = "工号", AllowBlank = false)]
        public string EmployeeID { get; set; }

        [Field(FieldLabel = "登录名", AllowBlank = false)]
        public string UserName { get; set; }

        [Field(FieldLabel = "密码", AllowBlank = false)]
        public string Password { get; set; }

        [Field(FieldLabel = "确认密码", AllowBlank = false)]
        public string PasswordConfirm { get; set; }

        [Field(FieldLabel = "激活")]
        public Nullable<bool> Activity { get; set; }

        public IEnumerable<ListItem> RoleNameItems
        {
            get
            {
                List<ListItem> result = new List<ListItem>();
                using (Entities db = new Entities())
                {
                    var roles = db.T_PE_Roles.ToList();
                    foreach (var item in roles)
                    {
                        result.Add(new ListItem(item.Description, item.RoleID));
                    }
                }
                return result;
            }
        }

        public IEnumerable<V_PE_UserRole> Users
        {
            get
            {
                using (Entities db = new Entities())
                {
                    string fitformat = string.Format("%{0}%", SearchWord);
                    if (RoleID == null)
                    {
                        return db.V_PE_UserRole.Where(l =>
                        (SqlFunctions.PatIndex(fitformat, l.UserID) > 0 ||
                        SqlFunctions.PatIndex(fitformat, l.UserName) > 0)).ToList();
                    }
                    return db.V_PE_UserRole.Where(l => l.RoleID == RoleID &&
                        (SqlFunctions.PatIndex(fitformat, l.UserID) > 0 ||
                        SqlFunctions.PatIndex(fitformat, l.UserName) > 0)).ToList();
                }
            }
        }

        public bool CreateUser(T_PE_Users newuser, T_PE_UserRoles newuserRoles)
        {
            using (Entities db = new Entities())
            {
                try
                {
                    if (!db.T_PE_Users.Where(l => l.UserName == newuser.UserName || l.EmployeeID == newuser.EmployeeID).ToList().Any())
                    {    
                        LoginUser user=new LoginUser();
                        newuserRoles.Creator = user.EmployeeName;
                        newuserRoles.CreatorID = user.EmployeeId;
                        newuserRoles.CreateTime = DateTime.Now;
                        db.T_PE_Users.Add(newuser);
                        db.T_PE_UserRoles.Add(newuserRoles);
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

            }

        }

	    public bool DetailUser(string id)
	    {
		    using (Entities db=new Entities())
		    {
			    var item = db.T_PE_Users.Find(id);
			    EmployeeID = item.EmployeeID;
				UserName = item.UserName;
				Password = item.Password;
				Activity = item.Activity;
				return true;
			}
	    }

        public bool ForbidUsers(List<V_PE_UserRole> users)
        {
            using (Entities db = new Entities())
            {
                foreach (var item in users)
                {
                    var user = db.T_PE_Users.Find(item.UserID);
                    if (user != null)
                    {
                        user.Activity = false;
                    }
                    else
                    {
                        return false;
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {

                    return false;
                }

                return true;

            }
        }

        public bool EnableUsers(List<V_PE_UserRole> users)
        {
            using (Entities db = new Entities())
            {
                foreach (var item in users)
                {
                    var user = db.T_PE_Users.Find(item.UserID);
                    if (user != null)
                    {
                        user.Activity = true ;
                    }
                    else
                    {
                        return false;
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {

                    return false;
                }

                return true;

            }
        }
    }


}