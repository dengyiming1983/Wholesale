using AddressParser;
using CSharpNameParser;
using CsvHelper;
using Notifications;
using ServiceUitls.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SummitDataService
{

    /*
     * build 0005 导出quote文件
     * build 0006 导出文件合并成一个文件。并且不需要输出header
     * 
     */

    class DataSynchronizer
    {
        
        private const string TAG = "DataSynchronizer";

        Thread process;
        private bool stopSync;

        private bool isDebug = false;
        
        private GlobalConfiguration config = null;

        /// <summary>
        /// 同步周期间隔时间（秒为单位）
        /// </summary>
        private int TimerIntervalOfSync = 10;

        Hashtable updateProductLst = new Hashtable();
        Hashtable updateProductOtherInfoLst = new Hashtable();

        ArrayList SQLStringList = new ArrayList();

        Dictionary<string, string> measureMap = new Dictionary<string, string>();
        Dictionary<string, string> brandMap = new Dictionary<string, string>();
        Dictionary<string, string> keyCodeMap = new Dictionary<string, string>();

        // do_type
        Dictionary<int, string> typeMap = new Dictionary<int, string>() {
            { 1,"ITEM" }, // item
            { 2,"VENDOR" }, // vendor
            { 3,"CUSTOMER" }, // customer
            { 4,"INVERTORY" }, // invertory
        };

        WholesaleDBManager dbmanager = new WholesaleDBManager();
        private bool IsProcessing = false;

        string error_file_path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\error_data";


        public delegate void EventHandler(object sender);
        public delegate void ToastMessageHandler(object sender, InfoEventArgs e);
        public event EventHandler workstart;
        public event EventHandler workstop;
        public event ToastMessageHandler ToastMessage;
		
		string display = "Read: {0}   Write: {1}";

        public void loadConnectionConfig()
        {
            config = GlobalConfiguration.Load();

            string odbc_conStr = String.Format("Dsn={0};Uid={1};Pwd={2}",
            config.odbc_dsn,
            config.odbc_usrid,
            config.odbc_pwd);

            DbHelperOdbc.connectionString = odbc_conStr;

            TimerIntervalOfSync = config.interval;

        }

        public void onStartup()
        {
            //if (!Directory.Exists(error_file_path))
            //{
            //    Directory.CreateDirectory(error_file_path);
            //}

            loadConnectionConfig();

            process = new Thread(doSyncData);
            process.Start();

        }

        public void onShutDown()
        {
            stopSync = true;

            if (process != null && process.ThreadState == ThreadState.Running)
            {
                process.Abort();
            }

            process = null;
        }


        private void doSyncData()
        {
            stopSync = false;

            try
            {
                LoadWholesaleMeasure();

                LoadWholesaleBrand();
            }
            catch (Exception ex)
            {
                if (config.isWriteLog)
                {
                    Logger.error(TAG, ex.Message);
                }
            }
           

            while (!stopSync)
            {
                try
                {
                   
                    //导出数据
                    doExportData();

                   
                }
                catch (Exception ex)
                {
                    if (config.isWriteLog)
                    {
                        Logger.error(TAG, ex.Message);
                    }
                }

               
                try
                {
                    SQLStringList.Clear();

                    //出现问题记录收集器
                    //构建csv文件
                    //string csvFile = error_file_path + string.Format(@"\{0}_{1}.csv", "error", "items");

                    //if (!Directory.Exists(error_file_path))
                    //{
                    //    Directory.CreateDirectory(error_file_path);
                    //}

                    //if (File.Exists(csvFile))
                    //{
                    //    File.Delete(csvFile);
                    //}

                    //TextWriter streamWriter = new StreamWriter(csvFile);
                    //CsvHelper.CsvWriter CsvWriter = new CsvHelper.CsvWriter(streamWriter);

                    //CsvWriter.Configuration.QuoteAllFields = true;
                    //CsvWriter.Configuration.HasHeaderRecord = true;


                    //
                    bool reloadBrand = false;

                    //string dir = Properties.Settings.Default.importFolderOfItem;
                    string dir = config.importFolderOfItem;

                    //InfoEventArgs msgEventArgs = new InfoEventArgs("","");

                    DirectoryInfo di = new DirectoryInfo(dir);
                    // FileInfo[] files = di.GetFiles("*.txt");

                    var files = di.GetFiles("*.*")
                        .Where(s => s.Extension.ToLower() == ".txt" || s.Extension.ToLower() == ".csv");

                    if (workstart != null)
                        workstart(this);

                    IsProcessing = true;


                    foreach (var item in files)
                    {
                      
                        keyCodeMap.Clear();
                        updateProductLst.Clear();
                        updateProductOtherInfoLst.Clear();

                        FileInfo csvfile = item;

						string orgName = csvfile.Name;
                        string fileNameNoExtension = Path.GetFileNameWithoutExtension(csvfile.Name).ToUpper();

                        if (    fileNameNoExtension.StartsWith("ITEM")
                                ||  fileNameNoExtension.StartsWith("CUST")
                                ||  fileNameNoExtension.StartsWith("ORDER")
                                ||  fileNameNoExtension.StartsWith("STAT")
                            )
                        {
                            //重命名
                            if (!isDebug)
                                csvfile.Rename(csvfile.Name + ".tmp");

                            
                            Console.WriteLine("正在读取CSV文件:" + csvfile.FullName);

                            //string msg = "";

                            //string filename = csvfile.Name;

                            //string path = csvfile.DirectoryName;
                            //string pp = csvfile.Directory.Parent.Parent.Name;

                            //int showlenght = 15;

                            ////if (path.Length > showlenght)
                            ////     path = "..." + path.Substring(path.Length - showlenght);

                            //path = "..." + path.Replace(pp, "");

                            //msg = "Reading " + path +"\\"+ filename;

                            if (ToastMessage != null)
                                ToastMessage(this, new InfoEventArgs("Importing " + orgName, ""));


                            //switch (fileNameNoExtension)
                            //{
                            //    case "ITEM":
                            //        //
                            //        DoImportItem(csvfile);
                            //        break;
                            //    case "CUST":
                            //        //
                            //        DoImportCustomer(csvfile);
                            //        break;

                            //    default:
                            //        break;
                            //}

                            try
                            {

                                if (fileNameNoExtension.StartsWith("ITEM"))
                                {
                                    DoImportItem(csvfile);
                                }
                                else if (fileNameNoExtension.StartsWith("CUST"))
                                {
                                    DoImportCustomer(csvfile);
                                }
                                else if (fileNameNoExtension.StartsWith("ORDER"))
                                {
                                    DoImportOrder(csvfile);
                                }
                                else if (fileNameNoExtension.StartsWith("STAT"))
                                {
                                    DoImportState(csvfile);
                                }
                            }
                            catch (Exception ex)
                            {
                                //发生异常，把tmp文件改回原来的名字
                                csvfile.Rename(orgName);
                                throw ex;
                            }

                            
                        }

                    }

                }
                catch (Exception ex)
                {
                    if (config.isWriteLog)
                    {
                        Logger.error(TAG, ex.Message);
                    }
                }
                finally
                {
                    if (workstop != null)
                        workstop(this);

                    IsProcessing = false;
                }

                Thread.Sleep(TimerIntervalOfSync * 1000);
            }
        }

        internal bool IsProcessingData()
        {
            //
            return IsProcessing;
        }

        private void InsertProduct(string upc,decimal price, string imagePath, int station,string name, string desc)
        {
            // insert or update product  插入或者更新一个新的产品
            StringBuilder sb = new StringBuilder();
            sb.Append("/* insert or update product -> upc: " + upc +" */");
            sb.Append("INSERT INTO oc_product ");
            sb.Append("(product_id, model, sku, upc, ean,");
            sb.Append("jan, isbn, mpn, location, quantity,");
            sb.Append("stock_status_id, image, manufacturer_id, shipping, price, ");
            sb.Append("points, tax_class_id, date_available, weight, weight_class_id,length, ");
            sb.Append("width, height, length_class_id, subtract, minimum, ");
            sb.Append("sort_order, status, viewed, date_added, date_modified) ");
            sb.Append("VALUES ");
            sb.Append("( null, ?upc, '', '', '', ");
            sb.Append("'','','','',0, ");
            sb.Append("0,?image,0,0, ?price, ");
            sb.Append("0, 0, '', 0, 0, 0,");
            sb.Append("0, 0, 0, 0, 0,");
            sb.Append("0, 0, 0,?date_added , '' )");
            sb.Append("ON DUPLICATE KEY UPDATE ");
            sb.Append("model = ?upc, image = ?image, price = ?price ");

            // ?upc ?image ?price ?date_added(当前时间)
            OdbcParameter[] parameters = {
                                new OdbcParameter("?upc", OdbcType.VarChar),
                                new OdbcParameter("?image", OdbcType.VarChar),
                                new OdbcParameter("?price", OdbcType.Decimal),
                                new OdbcParameter("?date_added", OdbcType.DateTime)
            //new MySqlParameter("?date_available", MySqlDbType.DateTime),
           // new MySqlParameter("?date_modified", MySqlDbType.DateTime),
            };
            
            parameters[0].Value = upc;
            parameters[1].Value = imagePath;
            parameters[2].Value = price;
            parameters[3].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //parameters[4].Value = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd HH:mm:ss"); // date_available
            //parameters[5].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//date_modified

            updateProductLst.Add(sb.ToString(), parameters);

            //产品和store之间建立关联
            StringBuilder to_store = new StringBuilder();
            to_store.Append("/* update oc_product_to_store -> upc: " + upc + " */");
            to_store.Append("INSERT INTO oc_product_to_store (product_id, store_id) ");
            to_store.Append("VALUES ");
            to_store.Append("((SELECT product_id FROM oc_product WHERE model = ?upc ), ?store_id) ");
            to_store.Append("ON DUPLICATE KEY UPDATE ");
            to_store.Append("product_id = (SELECT product_id FROM oc_product WHERE model = ?upc ), store_id = ?store_id ");


            OdbcParameter[] product_to_store_parameters = {
                                new OdbcParameter("?upc", OdbcType.VarChar),
                                new OdbcParameter("?store_id", OdbcType.Int)};

            product_to_store_parameters[0].Value = upc;
            product_to_store_parameters[1].Value = 0; // store default 0

            updateProductOtherInfoLst.Add(to_store.ToString(), product_to_store_parameters);

        }

        /// <summary>
        /// 同步Product记录
        /// </summary>
        /// <param name="upc"></param>
        /// <param name="price"></param>
        /// <param name="imagePath"></param>
        /// <param name="productIsExists"></param>
        private void SyncProduct(string upc, decimal price, string imagePath, bool productIsExists)
        {
            // ?upc ?image ?price ?date_added(当前时间)
            OdbcParameter[] parameters = {
                                    new OdbcParameter("?upc", OdbcType.VarChar),
                                    new OdbcParameter("?image", OdbcType.VarChar),
                                    new OdbcParameter("?price", OdbcType.Decimal),
                                    new OdbcParameter("?date_added", OdbcType.DateTime)
                                //new MySqlParameter("?date_available", MySqlDbType.DateTime),
                               // new MySqlParameter("?date_modified", MySqlDbType.DateTime),
                                };

            parameters[0].Value = upc;
            parameters[1].Value = imagePath;
            parameters[2].Value = price;
            parameters[3].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //parameters[4].Value = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd HH:mm:ss"); // date_available
            //parameters[5].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//date_modified

            if (!productIsExists)
            {
                StringBuilder insertSQL = new StringBuilder();
                insertSQL.Append("/* insert product -> upc: " + upc + " */");
                insertSQL.Append("INSERT INTO oc_product ");
                insertSQL.Append("(product_id, model, sku, upc, ean,");
                insertSQL.Append("jan, isbn, mpn, location, quantity,");
                insertSQL.Append("stock_status_id, image, manufacturer_id, shipping, price, ");
                insertSQL.Append("points, tax_class_id, date_available, weight, weight_class_id,length, ");
                insertSQL.Append("width, height, length_class_id, subtract, minimum, ");
                insertSQL.Append("sort_order, status, viewed, date_added, date_modified) ");
                insertSQL.Append("VALUES ");
                insertSQL.Append("( null, ?upc, '', '', '', ");
                insertSQL.Append("'','','','',0, ");
                insertSQL.Append("0,?image,0,0, ?price, ");
                insertSQL.Append("0, 0, '', 0, 0, 0,");
                insertSQL.Append("0, 0, 0, 0, 0,");
                insertSQL.Append("0, 0, 0,?date_added , '' )");

                updateProductLst.Add(insertSQL.ToString(), parameters);
            }
            else
            {
                StringBuilder updateSQL = new StringBuilder();
                updateSQL.Append("/* update product -> upc: " + upc + " */");
                updateSQL.Append("UPDATE oc_product SET ");
                updateSQL.Append("image = ?image, price = ?price ");
                updateSQL.Append("WHERE model = ?upc ");
                updateProductLst.Add(updateSQL.ToString(), parameters);
            }
        }

        //
        /// <summary>
        /// 同步产品的name和description
        /// </summary>
        private void SyncProductNameAndDesc(string upc, bool hasName, string name,bool hasDesc, string desc)
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

                updateProductOtherInfoLst.Add(merge_name_sql.ToString() + " /* update name */ ", parameters4Name);

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

                updateProductOtherInfoLst.Add(merge_name_sql.ToString() + " /* update description */ ", parameters4desc);
            }
        }

        /// <summary>
        /// 获取market数据中的存在的 p_code
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private string GetProductCode(string barcode)
        {
            /*
            IF NOT EXISTS(SELECT 1 FROM DBA.prodtable WHERE barcode = '000006901234')
            BEGIN
                SELECT 'product is not exists ' as res
            END
            ELSE
            BEGIN
                DECLARE @product_code varchar(30)
                SELECT @product_code = p_code FROM DBA.prodtable WHERE barcode = '000006901234'
                SELECT @product_code as res
            END
            */
            StringBuilder sb = new StringBuilder();
            sb.Append("IF NOT EXISTS(SELECT 1 FROM DBA.prodtable WHERE barcode = '" + barcode + "') ");
            sb.Append("BEGIN ");
            sb.Append("    SELECT 'product is not exists ' as res ");
            sb.Append("END ");
            sb.Append("ELSE ");
            sb.Append("BEGIN");
            sb.Append("    DECLARE @product_code varchar(30) ");
            sb.Append("    SELECT @product_code = p_code FROM DBA.prodtable WHERE barcode = '" + barcode + "' ");
            sb.Append("    SELECT @product_code as res");
            sb.Append("END ");

            return Convert.ToString(DbHelperOdbc.ExecuteScalar(sb.ToString()));

        }
        

        public void LoadWholesaleMeasure()
        {
            string sql = "SELECT * FROM DBA.t_measure_dsc ";
            DataSet ds = DbHelperOdbc.Query(sql);

            if (measureMap != null)
                measureMap.Clear();

            if (ds.Tables.Count > 0 )
            {
                int cnt = ds.Tables[0].Rows.Count;

                if (cnt  > 0)
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        string mes_dsc = Convert.ToString( ds.Tables[0].Rows[i]["mes_dsc"]);
                        string mes_short_dsc = Convert.ToString( ds.Tables[0].Rows[i]["mes_short_dsc"]);

                        measureMap.Add(mes_dsc, mes_short_dsc);
                    }
                }
                
            }
        }

        public void LoadWholesaleBrand()
        {
            string sql = "SELECT * FROM DBA.t_brand_lst ";
            DataSet ds = DbHelperOdbc.Query(sql);

            if (brandMap !=null)
                brandMap.Clear();

            if (ds.Tables.Count > 0)
            {
                int cnt = ds.Tables[0].Rows.Count;

                if (cnt > 0)
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        string bnd_name = Convert.ToString(ds.Tables[0].Rows[i]["bnd_name"]);
                        string bnd_id = Convert.ToString(ds.Tables[0].Rows[i]["bnd_id"]);

                        brandMap.Add(bnd_name, bnd_id);
                    }
                }

            }
        }

        public void ParsePack(string pack)
        {
            //pack.
        }


        /// <summary>
        /// 此函数为测试导出
        /// 从数据库中导出数据，格式如下
        /// Data Mappings              lenght 
        ///  1 - Item #                 (5)
        ///  2 - Description            (25)
        ///  3 - Brand                  (10)
        ///  4 - Pack                   (10)
        ///  5 - Qty on hand            (5)
        ///  6 - Gross weight           (5)
        ///  7 - Price A                (6)
        ///  8 - Price B                (6)
        ///  9 - Price C                (6)
        /// 10 - FOB                    (6)
        /// 11 - Cost                   (6)
        /// 12 - Vendor #               (4)
        /// 13 - Memo                   (15)
        /// 14 - Chinese Description    (30)
        /// 15 - UPC code               (15)
        /// 16 - Related Item Code      (6)
        /// 17 - Record type            (1)
        /// </summary>
        /// <returns></returns>
        public string ExportItemAsCSVFile()
        {
            string name = "";

            //string outputFolder = Properties.Settings.Default.ExportFolderOfItem;
            string outputFolder = config.exportFolderOfItem;

            //构建csv文件
            string csvFile = outputFolder + string.Format(@"\{0}_{1}.csv", name, "items");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            if (File.Exists(csvFile))
            {
                File.Delete(csvFile);
            }

            TextWriter streamWriter = new StreamWriter(csvFile);
            CsvHelper.CsvWriter csv = new CsvHelper.CsvWriter(streamWriter);

            csv.Configuration.QuoteAllFields = true;
            csv.Configuration.HasHeaderRecord = true;

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            // sb.Append("prodtable.item_no    AS ItemNumber, ");
            sb.Append("prodtable.smart_link    AS ItemNumber, ");
            sb.Append("prodtable.proname    AS Description, ");
            sb.Append("t_brand_lst.bnd_name AS Brand, ");
            sb.Append("t_summit_measure.sm_mes AS Pack, ");
            sb.Append("t_item_aval.al_qty   AS QtyOnHand, ");
            sb.Append("prodtable.item_gw    AS  GrossWeight, ");
            sb.Append("prodtable.p1         AS PriceA, ");
            sb.Append("prodtable.p2         AS PriceB, ");
            sb.Append("prodtable.p3         AS PriceC, ");
            sb.Append("prodtable.item_fob   AS FOB, ");
            sb.Append("prodtable.item_cost  AS Cost, ");
            sb.Append("prodtable.vdr_p_code AS VendorNumber, ");
            sb.Append("t_item_memo.im_memo  AS Memo, ");
            sb.Append("prodtable.cpronam    AS ChineseDescription, ");
            sb.Append("prodtable.barcode    AS UPCCode, ");
            sb.Append("prodtable.p_code     AS RelatedItemCode, ");
            sb.Append("'1'                  AS RecordType ");
            sb.Append("FROM DBA.prodtable ");
            sb.Append("LEFT JOIN DBA.t_brand_lst ON prodtable.item_brand = t_brand_lst.bnd_id ");
            sb.Append("LEFT JOIN DBA.t_item_memo ON prodtable.p_code = t_item_memo.im_pcode ");
            sb.Append("LEFT JOIN DBA.t_summit_measure ON prodtable.p_code = t_summit_measure.sm_pcode ");
            sb.Append("LEFT JOIN DBA.t_item_aval ON prodtable.p_code = t_item_aval.al_item ");

            //string sql = "";

            DataSet ds = DbHelperOdbc.Query(sb.ToString());

            if (ds.Tables.Count > 0)
            {
                int cnt = ds.Tables[0].Rows.Count;

                if (cnt > 0)
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        //string mes_dsc = Convert.ToString(ds.Tables[0].Rows[i]["mes_dsc"]);
                        //string mes_short_dsc = Convert.ToString(ds.Tables[0].Rows[i]["mes_short_dsc"]);

                        //measureMap.Add(mes_dsc, mes_short_dsc);


                        string ItemNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["ItemNumber"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ItemNumber"]);
                        string Description = Convert.IsDBNull(ds.Tables[0].Rows[i]["Description"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Description"]);
                        string Brand = Convert.IsDBNull(ds.Tables[0].Rows[i]["Brand"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Brand"]);
                        string Pack = Convert.IsDBNull(ds.Tables[0].Rows[i]["Pack"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Pack"]);
                        string QtyOnHand = Convert.IsDBNull(ds.Tables[0].Rows[i]["QtyOnHand"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["QtyOnHand"]);
                        string GrossWeight = Convert.IsDBNull(ds.Tables[0].Rows[i]["GrossWeight"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["GrossWeight"]);
                        string PriceA = Convert.IsDBNull(ds.Tables[0].Rows[i]["PriceA"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PriceA"]);
                        string PriceB = Convert.IsDBNull(ds.Tables[0].Rows[i]["PriceB"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PriceB"]);
                        string PriceC = Convert.IsDBNull(ds.Tables[0].Rows[i]["PriceC"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PriceC"]);
                        string FOB = Convert.IsDBNull(ds.Tables[0].Rows[i]["FOB"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FOB"]);
                        string Cost = Convert.IsDBNull(ds.Tables[0].Rows[i]["Cost"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Cost"]);
                        
                        string VendorNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["VendorNumber"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["VendorNumber"]);
                        string Memo = Convert.IsDBNull(ds.Tables[0].Rows[i]["Memo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Memo"]);
                        string ChineseDescription = Convert.IsDBNull(ds.Tables[0].Rows[i]["ChineseDescription"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ChineseDescription"]);
                        string UPCCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["UPCCode"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["UPCCode"]);
                        string RelatedItemCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["RelatedItemCode"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RelatedItemCode"]);
                        string RecordType = Convert.IsDBNull(ds.Tables[0].Rows[i]["RecordType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RecordType"]);
                        


                        if (Description.Length > 25)
                            Description = Description.Substring(0, 25);

                        if (!string.IsNullOrEmpty(GrossWeight))
                            GrossWeight = Convert.ToString(Math.Truncate (Convert.ToDecimal(GrossWeight) * 100));

                        if (!string.IsNullOrEmpty(PriceA))
                            PriceA = Convert.ToString(Math.Truncate(Convert.ToDecimal(PriceA) * 100));

                        if (!string.IsNullOrEmpty(PriceB))
                            PriceB = Convert.ToString(Math.Truncate(Convert.ToDecimal(PriceB) * 100));

                        if (!string.IsNullOrEmpty(PriceC))
                            PriceC = Convert.ToString(Math.Truncate(Convert.ToDecimal(PriceC) * 100));

                        if (!string.IsNullOrEmpty(FOB))
                            FOB = Convert.ToString(Math.Truncate(Convert.ToDecimal(FOB) * 100));

                        if (!string.IsNullOrEmpty(Cost))
                            Cost = Convert.ToString(Math.Truncate(Convert.ToDecimal(Cost) * 100));

                        if (Memo.Length > 15)
                            Memo = Memo.Substring(0,15);

                        if (ChineseDescription.Length > 30)
                            ChineseDescription = ChineseDescription.Substring(0, 30);

                        ItemNumber = ItemNumber.PadRight(5);
                        Description = Description.PadRight(25);
                        Brand = Brand.PadRight(10);
                        //Pack = Pack.PadRight(10);
                        QtyOnHand = QtyOnHand.PadLeft(5, '0');
                        GrossWeight = GrossWeight.PadLeft(5, '0');
                        PriceA = PriceA.PadLeft(6, '0');
                        PriceB = PriceB.PadLeft(6, '0');
                        PriceC = PriceC.PadLeft(6, '0');
                        FOB = FOB.PadLeft(6, '0');
                        Cost = Cost.PadLeft(6, '0');
                        VendorNumber = VendorNumber.PadLeft(4, '0');
                        Memo = Memo.PadRight(15);
                        //ChineseDescription = ChineseDescription.PadRight(30);
                        ChineseDescription.PadRightEx(30);
                        UPCCode = UPCCode.PadRight(15);
                        RelatedItemCode = RelatedItemCode.PadRight(6);

                        csv.WriteField(ItemNumber);
                        csv.WriteField(Description);
                        csv.WriteField(Brand);
                        csv.WriteField(Pack);
                        csv.WriteField(QtyOnHand);

                        csv.WriteField(GrossWeight);
                        csv.WriteField(PriceA);
                        csv.WriteField(PriceB);
                        csv.WriteField(PriceC);
                        csv.WriteField(FOB);

                        csv.WriteField(Cost);
                        csv.WriteField(VendorNumber);
                        csv.WriteField(Memo);
                        csv.WriteField(ChineseDescription);
                        csv.WriteField(UPCCode);

                        csv.WriteField(RelatedItemCode);
                        csv.WriteField(RecordType);

                        csv.NextRecord();
                    }
                }

            }

            csv.Dispose();

            return csvFile;
        }


        public void doExportData()
        {
            //获取是否有需要导出数据
            if (hasOutPutData())
            {
                if (workstart != null)
                    workstart(this);

                IsProcessing = true;

                List<int> types = GetOutoutDataList();

                foreach (var item in types)
                {
                    //导出t_summit_data_out中的数据

                    switch (item)
                    {
                        case 1:
                            DoOutputItem4K();
                            break;
                        case 2:
                            //DoOutputVendor();
                            break;
                        case 3:
                            DoOutputCustomer();
                            break;
                        case 4:
                            //DoOutputInvertory();
                            break;
                        case 5:
                            DoOutputQuoteInfo();
                            break;

                        default:
                            break;
                    }

                }

                if (workstop != null)
                    workstop(this);

                IsProcessing = false;

            }

        }

        private bool hasOutPutData()
        {
            string sql = "SELECT count(1) FROM DBA.t_summit_data_out ";

            return Convert.ToInt32(DbHelperOdbc.ExecuteScalar(sql)) > 0 ? true : false;
        }

        private List<int> GetOutoutDataList()
        {
            List<int> columnData = new List<int>();

            string sql = "SELECT DISTINCT do_type FROM DBA.t_summit_data_out ";

            DataSet ds =  DbHelperOdbc.Query(sql);

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    columnData.Add(Convert.ToInt32( dr["do_type"]));
                }
            }

            return columnData;
            
        }

        private void DoImportItem(FileInfo csvfile)
        {
            string csvPath = csvfile.FullName;

            TextReader streamReader = new StreamReader(csvPath, Encoding.Default);
            //TextReader streamReader = new StreamReader(csvPath, Encoding.GetEncoding("gb2312")); //测试使用gb2312导入文件是乱码
            
            var csv = new CsvReader(streamReader);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.QuoteAllFields = true;

            Console.WriteLine("Item File 开始读取分析文件...");

            //var map = csv.Configuration.AutoMap<KickStartEntryMap>();
            csv.Configuration.RegisterClassMap<SummitElementItemMap>();

            var records = csv.GetRecords<SummitElementItem>();

            long completed = 0;
            long read = 0;
			long write = 0;

            try
            {
                foreach (SummitElementItem record in records)
                //while (csv.Read())
                {
                    // build 0007
                    //record.SetEncoding(fileEncoding);

                    if (record.RecordType.Trim() != "0")
                        continue;

                    SQLStringList.Clear();

                    read++;

                    if (ToastMessage != null)
                        ToastMessage(this, new InfoEventArgs("", string.Format(display, read.ToString("N0"), write.ToString("N0"))));

                    //var record = csv.GetRecord<SummitElement2>();

                    string brandID = "";
                    string insertBrand = "";

                    string brand = record.Brand.Trim();
                    string brandName = record.BrandName;

                    brand = brand.Replace(" ", "");

                    Console.WriteLine("Brand -> " + brand);

                    //检查是否有该品牌数据
                    //if (Properties.Settings.Default.isInsertItems)
                    if (config.isInsertItems)
                    {
                        if (!string.IsNullOrEmpty(brand))
                        {
                            if (!brandMap.ContainsKey(brand))
                            {
                                int maxBrandId = dbmanager.GetMaxBrandID();

                                maxBrandId++;

                                //加入品牌数据列表
                                insertBrand = dbmanager.InsertBrand(maxBrandId, brand, brandName, "", "");

                                //插入品牌数据库
                                int cntBrand = DbHelperOdbc.ExecuteSql(insertBrand);

                                if (cntBrand > 0)
                                {
                                    brandID = Convert.ToString(maxBrandId);
                                    brandMap.Add(brand, brandID);
                                }

                            }
                            else
                            {
                                brandID = brandMap[brand];
                            }
                        }
                    }

                    string productCode = record.RelatedItemCode.Trim();
                    if (string.IsNullOrEmpty(productCode))
                        continue;

                    if (!keyCodeMap.ContainsKey(record.RelatedItemCode))
                    {
                        keyCodeMap.Add(record.RelatedItemCode, "New");
                    }
                    else
                    {
                        Console.WriteLine("Key重复了 -> " + record.RelatedItemCode);
                        continue;
                    }

                    string unicodeString = record.ChineseDescription;


                    //byte[] bs = Encoding.GetEncoding("UTF-8").GetBytes(unicodeString);
                    //bs = Encoding.Convert(Encoding.GetEncoding("UTF-8"), Encoding.GetEncoding("ANSI"), bs);
                    //string decodedString = Encoding.GetEncoding("ANSI").GetString(bs);


                    //插入产品信息
                    string insertProdtable = "";

                    //if (Properties.Settings.Default.isInsertItems)
                    if(config.isInsertItems)
                    {
                        insertProdtable = dbmanager.InsertProdtable(
                                record.RelatedItemCode,
                                record.UPCCode,
                                record.ItemNumber,
                                record.Description,
                                record.ChineseDescription,
                                brandID,
                                record.FOB,
                                record.Cost,
                                record.GrossWeight,
                                record.PriceA,
                                record.PriceB,
                                record.PriceC,
                                record.VendorNumber
                        );

                        string saveProdtableSQL = dbmanager.CreateSaveSQL("DBA.prodtable", "WHERE p_code = '" + record.RelatedItemCode + "'", insertProdtable, "");

                        //DbHelperOdbc.ExecuteSql(saveProdtableSQL);
                        SQLStringList.Add(saveProdtableSQL);
                    }



                    //SQLStringList.Add(saveProdtableSQL);

                    //记录有问题数据
                    //int cnt = DbHelperOdbc.ExecuteSql(saveProdtableSQL);

                    //if (cnt <= 0)
                    //{
                    //    CsvWriter.WriteField(record.ItemNumber);
                    //    CsvWriter.WriteField(record.Description);
                    //    CsvWriter.WriteField(record.Brand);
                    //    CsvWriter.WriteField(record.Pack);
                    //    CsvWriter.WriteField(record.QuantityOnHand);
                    //    CsvWriter.WriteField(record.GrossWeight);
                    //    CsvWriter.WriteField(record.PriceA);
                    //    CsvWriter.WriteField(record.PriceB);
                    //    CsvWriter.WriteField(record.PriceC);
                    //    CsvWriter.WriteField(record.FOB);
                    //    CsvWriter.WriteField(record.Cost);
                    //    CsvWriter.WriteField(record.VendorNumber);
                    //    CsvWriter.WriteField(record.Memo);
                    //    CsvWriter.WriteField(record.ChineseDescription);
                    //    CsvWriter.WriteField(record.UPCCode);
                    //    CsvWriter.WriteField(record.RelatedItemCode);
                    //    CsvWriter.WriteField(record.RecordType);
                    //    CsvWriter.NextRecord();
                    //    continue;
                    //}
                    Console.WriteLine("Key:" + record.RelatedItemCode + " For Prodtable. -> Completed.");
                    //Console.WriteLine("--> " + saveProdtableSQL);

                    //插入memo信息
                    if (!string.IsNullOrEmpty(record.Memo))
                    {
                        //if (Properties.Settings.Default.isInsertItems)
                        if (config.isInsertItems)
                        {
                            string insertMemo = "";
                            string saveMemoSQL = "";

                            insertMemo = dbmanager.InsertMemo(record.RelatedItemCode, record.Memo);

                            saveMemoSQL = dbmanager.CreateSaveSQL("DBA.t_item_memo", "WHERE im_pcode = '" + record.RelatedItemCode + "'", insertMemo, "");

                            // DbHelperOdbc.ExecuteSql(saveMemoSQL);
                            SQLStringList.Add(saveMemoSQL);

                            Console.WriteLine("Key:" + record.RelatedItemCode + " For t_item_memo. -> Completed.");
                        }
                    }


                    //插入measure信息
                    string packDesc = record.Pack.Trim();

                    if (!string.IsNullOrEmpty(packDesc))
                    {
                        //if (Properties.Settings.Default.isInsertItems)
                        if (config.isInsertItems)
                        {
                            string insertSummitMeasure = dbmanager.InsertSummitMeasure(record.RelatedItemCode, packDesc);

                            string saveSummitMeasure = dbmanager.CreateSaveSQL("DBA.t_summit_measure", "WHERE sm_pcode= '" + record.RelatedItemCode + "'", insertSummitMeasure, "");

                            // DbHelperOdbc.ExecuteSql(saveSummitMeasure);
                            SQLStringList.Add(saveSummitMeasure);

                            Console.WriteLine("Key:" + record.RelatedItemCode + " For t_summit_measure. -> Completed.");
                        }
                    }


                    //Console.WriteLine("loop record item number -> " + record.ItemNumber);

                    //处理qty on hand对t_item_loc表插入数据
                    //当插入新数据的时候，对lot_no,lot_date,lot_cost,lot_qty,lot_remain数据进行填充
                    //qty on hand -> lot_qty和lot_remain
                    //当更新数据的时候，对t_item_lot.lot_qty 和 t_item_lot_loc.ll_qty进行更新
                    //qty on hand -> t_item_lot.lot_qty 和 t_item_lot_loc.ll_qty
                    if (!string.IsNullOrEmpty(record.QuantityOnHand))
                    {
                        int qty = Convert.ToInt32(record.QuantityOnHand);

                        //int item_lot_id = dbmanager.GetItemLotMaxID();

                        //item_lot_id++;
                        int item_lot_id = 99; //2017-08-30 特定要求 Kevin: 固定为99

                        int item_lot_loc_no = 1;

                        string insertItemLot = "";

                        //if (Properties.Settings.Default.isInsertItems)
                        if (config.isInsertItems)
                            insertItemLot = dbmanager.InsertItemLot(item_lot_id, item_lot_loc_no, record.RelatedItemCode, qty, record.Cost);

                        string updateLot = dbmanager.UpdateItemLotLoc(item_lot_id, item_lot_loc_no, record.RelatedItemCode, qty, record.Cost);

                        string update_inv_sql = dbmanager.CreateSaveSQL("DBA.t_item_lot", "WHERE lot_item='" + record.RelatedItemCode + "'  AND lot_no =" + DBValueConvert.ToDBNumber(item_lot_id), insertItemLot, updateLot);
                        //try
                        //{
                        //    DbHelperOdbc.ExecuteSql(update_inv_sql);
                        //}
                        //catch (Exception ex)
                        //{

                        //    throw ex;
                        //}

                        SQLStringList.Add(update_inv_sql);

                        Console.WriteLine("Key:" + record.RelatedItemCode + " For t_item_lot -> Completed. ");

                    }

                    if (SQLStringList.Count > 0)
                    {
                        Console.WriteLine("开始对数据库进行更新操作...");

                        DateTime beforDT = System.DateTime.Now;

                        DbHelperOdbc.ExecuteSqlTran(SQLStringList);

                        DateTime afterDT = System.DateTime.Now;

                        TimeSpan ts = afterDT.Subtract(beforDT);

                        Console.WriteLine("批量执行更新数据总共花费 {0}ms. ({1}sec.)", ts.TotalMilliseconds, ts.TotalSeconds);

                    }

                    completed++;
                    write++;

                    Console.WriteLine("Key:" + record.RelatedItemCode + " For item. -> Completed.   Completed:" + completed);

                    if (ToastMessage != null)
                        ToastMessage(this, new InfoEventArgs("", string.Format(display, read.ToString("N0"), write.ToString("N0"))));

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                csv.Configuration.UnregisterClassMap<SummitElementItemMap>();
                //parser.Dispose();
                csv.Dispose();
                streamReader.Dispose();
            }
           
         

            Console.WriteLine("Item File 分析文件完成...");

            if (csvfile.Exists)
            {
                //完成后删除文件
                csvfile.Delete();
            }

            if (ToastMessage != null)
                ToastMessage(this, new InfoEventArgs("Import item data completed.", string.Format(display, read.ToString("N0"), write.ToString("N0"))));

        }

        private void DoImportCustomer(FileInfo csvfile)
        {
            string csvPath = csvfile.FullName;

            //出现问题记录收集器
            //构建error File文件
            //string errorFile = error_file_path + string.Format(@"\{0}_{1}.csv", "error", "Customer");

            //if (!Directory.Exists(error_file_path))
            //{
            //    Directory.CreateDirectory(error_file_path);
            //}

            //if (File.Exists(errorFile))
            //{
            //    File.Delete(errorFile);
            //}

            //TextWriter streamWriter = new StreamWriter(errorFile);
            //CsvHelper.CsvWriter CsvWriter = new CsvHelper.CsvWriter(streamWriter);

            //CsvWriter.Configuration.QuoteAllFields = true;
            //CsvWriter.Configuration.HasHeaderRecord = true;

            //
            //TextReader streamReader = new StreamReader(csvPath, Encoding.Default);
            TextReader streamReader = new StreamReader(csvPath, Encoding.Default);
            var csv = new CsvReader(streamReader);
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.QuoteAllFields = true;

            Console.WriteLine("Customer File 开始读取分析文件...");

            csv.Configuration.RegisterClassMap<SummitElementCustomerMap>();

            var records = csv.GetRecords<SummitElementCustomer>();

            //long totalCnt = records.Count();
            long completed = 0;
            long read = 0;
			long write = 0;

            try
            {
                foreach (SummitElementCustomer record in records)
                {
                    // build 0007
                    //record.SetEncoding(currentEncoding);

                    if (record.RecordType.Trim() != "0")
                        continue;

                    SQLStringList.Clear();

                    bool isMarkRecord = false;

                    read++;

                    if (ToastMessage != null)
                        ToastMessage(this, new InfoEventArgs("", string.Format(display, read.ToString("N0"), write.ToString("N0"))));


                    //插入客户信息
                    //string saveProdtableSQL = dbmanager.CreateSaveSQL("DBA.prodtable", "WHERE p_code = '" + record.RelatedItemCode + "'", insertProdtable, "");

                    var parser = new AddressParser.AddressParser();

                    // 解释bill地址
                    string address = "";
                    string city = "";
                    string state = "";
                    string zip = "";

                    string billTo123 = string.Format("{0} {1} {2}", record.BillTo1.Trim(),
                                                                    record.BillTo2.Trim(),
                                                                    record.BillTo3.Trim());

                    string billto = billTo123.Trim();

                    if (!string.IsNullOrEmpty(billto))
                    {
                        //var bill_address_res = parser.ParseAddress(billto);

                        string newBillTo1 = record.BillTo1.Trim();
                        string newBillTo2 = record.BillTo2.Trim();
                        string newBillTo3 = record.BillTo3.Trim();

                        newBillTo2 = fixState(newBillTo2);
                        newBillTo3 = fixState(newBillTo3);

                        AddressParseResult bill_address_res = null;

                        if (HaveZipCode(newBillTo2))
                        {
                            newBillTo2 = fixHaveZipCode(newBillTo2);

                            address = newBillTo1 + " " + newBillTo3;
                            address = address.Trim();

                            bill_address_res = parser.ParseAddress(address + " " + newBillTo2);
                        }
                        else if (HaveZipCode(newBillTo3))
                        {
                            newBillTo3 = fixHaveZipCode(newBillTo3);

                            address = newBillTo1 + " " + newBillTo2;
                            address = address.Trim();

                            bill_address_res = parser.ParseAddress(address + " " + newBillTo3);
                        }
                        else 
                        {
                            string last = "";
                            int len = 0;
                            if (newBillTo2.Length > 2)
                            {
                               last = newBillTo2.Substring(newBillTo2.Length -2, 2);
                               if (AddressParser.AddressParser.GetStates().ContainsValue(last))
                                {
                                    state = last;

                                    len = newBillTo2.Length - 2;
                                    city = newBillTo2.Substring(0, len);
                                }

                            }

                            if (newBillTo3.Length > 2)
                            {
                                last = newBillTo3.Substring(newBillTo3.Length - 2, 2);
                                if (AddressParser.AddressParser.GetStates().ContainsValue(last))
                                {
                                    state = last;

                                    len = newBillTo3.Length - 2;
                                    city = newBillTo3.Substring(0, len);
                                }
                               
                            }
                        }

                        if (bill_address_res != null)
                        {
                            //address = bill_address_res.StreetLine;
                            city = bill_address_res.City;
                            state = bill_address_res.State;
                            zip = bill_address_res.Zip;
                        }
                        else
                        {
                            address = billto;

                            isMarkRecord = true;
                        }
                     
                    }


                    // 解析ship地址
                    string ship_name = "";
                    string ship_cname = "";
                    string ship_address = "";
                    string ship_city = "";
                    string ship_state = "";
                    string ship_zip = "";

                    string shipTo123 = string.Format("{0} {1} {2}", record.ShipTo1.Trim(),
                                                                   record.ShipTo2.Trim(),
                                                                   record.ShipTo3.Trim());
                    string shipto = shipTo123.Trim();

                    if (!string.IsNullOrEmpty(shipto))
                    {

                        AddressParseResult ship_address_res = null;

                        if (shipto.ToUpper() == "SAME")
                        {
                            ship_name = record.Name;
                            ship_cname = record.ChineseName;
                            ship_address = address;
                            ship_city = city;
                            ship_state = state;
                            ship_zip = zip;

                        }
                        else
                        {
                            string newShipTo1 = record.ShipTo1.Trim();
                            string newShipTo2 = record.ShipTo2.Trim();
                            string newShipTo3 = record.ShipTo3.Trim();


                            if (HaveZipCode(newShipTo2))
                            {
                                newShipTo2 = fixState(newShipTo2);

                                newShipTo2 = fixHaveZipCode(newShipTo2);

                                ship_address = newShipTo1 + " " + newShipTo3;
                                ship_address = ship_address.Trim();

                                ship_address_res = parser.ParseAddress(ship_address + " " + newShipTo2);
                            }
                            else if (HaveZipCode(newShipTo3))
                            {
                                newShipTo3 = fixState(newShipTo3);

                                newShipTo3 = fixHaveZipCode(newShipTo3);

                                ship_address = newShipTo1 + " " + newShipTo2;
                                ship_address = ship_address.Trim();

                                ship_address_res = parser.ParseAddress(ship_address + " " + newShipTo3);
                            }
                            else
                            {
                                string last = "";
                                int len = 0;
                                if (newShipTo2.Length > 2)
                                {
                                    last = newShipTo2.Substring(newShipTo2.Length - 2, 2);
                                    if (AddressParser.AddressParser.GetStates().ContainsValue(last))
                                    {
                                        ship_city = last;

                                        len = newShipTo2.Length - 2;
                                        ship_city = newShipTo2.Substring(0, len);
                                    }
                                        
                                }

                                if (newShipTo3.Length > 2)
                                {
                                    last = newShipTo3.Substring(newShipTo3.Length - 2, 2);
                                    if (AddressParser.AddressParser.GetStates().ContainsValue(last))
                                    {
                                        ship_state = last;

                                        len = newShipTo3.Length - 2;
                                        ship_city = newShipTo3.Substring(0, len);
                                    }
                                  
                                }
                            }

                            if (ship_address_res != null)
                            {
                                //ship_address = ship_address_res.StreetLine;
                                ship_city = ship_address_res.City;
                                ship_state = ship_address_res.State;
                                ship_zip = ship_address_res.Zip;
                            }
                            else
                            {
                                ship_address = shipto;

                                isMarkRecord = true;
                            }

                            
                        }

                       

                    }

                    //if (isMarkRecord)
                    //    WriteErrorRecord(CsvWriter, record);


                    string insertCustomer = "";
                    string insertCustomerBalance = ""; /* build 0008 added */

                    //if (Properties.Settings.Default.isInsertCustomer)
                    if (config.isInsertCustomer)
                    {
                        insertCustomer = dbmanager.InsertCustomer(
                                        record.AcctNumber, record.Name, address, city, state,
                                        zip, record.Phone, record.Contact, record.Email, record.Balance,
                                        record.ChineseName, record.Region,
                                        ship_name, ship_cname, ship_address, ship_city, ship_state, ship_zip, record.SMNumber
                                        );

                        /* build 0008 added */
                        insertCustomerBalance = dbmanager.InsertCustomerBalance(
                                record.AcctNumber,
                                record.CurrentBalance,
                                record.v30DaysBalance,
                                record.v60DaysBalance,
                                record.v90DaysBalance
                            );

                    }

                    //string insertEmpl = dbmanager.InsertEmployee();
                    string updateCustomer = dbmanager.UpdateCustomer(record.AcctNumber, record.Balance);

                    /* build 0008 added */
                    string updateCustomerBalance = dbmanager.UpdateCustomerBalance(
                            record.AcctNumber,
                            record.CurrentBalance,
                            record.v30DaysBalance,
                            record.v60DaysBalance,
                            record.v90DaysBalance
                        );

                    SQLStringList.Add(
                            dbmanager.CreateSaveSQL("DBA.client", "WHERE ccode = '" + record.AcctNumber + "'", insertCustomer, updateCustomer)
                     );

                    /* build 0008 added */
                    SQLStringList.Add(
                           dbmanager.CreateSaveSQL("DBA.t_cmr_baln", "WHERE ccode = '" + record.AcctNumber + "'", insertCustomerBalance, updateCustomerBalance)
                    );

                    // salesman字段解析插入employee的表
                    if (!string.IsNullOrEmpty(record.SMNumber) &&
                        !string.IsNullOrEmpty(record.Salesman))
                    {
                        string firstName = "";
                        string lastName = "";

                        Name salemanName = new NameParser().Parse(record.Salesman);

                        firstName = salemanName.FirstName;
                        lastName = salemanName.LastName;

                        string insertEmpl = "";

                        //if (Properties.Settings.Default.isInsertCustomer)
                        if (config.isInsertCustomer)
                            insertEmpl = dbmanager.InsertEmployee(record.SMNumber, firstName, lastName);

                        //string updateEmpl = dbmanager.UpdateEmployee(record.SMNumber, firstName, lastName);

                        SQLStringList.Add(
                             //dbmanager.CreateSaveSQL("DBA.t_empl_profile", "WHERE empl_no = " + record.SMNumber, insertEmpl, updateEmpl)
                             dbmanager.CreateSaveSQL("DBA.t_empl_profile", "WHERE empl_no = " + record.SMNumber, insertEmpl, "")
                        );
                    }

                    //DbHelperOdbc.ExecuteSql(saveCustomer);


                    if (SQLStringList.Count > 0)
                    {
                        Console.WriteLine("开始对数据库进行更新操作...");

                        DateTime beforDT = System.DateTime.Now;

                        DbHelperOdbc.ExecuteSqlTran(SQLStringList);

                        DateTime afterDT = System.DateTime.Now;

                        TimeSpan ts = afterDT.Subtract(beforDT);

                        Console.WriteLine("批量执行更新数据总共花费 {0}ms. ({1}sec.)", ts.TotalMilliseconds, ts.TotalSeconds);

                    }

                    completed++;

                    write++;

                    if (ToastMessage != null)
                        ToastMessage(this, new InfoEventArgs("", string.Format(display, read.ToString("N0"), write.ToString("N0"))));


                    Console.WriteLine("Key:" + record.AcctNumber + " For client. -> Completed.   Completed:" + completed);

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {

                //CsvWriter.Dispose();// debug
                //streamWriter.Dispose();// debug

                csv.Configuration.UnregisterClassMap<SummitElementCustomerMap>();
                //parser.Dispose();
                csv.Dispose();
                streamReader.Dispose();
            }

            Console.WriteLine("Customer File 分析文件完成...");

            if (!isDebug)
            {
                if (csvfile.Exists)
                {
                    //完成后删除文件
                    csvfile.Delete();
                }
            }


            if (ToastMessage != null)
                ToastMessage(this, new InfoEventArgs("Import customer data completed.", string.Format(display, read.ToString("N0"), write.ToString("N0"))));
        }


        private void DoImportOrder(FileInfo csvfile)
        {
            string csvPath = csvfile.FullName;

            TextReader streamReader = new StreamReader(csvPath, Encoding.Default);
            var csv = new CsvReader(streamReader);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.QuoteAllFields = true;

            Console.WriteLine("Customer File 开始读取分析文件...");


            //csv.Configuration.RegisterClassMap<SummitElementMapOrder>();

            //var records = csv.GetRecords<SummitElementOrder>();

            long completed = 0;
            long read = 0;
            long write = 0;

            string dataFrom = "";
            decimal auto_id = 0;
            bool isOwnOrder = false;
            string status = "";

            try
            {

                while (csv.Read())
                {
                   
                    SQLStringList.Clear();

                    read++;

                    if (ToastMessage != null)
                        ToastMessage(this, new InfoEventArgs("", string.Format(display, read.ToString("N0"), write.ToString("N0"))));

                    var type = csv.GetField<string>(0);

                    
                    switch (type)
                    {
                        case "H":
                            var order = csv.GetRecord<SummitElementOrder>();
                            dataFrom = order.Source;
                            status = order.Status;

                            if (order.Status == "0")
                            {
                                isOwnOrder = true;
                                 
                            }
                            else
                            {
                                isOwnOrder = false;
                            }

                            if (!isOwnOrder)
                            {
                                //插入order记录
                                switch (dataFrom)
                                {
                                    case "1":

                                        switch (status)
                                        {
                                            case "1": // 1-Packed 
                                                //用 ref #查找quote进行更新
                                                   SQLStringList.Add(dbmanager.UpdateQuoteForPacked( 
                                                           order.AccountNumber,
                                                           order.PONumber,
                                                           order.Status,
                                                           order.RefNumber,
                                                           order.InvoiceNumber,
                                                           order.Salesman,
                                                           Convert.ToDecimal(order.OrderTotal),
                                                           order.ShipVia,
                                                           order.OrderDate,
                                                           order.DeliveryDate,
                                                           order.Message
                                                        ));
                                                
                                                break;
                                            case "2": // 2-Invoiced 
                                                      //用 invoice# 查找 t_invoice_lst是否存在invoice，存在update ,不存在insert
                                                      // file.ref# --> t_invoice_lst.inv_qo_no

                                                    if (!string.IsNullOrEmpty(order.RefNumber))
                                                    {
                                                        SQLStringList.Add(
                                                          dbmanager.SaveInvoice(
                                                          order.AccountNumber,
                                                          order.PONumber,
                                                          order.Status,
                                                          order.RefNumber,
                                                          order.InvoiceNumber,
                                                          order.Salesman,
                                                          Convert.ToDecimal(order.OrderTotal),
                                                          order.ShipVia,
                                                          order.OrderDate,
                                                          order.DeliveryDate,
                                                          order.Message,
                                                          order.ShiptoAdrs
                                                           )
                                                       );
                                                    }
                                                
                                                break;
                                            case "9": // 9-Canceled
                                                      // 用 file.invoice #和 t_invoice_lst.inv_id是否匹配，存在就set inv_cleared_fl = 'V', 不存在nothing to do
                                                if (!string.IsNullOrEmpty(order.InvoiceNumber))
                                                {
                                                    SQLStringList.Add(
                                                     dbmanager.CancelInvoice(order.InvoiceNumber)
                                                      );
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                        //auto_id = dbmanager.GetMaxQuoteID();

                                        //SQLStringList.Add(

                                        //       dbmanager.SaveQuote(auto_id,
                                        //       order.AccountNumber,
                                        //       order.PONumber,
                                        //       order.Status,
                                        //       order.RefNumber,
                                        //       order.InvoiceNumber,
                                        //       order.Salesman,
                                        //       Convert.ToDecimal(order.OrderTotal),
                                        //       order.ShipVia,
                                        //       order.OrderDate,
                                        //       order.DeliveryDate,
                                        //       order.Message,
                                        //       order.ShiptoAdrs
                                        //            )
                                        //        );
                                        break;
                                    //case "2":
                                    //    auto_id = dbmanager.GetMaxInvoiceID();

                                    //    SQLStringList.Add(
                                    //           dbmanager.SaveInvoice(auto_id,
                                    //           order.AccountNumber,
                                    //           order.PONumber,
                                    //           order.Status,
                                    //           order.RefNumber,
                                    //           order.InvoiceNumber,
                                    //           order.Salesman,
                                    //           Convert.ToDecimal(order.OrderTotal),
                                    //           order.ShipVia,
                                    //           order.OrderDate,
                                    //           order.DeliveryDate,
                                    //           order.Message,
                                    //           order.ShiptoAdrs
                                    //                    )
                                    //                );
                                    //    break;
                                    default:
                                        break;
                                }
                            }
                           
                            break;
                        case "L": /* line item */
                            var order_line_item = csv.GetRecord<SummitElementOrderLineItem>();

                            if (!isOwnOrder)
                            {
                                switch (dataFrom)
                                {
                                    case "1":
                                        switch (status)
                                        {
                                            case "1": // 1-Packed 
                                                      // 用 ref #查找quote进行更新
                                                if (!string.IsNullOrEmpty(order_line_item.RefNumber))
                                                {
                                                    SQLStringList.Add(
                                                        dbmanager.SaveQuoteDetail(
                                                            Convert.ToDecimal(order_line_item.RefNumber),
                                                            order_line_item.RefNumber,
                                                            order_line_item.InvoiceNumber,
                                                            order_line_item.ItemNumber,
                                                            order_line_item.Price,
                                                            order_line_item.Quantity,
                                                            order_line_item.LineNumber
                                                           )
                                                       );
                                                }
                                                

                                                break;
                                            case "2": // 2-Invoiced 
                                                      // 用 invoice# 查找 t_invoice_lst是否存在invoice，存在update ,不存在insert
                                                      // file.ref# --> t_invoice_lst.inv_qo_no

                                                if (!string.IsNullOrEmpty(order_line_item.RefNumber))
                                                {
                                                    SQLStringList.Add(
                                                      dbmanager.SaveInvoiceDetail(
                                                           Convert.ToDecimal(order_line_item.RefNumber),
                                                           order_line_item.RefNumber,
                                                           order_line_item.InvoiceNumber,
                                                           order_line_item.ItemNumber,
                                                           order_line_item.Price,
                                                           order_line_item.Quantity,
                                                           order_line_item.LineNumber
                                                          )
                                                      );
                                                }

                                                break;
                                            case "9": // 9-Canceled
                                                      // 用 file.invoice #和 t_invoice_lst.inv_id是否匹配，存在就set inv_cleared_fl = 'V', 不存在nothing to do
                                                if (!string.IsNullOrEmpty(order_line_item.InvoiceNumber))
                                                {
                                                    SQLStringList.Add(
                                                     dbmanager.CancelInvoiceDetail(order_line_item.InvoiceNumber,
                                                             order_line_item.LineNumber
                                                             )
                                                      );
                                                }
                                                break;
                                            default:
                                                break;
                                        }

                                        //SQLStringList.Add(
                                        //   dbmanager.SaveQuoteDetail(auto_id,
                                        //   order_line_item.RefNumber,
                                        //   order_line_item.InvoiceNumber,
                                        //   order_line_item.ItemNumber,
                                        //   order_line_item.Price,
                                        //   order_line_item.Quantity,
                                        //   order_line_item.LineNumber
                                        //       )
                                        //   );
                                        break;
                                    //case "2":
                                    //    SQLStringList.Add(
                                    //      dbmanager.SaveInvoiceDetail(
                                    //          auto_id,
                                    //       order_line_item.RefNumber,
                                    //       order_line_item.InvoiceNumber,
                                    //       order_line_item.ItemNumber,
                                    //       order_line_item.Price,
                                    //       order_line_item.Quantity,
                                    //       order_line_item.LineNumber
                                    //          )
                                    //      );
                                    //    break;
                                    default:
                                        break;
                                }
                            }
                            

                            break;
                        default:
                            break;
                    }

                    if (SQLStringList.Count > 0)
                    {
                        Console.WriteLine("开始对数据库进行更新操作...");

                        DateTime beforDT = System.DateTime.Now;

                        DbHelperOdbc.ExecuteSqlTran(SQLStringList);

                        DateTime afterDT = System.DateTime.Now;

                        TimeSpan ts = afterDT.Subtract(beforDT);

                        Console.WriteLine("批量执行更新数据总共花费 {0}ms. ({1}sec.)", ts.TotalMilliseconds, ts.TotalSeconds);

                    }

                    completed++;

                    write++;

                    if (ToastMessage != null)
                        ToastMessage(this, new InfoEventArgs("", string.Format(display, read.ToString("N0"), write.ToString("N0"))));


                }

                //foreach (SummitElementOrder record in records)
                //{
                //    SQLStringList.Clear();

                //    read++;


                //    ///在此写代码

                //    completed++;

                //    write++;

                //    if (ToastMessage != null)
                //        ToastMessage(this, new InfoEventArgs("", string.Format(display, read.ToString("N0"), write.ToString("N0"))));


                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }
             finally
            {

                //CsvWriter.Dispose();// debug
                //streamWriter.Dispose();// debug

                csv.Configuration.UnregisterClassMap<SummitElementCustomerMap>();
                //parser.Dispose();
                csv.Dispose();
                streamReader.Dispose();
            }

            Console.WriteLine("Customer File 分析文件完成...");

            if (!isDebug)
            {
                if (csvfile.Exists)
                {
                    //完成后删除文件
                    csvfile.Delete();
                }
            }


            if (ToastMessage != null)
                ToastMessage(this, new InfoEventArgs("Import Order data completed.", string.Format(display, read.ToString("N0"), write.ToString("N0"))));

        }


        private void DoImportState(FileInfo csvfile)
        {
            string csvPath = csvfile.FullName;

            TextReader streamReader = new StreamReader(csvPath, Encoding.Default);
            var csv = new CsvReader(streamReader);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.QuoteAllFields = true;

            Console.WriteLine("State File 开始读取分析文件...");

            csv.Configuration.RegisterClassMap<SummitElementStateMap>();

            var records = csv.GetRecords<SummitElementState>();

            long completed = 0;
            long read = 0;
            long write = 0; 

            try
            {
                 
                foreach (SummitElementState record in records)
                {
                    SQLStringList.Clear();

                    read++;
                    
                    SQLStringList.Add(
                                     dbmanager.InsertState(
                                         record.Cmr,
                                         record.cs_tran_no,
                                         record.cs_date,
                                         record.cs_due_date,
                                         record.total,
                                         record.paid,
                                         record.discount,
                                         record.balance

                                         )
                                     );

                    if (SQLStringList.Count > 0)
                    {
                        Console.WriteLine("开始对数据库进行更新操作...");

                        DateTime beforDT = System.DateTime.Now;

                        DbHelperOdbc.ExecuteSqlTran(SQLStringList);

                        DateTime afterDT = System.DateTime.Now;

                        TimeSpan ts = afterDT.Subtract(beforDT);

                        Console.WriteLine("批量执行更新数据总共花费 {0}ms. ({1}sec.)", ts.TotalMilliseconds, ts.TotalSeconds);

                    }

                    completed++;

                    write++;

                    if (ToastMessage != null)
                        ToastMessage(this, new InfoEventArgs("", string.Format(display, read.ToString("N0"), write.ToString("N0"))));


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                 
                csv.Configuration.UnregisterClassMap<SummitElementStateMap>();
            
                csv.Dispose();
                streamReader.Dispose();
            }

            Console.WriteLine("Customer File 分析文件完成...");

            if (!isDebug)
            {
                if (csvfile.Exists)
                {
                    //完成后删除文件
                    csvfile.Delete();
                }
            }


            if (ToastMessage != null)
                ToastMessage(this, new InfoEventArgs("Import state data completed.", string.Format(display, read.ToString("N0"), write.ToString("N0"))));

        }


        private string fixState(string line)
        {
            var lst = AddressParser.AddressParser.GetStates().Values.Select(x => Regex.Replace(x, @"(\w)", @"$1."));

            //处理state
            foreach (var item in lst)
            {
                if (line.IndexOf(item) > 0)
                {
                    line = line.Replace(item, item.Replace(".", ""));
                    break;
                }
            }

            return line;
        }

        private void DoOutputItem() {

            //string name = "";
            string name = DateTime.Now.ToString("yyyyMMddHHmmss");

            //string outputFolder = Properties.Settings.Default.ExportFolderOfItem;
             string outputFolder = config.exportFolderOfItem;

            //构建csv文件
            string csvFile = outputFolder + string.Format(@"\{0}_{1}.csv", name, "items");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            if (File.Exists(csvFile))
            {
                File.Delete(csvFile);
            }

            TextWriter streamWriter = new StreamWriter(csvFile);
            CsvHelper.CsvWriter csv = new CsvHelper.CsvWriter(streamWriter);

            csv.Configuration.QuoteAllFields = true;
            csv.Configuration.HasHeaderRecord = true;

            //查询所有需要导出Item
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("t_summit_data_out.do_id AS ID, ");
            sb.Append("prodtable.smart_link    AS ItemNumber, ");
            sb.Append("prodtable.proname    AS Description, ");
            sb.Append("t_brand_lst.bnd_name AS Brand, ");
            sb.Append("t_summit_measure.sm_mes AS Pack, ");
            sb.Append("t_item_aval.al_qty   AS QtyOnHand, ");
            sb.Append("prodtable.item_gw    AS  GrossWeight, ");
            sb.Append("prodtable.p1         AS PriceA, ");
            sb.Append("prodtable.p2         AS PriceB, ");
            sb.Append("prodtable.p3         AS PriceC, ");
            sb.Append("prodtable.item_fob   AS FOB, ");
            sb.Append("prodtable.item_cost  AS Cost, ");
            sb.Append("prodtable.vdr_p_code AS VendorNumber, ");
            sb.Append("t_item_memo.im_memo  AS Memo, ");
            sb.Append("prodtable.cpronam    AS ChineseDescription, ");
            sb.Append("prodtable.barcode    AS UPCCode, ");
            sb.Append("prodtable.p_code     AS RelatedItemCode, ");
            sb.Append("'1'                  AS RecordType ");
            sb.Append("FROM DBA.t_summit_data_out ");
            sb.Append("INNER JOIN DBA.prodtable ON prodtable.p_code = DBA.t_summit_data_out.do_key ");
            sb.Append("LEFT JOIN DBA.t_brand_lst ON prodtable.item_brand = t_brand_lst.bnd_id ");
            sb.Append("LEFT JOIN DBA.t_item_memo ON prodtable.p_code = t_item_memo.im_pcode ");
            sb.Append("LEFT JOIN DBA.t_summit_measure ON prodtable.p_code = t_summit_measure.sm_pcode ");
            sb.Append("LEFT JOIN DBA.t_item_aval ON prodtable.p_code = t_item_aval.al_item ");
            sb.Append("WHERE do_type = 1 ");
            sb.Append("ORDER BY do_crt_time ");

            //导出完成删除Item
            DataSet ds = DbHelperOdbc.Query(sb.ToString());

            string clearSQL = "";

            if (ds.Tables.Count > 0)
            {
                int cnt = ds.Tables[0].Rows.Count;

                if (cnt > 0)
                {
                   
                    for (int i = 0; i < cnt; i++)
                    {
                        //string mes_dsc = Convert.ToString(ds.Tables[0].Rows[i]["mes_dsc"]);
                        //string mes_short_dsc = Convert.ToString(ds.Tables[0].Rows[i]["mes_short_dsc"]);

                        //measureMap.Add(mes_dsc, mes_short_dsc);


                        string ItemNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["ItemNumber"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ItemNumber"]);
                        string Description = Convert.IsDBNull(ds.Tables[0].Rows[i]["Description"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Description"]);
                        string Brand = Convert.IsDBNull(ds.Tables[0].Rows[i]["Brand"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Brand"]);
                        string Pack = Convert.IsDBNull(ds.Tables[0].Rows[i]["Pack"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Pack"]);
                        string QtyOnHand = Convert.IsDBNull(ds.Tables[0].Rows[i]["QtyOnHand"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["QtyOnHand"]);
                        string GrossWeight = Convert.IsDBNull(ds.Tables[0].Rows[i]["GrossWeight"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["GrossWeight"]);
                        string PriceA = Convert.IsDBNull(ds.Tables[0].Rows[i]["PriceA"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PriceA"]);
                        string PriceB = Convert.IsDBNull(ds.Tables[0].Rows[i]["PriceB"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PriceB"]);
                        string PriceC = Convert.IsDBNull(ds.Tables[0].Rows[i]["PriceC"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PriceC"]);
                        string FOB = Convert.IsDBNull(ds.Tables[0].Rows[i]["FOB"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FOB"]);
                        string Cost = Convert.IsDBNull(ds.Tables[0].Rows[i]["Cost"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Cost"]);

                        string VendorNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["VendorNumber"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["VendorNumber"]);
                        string Memo = Convert.IsDBNull(ds.Tables[0].Rows[i]["Memo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Memo"]);
                        string ChineseDescription = Convert.IsDBNull(ds.Tables[0].Rows[i]["ChineseDescription"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ChineseDescription"]);
                        string UPCCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["UPCCode"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["UPCCode"]);
                        string RelatedItemCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["RelatedItemCode"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RelatedItemCode"]);
                        string RecordType = Convert.IsDBNull(ds.Tables[0].Rows[i]["RecordType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RecordType"]);



                        if (Description.Length > 25)
                            Description = Description.Substring(0, 25);

                        if (!string.IsNullOrEmpty(GrossWeight))
                            GrossWeight = Convert.ToString(Math.Truncate(Convert.ToDecimal(GrossWeight) * 100));

                        if (!string.IsNullOrEmpty(PriceA))
                            PriceA = Convert.ToString(Math.Truncate(Convert.ToDecimal(PriceA) * 100));

                        if (!string.IsNullOrEmpty(PriceB))
                            PriceB = Convert.ToString(Math.Truncate(Convert.ToDecimal(PriceB) * 100));

                        if (!string.IsNullOrEmpty(PriceC))
                            PriceC = Convert.ToString(Math.Truncate(Convert.ToDecimal(PriceC) * 100));

                        if (!string.IsNullOrEmpty(FOB))
                            FOB = Convert.ToString(Math.Truncate(Convert.ToDecimal(FOB) * 100));

                        if (!string.IsNullOrEmpty(Cost))
                            Cost = Convert.ToString(Math.Truncate(Convert.ToDecimal(Cost) * 100));

                        if (Memo.Length > 15)
                            Memo = Memo.Substring(0, 15);

                        if (ChineseDescription.Length > 30)
                            ChineseDescription = ChineseDescription.Substring(0, 30);



                        ItemNumber = ItemNumber.PadRight(5);
                        Description = Description.PadRight(25);
                        Brand = Brand.PadRight(10);
                        //Pack = Pack.PadRight(10);
                        QtyOnHand = QtyOnHand.PadLeft(5, '0');
                        GrossWeight = GrossWeight.PadLeft(5, '0');
                        PriceA = PriceA.PadLeft(6, '0');
                        PriceB = PriceB.PadLeft(6, '0');
                        PriceC = PriceC.PadLeft(6, '0');
                        FOB = FOB.PadLeft(6, '0');
                        Cost = Cost.PadLeft(6, '0');
                        VendorNumber = VendorNumber.PadLeft(4, '0');
                        Memo = Memo.PadRight(15);
                        //ChineseDescription = ChineseDescription.PadRight(30);
                        ChineseDescription.PadRightEx(30);
                        UPCCode = UPCCode.PadRight(15);
                        RelatedItemCode = RelatedItemCode.PadRight(6);

                        csv.WriteField(ItemNumber);
                        csv.WriteField(Description);
                        csv.WriteField(Brand);
                        csv.WriteField(Pack);
                        csv.WriteField(QtyOnHand);

                        csv.WriteField(GrossWeight);
                        csv.WriteField(PriceA);
                        csv.WriteField(PriceB);
                        csv.WriteField(PriceC);
                        csv.WriteField(FOB);

                        csv.WriteField(Cost);
                        csv.WriteField(VendorNumber);
                        csv.WriteField(Memo);
                        csv.WriteField(ChineseDescription);
                        csv.WriteField(UPCCode);

                        csv.WriteField(RelatedItemCode);
                        csv.WriteField(RecordType);

                        csv.NextRecord();
                    }

                    long start = Convert.ToInt64(ds.Tables[0].Rows[0]["ID"]);
                    long end = Convert.ToInt64(ds.Tables[0].Rows[cnt - 1]["ID"]);

                    clearSQL = "DELETE FROM DBA.t_summit_data_out WHERE do_type = 1 AND do_id >= " + start + " AND do_id <= " + end;


                }

            }

            csv.Dispose();

            if (!string.IsNullOrEmpty(clearSQL))
            {
                DbHelperOdbc.ExecuteSql(clearSQL);
            }

            //return csvFile;

        }

        /// <summary>
        /// 导出item数据函数
        /// 2017-08-30 特定要求 Kevin: QtyOnHand不出输出，不需要补充空格。不需要根据原来导入文件整体格式。
        /// </summary>
        private void DoOutputItem4K()
        {
            //string name = "";
            string name = DateTime.Now.ToString("yyyyMMddHHmmss");

            //string outputFolder = Properties.Settings.Default.ExportFolderOfItem;
            string outputFolder = config.exportFolderOfItem;

            //构建csv文件
            string exportFile = string.Format(@"{0}_{1}.txt", "item", name);
            string csvFile = outputFolder + "\\" + exportFile;

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            if (File.Exists(csvFile))
            {
                File.Delete(csvFile);
            }

            TextWriter streamWriter = new StreamWriter(csvFile);
            CsvHelper.CsvWriter csv = new CsvHelper.CsvWriter(streamWriter);

            csv.Configuration.QuoteAllFields = true;
            csv.Configuration.HasHeaderRecord = true;

            if (ToastMessage != null)
                ToastMessage(this, new InfoEventArgs("Exporting " + exportFile, ""));


            //查询所有需要导出Item
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("t_summit_data_out.do_id AS ID, ");
            sb.Append("prodtable.smart_link    AS ItemNumber, ");
            sb.Append("prodtable.proname    AS Description, ");
            sb.Append("t_brand_lst.bnd_name AS Brand, ");
            // sb.Append("t_summit_measure.sm_mes AS Pack, ");

            /************ build 0006 *************/
            //sb.Append(" CASE ");
            //sb.Append("         WHEN t_summit_measure.sm_mes IS NULL THEN ");
            //sb.Append("             CASE ");
            //sb.Append("                 WHEN pkg_3_dim IS NULL OR pkg_ct IS NULL OR item_pk_ct IS NULL THEN '' ");
            //sb.Append("                 WHEN pkg_3_dim = 1 THEN Convert(varchar(10),pkg_ct)+' ' + pkg_dsc + ' X' + LTRIM(STR(item_pk_ct, 30, 2)) + ' ' + UPPER(mes_short_dsc) ");
            //sb.Append("                 WHEN pkg_3_dim > 1 THEN Convert(varchar(10),pkg_3_dim) +'X' + Convert(varchar(10), pkg_ct) + ' ' + pkg_dsc + ' X' + LTRIM(STR(item_pk_ct, 30, 2)) + ' ' + UPPER(mes_short_dsc) ");
            //sb.Append("             END ");
            //sb.Append("         ELSE ");
            //sb.Append("             t_summit_measure.sm_mes ");
            //sb.Append(" END AS Pack, ");
            /************************************/

            /************ build 0007 *************/
            sb.Append("  CASE ");
            sb.Append("      WHEN pkg_3_dim IS NULL OR pkg_ct IS NULL OR item_pk_ct IS NULL THEN '' ");
            sb.Append("      WHEN pkg_3_dim = 1 THEN Convert(varchar(10),pkg_ct)+' ' + pkg_mes.mes_short_dsc + ' X' + LTRIM(STR(item_pk_ct, 30, 2)) + ' ' + item_mes.mes_short_dsc ");
            sb.Append("      WHEN pkg_3_dim > 1 THEN Convert(varchar(10),pkg_3_dim) +'X' + Convert(varchar(10), pkg_ct) + ' ' + pkg_mes.mes_short_dsc + ' X' + LTRIM(STR(item_pk_ct, 30, 2)) + ' ' + item_mes.mes_short_dsc ");
            sb.Append("  END ");
            sb.Append(" AS Pack, ");
            /************************************/

            //sb.Append("t_item_aval.al_qty   AS QtyOnHand, ");
            //sb.Append("prodtable.item_gw    AS  GrossWeight, "); // build 0006
            sb.Append("ISNULL(prodtable.pk_wt, '0') + ISNULL(prodtable.pk_wt_ext, '0') AS GrossWeight, "); // build 0007
            sb.Append("prodtable.p1         AS PriceA, ");
            sb.Append("prodtable.p2         AS PriceB, ");
            sb.Append("prodtable.p3         AS PriceC, ");
            sb.Append("prodtable.item_fob   AS FOB, ");
            sb.Append("prodtable.item_cost  AS Cost, ");
            sb.Append("prodtable.vdr_p_code AS VendorNumber, ");
            sb.Append("t_item_memo.im_memo  AS Memo, ");
            sb.Append("prodtable.cpronam    AS ChineseDescription, ");
            sb.Append("prodtable.barcode    AS UPCCode, ");
            sb.Append("prodtable.p_code     AS RelatedItemCode, ");
            sb.Append("'1'                  AS RecordType ");
            sb.Append("FROM DBA.t_summit_data_out ");
            sb.Append("INNER JOIN DBA.prodtable ON prodtable.p_code = DBA.t_summit_data_out.do_key ");
            sb.Append("LEFT JOIN DBA.t_brand_lst ON prodtable.item_brand = t_brand_lst.bnd_id ");
            sb.Append("LEFT JOIN DBA.t_item_memo ON prodtable.p_code = t_item_memo.im_pcode ");
            //sb.Append("LEFT JOIN DBA.t_summit_measure ON prodtable.p_code = t_summit_measure.sm_pcode "); // build 0007 不在使用t_summit_measure导出单位信息
            //sb.Append("LEFT JOIN DBA.t_item_aval ON prodtable.p_code = t_item_aval.al_item ");


            //sb.Append("LEFT JOIN DBA.t_measure_dsc ON DBA.t_measure_dsc.mes_dsc = DBA.prodtable.item_dsc "); // build 0006 弃用

            // build 0007
            sb.Append("LEFT JOIN(SELECT mes_dsc, mes_short_dsc FROM DBA.t_measure_dsc) AS pkg_mes ON pkg_mes.mes_dsc = DBA.prodtable.pkg_dsc ");
            sb.Append("LEFT JOIN(SELECT mes_dsc, mes_short_dsc FROM DBA.t_measure_dsc) AS item_mes ON item_mes.mes_dsc = DBA.prodtable.item_dsc ");

            sb.Append("WHERE do_type = 1 ");
            sb.Append("ORDER BY do_crt_time ");

            //导出完成删除Item
            DataSet ds = DbHelperOdbc.Query(sb.ToString());

            string clearSQL = "";

            if (ds.Tables.Count > 0)
            {
                int cnt = ds.Tables[0].Rows.Count;

                if (cnt > 0)
                {
                  
                    for (int i = 0; i < cnt; i++)
                    {
                        //string mes_dsc = Convert.ToString(ds.Tables[0].Rows[i]["mes_dsc"]);
                        //string mes_short_dsc = Convert.ToString(ds.Tables[0].Rows[i]["mes_short_dsc"]);

                        //measureMap.Add(mes_dsc, mes_short_dsc);


                        string ItemNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["ItemNumber"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ItemNumber"]);
                        string Description = Convert.IsDBNull(ds.Tables[0].Rows[i]["Description"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Description"]);
                        string Brand = Convert.IsDBNull(ds.Tables[0].Rows[i]["Brand"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Brand"]);
                        string Pack = Convert.IsDBNull(ds.Tables[0].Rows[i]["Pack"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Pack"]);
                        //string QtyOnHand = Convert.IsDBNull(ds.Tables[0].Rows[i]["QtyOnHand"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["QtyOnHand"]);
                        string GrossWeight = Convert.IsDBNull(ds.Tables[0].Rows[i]["GrossWeight"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["GrossWeight"]);
                        string PriceA = Convert.IsDBNull(ds.Tables[0].Rows[i]["PriceA"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PriceA"]);
                        string PriceB = Convert.IsDBNull(ds.Tables[0].Rows[i]["PriceB"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PriceB"]);
                        string PriceC = Convert.IsDBNull(ds.Tables[0].Rows[i]["PriceC"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PriceC"]);
                        string FOB = Convert.IsDBNull(ds.Tables[0].Rows[i]["FOB"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["FOB"]);
                        string Cost = Convert.IsDBNull(ds.Tables[0].Rows[i]["Cost"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Cost"]);

                        string VendorNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["VendorNumber"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["VendorNumber"]);
                        string Memo = Convert.IsDBNull(ds.Tables[0].Rows[i]["Memo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Memo"]);
                        string ChineseDescription = Convert.IsDBNull(ds.Tables[0].Rows[i]["ChineseDescription"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ChineseDescription"]);
                        string UPCCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["UPCCode"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["UPCCode"]);
                        string RelatedItemCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["RelatedItemCode"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RelatedItemCode"]);
                        string RecordType = Convert.IsDBNull(ds.Tables[0].Rows[i]["RecordType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RecordType"]);



                        //if (Description.Length > 25)
                        //    Description = Description.Substring(0, 25);

                        if (!string.IsNullOrEmpty(GrossWeight))
                            GrossWeight = Convert.ToString(Math.Truncate(Convert.ToDecimal(GrossWeight) * 100));

                        if (!string.IsNullOrEmpty(PriceA))
                            PriceA = Convert.ToString(Math.Truncate(Convert.ToDecimal(PriceA) * 100));

                        if (!string.IsNullOrEmpty(PriceB))
                            PriceB = Convert.ToString(Math.Truncate(Convert.ToDecimal(PriceB) * 100));

                        if (!string.IsNullOrEmpty(PriceC))
                            PriceC = Convert.ToString(Math.Truncate(Convert.ToDecimal(PriceC) * 100));

                        if (!string.IsNullOrEmpty(FOB))
                            FOB = Convert.ToString(Math.Truncate(Convert.ToDecimal(FOB) * 100));

                        if (!string.IsNullOrEmpty(Cost))
                            Cost = Convert.ToString(Math.Truncate(Convert.ToDecimal(Cost) * 100));

                        //if (Memo.Length > 15)
                        //    Memo = Memo.Substring(0, 15);

                        //if (ChineseDescription.Length > 30)
                        //    ChineseDescription = ChineseDescription.Substring(0, 30);



                        //ItemNumber = ItemNumber.PadRight(5);
                        //Description = Description.PadRight(25);
                        //Brand = Brand.PadRight(10);
                        ////Pack = Pack.PadRight(10);
                       // QtyOnHand = QtyOnHand.PadLeft(5, '0');
                        GrossWeight = GrossWeight.PadLeft(5, '0');
                        PriceA = PriceA.PadLeft(6, '0');
                        PriceB = PriceB.PadLeft(6, '0');
                        PriceC = PriceC.PadLeft(6, '0');
                        FOB = FOB.PadLeft(6, '0');
                        Cost = Cost.PadLeft(6, '0');
                        VendorNumber = VendorNumber.PadLeft(4, '0');
                        //Memo = Memo.PadRight(15);
                        ////ChineseDescription = ChineseDescription.PadRight(30);
                        //ChineseDescription.PadRightEx(30);
                        //UPCCode = UPCCode.PadRight(15);
                        //RelatedItemCode = RelatedItemCode.PadRight(6);

                        csv.WriteField(ItemNumber);
                        csv.WriteField(Description);
                        csv.WriteField(Brand);
                        csv.WriteField(Pack);
                        //csv.WriteField(QtyOnHand);

                        csv.WriteField(GrossWeight);
                        csv.WriteField(PriceA);
                        csv.WriteField(PriceB);
                        csv.WriteField(PriceC);
                        csv.WriteField(FOB);

                        csv.WriteField(Cost);
                        csv.WriteField(VendorNumber);
                        csv.WriteField(Memo);
                        csv.WriteField(ChineseDescription);
                        csv.WriteField(UPCCode);

                        csv.WriteField(RelatedItemCode);
                        csv.WriteField(RecordType);

                        csv.NextRecord();

                        if (ToastMessage != null)
                            ToastMessage(this, new InfoEventArgs("", string.Format("Completed: ", i.ToString("N0"))));

                    }

                    long start = Convert.ToInt64(ds.Tables[0].Rows[0]["ID"]);
                    long end = Convert.ToInt64(ds.Tables[0].Rows[cnt - 1]["ID"]);

                    clearSQL = "DELETE FROM DBA.t_summit_data_out WHERE do_type = 1 AND do_id >= " + start + " AND do_id <= " + end;


                }

            }

            csv.Dispose();

            if (!string.IsNullOrEmpty(clearSQL))
            {
                DbHelperOdbc.ExecuteSql(clearSQL);
            }

        }



        private void DoOutputCustomer()
        {
            string name = DateTime.Now.ToString("yyyyMMddHHmmss");

            //string outputFolder = Properties.Settings.Default.ExportFolderOfItem;
            string outputFolder = config.exportFolderOfItem;

            //构建csv文件
            string exportFile = string.Format(@"{0}_{1}.txt", "cust", name);
            string csvFile = outputFolder + "\\" + exportFile;

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            if (File.Exists(csvFile))
            {
                File.Delete(csvFile);
            }

            TextWriter streamWriter = new StreamWriter(csvFile);
            CsvHelper.CsvWriter csv = new CsvHelper.CsvWriter(streamWriter);

            csv.Configuration.QuoteAllFields = true;
            csv.Configuration.HasHeaderRecord = true;

            if (ToastMessage != null)
                ToastMessage(this, new InfoEventArgs("Exporting " + exportFile, ""));


            //查询所有需要导出customer
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("t_summit_data_out.do_id AS ID, ");
            sb.Append("client.ccode AS Acct#,");
            sb.Append("client.ename AS Name,");
            sb.Append("client.cust_lz_no AS Region,");

            //sb.Append("client.addr + ' ' + client.city + ' ' + client.stat + ' ' + client.zip AS BillTo,");
            //sb.Append("client.cust_shp_addr + ' ' + client.cust_shp_city + ' ' + client.cust_shp_state + ' ' + client.cust_shp_zip AS ShipTo,");

            sb.Append("client.addr AS BillTo1,");
            sb.Append("client.city + ' ' + client.stat + ' ' + client.zip AS BillTo2,");

            sb.Append("client.cust_shp_addr  AS ShipTo1,");
            sb.Append("client.cust_shp_city + ' ' + client.cust_shp_state + ' ' + client.cust_shp_zip AS ShipTo2,");

            sb.Append("client.tele AS Phone,");
            sb.Append("client.cust_credit_limit AS Balance,");
            sb.Append("t_cmr_baln.cur_baln  AS CurrentBalance,"); /* build 0008 added */
            sb.Append("t_cmr_baln.d30_baln  AS _30DaysBalance,"); /* build 0008 added */
            sb.Append("t_cmr_baln.d60_baln  AS _60DaysBalance,"); /* build 0008 added */
            sb.Append("t_cmr_baln.d90_baln  AS _90DaysBalance,"); /* build 0008 added */
            
            sb.Append("client.cust_sales_no AS SM#,");
            sb.Append("t_empl_profile.empl_first_nm + ' ' + t_empl_profile.empl_last_nm as Salesman,");
            sb.Append("client.cust_cntct AS Contact,");
            sb.Append("client.cust_email AS email,");
            sb.Append("client.cname AS ChineseName, ");
            sb.Append("'1' AS RecordType ");
            sb.Append("FROM DBA.t_summit_data_out ");
            sb.Append("INNER JOIN DBA.client ON client.ccode = DBA.t_summit_data_out.do_key ");
            sb.Append("LEFT JOIN DBA.t_cmr_baln ON client.ccode = t_cmr_baln.ccode "); /* build 0008 added */
            sb.Append("LEFT JOIN DBA.t_empl_profile ON client.cust_sales_no = t_empl_profile.empl_no ");
            sb.Append("WHERE do_type = 3 ");
            sb.Append("ORDER BY do_crt_time ");
         
            DataSet ds = DbHelperOdbc.Query(sb.ToString());

            string clearSQL = "";

            if (ds.Tables.Count > 0)
            {
                int cnt = ds.Tables[0].Rows.Count;

                if (cnt > 0)
                {
                    //2017-10-10 不需要header
                    //write header
                    //csv.WriteField("Acct#");
                    //csv.WriteField("Name");
                    //csv.WriteField("Region");
                    //csv.WriteField("Bill to 1");
                    //csv.WriteField("Bill to 2");
                    //csv.WriteField("Ship to 1");
                    //csv.WriteField("Ship to 2");
                    //csv.WriteField("Phone");
                    //csv.WriteField("Balance");
                    //csv.WriteField("SM#");
                    //csv.WriteField("Salesman");
                    //csv.WriteField("Contact");
                    //csv.WriteField("email");
                    //csv.WriteField("Chinese Name");
                    //csv.WriteField("Record Type");
                    //csv.NextRecord();

                    //write content
                    for (int i = 0; i < cnt; i++)
                    {

                        string AcctNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["Acct#"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Acct#"]);
                        string Name = Convert.IsDBNull(ds.Tables[0].Rows[i]["Name"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Name"]);
                        string Region = Convert.IsDBNull(ds.Tables[0].Rows[i]["Region"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Region"]);
                        //string BillTo = Convert.IsDBNull(ds.Tables[0].Rows[i]["BillTo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["BillTo"]);
                        //string ShipTo = Convert.IsDBNull(ds.Tables[0].Rows[i]["ShipTo"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ShipTo"]);

                        string BillTo1 = Convert.IsDBNull(ds.Tables[0].Rows[i]["BillTo1"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["BillTo1"]);
                        string BillTo2 = Convert.IsDBNull(ds.Tables[0].Rows[i]["BillTo2"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["BillTo2"]);
                        string ShipTo1 = Convert.IsDBNull(ds.Tables[0].Rows[i]["ShipTo1"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ShipTo1"]);
                        string ShipTo2 = Convert.IsDBNull(ds.Tables[0].Rows[i]["ShipTo2"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ShipTo2"]);


                        string Phone = Convert.IsDBNull(ds.Tables[0].Rows[i]["Phone"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Phone"]);
                        string Balance = Convert.IsDBNull(ds.Tables[0].Rows[i]["Balance"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Balance"]);
                        string CurrentBalance = Convert.IsDBNull(ds.Tables[0].Rows[i]["CurrentBalance"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["CurrentBalance"]); /* build 0008 added */
                        string _30DaysBalance = Convert.IsDBNull(ds.Tables[0].Rows[i]["_30DaysBalance"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["_30DaysBalance"]); /* build 0008 added */
                        string _60DaysBalance = Convert.IsDBNull(ds.Tables[0].Rows[i]["_60DaysBalance"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["_60DaysBalance"]); /* build 0008 added */
                        string _90DaysBalance = Convert.IsDBNull(ds.Tables[0].Rows[i]["_90DaysBalance"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["_90DaysBalance"]); /* build 0008 added */


                        string SMNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["SM#"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["SM#"]);
                        string Salesman = Convert.IsDBNull(ds.Tables[0].Rows[i]["Salesman"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Salesman"]);
                        string Contact = Convert.IsDBNull(ds.Tables[0].Rows[i]["Contact"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Contact"]);

                        string email = Convert.IsDBNull(ds.Tables[0].Rows[i]["email"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["email"]);
                        string ChineseName = Convert.IsDBNull(ds.Tables[0].Rows[i]["ChineseName"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ChineseName"]);

                      
                        //string ChineseDescription = Convert.IsDBNull(ds.Tables[0].Rows[i]["ChineseDescription"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ChineseDescription"]);
                        //string UPCCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["UPCCode"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["UPCCode"]);
                        //string RelatedItemCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["RelatedItemCode"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RelatedItemCode"]);
                        string RecordType = Convert.IsDBNull(ds.Tables[0].Rows[i]["RecordType"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["RecordType"]);

                        csv.WriteField(AcctNumber);
                        csv.WriteField(Name);
                        csv.WriteField(Region);
                        //csv.WriteField(BillTo);
                        //csv.WriteField(ShipTo);
                        csv.WriteField(BillTo1);    // bill to 1
                        csv.WriteField(BillTo2);    // bill to 2
                        csv.WriteField("");         // bill to 3
                        csv.WriteField(ShipTo1);    // ship to 1
                        csv.WriteField(ShipTo2);    // ship to 2
                        csv.WriteField("");         // ship to 3

                        csv.WriteField(Phone);
                        csv.WriteField(Balance);

                        csv.WriteField(CurrentBalance); /* build 0008 added */
                        csv.WriteField(_30DaysBalance); /* build 0008 added */
                        csv.WriteField(_60DaysBalance); /* build 0008 added */
                        csv.WriteField(_90DaysBalance); /* build 0008 added */

                        csv.WriteField(SMNumber);
                        csv.WriteField(Salesman);
                        csv.WriteField(Contact);
                        csv.WriteField(email);
                        csv.WriteField(ChineseName);
                        csv.WriteField(RecordType);

                        csv.NextRecord();

                        if (ToastMessage != null)
                            ToastMessage(this, new InfoEventArgs("", string.Format("Completed: ", i.ToString("N0"))));

                    }

                    long start = Convert.ToInt64(ds.Tables[0].Rows[0]["ID"]);
                    long end = Convert.ToInt64(ds.Tables[0].Rows[cnt - 1]["ID"]);

                    clearSQL = "DELETE FROM DBA.t_summit_data_out WHERE do_type = 3 AND do_id >= " + start + " AND do_id <= " + end;

                }
            }

            csv.Dispose();

            if (!string.IsNullOrEmpty(clearSQL))
            {
                DbHelperOdbc.ExecuteSql(clearSQL);
            }


        }
        //internal class MessageEventArgs : EventArgs
        //{
        //    private string _msg;
        //    public MessageEventArgs(string msg) : base()
        //    {
        //        this._msg = msg;
        //    }

        //    public string Message
        //    {
        //        set
        //        {
        //            this._msg = value;
        //        }
        //        get
        //        {
        //            return _msg;
        //        }
        //    }
        //}

        /// <summary>
        /// 合并两个quote文件，list 和 detail
        /// </summary>
        private void DoOutputQuoteInfoMerge()
        {
            string clearSQL = "";
            StringBuilder sb1 = new StringBuilder();

            sb1.Append("SELECT ");
            sb1.Append("MIN(do_id) AS _Start, ");
            sb1.Append("MAX(do_id) AS _End ");
            sb1.Append("FROM DBA.t_summit_data_out ");
            sb1.Append("WHERE do_type = 5 ");
            // sb.Append("ORDER BY do_crt_time ");

            DataSet ds1 = DbHelperOdbc.Query(sb1.ToString());

            if (ds1.Tables.Count > 0)
            {
                int cnt = ds1.Tables[0].Rows.Count;
                if (cnt > 0)
                {
                    decimal Start = Convert.IsDBNull(ds1.Tables[0].Rows[0]["_Start"]) ? -1 : Convert.ToDecimal(ds1.Tables[0].Rows[0]["_Start"]);
                    decimal End = Convert.IsDBNull(ds1.Tables[0].Rows[0]["_End"]) ? -1 : Convert.ToDecimal(ds1.Tables[0].Rows[0]["_End"]);

                    DoOutputQuoteMerge(Start, End);

                    clearSQL = "DELETE FROM DBA.t_summit_data_out WHERE do_type = 5 AND do_id >= " + Start + " AND do_id <= " + End + " ORDER BY do_crt_time";

                }

                if (!string.IsNullOrEmpty(clearSQL))
                {
                    DbHelperOdbc.ExecuteSql(clearSQL);
                }
            }
        }

        private void DoOutputQuoteInfo()
        {
            string clearSQL = "";
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("MIN(do_id) AS _Start, ");
            sb.Append("MAX(do_id) AS _End ");
            sb.Append("FROM DBA.t_summit_data_out ");
            sb.Append("WHERE do_type = 5 ");
           // sb.Append("ORDER BY do_crt_time ");

            DataSet ds = DbHelperOdbc.Query(sb.ToString());

            if (ds.Tables.Count > 0)
            {
                int cnt = ds.Tables[0].Rows.Count;
                if (cnt > 0)
                {
                    decimal Start = Convert.IsDBNull(ds.Tables[0].Rows[0]["_Start"]) ? -1 : Convert.ToDecimal(ds.Tables[0].Rows[0]["_Start"]);
                    decimal End = Convert.IsDBNull(ds.Tables[0].Rows[0]["_End"]) ? -1 : Convert.ToDecimal(ds.Tables[0].Rows[0]["_End"]);


                    /* build 0005 分别导出两个文件，一个是quote, 另外一个是quote detail
                    string name = DateTime.Now.ToString("yyyyMMddHHmmss");

                    DoOutputQuote(Start, End, name);

                    DoOutputQuoteDetail(Start, End, name);
                    */

                    // build 0006 导出两个文件合并成一个。并且不需要输出header
                    DoOutputQuoteMerge(Start, End);

                    clearSQL = "DELETE FROM DBA.t_summit_data_out WHERE do_type = 5 AND do_id >= " + Start + " AND do_id <= " + End + " ORDER BY do_crt_time";

                }

                if (!string.IsNullOrEmpty(clearSQL))
                {
                    DbHelperOdbc.ExecuteSql(clearSQL);
                }
            }
        }

        private void DoOutputQuoteMerge(decimal start, decimal end)
        {

            //string outputFolder = Properties.Settings.Default.ExportFolderOfItem;
            string outputFolder = config.exportFolderOfItem;

            //是否需要输出header , 需要设置 true
            bool needHeader = false; 
          

            //查询所有需要导出PO数据
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("t_summit_data_out.do_id AS ID, ");
            sb.Append("'H' AS Type, ");
            sb.Append("t_quote_lst.qo_cust_no AS Account#, ");
            sb.Append("t_quote_lst.qo_ref AS PO#, ");
            sb.Append("ItemlineCnt.cnt AS TotalLine, ");
            /* build 0008 */
            //sb.Append("CASE ");
            //sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'N' THEN 'Open' ");
            //sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'Y' THEN 'Closed' ");
            //sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'P' THEN 'Partial' ");
            //sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'V' THEN 'Void' ");
            //sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'A' THEN 'Adjusted' ");
            //sb.Append("END ");

            //sb.Append("CASE ");
            //sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'N' THEN '0' ");
            //sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'Y' THEN '2' ");
            //sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'P' THEN '1' ");
            //sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'V' THEN '9' ");
            //sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'A' THEN '3' ");
            //sb.Append("END ");
            sb.Append(" 0 "); //全部默认为 0
            /*******************/
            sb.Append(" AS Status, ");

            /* build 0008 bug fix */
            //sb.Append("t_quote_lst.qo_ref AS Ref#, ");
            sb.Append("t_quote_lst.qo_no AS Ref#, ");
           
            //sb.Append("t_quote_lst.qo_id AS Invoice, ");
            sb.Append("t_quote_lst.qo_ref AS Invoice, ");
            /*******************/

            sb.Append("CASE ");
            sb.Append("    WHEN t_empl_profile.empl_brief_nm IS NULL OR t_empl_profile.empl_brief_nm = '' THEN t_empl_profile.empl_first_nm + ' ' + t_empl_profile.empl_last_nm ");
            sb.Append("    ELSE t_empl_profile.empl_brief_nm ");
            sb.Append("END ");
            sb.Append(" AS Salesman, ");

            sb.Append("CASE ");
            sb.Append("WHEN t_empl_profile.empl_cms_fl = 'Y' THEN t_empl_profile.empl_cms_rt ");
            sb.Append("ELSE null ");
            sb.Append("END ");
            sb.Append("AS Commission, ");

            sb.Append("0 AS Discount, ");
            sb.Append("t_quote_lst.qo_total AS OrderTotal, ");
            sb.Append("t_ship_via.sh_via_name AS ShipVia, ");
            //sb.Append("t_quote_lst.qo_date AS OrderDate, ");
            sb.Append("CONVERT(char(10), t_quote_lst.qo_date, 120) AS OrderDate, ");
            //sb.Append("t_quote_lst.qo_delvy_date AS DelvDate, ");
            sb.Append("CONVERT(char(10), t_quote_lst.qo_delvy_date, 120) AS DelvDate, ");
            sb.Append("t_quote_lst.qo_memo AS OrderMessage, ");
            sb.Append("t_quote_lst.qo_shp_addr + ' ' + t_quote_lst.qo_shp_city + ' ' + t_quote_lst.qo_shp_st + ' ' + t_quote_lst.qo_shp_zip AS ShipToAdrs, ");
            sb.Append("2 AS Source ");
            sb.Append("FROM DBA.t_summit_data_out ");
            sb.Append("INNER JOIN DBA.t_quote_lst ON t_quote_lst.qo_id = DBA.t_summit_data_out.do_key ");
            sb.Append("LEFT JOIN DBA.t_empl_profile ON t_empl_profile.empl_no = t_quote_lst.qo_sales_no ");
            sb.Append("LEFT JOIN DBA.t_ship_via ON t_ship_via.sh_via_no = t_quote_lst.qo_ship_via ");
            sb.Append("LEFT JOIN(SELECT qo_id, count(1) AS cnt FROM DBA.t_quote_detail group by qo_id ) AS ItemlineCnt ON ItemlineCnt.qo_id = t_quote_lst.qo_id ");
            // sb.Append("WHERE do_type = 5 ");
            sb.Append("WHERE do_type = 5 AND do_id >= " + start + " AND do_id <= " + end);
            sb.Append("ORDER BY do_crt_time ");

            DataSet ds = DbHelperOdbc.Query(sb.ToString());

            if (ds.Tables.Count > 0)
            {
                int cnt = ds.Tables[0].Rows.Count;

                if (cnt > 0)
                {
                   

                    //write content
                    for (int i = 0; i < cnt; i++)
                    {

                        //构建csv文件
                        string name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string exportFile = string.Format(@"{0}_{1}.txt", "order", name);
                        string csvFile = outputFolder + "\\" + exportFile;

                        if (!Directory.Exists(outputFolder))
                        {
                            Directory.CreateDirectory(outputFolder);
                        }

                        if (File.Exists(csvFile))
                        {
                            File.Delete(csvFile);
                        }

                        TextWriter streamWriter = new StreamWriter(csvFile);
                        CsvHelper.CsvWriter csv = new CsvHelper.CsvWriter(streamWriter);

                        csv.Configuration.QuoteAllFields = true;
                        csv.Configuration.HasHeaderRecord = true;

                        if (ToastMessage != null)
                            ToastMessage(this, new InfoEventArgs("Exporting " + exportFile, ""));
 
                        if (needHeader)
                        {
                            //write header
                            csv.WriteField("Type");
                            csv.WriteField("Account #");
                            csv.WriteField("PO#");
                            csv.WriteField("Total line");
                            csv.WriteField("Status");
                            csv.WriteField("Ref#");
                            csv.WriteField("Invoice");
                            csv.WriteField("Salesman");
                            csv.WriteField("Commission");
                            csv.WriteField("Discount");
                            csv.WriteField("Order Total");
                            csv.WriteField("Ship Via");
                            csv.WriteField("Order Date");
                            csv.WriteField("Delv Date");
                            csv.WriteField("Message");
                            csv.WriteField("Ship to Adrs");
                            csv.WriteField("Source");
                            csv.NextRecord();
                        }

                        string ID = Convert.IsDBNull(ds.Tables[0].Rows[i]["ID"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ID"]);
                        string Type = Convert.IsDBNull(ds.Tables[0].Rows[i]["Type"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Type"]);
                        string AccountNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["Account#"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Account#"]);
                        string PONumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["PO#"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PO#"]);
                        string TotalLine = Convert.IsDBNull(ds.Tables[0].Rows[i]["TotalLine"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["TotalLine"]);
                        string Status = Convert.IsDBNull(ds.Tables[0].Rows[i]["Status"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Status"]);
                        string RefNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["Ref#"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Ref#"]);
                        string Invoice = Convert.IsDBNull(ds.Tables[0].Rows[i]["Invoice"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Invoice"]);
                        string Salesman = Convert.IsDBNull(ds.Tables[0].Rows[i]["Salesman"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Salesman"]);
                        string Commission = Convert.IsDBNull(ds.Tables[0].Rows[i]["Commission"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Commission"]);
                        string Discount = Convert.IsDBNull(ds.Tables[0].Rows[i]["Discount"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Discount"]);
                        string OrderTotal = Convert.IsDBNull(ds.Tables[0].Rows[i]["OrderTotal"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["OrderTotal"]);
                        string ShipVia = Convert.IsDBNull(ds.Tables[0].Rows[i]["ShipVia"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ShipVia"]);
                        string OrderDate = Convert.IsDBNull(ds.Tables[0].Rows[i]["OrderDate"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["OrderDate"]);
                        string DelvDate = Convert.IsDBNull(ds.Tables[0].Rows[i]["DelvDate"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["DelvDate"]);
                        string OrderMessage = Convert.IsDBNull(ds.Tables[0].Rows[i]["OrderMessage"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["OrderMessage"]);
                        string ShipToAdrs = Convert.IsDBNull(ds.Tables[0].Rows[i]["ShipToAdrs"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ShipToAdrs"]);
                        string Source = Convert.IsDBNull(ds.Tables[0].Rows[i]["Source"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Source"]);

                        if (!string.IsNullOrEmpty(Discount))
                            Discount = Convert.ToString(Math.Truncate(Convert.ToDecimal(Discount) * 100));

                        if (!string.IsNullOrEmpty(OrderTotal))
                            OrderTotal = Convert.ToString(Math.Truncate(Convert.ToDecimal(OrderTotal) * 100));

                        csv.WriteField(Type);
                        csv.WriteField(AccountNumber);
                        csv.WriteField(PONumber);
                        csv.WriteField(TotalLine);
                        csv.WriteField(Status);
                        csv.WriteField(RefNumber);
                        csv.WriteField(Invoice);
                        csv.WriteField(Salesman);
                        csv.WriteField(Commission);
                        csv.WriteField(Discount);
                        csv.WriteField(OrderTotal);
                        csv.WriteField(ShipVia);
                        csv.WriteField(OrderDate);
                        csv.WriteField(DelvDate);
                        csv.WriteField(OrderMessage);
                        csv.WriteField(ShipToAdrs);
                        csv.WriteField(Source);
                        
                        csv.NextRecord();
                        
                        #region export quote detail
                        //查询所有需要导出quote数据
                        StringBuilder sbDetail = new StringBuilder();

                        sbDetail.Append("SELECT ");
                        sbDetail.Append("t_summit_data_out.do_id AS ID, ");
                        sbDetail.Append("'L' AS Type, ");

                        /* 2018-01-10 bug fixed 更正reference#, invoice */
                        //sbDetail.Append("t_quote_lst.qo_ref AS Reference#, ");
                        //sbDetail.Append("t_quote_detail.qo_id AS Invoice, ");
                        sbDetail.Append("t_quote_lst.qo_no AS Reference#, ");
                        sbDetail.Append("t_quote_lst.qo_ref AS Invoice, ");
                        /***********************************************/
                        sbDetail.Append("t_quote_detail.qo_item_no AS Item#, ");
                        sbDetail.Append("t_quote_detail.qo_qty AS Quantity, ");
                        sbDetail.Append("t_quote_detail.qo_rate AS Price, ");
  
                        /* build 0008 removed */
                        //sbDetail.Append("CASE ");
                        //sbDetail.Append("   WHEN qo_3_dim IS NULL OR qo_qty_per_case IS NULL THEN '' ");
                        //sbDetail.Append("   WHEN qo_3_dim = 1 THEN Convert(varchar(10),qo_qty_per_case)+' ' + UPPER(qo_pkg_dsc) + ' X' + qo_unit_pkg ");
                        //sbDetail.Append("   WHEN qo_3_dim > 1 THEN Convert(varchar(10),qo_3_dim) +'X' + Convert(varchar(10), qo_qty_per_case) + ' ' + UPPER(qo_pkg_dsc) + ' X' + qo_unit_pkg ");
                        //sbDetail.Append("END AS _Message, ");

                        //sbDetail.Append("'' AS BackOrdered "); /* build 0008 removed */
                        sbDetail.Append("t_quote_detail.qo_entry_no AS Line# ");  /* build 0008 added */

                        /* 
                         * 2 - Orders originate from the salesmen in the field.
                         * 1 - Orders have been modified for price and quantity.
                         * 0 - Orders taken by the office, when salesmen aren't available. 
                         */

                        //sbDetail.Append("CASE ");
                        //sbDetail.Append("  WHEN qo_org_rate IS NULL AND qo_org_qty IS NULL ");
                        //sbDetail.Append("   THEN '0' ");
                        //sbDetail.Append("  WHEN(qo_org_rate IS NOT NULL AND qo_org_qty IS NOT NULL) ");
                        //sbDetail.Append("   AND(qo_rate IS NOT NULL AND qo_qty IS NOT NULL) ");
                        //sbDetail.Append("   AND(qo_org_rate <> qo_rate OR qo_org_qty <> qo_qty) ");
                        //sbDetail.Append("   THEN '1' ");
                        //sbDetail.Append("  WHEN(qo_org_rate IS NOT NULL AND qo_org_qty IS NOT NULL) ");
                        //sbDetail.Append("       AND(qo_rate IS NOT NULL AND qo_qty IS NOT NULL) ");
                        //sbDetail.Append("       AND(qo_org_rate = qo_rate AND qo_org_qty = qo_qty) ");
                        //sbDetail.Append("   THEN '2' ");
                        //sbDetail.Append("END AS OrderFrom ");


                        sbDetail.Append("FROM DBA.t_summit_data_out ");
                        sbDetail.Append("INNER JOIN DBA.t_quote_detail ON t_quote_detail.qo_id = DBA.t_summit_data_out.do_key ");
                        sbDetail.Append("LEFT JOIN DBA.t_quote_lst ON t_quote_lst.qo_id = t_quote_detail.qo_id ");
                        sbDetail.Append("LEFT JOIN DBA.t_ship_via ON t_ship_via.sh_via_no = t_quote_lst.qo_ship_via ");
                        sbDetail.Append("LEFT JOIN(SELECT qo_id, count(1) AS cnt FROM DBA.t_quote_detail group by qo_id ) AS ItemlineCnt ON ItemlineCnt.qo_id = t_quote_lst.qo_id ");
                        // sb.Append("WHERE do_type = 5 ");
                        sbDetail.Append("WHERE do_type = 5 ");
                        //sbDetail.Append("AND do_id >= " + start + " AND do_id <= " + end);
                        sbDetail.Append("AND do_id = " + ID );
                        sbDetail.Append("ORDER BY do_crt_time ");

                        DataSet dsDetail = DbHelperOdbc.Query(sbDetail.ToString());

                        if (dsDetail.Tables.Count > 0)
                        {
                            int cntDetail = dsDetail.Tables[0].Rows.Count;

                            if (cntDetail > 0)
                            {
                                if (needHeader)
                                {
                                    //write header
                                    csv.WriteField("Type");
                                    csv.WriteField("Reference #");
                                    csv.WriteField("Invoice");
                                    csv.WriteField("Item #");
                                    csv.WriteField("Quantity");
                                    csv.WriteField("Price");
                                    //csv.WriteField("Message");        /* build 0008 removed */
                                    //csv.WriteField("Back Ordered");   /* build 0008 removed */
                                    csv.WriteField("Line#");            /* build 0008 added */
                                    csv.NextRecord();
                                }
                   
                                //write content
                                for (int k = 0; k < cntDetail; k++)
                                {

                                    string Type2 = Convert.IsDBNull(dsDetail.Tables[0].Rows[k]["Type"]) ? "" : Convert.ToString(dsDetail.Tables[0].Rows[k]["Type"]);
                                    string ReferenceNumber = Convert.IsDBNull(dsDetail.Tables[0].Rows[k]["Reference#"]) ? "" : Convert.ToString(dsDetail.Tables[0].Rows[k]["Reference#"]);
                                    string Invoice2 = Convert.IsDBNull(dsDetail.Tables[0].Rows[k]["Invoice"]) ? "" : Convert.ToString(dsDetail.Tables[0].Rows[k]["Invoice"]);
                                    string ItemNumber = Convert.IsDBNull(dsDetail.Tables[0].Rows[k]["Item#"]) ? "" : Convert.ToString(dsDetail.Tables[0].Rows[k]["Item#"]);
                                    string Quantity = Convert.IsDBNull(dsDetail.Tables[0].Rows[k]["Quantity"]) ? "" : Convert.ToString(dsDetail.Tables[0].Rows[k]["Quantity"]);
                                    string Price = Convert.IsDBNull(dsDetail.Tables[0].Rows[k]["Price"]) ? "" : Convert.ToString(dsDetail.Tables[0].Rows[k]["Price"]);
                                    //string Message = Convert.IsDBNull(dsDetail.Tables[0].Rows[k]["_Message"]) ? "" : Convert.ToString(dsDetail.Tables[0].Rows[k]["_Message"]);            /* build 0008 removed */
                                    //string BackOrdered = Convert.IsDBNull(dsDetail.Tables[0].Rows[k]["BackOrdered"]) ? "" : Convert.ToString(dsDetail.Tables[0].Rows[k]["BackOrdered"]);  /* build 0008 removed */
                                    string LineNumber = Convert.IsDBNull(dsDetail.Tables[0].Rows[k]["Line#"]) ? "" : Convert.ToString(dsDetail.Tables[0].Rows[k]["Line#"]);                 /* build 0008 added */

                                    //if (!string.IsNullOrEmpty(Quantity))
                                    //    Quantity = Convert.ToString(Math.Truncate(Convert.ToDecimal(Quantity) * 100));

                                    if (!string.IsNullOrEmpty(Quantity))
                                        Quantity = Convert.ToString(Convert.ToInt32(Convert.ToDecimal(Quantity)));


                                    if (!string.IsNullOrEmpty(Price))
                                        Price = Convert.ToString(Math.Truncate(Convert.ToDecimal(Price) * 100));

                                    csv.WriteField(Type2);
                                    csv.WriteField(ReferenceNumber);
                                    csv.WriteField(Invoice2);
                                    csv.WriteField(ItemNumber);

                                    /* build 0008 change output sequence */
                                    //csv.WriteField(Quantity);
                                    //csv.WriteField(Price);
                                    csv.WriteField(Price);
                                    csv.WriteField(Quantity);
                                    /*************************************/

                                    //csv.WriteField(Message);        /* build 0008 removed */
                                    //csv.WriteField(BackOrdered);    /* build 0008 removed */
                                    csv.WriteField(LineNumber);      /* build 0008 added */
                                    //csv.WriteField("2");          /* build 0008 removed */

                                    csv.NextRecord();
                                }
                            }
                        }
                        #endregion

                        csv.Dispose();

                        if (ToastMessage != null)
                            ToastMessage(this, new InfoEventArgs("", string.Format("Completed: ", i.ToString("N0"))));

                       
                    }

                }
            }

        }

        private void DoOutputQuote(decimal start , decimal end, string name)
        {

            //string outputFolder = Properties.Settings.Default.ExportFolderOfItem;
            string outputFolder = config.exportFolderOfItem;

            //构建csv文件
            string exportFile = string.Format(@"{0}_{1}.txt", "qohd", name);
            string csvFile = outputFolder + "\\" + exportFile;

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            if (File.Exists(csvFile))
            {
                File.Delete(csvFile);
            }

            TextWriter streamWriter = new StreamWriter(csvFile);
            CsvHelper.CsvWriter csv = new CsvHelper.CsvWriter(streamWriter);

            csv.Configuration.QuoteAllFields = true;
            csv.Configuration.HasHeaderRecord = true;

            if (ToastMessage != null)
                ToastMessage(this, new InfoEventArgs("Exporting " + exportFile, ""));

            //查询所有需要导出PO数据
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("t_summit_data_out.do_id AS ID, ");
            sb.Append("'H' AS Type, ");
            sb.Append("t_quote_lst.qo_cust_no AS Account#, ");
            sb.Append("t_quote_lst.qo_ref AS PO#, ");
            sb.Append("ItemlineCnt.cnt AS TotalLine, ");
            sb.Append("CASE ");
            sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'N' THEN 'Open' ");
            sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'Y' THEN 'Closed' ");
            sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'P' THEN 'Partial' ");
            sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'V' THEN 'Void' ");
            sb.Append("    WHEN t_quote_lst.qo_cleared_fl = 'A' THEN 'Adjusted' ");
            sb.Append("END ");
            sb.Append(" AS Status, ");
            sb.Append("t_quote_lst.qo_ref AS Ref#, ");
            sb.Append("t_quote_lst.qo_id AS Invoice, ");
            sb.Append("CASE ");
            sb.Append("    WHEN t_empl_profile.empl_brief_nm IS NULL OR t_empl_profile.empl_brief_nm = '' THEN t_empl_profile.empl_first_nm + ' ' + t_empl_profile.empl_last_nm ");
            sb.Append("    ELSE t_empl_profile.empl_brief_nm ");
            sb.Append("END ");
            sb.Append(" AS Salesman, ");
            sb.Append("CASE ");
            sb.Append("WHEN t_empl_profile.empl_cms_fl = 'Y' THEN t_empl_profile.empl_cms_rt ");
            sb.Append("ELSE null ");
            sb.Append("END ");
            sb.Append("AS Commission, ");
            sb.Append("0 AS Discount, ");
            sb.Append("t_quote_lst.qo_total AS OrderTotal, ");
            sb.Append("t_ship_via.sh_via_name AS ShipVia, ");
            sb.Append("t_quote_lst.qo_date AS OrderDate, ");
            sb.Append("t_quote_lst.qo_delvy_date AS DelvDate, ");
            sb.Append("t_quote_lst.qo_memo AS OrderMessage, ");
            sb.Append("t_quote_lst.qo_shp_addr + ' ' + t_quote_lst.qo_shp_city + ' ' + t_quote_lst.qo_shp_st + ' ' + t_quote_lst.qo_shp_zip AS ShipToAdrs, ");
            sb.Append("2 AS Source ");
            sb.Append("FROM DBA.t_summit_data_out ");
            sb.Append("INNER JOIN DBA.t_quote_lst ON t_quote_lst.qo_id = DBA.t_summit_data_out.do_key ");
            sb.Append("LEFT JOIN DBA.t_empl_profile ON t_empl_profile.empl_no = t_quote_lst.qo_sales_no ");
            sb.Append("LEFT JOIN DBA.t_ship_via ON t_ship_via.sh_via_no = t_quote_lst.qo_ship_via ");
            sb.Append("LEFT JOIN(SELECT qo_id, count(1) AS cnt FROM DBA.t_quote_detail group by qo_id ) AS ItemlineCnt ON ItemlineCnt.qo_id = t_quote_lst.qo_id ");
            // sb.Append("WHERE do_type = 5 ");
            sb.Append("WHERE do_type = 5 AND do_id >= " + start + " AND do_id <= " + end);
            sb.Append("ORDER BY do_crt_time ");

            DataSet ds = DbHelperOdbc.Query(sb.ToString());

            if (ds.Tables.Count > 0)
            {
                int cnt = ds.Tables[0].Rows.Count;

                if (cnt > 0)
                {
                    //write header
                    csv.WriteField("Type");
                    csv.WriteField("Account #");
                    csv.WriteField("PO#");
                    csv.WriteField("Total line");
                    csv.WriteField("Status");
                    csv.WriteField("Ref#");
                    csv.WriteField("Invoice");
                    csv.WriteField("Salesman");
                    csv.WriteField("Commission");
                    csv.WriteField("Discount");
                    csv.WriteField("Order Total");
                    csv.WriteField("Ship Via");
                    csv.WriteField("Order Date");
                    csv.WriteField("Delv Date");
                    csv.WriteField("Message");
                    csv.WriteField("Ship to Adrs");
                    csv.WriteField("Source");

                    csv.NextRecord();

                    //write content
                    for (int i = 0; i < cnt; i++)
                    {

                        string Type = Convert.IsDBNull(ds.Tables[0].Rows[i]["Type"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Type"]);
                        string AccountNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["Account#"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Account#"]);
                        string PONumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["PO#"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["PO#"]);
                        string TotalLine = Convert.IsDBNull(ds.Tables[0].Rows[i]["TotalLine"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["TotalLine"]);
                        string Status = Convert.IsDBNull(ds.Tables[0].Rows[i]["Status"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Status"]);
                        string RefNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["Ref#"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Ref#"]);
                        string Invoice = Convert.IsDBNull(ds.Tables[0].Rows[i]["Invoice"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Invoice"]);
                        string Salesman = Convert.IsDBNull(ds.Tables[0].Rows[i]["Salesman"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Salesman"]);
                        string Commission = Convert.IsDBNull(ds.Tables[0].Rows[i]["Commission"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Commission"]);
                        string Discount = Convert.IsDBNull(ds.Tables[0].Rows[i]["Discount"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Discount"]);
                        string OrderTotal = Convert.IsDBNull(ds.Tables[0].Rows[i]["OrderTotal"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["OrderTotal"]);
                        string ShipVia = Convert.IsDBNull(ds.Tables[0].Rows[i]["ShipVia"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ShipVia"]);
                        string OrderDate = Convert.IsDBNull(ds.Tables[0].Rows[i]["OrderDate"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["OrderDate"]);
                        string DelvDate = Convert.IsDBNull(ds.Tables[0].Rows[i]["DelvDate"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["DelvDate"]);
                        string OrderMessage = Convert.IsDBNull(ds.Tables[0].Rows[i]["OrderMessage"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["OrderMessage"]);
                        string ShipToAdrs = Convert.IsDBNull(ds.Tables[0].Rows[i]["ShipToAdrs"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["ShipToAdrs"]);         
                        string Source = Convert.IsDBNull(ds.Tables[0].Rows[i]["Source"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Source"]);

                        csv.WriteField(Type);
                        csv.WriteField(AccountNumber);
                        csv.WriteField(PONumber);
                        csv.WriteField(TotalLine);
                        csv.WriteField(Status);
                        csv.WriteField(RefNumber);
                        csv.WriteField(Invoice);
                        csv.WriteField(Salesman);
                        csv.WriteField(Commission);
                        csv.WriteField(Discount);
                        csv.WriteField(OrderTotal);
                        csv.WriteField(ShipVia);
                        csv.WriteField(OrderDate);
                        csv.WriteField(DelvDate);
                        csv.WriteField(OrderMessage);
                        csv.WriteField(ShipToAdrs);
                        csv.WriteField(Source);

                        csv.NextRecord();

                        if (ToastMessage != null)
                            ToastMessage(this, new InfoEventArgs("", string.Format("Completed: ", i.ToString("N0"))));

                    }

                }
            }

            csv.Dispose();

           
        }

        /// <summary>
        /// 已经过时
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="name"></param>
        private void DoOutputQuoteDetail(decimal start, decimal end,string name)
        {

            //string outputFolder = Properties.Settings.Default.ExportFolderOfItem;
            string outputFolder = config.exportFolderOfItem;

            //构建csv文件
            string exportFile = string.Format(@"{0}_{1}.txt", "qodt", name);
            string csvFile = outputFolder + "\\" + exportFile;

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            if (File.Exists(csvFile))
            {
                File.Delete(csvFile);
            }

            TextWriter streamWriter = new StreamWriter(csvFile);
            CsvHelper.CsvWriter csv = new CsvHelper.CsvWriter(streamWriter);

            csv.Configuration.QuoteAllFields = true;
            csv.Configuration.HasHeaderRecord = true;

            if (ToastMessage != null)
                ToastMessage(this, new InfoEventArgs("Exporting " + exportFile, ""));

            //查询所有需要导出PO数据
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("t_summit_data_out.do_id AS ID, ");
            sb.Append("'L' AS Type, ");
            sb.Append("t_quote_lst.qo_ref AS Reference#, ");
            sb.Append("t_quote_detail.qo_id AS Invoice, ");
            sb.Append("t_quote_detail.qo_item_no AS Item#, ");
            sb.Append("t_quote_detail.qo_qty AS Quantity, ");
            sb.Append("t_quote_detail.qo_rate AS Price, ");
            sb.Append("CASE ");
            sb.Append("   WHEN qo_3_dim IS NULL OR qo_qty_per_case IS NULL THEN '' ");
            sb.Append("   WHEN qo_3_dim = 1 THEN Convert(varchar(10),qo_qty_per_case)+' ' + UPPER(qo_pkg_dsc) + ' X' + qo_unit_pkg ");
            sb.Append("   WHEN qo_3_dim > 1 THEN Convert(varchar(10),qo_3_dim) +'X' + Convert(varchar(10), qo_qty_per_case) + ' ' + UPPER(qo_pkg_dsc) + ' X' + qo_unit_pkg ");
            sb.Append("END AS _Message, ");
            sb.Append("'' AS BackOrdered ");
            sb.Append("FROM DBA.t_summit_data_out ");
            sb.Append("INNER JOIN DBA.t_quote_detail ON t_quote_detail.qo_id = DBA.t_summit_data_out.do_key ");
            sb.Append("LEFT JOIN DBA.t_quote_lst ON t_quote_lst.qo_id = t_quote_detail.qo_id ");
            sb.Append("LEFT JOIN DBA.t_ship_via ON t_ship_via.sh_via_no = t_quote_lst.qo_ship_via ");
            sb.Append("LEFT JOIN(SELECT qo_id, count(1) AS cnt FROM DBA.t_quote_detail group by qo_id ) AS ItemlineCnt ON ItemlineCnt.qo_id = t_quote_lst.qo_id ");
            // sb.Append("WHERE do_type = 5 ");
            sb.Append("WHERE do_type = 5 AND do_id >= " + start + " AND do_id <= " + end);
            sb.Append("ORDER BY do_crt_time ");

            DataSet ds = DbHelperOdbc.Query(sb.ToString());

           

            if (ds.Tables.Count > 0)
            {
                int cnt = ds.Tables[0].Rows.Count;

                if (cnt > 0)
                {
                    //write header
                    csv.WriteField("Type");
                    csv.WriteField("Reference #");
                    csv.WriteField("Invoice");
                    csv.WriteField("Item #");
                    csv.WriteField("Quantity");
                    csv.WriteField("Price");
                    csv.WriteField("Message");
                    csv.WriteField("Back Ordered");
                   

                    csv.NextRecord();

                    //write content
                    for (int i = 0; i < cnt; i++)
                    {

                        string Type = Convert.IsDBNull(ds.Tables[0].Rows[i]["Type"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Type"]);
                        string ReferenceNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["Reference#"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Reference#"]);
                        string Invoice = Convert.IsDBNull(ds.Tables[0].Rows[i]["Invoice"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Invoice"]);
                        string ItemNumber = Convert.IsDBNull(ds.Tables[0].Rows[i]["Item#"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Item#"]);
                        string Quantity = Convert.IsDBNull(ds.Tables[0].Rows[i]["Quantity"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Quantity"]);
                        string Price = Convert.IsDBNull(ds.Tables[0].Rows[i]["Price"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Price"]);
                        string Message = Convert.IsDBNull(ds.Tables[0].Rows[i]["_Message"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["_Message"]);
                        string BackOrdered = Convert.IsDBNull(ds.Tables[0].Rows[i]["BackOrdered"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["BackOrdered"]);

                        csv.WriteField(Type);
                        csv.WriteField(ReferenceNumber);
                        csv.WriteField(Invoice);
                        csv.WriteField(ItemNumber);
                        csv.WriteField(Quantity);
                        csv.WriteField(Price);
                        csv.WriteField(Message);
                        csv.WriteField(BackOrdered);

                        csv.NextRecord();

                        if (ToastMessage != null)
                            ToastMessage(this, new InfoEventArgs("", string.Format("Completed: ", i.ToString("N0"))));

                    }

                   
                }
            }

            csv.Dispose();

         
        }

       
        internal class InfoEventArgs : EventArgs
        {
            private string _info;
            private string _status;

            public InfoEventArgs(string info,string status) : base()
            {
                this._info = info;
                this._status = status;
            }

            public string Info
            {
                set
                {
                    this._info = value;
                }
                get
                {
                    return _info;
                }
            }

            public string status
            {
                set
                {
                    this._status = value;
                }
                get
                {
                    return _status;
                }
            }
        }

        public static byte[] Big5ToUtf8(byte[] src)
        {
            string s = Encoding.GetEncoding("big5").GetString(src);
            byte[] dst = Encoding.UTF8.GetBytes(s);
            return dst;
        }


        private bool HaveZipCode(ref string val)
        {
            val = val.Trim();
            if (val.Length > 10)
            {
                string before = val.Substring(val.Length - 5, 1);
                if (before == "-")
                {
                    string after = val.Substring(val.Length - 10, 5);

                    if (IsNumber(before) && IsNumber(after) )
                    {
                        return true;
                    }

                }
                else
                {
                    string zipCode = val.Substring(val.Length - 5, 5);

                    if (IsNumber(zipCode))
                    {
                        return true;
                    }
                }
            }
            else if (val.Length > 5)
            {
                string zipCode = val.Substring(val.Length - 5, 5);

                if (IsNumber(zipCode))
                {
                    int len = val.Length - val.Length - 5;
                    val = val.Substring(0, len) + " " + zipCode;
                    return true;
                }
            }

            return false;
            
            
        }

        public bool IsNumber(String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) &&
                   objNumberPattern.IsMatch(strNumber);
        }

        public bool IsZipCode(string zipCode)
        {
            string pattern = @"^\d{5} (\-\d{4})?$";
            Regex regex = new Regex(pattern);

            var re = Regex.Matches(zipCode, pattern);
            foreach (Match x in re)
                Console.WriteLine("ZIP Code -> " + x.Groups[1].Value);

            return regex.IsMatch(zipCode);
        }

        public bool HaveLongZipCode(string zipCode)
        {
            string pattern = @"^\d{5}\-\d{4}?$";

            var re = Regex.Matches(zipCode, pattern);
            foreach (Match x in re)
                Console.WriteLine("ZIP Code -> " + x.Groups[1].Value);

            
            return re.Count > 0 ? true : false;
        }

        public bool HaveShortZipCode(string zipCode)
        {
            string pattern = @"^\d{5}?$";

            var re = Regex.Matches(zipCode, pattern);
            foreach (Match x in re)
                Console.WriteLine("ZIP Code -> " + x.Groups[1].Value);


            return re.Count > 0 ? true : false;


           
        }

        public string fixHaveZipCode(string userInput)
        {
            Regex postalCodeRegex = new Regex(@"^.*(\d{5}(-\d{4})?$)|(^[ABCEGHJKLMNPRSTVXY]{1}\d{1}[A-Z]{1} *\d{1}[A-Z]{1}\d{1}).*$"
            , RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

            Match m = postalCodeRegex.Match(userInput);
            if (m.Success)
            {
                String postalCode = m.Groups[1].Value;
                // Set location based on postal code

                return userInput.Replace(postalCode, " ") + postalCode;
            }
            else
            {
                // Set location based on city
                return userInput;
            }
        }

        public bool HaveZipCode(string userInput)
        {
            Regex postalCodeRegex = new Regex(@"^.*(\d{5}(-\d{4})?$)|(^[ABCEGHJKLMNPRSTVXY]{1}\d{1}[A-Z]{1} *\d{1}[A-Z]{1}\d{1}).*$"
            , RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

            Match m = postalCodeRegex.Match(userInput);
            if (m.Success)
            {
                String postalCode = m.Groups[1].Value;
                // Set location based on postal code

                return true;
            }
            else
            {
                // Set location based on city
                return false;
            }
        }

        private void WriteErrorRecord(CsvWriter csvWriter, SummitElementCustomer record)
        {
            if (isDebug)
            {
                csvWriter.WriteField(record.AcctNumber);
                csvWriter.WriteField(record.Name);
                csvWriter.WriteField(record.Region);
                csvWriter.WriteField(record.BillTo1);
                csvWriter.WriteField(record.BillTo2);
                csvWriter.WriteField(record.BillTo3);
                csvWriter.WriteField(record.ShipTo1);
                csvWriter.WriteField(record.ShipTo2);
                csvWriter.WriteField(record.ShipTo3);
                csvWriter.WriteField(record.Phone);
                csvWriter.WriteField(record.Balance);
                csvWriter.WriteField(record.SMNumber);
                csvWriter.WriteField(record.Salesman);
                csvWriter.WriteField(record.Contact);
                csvWriter.WriteField(record.Email);
                csvWriter.WriteField(record.ChineseName);
                csvWriter.WriteField(record.RecordType);
                csvWriter.NextRecord();
  

            }
        }






    }
}
