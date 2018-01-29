using ServiceModel.Base;
using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;

namespace ServiceModel.MoleQPos
{
    /// <summary>
    /// NProdtable 的摘要说明
    /// </summary>
    public class prodtable : DBModel
    {
        public prodtable(string type) : base(type)
        {
          
           

        }

        public string barcode { get; set; }
        public string p_code { get; set; }
        public string proname { get; set; }
        public string cpronam { get; set; }
        public string p1 { get; set; }
        public string p2 { get; set; }
        public string p3 { get; set; }
        public string p4 { get; set; }
        public string dep { get; set; }
        public string food_stmp { get; set; }
        public string sale_tax { get; set; }
        public string amt { get; set; }
        public string order_limit { get; set; }
        public string amt_type { get; set; }
        public string qty_in_box { get; set; }
        public string ref_key { get; set; }
        public string unit_dsc { get; set; }
        public string unit_pk_ct { get; set; }
        public string item_dsc { get; set; }
        public string item_pk_ct { get; set; }
        public string item_typ_fl { get; set; }
        public string item_dp_fl { get; set; }
        public string vdr_p_code { get; set; }
        public string p5 { get; set; }
        public string p6 { get; set; }
        public string p7 { get; set; }
        public string p8 { get; set; }
        public string amt2 { get; set; }
        public string amt3 { get; set; }
        public string amt4 { get; set; }
        public string pkg_dsc { get; set; }
        public string cat_no { get; set; }
        public string item_cost { get; set; }
        public string pkg_ct { get; set; }
        public string item_gw { get; set; }
        public string crt_time { get; set; }
        public string pkg_3_dim { get; set; }
        public string item_fob { get; set; }
        public string hot_key { get; set; }
        public string def_loc { get; set; }
        public string item_brand { get; set; }
        public string item_origin { get; set; }
        public string smart_link { get; set; }
        public string member_fl { get; set; }
        public string sp_plvl_id { get; set; }
        public string hdby_id { get; set; }
        public string for_sale { get; set; }
        public string item_no { get; set; }
        public string pk_cd { get; set; }
        public string base_ct { get; set; }
        public string active_fl { get; set; }
        public string in_whole { get; set; }
        public string asset_acct { get; set; }
        public string income_acct { get; set; }
        public string cogs_acct { get; set; }
        public string item_type { get; set; }
        public string buy_in { get; set; }
        public int locid { get; set; }
        public int mdo_id { get; set; }
        public string create_time { get; set; }

        public string create_id { get; set; }

        DbHelperOdbc odbcConnection = null;
        public void SetConnection(DbHelperOdbc connection)
        {
            odbcConnection = connection;
        }
        public string UpdateCentralID()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("BEGIN");
            sb.Append(" declare @rowCnt int ");
            sb.Append(" declare @Result nvarchar(15) ");

            sb.Append(" SELECT @rowCnt = count(1) ");
            sb.Append(" FROM ");
            sb.Append(" ( ");
            sb.Append("     SELECT TOP 2 centralid as ID, count(1) as cnt ");
            sb.Append("     FROM dbo.prodtable ");
            sb.Append("     WHERE barcode = " + DBValueConvert.ToDBString(barcode));
            sb.Append("     AND centralid is not null ");
            sb.Append("     AND centralid <> '' ");
            sb.Append("     GROUP BY centralid ");
            sb.Append("     ORDER BY cnt desc ");
            sb.Append(" ) AS id_cnt_lst ");

            //sb.Append("WITH id_cnt_lst as ");
            //sb.Append("(");
            //sb.Append("    SELECT TOP 2 centralid as ID,count(1) as cnt ");
            //sb.Append("    FROM dbo.prodtable");
            //sb.Append("    WHERE barcode = " + DBValueConvert.ToDBString(barcode) );
            //sb.Append("    AND centralid is not null ");
            //sb.Append("    AND centralid<> '' ");
            //sb.Append("    GROUP BY centralid ");
            //sb.Append("    order by cnt desc ");
            //sb.Append(") ");

            //sb.Append("SELECT @rowCnt = count(1) ");
            //sb.Append("FROM id_cnt_lst ");

            sb.Append("    IF @rowCnt = 1 ");
            sb.Append("        BEGIN ");
            sb.Append("            SELECT  @Result = centralid ");
            sb.Append("            FROM dbo.prodtable ");
            sb.Append("            WHERE barcode = " + DBValueConvert.ToDBString(barcode) );
            sb.Append("            AND centralid is not null ");
            sb.Append("            AND centralid<> '' ");
            sb.Append("            GROUP BY centralid ");
            sb.Append("        END ");
            sb.Append("	ELSE ");
            sb.Append("        SELECT @Result = '' ");

            sb.Append("    UPDATE dbo.prodtable ");
            sb.Append("    SET centralid = @Result ");
            sb.Append("    WHERE barcode = " + DBValueConvert.ToDBString(barcode) );
            sb.Append("    AND locid = " + DBValueConvert.ToDBNumber(locid) + " ");
            sb.Append("END");

            return sb.ToString();
        }

        public override string CreateInsertSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("INSERT INTO dbo.prodtable (barcode,p_code,proname,cpronam,p1,p2,p3,p4,dep,food_stmp" +
                                       ",sale_tax,amt,order_limit,amt_type,qty_in_box,ref_key,unit_dsc,unit_pk_ct,item_dsc" +
                                        ",item_pk_ct,item_typ_fl,item_dp_fl,vdr_p_code,p5,p6,p7,p8,amt2,amt3,amt4,pkg_dsc,cat_no" +
                                        ",item_cost,pkg_ct,item_gw,crt_time,pkg_3_dim,item_fob,hot_key,def_loc,item_brand,item_origin" +
                                        ",smart_link,member_fl,sp_plvl_id,hdby_id,for_sale,item_no,pk_cd,base_ct,active_fl,in_whole,asset_acct" +
                                        ",income_acct,cogs_acct,item_type,buy_in,create_time,create_id,locid,centralid"
                                     + ") VALUES (");
                    sb.Append(GetInsertValueString());
                    sb.Append("," + DBValueConvert.ToDBNumber(locid)); // locid

                    // v 1.0.0.3
                    // sb.Append(",null"); // central id - new item is null or empty

                    // v 1.0.0.4
                    string centralid = null;
                    // Plu weight and plu fixed leave central id as blank.  All other set central is as barcode by default.
                    if (IsPluWeight() || IsPluFixed())
                    {
                        centralid = "";
                    }
                    else
                    {
                        centralid = barcode;
                       
                    }
                    sb.Append("," + DBValueConvert.ToDBString(centralid));

                    sb.Append(")");

                    break;
                case SYBASE:
                    sb.Append("INSERT INTO DBA.prodtable (barcode,p_code,proname,cpronam,p1,p2,p3,p4,dep,food_stmp" +
                                       ",sale_tax,amt,order_limit,amt_type,qty_in_box,ref_key,unit_dsc,unit_pk_ct,item_dsc" +
                                        ",item_pk_ct,item_typ_fl,item_dp_fl,vdr_p_code,p5,p6,p7,p8,amt2,amt3,amt4,pkg_dsc,cat_no" +
                                        ",item_cost,pkg_ct,item_gw,crt_time,pkg_3_dim,item_fob,hot_key,def_loc,item_brand,item_origin" +
                                        ",smart_link,member_fl,sp_plvl_id,hdby_id,for_sale,item_no,pk_cd,base_ct,active_fl,in_whole,asset_acct" +
                                        ",income_acct,cogs_acct,item_type,buy_in,create_time,create_id,mdo_id"
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
                "barcode", "p_code" ,"proname" ,"cpronam", "p1" ,
                "p2",
                 "p3" ,"p4","dep" ,"food_stmp","sale_tax" ,
                "amt" ,
                 "order_limit" ,"amt_type","qty_in_box","ref_key" ,"unit_dsc",
                 "unit_pk_ct","item_dsc" ,"item_pk_ct","item_typ_fl","item_dp_fl" ,
                 "vdr_p_code" ,"p5","p6" ,"p7","p8",
                 "amt2" ,"amt3" , "amt4","pkg_dsc" ,"cat_no",
                 "item_cost" ,"pkg_ct" , "item_gw" ,"crt_time" ,"pkg_3_dim" ,
                 "item_fob","hot_key","def_loc" ,"item_brand" ,"item_origin" ,
                 "smart_link" ,"member_fl" ,"sp_plvl_id" ,"hdby_id" ,"for_sale",
                 "item_no" ,"pk_cd" ,"base_ct" ,"active_fl","in_whole" ,
                 "asset_acct" ,"income_acct" ,"cogs_acct" , "item_type" , "buy_in"
                  });

        }

        public override string CreateDeleteSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("DELETE dbo.prodtable ");
                    sb.Append(" WHERE barcode = " + DBValueConvert.ToDBString(barcode));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("DELETE DBA.prodtable ");
                    sb.Append(" WHERE barcode = " + DBValueConvert.ToDBString(barcode));
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }
        
        public override void Parse(DataRow dataRow)
        {

            //下列是从wholesale.prodtable的所有变量。
            barcode     = Convert.IsDBNull(dataRow["barcode"])? null : dataRow["barcode"].ToString(); //VARCHAR(15) NOT NULL;
            p_code      = Convert.IsDBNull(dataRow["p_code"])? null : dataRow["p_code"].ToString();    //VARCHAR(15) NOT NULL;
            proname     = Convert.IsDBNull(dataRow["proname"])? null : dataRow["proname"].ToString();    //CHAR(50) NULL;
            cpronam     = Convert.IsDBNull(dataRow["cpronam"])? null : dataRow["cpronam"].ToString();    //VARCHAR(50) NULL;
            p1          = Convert.IsDBNull(dataRow["p1"])? null : dataRow["p1"].ToString();    //NUMERIC(8, 2) NULL;
            p2          = Convert.IsDBNull(dataRow["p2"])? null : dataRow["p2"].ToString();    //NUMERIC(8, 2) NULL;
            p3          = Convert.IsDBNull(dataRow["p3"])? null : dataRow["p3"].ToString();    //NUMERIC(8, 2) NULL;
            p4          = Convert.IsDBNull(dataRow["p4"])? null : dataRow["p4"].ToString();    //NUMERIC(8, 2) NULL;
            dep         = Convert.IsDBNull(dataRow["dep"])? null : dataRow["dep"].ToString();    //NUMERIC(4, 0) NULL;
            //food_stmp   = Convert.IsDBNull(dataRow["food_stmp"])? null : dataRow["food_stmp"].ToString();    //NUMERIC(2, 0) NULL;
            sale_tax    = Convert.IsDBNull(dataRow["sale_tax"])? null : dataRow["sale_tax"].ToString();    //CHAR(1) NULL;
            //amt         = Convert.IsDBNull(dataRow["amt"])? null : dataRow["amt"].ToString();    //NUMERIC(13, 4) NULL;
            order_limit  = Convert.IsDBNull(dataRow["order_limit"])? null : dataRow["order_limit"].ToString();    //NUMERIC(10, 2) NULL;
            amt_type    = Convert.IsDBNull(dataRow["amt_type"])? null : dataRow["amt_type"].ToString();    //CHAR(1) NULL;
            qty_in_box  = Convert.IsDBNull(dataRow["qty_in_box"])? null : dataRow["qty_in_box"].ToString();    //NUMERIC(5, 0) NULL;
            ref_key     = Convert.IsDBNull(dataRow["ref_key"])? null : dataRow["ref_key"].ToString();    //VARCHAR(15) NULL;
            //unit_dsc    = Convert.IsDBNull(dataRow["unit_dsc"])? null : dataRow["unit_dsc"].ToString();    //VARCHAR(30) NULL;
            //unit_pk_ct  = Convert.IsDBNull(dataRow["unit_pk_ct"])? null : dataRow["unit_pk_ct"].ToString();    //NUMERIC(10, 4) NULL;
            item_dsc    = Convert.IsDBNull(dataRow["item_dsc"])? null : dataRow["item_dsc"].ToString();    //VARCHAR(30) NULL;
            item_pk_ct  = Convert.IsDBNull(dataRow["item_pk_ct"])? null : dataRow["item_pk_ct"].ToString();    //NUMERIC(10, 2) NULL;
            //item_typ_fl  = Convert.IsDBNull(dataRow["item_typ_fl"])? null : dataRow["item_typ_fl"].ToString();    //CHAR(1) NULL;
            //item_dp_fl  = Convert.IsDBNull(dataRow["item_dp_fl"])? null : dataRow["item_dp_fl"].ToString();    //CHAR(1) NULL;
            vdr_p_code  = Convert.IsDBNull(dataRow["vdr_p_code"])? null : dataRow["vdr_p_code"].ToString();    //VARCHAR(12) NULL;
            p5          = Convert.IsDBNull(dataRow["p5"])? null : dataRow["p5"].ToString();    //NUMERIC(8, 2) NULL;
            //p6          = Convert.IsDBNull(dataRow["p6"])? null : dataRow["p6"].ToString();    //NUMERIC(8, 2) NULL;
            //p7          = Convert.IsDBNull(dataRow["p7"])? null : dataRow["p7"].ToString();    //NUMERIC(8, 2) NULL;
            //p8          = Convert.IsDBNull(dataRow["p8"])? null : dataRow["p8"].ToString();    //NUMERIC(8, 2) NULL;
            //amt2        = Convert.IsDBNull(dataRow["amt2"])? null : dataRow["amt2"].ToString();    //NUMERIC(13, 4) NULL;
            //amt3        = Convert.IsDBNull(dataRow["amt3"])? null : dataRow["amt3"].ToString();    //NUMERIC(13, 4) NULL;
            //amt4        = Convert.IsDBNull(dataRow["amt4"])? null : dataRow["amt4"].ToString();    //NUMERIC(13, 4) NULL;
            pkg_dsc     = Convert.IsDBNull(dataRow["pkg_dsc"])? null : dataRow["pkg_dsc"].ToString();    //VARCHAR(30) NULL;
            cat_no      = Convert.IsDBNull(dataRow["cat_no"])? null : dataRow["cat_no"].ToString();    //INTEGER NULL; // Kevin说不需要搬这个field
            item_cost  = Convert.IsDBNull(dataRow["item_cost"])? null : dataRow["item_cost"].ToString();    //NUMERIC(8, 2) NULL;
            pkg_ct      = Convert.IsDBNull(dataRow["pkg_ct"])? null : dataRow["pkg_ct"].ToString();    //NUMERIC(10, 2) NULL;
            item_gw     = Convert.IsDBNull(dataRow["item_gw"])? null : dataRow["item_gw"].ToString();    //NUMERIC(8, 2) NULL;
            crt_time    = Convert.IsDBNull(dataRow["crt_time"])? null : dataRow["crt_time"].ToString();    //TIMESTAMP NULL;
            pkg_3_dim  = Convert.IsDBNull(dataRow["pkg_3_dim"])? null : dataRow["pkg_3_dim"].ToString();    //INTEGER NULL;
            item_fob  = Convert.IsDBNull(dataRow["item_fob"])? null : dataRow["item_fob"].ToString();    //NUMERIC(8, 2) NULL;
            hot_key  = Convert.IsDBNull(dataRow["hot_key"])? null : dataRow["hot_key"].ToString();    //CHAR(4) NULL;
            def_loc  = Convert.IsDBNull(dataRow["def_loc"])? null : dataRow["def_loc"].ToString();    //INTEGER NOT NULL;
            item_brand  = Convert.IsDBNull(dataRow["item_brand"])? null : dataRow["item_brand"].ToString();    //INTEGER NULL;
            item_origin  = Convert.IsDBNull(dataRow["item_origin"])? null : dataRow["item_origin"].ToString();    //INTEGER NULL;
            smart_link  = Convert.IsDBNull(dataRow["smart_link"])? null : dataRow["smart_link"].ToString();    //VARCHAR(15) NULL;
            //member_fl  = Convert.IsDBNull(dataRow["member_fl"])? null : dataRow["member_fl"].ToString();    //CHAR(1) NULL;
            //sp_plvl_id  = Convert.IsDBNull(dataRow["sp_plvl_id"])? null : dataRow["sp_plvl_id"].ToString();    //INTEGER NULL;
            hdby_id  = Convert.IsDBNull(dataRow["hdby_id"])? null : dataRow["hdby_id"].ToString();    //INTEGER NULL;
            //for_sale  = Convert.IsDBNull(dataRow["for_sale"])? null : dataRow["for_sale"].ToString();    //CHAR(1) NOT NULL;
            item_no  = Convert.IsDBNull(dataRow["item_no"])? null : dataRow["item_no"].ToString();    //VARCHAR(15) NOT NULL;
            //pk_cd  = Convert.IsDBNull(dataRow["pk_cd"])? null : dataRow["pk_cd"].ToString();    //VARCHAR(2) NULL;
            base_ct  = Convert.IsDBNull(dataRow["base_ct"])? null : dataRow["base_ct"].ToString();    //NUMERIC(13, 4) NOT NULL;
            active_fl  = Convert.IsDBNull(dataRow["active_fl"])? null : dataRow["active_fl"].ToString();    //CHAR(1) NOT NULL;
            //in_whole  = Convert.IsDBNull(dataRow["in_whole"])? null : dataRow["in_whole"].ToString();    //CHAR(1) NOT NULL;
            //asset_acct  = Convert.IsDBNull(dataRow["asset_acct"])? null : dataRow["asset_acct"].ToString();    //INTEGER NULL;
            //income_acct  = Convert.IsDBNull(dataRow["income_acct"])? null : dataRow["income_acct"].ToString();    //INTEGER NULL;
            //cogs_acct  = Convert.IsDBNull(dataRow["cogs_acct"])? null : dataRow["cogs_acct"].ToString();    //INTEGER NULL;
            item_type  = Convert.IsDBNull(dataRow["item_type"])? null : dataRow["item_type"].ToString();    //CHAR(1) NOT NULL;
            //buy_in  = Convert.IsDBNull(dataRow["buy_in"])? null : dataRow["buy_in"].ToString();    //CHAR(1) NOT NULL;
            //mdo_id  = Convert.IsDBNull(dataRow["mdo_id"])? 0 : Convert.ToInt32( dataRow["mdo_id"]);    //INTEGER NULL;
            //create_time  = Convert.IsDBNull(dataRow["create_time"])? null : dataRow["create_time"].ToString();    //"datetime  = Convert.IsDBNull(dataRow[""])? null : dataRow[""].ToString();    //NULL;
            //create_id  = Convert.IsDBNull(dataRow["create_id"])? null : dataRow["create_id"].ToString();    //VARCHAR(20) NULL;

            //p4 = Convert.IsDBNull(dataRow["min_price"]) ? null : dataRow["min_price"].ToString();
            //特殊处理 如果wholesale.p5为null ,market.p4为0
            if (p5 == null)
            {
                p4 = "0";
            }
            else
            {
                p4 = p5;
            }

            if (p1 == null) p1 = "0";
            if (p2 == null) p2 = "0";
            if (p3 == null) p3 = "0";

            //market.prodtable默认参数在此赋值
            food_stmp = "0";
            ref_key = null;
            item_dp_fl = "N";
            hot_key = null;
            smart_link = null;
            member_fl = "Y";
            in_whole = "Y";

            item_typ_fl = "R";
            income_acct = "6";
            for_sale = "Y";
            buy_in = "Y";
            asset_acct = "3";
            cogs_acct = "7";
            cat_no = null;

        }

        public override string CreateUpdateSQL(string[] updateColumn)
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:
                    sb.Append("UPDATE dbo.prodtable SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(" WHERE barcode = " + DBValueConvert.ToDBString(barcode));
                    sb.Append(" AND locid = " + DBValueConvert.ToDBNumber(locid));
                    break;
                case SYBASE:
                    sb.Append("UPDATE DBA.prodtable SET ");
                    sb.Append(GetUpdateSetValueString(updateColumn));
                    sb.Append(",mdo_id = " + DBValueConvert.ToDBNumber(mdo_id));
                    sb.Append(" WHERE barcode = " + DBValueConvert.ToDBString(barcode));
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public string GetInsertValueString()
        {

            if (for_sale == null)
                for_sale = "";

            if (in_whole == null)
                in_whole = "";

            if (buy_in == null)
                buy_in = "";
            
            string valueStr = "";
            valueStr += DBValueConvert.ToDBString(barcode) + ",";
            // valueStr += DBValueConvert.ToSQLvalue(p_code) + ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBString(p_code) : DBValueConvert.ToDBString(p_code); valueStr += ",";

            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(proname) : DBValueConvert.ToDBString(proname); valueStr += ",";
            valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(cpronam) : DBValueConvert.ToDBString(cpronam); valueStr += ",";
            valueStr += DBValueConvert.ToDBNumber(p1) + ",";
            valueStr += DBValueConvert.ToDBNumber(p2) + ",";
            valueStr += DBValueConvert.ToDBNumber(p3) + ",";
            valueStr += DBValueConvert.ToDBNumber(p4) + ",";
            valueStr += DBValueConvert.ToDBNumber(dep) + ",";
            valueStr += DBValueConvert.ToDBNumber(food_stmp) + ",";
            valueStr += DBValueConvert.ToDBString(sale_tax) + ",";
            valueStr += DBValueConvert.ToDBNumber(amt) + ",";
            valueStr += DBValueConvert.ToDBNumber(order_limit) + ",";
            valueStr += DBValueConvert.ToDBString(amt_type) + ",";
            valueStr += DBValueConvert.ToDBNumber(qty_in_box) + ",";
            valueStr += DBValueConvert.ToDBString(ref_key) + ",";
            valueStr += DBValueConvert.ToDBString(unit_dsc) + ",";
            valueStr += DBValueConvert.ToDBNumber(unit_pk_ct) + ",";
            valueStr += DBValueConvert.ToDBString(item_dsc) + ",";
            valueStr += DBValueConvert.ToDBNumber(item_pk_ct) + ",";
            valueStr += DBValueConvert.ToDBString(item_typ_fl) + ",";
            valueStr += DBValueConvert.ToDBString(item_dp_fl) + ",";
            valueStr += DBValueConvert.ToDBString(vdr_p_code) + ",";
            valueStr += DBValueConvert.ToDBNumber(p5) + ",";
            valueStr += DBValueConvert.ToDBNumber(p6) + ",";
            valueStr += DBValueConvert.ToDBNumber(p7) + ",";
            valueStr += DBValueConvert.ToDBNumber(p8) + ",";
            valueStr += DBValueConvert.ToDBNumber(amt2) + ",";
            valueStr += DBValueConvert.ToDBNumber(amt3) + ",";
            valueStr += DBValueConvert.ToDBNumber(amt4) + ",";
            valueStr += DBValueConvert.ToDBString(pkg_dsc) + ",";
            valueStr += DBValueConvert.ToDBNumber(cat_no) + ",";
            valueStr += DBValueConvert.ToDBNumber(item_cost) + ",";
            valueStr += DBValueConvert.ToDBNumber(pkg_ct) + ",";
            valueStr += DBValueConvert.ToDBNumber(item_gw) + ",";
            valueStr += DBValueConvert.ToDBString( DBValueConvert.ToDBDateTime(crt_time)) + ",";
            valueStr += DBValueConvert.ToDBNumber(pkg_3_dim) + ",";
            valueStr += DBValueConvert.ToDBNumber(item_fob) + ",";
            valueStr += DBValueConvert.ToDBString(hot_key) + ","; 
            valueStr += DBValueConvert.ToDBNumber(def_loc) + ",";
            valueStr += DBValueConvert.ToDBNumber(item_brand) + ",";
            valueStr += DBValueConvert.ToDBNumber(item_origin) + ",";
            valueStr += DBValueConvert.ToDBString(smart_link) + ",";
            valueStr += DBValueConvert.ToDBString(member_fl) + ",";
            valueStr += DBValueConvert.ToDBNumber(sp_plvl_id) + ",";
            valueStr += DBValueConvert.ToDBNumber(hdby_id) + ",";
            valueStr += DBValueConvert.ToDBString(for_sale) + ",";
            valueStr += DBValueConvert.ToDBString(item_no) + ",";
            valueStr += DBValueConvert.ToDBString(pk_cd) + ",";
            valueStr += DBValueConvert.ToDBNumber(base_ct) + ",";
            valueStr += DBValueConvert.ToDBString(active_fl) + ",";
            valueStr += DBValueConvert.ToDBString(in_whole) + ",";
            valueStr += DBValueConvert.ToDBNumber(asset_acct) + ",";
            valueStr += DBValueConvert.ToDBNumber(income_acct) + ",";
            valueStr += DBValueConvert.ToDBNumber(cogs_acct) + ",";
            valueStr += DBValueConvert.ToDBString(item_type) + ",";
            valueStr += DBValueConvert.ToDBString(buy_in) + ",";
            valueStr += DBValueConvert.ToDBString(DBValueConvert.ToDBDateTime(create_time)) + ",";
            valueStr += DBValueConvert.ToDBString(create_id);
            
            return valueStr;
        }

        private bool IsPluFixed()
        {
            return amt_type.ToUpper() == "P" ? true : false;
        }

        private bool IsPluWeight()
        {
            return amt_type.ToUpper() == "W"? true: false;
        }

        public string GetUpdateSetValueString(string[] cols)
        {
            List<string> colnums = new List<string>(cols);

            string valueStr = "";
            if (colnums.Contains("barcode")) valueStr += "barcode = " + DBValueConvert.ToDBString(barcode) + ",";
            if (colnums.Contains("p_code")) valueStr += "p_code = " + DBValueConvert.ToDBString(p_code) + ",";

            if (colnums.Contains("proname"))
            {
                valueStr += "proname = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(proname) : DBValueConvert.ToDBString(proname);
                valueStr += ",";
            }
            if (colnums.Contains("cpronam"))
            {
                valueStr += "cpronam = ";
                valueStr += (_type == SQL_SERVER) ? DBValueConvert.ToDBVarChar(cpronam) : DBValueConvert.ToDBString(cpronam);
                valueStr += ",";
            }

            if (colnums.Contains("p1")) valueStr += "p1 = " + DBValueConvert.ToDBNumber(p1) + ",";
            if (colnums.Contains("p2")) valueStr += "p2 = " + DBValueConvert.ToDBNumber(p2) + ",";
            if (colnums.Contains("p3")) valueStr += "p3 = " + DBValueConvert.ToDBNumber(p3) + ",";
            if (colnums.Contains("p4")) valueStr += "p4 = " + DBValueConvert.ToDBNumber(p4) + ",";
            if (colnums.Contains("dep")) valueStr += "dep = " + DBValueConvert.ToDBNumber(dep) + ",";
            if (colnums.Contains("food_stmp")) valueStr += "food_stmp = " + DBValueConvert.ToDBNumber(food_stmp) + ",";
            if (colnums.Contains("sale_tax")) valueStr += "sale_tax = " + DBValueConvert.ToDBString(sale_tax) + ",";
            if (colnums.Contains("amt")) valueStr += "amt = " + DBValueConvert.ToDBNumber(amt) + ",";
            if (colnums.Contains("order_limit")) valueStr += "order_limit = " + DBValueConvert.ToDBNumber(order_limit) + ",";
            if (colnums.Contains("amt_type")) valueStr += "amt_type = " + DBValueConvert.ToDBString(amt_type) + ",";
            if (colnums.Contains("qty_in_box")) valueStr += "qty_in_box = " + DBValueConvert.ToDBNumber(qty_in_box) + ",";
            if (colnums.Contains("ref_key")) valueStr += "ref_key = " + DBValueConvert.ToDBString(ref_key) + ",";
            if (colnums.Contains("unit_dsc")) valueStr += "unit_dsc = " + DBValueConvert.ToDBString(unit_dsc) + ",";
            if (colnums.Contains("unit_pk_ct")) valueStr += "unit_pk_ct = " + DBValueConvert.ToDBNumber(unit_pk_ct) + ",";
            if (colnums.Contains("item_dsc")) valueStr += "item_dsc = " + DBValueConvert.ToDBString(item_dsc) + ",";
            if (colnums.Contains("item_pk_ct")) valueStr += "item_pk_ct =" + DBValueConvert.ToDBNumber(item_pk_ct) + ",";
            if (colnums.Contains("item_typ_fl")) valueStr += "item_typ_fl = " + DBValueConvert.ToDBString(item_typ_fl) + ",";
            if (colnums.Contains("item_dp_fl")) valueStr += "item_dp_fl = " + DBValueConvert.ToDBString(item_dp_fl) + ",";
            if (colnums.Contains("vdr_p_code")) valueStr += "vdr_p_code = " + DBValueConvert.ToDBString(vdr_p_code) + ",";
            if (colnums.Contains("p5")) valueStr += "p5 = " + DBValueConvert.ToDBNumber(p5) + ",";
            if (colnums.Contains("p6")) valueStr += "p6 = " + DBValueConvert.ToDBNumber(p6) + ",";
            if (colnums.Contains("p7")) valueStr += "p7 = " + DBValueConvert.ToDBNumber(p7) + ",";
            if (colnums.Contains("p8")) valueStr += "p8 = " + DBValueConvert.ToDBNumber(p8) + ",";
            if (colnums.Contains("amt2")) valueStr += "amt2 = " + DBValueConvert.ToDBNumber(amt2) + ",";
            if (colnums.Contains("amt3")) valueStr += "amt3 = " + DBValueConvert.ToDBNumber(amt3) + ",";
            if (colnums.Contains("amt4")) valueStr += "amt4 = " + DBValueConvert.ToDBNumber(amt4) + ",";
            if (colnums.Contains("pkg_dsc")) valueStr += "pkg_dsc = " + DBValueConvert.ToDBString(pkg_dsc) + ",";
            if (colnums.Contains("cat_no")) valueStr += "cat_no = " + DBValueConvert.ToDBNumber(cat_no) + ",";
            if (colnums.Contains("item_cost")) valueStr += "item_cost = " + DBValueConvert.ToDBNumber(item_cost) + ",";
            if (colnums.Contains("pkg_ct")) valueStr += "pkg_ct = " + DBValueConvert.ToDBNumber(pkg_ct) + ",";
            if (colnums.Contains("item_gw")) valueStr += "item_gw = " + DBValueConvert.ToDBNumber(item_gw) + ",";
            if (colnums.Contains("crt_time")) valueStr += "crt_time = " + DBValueConvert.ToDBString( DBValueConvert.ToDBDateTime(crt_time))+ ",";
            if (colnums.Contains("pkg_3_dim")) valueStr += "pkg_3_dim =" + DBValueConvert.ToDBNumber(pkg_3_dim) + ",";
            if (colnums.Contains("item_fob")) valueStr += "item_fob = " + DBValueConvert.ToDBNumber(item_fob) + ",";
            if (colnums.Contains("hot_key")) valueStr += "hot_key = " + DBValueConvert.ToDBString(hot_key) + ",";
            if (colnums.Contains("def_loc")) valueStr += "def_loc = " + DBValueConvert.ToDBNumber(def_loc) + ",";
            if (colnums.Contains("item_brand")) valueStr += "item_brand = " + DBValueConvert.ToDBNumber(item_brand) + ",";
            if (colnums.Contains("item_origin")) valueStr += "item_origin = " + DBValueConvert.ToDBNumber(item_origin) + ",";
            if (colnums.Contains("smart_link")) valueStr += "smart_link = " + DBValueConvert.ToDBString(smart_link) + ",";
            if (colnums.Contains("member_fl")) valueStr += "member_fl = " + DBValueConvert.ToDBString(member_fl) + ",";
            if (colnums.Contains("sp_plvl_id")) valueStr += "sp_plvl_id = " + DBValueConvert.ToDBNumber(sp_plvl_id) + ",";
            if (colnums.Contains("hdby_id")) valueStr += "hdby_id = " + DBValueConvert.ToDBNumber(hdby_id) + ",";
            if (colnums.Contains("for_sale")) valueStr += "for_sale = " + DBValueConvert.ToDBString(for_sale) + ",";
            if (colnums.Contains("item_no")) valueStr += "item_no = " + DBValueConvert.ToDBString(item_no) + ",";
            if (colnums.Contains("pk_cd")) valueStr += "pk_cd = " + DBValueConvert.ToDBString(pk_cd) + ",";
            if (colnums.Contains("base_ct")) valueStr += "base_ct = " + DBValueConvert.ToDBNumber(base_ct) + ",";
            if (colnums.Contains("active_fl")) valueStr += "active_fl = " + DBValueConvert.ToDBString(active_fl) + ",";
            if (colnums.Contains("in_whole")) valueStr += "in_whole = " + DBValueConvert.ToDBString(in_whole) + ",";
            if (colnums.Contains("asset_acct")) valueStr += "asset_acct = " + DBValueConvert.ToDBNumber(asset_acct) + ",";
            if (colnums.Contains("income_acct")) valueStr += "income_acct = " + DBValueConvert.ToDBNumber(income_acct) + ",";
            if (colnums.Contains("cogs_acct")) valueStr += "cogs_acct = " + DBValueConvert.ToDBNumber(cogs_acct) + ",";
            if (colnums.Contains("item_type")) valueStr += "item_type = " + DBValueConvert.ToDBString(item_type) + ",";
            if (colnums.Contains("buy_in")) valueStr += "buy_in = " + DBValueConvert.ToDBString(buy_in) + ",";
            
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
                            "barcode = " + DBValueConvert.ToDBString(barcode),
                            "locid = " + DBValueConvert.ToDBNumber(locid)
                    };
                    break;
                case SYBASE:
                    re = new string[]{
                            "barcode = " + DBValueConvert.ToDBString(barcode)
                    };
                    break;
                default:
                    break;
            }

            return re;
        }

        private decimal MaxPcode()
        {
            decimal max = Convert.ToDecimal(odbcConnection.ExecuteScalar("select Max(convert(numeric(12,0),p_code)) from dba.prodtable where isNumeric(p_code) = 1 and CHARINDEX('E',p_code) = 0 and Length(p_code) < 12"));
            max++;
            return max;
        }
      
        public override string CreateSaveSQL()
        {
            StringBuilder sb = new StringBuilder();

            switch (_type)
            {
                case SQL_SERVER:

                    string where = UniversalSQLStatementGenerator.GetWhereClause(GetWhereClause());

                    sb.Append("IF NOT EXISTS (SELECT 1 FROM dbo.prodtable " + where  +" )");
                    sb.Append(" BEGIN ");
                    sb.Append(CreateInsertSQL());
                    sb.Append(" END ");
                    sb.Append(" ELSE BEGIN ");
                    sb.Append(CreateUpdateSQL());
                    sb.Append(" END ");
                    //if (barcode.Length == 11 || barcode.Length == 13)
                    //{
                    //    sb.Append(UpdateCentralID());
                    //}

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
 