using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Models;
using DeerInformation.BaseType;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.Extensions;
using RouteData = System.Web.Routing.RouteData;

namespace DeerInformation.Controllers
{
    [UserAuthorize]
    [DirectController]
    public class HomeController : Controller
    {
        //存储目录
        private readonly MainMenu _mainmenu = new MainMenu();

        // GET: /Home/
        public ActionResult Index()
        {
            ViewBag.position = "当前位置";
            var defaultItem = _mainmenu.usermenu.Where(l => l.pid == "-1").OrderBy(l => l.index).First();
            if (defaultItem != null)
            {
                _mainmenu.currentid = defaultItem.id;
                _mainmenu.currenturl = _mainmenu.defaluturl;
                X.GetCmp<Button>(_mainmenu.currentid).SetPressed();
            }
            return View(_mainmenu);
        }

        [AllowAnonymous]
        public ActionResult Login(string account, string password, string validateCode, bool? flag)
        {
            if (ValidateRequest && account != null && password != null && validateCode != null)
            {
                Login newLogin = new Login();
                if (newLogin.LoginAuthority(account, password, validateCode, flag.booldata(), this.Response, this.Session))
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        public ActionResult LoginOut()
        {
            Login newLogin = new Login();
            if (newLogin.LoginOut(this.Response, this.Session))
            {
                return RedirectToAction("Login");
            }
            return this.Direct();
        }

        public ActionResult Changemenu(string pid, string url)
        {
            ViewBag.position = "当前位置";

            //功能模块一级菜单id
            _mainmenu.currentid = pid;
            //模块默认页
            _mainmenu.currenturl = _mainmenu.defaluturl;

            return this.PartialExtView(
                    viewName: "changemenu",
                    containerId: "contian_1",
                    model: _mainmenu,
                    mode: RenderMode.AddTo,
                    clearContainer: true
                    );

        }

        public ActionResult Load(string url)
        {
            _mainmenu.currenturl = url;
            return this.PartialExtView(
                viewName: "content",
                containerId: "maincontent",
                model: _mainmenu,
                mode: RenderMode.AddTo,
                clearContainer: true
                );
        }

        [DirectMethod(Namespace = "Home")]
        public ActionResult NewWindow(string url, string title, int height = 600, int width = 500)
        {
            Window win = new Window
            {
                ID = "win",
                Title = title,
                Height = height,
                Width = width,
                BodyPadding = 5,
                Modal = false,
                Minimizable = true,
                Maximizable = true,
                Constrain = true,
                CloseAction = CloseAction.Destroy,
                Resizable = true,
                Loader = new ComponentLoader
                {
                    Mode = LoadMode.Frame
                }
            };
            win.Loader.Url = url;
            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult GetMessage()
        {
            try
            {
                var msg = new MessageManage();
                if (msg.GetNewMessageCount > 0)
                {
                    //string messagetext = string.Format("Ext.Msg.notify('{0}', '{1}');",
                    //    string.Format("{0}条新消息", msg.GetNewMessageCount),
                    //    string.Format("{0}个任务需要审核！", msg.GetTaskCount));
                    //X.AddScript(messagetext);
                    string title = string.Format("{0}条新消息", msg.GetNewMessageCount);
                    string content = string.Format("<a target='_blank' href='{1}'>{0}个任务需要审核！</a>", msg.GetTaskCount, Url.Action("TaskCheck", "WorkPlatform", new { area = "user" }));
                    X.MessageBox.Notify(new NotificationConfig { AutoHide = true, HideDelay = 5000, Title = title, Html = content, Closable = true }).Show();

                    msg.GetNewMessage();
                }
                this.GetCmp<Label>("messagenum").Text = msg.GetMessageCount.ToString();
                this.GetCmp<Label>("tasknum").Text = msg.GetTaskCount.ToString();
                this.GetCmp<Label>("ccnum").Text = msg.GetCcCount.ToString();
            }
            catch (Exception)
            {

                return this.Direct(false);
            }

            return this.Direct(true);
        }

        public ActionResult DeatilTask()
        {
            return Load(Url.Action("TaskCheck", "WorkPlatform", new { area = "user" }));
        }

        public ActionResult DeatilMessage()
        {
            return Load(Url.Action("Message", "WorkPlatform", new { area = "user" }));
        }

        public ActionResult DetailCc()
        {
            return Load(Url.Action("CcMessage", "WorkPlatform", new { area = "user" }));
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View("Error");
        }

        [AllowAnonymous]
        public ActionResult Unauthorized()
        {
            return View("Unauthorized");
        }

        [AllowAnonymous]
        public ActionResult Expire()
        {
            return View("Expire");
        }

        [AllowAnonymous]
        public ActionResult GetValidateCodeImg()
        {
            const int width = 100;
            const int height = 30;
            const int fontsize = 20;
            string code = string.Empty;
            byte[] bytes = ValidateCode.CreateValidateGraphic(out code, 4, width, height, fontsize);
            Session.Add("ValidateCode", code);//设置验证码Session
            return File(bytes, @"image/jpeg");
        }
    }
}
