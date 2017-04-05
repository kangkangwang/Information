using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using Ext.Net.MVC;

namespace DeerInformation.Areas.person.Models
{
    public class NodeWithPreID
    {
        public Node node { get; set; }
        public string preid { get; set; }
    }
}