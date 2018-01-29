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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;

namespace WholesaleMonitor
{
    class DataSynchronizer
    {

        private const string TAG = "DataSynchronizer";

        Thread process;
        private bool stopSync;

        private int cnt = 0;
        private bool isDebug = false;

        private GlobalConfiguration config = null;

        private Notification _Notification = null;

        private bool speaker = false;

        private bool IsWriteLog = false;

        /// <summary>
        /// 同步周期间隔时间（秒为单位）
        /// </summary>
        private int TimerIntervalOfSync = 10;

        public void loadConnectionConfig()
        {
            config = GlobalConfiguration.Load();

            //string odbc_conStr = String.Format("Dsn={0};Uid={1};Pwd={2}",
            //config.odbc_dsn,
            //config.odbc_usrid,
            //config.odbc_pwd);

            //DbHelperOdbc.connectionString = odbc_conStr;

            TimerIntervalOfSync = config.interval;

            speaker = Properties.Settings.Default.mute;

            if (isDebug)
            {
                Properties.Settings.Default.isDefault = true;
                Properties.Settings.Default.Save();
            }
            

        }



        public void onStartup()
        {
            loadConnectionConfig();

            //try
            //{
            //    DbHelperOdbc.TestConnection();
            //    DbHelperOdbc.MoleQOEMSetOption();
            //}
            //catch (Exception ex)
            //{
            //    if (config.isWriteLog)
            //    {
            //        Logger.error(TAG, ex.Message);
            //    }

            //    return;
            //}


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


        private bool firstRun = true;
        private void doSyncData()
        {

            stopSync = false;

            while (!stopSync)
            {

                //var MyIni = new IniFile();

                ////MyIni.Write("DSN", "wholesale", "Setting");
                ////MyIni.Write("Interval", "10", "Setting");
                ////MyIni.Write("Log", "0", "Setting");

                //string interval = MyIni.Read("Interval", "Setting");
                //string dsn = MyIni.Read("DSN", "Setting");
                //string Log = MyIni.Read("Log","Setting");
                //string lastUploadTime = MyIni.Read("LastUploadTime", "Setting");

                string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string iniFileName = path + "Monitor.ini";
               
                if (!File.Exists(iniFileName))
                {
                    if (NotifiCallback != null)
                        NotifiCallback(this, new BalloonTipEventArgs(5000, "INI file", "The Monitor.ini not found.", ToolTipIcon.Error));

                    //IniData defaultConfig = ModifyINIData(new IniData());

                    //parser.WriteFile(iniFileName, defaultConfig);

                    Thread.Sleep(30 * 1000);
                    continue;
                }


                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(iniFileName);

                string interval = data["Setting"]["Interval"]; 
                string dsn = data["Setting"]["DSN"];
                string Log = data["Setting"]["Log"];
                string lastUploadTime = data["Setting"]["lastUploadTime"];


                if (string.IsNullOrEmpty(interval) || string.IsNullOrEmpty(dsn) || string.IsNullOrEmpty(Log) )
                {
                    if (NotifiCallback != null)
                        NotifiCallback(this, new BalloonTipEventArgs(5000, "INI file", "The contents in the INI file could not be read.", ToolTipIcon.Error));

                    Thread.Sleep(30 * 1000);
                    continue;
                }

                int log = 0;
                
                int.TryParse(interval, out TimerIntervalOfSync);
                int.TryParse(Log, out log);

                IsWriteLog = (log == 0) ? false : true;

                // connect database
                try
                {
                    DbHelperOdbc.connectionString = getOdbcConnStr(dsn, "adm0", "systemcom");
                    DbHelperOdbc.TestConnection();
                    DbHelperOdbc.MoleQOEMSetOption();

                    if (firstRun)
                    {
                        //Properties.Settings.Default.lastID = getMaxQuoteId();
                        //Properties.Settings.Default.Save();

                        lastUploadTime = GetLastUploadTime();

                        if (!string.IsNullOrEmpty(lastUploadTime))
                        {
                            // MyIni.Write("LastUploadTime", lastUploadTime, "Setting");
                            data["Setting"]["lastUploadTime"] = lastUploadTime;
                            parser.WriteFile(iniFileName, data);
                        }

                        firstRun = false;
                    }
                }
                catch (Exception ex)
                {
                    
                    if (NotifiCallback != null)
                        NotifiCallback(this, new BalloonTipEventArgs(5000,"Database Connection" , ex.Message ,ToolTipIcon.Error));

                    //if (config.isWriteLog)
                    if (IsWriteLog)
                    {
                        Logger.error(TAG, ex.Message);
                    }

                    Thread.Sleep(TimerIntervalOfSync * 1000);
                    continue;
                }

                try
                {
                    //long newID = 0;
                    //long lastID = Properties.Settings.Default.lastID;

                    //cnt++;
                    //lastID -= cnt;

                    //
                    //newID = getMaxQuoteId();

                    //
                    //string newUploadTime = "";

                    //newUploadTime = GetLastUploadTime();

                    //if (string.IsNullOrEmpty(lastUploadTime))
                    //{
                    //    newUploadTime = lastUploadTime;
                    //}

                    //if (newID > lastID)

                    int uploadCnt = 0;

                    string sql = string.Format("SELECT count(1) as cnt, Max(upld_time) as lastUploadTime FROM DBA.t_quote_lst WHERE upld_time > '{0}' ", lastUploadTime);

                    DataSet ds = DbHelperOdbc.Query(sql);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        uploadCnt = Convert.IsDBNull(ds.Tables[0].Rows[0]["cnt"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["cnt"]);
                        lastUploadTime = Convert.IsDBNull(ds.Tables[0].Rows[0]["lastUploadTime"]) ? "" :  Convert.ToDateTime(ds.Tables[0].Rows[0]["lastUploadTime"]).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }


                    //int quoteCnt = GetQuoteUploadedCount(lastUploadTime);
                    //if (quoteCnt > 0)
                    if (uploadCnt > 0)
                    {

                       // int quoteCnt = Convert.ToInt32( newID - lastID); //getQuoteUploadedCount(lastID, newID);

                        string body = string.Format("[{0}]  {1} uploaded", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), string.Format("{0:#,###}", uploadCnt) );

                        if (_Notification == null)
                        {
                            _Notification = new Notification("New Quotes", body, -1, FormAnimator.AnimationMethod.Fade, FormAnimator.AnimationDirection.Up);
                            _Notification.MuteEvent += MuteEventClick;
                            _Notification.setVolume(speaker);

                        }

                        if (_Notification != null)
                        {
                            _Notification.Append("New Quotes", body, -1);

                            if (speaker)
                                PlayNotificationSound("normal");
                          
                            if (!_Notification.isShowing())
                            {
                                _Notification.ShowFromManager();
                            }
                        }

                        //Properties.Settings.Default.lastID = newID;
                        //Properties.Settings.Default.Save();

                        //lastUploadTime = GetLastUploadTime();

                        //if (!string.IsNullOrEmpty(lastUploadTime))
                        //{
                        //    MyIni.Write("LastUploadTime", lastUploadTime, "Setting");
                        //}

                        if (!string.IsNullOrEmpty(lastUploadTime))
                        {
                            // MyIni.Write("LastUploadTime", lastUploadTime, "Setting");
                            data["Setting"]["lastUploadTime"] = lastUploadTime;
                            parser.WriteFile(iniFileName, data);
                        }
                           

                    }

                }
                catch (Exception ex)
                {
                    //if (config.isWriteLog)
                    if (IsWriteLog)
                    {
                        Logger.error(TAG, ex.Message);
                    }
                }

                Thread.Sleep(TimerIntervalOfSync * 1000);

            }
        }

        private void MuteEventClick(object sender, MuteEventArgs e)
        {
            speaker = e.Mute;
            Properties.Settings.Default.mute = e.Mute;
            Properties.Settings.Default.Save();
        }

        private long getMaxQuoteId()
        {
            string sql = "SELECT MAX(qo_id) FROM DBA.t_quote_lst ";

            return Convert.ToInt64(DbHelperOdbc.ExecuteScalar(sql));
        }

        private int getQuoteUploadedCount(long start, long end)
        {
            string sql = string.Format("SELECT count(*) FROM DBA.t_quote_lst WHERE qo_id > {0} AND qo_id <= {1} ", start, end);

            return Convert.ToInt32(DbHelperOdbc.ExecuteScalar(sql));
        }

        private string GetLastUploadTime()
        {
            string sql = "SELECT MAX(updt_time) FROM DBA.t_quote_lst ";

            object re = DbHelperOdbc.ExecuteScalar(sql);

            return Convert.IsDBNull(re)? null:Convert.ToDateTime( re).ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        private int GetQuoteUploadedCount(string timestamp )
        {
            string sql = string.Format("SELECT count(*) FROM DBA.t_quote_lst WHERE upld_time > '{0}' ", timestamp);

            return Convert.ToInt32(DbHelperOdbc.ExecuteScalar(sql));
        }

       

        private static void PlayNotificationSound(string sound)
        {
            var soundsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");
            var soundFile = Path.Combine(soundsFolder, sound + ".wav");

            if (File.Exists(soundFile))
            {
                using (var player = new System.Media.SoundPlayer(soundFile))
                {
                    player.Play();
                }
            }

        }

        private void ShowNotification(string title, string body, int duration)
        {
            new Notification(title, body, duration, FormAnimator.AnimationMethod.Fade,
                  FormAnimator.AnimationDirection.Up).ShowFromManager();
            PlayNotificationSound("normal");
        }

        private Notification _notification = null;

        public event SynchronizerEventHandler NotifiCallback = null;

        public delegate void SynchronizerEventHandler(object sender, BalloonTipEventArgs e);

        private void ShowNotification2(string title, string body, int duration)
        {
            if (_notification != null && !_notification.isShowing())
            {
                _notification.ShowFromManager();
            }
            else
            {
                _notification = new Notification(title, body, duration, FormAnimator.AnimationMethod.Fade,
                  FormAnimator.AnimationDirection.Up);
            }



            PlayNotificationSound("normal");
        }

        public void ShowNotification()
        {
            if (_Notification == null)
            {
                _Notification = new Notification("New Quotes", "", -1, FormAnimator.AnimationMethod.Fade, FormAnimator.AnimationDirection.Up);
                _Notification.MuteEvent += MuteEventClick;
                _Notification.setVolume(speaker);

            }

            if (_Notification != null && !_Notification.isShowing())
            {
                _Notification.ShowFromManager();
            }
        }

        private string getOdbcConnStr(string dsn,string uid, string pwd)
        {
            return String.Format("Dsn={0};Uid={1};Pwd={2}",
             dsn,
             uid,
             pwd);

        }

        static IniData ModifyINIData(IniData modifiedParsedData)
        {
            modifiedParsedData.Sections.AddSection("Setting");
            modifiedParsedData.Sections.GetSectionData("Setting").Keys.AddKey("Interval", "10");
            modifiedParsedData.Sections.GetSectionData("Setting").Keys.AddKey("DSN", "wholesale");
            modifiedParsedData.Sections.GetSectionData("Setting").Keys.AddKey("Log", "0");
            modifiedParsedData.Sections.GetSectionData("Setting").Keys.AddKey("lastUploadTime", "2000-01-01 00:00:01.001");

            return modifiedParsedData;
        }



    }
}
