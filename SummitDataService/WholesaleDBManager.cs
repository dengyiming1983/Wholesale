using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummitDataService
{
    public class WholesaleDBManager
    {

        private void SyncProductNameAndDesc(string upc, bool hasName, string name, bool hasDesc, string desc)
        {
            StringBuilder merge_name_sql = new StringBuilder();
            merge_name_sql.Append("/* update oc_product_description -> upc: " + upc + " */");
            merge_name_sql.Append("INSERT INTO oc_product_description (product_id, language_id,name,description,tag,meta_title,meta_description,meta_keyword) ");
            merge_name_sql.Append("VALUES ");
            merge_name_sql.Append("((SELECT product_id FROM oc_product WHERE model = ?upc ), ?language_id,?name, '&lt;p&gt;&lt;br&gt;&lt;/p&gt;', '', '', '', '') ");
            merge_name_sql.Append("ON DUPLICATE KEY UPDATE ");
            merge_name_sql.Append("product_id = (SELECT product_id FROM oc_product WHERE model = ?upc ), name = ?name ");

            if (hasName)
            {
                //?upc ?name ?desc
                OdbcParameter[] parameters4Name = {
                                    new OdbcParameter("?upc",  OdbcType.VarChar),
                                    new OdbcParameter("?name", OdbcType.VarChar),
                                    new OdbcParameter("?language_id", OdbcType.Int)
                };

                parameters4Name[0].Value = upc;
                parameters4Name[1].Value = name;
                parameters4Name[2].Value = 1;

                //updateProductOtherInfoLst.Add(merge_name_sql.ToString() + " /* update name */ ", parameters4Name);

            }

            if (hasDesc)
            {

                OdbcParameter[] parameters4desc = {
                                    new OdbcParameter("?upc", OdbcType.VarChar),
                                    new OdbcParameter("?name", OdbcType.VarChar),
                                    new OdbcParameter("?language_id", OdbcType.Int)
                };
                parameters4desc[0].Value = upc;
                parameters4desc[1].Value = desc;
                parameters4desc[2].Value = 2;

                //updateProductOtherInfoLst.Add(merge_name_sql.ToString() + " /* update description */ ", parameters4desc);
            }
        }

        public string InsertProdtable(string key , string barcode , string itemNumber,string itemName , string itemDesc, 
                                    string brandID, string fob, string cost, string gw ,string priceA,string priceB,string priceC,
                                    string VendorNumber)
        {
         
            decimal minPrice = 0;
            decimal p1 = Convert.ToDecimal(priceA);
            decimal p2 = Convert.ToDecimal(priceB);
            decimal p3 = Convert.ToDecimal(priceC);

            minPrice = Math.Min(Math.Min(p1, p2), p3);

            string ref_key = "null";

            if(key != itemNumber)
            {
                ref_key = "'1'";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO DBA.prodtable (barcode,p_code,proname,cpronam,p1,p2,p3,p4,dep" +
                                      ",sale_tax,order_limit,amt_type,qty_in_box, ref_key,item_dsc" +
                                       ",item_pk_ct,vdr_p_code,p5,p1_b,p2_b,p3_b,pkg_dsc" +
                                       ",item_cost,pkg_ct,item_gw,crt_time,pkg_3_dim,item_fob,hot_key,def_loc,item_brand,item_origin" +
                                       ",smart_link,hdby_id,item_no,base_ct,active_fl" +
                                       ",item_type,min_price,p4_b,p5_b,pk_wt,pk_wt_ext"
                                    + ") VALUES (");
            sb.Append(DBValueConvert.ToDBString(barcode) +","); // barcode
            sb.Append(DBValueConvert.ToDBString(key) + ","); // p_code
            sb.Append(DBValueConvert.ToDBString(itemName) + ",");// proname
            sb.Append(DBValueConvert.ToDBString(itemDesc) + ",");// cpronam
            sb.Append(DBValueConvert.ToDBNumber(priceA) + ","); // p1
            sb.Append(DBValueConvert.ToDBNumber(priceB) + ","); // p2
            sb.Append(DBValueConvert.ToDBNumber(priceC) + ","); // p3
            sb.Append("null,null,'N',null,'S',null,"+ ref_key + ",null,null," + DBValueConvert.ToDBString(VendorNumber) + ",null,");
            sb.Append(DBValueConvert.ToDBNumber(priceA) + ","); // p1_b
            sb.Append(DBValueConvert.ToDBNumber(priceB) + ","); // p2_b
            sb.Append(DBValueConvert.ToDBNumber(priceC) + ","); // p3_b
            sb.Append("null,");
            sb.Append(DBValueConvert.ToDBNumber(cost) + ",1, null,null,1,");
            sb.Append(DBValueConvert.ToDBNumber(fob) + ",");
            sb.Append("null,1," + DBValueConvert.ToDBNumber(brandID) + ",null,"+ DBValueConvert.ToDBString(itemNumber) + ",null,"+DBValueConvert.ToDBString(key) + ",1.0000,'Y','I'," + DBValueConvert.ToDBNumber(minPrice) + ",null,null," + DBValueConvert.ToDBNumber(gw) + ",null");
            sb.Append(")");

            return sb.ToString();

        }

        public string InsertBrand(int id, string name, string dsc ,string origin, string code)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO DBA.t_brand_lst (bnd_id,bnd_name,bnd_dsc,bnd_origin,bnd_code ) VALUES ( ");
            sb.Append(DBValueConvert.ToDBNumber(id) + ",");
            sb.Append(DBValueConvert.ToDBString(name) + ",");
            sb.Append(DBValueConvert.ToDBString(dsc) + ",");
            sb.Append("null,null");
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// support sybase and sqlserver
        /// </summary>
        /// <returns></returns>
        public string CreateSaveSQL(string table ,string whereClause, string inserSql, string updateSql)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("IF NOT EXISTS (SELECT 1 FROM " + table + " " + whereClause + " )");
            sb.Append(" BEGIN ");
            sb.Append(inserSql);
            sb.Append(" END ");
            sb.Append(" ELSE BEGIN ");
            sb.Append(updateSql);
            sb.Append(" END ");
            
            return sb.ToString();
        }

        public int GetMaxBrandID()
        {
            object res = DbHelperOdbc.ExecuteScalar("SELECT ISNULL(CONVERT(int,max(bnd_id)), 0) FROM DBA.t_brand_lst");

            return Convert.ToInt32(res);

        }

        public int GetItemLotMaxID()
        {
            object res = DbHelperOdbc.ExecuteScalar("SELECT ISNULL(CONVERT(int,max(lot_no)), 0) FROM DBA.t_item_lot");

            return Convert.ToInt32(res);
        }

        public string InsertMemo(string itemNumber, string memo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO DBA.t_item_memo (im_pcode,im_memo ) VALUES ( ");
            sb.Append(DBValueConvert.ToDBString(itemNumber) + ",");
            sb.Append(DBValueConvert.ToDBString(memo));
            sb.Append(")");

            return sb.ToString();
        }

        public string UpdateMemo(string itemNumber, string memo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE DBA.t_item_memo ");
            sb.Append("SET im_memo ="+ DBValueConvert.ToDBString(memo) );
            sb.Append(" WHERE im_pcode = " + DBValueConvert.ToDBString(itemNumber));
            return sb.ToString();
        }

        public string InsertSummitMeasure(string itemNumber, string pack)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO DBA.t_summit_measure (sm_pcode,sm_mes ) VALUES ( ");
            sb.Append(DBValueConvert.ToDBString(itemNumber) + ",");
            sb.Append(DBValueConvert.ToDBString(pack));
            sb.Append(")");

            return sb.ToString();
        }

        public string InsertItemLot(int item_lot_id, int item_lot_loc_no ,string relatedItemCode, int qty ,string cost)
        {
            //lot_no,lot_date,lot_cost,lot_qty,lot_remain
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO DBA.t_item_lot (lot_no,lot_item,lot_date,lot_cost,lot_qty,lot_remain,lot_loc,lot_vdr_no,lot_hold,lot_sug_sale_dt,lot_adj_fl,lot_base_rate,lot_exp_date ) VALUES ( ");
            sb.Append(DBValueConvert.ToDBNumber(item_lot_id) + ",");
            sb.Append(DBValueConvert.ToDBString(relatedItemCode) + ",");
            sb.Append("Getdate(), ");
            sb.Append(DBValueConvert.ToDBNumber(cost) + ",");
            sb.Append(DBValueConvert.ToDBNumber(qty) + ",");        //lot_qty
            sb.Append(DBValueConvert.ToDBNumber(qty) + ",");        //lot_remain
            sb.Append(DBValueConvert.ToDBNumber(item_lot_loc_no) + ",");  //lot_loc
            sb.Append("null,'N',null,'N',1,null");
            sb.Append(")");

            return sb.ToString();

        }

        public string UpdateItemLotLoc(int item_lot_id, int item_lot_loc_no, string relatedItemCode ,int qty, string cost)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE DBA.t_item_lot ");
            sb.Append("SET lot_qty = " + DBValueConvert.ToDBNumber(qty) + ",");
            sb.Append("lot_date = Getdate(),"); 
            sb.Append("lot_cost = " + DBValueConvert.ToDBNumber(cost));
            sb.Append(" WHERE lot_item = " + DBValueConvert.ToDBString(relatedItemCode));
            sb.Append(" AND lot_no = " + DBValueConvert.ToDBNumber(item_lot_id));

            sb.Append(" ");

            sb.Append("UPDATE DBA.t_item_lot_loc ");
            sb.Append("SET ll_qty = " + DBValueConvert.ToDBNumber(qty) +",");
            sb.Append("ll_qty_trnsit = 0 ");
            sb.Append(" WHERE ll_item = " + DBValueConvert.ToDBString(relatedItemCode));
            sb.Append(" AND ll_lot_no = " + DBValueConvert.ToDBNumber(item_lot_id));
            sb.Append(" AND ll_loc_no = " + DBValueConvert.ToDBNumber(item_lot_loc_no));

            sb.Append(" ");

            return sb.ToString();

        }

        public string InsertCustomer(string number, string name,string addr, string city, string stat,
                                      string zip, string phone,string contact, string email,string balance ,
                                      string chineseName, string region,
                                      string ship_name,string ship_cname,string ship_addr,string ship_city, string ship_state ,string ship_zip,string sales_no)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO DBA.client ( ");

            sb.Append("ccode,ename,cname,addr,city,");
            sb.Append("stat,zip,tele,clt_type,cust_bill_nm,");
            sb.Append("cust_cntct,cust_alt_cntct,cust_alt_phn,cust_fax,cust_term,");
            sb.Append("cust_credit_limit,cust_taxable,cust_email,cust_resale_no,cust_active,");
            sb.Append("cust_shp_nm,cust_shp_addr,cust_shp_city,cust_shp_state,cust_shp_zip,");
            sb.Append("cust_sales_no,bill_cnm,shp_cnm,cust_unlimit_fl,ctry_nm,");
            sb.Append("cust_shp_ctry,cust_hold,cust_extra_date,cust_extra_1,cust_extra_2,");
            sb.Append("ship_via,max_overdue_dt,max_overdue_amt,cust_memo,price_grp_id,");
            sb.Append("cust_fed_no,cust_fed_state,cust_fed_expy,cust_resale_state,cust_resale_expy,");
            sb.Append("cust_lz_no,status_code,cmr_race,updt_time");
            sb.Append(" ) VALUES (  ");

            sb.Append(DBValueConvert.ToDBString(number) + ","); //ccode
            sb.Append(DBValueConvert.ToDBString(name) + ","); // ename
            sb.Append(DBValueConvert.ToDBString(chineseName) + ","); // cname
            sb.Append(DBValueConvert.ToDBString(addr) + ","); // addr
            sb.Append(DBValueConvert.ToDBString(city) + ","); // city

            sb.Append(DBValueConvert.ToDBString(stat) + ","); // stat
            sb.Append(DBValueConvert.ToDBString(zip) + ","); // zip
            sb.Append(DBValueConvert.ToDBString(phone) + ","); // tele
            sb.Append("'W',");
            sb.Append(DBValueConvert.ToDBString(name) + ","); // cust_bill_nm

            sb.Append(DBValueConvert.ToDBString(contact) + ","); //cust_cntct
            sb.Append("'','','',null,");

            sb.Append(DBValueConvert.ToDBNumber(balance) + ","); // cust_credit_limit
            sb.Append("'',");
            sb.Append(DBValueConvert.ToDBString(email) + ","); //cust_email
            sb.Append("'','Y',");

            sb.Append(DBValueConvert.ToDBString(ship_name) + ","); 
            sb.Append(DBValueConvert.ToDBString(ship_addr) + ","); 
            sb.Append(DBValueConvert.ToDBString(ship_city) + ","); 
            sb.Append(DBValueConvert.ToDBString(ship_state) + ","); 
            sb.Append(DBValueConvert.ToDBString(ship_zip) + ",");

            sb.Append(DBValueConvert.ToDBNumber(sales_no) + ","); // cust_sales_no
            sb.Append(DBValueConvert.ToDBString(chineseName) + ","); // bill_cnm
            sb.Append(DBValueConvert.ToDBString(ship_cname) + ","); // shp_cnm
            sb.Append("'N','US',");

            sb.Append("'US','N',null,'','',");
            sb.Append("null,null,null,'',1,");
            sb.Append("'','',null,'',null,");

            sb.Append(DBValueConvert.ToDBString(region) + ","); // cust_lz_no
            sb.Append("null,null,null ");

            sb.Append(")");

            return sb.ToString();
        }


        public string InsertCustomerBalance(string number, 
            string currentBalance, 
            string v30DaysBalance, 
            string v60DaysBalance, 
            string v90DaysBalance)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO DBA.t_cmr_baln ( ");

            sb.Append("ccode,cur_baln, d30_baln, d60_baln, d90_baln ");
            sb.Append(" ) VALUES (  ");

            sb.Append(DBValueConvert.ToDBString(number) + ","); //ccode
            sb.Append(DBValueConvert.ToDBNumber(currentBalance) + ","); // cur_baln
            sb.Append(DBValueConvert.ToDBNumber(v30DaysBalance) + ","); // d30_baln
            sb.Append(DBValueConvert.ToDBNumber(v60DaysBalance) + ","); // d60_baln
            sb.Append(DBValueConvert.ToDBNumber(v90DaysBalance) ); // d90_baln

            sb.Append(" )");

            return sb.ToString();
        }

        public string UpdateCustomer(string number,string balance)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE DBA.client ");
            sb.Append("SET cust_credit_limit = " + DBValueConvert.ToDBNumber(balance));
            sb.Append(" WHERE ccode = " + DBValueConvert.ToDBString(number));
            
            return sb.ToString();
        }

        public string UpdateCustomerBalance(string number,
            string currentBalance,
            string v30DaysBalance,
            string v60DaysBalance,
            string v90DaysBalance)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE DBA.t_cmr_baln SET ");
            sb.Append("cur_baln = " + DBValueConvert.ToDBNumber(currentBalance) + ","); //cur_baln
            sb.Append("d30_baln = " + DBValueConvert.ToDBNumber(v30DaysBalance) + ","); //d30_baln
            sb.Append("d60_baln = " + DBValueConvert.ToDBNumber(v60DaysBalance) + ","); //d60_baln
            sb.Append("d90_baln = " + DBValueConvert.ToDBNumber(v90DaysBalance) ); //d90_baln
            sb.Append(" WHERE ccode = " + DBValueConvert.ToDBString(number));

            return sb.ToString();
        }



        public string InsertEmployee(string smNo,string firstName,string lastName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO DBA.t_empl_profile ( ");

            sb.Append("empl_no,empl_first_nm,empl_last_nm,empl_ss_no,empl_status,");
            sb.Append("empl_start_dt,empl_salary,empl_address,empl_city,empl_zip,");
            sb.Append("empl_phone,empl_state,empl_sex,empl_salary_type,empl_remark,");
            sb.Append("empl_end_dt,empl_alt_phone,empl_mi,empl_brief_nm,empl_dep_id,");
            sb.Append("empl_posit_id,empl_pay_id,empl_login_id,empl_lbl,empl_cms_fl,");
            sb.Append("empl_cms_rt");

            sb.Append(" ) VALUES (  ");

            //sb.Append(" SELECT coalesce(max(empl_no),0) as number From dba.t_empl_profile , ");
            sb.Append(DBValueConvert.ToDBNumber(smNo) + ","); //empl_no
            sb.Append(DBValueConvert.ToDBString(firstName) + ","); //empl_first_nm
            sb.Append(DBValueConvert.ToDBString(lastName) + ","); //empl_last_nm
            sb.Append("'','Y',");

            sb.Append("null,null,'','','',");
            sb.Append("'','','','','',");
            sb.Append("null,'','','',null,");
            sb.Append("null,'','',"+ DBValueConvert.ToDBString(smNo) + ",'',");

            sb.Append("null");
            sb.Append(")");

            return sb.ToString();
        }

        public string UpdateEmployee(string smNo, string firstName, string lastName)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE DBA.t_empl_profile SET ");
            sb.Append("empl_first_nm = " + DBValueConvert.ToDBString(firstName) + ",");
            sb.Append("empl_last_nm = " + DBValueConvert.ToDBString(lastName) );
            sb.Append(" WHERE empl_no = " + DBValueConvert.ToDBNumber(smNo));

            return sb.ToString();
        }



        //qoute and invorice

        public decimal GetMaxQuoteID()
        {
            object res = DbHelperOdbc.ExecuteScalar("SELECT ISNULL(MAX(qo_id+1),1) FROM DBA.t_quote_lst");

            return Convert.ToDecimal(res);

        }

        public decimal GetMaxInvoiceID()
        {
            object res = DbHelperOdbc.ExecuteScalar("SELECT ISNULL(MAX(inv_id+1),1)  FROM DBA.t_invoice_lst");

            return Convert.ToDecimal(res);

        }

        public string SaveInvoice(string accountno, string poNumber, string status, string RefNumber, string invoiceno, string salesname, decimal ordertotal,
                                                    string shipvia, string orderdate, string deliverydate, string message, string shiptoadr)
        {
            decimal inv_id = Convert.ToDecimal( RefNumber);
            string table = "DBA.t_invoice_lst";
            string where = "WHERE inv_id = " + inv_id;

            return CreateSaveSQL(table, where,
                InsertInvoice(
                 accountno, poNumber, status, RefNumber, invoiceno, salesname, ordertotal, shipvia, orderdate, deliverydate, message
                ),
                UpdateInvoice(
                 inv_id,accountno, poNumber, status, RefNumber, invoiceno, salesname, ordertotal, shipvia, orderdate, deliverydate, message
                ));

        }

        public string InsertInvoice( string accountno, string po_no, string status, string ref_no,
            string invoiceno, string salesman, decimal ordertotal, string shipvia,
            string orderdate, string deliverydate, string message)
        {

            decimal inv_id = GetMaxInvoiceID();

            //查询customer数据
            string inv_cust_no = "", inv_cmr_nm = "", inv_cmr_cnm = "", inv_cmr_addr = "", inv_cmr_city = "", inv_cmr_st = "", inv_cmr_zip = "";
            int inv_terms = 0;
            inv_cust_no = accountno;
            string custSql = string.Format("select ccode,ename,cname,addr,city,stat,zip,cust_term from dba.client where ccode = '{0}'", accountno);
            DataSet custDS = DbHelperOdbc.Query(custSql);
            if (custDS.Tables[0].Rows.Count > 0)
            {
                DataTable dt = custDS.Tables[0];
                inv_cmr_nm = dt.Rows[0]["ename"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["ename"]);
                inv_cmr_cnm = dt.Rows[0]["cname"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["cname"]);
                inv_cmr_addr = dt.Rows[0]["addr"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["addr"]);
                inv_cmr_city = dt.Rows[0]["city"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["city"]);
                inv_cmr_st = dt.Rows[0]["stat"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["stat"]);
                inv_cmr_zip = dt.Rows[0]["zip"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["zip"]);
                inv_terms = dt.Rows[0]["cust_term"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["cust_term"]);
            }
            ///////////////////

            //查询Employee的Sale No
            int inv_sales_no = 0;

            string saleNoSql = string.Format("select empl_no from dba.t_empl_profile where (Trim(empl_last_nm)+' '+Trim(empl_first_nm)) = '{0}'", salesman);
            inv_sales_no = Convert.ToInt32(DbHelperOdbc.ExecuteScalar(saleNoSql));
            ///////////////////////

            //查询ShipVia数据
            string inv_shp_nm = "", inv_shp_cnm = "", inv_shp_addr = "", inv_shp_city = "", inv_shp_st = "", inv_shp_zip = "";
            int inv_ship_via = 0;
            string shipviaSql = string.Format("select sh_via_no,sh_via_name,sh_via_desc,sh_via_addr,sh_via_city,sh_via_state,sh_via_zip from dba.t_ship_via where sh_via_name = '{0}'", shipvia);
            DataSet shipviaDS = DbHelperOdbc.Query(shipviaSql);
            if (shipviaDS.Tables[0].Rows.Count > 0)
            {
                DataTable svdt = shipviaDS.Tables[0];
                inv_ship_via = svdt.Rows[0]["sh_via_no"] == DBNull.Value ? 0 : Convert.ToInt32(svdt.Rows[0]["sh_via_no"]);
                inv_shp_nm = svdt.Rows[0]["sh_via_name"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_name"]);
                inv_shp_cnm = svdt.Rows[0]["sh_via_desc"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_desc"]);
                inv_shp_addr = svdt.Rows[0]["sh_via_addr"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_addr"]);
                inv_shp_city = svdt.Rows[0]["sh_via_city"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_city"]);
                inv_shp_st = svdt.Rows[0]["sh_via_state"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_state"]);
                inv_shp_zip = svdt.Rows[0]["sh_via_zip"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_zip"]);
            }
            /////////////////

            decimal inv_tax_rate = 0, inv_tax_amt = 0, inv_paid = 0, inv_adj_no = 0;
            string inv_print_fl = "N", inv_station_enter = "N";
            int inv_loc = getCompanyLoc();

            StringBuilder sb = new StringBuilder();
            sb.Append(" INSERT INTO DBA.t_invoice_lst  ( ");
            sb.Append(" inv_id, inv_date, inv_cust_no, inv_cleared_fl, inv_terms,");
            sb.Append(" inv_due_date, inv_ref, inv_tax_rate, inv_tax_amt, inv_total, ");
            sb.Append(" inv_memo, inv_print_fl, inv_station_enter, inv_sales_no, inv_paid,");
            sb.Append(" inv_cmr_nm, inv_cmr_cnm, inv_cmr_addr, inv_cmr_city, inv_cmr_st, ");
            sb.Append(" inv_cmr_zip, inv_shp_nm, inv_shp_cnm, inv_shp_addr, inv_shp_city,");
            sb.Append(" inv_shp_st, inv_shp_zip, inv_ship_via, inv_adj_no, crt_time,");
            sb.Append(" inv_stn, so_id, inv_loc, inv_delvy_date, inv_qo_no ");
            sb.Append(" ) VALUES (  ");

            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(ref_no)); //inv_id
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(orderdate))); //inv_date
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_cust_no));       //inv_cust_no
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString("N") ); //DBValueConvert.ToDBString(inv_cleared_fl));    // inv_cleared_fl
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_terms));         //inv_terms

            sb.AppendFormat(" {0},", "null");// DBValueConvert.ToDBString(DBValueConvert.ToDBDate(inv_due_date)));    //inv_due_date
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(invoiceno));     //inv_ref
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_tax_rate));  //inv_tax_rate
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_tax_amt));   //inv_tax_amt
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(ordertotal));    //inv_total

            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(message));       // inv_memo
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_print_fl)); // inv_print_fl
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_station_enter)); // inv_station_enter
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_sales_no)); // inv_sales_no
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_paid));      // inv_paid

            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_cmr_nm));    // inv_cmr_nm
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_cmr_cnm));   // inv_cmr_cnm
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_cmr_addr));   // inv_cmr_addr
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_cmr_city));   // inv_cmr_city
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_cmr_st));   // inv_cmr_st

            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_cmr_zip));   // inv_cmr_zip
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_shp_nm));    // inv_shp_nm
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_shp_cnm));   // inv_shp_cnm
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_shp_addr));   // inv_shp_addr
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_cmr_city));   // inv_cmr_city

            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_shp_st));   // inv_shp_st
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_shp_zip));   // inv_shp_zip
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_ship_via));   // inv_ship_via
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_adj_no));   // inv_adj_no
            sb.AppendFormat(" {0},", "GetDate()");   // crt_time

            sb.AppendFormat(" {0},", "null");   // inv_stn
            sb.AppendFormat(" {0},", "null");   // so_id
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_loc));   // inv_loc
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(deliverydate)));   // inv_delvy_date
            sb.AppendFormat(" {0}", DBValueConvert.ToDBNumber(ref_no));   // inv_qo_no

            sb.Append(") ");

            return sb.ToString();
        }

        public string UpdateInvoice(decimal inv_id, string accountno, string po_no, string status, string ref_no,
            string invoiceno, string salesman, decimal ordertotal, string shipvia,
            string orderdate, string deliverydate, string message)
        {
            //string inv_cleared_fl = "";
            //if (status.Equals("0")) //new -open
            //{
            //    inv_cleared_fl = "N";
            //}
            //else if (status.Equals("1"))//Packed --不确定
            //{
            //    inv_cleared_fl = "P";
            //}
            //else if (status.Equals("2"))//Invoiced - closed
            //{
            //    inv_cleared_fl = "Y";
            //}
            //else if (status.Equals("9"))//Canceled - void
            //{
            //    inv_cleared_fl = "V";
            //}

            //查询customer数据
            string inv_cust_no = "", inv_cmr_nm = "", inv_cmr_cnm = "", inv_cmr_addr = "", inv_cmr_city = "", inv_cmr_st = "", inv_cmr_zip = "";
            int inv_terms = 0;
            inv_cust_no = accountno;
            string custSql = string.Format("select ccode,ename,cname,addr,city,stat,zip,cust_term from dba.client where ccode = '{0}'", accountno);
            DataSet custDS = DbHelperOdbc.Query(custSql);
            if (custDS.Tables[0].Rows.Count > 0)
            {
                DataTable dt = custDS.Tables[0];
                inv_cmr_nm = dt.Rows[0]["ename"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["ename"]);
                inv_cmr_cnm = dt.Rows[0]["cname"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["cname"]);
                inv_cmr_addr = dt.Rows[0]["addr"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["addr"]);
                inv_cmr_city = dt.Rows[0]["city"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["city"]);
                inv_cmr_st = dt.Rows[0]["stat"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["stat"]);
                inv_cmr_zip = dt.Rows[0]["zip"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["zip"]);
                inv_terms = dt.Rows[0]["cust_term"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["cust_term"]);
            }
            ///////////////////

            //查询Employee的Sale No
            int inv_sales_no = 0;

            string saleNoSql = string.Format("select empl_no from dba.t_empl_profile where (Trim(empl_last_nm)+' '+Trim(empl_first_nm)) = '{0}'", salesman);
            inv_sales_no = Convert.ToInt32(DbHelperOdbc.ExecuteScalar(saleNoSql));
            ///////////////////////

            //查询ShipVia数据
            string inv_shp_nm = "", inv_shp_cnm = "", inv_shp_addr = "", inv_shp_city = "", inv_shp_st = "", inv_shp_zip = "";
            int inv_ship_via = 0;
            string shipviaSql = string.Format("select sh_via_no,sh_via_name,sh_via_desc,sh_via_addr,sh_via_city,sh_via_state,sh_via_zip from dba.t_ship_via where sh_via_name = '{0}'", shipvia);
            DataSet shipviaDS = DbHelperOdbc.Query(shipviaSql);
            if (shipviaDS.Tables[0].Rows.Count > 0)
            {
                DataTable svdt = shipviaDS.Tables[0];
                inv_ship_via = svdt.Rows[0]["sh_via_no"] == DBNull.Value ? 0 : Convert.ToInt32(svdt.Rows[0]["sh_via_no"]);
                inv_shp_nm = svdt.Rows[0]["sh_via_name"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_name"]);
                inv_shp_cnm = svdt.Rows[0]["sh_via_desc"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_desc"]);
                inv_shp_addr = svdt.Rows[0]["sh_via_addr"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_addr"]);
                inv_shp_city = svdt.Rows[0]["sh_via_city"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_city"]);
                inv_shp_st = svdt.Rows[0]["sh_via_state"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_state"]);
                inv_shp_zip = svdt.Rows[0]["sh_via_zip"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_zip"]);
            }
            /////////////////
            int inv_loc = getCompanyLoc();

            StringBuilder sb = new StringBuilder();
            sb.Append(" UPDATE DBA.t_invoice_lst  SET ");
            sb.AppendFormat("inv_date = {0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(orderdate)));
            sb.AppendFormat("inv_cust_no = {0},", DBValueConvert.ToDBString(inv_cust_no));
            //sb.AppendFormat("inv_cleared_fl = {0},", DBValueConvert.ToDBString(inv_cleared_fl));
            sb.AppendFormat("inv_terms = {0},", DBValueConvert.ToDBNumber(inv_terms));
            sb.AppendFormat("inv_ref = {0},", DBValueConvert.ToDBString(invoiceno));
            sb.AppendFormat("inv_total = {0},", DBValueConvert.ToDBNumber(ordertotal));
            sb.AppendFormat("inv_memo = {0},", DBValueConvert.ToDBString(message));
            sb.AppendFormat("inv_sales_no = {0},", DBValueConvert.ToDBNumber(inv_sales_no));
            sb.AppendFormat("inv_cmr_nm = {0},", DBValueConvert.ToDBString(inv_cmr_nm));
            sb.AppendFormat("inv_cmr_cnm = {0},", DBValueConvert.ToDBString(inv_cmr_cnm));
            sb.AppendFormat("inv_cmr_addr = {0},", DBValueConvert.ToDBString(inv_cmr_addr));
            sb.AppendFormat("inv_cmr_city = {0},", DBValueConvert.ToDBString(inv_cmr_city));
            sb.AppendFormat("inv_cmr_st = {0},", DBValueConvert.ToDBString(inv_cmr_st));

            sb.AppendFormat("inv_cmr_zip = {0},", DBValueConvert.ToDBString(inv_cmr_zip));
            sb.AppendFormat("inv_shp_nm = {0},", DBValueConvert.ToDBString(inv_shp_nm));
            sb.AppendFormat("inv_shp_cnm = {0},", DBValueConvert.ToDBString(inv_shp_cnm));
            sb.AppendFormat("inv_shp_addr = {0},", DBValueConvert.ToDBString(inv_shp_addr));
            sb.AppendFormat("inv_shp_city = {0},", DBValueConvert.ToDBString(inv_shp_city));

            sb.AppendFormat("inv_shp_st = {0},", DBValueConvert.ToDBString(inv_shp_st));
            sb.AppendFormat("inv_shp_zip = {0},", DBValueConvert.ToDBString(inv_shp_zip));
            sb.AppendFormat("inv_ship_via = {0},", DBValueConvert.ToDBNumber(inv_ship_via));
            sb.AppendFormat("inv_loc = {0}, ", DBValueConvert.ToDBNumber(inv_loc));
            sb.AppendFormat("inv_qo_no = {0} ", DBValueConvert.ToDBNumber(ref_no));  
            sb.Append(" WHERE inv_id = " + DBValueConvert.ToDBNumber(inv_id));

            return sb.ToString(); 
        }

        public string SaveQuote(decimal qo_id, string accountno, string poNumber, string status, string RefNumber, string invoiceno, string salesname, decimal ordertotal,
                                                    string shipvia, string orderdate, string deliverydate, string message, string shiptoadrs)
        {
            string table = "DBA.t_quote_lst";
            string where = " WHERE qo_id = " + qo_id;

            return CreateSaveSQL(table, where,
                InsertQuote(qo_id,  poNumber, accountno,  status, RefNumber  ,invoiceno, salesname, ordertotal, shipvia, orderdate, deliverydate, message),
                UpdateQuote(qo_id,  poNumber, accountno,  status,  RefNumber , invoiceno, salesname, ordertotal, shipvia, orderdate, deliverydate, message));

        }

        public string InsertQuote(decimal qo_id, string accountno, string po_no, string status, string ref_no,
            string invoiceno, string salesman, decimal ordertotal, string shipvia,
            string orderdate, string deliverydate, string message)
        {
            string qo_cleared_fl = "";
            if (status.Equals("0")) //new -open
            {
                qo_cleared_fl = "N";
            }
            else if (status.Equals("1"))//Packed --不确定
            {
                qo_cleared_fl = "P";
            }
            else if (status.Equals("2"))//Invoiced - closed
            {
                qo_cleared_fl = "Y";
            }
            else if (status.Equals("9"))//Canceled - void
            {
                qo_cleared_fl = "V";
            }

            //查询customer数据
            string qo_cust_no = "", qo_cmr_nm = "", qo_cmr_cnm = "", qo_cmr_addr = "", qo_cmr_city = "", qo_cmr_st = "", qo_cmr_zip = "";

            qo_cust_no = accountno;
            string custSql = string.Format("select ccode,ename,cname,addr,city,stat,zip from dba.client where ccode = '{0}'", accountno);
            DataSet custDS = DbHelperOdbc.Query(custSql);
            if (custDS.Tables[0].Rows.Count > 0)
            {
                DataTable dt = custDS.Tables[0];
                qo_cmr_nm = dt.Rows[0]["ename"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["ename"]);
                qo_cmr_cnm = dt.Rows[0]["cname"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["cname"]);
                qo_cmr_addr = dt.Rows[0]["addr"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["addr"]);
                qo_cmr_city = dt.Rows[0]["city"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["city"]);
                qo_cmr_st = dt.Rows[0]["stat"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["stat"]);
                qo_cmr_zip = dt.Rows[0]["zip"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["zip"]);
            }
            ///////////////////

            //查询Employee的Sale No
            int qo_sales_no = 0;

            string saleNoSql = string.Format("select empl_no from dba.t_empl_profile where (Trim(empl_last_nm)+' '+Trim(empl_first_nm)) = '{0}'", salesman);
            qo_sales_no = Convert.ToInt32(DbHelperOdbc.ExecuteScalar(saleNoSql));
            ///////////////////////

            //查询ShipVia数据
            string qo_shp_nm = "", qo_shp_cnm = "", qo_shp_addr = "", qo_shp_city = "", qo_shp_st = "", qo_shp_zip = "";
            int qo_ship_via = 0;
            string shipviaSql = string.Format("select sh_via_no,sh_via_name,sh_via_desc,sh_via_addr,sh_via_city,sh_via_state,sh_via_zip from dba.t_ship_via where sh_via_name = '{0}'", shipvia);
            DataSet shipviaDS = DbHelperOdbc.Query(shipviaSql);
            if (shipviaDS.Tables[0].Rows.Count > 0)
            {
                DataTable svdt = shipviaDS.Tables[0];
                qo_ship_via = svdt.Rows[0]["sh_via_no"] == DBNull.Value ? 0 : Convert.ToInt32(svdt.Rows[0]["sh_via_no"]);
                qo_shp_nm = svdt.Rows[0]["sh_via_name"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_name"]);
                qo_shp_cnm = svdt.Rows[0]["sh_via_desc"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_desc"]);
                qo_shp_addr = svdt.Rows[0]["sh_via_addr"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_addr"]);
                qo_shp_city = svdt.Rows[0]["sh_via_city"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_city"]);
                qo_shp_st = svdt.Rows[0]["sh_via_state"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_state"]);
                qo_shp_zip = svdt.Rows[0]["sh_via_zip"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_zip"]);
            }
            /////////////////

            string qo_ref = invoiceno, qo_map_loc = "";
            decimal qo_tax_rate = 0, qo_tax_amt = 0;
            int qo_loc = getCompanyLoc();

            StringBuilder sb = new StringBuilder();
            sb.Append(" INSERT INTO DBA.t_quote_lst  ( ");
            sb.Append(" qo_id,qo_no,qo_date,qo_cust_no,qo_cleared_fl,");
            sb.Append(" qo_expire_date,qo_ref,qo_tax_rate,qo_tax_amt,qo_total, ");
            sb.Append(" qo_memo,qo_sales_no,qo_cmr_nm,qo_cmr_cnm,qo_cmr_addr,");
            sb.Append(" qo_cmr_city,qo_cmr_st,qo_cmr_zip,qo_loc,qo_map_loc, ");
            sb.Append(" updt_time,qo_delvy_date,qo_shp_nm,qo_shp_cnm,qo_shp_addr,");
            sb.Append(" qo_shp_city,qo_shp_st,qo_shp_zip,qo_ship_via,upld_time");
            sb.Append(" ) VALUES (  ");

            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_id)); //qo_id
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(ref_no)); //qo_no
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(orderdate)));       //qo_date
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_cust_no));    // qo_cust_no
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_cleared_fl)); //qo_cleared_fl

            sb.AppendFormat(" {0},", "null"); // DBValueConvert.ToDBString((DBValueConvert.ToDBDate(qo_expire_date))); //qo_expire_date
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_ref));     //qo_ref
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_tax_rate));   //qo_tax_rate
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_tax_amt));    // qo_tax_amt
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(ordertotal)); //qo_total

            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(message)); //qo_memo
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_sales_no)); //qo_sales_no
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_cmr_nm)); //qo_cmr_nm
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_cmr_cnm)); //qo_cmr_cnm
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_cmr_addr)); //qo_cmr_addr

            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_cmr_city));   //qo_cmr_city
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_cmr_st));     //qo_cmr_st
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_cmr_zip));    //qo_cmr_zip
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_loc));        //qo_loc
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_map_loc));    //qo_map_loc

            sb.AppendFormat(" {0},", "getdate()"); //DBValueConvert.ToDBString(updt_time));    //updt_time
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(deliverydate)));    //qo_delvy_date
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_shp_nm));    //qo_shp_nm
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_shp_cnm));    //qo_shp_cnm
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_shp_addr));    //qo_shp_addr

            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_shp_city));    //qo_shp_city
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_shp_st));    //qo_shp_st
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_shp_zip));    //qo_shp_zip
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_ship_via));    //qo_ship_via
            sb.AppendFormat(" {0}", "null");    //upld_time

            sb.Append(") ");
            return sb.ToString();
        }

        public string UpdateQuote(decimal qo_id, string accountno, string po_no, string status, string ref_no,
            string invoiceno, string salesman, decimal ordertotal, string shipvia,
            string orderdate, string deliverydate, string message)
        {

            string qo_cleared_fl = "";
            if (status.Equals("0")) //new -open
            {
                qo_cleared_fl = "N";
            }
            else if (status.Equals("1"))//Packed --不确定
            {
                qo_cleared_fl = "P";
            }
            else if (status.Equals("2"))//Invoiced - closed
            {
                qo_cleared_fl = "Y";
            }
            else if (status.Equals("9"))//Canceled - void
            {
                qo_cleared_fl = "V";
            }

            //查询customer数据
            string qo_cust_no = "", qo_cmr_nm = "", qo_cmr_cnm = "", qo_cmr_addr = "", qo_cmr_city = "", qo_cmr_st = "", qo_cmr_zip = "";

            qo_cust_no = accountno;
            string custSql = string.Format("select ccode,ename,cname,addr,city,stat,zip from dba.client where ccode = '{0}'", accountno);
            DataSet custDS = DbHelperOdbc.Query(custSql);
            if (custDS.Tables[0].Rows.Count > 0)
            {
                DataTable dt = custDS.Tables[0];
                qo_cmr_nm = dt.Rows[0]["ename"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["ename"]);
                qo_cmr_cnm = dt.Rows[0]["cname"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["cname"]);
                qo_cmr_addr = dt.Rows[0]["addr"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["addr"]);
                qo_cmr_city = dt.Rows[0]["city"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["city"]);
                qo_cmr_st = dt.Rows[0]["stat"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["stat"]);
                qo_cmr_zip = dt.Rows[0]["zip"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["zip"]);
            }
            ///////////////////

            //查询Employee的Sale No
            int qo_sales_no = 0;

            string saleNoSql = string.Format("select empl_no from dba.t_empl_profile where (Trim(empl_last_nm)+' '+Trim(empl_first_nm)) = '{0}'", salesman);
            qo_sales_no = Convert.ToInt32(DbHelperOdbc.ExecuteScalar(saleNoSql));
            ///////////////////////

            //查询ShipVia数据
            string qo_shp_nm = "", qo_shp_cnm = "", qo_shp_addr = "", qo_shp_city = "", qo_shp_st = "", qo_shp_zip = "";
            int qo_ship_via = 0;
            string shipviaSql = string.Format("select sh_via_no,sh_via_name,sh_via_desc,sh_via_addr,sh_via_city,sh_via_state,sh_via_zip from dba.t_ship_via where sh_via_name = '{0}'", shipvia);
            DataSet shipviaDS = DbHelperOdbc.Query(shipviaSql);
            if (shipviaDS.Tables[0].Rows.Count > 0)
            {
                DataTable svdt = shipviaDS.Tables[0];
                qo_ship_via = svdt.Rows[0]["sh_via_no"] == DBNull.Value ? 0 : Convert.ToInt32(svdt.Rows[0]["sh_via_no"]);
                qo_shp_nm = svdt.Rows[0]["sh_via_name"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_name"]);
                qo_shp_cnm = svdt.Rows[0]["sh_via_desc"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_desc"]);
                qo_shp_addr = svdt.Rows[0]["sh_via_addr"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_addr"]);
                qo_shp_city = svdt.Rows[0]["sh_via_city"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_city"]);
                qo_shp_st = svdt.Rows[0]["sh_via_state"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_state"]);
                qo_shp_zip = svdt.Rows[0]["sh_via_zip"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_zip"]);
            }
            /////////////////

            string qo_ref = invoiceno;
            int qo_loc = getCompanyLoc();

            StringBuilder sb = new StringBuilder();
            sb.Append(" UPDATE DBA.t_quote_lst set  ");

            sb.AppendFormat(" qo_no = {0},", DBValueConvert.ToDBNumber(ref_no)); //qo_no
            sb.AppendFormat(" orderdate = {0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(orderdate)));       //qo_date
            sb.AppendFormat(" qo_cust_no = {0},", DBValueConvert.ToDBString(qo_cust_no));    // qo_cust_no
            sb.AppendFormat(" qo_cleared_fl = {0},", DBValueConvert.ToDBString(qo_cleared_fl)); //qo_cleared_fl
            sb.AppendFormat(" qo_ref = {0},", DBValueConvert.ToDBString(qo_ref));     //qo_ref

            sb.AppendFormat(" qo_total = {0},", DBValueConvert.ToDBNumber(ordertotal)); //qo_total            
            sb.AppendFormat(" qo_memo = {0},", DBValueConvert.ToDBString(message)); //qo_memo
            sb.AppendFormat(" qo_sales_no = {0},", DBValueConvert.ToDBNumber(qo_sales_no)); //qo_sales_no
            sb.AppendFormat(" qo_cmr_nm = {0},", DBValueConvert.ToDBString(qo_cmr_nm)); //qo_cmr_nm
            sb.AppendFormat(" qo_cmr_cnm = {0},", DBValueConvert.ToDBString(qo_cmr_cnm)); //qo_cmr_cnm

            sb.AppendFormat(" qo_cmr_addr = {0},", DBValueConvert.ToDBString(qo_cmr_addr)); //qo_cmr_addr
            sb.AppendFormat(" qo_cmr_city = {0},", DBValueConvert.ToDBString(qo_cmr_city));   //qo_cmr_city
            sb.AppendFormat(" qo_cmr_st = {0},", DBValueConvert.ToDBString(qo_cmr_st));     //qo_cmr_st
            sb.AppendFormat(" qo_cmr_zip = {0},", DBValueConvert.ToDBString(qo_cmr_zip));    //qo_cmr_zip
            sb.AppendFormat(" qo_loc = {0},", DBValueConvert.ToDBNumber(qo_loc));        //qo_loc

            sb.AppendFormat(" updt_time = {0},", "getdate()"); //DBValueConvert.ToDBString(updt_time));    //updt_time
            sb.AppendFormat(" qo_delvy_date = {0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(deliverydate)));    //qo_delvy_date
            sb.AppendFormat(" qo_shp_nm = {0},", DBValueConvert.ToDBString(qo_shp_nm));    //qo_shp_nm
            sb.AppendFormat(" qo_shp_cnm = {0},", DBValueConvert.ToDBString(qo_shp_cnm));    //qo_shp_cnm
            sb.AppendFormat(" qo_shp_addr = {0},", DBValueConvert.ToDBString(qo_shp_addr));    //qo_shp_addr

            sb.AppendFormat(" qo_shp_city = {0},", DBValueConvert.ToDBString(qo_shp_city));    //qo_shp_city
            sb.AppendFormat(" qo_shp_st = {0},", DBValueConvert.ToDBString(qo_shp_st));    //qo_shp_st
            sb.AppendFormat(" qo_shp_zip = {0},", DBValueConvert.ToDBString(qo_shp_zip));    //qo_shp_zip
            sb.AppendFormat(" qo_ship_via = {0}", DBValueConvert.ToDBNumber(qo_ship_via));    //qo_ship_via

            sb.Append(" WHERE qo_id = " + qo_id);
            return sb.ToString();
        }


        public bool IsExistQuote(string refNumber)
        {
            object res = DbHelperOdbc.ExecuteScalar("SELECT COUNT(1) FROM DBA.t_quote_lst WHERE qo_no = " + DBValueConvert.ToDBNumber(refNumber) + " ");

            return Convert.ToInt32(res)>0?true:false;
        }

        public string UpdateQuoteForPacked( string accountno, string po_no, string status, string ref_no,
            string invoiceno, string salesman, decimal ordertotal, string shipvia,
            string orderdate, string deliverydate, string message)
        {
             
            //查询customer数据
            string qo_cust_no = "", qo_cmr_nm = "", qo_cmr_cnm = "", qo_cmr_addr = "", qo_cmr_city = "", qo_cmr_st = "", qo_cmr_zip = "";

            qo_cust_no = accountno;
            string custSql = string.Format("select ccode,ename,cname,addr,city,stat,zip from dba.client where ccode = '{0}'", accountno);
            DataSet custDS = DbHelperOdbc.Query(custSql);
            if (custDS.Tables[0].Rows.Count > 0)
            {
                DataTable dt = custDS.Tables[0];
                qo_cmr_nm = dt.Rows[0]["ename"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["ename"]);
                qo_cmr_cnm = dt.Rows[0]["cname"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["cname"]);
                qo_cmr_addr = dt.Rows[0]["addr"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["addr"]);
                qo_cmr_city = dt.Rows[0]["city"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["city"]);
                qo_cmr_st = dt.Rows[0]["stat"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["stat"]);
                qo_cmr_zip = dt.Rows[0]["zip"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["zip"]);
            }
            ///////////////////

            //查询Employee的Sale No
            int qo_sales_no = 0;

            string saleNoSql = string.Format("select empl_no from dba.t_empl_profile where (Trim(empl_last_nm)+' '+Trim(empl_first_nm)) = '{0}'", salesman);
            qo_sales_no = Convert.ToInt32(DbHelperOdbc.ExecuteScalar(saleNoSql));
            ///////////////////////

            //查询ShipVia数据
            string qo_shp_nm = "", qo_shp_cnm = "", qo_shp_addr = "", qo_shp_city = "", qo_shp_st = "", qo_shp_zip = "";
            int qo_ship_via = 0;
            string shipviaSql = string.Format("select sh_via_no,sh_via_name,sh_via_desc,sh_via_addr,sh_via_city,sh_via_state,sh_via_zip from dba.t_ship_via where sh_via_name = '{0}'", shipvia);
            DataSet shipviaDS = DbHelperOdbc.Query(shipviaSql);
            if (shipviaDS.Tables[0].Rows.Count > 0)
            {
                DataTable svdt = shipviaDS.Tables[0];
                qo_ship_via = svdt.Rows[0]["sh_via_no"] == DBNull.Value ? 0 : Convert.ToInt32(svdt.Rows[0]["sh_via_no"]);
                qo_shp_nm = svdt.Rows[0]["sh_via_name"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_name"]);
                qo_shp_cnm = svdt.Rows[0]["sh_via_desc"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_desc"]);
                qo_shp_addr = svdt.Rows[0]["sh_via_addr"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_addr"]);
                qo_shp_city = svdt.Rows[0]["sh_via_city"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_city"]);
                qo_shp_st = svdt.Rows[0]["sh_via_state"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_state"]);
                qo_shp_zip = svdt.Rows[0]["sh_via_zip"] == DBNull.Value ? "" : Convert.ToString(svdt.Rows[0]["sh_via_zip"]);
            }
            /////////////////

            string qo_ref = invoiceno;
            int qo_loc = getCompanyLoc();

            StringBuilder sb = new StringBuilder();
            sb.Append(" UPDATE DBA.t_quote_lst set  ");

            //sb.AppendFormat(" qo_no = {0},", DBValueConvert.ToDBNumber(ref_no)); //qo_no
            sb.AppendFormat(" qo_date = {0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(orderdate)));       //qo_date
            sb.AppendFormat(" qo_cust_no = {0},", DBValueConvert.ToDBString(qo_cust_no));    // qo_cust_no
           // sb.AppendFormat(" qo_cleared_fl = {0},", DBValueConvert.ToDBString(qo_cleared_fl)); //qo_cleared_fl
            sb.AppendFormat(" qo_ref = {0},", DBValueConvert.ToDBString(qo_ref));     //qo_ref

            sb.AppendFormat(" qo_total = {0},", DBValueConvert.ToDBNumber(ordertotal)); //qo_total            
            sb.AppendFormat(" qo_memo = {0},", DBValueConvert.ToDBString(message)); //qo_memo
            sb.AppendFormat(" qo_sales_no = {0},", DBValueConvert.ToDBNumber(qo_sales_no)); //qo_sales_no
            sb.AppendFormat(" qo_cmr_nm = {0},", DBValueConvert.ToDBString(qo_cmr_nm)); //qo_cmr_nm
            sb.AppendFormat(" qo_cmr_cnm = {0},", DBValueConvert.ToDBString(qo_cmr_cnm)); //qo_cmr_cnm

            sb.AppendFormat(" qo_cmr_addr = {0},", DBValueConvert.ToDBString(qo_cmr_addr)); //qo_cmr_addr
            sb.AppendFormat(" qo_cmr_city = {0},", DBValueConvert.ToDBString(qo_cmr_city));   //qo_cmr_city
            sb.AppendFormat(" qo_cmr_st = {0},", DBValueConvert.ToDBString(qo_cmr_st));     //qo_cmr_st
            sb.AppendFormat(" qo_cmr_zip = {0},", DBValueConvert.ToDBString(qo_cmr_zip));    //qo_cmr_zip
            sb.AppendFormat(" qo_loc = {0},", DBValueConvert.ToDBNumber(qo_loc));        //qo_loc

            sb.AppendFormat(" updt_time = {0},", "getdate()"); //DBValueConvert.ToDBString(updt_time));    //updt_time
            sb.AppendFormat(" qo_delvy_date = {0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(deliverydate)));    //qo_delvy_date
            sb.AppendFormat(" qo_shp_nm = {0},", DBValueConvert.ToDBString(qo_shp_nm));    //qo_shp_nm
            sb.AppendFormat(" qo_shp_cnm = {0},", DBValueConvert.ToDBString(qo_shp_cnm));    //qo_shp_cnm
            sb.AppendFormat(" qo_shp_addr = {0},", DBValueConvert.ToDBString(qo_shp_addr));    //qo_shp_addr

            sb.AppendFormat(" qo_shp_city = {0},", DBValueConvert.ToDBString(qo_shp_city));    //qo_shp_city
            sb.AppendFormat(" qo_shp_st = {0},", DBValueConvert.ToDBString(qo_shp_st));    //qo_shp_st
            sb.AppendFormat(" qo_shp_zip = {0},", DBValueConvert.ToDBString(qo_shp_zip));    //qo_shp_zip
            sb.AppendFormat(" qo_ship_via = {0}", DBValueConvert.ToDBNumber(qo_ship_via));    //qo_ship_via

            sb.AppendFormat(" WHERE qo_no = {0} " , DBValueConvert.ToDBNumber(ref_no));
            return sb.ToString();
        }


        //查询Company Loc#
        public int getCompanyLoc()
        {
            int inv_loc = 0;
            string invLocSql = string.Format("select comp_entry_no from dba.t_company_info where comp_default = 'Y'");
            inv_loc = Convert.ToInt32(DbHelperOdbc.GetSingle(invLocSql));
            return inv_loc;
        }

        public int GetItemLotNo(string p_code)
        {
            object res = DbHelperOdbc.ExecuteScalar("select max(lot_no) from dba.t_item_lot where lot_item = '" + p_code + "' ");

            return Convert.ToInt32(res);
        }

        public int GetShipViaNo(string shipvia)
        {
            object res = DbHelperOdbc.ExecuteScalar("select sh_via_no from dba.t_ship_via where sh_via_name = '" + shipvia + "'");

            return Convert.ToInt32(res);
        }

        public string SaveInvoiceDetail(decimal inv_id, string refNumber, string invoiceNumber, string itemNumber, string price, string quantity, string lineNumber)
        {
            string table = "DBA.t_invoice_detail";
            string where = "WHERE inv_id = " + DBValueConvert.ToDBNumber(inv_id) + " AND inv_entry_no = " + DBValueConvert.ToDBNumber(lineNumber);

            return CreateSaveSQL(table, where,
                InsertInvoiceDetail(inv_id, refNumber, invoiceNumber, itemNumber, price, quantity, lineNumber),
                UpdateInvoiceDetail(inv_id, refNumber, invoiceNumber, itemNumber, price, quantity, lineNumber));

        }

        public string InsertInvoiceDetail(decimal inv_id, string refNumber, string invoiceNumber, string itemNumber, string price, string quantity, string lineNumber)
        {
            //查询prodtable数据
            object inv_entry_lnk = null, inv_adj_no = null, inv_cost = null;
            string inv_pcode = "", inv_item_nm = "", inv_item_dsc = "", inv_unit_pkg = "", inv_tax_fl = "", inv_pkg_dsc = "", inv_sp_fl = "", inv_3_dim = "",
                inv_item_type = "",inv_remark = "", inv_retail_type = "", inv_wg = "";
            int item_pk_ct = 0, inv_lot = 0,inv_qty_per_case = 0;
            decimal inv_bo_qty = 0;
            string c_item_dsc = "";

            string custSql = string.Format("select p_code,proname,cpronam,item_pk_ct,c_item_dsc = a.mes_short_dsc,sale_tax,c_pkg_dsc = b.mes_short_dsc," +
                "inv_sp_fl = (CASE WHEN item_type = 'I' THEN 'N' ELSE 'Y' END),pkg_3_dim,item_type,amt_type,item_gw,pkg_ct" +
                " from dba.prodtable,dba.t_measure_dsc a,dba.t_measure_dsc b where item_no = '{0}' and item_dsc *= a.mes_dsc and pkg_dsc *= b.mes_dsc", itemNumber);
            DataSet custDS = DbHelperOdbc.Query(custSql);
            if (custDS.Tables[0].Rows.Count > 0)
            {
                DataTable dt = custDS.Tables[0];
                inv_pcode = dt.Rows[0]["p_code"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["p_code"]);
                inv_item_nm = dt.Rows[0]["proname"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["proname"]);
                inv_item_dsc = dt.Rows[0]["cpronam"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["cpronam"]);
                item_pk_ct = dt.Rows[0]["item_pk_ct"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["item_pk_ct"]);
                c_item_dsc = dt.Rows[0]["c_item_dsc"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["c_item_dsc"]);
                inv_tax_fl = dt.Rows[0]["sale_tax"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["sale_tax"]);
                inv_pkg_dsc = dt.Rows[0]["c_pkg_dsc"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["c_pkg_dsc"]);
                inv_sp_fl = dt.Rows[0]["inv_sp_fl"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["inv_sp_fl"]);
                inv_3_dim = dt.Rows[0]["pkg_3_dim"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["pkg_3_dim"]);
                inv_item_type = dt.Rows[0]["item_type"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["item_type"]);
                inv_retail_type = dt.Rows[0]["amt_type"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["amt_type"]);
                inv_wg = dt.Rows[0]["item_gw"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["item_gw"]);
                inv_qty_per_case = dt.Rows[0]["pkg_ct"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["pkg_ct"]);

                inv_unit_pkg = Convert.ToString(item_pk_ct) + " " + c_item_dsc;

                inv_lot = GetItemLotNo(inv_pcode);
            }
            ///////////////////

            int inv_item_loc = getCompanyLoc();

            StringBuilder sb = new StringBuilder();
            sb.Append(" INSERT INTO DBA.t_invoice_detail ( ");

            sb.Append(" inv_id,     inv_entry_no,  inv_pcode,        inv_item_nm,  inv_item_dsc, ");
            sb.Append(" inv_rate,   inv_qty,       inv_qty_per_case, inv_unit_pkg, inv_item_loc, ");
            sb.Append(" inv_tax_fl, inv_pkg_dsc,   inv_sp_fl,        inv_lot,       inv_3_dim,    ");
            sb.Append(" inv_item_type,inv_bo_qty, inv_entry_lnk,    inv_item_date,  inv_item_no,");
            sb.Append(" inv_adj_no, inv_remark,     inv_retail_type, updt_time,     inv_cost,   inv_wg");
            sb.Append(" ) VALUES ( ");
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_id));       // inv_id
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(lineNumber));        // inv_entry_no
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_pcode));      // inv_pcode
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_item_nm));    // inv_item_nm
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_item_dsc));   // inv_item_dsc

            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(price));           // inv_rate
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(quantity));            // inv_qty
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_qty_per_case));   // inv_qty_per_case
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_unit_pkg));       // inv_unit_pkg
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_item_loc));       // inv_item_loc

            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_tax_fl));     // inv_tax_fl
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_pkg_dsc));    // inv_pkg_dsc
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_sp_fl));      // inv_sp_fl
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_lot));      // inv_lot
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_3_dim));      // inv_3_dim

            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_item_type));  // inv_item_type
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_bo_qty));      // inv_bo_qty
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_entry_lnk));        // inv_entry_lnk
            sb.AppendFormat(" {0},", "getDate()");//DBValueConvert.ToDBString(inv_item_date));        // inv_item_date
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(itemNumber));        // inv_item_no


            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_adj_no));      // inv_adj_no
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_remark));        // inv_remark
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(inv_retail_type));     // inv_retail_type
            sb.AppendFormat(" {0},", "getDate()");                                         // updt_time
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(inv_cost));        // inv_cost
            sb.AppendFormat(" {0}", DBValueConvert.ToDBNumber(inv_wg));     // inv_wg

            sb.Append(") ");
            return sb.ToString();
        }

        public string UpdateInvoiceDetail(decimal inv_id, string refNumber, string invoiceNumber, string itemNumber, string price, string quantity, string lineNumber)
        {
            //查询prodtable数据
            //object inv_qty_per_case = null, inv_entry_lnk = null, inv_adj_no = null, inv_cost = null;
            string inv_pcode = "", inv_item_nm = "", inv_item_dsc = "", inv_unit_pkg = "", inv_tax_fl = "", inv_pkg_dsc = "", inv_sp_fl = "", inv_3_dim = "",
                inv_item_type = "", inv_item_date = "", inv_remark = "", inv_retail_type = "", inv_wg = "";
            int item_pk_ct = 0, inv_qty_per_case = 0;
            //decimal inv_bo_qty = 0;
            string c_item_dsc = "";

            string custSql = string.Format("select p_code,proname,cpronam,item_pk_ct,c_item_dsc = a.mes_short_dsc,sale_tax,c_pkg_dsc = b.mes_short_dsc," +
                "inv_sp_fl = (CASE WHEN item_type = 'I' THEN 'N' ELSE 'Y' END),pkg_3_dim,item_type,amt_type,item_gw,pkg_ct" +
                " from dba.prodtable,dba.t_measure_dsc a,dba.t_measure_dsc b where item_no = '{0}' and item_dsc *= a.mes_dsc and pkg_dsc *= b.mes_dsc", itemNumber);
            DataSet custDS = DbHelperOdbc.Query(custSql);
            if (custDS.Tables[0].Rows.Count > 0)
            {
                DataTable dt = custDS.Tables[0];
                inv_pcode = dt.Rows[0]["p_code"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["p_code"]);
                inv_item_nm = dt.Rows[0]["proname"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["proname"]);
                inv_item_dsc = dt.Rows[0]["cpronam"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["cpronam"]);
                item_pk_ct = dt.Rows[0]["item_pk_ct"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["item_pk_ct"]);
                c_item_dsc = dt.Rows[0]["c_item_dsc"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["c_item_dsc"]);
                inv_tax_fl = dt.Rows[0]["sale_tax"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["sale_tax"]);
                inv_pkg_dsc = dt.Rows[0]["c_pkg_dsc"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["c_pkg_dsc"]);
                inv_sp_fl = dt.Rows[0]["inv_sp_fl"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["inv_sp_fl"]);
                inv_3_dim = dt.Rows[0]["pkg_3_dim"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["pkg_3_dim"]);
                inv_item_type = dt.Rows[0]["item_type"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["item_type"]);
                inv_retail_type = dt.Rows[0]["amt_type"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["amt_type"]);
                inv_wg = dt.Rows[0]["item_gw"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["item_gw"]);
                inv_qty_per_case = dt.Rows[0]["pkg_ct"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["pkg_ct"]);

                inv_unit_pkg = Convert.ToString(item_pk_ct) + " " + c_item_dsc;
            }
            ///////////////////

            int inv_item_loc = getCompanyLoc();

            StringBuilder sb = new StringBuilder();
            sb.Append(" UPDATE DBA.t_invoice_detail  SET ");
            
            sb.AppendFormat(" inv_pcode = {0},", DBValueConvert.ToDBString(inv_pcode));      // inv_pcode
            sb.AppendFormat(" inv_item_nm = {0},", DBValueConvert.ToDBString(inv_item_nm));    // inv_item_nm
            sb.AppendFormat(" inv_item_dsc = {0},", DBValueConvert.ToDBString(inv_item_dsc));   // inv_item_dsc
            sb.AppendFormat(" inv_rate = {0},", DBValueConvert.ToDBNumber(price));           // inv_rate
            sb.AppendFormat(" inv_qty = {0},", DBValueConvert.ToDBNumber(quantity));            // inv_qty

            sb.AppendFormat(" inv_qty_per_case = {0},", DBValueConvert.ToDBNumber(inv_qty_per_case));            // inv_qty_per_case
            sb.AppendFormat(" inv_unit_pkg = {0},", DBValueConvert.ToDBString(inv_unit_pkg));       // inv_unit_pkg
            sb.AppendFormat(" inv_item_loc = {0},", DBValueConvert.ToDBNumber(inv_item_loc));       // inv_item_loc
            sb.AppendFormat(" inv_tax_fl = {0},", DBValueConvert.ToDBString(inv_tax_fl));     // inv_tax_fl
            sb.AppendFormat(" inv_pkg_dsc = {0},", DBValueConvert.ToDBString(inv_pkg_dsc));    // inv_pkg_dsc

            sb.AppendFormat(" inv_sp_fl = {0},", DBValueConvert.ToDBString(inv_sp_fl));      // inv_sp_fl
            sb.AppendFormat(" inv_3_dim = {0},", DBValueConvert.ToDBNumber(inv_3_dim));      // inv_3_dim
            sb.AppendFormat(" inv_item_type = {0},", DBValueConvert.ToDBString(inv_item_type));  // inv_item_type
            sb.AppendFormat(" inv_item_no = {0},", DBValueConvert.ToDBString(itemNumber));        // inv_item_no
            sb.AppendFormat(" inv_retail_type = {0},", DBValueConvert.ToDBString(inv_retail_type));     // inv_retail_type
            sb.AppendFormat(" updt_time = {0},", "getDate()");                                      // updt_time
            sb.AppendFormat(" inv_wg = {0}", DBValueConvert.ToDBNumber(inv_wg));     // inv_wg

            sb.Append(" WHERE inv_id = " + DBValueConvert.ToDBNumber(inv_id) + " AND inv_entry_no = " + DBValueConvert.ToDBNumber(lineNumber));
            return sb.ToString();
        }

        

        public object SaveQuoteDetail(decimal qo_id, string refNumber, string invoiceNumber, string itemNumber, string price, string quantity, string lineNumber)
        {
            string table = "DBA.t_quote_detail";
            string where = "WHERE qo_id = " + DBValueConvert.ToDBNumber(qo_id) + " AND qo_entry_no = " + DBValueConvert.ToDBNumber(lineNumber);

            return CreateSaveSQL(table, where,
                InsertQuoteDetail( qo_id,  refNumber,  invoiceNumber,  itemNumber,  price,  quantity,  lineNumber ),
                UpdateQuoteDetail(qo_id, refNumber, invoiceNumber, itemNumber, price, quantity, lineNumber));
        }

        private string UpdateQuoteDetail(decimal qo_id, string refNumber, string invoiceNumber, string itemNumber, string price, string quantity, string lineNumber)
        {
            //查询prodtable数据
            //object qo_qty_per_case = null, qo_entry_lnk = null, qo_auth_fl = null, qo_org_rate = null, qo_org_qty = null, qo_inv_rate = null, qo_inv_qty = null;
            string qo_pcode = "", qo_item_nm = "", qo_item_dsc = "", qo_unit_pkg = "", qo_tax_fl = "", qo_pkg_dsc = "", qo_sp_fl = "", qo_3_dim = "",
                qo_item_type = "", qo_retail_type = "", qo_wg = "";
            int item_pk_ct = 0, qo_qty_per_case = 0;
            string c_item_dsc = "";

            string custSql = string.Format("select p_code,proname,cpronam,item_pk_ct,c_item_dsc = a.mes_short_dsc,sale_tax,c_pkg_dsc = b.mes_short_dsc," +
                "qo_sp_fl = (CASE WHEN item_type = 'I' THEN 'N' ELSE 'Y' END),pkg_3_dim,item_type,amt_type,item_gw,pkg_ct" +
                " from dba.prodtable,dba.t_measure_dsc a,dba.t_measure_dsc b where item_no = '{0}' and item_dsc *= a.mes_dsc and pkg_dsc *= b.mes_dsc", itemNumber);
            DataSet custDS = DbHelperOdbc.Query(custSql);
            if (custDS.Tables[0].Rows.Count > 0)
            {
                DataTable dt = custDS.Tables[0];
                qo_pcode = dt.Rows[0]["p_code"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["p_code"]);
                qo_item_nm = dt.Rows[0]["proname"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["proname"]);
                qo_item_dsc = dt.Rows[0]["cpronam"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["cpronam"]);
                item_pk_ct = dt.Rows[0]["item_pk_ct"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["item_pk_ct"]);
                c_item_dsc = dt.Rows[0]["c_item_dsc"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["c_item_dsc"]);
                qo_tax_fl = dt.Rows[0]["sale_tax"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["sale_tax"]);
                qo_pkg_dsc = dt.Rows[0]["c_pkg_dsc"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["c_pkg_dsc"]);
                qo_sp_fl = dt.Rows[0]["qo_sp_fl"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["qo_sp_fl"]);
                qo_3_dim = dt.Rows[0]["pkg_3_dim"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["pkg_3_dim"]);
                qo_item_type = dt.Rows[0]["item_type"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["item_type"]);
                qo_retail_type = dt.Rows[0]["amt_type"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["amt_type"]);
                qo_wg = dt.Rows[0]["item_gw"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["item_gw"]);
                qo_qty_per_case = dt.Rows[0]["pkg_ct"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["pkg_ct"]);

                qo_unit_pkg = Convert.ToString(item_pk_ct) + " " + c_item_dsc;
            }
            ///////////////////

            int qo_item_loc = getCompanyLoc();

            StringBuilder sb = new StringBuilder();
            sb.Append(" UPDATE DBA.t_quote_detail  set ");

            sb.AppendFormat(" qo_pcode = {0},", DBValueConvert.ToDBString(qo_pcode));      // qo_pcode
            sb.AppendFormat(" qo_item_nm = {0},", DBValueConvert.ToDBString(qo_item_nm));    // qo_item_nm
            sb.AppendFormat(" qo_item_dsc = {0},", DBValueConvert.ToDBString(qo_item_dsc));   // qo_item_dsc
            sb.AppendFormat(" qo_rate = {0},", DBValueConvert.ToDBNumber(price));           // qo_rate
            sb.AppendFormat(" qo_qty = {0},", DBValueConvert.ToDBNumber(quantity));            // qo_qty

            sb.AppendFormat(" qo_qty_per_case = {0},", DBValueConvert.ToDBNumber(qo_qty_per_case));            // qo_qty_per_case
            sb.AppendFormat(" qo_unit_pkg = {0},", DBValueConvert.ToDBString(qo_unit_pkg));     // qo_unit_pkg
            sb.AppendFormat(" qo_item_loc = {0},", DBValueConvert.ToDBNumber(qo_item_loc));       // qo_item_loc
            sb.AppendFormat(" qo_tax_fl = {0},", DBValueConvert.ToDBString(qo_tax_fl));     // qo_tax_fl
            sb.AppendFormat(" qo_pkg_dsc = {0},", DBValueConvert.ToDBString(qo_pkg_dsc));    // qo_pkg_dsc

            sb.AppendFormat(" qo_sp_fl = {0},", DBValueConvert.ToDBString(qo_sp_fl));      // qo_sp_fl
            sb.AppendFormat(" qo_3_dim = {0},", DBValueConvert.ToDBNumber(qo_3_dim));      // qo_3_dim
            sb.AppendFormat(" qo_item_type = {0},", DBValueConvert.ToDBString(qo_item_type));  // qo_item_type
            sb.AppendFormat(" qo_item_no = {0},", DBValueConvert.ToDBString(itemNumber));        // qo_item_no
            sb.AppendFormat(" qo_retail_type = {0},", DBValueConvert.ToDBString(qo_retail_type));     // qo_retail_type
            sb.AppendFormat(" qo_wg = {0}", DBValueConvert.ToDBNumber(qo_wg));             // qo_wg

            sb.Append(" WHERE qo_id = " + DBValueConvert.ToDBNumber(qo_id) + " AND qo_entry_no = " + DBValueConvert.ToDBNumber(lineNumber));
            return sb.ToString();
        }

        public string CancelInvoice(string invoiceNumber)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" UPDATE DBA.t_invoice_lst  SET ");
            sb.AppendFormat("inv_cleared_fl = {0} ", DBValueConvert.ToDBString("V"));
            sb.Append(" WHERE inv_id = " + DBValueConvert.ToDBNumber(invoiceNumber));

            return sb.ToString();
        }

        public string CancelInvoiceDetail(string inv_id,  string lineNumber)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(" UPDATE DBA.t_invoice_detail  SET ");
            sb.AppendFormat("inv_sp_fl = {0},", DBValueConvert.ToDBString("Y") );
            sb.AppendFormat("inv_lot = {0} ", DBValueConvert.ToDBNumber(0));
            sb.Append(" WHERE inv_id = " + DBValueConvert.ToDBNumber(inv_id) + " AND inv_entry_no = " + DBValueConvert.ToDBNumber(lineNumber));

            return sb.ToString();
            
        }

        private string InsertQuoteDetail(decimal qo_id, string refNumber, string invoiceNumber, string itemNumber, string price, string quantity, string lineNumber)
        {
            //查询prodtable数据
            object qo_entry_lnk = null, qo_auth_fl = null, qo_org_rate = null, qo_org_qty = null, qo_inv_rate = null, qo_inv_qty = null;
            string qo_pcode = "", qo_item_nm = "", qo_item_dsc = "", qo_unit_pkg = "", qo_tax_fl = "", qo_pkg_dsc = "", qo_sp_fl = "", qo_3_dim = "",
                qo_item_type = "", qo_retail_type = "", qo_wg = "";
            int item_pk_ct = 0, qo_qty_per_case = 0;
            string c_item_dsc = "";

            string custSql = string.Format("select p_code,proname,cpronam,item_pk_ct,c_item_dsc = a.mes_short_dsc,sale_tax,c_pkg_dsc = b.mes_short_dsc," +
                "qo_sp_fl = (CASE WHEN item_type = 'I' THEN 'N' ELSE 'Y' END),pkg_3_dim,item_type,amt_type,item_gw,pkg_ct"+
                " from dba.prodtable,dba.t_measure_dsc a,dba.t_measure_dsc b where item_no = '{0}' and item_dsc *= a.mes_dsc and pkg_dsc *= b.mes_dsc", itemNumber);
            DataSet custDS = DbHelperOdbc.Query(custSql);
            if (custDS.Tables[0].Rows.Count > 0)
            {
                DataTable dt = custDS.Tables[0];
                qo_pcode = dt.Rows[0]["p_code"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["p_code"]);
                qo_item_nm = dt.Rows[0]["proname"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["proname"]);
                qo_item_dsc = dt.Rows[0]["cpronam"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["cpronam"]);
                item_pk_ct = dt.Rows[0]["item_pk_ct"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["item_pk_ct"]);
                c_item_dsc = dt.Rows[0]["c_item_dsc"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["c_item_dsc"]);
                qo_tax_fl = dt.Rows[0]["sale_tax"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["sale_tax"]);
                qo_pkg_dsc = dt.Rows[0]["c_pkg_dsc"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["c_pkg_dsc"]);
                qo_sp_fl = dt.Rows[0]["qo_sp_fl"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["qo_sp_fl"]);
                qo_3_dim = dt.Rows[0]["pkg_3_dim"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["pkg_3_dim"]);
                qo_item_type = dt.Rows[0]["item_type"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["item_type"]);
                qo_retail_type = dt.Rows[0]["amt_type"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["amt_type"]);
                qo_wg = dt.Rows[0]["item_gw"] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0]["item_gw"]);
                qo_qty_per_case = dt.Rows[0]["pkg_ct"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["pkg_ct"]);

                qo_unit_pkg = Convert.ToString(item_pk_ct) + " " + c_item_dsc;
            }
            ///////////////////

            int qo_item_loc = getCompanyLoc();

            StringBuilder sb = new StringBuilder();
            sb.Append(" INSERT INTO DBA.t_quote_detail  ( ");

            sb.Append(" qo_id,     qo_entry_no,  qo_pcode,        qo_item_nm,  qo_item_dsc, ");
            sb.Append(" qo_rate,   qo_qty,       qo_qty_per_case, qo_unit_pkg, qo_item_loc, ");
            sb.Append(" qo_tax_fl, qo_pkg_dsc,   qo_sp_fl,        qo_3_dim,    qo_item_type,");
            sb.Append(" qo_entry_lnk, qo_item_no, qo_retail_type, qo_wg,       qo_auth_fl, ");
            sb.Append(" qo_org_rate, qo_org_qty, qo_inv_rate, qo_inv_qty");
            sb.Append(" ) VALUES ( ");
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_id)  );       // qo_id
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(lineNumber));        // qo_entry_no
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_pcode));      // qo_pcode
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_item_nm));    // qo_item_nm
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_item_dsc));   // qo_item_dsc

            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(price));           // qo_rate
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(quantity));            // qo_qty
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_qty_per_case));   // qo_qty_per_case
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_unit_pkg));       // qo_unit_pkg
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_item_loc));       // qo_item_loc

            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_tax_fl));     // qo_tax_fl
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_pkg_dsc));    // qo_pkg_dsc
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_sp_fl));      // qo_sp_fl
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_3_dim));      // qo_3_dim
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_item_type));  // qo_item_type

            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_entry_lnk));      // qo_entry_lnk
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(itemNumber));        // qo_item_no
            sb.AppendFormat(" {0},", DBValueConvert.ToDBString(qo_retail_type));     // qo_retail_type
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_wg));             // qo_wg
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_auth_fl));        // qo_auth_fl

            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_org_rate));      // qo_org_rate
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_org_qty));        // qo_org_qty
            sb.AppendFormat(" {0},", DBValueConvert.ToDBNumber(qo_inv_rate));     // qo_inv_rate
            sb.AppendFormat(" {0}", DBValueConvert.ToDBNumber(qo_inv_qty));        // qo_inv_qty

            sb.Append(") ");
            return sb.ToString();
        }

        public string InsertState(string cmr ,string tran_no,string date,string due_date,string total,string str_paid, string discount,string balance)
        {
            if (string.IsNullOrEmpty(DBValueConvert.ToDBDate(date)))
                return "";

            string type = "", dsc ="";

            decimal dec_total = 0;
            if (decimal.TryParse(total,out dec_total))
            {
                 
                if (dec_total == 0)
                {
                    return "";
                }
                else if (dec_total < 0)
                {
                    type = "C";
                }
                else
                {
                    type = "I";
                }

            }
            else
            {
                return "";
            }

            decimal paid = 0;
            decimal dec_balance = 0;
            if (decimal.TryParse(balance, out dec_balance))
            {
                paid = Math.Abs( dec_total) - Math.Abs(dec_balance);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(" INSERT INTO DBA.t_cmr_statement ( ");
            sb.Append(" cs_id,cs_cmr,cs_type,cs_tran_no,cs_date,cs_due_date, cs_total,cs_paid  ");
            sb.Append(" ) VALUES ( ");
            sb.AppendFormat("{0},", "null");
            sb.AppendFormat("{0},", DBValueConvert.ToDBString(cmr)  );
            sb.AppendFormat("{0},", DBValueConvert.ToDBString(type)  );
            sb.AppendFormat("{0},", DBValueConvert.ToDBNumber(tran_no)  );
            sb.AppendFormat("{0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(date))  );
            sb.AppendFormat("{0},", DBValueConvert.ToDBString(DBValueConvert.ToDBDate(due_date)) );
            sb.AppendFormat("{0},", DBValueConvert.ToDBNumber(Math.Abs(dec_total)));
            sb.AppendFormat("{0}", DBValueConvert.ToDBNumber(paid));
            sb.Append(")");

            return sb.ToString();
        }

  

    }
}
