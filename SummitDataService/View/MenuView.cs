using Notifications;
using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace SummitDataService.View
{
    public class MenuView
    {

        private NotifyIcon _notifyIcon;
        private ContextMenu contextMenu1;

        private bool _isFirstRun;
        private MenuItem enableItem;
        private MenuItem modeItem;
        private MenuItem eixtOption;

        private frmConfig configForm;
        private frmRequestPwd requestPwdForm;

        private bool isShowingBalloonTip = false;

        private DataSynchronizer syncer = new DataSynchronizer();
        private MenuItem settingMenuItem;
        private MenuItem DisplayItem;

        public MenuView()
        {

            syncer.workstart += new DataSynchronizer.EventHandler(WorkStart);
            syncer.workstop += new DataSynchronizer.EventHandler(WorkStop);
           // syncer.workToastMessage += new DataSynchronizer.EventHandler(WorkToastMessage);
            syncer.ToastMessage += new DataSynchronizer.ToastMessageHandler(this.ToastMessage);

            LoadMenu();

            _notifyIcon = new NotifyIcon();
            UpdateTrayIcon();
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenu = contextMenu1;
            _notifyIcon.BalloonTipClosed += new EventHandler(notifyIcon_BalloonTipClosed);

            GlobalConfiguration config = GlobalConfiguration.Load();
            if (config.isDefault)
            {
                _isFirstRun = true;
                ShowConfigForm();
            }
            else
            {
                //if (!this.enableItem.Checked && checkConnections(config))
                //{
                //    controller.start();
                //    this.enableItem.Checked = true;
                //}

                if (!this.enableItem.Checked)
                {
                    syncer.onStartup();
                    setEnabled(false);
                }
            }

        }

        private void ToastMessage(object sender, DataSynchronizer.InfoEventArgs e)
        {
            //showMyBalloonTip(29000, "", e.Message, ToolTipIcon.Warning);

            if (_Notification == null)
            {
                _Notification = new Notification("", "", -1, FormAnimator.AnimationMethod.Fade, FormAnimator.AnimationDirection.Up);

            }

            //if (_Notification != null && !_Notification.isShowing())
            //{
            //    _Notification.ShowFromManager();
            //}

            if (_Notification != null)
            {
                if (!string.IsNullOrEmpty(e.Info))
                    _Notification.setMsg(e.Info);

                if (!string.IsNullOrEmpty(e.status))
                    _Notification.setMsg2(e.status);

                _Notification.RefreshMsg();
            }

            
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

        private void WorkStart(object sender)
        {
            this.enableItem.Enabled = false;
            this.DisplayItem.Enabled = true;
            eixtOption.Enabled = false;
        }

        private void WorkStop(object sender)
        {
            this.enableItem.Enabled = true;
            this.DisplayItem.Enabled = false;
            eixtOption.Enabled = true;
        }

        private void WorkToastMessage(object sender , EventArgs args)
        {
            if (_Notification != null && !_Notification.isShowing())
            {
                _Notification.ShowFromManager();
            }

            //_Notification.RefreshMessage(args.message);
        }

        private void LoadMenu()
        {
            DisplayItem = CreateMenuItem("Progress...", new EventHandler(this.DisplayItem_Click));
            DisplayItem.Enabled = false;

            settingMenuItem = CreateMenuItem("Setting", new EventHandler(this.Config_Click));

            this.contextMenu1 = new ContextMenu(new MenuItem[] {
                DisplayItem,
                 this.enableItem =  CreateMenuItem("Enable",  new EventHandler(this.EnableItem_Click)),
                //new MenuItem("-"),
                // CreateMenuItem("Manual", new EventHandler(this.Upload_Manual_Click)),
                new MenuItem("-"),
                //CreateMenuItem("Upload Setting", new EventHandler(this.Upload_Setting_Click)),
                settingMenuItem,
                CreateMenuItem("About...", new EventHandler(this.AboutItem_Click)),
                new MenuItem("-"),
                 this.eixtOption = CreateMenuItem("Exit", new EventHandler(this.Quit_Click))
            });
        }

        private void DisplayItem_Click(object sender, EventArgs e)
        {
            ShowNotification();
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
                icon = Properties.Resources.moleq_logo_16;
            }
            //else if (dpi < 121)
            //{
            //    // dpi = 120;
            //    icon = Resources.ss20;
            //}
            else
            {
                //icon = Resources.cloud_24px;
                icon = Properties.Resources.moleq_logo_24;
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
            // TaskScheduler.TerminateAllTask();

            if (syncer != null && syncer.IsProcessingData())
            {
                if (MessageBox.Show("Service is processing data, are you sure exit now? ", "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) 
                    == DialogResult.No)
                {
                    return;
                }
                
            }
 
            //if (syncer != null)
            //{
            //    syncer.onShutDown();
            //}

            //_notifyIcon.Visible = false;
            //Application.Exit();

            ShowRequestPwdForm();


        }

        private void AppQuit()
        {
            if (syncer != null)
            {
                syncer.onShutDown();
            }

            _notifyIcon.Visible = false;
            Application.Exit();
        }

        private void ShowRequestPwdForm()
        {
            if (requestPwdForm != null)
            {
                requestPwdForm.Activate();
            }
            else
            {
                requestPwdForm = new frmRequestPwd();
                requestPwdForm.Show();
                requestPwdForm.FormClosed += requestPwdForm_FormClosed;
                requestPwdForm.PwdConfirmed += requestPwdForm_PwdConfirmed;
            }
        }

        void requestPwdForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            requestPwdForm = null;
        }

        void requestPwdForm_PwdConfirmed(object sender, EventArgs e)
        {
            AppQuit();
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            //syncer.ExportItemAsCSVFile();

            //syncer.doExportData();

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

        private Notification _Notification = null;
        public void ShowNotification()
        {
            if (_Notification == null)
            {
                _Notification = new Notification("", "", -1, FormAnimator.AnimationMethod.Fade, FormAnimator.AnimationDirection.Up);

            }

            if (_Notification != null && !_Notification.isShowing())
            {
                _Notification.ShowFromManager();
            }
        }



    }
}
