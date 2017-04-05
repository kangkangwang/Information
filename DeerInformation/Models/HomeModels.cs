using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.EnterpriseServices.Internal;
using System.Runtime.InteropServices;
using System.Web.Services.Description;
using DeerInformation.BaseType;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using DeerInformation.Extensions;

namespace DeerInformation.Models
{

    //主菜单
    public class MainMenu
    {
        private List<MainMenuItem> _usermenu;
        //获取用户菜单及权限
        public List<MainMenuItem> usermenu
        {
            get
            {
                if (_usermenu == null)
                {
                    _usermenu = new List<MainMenuItem>();
                    LoginUser user = new LoginUser();
                    using (Entities dmentity = new Entities())
                    {
                        var MmenuData = from m in dmentity.V_PE_Permission_Menu_Roles_Users
                            where m.Visible == true && user.UserId == m.UserID
                            select new {m.MenuID, m.TextCN, m.URL, m.OrderID, m.ParentID, m.Action, m.DefaultURL};

                        foreach (var item in MmenuData)
                        {
                            var Tmenu = _usermenu.Find(m => m.id == item.MenuID);
                            PermissionType Tpermission;
                            if (false == Enum.TryParse(item.Action, true, out Tpermission))
                            {
                                continue;
                            }
                            if (Tmenu == default(MainMenuItem))
                            {
                                MainMenuItem menuData = new MainMenuItem();
                                menuData.id = item.MenuID;
                                menuData.name = item.TextCN;
                                menuData.url = item.URL;
                                menuData.DefaultURL = item.DefaultURL.booldata();
                                menuData.permission.Add(Tpermission);
                                if (item.OrderID != null)
                                {
                                    menuData.index = (int) item.OrderID;
                                }
                                menuData.pid = item.ParentID;
                                _usermenu.Add(menuData);
                            }
                            else
                            {
                                Tmenu.permission.Add(Tpermission);
                            }

                        }
                    }
                }

                return _usermenu;
            }
        }

        //获取可读菜单项
        public List<MainMenuItem> GetReadMainMenuItems
        {
            get { return usermenu.FindAll(m => m.pid == currentid && m.permission.Contains(PermissionType.Read)); }
        }
        public string currentid { get; set; }

        public string currenturl { get; set; }

        public string defaluturl
        {
            get
            {
                if (currentid != null)
                {
                    MainMenuItem secondmenu = GetReadMainMenuItems.Where(l=>l.DefaultURL==true).OrderBy(l => l.index).FirstOrDefault();
                    if (secondmenu != null)
                    {
                        MainMenuItem defaultmenu = usermenu.Where(l => l.pid == secondmenu.id&&l.DefaultURL==true).OrderBy(l => l.index).FirstOrDefault();
                        if (defaultmenu != null)
                        {
                            return defaultmenu.url;
                        }
                    }

                }
                return null;
            }
        }
        //用户昵称
        public string Name
        {
            get
            {
                using (Entities db=new Entities())
                {

                    return db.T_PE_Users.Find(new LoginUser().UserId).Name;
                }
                
            }
        }

        //用户图片
        public string UserImageUrl
        {
            get
            {
                using (Entities db = new Entities())
                {

                    return db.T_PE_Users.Find(new LoginUser().UserId).UserImage;
                }
            }
        }
    }


}