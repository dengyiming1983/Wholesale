using WholesaleMonitor.Properties;
using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WholesaleMonitor.Properties;

namespace WholesaleMonitor.View
{
    public partial class frmConfig : Form
    {


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
            DbHelperOdbc.connectionString = getOdbcConnStr();

            WholesaleMonitor.Properties.Settings.Default.odbc_dsn = this.tb_odbc_dsn.Text;
            Properties.Settings.Default.odbc_usrid = this.tb_odbc_usrid.Text;
            Properties.Settings.Default.odbc_pwd = this.tb_odbc_pwd.Text;

            Properties.Settings.Default.interval = Convert.ToInt32( this.nudInterval.Value);
            Properties.Settings.Default.isWriteLog = cb_writeLog.Checked;

            Properties.Settings.Default.isDefault = false;

            Properties.Settings.Default.Save();

        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.FromHandle(Resources.ic_setting_16px.GetHicon());

            LoadConfiguration();

        }

        private void LoadConfiguration()
        {
            //config = GlobalConfiguration.Load();

            this.tb_odbc_dsn.Text = Properties.Settings.Default.odbc_dsn;
            this.tb_odbc_usrid.Text = Properties.Settings.Default.odbc_usrid;
            this.tb_odbc_pwd.Text = Properties.Settings.Default.odbc_pwd;
            
            this.nudInterval.Value = Properties.Settings.Default.interval;
            this.cb_writeLog.Checked = Properties.Settings.Default.isWriteLog;

        }

        private string getOdbcConnStr()
        {
            return String.Format("Dsn={0};Uid={1};Pwd={2}",
             this.tb_odbc_dsn.Text,
             this.tb_odbc_usrid.Text,
             this.tb_odbc_pwd.Text);

        }

        private void btn_odbc_testcnn_Click(object sender, EventArgs e)
        {
            OdbcConnection conn = new OdbcConnection(getOdbcConnStr());
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

        private void btn_clear_cache_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "The last upload quote number will be reset. \nDo you want to continue?", 
                "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {

                Properties.Settings.Default.lastID = 0;
                Properties.Settings.Default.Save();

                MessageBox.Show("Reset complete!");
            }
        }

       
    }
}
