using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeerInformation.Models;

namespace DeerInformation.Extensions
{
    public static class SerialNum
    {
        public static string NewSerialNum()
        {
            using (Entities db = new Entities())
            {
                var result = db.P_PE_GetSerialNum().ToList();
                return result[0];
            }

        } 
    }
}