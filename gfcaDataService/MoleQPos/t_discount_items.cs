using ServiceModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ServiceUitls.Database;

namespace ServiceModel.MoleQPos
{
    public class t_discount_items: DBModel
    {

        public string p_code { get; set; }
        public string box_prc_save { get; set; }
        public string unit_prc_save { get; set; }
        public string start_dt { get; set; }
        public string end_dt { get; set; }
        public string limit_no { get; set; }
        public int locid { get; set; }
        public int mdo_id { get; set; }
        public string no_exp { get; set; }
        public string plus_vip { get; set; }
        public string plus_mbrvip { get; set; }
        public string plus_reward { get; set; }


        public t_discount_items(string type) : base(type)
        {
        }

        public override string CreateInsertSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("INSERT INTO dbo.t_discount_items ( " +
                                       "p_code,box_prc_save,unit_prc_save,start_dt,end_dt" +
                                       ",limit_no,no_exp,plus_vip,plus_mbrvip,plus_reward" +
                                        ",locid"
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append("," + DBValueConvert.ToDBNumber(locid)); // locid
                    sb.Append(")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA.t_discount_items (" +
                                       "p_code,box_prc_save,unit_prc_save,start_dt,end_dt" +
                                       ",limit_no,no_exp,plus_vip,plus_mbrvip,plus_reward,mdo_id"
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
                /*"p_code",*/
                "box_prc_save","unit_prc_save","start_dt","end_dt" ,"limit_no","no_exp","plus_vip","plus_mbrvip","plus_reward"
                  });

        }

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo.t_discount_items SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE p_code = " + DBValueConvert.ToDBString(p_code));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA.t_discount_items SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(",mdo_id = " + DBValueConvert.ToDBNumber(mdo_id));
                    sb.Append(" WHERE p_code = " + DBValueConvert.ToDBString(p_code));
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
                    sb.Append("DELETE dbo.t_discount_items ");
                    sb.Append(" WHERE p_code = " + DBValueConvert.ToDBString(p_code));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("DELETE DBA.t_discount_items ");
                    sb.Append(" WHERE p_code = " + DBValueConvert.ToDBString(p_code));
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

        public string GetInsertValueString()
        {
            string valueStr = "";
            valueStr += DBValueConvert.ToDBString(p_code) + ",";
            valueStr += DBValueConvert.ToDBNumber(box_prc_save) + ",";
            valueStr += DBValueConvert.ToDBNumber(unit_prc_save) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(start_dt)) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(end_dt)) + ",";
            valueStr += DBValueConvert.ToDBNumber(limit_no) + ",";
            valueStr += DBValueConvert.ToDBString(no_exp) + ",";
            valueStr += DBValueConvert.ToDBString(plus_vip) + ",";
            valueStr += DBValueConvert.ToDBString(plus_mbrvip) + ",";
            valueStr += DBValueConvert.ToDBString(plus_reward);

            return valueStr;
        }

        public string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";
            if (colnums.Contains("p_code")) valueStr += "p_code = " + DBValueConvert.ToDBString(p_code) + ",";
            if (colnums.Contains("box_prc_save")) valueStr += "box_prc_save = " + DBValueConvert.ToDBNumber(box_prc_save) + ",";
            if (colnums.Contains("unit_prc_save")) valueStr += "unit_prc_save = " + DBValueConvert.ToDBNumber(unit_prc_save) + ",";
            if (colnums.Contains("start_dt")) valueStr += "start_dt = " +DBValueConvert.ToDBString( DBValueConvert.ToDBDateTime(start_dt)) + ",";
            if (colnums.Contains("end_dt")) valueStr += "end_dt = " + DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(end_dt)) + ",";
            if (colnums.Contains("limit_no")) valueStr += "limit_no = " + DBValueConvert.ToDBNumber(limit_no) + ",";

            if (colnums.Contains("no_exp")) valueStr += "no_exp = " + DBValueConvert.ToDBString(no_exp) + ",";
            if (colnums.Contains("plus_vip")) valueStr += "plus_vip = " + DBValueConvert.ToDBString(plus_vip) + ",";
            if (colnums.Contains("plus_mbrvip")) valueStr += "plus_mbrvip = " + DBValueConvert.ToDBString(plus_mbrvip) + ",";
            if (colnums.Contains("plus_reward")) valueStr += "plus_reward = " + DBValueConvert.ToDBString(plus_reward) + ",";

            if (colnums.Count > 1) valueStr = valueStr.Substring(0, valueStr.Length - 1);
            return valueStr;
        }

        public override string[] GetWhereClause()
        {
            string[] re = null;

            switch (_type)
            {
                case SQL_SERVER:
                    re = new string[]{
                            "p_code = " + DBValueConvert.ToDBString(p_code),
                            "locid = " + DBValueConvert.ToDBNumber(locid)
                    };
                    break;
                case SYBASE:
                    re = new string[]{
                            "p_code = " + DBValueConvert.ToDBString(p_code)
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

                    sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo.t_discount_items " + where + " )");
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
