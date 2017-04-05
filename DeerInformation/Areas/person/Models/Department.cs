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
    public class Department
    {
        public string ID
        {
            get;
            set;
        }

        public string ID1
        {
            get;
            set;
        }

        public string ID2
        {
            get;
            set;
        }

        public string ID3
        {
            get;
            set;
        }

        public string ID4
        {
            get;
            set;
        }

        public string PreID
        {
            get;
            set;
        }

        public string No
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        public int DOrder
        {
            get;
            set;
        }

        public bool Valid
        {
            get;
            set;
        }

        public string Remark
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

        public T_HR_Department1 ToDB1(int flag)
        {
            T_HR_Department1 dep = new T_HR_Department1();
            dep.ID1 = ID;
            dep.Department1No = No;
            dep.Department1Name = Name;
            dep.Remark = Remark;
            dep.CreaterName = CreaterName;
            dep.CreateTime = CreateTime;
            if(flag==2)
            {
                dep.EditorName = EditorName;
                dep.EditeTime = EditeTime;
            }
            dep.DOrder = DOrder;
            dep.Valid = Valid;

            return dep;
        }

        public T_HR_Department2 ToDB2(int flag)
        {
            T_HR_Department2 dep = new T_HR_Department2();
            dep.ID2 = ID;
            dep.ID1 = PreID;
            dep.Department2No = No;
            dep.Department2Name = Name;
            dep.Remark = Remark;
            dep.CreaterName = CreaterName;
            dep.CreateTime = CreateTime;
            if (flag == 2)
            {
                dep.EditorName = EditorName;
                dep.EditeTime = EditeTime;
            }
            dep.DOrder = DOrder;
            dep.Valid = Valid;

            return dep;
        }

        public T_HR_Department3 ToDB3(int flag)
        {
            T_HR_Department3 dep = new T_HR_Department3();
            dep.ID3 = ID;
            dep.ID2 = PreID;
            dep.Department3No = No;
            dep.Department3Name = Name;
            dep.Remark = Remark;
            dep.CreaterName = CreaterName;
            dep.CreateTime = CreateTime;
            if (flag == 2)
            {
                dep.EditorName = EditorName;
                dep.EditeTime = EditeTime;
            }
            dep.DOrder = DOrder;
            dep.Valid = Valid;

            return dep;
        }

        public T_HR_Department4 ToDB4(int flag)
        {
            T_HR_Department4 dep = new T_HR_Department4();
            dep.ID4 = ID;
            dep.ID3 = PreID;
            dep.Department4No = No;
            dep.Department4Name = Name;
            dep.Remark = Remark;
            dep.CreaterName = CreaterName;
            dep.CreateTime = CreateTime;
            if (flag == 2)
            {
                dep.EditorName = EditorName;
                dep.EditeTime = EditeTime;
            }
            dep.DOrder = DOrder;
            dep.Valid = Valid;

            return dep;
        }

        public T_HR_Department5 ToDB5(int flag)
        {
            T_HR_Department5 dep = new T_HR_Department5();
            dep.ID5 = ID;
            dep.ID4 = PreID;
            dep.Department5No = No;
            dep.Department5Name = Name;
            dep.Remark = Remark;
            dep.CreaterName = CreaterName;
            dep.CreateTime = CreateTime;
            if (flag == 2)
            {
                dep.EditorName = EditorName;
                dep.EditeTime = EditeTime;
            }
            dep.DOrder = DOrder;
            dep.Valid = Valid;

            return dep;
        }



    }
}