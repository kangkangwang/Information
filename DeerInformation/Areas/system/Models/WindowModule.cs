using System;
using System.Web;
using Ext.Net;

namespace DeerInformation.Areas.system.Models
{
    public class WindowModule : Window
    {
        public WindowModule():base()
        {
            ID = "win";
            Title = "detail";
            AutoShow = true;
            Icon = Icon.BookEdit;
            Constrain = true;
            Closable = true;
            Maximizable = true;
            Width = 700;
            Height = 510;
            Layout = "fit";
            AutoRender = true;
            CloseAction = Ext.Net.CloseAction.Destroy;
            ButtonAlign = Alignment.Center;
            Loader = new ComponentLoader
            {
                Mode = LoadMode.Frame
            };

        }
        
    }
}
