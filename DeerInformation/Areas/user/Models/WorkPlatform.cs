using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Data.Objects;
using System.Linq;
using System.Web;
using DeerInformation.Models;

namespace DeerInformation.Areas.user.Models
{
    public class WorkPlatform
    {
        /// <summary>
        /// 获取工作日历信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GetCalendarData()
        {
            using (Entities db = new Entities())
            {
                int dayofweek = (int)DateTime.Today.DayOfWeek;
                DateTime beginDate = DateTime.Today.AddDays(-dayofweek);
                DateTime endDate = beginDate.AddDays(6);
                //查询假期时间
                var holidayLst = db.T_HR_Scheduling.Where(l=>( l.StartTime >= beginDate && l.StartTime <= endDate)||(l.EndTime >= beginDate && l.EndTime <= endDate)).ToList();
                List<dynamic> result=new List<dynamic>();
                for (int i = 0; i < 7; i++)
                {
                    var curDate = beginDate.AddDays(i);
                    var flag =
                        holidayLst.FirstOrDefault(
                            l =>
                                (l.StartTime < curDate && l.EndTime > curDate) ||
                                (l.StartTime == curDate && l.HolidayType1 == "上午") || l.EndTime == curDate);
                    result.Add(
                        new {date = curDate.ToShortDateString(),week=((DayOfWeek)i).ToString(), am_pm = "AM", schedule = flag == null ? "工作" : "休假"});
                    flag =
                        holidayLst.FirstOrDefault(
                            l =>
                                (l.StartTime < curDate && l.EndTime > curDate) || l.StartTime == curDate ||
                                (l.EndTime == curDate && l.HolidayType2 == "下午"));
                    result.Add(
                        new { date = curDate.ToShortDateString(),week=((DayOfWeek)i).ToString(), am_pm = "PM", schedule = flag == null ? "工作" : "休假" });
                }
                return result;
            }
        }

        /// <summary>
        /// 获取任务和消息
        /// </summary>
        /// <returns></returns>
        public static List<ViewUnreadMessage> GetUnreadMessage()
        {
            using (Entities db = new Entities())
            {
                LoginUser user = new LoginUser();
                var result = db.T_US_Message.Where(l => l.Solved == false && l.EmployeeID == user.EmployeeId)
                    .Select(msg => new ViewUnreadMessage() { id = msg.ID, title = msg.Title, type = msg.Type, createtime = msg.CreateTime, url = msg.Url })
					.OrderByDescending(l=>l.createtime).ToList();
                return result;
            }
        }
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <returns></returns>
        public static List<ViewTask> GetTask()
        {
            using (Entities db = new Entities())
            {
                LoginUser user = new LoginUser();
                var result = db.V_CH_TaskFunc.Where(l => l.Expire == false && l.StaffID == user.EmployeeId)
                    .Select(msg=>new ViewTask() {type=msg.Name,createtime=msg.CreateTime,endtime=msg.EndTime, url=msg.Url} ).ToList();
                return result;
            }
        }

        internal static List<ViewCcMessage> GetCcMessage()
        {
            using (Entities db = new Entities())
            {
                LoginUser user = new LoginUser();
                var result = db.T_US_Message.Where(l => l.EmployeeID == user.EmployeeId&&l.Type== "抄送消息")
                    .Select(msg => new ViewCcMessage() { id = msg.ID, createtime = msg.CreateTime, title = msg.Title, url = msg.Url, read = msg.Solved })
                    .OrderBy(o => o.read).ThenByDescending(o => o.createtime).ToList();
                return result;
            }
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <returns></returns>
        public static List<ViewMessage> GetMessage()
        {
            using (Entities db = new Entities())
            {
                LoginUser user = new LoginUser();
                var result = db.T_US_Message.Where(l =>l.EmployeeID == user.EmployeeId)
                    .Select(msg => new ViewMessage() {id=msg.ID, type = msg.Type, createtime = msg.CreateTime, title = msg.Title, url = msg.Url, read = msg.Solved })
                    .OrderBy(o=>o.read).ThenByDescending(o=>o.createtime).ToList();
                return result;
            }
        }
        /// <summary>
        /// 获取考勤信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GetCheck()
        {
	        using (Entities db =new Entities())
	        {
		        DateTime date = DateTime.Now;
				LoginUser user=new LoginUser();;
		        return  db.V_HR_AttendAssessWithExceptionType.Where(
					l => l.UserId == user.EmployeeId && EntityFunctions.DiffMonths(l.AttendDate, date) == 0).OrderBy(l => l.AttendDate).ToList();
	        }
        }

        /// <summary>
        /// 获取工作日报信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GetWorkreport()
        {
			using (Entities db = new Entities())
			{
				DateTime date = DateTime.Now;
				LoginUser user = new LoginUser(); ;
				return db.V_US_WorkReportWithState.Where(
					l => l.Creator == user.EmployeeId && EntityFunctions.DiffMonths(l.Date, date) == 0).OrderBy(l => l.Date).ToList();
			}
        }

	    public static T_US_Message ReadMessage(string id)
	    {
		    using (Entities db = new Entities())
		    {
			    int dbId;
			    if (!int.TryParse(id,out dbId))
			    {
				    return null;
			    }
				var result = db.T_US_Message.Find(dbId);
			    if (result!=null)
			    {
				    result.Solved = true;
				    db.SaveChanges();
				    return result;
			    }
		        return null;
		    }
	    }

    }
}