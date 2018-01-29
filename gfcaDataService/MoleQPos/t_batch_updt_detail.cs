using ServiceModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ServiceUitls.Database;

namespace ServiceModel.MoleQPos
{
    public class t_batch_updt_detail : DBModel
    {
        private static string tableName = "t_batch_updt_detail";

        public t_batch_updt_detail(string type) : base(type)
        {
        }

        public string bd_id { get; set; }
        public string bd_bu_id { get; set; }
        public string bd_item { get; set; }
        public string bd_price { get; set; }
        public string bd_new_price { get; set; }
        public string bd_fl { get; set; }
        public string bd_date { get; set; }
        public string bd_start_time { get; set; }
        public string bd_cmpl_time { get; set; }
        public string bd_create_time { get; set; }
        public string bd_type { get; set; }
        public string bd_bu_type { get; set; }
        public string bd_discnt_start_date { get; set; }
        public string bd_discnt_end_date { get; set; }
        public string bd_discnt_limit { get; set; }
        public string bd_no_exp { get; set; }
        public string bd_plus_vip { get; set; }
        public string bd_plus_mbrvip { get; set; }
        public string bd_plus_reward { get; set; }

        public int locid { get; set; }
        public string centralid { get; set; }
        public int mdo_id { get; set; }


        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("DELETE dbo." + tableName);
                    sb.Append(" WHERE bd_id = " + DBValueConvert.ToDBNumber(bd_id));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("DELETE DBA." + tableName);
                    sb.Append(" WHERE bd_id = " + DBValueConvert.ToDBNumber(bd_id));
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public override string CreateInsertSQL()
        {

            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("INSERT INTO dbo." + tableName + " ( " +
                                        "bd_id,bd_bu_id,bd_item,bd_price,bd_new_price," +
                                        "bd_fl,bd_date,bd_start_time,bd_cmpl_time,bd_create_time," +
                                        "bd_type,bd_bu_type,bd_discnt_start_date,bd_discnt_end_date,bd_discnt_limit," +
                                        "bd_no_exp,bd_plus_vip,bd_plus_mbrvip,bd_plus_reward," +
                                        "locid"
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append("," + DBValueConvert.ToDBNumber(locid)); // locid
                   // sb.Append(",null"); // central id - new item is null or empty
                    sb.Append(")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA." + tableName + " (" +
                                        "bd_id,bd_bu_id,bd_item,bd_price,bd_new_price," +
                                        "bd_fl,bd_date,bd_start_time,bd_cmpl_time,bd_create_time," +
                                        "bd_type,bd_bu_type,bd_discnt_start_date,bd_discnt_end_date,bd_discnt_limit," +
                                        "bd_no_exp,bd_plus_vip,bd_plus_mbrvip,bd_plus_reward," +
                                        "mdo_id"
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append("," + DBValueConvert.ToDBNumber(mdo_id)); // mdo_id
                    sb.Append(")");
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public override string CreateSaveSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:

                    string where = UniversalSQLStatementGenerator.GetWhereClause(GetWhereClause());

                    sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo." + tableName + " " + where + " )");
                    sb.Append(" BEGIN ");
                    sb.Append(CreateInsertSQL());
                    sb.Append(" END ");
                    sb.Append(" ELSE BEGIN ");
                    sb.Append(CreateUpdateSQL());
                    sb.Append(" END ");

                    break;
                case SYBASE:

                    throw new NotImplementedException();

                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public override string CreateUpdateSQL()
        {
            return CreateUpdateSQL(new string[]{
                /*"bd_id",*/
                "bd_bu_id","bd_item","bd_price","bd_new_price","bd_fl",
                "bd_date","bd_start_time","bd_cmpl_time","bd_create_time","bd_type",
                "bd_bu_type","bd_discnt_start_date","bd_discnt_end_date","bd_discnt_limit","bd_no_exp",
                "bd_plus_vip","bd_plus_mbrvip","bd_plus_reward",
                  });
        }

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo." + tableName + " SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE bd_id = " + DBValueConvert.ToDBNumber(bd_id));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA." + tableName + " SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(",mdo_id = " + DBValueConvert.ToDBNumber(mdo_id));
                    sb.Append(" WHERE bd_id = " + DBValueConvert.ToDBNumber(bd_id));
                    break;
                default:
                    break;
            }
            return sb.ToString();
        }

        public override string[] GetWhereClause()
        {
            string[] re = null;

            switch (_type)
            {
                case SQL_SERVER:
                    re = new string[]{
                            "bd_id = " + DBValueConvert.ToDBNumber(bd_id),
                            "locid = " + DBValueConvert.ToDBNumber(locid)
                    };
                    break;
                case SYBASE:
                    re = new string[]{
                            "bd_id = " + DBValueConvert.ToDBString(bd_id)
                    };
                    break;
                default:
                    break;
            }

            return re;
        }

        public override void Parse(DataRow dataRow)
        {
            throw new NotImplementedException();
        }

        private string GetInsertValueString()
        {
            string valueStr = "";
            valueStr += DBValueConvert.ToDBNumber(bd_id) + ",";
            valueStr += DBValueConvert.ToDBNumber(bd_bu_id) + ",";
            valueStr += DBValueConvert.ToDBString(bd_item) + ",";
            valueStr += DBValueConvert.ToDBNumber(bd_price) + ",";
            valueStr += DBValueConvert.ToDBNumber(bd_new_price) + ",";
            valueStr += DBValueConvert.ToDBString(bd_fl) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(bd_date)) + ",";
            valueStr += DBValueConvert.ToDBString(bd_start_time) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBTimestamp(bd_cmpl_time)) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBTimestamp(bd_create_time)) + ",";
            valueStr += DBValueConvert.ToDBString(bd_type) + ",";
            valueStr += DBValueConvert.ToDBString(bd_bu_type) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDate(bd_discnt_start_date)) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDate(bd_discnt_end_date)) + ",";
            valueStr += DBValueConvert.ToDBNumber(bd_discnt_limit) + ",";
            valueStr += DBValueConvert.ToDBString(bd_no_exp) + ",";
            valueStr += DBValueConvert.ToDBString(bd_plus_vip) + ",";
            valueStr += DBValueConvert.ToDBString(bd_plus_mbrvip) + ",";
            valueStr += DBValueConvert.ToDBString(bd_plus_reward);

            return valueStr;
        }


        private string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";
            if (colnums.Contains("bd_id")) valueStr += "bd_id = " + DBValueConvert.ToDBNumber(bd_id) + ",";
            if (colnums.Contains("bd_bu_id")) valueStr += "bd_bu_id = " + DBValueConvert.ToDBNumber(bd_bu_id) + ",";
            if (colnums.Contains("bd_item")) valueStr += "bd_item = " + DBValueConvert.ToDBString(bd_item) + ",";
            if (colnums.Contains("bd_price")) valueStr += "bd_price = " + DBValueConvert.ToDBNumber(bd_price) + ",";
            if (colnums.Contains("bd_new_price")) valueStr += "bd_new_price = " + DBValueConvert.ToDBNumber(bd_new_price) + ",";
            if (colnums.Contains("bd_fl")) valueStr += "bd_fl = " + DBValueConvert.ToDBString(bd_fl) + ",";
            if (colnums.Contains("bd_date")) valueStr += "bd_date = " + DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(bd_date)) + ",";
            if (colnums.Contains("bd_start_time")) valueStr += "bd_start_time = " + DBValueConvert.ToDBString(bd_start_time) + ",";
            if (colnums.Contains("bd_cmpl_time")) valueStr += "bd_cmpl_time = " + DBValueConvert.ToDBString(DBValueConvert.ToDBTimestamp(bd_cmpl_time)) + ",";
            if (colnums.Contains("bd_create_time")) valueStr += "bd_create_time = " + DBValueConvert.ToDBString(DBValueConvert.ToDBTimestamp(bd_create_time)) + ",";
            if (colnums.Contains("bd_type")) valueStr += "bd_type = " + DBValueConvert.ToDBString(bd_type) + ",";
            if (colnums.Contains("bd_bu_type")) valueStr += "bd_bu_type = " + DBValueConvert.ToDBString(bd_bu_type) + ",";
            if (colnums.Contains("bd_discnt_start_date")) valueStr += "bd_discnt_start_date = " + DBValueConvert.ToDBString(DBValueConvert.ToDBDate(bd_discnt_start_date)) + ",";
            if (colnums.Contains("bd_discnt_end_date")) valueStr += "bd_discnt_end_date = " + DBValueConvert.ToDBString(DBValueConvert.ToDBDate(bd_discnt_end_date)) + ",";
            if (colnums.Contains("bd_discnt_limit")) valueStr += "bd_discnt_limit = " + DBValueConvert.ToDBNumber(bd_discnt_limit) + ",";
            if (colnums.Contains("bd_no_exp")) valueStr += "bd_no_exp = " + DBValueConvert.ToDBString(bd_no_exp) + ",";
            if (colnums.Contains("bd_plus_vip")) valueStr += "bd_plus_vip = " + DBValueConvert.ToDBString(bd_plus_vip) + ",";
            if (colnums.Contains("bd_plus_mbrvip")) valueStr += "bd_plus_mbrvip = " + DBValueConvert.ToDBString(bd_plus_mbrvip) + ",";
            if (colnums.Contains("bd_plus_reward")) valueStr += "bd_plus_reward = " + DBValueConvert.ToDBString(bd_plus_reward) + ",";

            if (colnums.Count > 1) valueStr = valueStr.Substring(0, valueStr.Length - 1);
            return valueStr;
        }
    }
}
