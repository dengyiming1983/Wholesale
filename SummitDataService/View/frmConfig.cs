using SummitDataService.Properties;
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

namespace SummitDataService.View
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

            string path = tb_input.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Please set an import item file path.");
                this.tb_input.Focus();
                return;
            }

            if (!Directory.Exists(path))
            {
                MessageBox.Show("Import item folder does not exist. Please re set the path.");
                this.tb_input.Focus();
                return;
            }

            // 以下代码设置import 图片路径, 并未知道是否使用。如需要使用移除注释并在空间中visible为可见
            //path = tbImageFolder.Text.Trim();
            //if (string.IsNullOrEmpty(path))
            //{
            //    MessageBox.Show("Please set an import image file path. ");
            //    this.tbImageFolder.Focus();
            //    return;
            //}

            //if (!Directory.Exists(path))
            //{
            //    MessageBox.Show("Import image folder does not exist. Please re set the path.");
            //    this.tbImageFolder.Focus();
            //    return;
            //}

            SaveConfig();

            this.Close();
        }

        private void SaveConfig()
        {
            
            //Properties.Settings.Default.odbc_dsn = this.tb_odbc_dsn.Text;
            //Properties.Settings.Default.odbc_usrid = this.tb_odbc_usrid.Text;
            //Properties.Settings.Default.odbc_pwd = this.tb_odbc_pwd.Text;

            //Properties.Settings.Default.isWriteLog = cb_writeLog.Checked;
            //Properties.Settings.Default.interval = Convert.ToInt32(nudInerval.Value);

            //Properties.Settings.Default.importFolderOfItem = this.tb_input.Text;
            //Properties.Settings.Default.ExportFolderOfItem = this.tb_output.Text;

            //Properties.Settings.Default.isInsertItems = cb_insertItems.Checked;
            //Properties.Settings.Default.isInsertCustomer = cb_insertCustomer.Checked;

            //Properties.Settings.Default.isDefault = false; // 打开后，初始运行程序直接跳到设置界面

            //Properties.Settings.Default.Save();


            config = new GlobalConfiguration();

            config.isDefault = true;

            config.odbc_dsn = this.tb_odbc_dsn.Text;
            config.odbc_usrid = this.tb_odbc_usrid.Text;
            config.odbc_pwd = this.tb_odbc_pwd.Text;

            config.isWriteLog = cb_writeLog.Checked;
            config.interval = Convert.ToInt32(nudInerval.Value);

            config.isInsertItems = cb_insertItems.Checked;
            config.isInsertCustomer = cb_insertCustomer.Checked;

            config.importFolderOfItem = this.tb_input.Text;
            config.exportFolderOfItem = this.tb_output.Text;

            GlobalConfiguration.Save(config);

            DbHelperOdbc.connectionString = getOdbcConnStr();

        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.FromHandle(Resources.ic_setting_16px.GetHicon());

            LoadConfiguration();
            
        }

        private void LoadConfiguration()
        {
            //this.tb_odbc_dsn.Text = Properties.Settings.Default.odbc_dsn;
            //this.tb_odbc_usrid.Text = Properties.Settings.Default.odbc_usrid;
            //this.tb_odbc_pwd.Text = Properties.Settings.Default.odbc_pwd;

            //this.cb_writeLog.Checked = Properties.Settings.Default.isWriteLog;
            //this.cb_insertItems.Checked = Properties.Settings.Default.isInsertItems;
            //this.cb_insertCustomer.Checked = Properties.Settings.Default.isInsertCustomer;

            //this.nudInerval.Value = Properties.Settings.Default.interval;

            //this.tb_input.Text = Properties.Settings.Default.importFolderOfItem;
            //this.tb_output.Text = Properties.Settings.Default.ExportFolderOfItem;

            config = GlobalConfiguration.Load();

            this.tb_odbc_dsn.Text = config.odbc_dsn;
            this.tb_odbc_usrid.Text = config.odbc_usrid;
            this.tb_odbc_pwd.Text = config.odbc_pwd;

            this.cb_writeLog.Checked = config.isWriteLog;
            this.cb_insertItems.Checked = config.isInsertItems;
            this.cb_insertCustomer.Checked = config.isInsertCustomer;

            this.nudInerval.Value = config.interval;

            this.tb_input.Text = config.importFolderOfItem;
            this.tb_output.Text = config.exportFolderOfItem;
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
        private void btnItemBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Reset();

            if (!string.IsNullOrEmpty(tb_input.Text))
            {
                folderBrowserDialog1.SelectedPath = tb_input.Text;
            }

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tb_input.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btn_output_browse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Reset();

            if (!string.IsNullOrEmpty(tb_output.Text))
            {
                folderBrowserDialog1.SelectedPath = tb_output.Text;
            }

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tb_output.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
