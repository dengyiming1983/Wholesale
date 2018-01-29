using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace WholesaleMonitor
{
    public class GlobalConfiguration
    {

        public DateTime openTime;
        public DateTime closeTime;

        public int period_Minute = 1;

        public string odbc_dsn;
        public string odbc_pwd;
        public string odbc_usrid;

        public string sqlsvr_database;
        public string sqlsvr_pwd;
        public string sqlsvr_server;
        public string sqlsvr_user;

        public string mysql_hostname;
        public string mysql_port;
        public string mysql_username;
        public string mysql_pwd;
        public string mysql_database;
        
     
        public int interval;
        public bool isDefault;
        public bool isWriteLog;
        public DateTime dailytime;

        public static GlobalConfiguration Load()
        {
            GlobalConfiguration config = new GlobalConfiguration();
            config.odbc_dsn = Properties.Settings.Default.odbc_dsn;
            config.odbc_usrid = Properties.Settings.Default.odbc_usrid;
            config.odbc_pwd = Properties.Settings.Default.odbc_pwd;

            config.isDefault = Properties.Settings.Default.isDefault;

            config.isWriteLog = Properties.Settings.Default.isWriteLog;
            config.interval = Properties.Settings.Default.interval;

            return config;
        }

        public static void Save(GlobalConfiguration config)
        {

            config.isDefault = false;
            Properties.Settings.Default.isDefault = config.isDefault;

            Properties.Settings.Default.odbc_dsn = config.odbc_dsn;
            Properties.Settings.Default.odbc_usrid = config.odbc_usrid;
            Properties.Settings.Default.odbc_pwd = config.odbc_pwd;

            Properties.Settings.Default.isWriteLog = config.isWriteLog;
            Properties.Settings.Default.interval = config.interval;


            Properties.Settings.Default.Save();
        }


  
     
    }
}
