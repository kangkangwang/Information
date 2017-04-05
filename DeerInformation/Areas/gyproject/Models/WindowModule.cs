using System;
using System.Web;
using Ext.Net;

namespace DeerInformation.Areas.gyproject.Models
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

    public sealed class PrintWindow : WindowModule
    {
        public PrintWindow():base()
        {
            Width = 695;
            Height = 842;
            //this.TopBar.Add(new Toolbar()
            //    {
            //        ColumnWidth = 1,
            //        Items =
            //        {
            //            new Button()
            //            {
            //                Text = "Print",
            //                Icon = Icon.Printer,
            //                Handler = "window.print();"
            //            }
            //        }
            //    });
        }
    }
}
