using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ServiceModel.MoleQPos
{
   public class InMessage:CommMessage
    {
        public InMessage(string type) : base(type)
        {
        }

        public string msg ="";

        public override void Parse(DataRow dataRow)
        {
            id = Convert.ToDecimal(dataRow["ic_id"]);
            data = Convert.ToString(dataRow["ic_data"]);
            createTime = DBValueConvert.ToDBDateTime(dataRow["ic_create_time"]);
            executeTime = DBValueConvert.ToDBDateTime(dataRow["ic_execute_time"]);
            completeTime = DBValueConvert.ToDBDateTime(dataRow["ic_complete_time"]);
            storeId = Convert.ToInt32(dataRow["ic_store_id"]);
            status = DBValueConvert.ToDBNumber(dataRow["ic_status"]);
            msg = DBValueConvert.ToDBNumber(dataRow["ic_msg"]);
        }

        public override string CreateInsertSQL()
        {
            StringBuilder sb = new StringBuilder();
           
            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("INSERT INTO dbo.t_in_csmsg (ic_data,ic_create_time,ic_execute_time,ic_complete_time,ic_store_id,ic_status) VALUES (" );
                    sb.Append(DBValueConvert.ToDBVarChar(data) + ",");
                    sb.Append(DBValueConvert.ToDBString(createTime) + ",");
                    sb.Append(DBValueConvert.ToDBString(executeTime) + ",");
                    sb.Append(DBValueConvert.ToDBString(completeTime) + ",");
                    sb.Append(storeId + ",");
                    sb.Append(DBValueConvert.ToDBString(status) + ")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA.t_in_csmsg (ic_data,ic_create_time,ic_execute_time,ic_complete_time,ic_store_id,ic_status) VALUES (");
                    sb.Append(DBValueConvert.ToDBString(data) + ",");
                    sb.Append(DBValueConvert.ToDBString(createTime) + ",");
                    sb.Append(DBValueConvert.ToDBString(executeTime) + ",");
                    sb.Append(DBValueConvert.ToDBString(completeTime) + ",");
                    sb.Append(storeId + ",");
                    sb.Append(DBValueConvert.ToDBString(status) + ")");
                    break;
                default:
                    break;
            }

            return sb.ToString();
          
        }

      
        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" DELETE ");

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append(" FROM dbo.t_in_csmsg ");
                    break;
                case SYBASE:
                    sb.Append(" FROM DBA.t_in_csmsg ");
                    break;
                default:
                    break;
            }
            sb.Append(" WHERE ic_id = " + id);
           // sb.Append(" AND ic_store_id = " + storeId);

            return sb.ToString();
        }

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo.t_in_csmsg SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE ic_id = " + id);

                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA.t_in_csmsg SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE ic_id = " + id);
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";

            if (colnums.Contains("ic_id")) valueStr += "ic_id = " + id + ",";
            if (colnums.Contains("ic_data"))
            {
                valueStr += "ic_data = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(data) : DBValueConvert.ToDBString(data);
                valueStr += ",";
            }
            if (colnums.Contains("ic_create_time")) valueStr += "ic_create_time = " + DBValueConvert.ToDBString(createTime) + ",";
            if (colnums.Contains("ic_execute_time")) valueStr += "ic_execute_time = " + DBValueConvert.ToDBString(executeTime) + ",";
            if (colnums.Contains("ic_complete_time")) valueStr += "ic_complete_time = " + DBValueConvert.ToDBString(completeTime) + ",";
            if (colnums.Contains("ic_store_id")) valueStr += "ic_store_id = " + storeId + ",";
            if (colnums.Contains("ic_status")) valueStr += "ic_status = " + DBValueConvert.ToDBString(status) + ",";
            if (colnums.Contains("ic_msg"))
            {
                valueStr += "ic_msg = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(msg) : DBValueConvert.ToDBString(msg);
                valueStr += ",";
            }
          
            if (colnums.Count > 1) valueStr = valueStr.Substring(0, valueStr.Length - 1);
            return valueStr;
        }
    }
}
