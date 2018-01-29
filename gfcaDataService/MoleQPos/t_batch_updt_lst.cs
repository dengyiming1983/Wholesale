using ServiceModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ServiceUitls.Database;

namespace ServiceModel.MoleQPos
{
    public class t_batch_updt_lst : DBModel
    {
        public t_batch_updt_lst(string type) : base(type)
        {
        }

        private static string tableName = "t_batch_updt_lst";

        public string bu_id { get; set; }
        public string bu_code { get; set; }
        public string bu_desc { get; set; }
        public string bu_run_type { get; set; }
        public string bu_start_date { get; set; }
        public string bu_start_time { get; set; }
        public string bu_active_fl { get; set; }
        public string bu_back_date { get; set; }
        public string bu_back_time { get; set; }
        public string bu_back_fl { get; set; }
        public string bu_type { get; set; }
        public string bu_discount_start_date { get; set; }
        public string bu_discount_end_date { get; set; }


        public int locid { get; set; }
        public string centralid { get; set; }
        public int mdo_id { get; set; }

        public override string CreateSaveSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:

                    string where = UniversalSQLStatementGenerator.GetWhereClause(GetWhereClause());

                    sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo."+ tableName + " "+ where + " )");
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

        public override string CreateInsertSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("INSERT INTO dbo."+ tableName + " ( " +
                                       "bu_id,bu_code,bu_desc,bu_run_type,bu_start_date," +
                                       "bu_start_time,bu_active_fl,bu_back_date,bu_back_time,bu_back_fl," +
                                       "bu_type,bu_discount_start_date,bu_discount_end_date," +
                                        "locid"
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append("," + DBValueConvert.ToDBNumber(locid)); // locid
                    //sb.Append(",null"); // central id - new item is null or empty
                    sb.Append(")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA." + tableName + " (" +
                                       "bu_id,bu_code,bu_desc,bu_run_type,bu_start_date," +
                                       "bu_start_time,bu_active_fl,bu_back_date,bu_back_time,bu_back_fl," +
                                       "bu_type,bu_discount_start_date,bu_discount_end_date," +
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

        public override string CreateUpdateSQL()
        {
            return CreateUpdateSQL(new string[]{
                /*"bu_id",*/
                 "bu_code","bu_desc","bu_run_type","bu_start_date","bu_start_time",
                "bu_active_fl","bu_back_date","bu_back_time","bu_back_fl","bu_type",
                "bu_discount_start_date","bu_discount_end_date",
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
                    sb.Append(" WHERE bu_id = " + DBValueConvert.ToDBNumber(bu_id));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA." + tableName + " SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(",mdo_id = " + DBValueConvert.ToDBNumber(mdo_id));
                    sb.Append(" WHERE bu_id = " + DBValueConvert.ToDBNumber(bu_id));
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("DELETE dbo." + tableName);
                    sb.Append(" WHERE bu_id = " + DBValueConvert.ToDBNumber(bu_id));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("DELETE DBA." + tableName);
                    sb.Append(" WHERE bu_id = " + DBValueConvert.ToDBNumber(bu_id));
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public override void Parse(DataRow dataRow)
        {
            throw new NotImplementedException();
        }

        public override string[] GetWhereClause()
        {
            string[] re = null;

            switch (_type)
            {
                case SQL_SERVER:
                    re = new string[]{
                            "bu_id = " + DBValueConvert.ToDBNumber(bu_id),
                            "locid = " + DBValueConvert.ToDBNumber(locid)
                    };
                    break;
                case SYBASE:
                    re = new string[]{
                            "bu_id = " + DBValueConvert.ToDBNumber(bu_id)
                    };
                    break;
                default:
                    break;
            }

            return re;
        }

        private string GetInsertValueString()
        {
            string valueStr = "";
            valueStr += DBValueConvert.ToDBNumber(bu_id) + ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(bu_code) : DBValueConvert.ToDBString(bu_code); valueStr += ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(bu_desc) : DBValueConvert.ToDBString(bu_desc); valueStr += ",";
            valueStr += DBValueConvert.ToDBNumber(bu_run_type) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(bu_start_date)) + ",";
            valueStr += DBValueConvert.ToDBString(bu_start_time) + ",";
            valueStr += DBValueConvert.ToDBString(bu_active_fl) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(bu_back_date)) + ",";
            valueStr += DBValueConvert.ToDBString(bu_back_time) + ",";
            valueStr += DBValueConvert.ToDBString(bu_back_fl) + ",";
            valueStr += DBValueConvert.ToDBString(bu_type) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(bu_discount_start_date)) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(bu_discount_end_date));

            return valueStr;
        }

        private string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";
            if (colnums.Contains("bu_id")) valueStr += "bu_id = " + DBValueConvert.ToDBNumber(bu_id) + ",";
            if (colnums.Contains("bu_code"))
            {
                valueStr += "bu_code = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(bu_code) : DBValueConvert.ToDBString(bu_code);
                valueStr += ",";
            }

            if (colnums.Contains("bu_desc"))
            {
                valueStr += "bu_desc = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(bu_desc) : DBValueConvert.ToDBString(bu_desc);
                valueStr += ",";
            }

            if (colnums.Contains("bu_run_type")) valueStr += "bu_run_type = " + DBValueConvert.ToDBNumber(bu_run_type) + ",";
            if (colnums.Contains("bu_start_date")) valueStr += "bu_start_date = " + DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(bu_start_date)) + ",";
            if (colnums.Contains("bu_start_time")) valueStr += "bu_start_time = " + DBValueConvert.ToDBString(bu_start_time) + ",";
            if (colnums.Contains("bu_active_fl")) valueStr += "bu_active_fl = " + DBValueConvert.ToDBString(bu_active_fl) + ",";
            if (colnums.Contains("bu_back_date")) valueStr += "bu_back_date = " + DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(bu_back_date)) + ",";
            if (colnums.Contains("bu_back_time")) valueStr += "bu_back_time = " + DBValueConvert.ToDBString(bu_back_time) + ",";
            if (colnums.Contains("bu_back_fl")) valueStr += "bu_back_fl = " + DBValueConvert.ToDBString(bu_back_fl) + ",";
            if (colnums.Contains("bu_type")) valueStr += "bu_type = " + DBValueConvert.ToDBString(bu_type) + ",";
            if (colnums.Contains("bu_discount_start_date")) valueStr += "bu_discount_start_date = " + DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(bu_discount_start_date)) + ",";
            if (colnums.Contains("bu_discount_end_date")) valueStr += "bu_discount_end_date = " + DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(bu_discount_end_date)) + ",";

            if (colnums.Count > 1) valueStr = valueStr.Substring(0, valueStr.Length - 1);
            return valueStr;
        }
    }
}
