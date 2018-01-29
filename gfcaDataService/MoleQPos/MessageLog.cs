using ServiceModel.Base;
using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ServiceModel.MoleQPos
{
   public class MessageLog : DBModel
    {
        public decimal cl_id { get; set; }
        public string cl_msg { get; set; }
        public System.DateTime cl_create_time { get; set; }
        public System.DateTime cl_execute_time { get; set; }
        public int cl_origin_id { get; set; }
        public int cl_store_id { get; set; }


        public MessageLog(string type) : base(type)
        {
        }

        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("DELETE dbo.t_csmsg_log ");
                    sb.Append(" WHERE cl_id = " + DBValueConvert.ToDBNumber(cl_id));
                    break;
                case SYBASE:
                    sb.Append("DELETE DBA.t_csmsg_log ");
                    sb.Append(" WHERE cl_id = " + DBValueConvert.ToDBNumber(cl_id));
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
                    sb.Append("INSERT INTO dbo.t_csmsg_log(cl_msg,cl_create_time,cl_execute_time,cl_origin_id,cl_store_id) VALUES (");
                   // sb.Append("null,");
                    sb.Append(DBValueConvert.ToDBVarChar(cl_msg) + ",");
                    sb.Append(DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(cl_create_time)) + ",");
                    sb.Append(DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(cl_execute_time)) + ",");
                    sb.Append(cl_origin_id + ",");
                    sb.Append(cl_store_id + ")" );
                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA.t_csmsg_log(cl_id,cl_msg,cl_create_time,cl_execute_time,cl_origin_id) VALUES (");
                    sb.Append("null,");
                    sb.Append(DBValueConvert.ToDBString(cl_msg) + ",");
                    sb.Append(DBValueConvert.ToDBString ( DBValueConvert.ToDBDateTime(cl_create_time ) ) + ",");
                    sb.Append(DBValueConvert.ToDBString ( DBValueConvert.ToDBDateTime(cl_execute_time ) ) + ",");
                    sb.Append(cl_origin_id + ")");
                    break;
                default:
                    break;

            }
            return sb.ToString();
        }

        public override string CreateUpdateSQL()
        {
            return CreateUpdateSQL(new string[]{
                 /*"cl_id",*/
                 "cl_msg","cl_create_time","cl_execute_time","cl_origin_id","cl_store_id"
                  });
        }

        public string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";

            if (colnums.Contains("cl_id")) valueStr += "cl_id = " + DBValueConvert.ToDBNumber(cl_id) + ",";
            if (colnums.Contains("cl_msg"))
            {
                valueStr += "cl_msg = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(cl_msg) : DBValueConvert.ToDBString(cl_msg);
                valueStr += ",";
            }
           
            if (colnums.Contains("cl_create_time"))  valueStr += "cl_create_time = " + DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(cl_create_time)) + ",";
            if (colnums.Contains("cl_execute_time")) valueStr += "cl_execute_time = " +  DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(cl_execute_time)) + ",";
            if (colnums.Contains("cl_origin_id")) valueStr += "cl_origin_id = " + DBValueConvert.ToDBNumber(cl_origin_id) + ",";
            if (colnums.Contains("cl_store_id")) valueStr += "cl_store_id = " + DBValueConvert.ToDBNumber(cl_store_id) + ",";

          
            if (colnums.Count > 1) valueStr = valueStr.Substring(0, valueStr.Length - 1);
            return valueStr;
        }

        public override void Parse(DataRow dataRow)
        {
            cl_id = Convert.ToInt32(dataRow["cl_id"]);
            cl_msg = DBValueConvert.ToDBNumber(dataRow["cl_msg"]);
            cl_create_time = Convert.ToDateTime(dataRow["cl_create_time"]);
            cl_execute_time = Convert.ToDateTime(dataRow["cl_execute_time"]);
            cl_origin_id = Convert.ToInt32(dataRow["cl_origin_id"]);

            if(_type == SQL_SERVER)
                cl_store_id = Convert.ToInt32(dataRow["cl_store_id"]);
          
           
        }

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo.t_csmsg_log SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE cl_id = " + DBValueConvert.ToDBNumber(cl_id));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA.t_csmsg_log SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE cl_id = " + DBValueConvert.ToDBNumber(cl_id));
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
                            "cl_id = " + DBValueConvert.ToDBNumber(cl_id)
                    };
                    break;
                case SYBASE:
                    re = new string[]{
                            "cl_id = " + DBValueConvert.ToDBNumber(cl_id)
                    };
                    break;
                default:
                    break;
            }

            return re;
        }

        public override string CreateSaveSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:

                    string where = UniversalSQLStatementGenerator.GetWhereClause(GetWhereClause());

                    sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo.t_csmsg_log " + where + " )");
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
    }
}
