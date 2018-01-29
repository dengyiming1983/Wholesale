using ServiceModel.Base;
using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ServiceModel.MoleQPos
{
    /// <summary>
    /// 通信信息模型
    /// </summary>
    public class CommMessage:DBModel
    {
        public decimal id;
        public string data;
        public string createTime;
        public string executeTime;
        public string completeTime;
        public int storeId;
        public string status;

        public CommMessage(string type) : base(type)
        {
        }

        public override string CreateInsertSQL()
        {
            throw new NotImplementedException();
        }

        public override string CreateUpdateSQL()
        {
            throw new NotImplementedException();
        }

        public override string CreateDeleteSQL()
        {
            throw new NotImplementedException();
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

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            throw new NotImplementedException();
        }

        public override string[] GetWhereClause()
        {
            throw new NotImplementedException();
        }

        public override string CreateSaveSQL()
        {
            throw new NotImplementedException();
        }
    }
}
