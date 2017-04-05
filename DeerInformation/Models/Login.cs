using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DeerInformation.Extensions;
using Ext.Net;
using Ext.Net.Utilities;

namespace DeerInformation.Models
{
    //用户登录模型
    public class Login
    {

        [Required]
        [Display(Name = "用户名")]
        [StringLength(10)]
        public string Account { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "验证码")]
        public string ValidateCode { get; set; }

        [Display(Name = "记住我")]
        public bool Flag { get; set; }

        //写入登录的cookies信息 设置用户风格
        public bool LoginAuthority(string account, string password,string validateCode, bool flag, HttpResponseBase response, HttpSessionStateBase session)
        {
            //验证码
            if (session["ValidateCode"].IsNull() || session["ValidateCode"].ToString().ToLower() != validateCode.ToLower())
            {
                response.Write("<script>alert('验证码错误！')</script>");
                return false;
            }
            
            using (Entities db = new Entities())
            {
                var search = db.V_PE_UserRole.Where(l => l.UserName == account && l.Activity == true).ToList();
                if (search.Any())
                {
                    var employeeId = search.First().EmployeeID;
                    if (db.T_HR_Staff.FirstOrDefault(l => l.StaffID == employeeId) == null)
                    {
                        response.Write("<script>alert('存在此用户但是当前没有注册该员工基本信息,因此禁止该用户的登录！')</script>");
                        return false;
                    }
                    Authentication authenticationobj = new Authentication();
                    V_PE_UserRole applyment = search.First();
                    
                    string encodepassword = EncryptionCommon.Sha256(password);
                    var admitted = authenticationobj.IsUser(applyment.UserID, account, encodepassword);
                    if (admitted)
                    {
                        T_PE_Users user = db.T_PE_Users.Find(applyment.UserID);
                        HttpCookie cookie = new HttpCookie("User");
                        cookie.Values.Add("UserName", HttpUtility.UrlEncode(account) );
                        cookie.Values.Add("Password", HttpUtility.UrlEncode(encodepassword));
                        cookie.Values.Add("UserID", HttpUtility.UrlEncode(applyment.UserID));
                        if (flag)
                        {
                            cookie.Expires = DateTime.Now.AddMinutes(30);
                        }
                        response.Cookies.Add(cookie);
                        Theme userstyle;
                        if (Enum.TryParse(user.Style,true,out userstyle ))
                        {
                            //session["Ext.Net.Theme"] = userstyle;
                            session["Ext.Net.Theme"] = Theme.Default; 
                        }
                        else
                        {
                            session["Ext.Net.Theme"] = Theme.Default;                            
                        }
                        
                        return true;
                    }

                }
            }

            return false ;
        }

        //登出 清除登录信息
        public bool LoginOut(HttpResponseBase response, HttpSessionStateBase session)
        {

            HttpCookie userCookies = response.Cookies["User"];
            if (userCookies!=null)
            {
                userCookies.Expires=DateTime.Now.AddDays(-1);
                response.Cookies.Add(userCookies);
            }
            session.Clear();
            return true;
        }
    }


}