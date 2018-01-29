using ServiceModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ServiceUitls.Database;

namespace ServiceModel.MoleQPos
{
    public class t_measure_dsc : DBModel
    {
        public t_measure_dsc(string type) : base(type)
        {
        }

        public string mes_dsc { get; set; }
        public string mes_short_dsc { get; set; }
        public int mdo_id { get; set; }

        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("DELETE dbo.t_measure_dsc ");
                    sb.Append(" WHERE mes_dsc = " + DBValueConvert.ToDBString(mes_dsc));
                    break;
                case SYBASE:
                    sb.Append("DELETE DBA.t_measure_dsc ");
                    sb.Append(" WHERE mes_dsc = " + DBValueConvert.ToDBString(mes_dsc));
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
                    sb.Append("INSERT INTO dbo.t_measure_dsc ( " +
                                       "mes_dsc,mes_short_dsc" 
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append(")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA.t_measure_dsc (" +
                                         "mes_dsc,mes_short_dsc" +
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
                /*"mes_dsc",*/
                 "mes_short_dsc"
                  });
        }

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo.t_measure_dsc SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE mes_dsc = " + DBValueConvert.ToDBVarChar(mes_dsc));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA.t_measure_dsc SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(",mdo_id = " + DBValueConvert.ToDBNumber(mdo_id));
                    sb.Append(" WHERE mes_dsc = " + DBValueConvert.ToDBString(mes_dsc));
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
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(mes_dsc) : DBValueConvert.ToDBString(mes_dsc); valueStr += ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(mes_short_dsc) : DBValueConvert.ToDBString(mes_short_dsc);
            return valueStr;
        }

        public string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";
            if (colnums.Contains("mes_dsc"))
            {
                valueStr += "mes_dsc = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(mes_dsc) : DBValueConvert.ToDBString(mes_dsc);
                valueStr += ",";
            }

            if (colnums.Contains("mes_short_dsc"))
            {
                valueStr += "mes_short_dsc = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(mes_short_dsc) : DBValueConvert.ToDBString(mes_short_dsc);
                valueStr += ",";
            }
            
            if (colnums.Count > 0) valueStr = valueStr.Substring(0, valueStr.Length - 1);
            return valueStr;
        }

        public override string[] GetWhereClause()
        {
            string[] re = null;

            switch (_type)
            {
                case SQL_SERVER:
                    re = new string[]{
                            "mes_dsc = " + DBValueConvert.ToDBVarChar(mes_dsc)
                    };
                    break;
                case SYBASE:
                    re = new string[]{
                            "mes_dsc = " + DBValueConvert.ToDBString(mes_dsc)
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

                    sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo.t_measure_dsc " + where + " )");
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
