using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace SummitDataService
{
    public class GlobalConfiguration
    {

        public DateTime openTime;
        public DateTime closeTime;

        public int period_Minute = 1;

        public string odbc_dsn;
        public string odbc_pwd;
        public string odbc_usrid;

        public int interval;
        public bool isDefault;
        public bool isWriteLog;
        public bool isInsertItems;
        public bool isInsertCustomer;
        public DateTime dailytime;
        public string importFolderOfItem;
        public string exportFolderOfItem;


        static string filename = Environment.CurrentDirectory + "\\setup.cfg";

        /*
        public static GlobalConfiguration Load()
        {
            GlobalConfiguration config = new GlobalConfiguration();

            config.isDefault = Properties.Settings.Default.isDefault;

            config.odbc_dsn = Properties.Settings.Default.odbc_dsn;
            config.odbc_pwd = Properties.Settings.Default.odbc_pwd;
            config.odbc_usrid = Properties.Settings.Default.odbc_usrid;

            config.isWriteLog = Properties.Settings.Default.isWriteLog;
            config.isInsertItems = Properties.Settings.Default.isInsertItems;
            config.interval = Properties.Settings.Default.interval;

            config.importFolderOfItem = Properties.Settings.Default.importFolderOfItem;
            config.exportFolderOfItem = Properties.Settings.Default.ExportFolderOfItem;

            return config;
        }

        public static void Save(GlobalConfiguration config)
        {

            config.isDefault = false;

            Properties.Settings.Default.isWriteLog = config.isWriteLog;
            Properties.Settings.Default.interval = config.interval;

            Properties.Settings.Default.importFolderOfItem = config.importFolderOfItem;
            Properties.Settings.Default.ExportFolderOfItem = config.exportFolderOfItem;

            Properties.Settings.Default.Save();
        }
        */

        public static void Save(GlobalConfiguration config)
        {
            config.isDefault = false;

            var serializer = new JavaScriptSerializer();
            var json_si = serializer.Serialize(config);

            string EncryptedStr = EncryptUtils.Base64Encrypt(json_si);

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            File.WriteAllText(filename, EncryptedStr, Encoding.UTF8);
            //隐藏文件
            File.SetAttributes(filename, FileAttributes.Hidden);
        }

        public static GlobalConfiguration Load()
        {
            GlobalConfiguration config = null;
            if (File.Exists(filename))
            {

                string str = File.ReadAllText(filename);
                string jsonStr = EncryptUtils.Base64Decrypt(str);

               
                config = new JavaScriptSerializer().Deserialize<GlobalConfiguration>(jsonStr);
            }
            else
            {
                config = new GlobalConfiguration();

                config.isDefault = true;

                config.odbc_dsn = "";
                config.odbc_usrid = "";
                config.odbc_pwd = "";

                config.isWriteLog = false;
                config.isInsertItems = false;
                config.isInsertCustomer = false ;
                config.interval = 1;

                config.importFolderOfItem = "";
                config.exportFolderOfItem = "";

            }

            return config;
        }

    }
}
