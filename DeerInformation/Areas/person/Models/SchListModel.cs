using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DeerInformation.Models;
using Ext.Net;
using Ext.Net.MVC;
using DeerInformation.BaseType;

namespace DeerInformation.Areas.person.Models
{
    public class SchListModel
    {
        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string AMTime
        {
            get;
            set;
        }

        public string AMValidGoTime
        {
            get;
            set;
        }

        public string AMValidOffTime
        {
            get;
            set;
        }

        public string PMTime
        {
            get;
            set;
        }

        public string PMValidGoTime
        {
            get;
            set;
        }

        public string PMValidOffTime
        {
            get;
            set;
        }

        public bool Two
        {
            get;
            set;
        }

        public bool Four
        {
            get;
            set;
        }

        public string AttNum
        {
            get;
            set;
        }

        public string CreaterName
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public string EditorName
        {
            get;
            set;
        }

        public DateTime EditeTime
        {
            get;
            set;
        }

        public SchListModel()
        {
        }

        public T_HR_SchList ToDB(int flag)
        {
            T_HR_SchList bt = new T_HR_SchList();
            bt.ID = ID;
            bt.Name = Name;
            bt.AMClassGoOn = AMTime.Substring(0, 5) + ":00";
            bt.AMClassOff = AMTime.Substring(6, 5) + ":00";
            bt.ACGST = AMValidGoTime.Substring(0, 5) + ":00";
            bt.ACGET = AMValidGoTime.Substring(6, 5) + ":00";
            bt.ACOST = AMValidOffTime.Substring(0, 5) + ":00";
            bt.ACOET = AMValidOffTime.Substring(6, 5) + ":00";
            bt.PMClassGoOn = PMTime.Substring(0, 5) + ":00";
            bt.PMClassOff = PMTime.Substring(6, 5) + ":00";
            bt.PCGST = PMValidGoTime.Substring(0, 5) + ":00";
            bt.PCGET = PMValidGoTime.Substring(6, 5) + ":00";
            bt.PCOST = PMValidOffTime.Substring(0, 5) + ":00";
            bt.PCOET = PMValidOffTime.Substring(6, 5) + ":00";
            if(AttNum=="2次卡")
            {
                Two = true;
                Four = false;
            }
            else
            {
                Two = false;
                Four = true;
            }
            bt.Two = Two;
            bt.Four = Four;
            bt.CreaterName = CreaterName;
            bt.CreateTime = CreateTime;
            if(flag!=1)
            {
                bt.EditorName = EditorName;
                bt.EditeTime = EditeTime;
            }

            return bt;
        }



    }
}