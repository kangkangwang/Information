using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;


namespace DeerInformation.Areas.gyproject.ShareModule
{
    public class WinModule:Window
    {
        public WinModule():base()
        {
            ID = "win";
            AutoShow = true;
            Icon = Icon.BookEdit;
            Constrain = true;
            Closable = true;
            Maximizable = true;
            Width = 600;
            Height = 600;
            Layout = "fit";
            AutoRender = true;
            CloseAction = Ext.Net.CloseAction.Destroy;
            ButtonAlign = Alignment.Center;
            Loader = new ComponentLoader
            {
                Mode = LoadMode.Frame,
                DisableCaching = true,
            };
        }
    }
}