using ServiceModel.Base;
using ServiceModel.MoleQPos;
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

namespace gfcaDataService
{

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
		
		

        WholesaleConnection _WholesaleConnection = new WholesaleConnection();
        MarketConnection _MarketConnection = new MarketConnection();


        // oc_type
        Dictionary<int, string> typeMap = new Dictionary<int, string>() {
            { 1,"ITEM" }, // item
            { 2,"VENDOR" }, // vendor
            { 3,"CUSTOMER" }, // customer
            { 4,"INVERTORY" }, // invertory
        };
        
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


            _WholesaleConnection.SetConnectString(String.Format("Dsn={0};Uid={1};Pwd={2}",
                                                    config.odbc_dsn_wholesale,
                                                    config.odbc_usrid_wholesale,
                                                    config.odbc_pwd_wholesale
                                                    ));

            _MarketConnection.SetConnectString( String.Format("Dsn={0};Uid={1};Pwd={2}",
                                                    config.odbc_dsn_mkt,
                                                    config.odbc_usrid_mkt,
                                                    config.odbc_pwd_mkt
                                                      ));

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
             
                Thread.Sleep(TimerIntervalOfSync * 1000);
            }
        }

        internal bool IsProcessingData()
        {
            //
            return IsProcessing;
        }


      
        public void doExportData()
        {

            while (HaveJob())
            {
                if (workstart != null)
                    workstart(this);

                IsProcessing = true;

                int type = GetOutoutDataList();

                switch (type)
                {
                    case 1:
                        DoInsertProduct();
                        break;
                    case 2:
                        //DoOutputVendor();
                        break;
                    case 3:
                        //DoOutputCustomer();
                        break;
                    case 4:
                        //DoOutputInvertory();
                        break;
                    case 5:
                        // DoOutputQuoteInfo();
                        break;

                    default:
                        break;
                }


                if (workstop != null)
                    workstop(this);

                IsProcessing = false;
            }

     

        }


        private int GetOutoutDataList()
        {
            string sql = "SELECT TOP 1 t_out_cache.oc_type FROM DBA.t_out_cache WHERE oc_status = 'R' Order By oc_create_time";

            return Convert.ToInt32(_WholesaleConnection.ExecuteScalar(sql));

        }

 

        private void DoInsertProduct()
        {
            
            if (ToastMessage != null)
                ToastMessage(this, new InfoEventArgs("--------- start --------", ""));


            //查询所有需要导出Item
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT TOP 1 ");
            sb.Append("t_out_cache.oc_id AS ID, ");
            sb.Append("* ");
            sb.Append("FROM DBA.t_out_cache ");
            sb.Append("INNER JOIN DBA.prodtable ON prodtable.p_code = DBA.t_out_cache.oc_key ");
            sb.Append("WHERE oc_type = 1 ");
            sb.Append("AND oc_status = 'R' ");
            sb.Append("ORDER BY oc_create_time ");

            //导出完成删除Item
            DataSet ds = _WholesaleConnection.Query(sb.ToString());

            string clearSQL = "";

            if (ds.Tables.Count > 0)
            {
                int cnt = ds.Tables[0].Rows.Count;

                if (cnt > 0)
                {
                  
                    for (int i = 0; i < cnt; i++)
                    {
                        long id = Convert.ToInt64(ds.Tables[0].Rows[i]["ID"]);


                        try
                        {
                            //更新到market 数据库中
                            prodtable product = new prodtable(DBModel.SYBASE);
                            product.SetConnection(_MarketConnection);
                            product.Parse(ds.Tables[0].Rows[i]);

                            string insertsql = product.CreateInsertSQL();
                            _MarketConnection.ExecuteSql(insertsql);

                            clearSQL = "DELETE FROM DBA.t_out_cache WHERE oc_type = 1 AND oc_id = " + id;

                            if (!string.IsNullOrEmpty(clearSQL))
                            {
                                _WholesaleConnection.ExecuteSql(clearSQL);
                            }
                        }
                        catch (Exception ex)
                        {
                            string err_msg = ex.Message;
							
							err_msg = CleanError(err_msg,100);

                            err_msg = err_msg.Replace("'", "''");
                            
                            //在wholesale数据库中标记错误
                            _WholesaleConnection.ExecuteSql("Update DBA.t_out_cache SET oc_status = 'E', oc_msg = '"+ err_msg + "' WHERE oc_type = 1 AND oc_id = " + id);
                         
                        }
                        

                        if (ToastMessage != null)
                            ToastMessage(this, new InfoEventArgs("", string.Format("Completed: ", i.ToString("N0"))));


                        
                    }
                    
                }

            }
           

        }


        private bool HaveJob()
        {
            string sql = "SELECT count(*) FROM DBA.t_out_cache WHERE oc_status = 'R' ";

            return Convert.ToInt32(_WholesaleConnection.ExecuteScalar(sql)) > 0 ? true : false;
        }
		
		
		/// <summary>
        /// 将错误信息处理，进行简化，剔除关键字内容。并截取指定长度
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        /// <param name="errLimitLenght">错误信息限制长度</param>
		private string CleanError(string errMsg, int errLimitLenght)
        {
            //过滤error message 关键字 如：[Sybase][ODBC Driver][Adaptive Server Anywhere]
			string[] errorKeyword = { 	"[Sybase]", 
										"[ODBC Driver]", 
										"[Adaptive Server Anywhere]" 
									};
		
            string msg = errMsg;
            if (errorKeyword!=null && errorKeyword.Length > 0)
            {
                for (int i = 0; i < errorKeyword.Length; i++)
                {
                    msg = msg.Replace(errorKeyword[i], "");
                }
            }
			
            msg = msg.Substring(0, (msg.Length <= errLimitLenght) ? msg.Length : errLimitLenght);
           
			return msg.Trim();
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





    }
}
