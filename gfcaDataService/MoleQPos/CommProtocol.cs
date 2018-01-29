using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ServiceModel.MoleQPos
{
    public class CommProtocol
    {
        public int cp_id { get; set; }
        public string cp_data { get; set; }
        public DateTime cp_local_time { get; set; }

        public string GetSql(DataRow dataRow, int storeid)
        {
            string result = "";
            //result += getUpdateString(dataRow);
            //result += " IF @@ROWCOUNT=0 ";
            result += getInsertString(dataRow, storeid) + ";";
            return result;
        }

        public string GetDeleteSQL(DataRow dataRow)
        {
            return "DELETE dbo.t_comm_protocol WHERE cp_id = " + dataRow["cp_id"];
        }

        public string getInsertString(DataRow dataRow, int storeid)
        {
            String valueStr = "";

            valueStr += "INSERT into dbo.t_comm_protocol (cp_data,cp_local_time,cp_locid)";
            valueStr += " values(" + DBValueConvert.ToDBVarChar(dataRow["cp_data"]) + "," + DBValueConvert.ToDBString(dataRow["cp_local_time"]) + "," + storeid + ")";

            return valueStr;
        }
    }
}
