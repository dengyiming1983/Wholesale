using gfcaDataService.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ServiceUitls.Database;

namespace gfcaDataService.View
{
    public partial class frmConfig : Form
    {
        private GlobalConfiguration config = null;
        //private string odbc_connStr;
        private string mysql_connStr;

        public frmConfig()
        {
            InitializeComponent();
        }

        private void btn_config_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_config_ok_Click(object sender, EventArgs e)
        {
            
            SaveConfig();

            this.Close();
        }

        private void SaveConfig()
        {
            
            config = new GlobalConfiguration();

            config.isDefault = true;

            config.odbc_dsn_wholesale = this.tb_odbc_dsn_wholesale.Text;
            config.odbc_usrid_wholesale = this.tb_odbc_usrid_wholesale.Text;
            config.odbc_pwd_wholesale = this.tb_odbc_pwd_wholesale.Text;

            config.isWriteLog = cb_writeLog.Checked;
            config.interval = Convert.ToInt32(nudInerval.Value);
            
            config.odbc_dsn_mkt = this.tb_odbc_dsn_mkt.Text;
            config.odbc_pwd_mkt = this.tb_odbc_pwd_mkt.Text;
            config.odbc_usrid_mkt = this.tb_odbc_usrid_mkt.Text;

            GlobalConfiguration.Save(config);


          

        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.FromHandle(Resources.ic_setting_16px.GetHicon());

            LoadConfiguration();
            
        }

        private void LoadConfiguration()
        {
  
            config = GlobalConfiguration.Load();

            this.tb_odbc_dsn_wholesale.Text = config.odbc_dsn_wholesale;
            this.tb_odbc_usrid_wholesale.Text = config.odbc_usrid_wholesale;
            this.tb_odbc_pwd_wholesale.Text = config.odbc_pwd_wholesale;

            this.cb_writeLog.Checked = config.isWriteLog;

            this.nudInerval.Value = config.interval;

            this.tb_odbc_dsn_mkt.Text = config.odbc_dsn_mkt;
            this.tb_odbc_usrid_mkt.Text = config.odbc_usrid_mkt;
            this.tb_odbc_pwd_mkt.Text = config.odbc_pwd_mkt;
        }



        private string GetOdbcConnStr(string dsn,string usrid,string pwd)
        {
            return String.Format("Dsn={0};Uid={1};Pwd={2}",
             dsn,
             usrid,
             pwd);

        }

        private void btn_odbc_testcnn_wholesale_Click(object sender, EventArgs e)
        {
            string connectionString = GetOdbcConnStr(
                this.tb_odbc_dsn_wholesale.Text,
                this.tb_odbc_usrid_wholesale.Text,
                this.tb_odbc_pwd_wholesale.Text
                );

            TestConnection(connectionString);
        }

        private void TestConnection(string connectionString)
        {
            OdbcConnection conn = new OdbcConnection(connectionString);
            try
            {
                bool connected = false;
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    connected = true;
                    conn.Close();
                }

                if (connected)
                {
                    MessageBox.Show("Connect to ODBC database successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to connect ODBC database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_odbc_testcnn_mkt_Click(object sender, EventArgs e)
        {
            string connectionString = GetOdbcConnStr(
               this.tb_odbc_dsn_mkt.Text,
               this.tb_odbc_usrid_mkt.Text,
               this.tb_odbc_pwd_mkt.Text
               );

            TestConnection(connectionString);
        }
    }
}
