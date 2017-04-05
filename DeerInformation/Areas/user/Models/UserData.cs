using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using System.Reflection;


namespace DeerInformation.Areas.user.Models
{
    /// <summary>
    /// editor：WKK
    /// date：2016-9-14
    /// </summary>
    public class UserData
    {
        [Field(ReadOnly = true, FieldLabel = "账号名称")]
        public string UserID { get; set; }

        [Field(ReadOnly = true, FieldLabel = "用户名")]
        public string UserName { get; set; }

        [Field(AllowBlank = true, FieldLabel = "昵称")]
        public string Name { get; set; }

        [Field(AllowBlank = true, FieldLabel = "密码")]
        public string Password { get; set; }

        [Field(AllowBlank = true, FieldLabel = "原始密码")]
        public string PasswordOrigin { get; set; }

        [Field(AllowBlank = true, FieldLabel = "确认密码")]
        public string PasswordComfirm { get; set; }

        [Field(AllowBlank = true, FieldLabel = "部门名称")]
        public string UserDspName { get; set; }

        [Field(AllowBlank = true, FieldLabel = "性别")]
        public string Sex { get; set; }

        [Field(AllowBlank = true, FieldLabel = "生日")]
        public Nullable<System.DateTime> Birthday { get; set; }

        [Field(AllowBlank = true, FieldLabel = "移动电话")]
        public string MobilePhone { get; set; }

        [Field(AllowBlank = true, FieldLabel = "座机电话")]
        public string OfficePhone { get; set; }

        [Field(AllowBlank = true, FieldLabel = "家庭电话")]
        public string HomePhone { get; set; }

        [Field(AllowBlank = true, FieldLabel = "E-mail", Vtype = "email")]
        public string Email { get; set; }

        [Field(AllowBlank = true, FieldLabel = "启用Email信息")]
        public bool SendEmailMsg { get; set; }

        [Field(AllowBlank = true, FieldLabel = "QQ")]
        public string QQ { get; set; }

        [Field(AllowBlank = true, FieldLabel = "地址")]
        public string Address { get; set; }

        [Field(AllowBlank = true, FieldLabel = "员工工号",ReadOnly = true)]
        public string EmployeeID { get; set; }

        [Field(FieldLabel = "启用状态", ReadOnly = true)]
        public Nullable<bool> Activity { get; set; }

        [Field(AllowBlank = true, FieldLabel = "员工类型")]
        public string UserType { get; set; }

        [Field(AllowBlank = true, FieldLabel = "部门ID")]
        public string DeptID { get; set; }

        [Field(AllowBlank = true, FieldLabel = "部门名称")]
        public string DeptName { get; set; }

        [Field(AllowBlank = true, FieldLabel = "公司ID")]
        public string CompanyID { get; set; }

        [Field(AllowBlank = true, FieldLabel = "公司名称")]
        public string CompanyName { get; set; }

        [Field(AllowBlank = true, FieldLabel = "主题风格")]
        public string Style { get; set; }

        [Field(AllowBlank = true, FieldLabel = "创建者")]
        public string Creator { get; set; }

        [Field(AllowBlank = true, FieldLabel = "创建者ID")]
        public string CreatorID { get; set; }

        [Field(AllowBlank = true, FieldLabel = "创建时间")]
        public Nullable<System.DateTime> CreateTime { get; set; }

        [Field(AllowBlank = true, FieldLabel = "编辑者")]
        public string Editor { get; set; }

        [Field(AllowBlank = true, FieldLabel = "编辑者ID")]
        public string EditorID { get; set; }

        [Field(AllowBlank = true, FieldLabel = "编辑时间")]
        public Nullable<System.DateTime> EditeTime { get; set; }

        [Field(AllowBlank = true, FieldLabel = "上次登录IP")]
        public string LastLoginIP { get; set; }

        [Field(AllowBlank = true, FieldLabel = "上次登录时间")]
        public Nullable<System.DateTime> LastLoginTime { get; set; }

        [Field(AllowBlank = true, FieldLabel = "上次登录Mac地址")]
        public string LastMacAddress { get; set; }

        [Field(AllowBlank = true, FieldLabel = "当前登录IP")]
        public string CurrentLoginIP { get; set; }

        [Field(AllowBlank = true, FieldLabel = "当前登录时间")]
        public Nullable<System.DateTime> CurrentLoginTime { get; set; }

        [Field(AllowBlank = true, FieldLabel = "当前登录Mac地址")]
        public string CurrentMacAddress { get; set; }

        [Field(AllowBlank = true, FieldLabel = "上次修改密码时间")]
        public Nullable<System.DateTime> LastPasswordTime { get; set; }

        public HttpPostedFile UserImageFile { get; set; }

        public string UserImageFileType { get; set; }

        public string UserImage { get; set; }

        public string UserImageSavePath { get; set; }

        public bool SavaData()
        {
            try
            {
                if (UserImageSavePath != null && Path.GetDirectoryName(UserImageSavePath) != null)
                {
                    if (Directory.Exists(Path.GetDirectoryName(UserImageSavePath)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(UserImageSavePath));
                    }

                    UserImageFile.SaveAs(UserImageSavePath);
                }

                using (Entities db = new Entities())
                {
                    LoginUser user = new LoginUser();
                    var origin = db.T_PE_Users.Find(user.UserId);
                    origin.Name = Name;
                    //origin.Password = Password;
                    origin.UserImage = UserImage ?? origin.UserImage;
                    origin.UserDspName = UserDspName;
                    origin.Sex = Sex;
                    origin.Birthday = Birthday;
                    origin.MobilePhone = MobilePhone;
                    origin.OfficePhone = OfficePhone;
                    origin.HomePhone = HomePhone;
                    origin.SendEmailMsg = SendEmailMsg;
                    origin.Email = Email;
                    origin.QQ = QQ;
                    origin.Address = Address;
                    origin.EmployeeID = EmployeeID;
                    origin.Style = Style;
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }

        public UserData GetCurrentData()
        {
            using (Entities db = new Entities())
            {
                try
                {
                    UserData returnData = new UserData(); ;
                    LoginUser user = new LoginUser();
                    var origin = db.T_PE_Users.Find(user.UserId);
                    returnData.UserID = origin.UserID;
                    returnData.UserName = origin.UserName;
                    returnData.Name = origin.Name;
                    returnData.Password = origin.Password;
                    returnData.UserDspName = origin.UserDspName;
                    returnData.Sex = origin.Sex;
                    returnData.Birthday = origin.Birthday;
                    returnData.MobilePhone = origin.MobilePhone;
                    returnData.OfficePhone = origin.OfficePhone;
                    returnData.HomePhone = origin.HomePhone;
                    returnData.SendEmailMsg = origin.SendEmailMsg??false;
                    returnData.Email = origin.Email;
                    returnData.QQ = origin.QQ;
                    returnData.Address = origin.Address;
                    returnData.EmployeeID = origin.EmployeeID;
                    returnData.Activity = origin.Activity;
                    returnData.Style = origin.Style;
                    return returnData;
                }
                catch (Exception )
                {
                    return null;
                }
            }
        }

        public bool SavePassword()
        {
            using (Entities db = new Entities())
            {
                try
                {
                    LoginUser user = new LoginUser();
                    var origin = db.T_PE_Users.Find(user.UserId);
                    if (origin.Password==PasswordOrigin)
                    {
                        origin.Password = Password;
                        db.SaveChanges();
                    }
                    return true;
                }
                catch (Exception )
                {
                    return false ;
                }
                
            }
        }
    }
}