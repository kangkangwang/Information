using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeerInformation.Areas.system.Models
{
    public class funcmenu
    {

        private string m_ID;
        private string m_parentID;
        private string m_TextEN;
        private string m_textCN;
        private string m_URL;
        private string m_ImageURL;
        private Nullable<bool> m_Visible;
        private string m_Creator;
        private Nullable< DateTime> m_CreateTime;

        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public string ParentID
        {
            get { return m_parentID; }
            set { m_parentID = value; }
        }

        public string TextEN
        {
            get { return m_TextEN; }
            set { m_TextEN = value; }
        }

        public string TextCN
        {
            get { return m_textCN; }
            set { m_textCN = value; }
        }

        public string URL
        {
            get { return m_URL; }
            set { m_URL = value; }
        }

        public string ImageURL
        {
            get { return m_ImageURL; }
            set { m_ImageURL = value; }
        }

        public Nullable<bool> Visible
        {
            get { return m_Visible; }
            set { m_Visible = value; }
        }

        public string Creator
        {
            get { return m_Creator; }
            set { m_Creator = value; }
        }

        public Nullable<DateTime> CreateTime
        {
            get { return m_CreateTime; }
            set { m_CreateTime = value; }
        }




    }
}