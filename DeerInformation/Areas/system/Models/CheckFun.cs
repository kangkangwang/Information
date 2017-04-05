using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeerInformation.Models;

namespace DeerInformation.Areas.system.Models
{
    public class CheckFun
    {
        private readonly Entities _entities = new Entities();

        internal List<dynamic> UsersFilter(string keyWord, string funId)
        {
            var users = UsersBelongFun(funId).Where(l => l.EmployeeID.Contains(keyWord) || l.UserName.Contains(keyWord)).ToList<dynamic>();
            return users;
        }

        internal List<dynamic> UsersBelongFun(string funId)
        {
            var users = _entities.T_CH_Checkfunc.Find(funId);
            var usersList = new string[0];
            if (users != null)
            {
                string ccpeopleString = users.CcPeople ?? string.Empty;
                usersList = ccpeopleString.Split(',');
            }
            var resultList = _entities.T_PE_Users.Select(
                l =>
                    new
                    {
                        UserID = l.UserID,
                        UserName = l.UserName,
                        Activity = l.Activity,
                        EmployeeID = l.EmployeeID,
                        Grant = usersList.Contains(l.UserID)
                    }).ToList<dynamic>();
            return resultList;
        }

        public bool AddCcpeopleToRole(List<dynamic> jsonToList, string funId)
        {
            var handle = _entities.T_CH_Checkfunc.Find(funId);
            if (handle == null) return false;
            string ccpeopleString = handle.CcPeople ?? string.Empty;
            List<string> cPeoLst = ccpeopleString.Split(',').ToList();
            foreach (var cPeo in jsonToList)
            {
                if ((bool)cPeo.Grant)
                {
                    cPeoLst.Add(cPeo.UserID.ToString());
                }
                else
                {
                    cPeoLst.Remove(cPeo.UserID.ToString());
                }
            }
            handle.CcPeople = string.Join(",", cPeoLst);
            try
            {
                _entities.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}