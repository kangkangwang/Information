using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeerInformation.Extensions
{
    public static class ValidateTool
    {
        public static bool ValidNumCheck(List<dynamic> store, string allowValue, string actualValue)
        {
            try
            {
                foreach (var item in store)
                {
                    var da = item[allowValue].ToString();
                    var db = item[actualValue].ToString();
                    if (db == string.Empty) return false;
                    if (Convert.ToDecimal(db) > Convert.ToDecimal(da)) return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}