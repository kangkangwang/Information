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
using System.Data;
using DeerInformation.BaseType;
using System.IO;
using DeerInformation.Extensions;


namespace DeerInformation.Areas.person.Controllers
{
    [DirectController(AreaName = "person")]
    public class PositionCategoryController : Controller
    {
        //by:xgw
        // GET: /person/PositionCategory/

        Entities entities = new Entities();
        [VisitAuthorize(Read = true)]
        public ActionResult Index()
        {
            try
            {
                var list = entities.T_HR_PositionCategory.ToList();

                return View(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }

        }

        public ActionResult Getalldata(string id, string name)//查询按钮响应
        {
            try
            {
                var list = SearchData(id, name);

                return this.Store(list);
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }
        [VisitAuthorize(Create = true)]
        public ActionResult Add()//添加响应
        {
            Window win = new Window
            {

                ID = "windowPC",
                Title = "添加",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddPositionCategory", "PositionCategory", new { pcid = "-1" }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.PCReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();
        }

        public ActionResult AddPC(T_HR_PositionCategory pc)//AddPositionCategory保存相应
        {
            DirectResult r = new DirectResult();
            T_HR_PositionCategory pcupdate = entities.T_HR_PositionCategory.Find(pc.PositionCategoryID);

            if (pcupdate == null)//为空为添加
            {
                pc.CreaterName = "admin";//后期改为用户名
                pc.CreateTime = DateTime.Now;
                entities.T_HR_PositionCategory.Add(pc);
                try
                {
                    entities.SaveChanges();
                    r.Success = true;
                    X.Msg.Alert("提示", "保存成功！", new JFunction { Fn = "closewindow" }).Show();
                }
                catch (Exception e)
                {
                    X.Msg.Alert("警告", "数据保存失败！<br /> note:" + e.Message, new JFunction { Fn = "closewindow" }).Show();
                    r.Success = false;
                }
            }
            else//否则为修改
            {
                pcupdate.PositionCategoryName = pc.PositionCategoryName;
                pcupdate.Remark = pc.Remark;
                pcupdate.EditorName = "admin";//后期改为用户名
                pcupdate.EditorTime = DateTime.Now;
                try
                {
                    entities.SaveChanges();
                    r.Success = true;
                    X.Msg.Alert("提示", "修改成功！", new JFunction { Fn = "closewindow" }).Show();
                }
                catch (Exception e)
                {
                    X.Msg.Alert("警告", "数据修改失败！<br /> note:" + e.Message, new JFunction { Fn = "closewindow" }).Show();
                    r.Success = false;
                }
            }
            return r;
        }

        public ActionResult AddPositionCategory(string pcid)//在修改时传递的为PositionCategoryID
        {
            if (pcid == "-1")//-1为添加，自动生成PositionCategoryID
            {
                string id, idnum;
                int num, n;
                T_HR_PositionCategory list = (from o in entities.T_HR_PositionCategory
                                              orderby o.PositionCategoryID descending
                                              select o).First();
                id = list.PositionCategoryID.ToString();
                num = int.Parse(id.Substring(2, 4)) + 1;
                idnum = num.ToString();
                n = idnum.Length;
                for (int i = 0; i < 4 - n; i++)
                {
                    idnum = "0" + idnum;
                }
                id = "PC" + idnum;
                ViewData["AutoID"] = id;
                return View();
            }
            else//否则为修改
            {
                T_HR_PositionCategory item = (from o in entities.T_HR_PositionCategory
                                              where o.PositionCategoryID == pcid
                                              select o).First();
                ViewData["AutoID"] = pcid;

                return View(item);
            }
        }

        [VisitAuthorize(Update = true)]
        public ActionResult Update(string id)//修改相应，id为PositionCategoryID
        {
            Window win = new Window
            {

                ID = "windowPC",
                Title = "修改",
                Height = 500,
                Width = 800,
                BodyPadding = 5,
                Modal = true,
                CloseAction = CloseAction.Destroy,
                Loader = new ComponentLoader
                {
                    Url = Url.Action("AddPositionCategory", "PositionCategory", new { pcid = id }),
                    DisableCaching = true,
                    Mode = LoadMode.Frame
                },
                Listeners =
                {
                    Close =
                    {
                        Handler = "App.direct.person.PCReload()",
                    }
                }
            };

            win.Render(RenderMode.Auto);
            return this.Direct();


        }

        #region directevent删除，弃用
        //public ActionResult Delete(string selectedData)
        //{
        //    string pcid;
        //    Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(selectedData);

        //    if (values.Length > 0)
        //    {
        //        foreach (Dictionary<string, string> row in values)
        //        {
        //            pcid = row["PositionCategoryID"];
        //            T_HR_PositionCategory de = entities.T_HR_PositionCategory.Find(pcid);
        //            if (de != null)
        //            {
        //                entities.T_HR_PositionCategory.Remove(de);
        //                try
        //                {
        //                    entities.SaveChanges();
        //                }
        //                catch (Exception e)
        //                {
        //                    X.Msg.Alert("警告", "数据删除失败！<br /> note:" + e.Message).Show();
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        X.Msg.Alert("提示", "未选择任何列！").Show();
        //    }

        //    return this.Direct();
        //}
        #endregion

        private List<T_HR_PositionCategory> SearchData(string id, string name)//查询时根据PositionCategoryID和PositionCategoryName进行模糊查询
        {
            var list = new List<T_HR_PositionCategory>();

            list = entities.T_HR_PositionCategory.ToList();

            if (!String.IsNullOrEmpty(id))
            {
                list = (from o in list
                        where o.PositionCategoryID.Contains(id)
                        select o).ToList();
            }

            if (!String.IsNullOrEmpty(name))
            {
                list = (from o in list
                        where o.PositionCategoryName.Contains(name)
                        select o).ToList();
            }

            return list;
        }

        [DirectMethod]
        public ActionResult PCReload()//刷新gridpanel的store
        {
            try
            {
                var list = entities.T_HR_PositionCategory.ToList();

                var store = X.GetCmp<Store>("PCStore");
                store.LoadData(list);
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("警告", "数据库查询失败！<br /> note:" + e.Message).Show();
                return this.Direct();
            }
        }
        [VisitAuthorize(Delete = true)]
        [DirectMethod]
        public ActionResult JSDelete(string selectedData)//删除响应
        {
            string pcid;
            Dictionary<string, string>[] values = JSON.Deserialize<Dictionary<string, string>[]>(selectedData);

            if (values.Length > 0)//js代码已经处理过，此处判断无用，可删
            {
                foreach (Dictionary<string, string> row in values)
                {
                    pcid = row["PositionCategoryID"];
                    T_HR_PositionCategory de = entities.T_HR_PositionCategory.Find(pcid);
                    if (de != null)
                    {
                        entities.T_HR_PositionCategory.Remove(de);
                        try
                        {
                            entities.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            X.Msg.Alert("警告", "数据删除失败！<br /> note:" + e.Message).Show();
                        }
                    }
                }
            }
            else
            {
                X.Msg.Alert("提示", "未选择任何列！").Show();
            }

            return this.Direct();
        }



    }
}
