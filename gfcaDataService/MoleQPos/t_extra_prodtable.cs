using ServiceModel.Base;
using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ServiceModel.MoleQPos
{

    public class t_extra_prodtable : DBModel
    {
        public t_extra_prodtable(string type) : base(type)
        {
        }

        public string p_code { get; set; }
        public string lost_pct { get; set; }
        public string good_for_day { get; set; }
        public string part_no { get; set; }
        public string child_fl { get; set; }
        public string parent_code { get; set; }
        public string ref_cost { get; set; }
        public string mo_no { get; set; }
        public string tax_grp { get; set; }
        public string no_vip_dnt { get; set; }
        public string legal_age { get; set; }
        public string wic_fl { get; set; }
        public string wic_prc { get; set; }
        public string rv_item { get; set; }
        public string final_sale { get; set; }
        public string case_qty { get; set; }
        public string expected_gpm { get; set; }
        public string actual_gpm { get; set; }
        public string cvv_fl { get; set; }
        public string misc_fl { get; set; }
        public string service_fl { get; set; }
        public string location_code { get; set; }
        //public string dnt_plus_vip { get; set; }
        //public string dnt_plus_mbrvip { get; set; }
        public string tare_value { get; set; }
        public int locid { get; set; }
        public int mdo_id { get; set; }
        public string ex_fm_reward { get; set; }
        //public string dnt_plus_reward { get; set; }
        public string subcharge_no { get; set; }

        public override string CreateInsertSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("INSERT INTO dbo.t_extra_prodtable ("+
                                        "p_code, lost_pct, good_for_day, part_no, child_fl, parent_code, ref_cost, mo_no, tax_grp, no_vip_dnt" +
                                        ",legal_age,wic_fl,wic_prc,rv_item,final_sale,case_qty,expected_gpm,actual_gpm,cvv_fl" +
                                        ",misc_fl,service_fl,location_code,tare_value,locid,ex_fm_reward,subcharge_no"
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append("," + DBValueConvert.ToDBNumber(locid));
                    sb.Append(")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA.t_extra_prodtable (" + 
                                        "p_code, lost_pct, good_for_day, part_no, child_fl, parent_code, ref_cost, mo_no, tax_grp, no_vip_dnt" +
                                        ",legal_age,wic_fl,wic_prc,rv_item,final_sale,case_qty,expected_gpm,actual_gpm,cvv_fl" +
                                        ",misc_fl,service_fl,location_code,tare_value,ex_fm_reward,subcharge_no" +
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
                /*"p_code",*/
                "lost_pct","good_for_day","part_no","child_fl","parent_code","ref_cost","mo_no","tax_grp","no_vip_dnt",
                "legal_age","wic_fl","wic_prc","rv_item","final_sale","case_qty","expected_gpm","actual_gpm","cvv_fl",
                "misc_fl","service_fl","location_code","tare_value","ex_fm_reward","subcharge_no"
                  });

          
        }

        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("DELETE dbo.t_extra_prodtable ");
                    sb.Append(" WHERE p_code = " + DBValueConvert.ToDBString(p_code));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));

                    break;
                case SYBASE:
                    sb.Append("DELETE DBA.t_extra_prodtable ");
                    sb.Append(",mdo_id = " + DBValueConvert.ToDBNumber(mdo_id));
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

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo.t_extra_prodtable SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE p_code = " + DBValueConvert.ToDBString(p_code));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA.t_extra_prodtable SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE p_code = " + DBValueConvert.ToDBString(p_code));
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public string GetInsertValueString()
        {

            string valueStr = "";

            valueStr +=  DBValueConvert.ToDBString(p_code) + ",";
            valueStr +=  DBValueConvert.ToDBNumber(lost_pct) + ",";
            valueStr +=  DBValueConvert.ToDBNumber(good_for_day) + ",";
            valueStr +=  DBValueConvert.ToDBString(part_no) + ",";
            valueStr +=  DBValueConvert.ToDBString(child_fl) + ",";
            valueStr +=  DBValueConvert.ToDBString(parent_code) + ",";
            valueStr +=  DBValueConvert.ToDBNumber(ref_cost) + ",";
            valueStr +=  DBValueConvert.ToDBString(mo_no) + ",";
            valueStr +=  DBValueConvert.ToDBNumber(tax_grp) + ",";
            valueStr +=  DBValueConvert.ToDBString(no_vip_dnt) + ",";
            valueStr +=  DBValueConvert.ToDBNumber(legal_age) + ",";
            valueStr +=  DBValueConvert.ToDBString(wic_fl) + ",";
            valueStr +=  DBValueConvert.ToDBNumber(wic_prc) + ",";
            valueStr +=  DBValueConvert.ToDBString(rv_item) + ",";
            valueStr +=  DBValueConvert.ToDBString(final_sale) + ",";
            valueStr +=  DBValueConvert.ToDBNumber(case_qty) + ",";
            valueStr +=  DBValueConvert.ToDBNumber(expected_gpm) + ",";
            valueStr +=  DBValueConvert.ToDBNumber(actual_gpm) + ",";
            valueStr +=  DBValueConvert.ToDBString(cvv_fl) + ",";
            valueStr +=  DBValueConvert.ToDBString(misc_fl) + ",";
            valueStr +=  DBValueConvert.ToDBString(service_fl) + ",";
            valueStr +=  DBValueConvert.ToDBString(location_code) + ",";
            //valueStr +=  DBValueConvert.ToDBString(dnt_plus_vip) + ",";
            //valueStr +=  DBValueConvert.ToDBString(dnt_plus_mbrvip) + ",";
            valueStr +=  DBValueConvert.ToDBNumber(tare_value) + ",";
            valueStr +=  DBValueConvert.ToDBString(ex_fm_reward) + ",";
            valueStr += DBValueConvert.ToDBNumber(subcharge_no);
            //valueStr +=  DBValueConvert.ToDBString(dnt_plus_reward); 

            return valueStr;
        }

        public string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";

            if (colnums.Contains("p_code")) valueStr += "p_code = " + DBValueConvert.ToDBString(p_code) + ",";
            if (colnums.Contains("lost_pct")) valueStr += "lost_pct = " + DBValueConvert.ToDBNumber(lost_pct) + ",";
            if (colnums.Contains("good_for_day")) valueStr += "good_for_day = " + DBValueConvert.ToDBNumber(good_for_day) + ",";
            if (colnums.Contains("part_no")) valueStr += "part_no = " + DBValueConvert.ToDBString(part_no) + ",";
            if (colnums.Contains("child_fl")) valueStr += "child_fl = " + DBValueConvert.ToDBString(child_fl) + ",";
            if (colnums.Contains("parent_code")) valueStr += "parent_code = " + DBValueConvert.ToDBString(parent_code) + ",";
            if (colnums.Contains("ref_cost")) valueStr += "ref_cost = " + DBValueConvert.ToDBNumber(ref_cost) + ",";
            if (colnums.Contains("mo_no")) valueStr += "mo_no = " + DBValueConvert.ToDBString(mo_no) + ",";
            if (colnums.Contains("tax_grp")) valueStr += "tax_grp = " + DBValueConvert.ToDBNumber(tax_grp) + ",";
            if (colnums.Contains("no_vip_dnt")) valueStr += "no_vip_dnt = " + DBValueConvert.ToDBString(no_vip_dnt) + ",";
            if (colnums.Contains("legal_age")) valueStr += "legal_age = " + DBValueConvert.ToDBNumber(legal_age) + ",";
            if (colnums.Contains("wic_fl")) valueStr += "wic_fl = " + DBValueConvert.ToDBString(wic_fl) + ",";
            if (colnums.Contains("wic_prc")) valueStr += "wic_prc = " + DBValueConvert.ToDBNumber(wic_prc) + ",";
            if (colnums.Contains("rv_item")) valueStr += "rv_item = " + DBValueConvert.ToDBString(rv_item) + ",";
            if (colnums.Contains("final_sale")) valueStr += "final_sale = " + DBValueConvert.ToDBString(final_sale) + ",";
            if (colnums.Contains("case_qty")) valueStr += "case_qty = " + DBValueConvert.ToDBNumber(case_qty) + ",";
            if (colnums.Contains("expected_gpm")) valueStr += "expected_gpm = " + DBValueConvert.ToDBNumber(expected_gpm) + ",";
            if (colnums.Contains("actual_gpm")) valueStr += "actual_gpm = " + DBValueConvert.ToDBNumber(actual_gpm) + ",";
            if (colnums.Contains("cvv_fl")) valueStr += "cvv_fl = " + DBValueConvert.ToDBString(cvv_fl) + ",";
            if (colnums.Contains("misc_fl")) valueStr += "misc_fl =" + DBValueConvert.ToDBString(misc_fl) + ",";
            if (colnums.Contains("service_fl")) valueStr += "service_fl = " + DBValueConvert.ToDBString(service_fl) + ",";
            if (colnums.Contains("location_code")) valueStr += "location_code = " + DBValueConvert.ToDBString(location_code) + ",";
            //if (colnums.Contains("dnt_plus_vip")) valueStr += "dnt_plus_vip = " + DBValueConvert.ToDBString(dnt_plus_vip) + ",";
            //if (colnums.Contains("dnt_plus_mbrvip")) valueStr += "dnt_plus_mbrvip = " + DBValueConvert.ToDBString(dnt_plus_mbrvip) + ",";
            if (colnums.Contains("tare_value")) valueStr += "tare_value = " + DBValueConvert.ToDBNumber(tare_value) + ",";
            if (colnums.Contains("ex_fm_reward")) valueStr += "ex_fm_reward = " + DBValueConvert.ToDBString(ex_fm_reward) + ",";
            //if (colnums.Contains("dnt_plus_reward")) valueStr += "dnt_plus_reward =" + DBValueConvert.ToDBString(dnt_plus_reward) + ",";
            if (colnums.Contains("subcharge_no")) valueStr += "subcharge_no = " + DBValueConvert.ToDBNumber(subcharge_no) + ",";

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

                    //sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo.t_extra_prodtable " + where + " )");
                    //sb.Append(" BEGIN ");
                    //sb.Append(CreateInsertSQL());
                    //sb.Append(" END ");
                    //sb.Append(" ELSE BEGIN ");
                    //sb.Append(CreateUpdateSQL());
                    //sb.Append(" END ");

                    //特殊需求，如果Loacl上存extra的更新数据到cloud，但是cloud没有这个Item数据的时候就插入一个Log
                    MessageLog log = new MessageLog(_type);

                    log.cl_msg = " Item not found. (p_code: "+p_code+")";
                    log.cl_create_time = DateTime.Now;
                    log.cl_execute_time = DateTime.Now;
                    log.cl_origin_id = mdo_id; 
                    log.cl_store_id = locid;

                    sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo.prodtable " + where + " )");
                    sb.Append(" BEGIN ");
                    sb.Append( log.CreateInsertSQL());
                    sb.Append(" END ");
                    sb.Append(" ELSE BEGIN ");
                    sb.Append(CreateUpdateSQL());
                    sb.Append(" END ");

                    break;
                case SYBASE:
                    //sb.Append("INSERT INTO DBA.prodtable (barcode,p_code,proname,cpronam,p1,p2,p3,p4,dep,food_stmp" +
                    //                   ",sale_tax,amt,order_limit,amt_type,qty_in_box,ref_key,unit_dsc,unit_pk_ct,item_dsc" +
                    //                    ",item_pk_ct,item_typ_fl,item_dp_fl,vdr_p_code,p5,p6,p7,p8,amt2,amt3,amt4,pkg_dsc,cat_no" +
                    //                    ",item_cost,pkg_ct,item_gw,crt_time,pkg_3_dim,item_fob,hot_key,def_loc,item_brand,item_origin" +
                    //                    ",smart_link,member_fl,sp_plvl_id,hdby_id,for_sale,item_no,pk_cd,base_ct,active_fl,in_whole,asset_acct" +
                    //                    ",income_acct,cogs_acct,item_type,buy_in,mdo_id"
                    //                 + ") VALUES (");
                    //sb.Append(GetInsertValueString());
                    //sb.Append("," + DBValueConvert.ToDBString(mdo_id)); // mdo_id
                    //sb.Append(")");

                    throw new NotImplementedException();

                    break;
                default:
                    break;
            }

            return sb.ToString();
        }
    }
}