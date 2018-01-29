using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceUitls.Database
{
    /// <summary>
    /// 通用SQL语句生成器
    /// Copyright (C) 2015 By Ming 
    /// QQ: 147877305
    /// Last update 2015-07-04
    /// </summary>
    public class UniversalSQLStatementGenerator
    {
        public static string GetTableCount(string tableName, string where)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT COUNT(*) ");
            sb.Append(" FROM " + tableName);

            if (!string.IsNullOrEmpty(where))
            {
                sb.Append(" WHERE 1=1 AND " + where);
            }

            return sb.ToString();
        }

        public static string GetDelete(string tableName, string where)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" DELETE ");
            sb.Append(" FROM " + tableName);

            if (!string.IsNullOrEmpty(where))
            {
                sb.Append(" WHERE 1=1 AND " + where);
            }

            return sb.ToString();
        }

        public static string GetPageSQLWithWhere(string tableName, string pkName, int pageSize, int page, string where)
        {
            return GetPageSQLWithWhere(null, tableName, pkName, pageSize, page, where);
        }

        public static string GetPageSQLWithWhere(string outputValue, string tableName, string pkName, int pageSize, int page, string where)
        {
            string outValue = string.IsNullOrEmpty(outputValue) ? " *" : outputValue;
            string w1 = string.IsNullOrEmpty(where) ? "" : " AND " + where;
            string w2 = string.IsNullOrEmpty(where) ? "" : " WHERE 1=1 AND " + where;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT TOP " + pageSize + " " + outValue);
            sb.Append(" FROM " + tableName);
            sb.Append(" WHERE ( " + pkName + " >= (");
            sb.Append(" SELECT MAX(" + pkName + ")");
            sb.Append(" FROM ( SELECT TOP " + ((page - 1) * pageSize + 1) + " " + pkName);
            sb.Append(" FROM " + tableName + w2);
            sb.Append(" ORDER BY " + pkName + ") AS T )) " + w1);
            sb.Append(" ORDER BY " + pkName);

            return sb.ToString();

        }

        public static string GetTopRecords(string outputValue, string tableName, string pkName, string where, int top)
        {
            string outValue = string.IsNullOrEmpty(outputValue) ? " *" : outputValue;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT TOP " + top + " " + outValue);
            sb.Append(" FROM " + tableName);
            if (!string.IsNullOrEmpty(where))
            {
                sb.Append(" WHERE 1=1 AND " + where);
            }
            sb.Append(" ORDER BY " + pkName);

            return sb.ToString();

        }

        public static string GetDeleteSQL(string tableName, string where)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" DELETE ");
            sb.Append(" FROM " + tableName);

            if (!string.IsNullOrEmpty(where))
            {
                sb.Append(" WHERE 1=1 AND " + where);
            }

            return sb.ToString();
        }

        /// <summary>
        /// return "WHERE xxx = xxx AND xxx = xxx"
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string GetWhereClause(string[] values)
        {
            if (values == null)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append(" WHERE ");
           
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    sb.Append(" AND " );
               
                sb.Append(values[i]);
            }

            return sb.ToString();
        }

       
    }
}
