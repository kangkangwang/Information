using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;

namespace DeerInformation.Models
{
    public static class PageHelper
    {
        public static Paging<T> GetPage<T>(this List<T> list, StoreRequestParameters parameters) 
        {
            int start = parameters.Start;
            int limit = parameters.Limit;

            if ((start + limit) > list.Count)
            {
                limit = list.Count - start;
            }
            List<T> ranglist = (start < 0 || limit < 0) ? list : list.GetRange(start, limit);
            return new Paging<T>(ranglist, list.Count);
        }
    }
}