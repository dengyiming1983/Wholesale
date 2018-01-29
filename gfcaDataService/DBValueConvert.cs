using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ServiceUitls.Database
{
    public class DBValueConvert
    {

        /// <summary>
        /// Convert field Type for numeric , int ,number 
        /// </summary>
        /// <returns></returns>
        public static string ToDBNumber(Object obj)
        {
            if (obj== null || Convert.IsDBNull(obj) || String.IsNullOrEmpty(obj.ToString().Trim()))
            {
                return "null";
            }

            if (IsInt(obj.ToString().Trim()))
            {
                return Convert.ToDecimal(obj).ToString();
            }

            if (IsNumeric(obj.ToString().Trim()))
            {
               return Convert.ToDecimal(obj).ToString();
            }

            return obj.ToString();
            //return (Convert.IsDBNull(obj) || String.IsNullOrEmpty(obj.ToString())) ? "null" : obj.ToString();
        }

        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }
        public static bool IsUnsign(string value)
        {
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
        }

        public static bool isTel(string strInput)
        {
            return Regex.IsMatch(strInput, @"\d{3}-\d{8}|\d{4}-\d{7}");
        }

        public bool IsNumber(String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) &&
                   objNumberPattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Convert field Type for nvarchar, ntext by SQL Server
        /// </summary>
        /// <returns></returns>
        public static string ToDBVarChar(Object obj)
        {

            return Convert.IsDBNull(obj) ? "null" : "N'" + obj.ToString().Replace("'", "''") + "'";
        }

        /// <summary>
        /// Convert field Type for char , varcahr , text
        /// </summary>
        /// <returns></returns>
        public static string ToDBString(Object obj)
        {
            if (obj == null)
                return "null";

            return  Convert.IsDBNull(obj) ? "null" : "'" + obj.ToString().Replace("'", "''") + "'";
        }

        /// <summary>
        /// Convert field Type for datetime -> eg: '2016-09-29'
        /// </summary>
        /// <returns></returns>
        public static string ToDBDate(object v)
        {
            return Convert.IsDBNull(v) || v == null || string.IsNullOrEmpty(Convert.ToString(v)) ? null : string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(v));
        }

        /// <summary>
        /// Convert field Type for datetime -> eg: '2016-09-29 23:56:36'
        /// </summary>
        /// <returns></returns>
        public static string ToDBDateTime(object v)
        {
            return Convert.IsDBNull(v) || v == null || string.IsNullOrEmpty(Convert.ToString(v))? null : string.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(v));
        }

        /// <summary>
        /// Convert field Type for datetime -> eg: '2016-09-29 23:56:36.888'
        /// </summary>
        /// <returns></returns>
        public static string ToDBTimestamp(object v)
        {
            return Convert.IsDBNull(v) || v == null || string.IsNullOrEmpty(Convert.ToString(v)) ? null : string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", Convert.ToDateTime(v));

        }

        public static String GetMD5(string input)

        {

            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();

            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);

            bs = x.ComputeHash(bs);

            System.Text.StringBuilder s = new System.Text.StringBuilder();

            foreach (byte b in bs)
            {

                s.Append(b.ToString("x2").ToLower());

            }

            return s.ToString();
        }

        public static string GetMD5_2(string sDataIn)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower();
        }

    }
}
