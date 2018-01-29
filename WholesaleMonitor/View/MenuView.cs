using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static WholesaleMonitor.DataSynchronizer;

namespace WholesaleMonitor.View
{
    public class MenuView
    {

        private NotifyIcon _notifyIcon;
        private ContextMenu contextMenu1;

        private bool _isFirstRun;
        /* 2017-08-04 remove enable item on menu */
        private MenuItem enableItem;

        private frmConfig configForm;

        private bool isShowingBalloonTip = false;

        private DataSynchronizer syncer = new DataSynchronizer();
        private MenuItem settingMenuItem;

        // private MenuItem testItem;
        private MenuItem DisplayItem;

        public MenuView()
        {

            LoadMenu();

            _notifyIcon = new NotifyIcon();
            UpdateTrayIcon();
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenu = contextMenu1;
            _notifyIcon.BalloonTipClosed += new EventHandler(notifyIcon_BalloonTipClosed);

            //GlobalConfiguration config = GlobalConfiguration.Load();
            //if (config.isDefault)
            //{
            //    _isFirstRun = true;
            //    ShowConfigForm();
            //}
            //else
            //{
            //    //if (!this.enableItem.Checked && checkConnections(config))
            //    //{
            //    //    controller.start();
            //    //    this.enableItem.Checked = true;
            //    //}

            //    if (!this.enableItem.Checked)
            //    {
            //        syncer.onStartup();
            //        setEnabled(false);
            //    }
            //}


            syncer.NotifiCallback += new SynchronizerEventHandler(Synchronizer_ToastNotification);

            syncer.onStartup();

        }

        private void Synchronizer_ToastNotification(object sender, BalloonTipEventArgs e)
        {
            _notifyIcon.ShowBalloonTip(e.timeout, e.tipTitle, e.tipText, e.tipIcon);
        }

        private void setEnabled(bool b)
        {
            if (b)
            {
                this.enableItem.Checked = false;
                settingMenuItem.Enabled = true;
            }
            else
            {
                this.enableItem.Checked = true;
                settingMenuItem.Enabled = false;
            }
        }



        private void LoadMenu()
        {
            //this.testItem = CreateMenuItem("Test Notifi", new EventHandler(this.Test_Click));
            DisplayItem = CreateMenuItem("Display", new EventHandler(this.DisplayItem_Click));

            /* 2017-08-04 remove enable item on menu */
            // settingMenuItem = CreateMenuItem("Setting", new EventHandler(this.Config_Click));


            this.contextMenu1 = new ContextMenu(new MenuItem[] {
                DisplayItem,
                //testItem,
               //  this.enableItem =  CreateMenuItem("Enable",  new EventHandler(this.EnableItem_Click)),
                //new MenuItem("-"),
                // CreateMenuItem("Manual", new EventHandler(this.Upload_Manual_Click)),
                new MenuItem("-"),
                //CreateMenuItem("Upload Setting", new EventHandler(this.Upload_Setting_Click)),
               // settingMenuItem,
                CreateMenuItem("About...", new EventHandler(this.AboutItem_Click)),
                new MenuItem("-"),
                CreateMenuItem("Exit", new EventHandler(this.Quit_Click))
            });
        }

        private void Test_Click(object sender, EventArgs e)
        {
            //ShowNotification("New Quote", "has new quote to add ", 5);

        }

        private void DisplayItem_Click(object sender, EventArgs e)
        {
           syncer.ShowNotification();
        }

        private void EnableItem_Click(object sender, EventArgs e)
        {
            this.enableItem.Checked = !enableItem.Checked;

            GlobalConfiguration config = GlobalConfiguration.Load();

            if (this.enableItem.Checked)
            {
                syncer.onStartup();
                setEnabled(false);
            }
            else
            {
                syncer.onShutDown();
                setEnabled(true);
            }


        }

        public bool checkConnections(GlobalConfiguration config)
        {


            bool haveError = false;
            string odbc_conStr = String.Format("Dsn={0};Uid={1};Pwd={2}",
              config.odbc_dsn,
              config.odbc_usrid,
              config.odbc_pwd);

            string mysql_connStr = String.Format("server={0};uid={1};pwd={2};database={3}",
                  config.mysql_hostname,
                  config.mysql_username,
                  config.mysql_pwd,
                  config.mysql_database);

            DbHelperOdbc.connectionString = odbc_conStr;

            if (!haveError)
            {
                try
                {
                    DbHelperOdbc.TestConnection();
                    DbHelperOdbc.MoleQOEMSetOption();
                }
                catch (Exception ex)
                {
                    //this.notifyIcon1.ShowBalloonTip(5000, "Information:", ex.Message, ToolTipIcon.Error);
                    showMyBalloonTip(5000, "Information:", ex.Message, ToolTipIcon.Error);
                    haveError = true;
                }
            }

            return !haveError;
        }

        private void notifyIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            isShowingBalloonTip = false;
        }

        private void showMyBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon icon)
        {
            if (!isShowingBalloonTip)
            {
                _notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, icon);
                isShowingBalloonTip = true;
            }
        }

        private void UpdateTrayIcon()
        {
            int dpi;
            Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
            dpi = (int)graphics.DpiX;
            graphics.Dispose();
            Bitmap icon = null;
            if (dpi < 97)
            {
                // dpi = 96;
                //icon = Resources.cloud_16px;
                icon = WholesaleMonitor.Properties.Resources.moleq_logo_16;
            }
            //else if (dpi < 121)
            //{
            //    // dpi = 120;
            //    icon = Resources.ss20;
            //}
            else
            {
                //icon = Resources.cloud_24px;
                icon = WholesaleMonitor.Properties.Resources.moleq_logo_24;
            }

            _notifyIcon.Icon = Icon.FromHandle(icon.GetHicon());

            string show_text = string.Format("Product: {0}\r\n Version: {1}\r\n", Application.ProductName, Application.ProductVersion);
            _notifyIcon.Text = show_text;
        }

        private MenuItem CreateMenuItem(string text, EventHandler click)
        {
            return new MenuItem(text, click);
        }

        private MenuItem CreateMenuGroup(string text, MenuItem[] items)
        {
            return new MenuItem(text, items);
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Do you want to exit the wholdsale monitor ?",
                "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {

                // TaskScheduler.TerminateAllTask();
                if (syncer != null)
                {
                    syncer.onShutDown();
                }

                _notifyIcon.Visible = false;
                Application.Exit();
            }

            
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            // Process.Start("http://www.moleq.com");
            new AboutBox1().ShowDialog();
        }

        private void Config_Click(object sender, EventArgs e)
        {
            //if (!isEditSettingAvable())
            //    return;

            ShowConfigForm();
        }



        private void ShowConfigForm()
        {
            if (configForm != null)
            {
                configForm.Activate();
            }
            else
            {
                configForm = new frmConfig();
                configForm.Show();
                configForm.FormClosed += configForm_FormClosed;
            }
        }

        void configForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            configForm = null;
            //Util.Utils.ReleaseMemory(true);
            //ShowFirstTimeBalloon();
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

       
    }
}
