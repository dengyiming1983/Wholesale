using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gfcaDataService
{
    public class GlobalConfiguration
    {


        public int period_Minute = 1;

        public string odbc_dsn_wholesale;
        public string odbc_pwd_wholesale;
        public string odbc_usrid_wholesale;

        public int interval;
        public bool isDefault;
        public bool isWriteLog;
        
        public string odbc_dsn_mkt;
        public string odbc_pwd_mkt;
        public string odbc_usrid_mkt;


        static string filename = Environment.CurrentDirectory + "\\setup.cfg";

        
        public static void Save(GlobalConfiguration config)
        {
            config.isDefault = false;

            string json_si = Newtonsoft.Json.JsonConvert.SerializeObject(config);
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

                config = JsonConvert.DeserializeObject<GlobalConfiguration>(jsonStr);

            }
            else
            {
                config = new GlobalConfiguration();

                config.isDefault = true;

                config.odbc_dsn_wholesale = "";
                config.odbc_usrid_wholesale = "";
                config.odbc_pwd_wholesale = "";

                config.isWriteLog = false;
                config.interval = 1;

                config.odbc_dsn_mkt = "";
                config.odbc_pwd_mkt = "";
                config.odbc_usrid_mkt = "";

            }

            return config;
        }

    }
}
