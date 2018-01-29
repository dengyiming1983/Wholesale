using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ServiceModel.MoleQPos
{
   public class OutMessage : CommMessage
    {
        public OutMessage(string type) : base(type)
        {
        }

        public override void Parse(DataRow dataRow)
        {
            id = Convert.ToDecimal(dataRow["oc_id"]);
            data = Convert.ToString(dataRow["oc_data"]);
            createTime = DBValueConvert.ToDBDateTime(dataRow["oc_create_time"]);
            executeTime = DBValueConvert.ToDBDateTime(dataRow["oc_execute_time"]);
            completeTime = DBValueConvert.ToDBDateTime(dataRow["oc_complete_time"]);
            storeId = Convert.ToInt32(dataRow["oc_store_id"]);
            status = DBValueConvert.ToDBNumber(dataRow["oc_status"]);
        }

        public override string CreateInsertSQL()
        {
           StringBuilder sb = new StringBuilder();
           
            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("INSERT INTO dbo.t_out_csmsg (oc_data,oc_create_time,oc_execute_time,oc_complete_time,oc_store_id,oc_status) VALUES (" );
                    sb.Append(DBValueConvert.ToDBVarChar(data) + ",");
                    sb.Append(DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(createTime)) + ",");
                    sb.Append(DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(executeTime)) + ",");
                    sb.Append(DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(completeTime)) + ",");
                    sb.Append(storeId + ",");
                    sb.Append(DBValueConvert.ToDBString(status) + ")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA.t_out_csmsg (oc_data,oc_create_time,oc_execute_time,oc_complete_time,oc_store_id,oc_status) VALUES (");
                    sb.Append(DBValueConvert.ToDBString(data) + ",");
                    sb.Append(DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(createTime)) + ",");
                    sb.Append(DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(executeTime)) + ",");
                    sb.Append(DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(completeTime)) + ",");
                    sb.Append(storeId + ",");
                    sb.Append(DBValueConvert.ToDBString(status) + ")");
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public override string CreateUpdateSQL()
        {
            throw new NotImplementedException();
        }

        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" DELETE ");

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append(" FROM dbo.t_out_csmsg ");
                    break;
                case SYBASE:
                    sb.Append(" FROM DBA.t_out_csmsg ");
                    break;
                default:
                    break;
            }
            sb.Append(" WHERE oc_id = " + id);
            //sb.Append(" AND oc_store_id = " + storeId);

            return sb.ToString();
        }
    }
}
