using ServiceModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ServiceUitls.Database;

namespace ServiceModel.MoleQPos
{
    public class t_measure_cnv : DBModel
    {
        public t_measure_cnv(string type) : base(type)
        {
        }

        public string mes_from { get; set; }
        public string mes_to { get; set; }
        public string mes_cnv_val { get; set; }
        public string mes_default { get; set; }
        public int mdo_id { get; set; }

        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("DELETE dbo.t_measure_cnv ");
                    sb.Append(" WHERE mes_from = " + DBValueConvert.ToDBVarChar(mes_from));
                    sb.Append(" AND mes_to = " + DBValueConvert.ToDBVarChar(mes_to));
                    break;
                case SYBASE:
                    sb.Append("DELETE DBA.t_measure_cnv ");
                    sb.Append(" WHERE mes_from = " + DBValueConvert.ToDBString(mes_from));
                    sb.Append(" AND mes_to = " + DBValueConvert.ToDBString(mes_to));
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
                    sb.Append("INSERT INTO dbo.t_measure_cnv ( " +
                                       "mes_from,mes_to,mes_cnv_val,mes_default" 
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append(")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA.t_measure_cnv (" +
                                       "mes_from,mes_to,mes_cnv_val,mes_default"+
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
                /*"mes_from","mes_to",*/
                 "mes_cnv_val","mes_default"
                  });
        }

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo.t_measure_cnv SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE mes_from = " + DBValueConvert.ToDBVarChar(mes_from));
                    sb.Append(" AND mes_to = " + DBValueConvert.ToDBVarChar(mes_to));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA.t_measure_cnv SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(",mdo_id = " + DBValueConvert.ToDBNumber(mdo_id));
                    sb.Append(" WHERE mes_from = " + DBValueConvert.ToDBString(mes_from));
                    sb.Append(" AND mes_to = " + DBValueConvert.ToDBString(mes_to));
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
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(mes_from) : DBValueConvert.ToDBString(mes_from); valueStr += ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(mes_to) : DBValueConvert.ToDBString(mes_to); valueStr += ",";
            valueStr += DBValueConvert.ToDBNumber(mes_cnv_val) + ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(mes_default) : DBValueConvert.ToDBString(mes_default);

            return valueStr;
        }

        public string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";
            if (colnums.Contains("mes_from"))
            {
                valueStr += "mes_from = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(mes_from) : DBValueConvert.ToDBString(mes_from);
                valueStr += ",";
            }

            if (colnums.Contains("mes_to"))
            {
                valueStr += "mes_to = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(mes_to) : DBValueConvert.ToDBString(mes_to);
                valueStr += ",";
            }

            if (colnums.Contains("mes_cnv_val")) valueStr += "mes_cnv_val = " + DBValueConvert.ToDBNumber(mes_cnv_val) + ",";
            if (colnums.Contains("mes_default"))
            {
                valueStr += "mes_default = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(mes_default) : DBValueConvert.ToDBString(mes_default);
                valueStr += ",";
            }

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
                            "mes_from = " + DBValueConvert.ToDBVarChar(mes_from),
                            "mes_to = " + DBValueConvert.ToDBVarChar(mes_to)
                    };
                    break;
                case SYBASE:
                    re = new string[]{
                            "mes_from = " + DBValueConvert.ToDBString(mes_from),
                            "mes_to = " + DBValueConvert.ToDBString(mes_to)
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

                    sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo.t_measure_cnv " + where + " )");
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
