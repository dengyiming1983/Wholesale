using ServiceModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ServiceUitls.Database;

namespace ServiceModel.MoleQPos
{
   public class t_vendor : DBModel
    {
        public t_vendor(string type) : base(type)
        {
        }

        private static string tableName = "t_vendor";

        public string vdr_number { get; set; }
        public string vdr_name { get; set; }
        public string vdr_cname { get; set; }
        public string vdr_address { get; set; }
        public string vdr_city { get; set; }
        public string vdr_state { get; set; }
        public string vdr_zip { get; set; }
        public string vdr_phone { get; set; }
        public string vdr_contact { get; set; }
        public string vdr_remark { get; set; }
        public string vdr_fax { get; set; }
        public string vdr_alt_cntct { get; set; }
        public string vdr_alt_phn { get; set; }
        public string vdr_on_chk { get; set; }
        public string vdr_term { get; set; }
        public string vdr_email { get; set; }
        public string vdr_active { get; set; }
        public string vdr_mail_nm { get; set; }
        public string vdr_discnt_rt { get; set; }
        public string vdr_bill_type { get; set; }
        public string vdr_ctry { get; set; }
        public string vdr_lz_no { get; set; }
        public string vdr_web_site { get; set; }
        public string vdr_extra_info { get; set; }

        public int locid { get; set; }
        public string centralid { get; set; }
        public int mdo_id { get; set; }

        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("DELETE dbo."+ tableName);
                    sb.Append(" WHERE vdr_number = " + DBValueConvert.ToDBString(vdr_number));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("DELETE DBA." + tableName);
                    sb.Append(" WHERE vdr_number = " + DBValueConvert.ToDBString(vdr_number));
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
                    sb.Append("INSERT INTO dbo." + tableName + " ( " +
                                        "vdr_number,vdr_name,vdr_cname,vdr_address,vdr_city," +
                                        "vdr_state,vdr_zip,vdr_phone,vdr_contact,vdr_remark," +
                                        "vdr_fax,vdr_alt_cntct,vdr_alt_phn,vdr_on_chk,vdr_term," +
                                        "vdr_email,vdr_active,vdr_mail_nm,vdr_discnt_rt,vdr_bill_type," +
                                        "vdr_ctry,vdr_lz_no,vdr_web_site,vdr_extra_info," +
                                        "locid"
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append("," + DBValueConvert.ToDBNumber(locid)); // locid
                   // sb.Append(",null"); // central id - new item is null or empty
                    sb.Append(")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA." + tableName + " (" +
                                        "vdr_number,vdr_name,vdr_cname,vdr_address,vdr_city," +
                                        "vdr_state,vdr_zip,vdr_phone,vdr_contact,vdr_remark," +
                                        "vdr_fax,vdr_alt_cntct,vdr_alt_phn,vdr_on_chk,vdr_term," +
                                        "vdr_email,vdr_active,vdr_mail_nm,vdr_discnt_rt,vdr_bill_type," +
                                        "vdr_ctry,vdr_lz_no,vdr_web_site,vdr_extra_info," +
                                        "mdo_id"
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

                    sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo." + tableName + " "+ where + ")");
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
                /*"vdr_number",*/
                 "vdr_name","vdr_cname","vdr_address","vdr_city","vdr_state",
                "vdr_zip","vdr_phone","vdr_contact","vdr_remark","vdr_fax",
                "vdr_alt_cntct","vdr_alt_phn","vdr_on_chk","vdr_term","vdr_email",
                "vdr_active","vdr_mail_nm","vdr_discnt_rt","vdr_bill_type","vdr_ctry",
                "vdr_lz_no","vdr_web_site","vdr_extra_info",
                  });
        }

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo." + tableName + " SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE vdr_number = " + DBValueConvert.ToDBString(vdr_number));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA." + tableName + " SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(",mdo_id = " + DBValueConvert.ToDBNumber(mdo_id));
                    sb.Append(" WHERE vdr_number = " + DBValueConvert.ToDBString(vdr_number));
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
                            "vdr_number = " + DBValueConvert.ToDBString(vdr_number),
                            "locid = " + DBValueConvert.ToDBNumber(locid)
                    };
                    break;
                case SYBASE:
                    re = new string[]{
                            "vdr_number = " + DBValueConvert.ToDBString(vdr_number)
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

        private string GetInsertValueString()
        {
            string valueStr = "";
            valueStr += DBValueConvert.ToDBString(vdr_number) + ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(vdr_name) : DBValueConvert.ToDBString(vdr_name); valueStr += ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(vdr_cname) : DBValueConvert.ToDBString(vdr_cname); valueStr += ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(vdr_address) : DBValueConvert.ToDBString(vdr_address); valueStr += ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(vdr_city) : DBValueConvert.ToDBString(vdr_city); valueStr += ",";
            valueStr += DBValueConvert.ToDBString(vdr_state) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_zip) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_phone) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_contact) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_remark) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_fax) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_alt_cntct) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_alt_phn) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_on_chk) + ",";
            valueStr += DBValueConvert.ToDBNumber(vdr_term) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_email) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_active) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_mail_nm) + ",";
            valueStr += DBValueConvert.ToDBNumber(vdr_discnt_rt) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_bill_type) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_ctry) + ",";
            valueStr += DBValueConvert.ToDBNumber(vdr_lz_no) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_web_site) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_extra_info);

            return valueStr;
        }

        private string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";
            if (colnums.Contains("vdr_number")) valueStr += "vdr_number = " + DBValueConvert.ToDBString(vdr_number) + ",";
            if (colnums.Contains("vdr_name"))
            {
                valueStr += "vdr_name = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(vdr_name) : DBValueConvert.ToDBString(vdr_name);
                valueStr += ",";
            }

            if (colnums.Contains("vdr_cname"))
            {
                valueStr += "vdr_cname = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(vdr_cname) : DBValueConvert.ToDBString(vdr_cname);
                valueStr += ",";
            }

            if (colnums.Contains("vdr_address"))
            {
                valueStr += "vdr_address = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(vdr_address) : DBValueConvert.ToDBString(vdr_address);
                valueStr += ",";
            }

            if (colnums.Contains("vdr_city"))
            {
                valueStr += "vdr_city = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(vdr_city) : DBValueConvert.ToDBString(vdr_city);
                valueStr += ",";
            }

            if (colnums.Contains("vdr_state")) valueStr += "vdr_state = " + DBValueConvert.ToDBString(vdr_state) + ",";
            if (colnums.Contains("vdr_zip")) valueStr += "vdr_zip = " + DBValueConvert.ToDBString(vdr_zip) + ",";
            if (colnums.Contains("vdr_phone")) valueStr += "vdr_phone = " + DBValueConvert.ToDBString(vdr_phone) + ",";
            if (colnums.Contains("vdr_contact")) valueStr += "vdr_contact = " + DBValueConvert.ToDBString(vdr_contact) + ",";
            if (colnums.Contains("vdr_remark")) valueStr += "vdr_remark = " + DBValueConvert.ToDBString(vdr_remark) + ",";
            if (colnums.Contains("vdr_fax")) valueStr += "vdr_fax = " + DBValueConvert.ToDBString(vdr_fax) + ",";
            if (colnums.Contains("vdr_alt_cntct")) valueStr += "vdr_alt_cntct = " + DBValueConvert.ToDBString(vdr_alt_cntct) + ",";
            if (colnums.Contains("vdr_alt_phn")) valueStr += "vdr_alt_phn = " + DBValueConvert.ToDBString(vdr_alt_phn) + ",";
            if (colnums.Contains("vdr_on_chk")) valueStr += "vdr_on_chk = " + DBValueConvert.ToDBString(vdr_on_chk) + ",";
            if (colnums.Contains("vdr_term")) valueStr += "vdr_term = " + DBValueConvert.ToDBNumber(vdr_term) + ",";
            if (colnums.Contains("vdr_email")) valueStr += "vdr_email = " + DBValueConvert.ToDBString(vdr_email) + ",";
            if (colnums.Contains("vdr_active")) valueStr += "vdr_active = " + DBValueConvert.ToDBString(vdr_active) + ",";
            if (colnums.Contains("vdr_mail_nm")) valueStr += "vdr_mail_nm = " + DBValueConvert.ToDBString(vdr_mail_nm) + ",";
            if (colnums.Contains("vdr_discnt_rt")) valueStr += "vdr_discnt_rt = " + DBValueConvert.ToDBNumber(vdr_discnt_rt) + ",";
            if (colnums.Contains("vdr_bill_type")) valueStr += "vdr_bill_type = " + DBValueConvert.ToDBNumber(vdr_bill_type) + ",";
            if (colnums.Contains("vdr_ctry")) valueStr += "vdr_ctry = " + DBValueConvert.ToDBString(vdr_ctry) + ",";
            if (colnums.Contains("vdr_lz_no")) valueStr += "vdr_lz_no = " + DBValueConvert.ToDBNumber(vdr_lz_no) + ",";
            if (colnums.Contains("vdr_web_site")) valueStr += "vdr_web_site = " + DBValueConvert.ToDBString(vdr_web_site) + ",";
            if (colnums.Contains("vdr_extra_info")) valueStr += "vdr_extra_info = " + DBValueConvert.ToDBString(vdr_extra_info) + ",";

            if (colnums.Count > 1) valueStr = valueStr.Substring(0, valueStr.Length - 1);
            return valueStr;
        }
    }
}
