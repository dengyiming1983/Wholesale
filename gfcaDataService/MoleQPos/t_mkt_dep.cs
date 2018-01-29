using ServiceModel.Base;
using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ServiceModel.MoleQPos
{
    public class t_mkt_dep : DBModel
    {

        public string dep_no { get; set; }
        public string dep_name { get; set; }
        public string dep_display { get; set; }
        public string dep_fsmp_fl { get; set; }
        public string dep_tax_fl { get; set; }
        public string dep_tax_grp { get; set; }
        public string dep_no_vip_dnt { get; set; }
        public string show_on_pos { get; set; }
        public int locid { get; set; }
        public string centralid { get; set; }
        public int mdo_id { get; set; }

        public t_mkt_dep(string type) : base(type)
        {
        }

        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("DELETE dbo.t_mkt_dep ");
                    sb.Append(" WHERE dep_no = " + DBValueConvert.ToDBNumber(dep_no));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("DELETE DBA.t_mkt_dep ");
                    sb.Append(" WHERE dep_no = " + DBValueConvert.ToDBNumber(dep_no));
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
                    sb.Append("INSERT INTO dbo.t_mkt_dep ( "+ 
                                       "dep_no,dep_name,dep_display,dep_fsmp_fl,dep_tax_fl" +
                                       ",dep_tax_grp,dep_no_vip_dnt,show_on_pos" +
                                        ",locid,centralid"
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append("," + DBValueConvert.ToDBNumber(locid)); // locid
                    sb.Append(",null"); // central id - new item is null or empty
                    sb.Append(")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA.t_mkt_dep ("+ 
                                       "dep_no,dep_name,dep_display,dep_fsmp_fl,dep_tax_fl" +
                                       ",dep_tax_grp,dep_no_vip_dnt,show_on_pos"+
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

        public override string CreateUpdateSQL()
        {
            return CreateUpdateSQL(new string[]{
                /*"dep_no",*/
                 "dep_name","dep_display","dep_fsmp_fl","dep_tax_fl","dep_tax_grp","dep_no_vip_dnt","show_on_pos"
                  });
        }

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo.t_mkt_dep SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE dep_no = " + DBValueConvert.ToDBNumber(dep_no));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA.t_mkt_dep SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(",mdo_id = " + DBValueConvert.ToDBNumber(mdo_id));
                    sb.Append(" WHERE dep_no = " + DBValueConvert.ToDBNumber(dep_no));
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
            valueStr += DBValueConvert.ToDBNumber(dep_no) + ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(dep_name) : DBValueConvert.ToDBString(dep_name); valueStr += ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(dep_display) : DBValueConvert.ToDBString(dep_display); valueStr += ",";
            valueStr += DBValueConvert.ToDBNumber(dep_fsmp_fl) + ",";
            valueStr += DBValueConvert.ToDBString(dep_tax_fl) + ",";
            valueStr += DBValueConvert.ToDBNumber(dep_tax_grp) + ",";
            valueStr += DBValueConvert.ToDBString(dep_no_vip_dnt) + ",";
            valueStr += DBValueConvert.ToDBString(show_on_pos);

            return valueStr;
        }

        public string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";
            if (colnums.Contains("dep_no")) valueStr += "dep_no = " + DBValueConvert.ToDBNumber(dep_no) + ",";
            if (colnums.Contains("dep_name"))
            {
                valueStr += "dep_name = " ;
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(dep_name) : DBValueConvert.ToDBString(dep_name);
                valueStr += ",";
            }

            if (colnums.Contains("dep_display"))
            {
                valueStr += "dep_display = " ;
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(dep_display) : DBValueConvert.ToDBString(dep_display);
                valueStr += ",";
            }

            if (colnums.Contains("dep_fsmp_fl")) valueStr += "dep_fsmp_fl = " + DBValueConvert.ToDBNumber(dep_fsmp_fl) + ",";
            if (colnums.Contains("dep_tax_fl")) valueStr += "dep_tax_fl = " + DBValueConvert.ToDBString(dep_tax_fl) + ",";
            if (colnums.Contains("dep_tax_grp")) valueStr += "dep_tax_grp = " + DBValueConvert.ToDBNumber(dep_tax_grp) + ",";
            if (colnums.Contains("dep_no_vip_dnt")) valueStr += "dep_no_vip_dnt = " + DBValueConvert.ToDBString(dep_no_vip_dnt) + ",";
            if (colnums.Contains("show_on_pos")) valueStr += "show_on_pos = " + DBValueConvert.ToDBString(show_on_pos) + ",";
            
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
                            "dep_no = " + DBValueConvert.ToDBNumber(dep_no),
                            "locid = " + DBValueConvert.ToDBNumber(locid)
                    };
                    break;
                case SYBASE:
                    re = new string[]{
                            "dep_no = " + DBValueConvert.ToDBNumber(dep_no)
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

                    sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo.t_mkt_dep " + where + " )");
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
