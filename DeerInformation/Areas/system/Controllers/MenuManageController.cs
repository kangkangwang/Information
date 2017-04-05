using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Extensions;

namespace DeerInformation.Areas.system.Models
{
    /// <summary>
    /// editor：WKK
    /// date：2016-6-10
    /// </summary>
    [DirectController(AreaName = "system")]
    [UserAuthorize]
    [VisitAuthorize(Read = true,Update = true,Create = true,Delete = true)]
    public class MenuManageController : Controller
    {
        private Entities db = new Entities();

        private Window win = new Window()
        {
            ID = "detail",
            Title = "detail",
            AutoShow = true,
            Icon = Icon.BookEdit,
            Constrain = true,
            Closable = true,
            Maximizable = true,
            Width = 500,
            Height = 500,
            Loader = new ComponentLoader
            {
                Mode = LoadMode.Frame
            }
            ,
            Listeners =
            {
                Close =
                {
                    //Handler = "App.direct.system.Reload()"
                    //Handler = "refreshTree(Ext.getCmp('treegrid'));"
                    Handler = "refreshTree(App.treegrid);"
                }
            }

        };
        //
        // GET: /system/MenuManage/
        [VisitAuthorize(Create = false, Update = false, Read = true, Delete = false)]
        public ActionResult Index()
        {
            return View(LoadNodes());
        }

        //
        // GET: /system/MenuManage/Details/5
        [DirectMethod]
        public ActionResult DetailButton(string id)
        {
            win.Loader.Url = Url.Action("Details", "MenuManage", new { id = id });
            win.Render();
            TempData["winname"] = win.ID;
            return this.Direct();
        }
        [DirectMethod]
        [VisitAuthorize(Create = true, Update = false, Read = false, Delete = false)]
        public ActionResult AddButton(string id)
        {
            win.Loader.Url = Url.Action("Create", "MenuManage", new { id = id });
            win.Render();
            TempData["winname"] = win.ID;
            return this.Direct();
        }
        [DirectMethod]
        public ActionResult DeteleButton(string id)
        {
            X.Msg.Confirm("消息", "确认删除该项?", new MessageBoxButtonsConfig
            {
                Yes = new MessageBoxButtonConfig
                {
                    Handler = string.Format("App.direct.system.Remove('{0}');", id),
                    Text = "确认"
                },
                No = new MessageBoxButtonConfig
                {
                    Handler = "",
                    Text = "取消"
                }
            }).Show();


            return this.Direct();
        }

        public ActionResult Details(string id)
        {
            ViewBag.winname = TempData["winname"];
            var modle = db.T_PE_Menu.Find(id);
            if (modle == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(modle);
            }
        }

        // GET: /system/MenuManage/Create
        [DirectMethod]
        public ActionResult Create(string id)
        {
            ViewBag.winname = TempData["winname"];
            T_PE_Menu newitem = new T_PE_Menu();
            newitem.ParentID = id;
            newitem.MenuID = Guid.NewGuid().ToString();
            return View(newitem);
        }

        public ActionResult Cancel(T_PE_Menu menu)
        {

            return this.Direct();
        }

        //Detail 编辑保存按钮
        [VisitAuthorize(Create = false, Update = true, Read = false, Delete = false)]
        public ActionResult Edit(T_PE_Menu menu)
        {
            db.T_PE_Menu.Attach(menu);
            db.Entry(menu).State = EntityState.Modified;
            db.SaveChanges();
            return this.Direct(true);
        }
        //Create 增加保存按钮
        [VisitAuthorize(Create = true , Update = false, Read = false, Delete = false)]
        public ActionResult Add(T_PE_Menu menu)
        {
            db.T_PE_Menu.Add(menu);
            db.SaveChanges();
            return this.Direct(true);
        }
        //删除确认响应
        [DirectMethod]
        [VisitAuthorize(Create = false, Update = false, Read = false, Delete = true)]
        public ActionResult Remove(string id)
        {
            var content = db.T_PE_Menu.Find(id);
            if (content != null)
            {
                db.T_PE_Menu.Remove(content);
                db.SaveChanges();
            }
            return this.Reload();
        }
        
        //返回 树root节点
        [DirectMethod]
        public ActionResult Reload()
        {
            return this.RedirectToAction("index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [DirectMethod]
        public Node LoadNodes()
        {

            Node root = new Node();

            root.CustomAttributes.Add(new ConfigItem("ID", -1));

            root.Expanded = true;

            var menu = from li in db.T_PE_Menu
                       select new funcmenu { ID = li.MenuID, ParentID = li.ParentID, TextEN = li.TextEN, TextCN = li.TextCN, URL = li.URL, ImageURL = li.ImageURL, Visible = li.Visible, Creator = li.Creator, CreateTime = li.CreateTime };

            root.Children.Clear();

            FillMenu(menu, "-1", root);

            return root;
        }

        public void FillMenu(IEnumerable<funcmenu> menu, string pid, Node nodec)
        {

            var li = menu.Where(la =>
            {
                return la.ParentID == pid;
            });

            if (li.Count() == 0)
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
                nodec.Children.Add(i);
                FillMenu(menu, item.ID, nodec.Children[index]);
                index++;
            }

            return;
        }

        [DirectMethod(IDMode = DirectMethodProxyIDMode.None)]
        public ActionResult RefreshMenu()
        {
            return this.Direct(LoadNodes());
        }

    }
}