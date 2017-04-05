using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace DeerInformation.Extensions
{
    public static class EncryptionCommon
    {
        public static string Sha256(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            SHA256Managed com = new SHA256Managed();
            byte[] code = com.ComputeHash(data);
            return BitConverter.ToString(code).Replace("-", "").ToLower();
        }

    }
}