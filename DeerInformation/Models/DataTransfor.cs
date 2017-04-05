using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using Newtonsoft.Json.Linq;

namespace DeerInformation.Models
{
    public static class DataTransfor
    {
        #region Nullable转向确定值 （默认值在这里设置）
        public static bool booldata(this Nullable<bool> data)
        {
            return data == null ? false : data.Value;
        }
        public static DateTime datetimedata(this Nullable<DateTime> data)
        {
            return data == null ? new DateTime(1900, 1, 1) : data.Value;
        }

        public static int intdata(this Nullable<int> data)
        {
            return data ?? 0;
        }
        public static Decimal decimaldata(this Nullable<Decimal> data)
        {
            return data ==null?default(Decimal):data.Value;
        }
        #endregion 

        public static bool IntToBool(this int data)
        {
            if (data != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<T> JsonToList<T>(this string src)
        {
            JArray list = JArray.Parse(src);
            return list.Select(item => JSON.Deserialize<T>(item.ToString())).ToList();
        }
    }
}