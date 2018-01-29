using ServiceModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ServiceUitls.Database;

namespace ServiceModel.MoleQPos
{
    public class t_item_vdr_link : DBModel
    {
        public t_item_vdr_link(string type) : base(type)
        {
        }

        private static string tableName = "t_item_vdr_link";

        public string link_pcode { get; set; }
        public string link_vdr_no { get; set; }
        public string link_vdr_pcode { get; set; }
        public string link_vdr_price { get; set; }
        public string link_vdr_date { get; set; }
        public string link_auth_fl { get; set; }
        public string link_vdr_org_price { get; set; }
        public string link_last_fl { get; set; }

        public int locid { get; set; }
        public string centralid { get; set; }
        public int mdo_id { get; set; }


        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("DELETE dbo." + tableName );
                    sb.Append(" WHERE link_pcode = " + DBValueConvert.ToDBString(link_pcode));
                    sb.Append(" AND link_vdr_no = " + DBValueConvert.ToDBString(link_vdr_no));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("DELETE DBA." + tableName);
                    sb.Append(" WHERE link_pcode = " + DBValueConvert.ToDBString(link_pcode));
                    sb.Append(" AND link_vdr_no = " + DBValueConvert.ToDBString(link_vdr_no));
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
                    sb.Append("INSERT INTO dbo." + tableName + "( " +
                                       "link_pcode, link_vdr_no, link_vdr_pcode, link_vdr_price, link_vdr_date" +
                                       ",link_auth_fl, link_vdr_org_price, link_last_fl" +
                                        ",locid"
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append("," + DBValueConvert.ToDBNumber(locid)); // locid
                    //sb.Append(",null"); // central id - new item is null or empty
                    sb.Append(")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA." + tableName + " (" +
                                       "link_pcode, link_vdr_no, link_vdr_pcode, link_vdr_price, link_vdr_date" +
                                       ",link_auth_fl, link_vdr_org_price, link_last_fl" +
                                       ",mdo_id"
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

        public override string CreateUpdateSQL()
        {
            return CreateUpdateSQL(new string[]{
                /*"link_pcode","link_vdr_no",*/
                 "link_vdr_pcode","link_vdr_price","link_vdr_date","link_auth_fl","link_vdr_org_price","link_last_fl"
                  });
        }

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo."+tableName +" SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE link_pcode = " + DBValueConvert.ToDBString(link_pcode));
                    sb.Append(" AND link_vdr_no = " + DBValueConvert.ToDBString(link_vdr_no));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA." + tableName + " SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(",mdo_id = " + DBValueConvert.ToDBNumber(mdo_id));
                    sb.Append(" WHERE link_pcode = " + DBValueConvert.ToDBString(link_pcode));
                    sb.Append(" AND link_vdr_no = " + DBValueConvert.ToDBString(link_vdr_no));
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
                            "link_pcode = " + DBValueConvert.ToDBString(link_pcode),
                            "link_vdr_no = " + DBValueConvert.ToDBString(link_vdr_no),
                            "locid = " + DBValueConvert.ToDBNumber(locid)
                    };
                    break;
                case SYBASE:
                    re = new string[]{
                            "link_pcode = " + DBValueConvert.ToDBString(link_pcode),
                            "link_vdr_no = " + DBValueConvert.ToDBString(link_vdr_no),
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

        public string GetInsertValueString()
        {
            string valueStr = "";
            valueStr += DBValueConvert.ToDBString(link_pcode) + ",";
            valueStr += DBValueConvert.ToDBString(link_vdr_no) + ",";
            //valueStr += DBValueConvert.ToDBString(link_vdr_pcode) + ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(link_vdr_pcode) : DBValueConvert.ToDBString(link_vdr_pcode); valueStr += ",";
            valueStr += DBValueConvert.ToDBNumber(link_vdr_price) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(link_vdr_date)) + ",";
            valueStr += DBValueConvert.ToDBString(link_auth_fl) + ",";
            valueStr += DBValueConvert.ToDBNumber(link_vdr_org_price) + ",";
            valueStr += DBValueConvert.ToDBString(link_last_fl);

            return valueStr;
        }


        public string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";
            if (colnums.Contains("link_pcode")) valueStr += "link_pcode = " + DBValueConvert.ToDBString(link_pcode) + ",";
            if (colnums.Contains("link_vdr_no")) valueStr += "link_vdr_no = " + DBValueConvert.ToDBString(link_vdr_no) + ",";
            //if (colnums.Contains("link_vdr_pcode")) valueStr += "link_vdr_no = " + DBValueConvert.ToDBString(link_vdr_pcode) + ",";

            if (colnums.Contains("link_vdr_pcode"))
            {
                valueStr += "link_vdr_pcode = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(link_vdr_pcode) : DBValueConvert.ToDBString(link_vdr_pcode);
                valueStr += ",";
            }

            if (colnums.Contains("link_vdr_price")) valueStr += "link_vdr_price = " + DBValueConvert.ToDBNumber(link_vdr_price) + ",";
            if (colnums.Contains("link_vdr_date")) valueStr += "link_vdr_date = " + DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(link_vdr_date)) + ",";
            if (colnums.Contains("link_auth_fl")) valueStr += "link_auth_fl = " + DBValueConvert.ToDBString(link_auth_fl) + ",";
            if (colnums.Contains("link_vdr_org_price")) valueStr += "link_vdr_org_price = " + DBValueConvert.ToDBNumber(link_vdr_org_price) + ",";
            if (colnums.Contains("link_last_fl")) valueStr += "link_last_fl = " + DBValueConvert.ToDBString(link_last_fl) + ",";

            if (colnums.Count > 1) valueStr = valueStr.Substring(0, valueStr.Length - 1);
            return valueStr;
        }
    }
}
