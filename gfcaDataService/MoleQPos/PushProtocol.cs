using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ServiceModel.MoleQPos
{
    public class PushProtocol
    {
        public int pp_id { get; set; }
        public string pp_data { get; set; }
        public DateTime pp_createtime { get; set; }
        public int locid { get; set; }

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
            return "DELETE DBA.t_comm_protocol WHERE cp_id = " + dataRow["cp_id"];
        }

        public string getInsertString(DataRow dataRow, int storeid)
        {
            String valueStr = "";

            valueStr += "INSERT into DBA.t_push_protocol (pp_data,pp_createtime)";
            valueStr += " values(" + DBValueConvert.ToDBString(dataRow["pp_data"]) + "," +  DBValueConvert.ToDBString(Convert.ToDateTime(dataRow["pp_createtime"]).ToString("yyyy-MM-dd HH:mm:ss")) + ")";

            return valueStr;
        }
    }
}
