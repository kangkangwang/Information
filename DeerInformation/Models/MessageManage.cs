using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeerInformation.Models
{
    /// <summary>
    /// 处理新消息和新任务的类
    /// </summary>
    public class MessageManage
    {
        private const string CcType = "抄送消息";

        private readonly LoginUser _user;

        public MessageManage()
        {
            _user = new LoginUser();
        }

        public IEnumerable<T_US_Message> GetNewMessage()
        {
            using (Entities db = new Entities())
            {
                try
                {
                    var result =
                        db.T_US_Message.Where(l => l.Sent == false && l.EmployeeID == _user.EmployeeId).ToList();
                    IEnumerator<T_US_Message> itemEnumerator = result.GetEnumerator();
                    while (itemEnumerator.MoveNext())
                    {
                        itemEnumerator.Current.Sent = true;
                    }
                    itemEnumerator.Dispose();
                    db.SaveChanges();
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }

        public List<T_CH_Task> GetNewTask()
        {
            using (Entities db = new Entities())
            {
                try
                {
                    var result = db.T_CH_Task.Where(l => l.Expire == false && l.StaffID == _user.EmployeeId).ToList();

                    return result;
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }

        public List<T_US_Message> GetNewCc()
        {
            using (Entities db = new Entities())
            {
                try
                {
                    var result =
                        db.T_US_Message.Where(l => l.Sent == false && l.EmployeeID == _user.EmployeeId && l.Type == CcType).ToList();
                    IEnumerator<T_US_Message> itemEnumerator = result.GetEnumerator();
                    while (itemEnumerator.MoveNext())
                    {
                        itemEnumerator.Current.Sent = true;
                    }
                    itemEnumerator.Dispose();
                    db.SaveChanges();
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public int GetTaskCount
        {
            get
            {
                using (Entities db = new Entities())
                {
                    int count = db.T_CH_Task.Where(l => l.StaffID == _user.EmployeeId && l.Expire == false).ToList().Count;

                    return count;
                }
            }

        }
        //获取未发送消息条数
        public int GetNewMessageCount
        {
            get
            {
                using (Entities db = new Entities())
                {
                    int count = db.T_US_Message.Where(l => l.EmployeeID == _user.EmployeeId && l.Sent == false).ToList().Count;

                    return count;
                }
            }
        }

        //获取未处理消息条数
        public int GetMessageCount
        {
            get
            {
                using (Entities db = new Entities())
                {
                    int count = db.T_US_Message.Where(l => l.EmployeeID == _user.EmployeeId && l.Solved == false).ToList().Count;

                    return count;
                }
            }
        }

        public int GetNewCcCount
        {
            get
            {
                using (Entities db = new Entities())
                {
                    int count = db.T_US_Message.Where(l => l.EmployeeID == _user.EmployeeId && l.Sent == false && l.Type == CcType).ToList().Count;

                    return count;
                }
            }
        }

        public int GetCcCount
        {
            get
            {
                using (Entities db = new Entities())
                {
                    int count = db.T_US_Message.Where(l => l.EmployeeID == _user.EmployeeId && l.Solved == false && l.Type == CcType).ToList().Count;

                    return count;
                }
            }
        }
    }

}