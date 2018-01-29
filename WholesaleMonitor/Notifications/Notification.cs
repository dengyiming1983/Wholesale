// =====COPYRIGHT=====
// Code originally retrieved from http://www.vbforums.com/showthread.php?t=547778 - no license information supplied
// =====COPYRIGHT=====

using Notifications.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Notifications
{
    public partial class Notification : Form
    {
        public ManualResetEvent ClosingEvent = new ManualResetEvent(false);
        //public event EventHandler<EventArgs> OnNotificationClicked;
        static readonly NotificationManager NotificationManager;
        Form _extendedViewForm;
        static readonly List<Notification> OpenNotifications = new List<Notification>();
        bool _allowFocus;
        readonly FormAnimator _animator;
        IntPtr _currentForegroundWindow;
        static readonly Font SmallFont = new Font("Tahoma", 8, FontStyle.Regular, GraphicsUnit.Point, 0);
        static readonly Font DefaultFont = new Font("Tahoma", 11.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
     
        private int separation = 1;
        private bool isShow = false;
      
        public delegate void MuteEventHandler(object sender, MuteEventArgs e);
        public event MuteEventHandler MuteEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="duration"></param>
        /// <param name="animation"></param>
        /// <param name="direction"></param>
        public Notification(string title, string body, int duration, FormAnimator.AnimationMethod animation, FormAnimator.AnimationDirection direction, Form extendedViewForm = null)
        {
            _extendedViewForm = extendedViewForm;
            InitializeComponent();

            if (duration < 0)
                duration = int.MaxValue;
            else
                duration = duration * 1000;

            lifeTimer.Interval = duration;
            labelTitle.Text = title;
            //if (body.Length > 100)
            //    labelBody.Font = SmallFont;
            //labelBody.Text = body;

            _animator = new FormAnimator(this, animation, direction, 500);

            Region = Region.FromHrgn(NativeMethods.CreateRoundRectRgn(0, 0, Width - 5, Height - 5, 15, 15));
            this.FormClosed += (sender, args) => ClosingEvent.Set();
        }

        #region Methods

        /// <summary>
        /// Displays the form
        /// </summary>
        /// <remarks>
        /// Required to allow the form to determine the current foreground window before being displayed
        /// </remarks>
        public new void Show()
        {
            // Determine the current foreground window so it can be reactivated each time this form tries to get the focus
            _currentForegroundWindow = NativeMethods.GetForegroundWindow();

            base.Show();

            isShow = true;

           
            

            RefreshView();


        }

        public new void Hide()
        {
            base.Hide();

            isShow = false;
        }
        /// <summary>
        /// For safe usage form non windows forms application
        /// </summary>
        public void ShowFromManager()
        {
            NotificationManager.Show(this);
        }

        public void Append(string v1, string body, int v2)
        {

            NotificationItem ni = new NotificationItem();
            ni.setBody(body);

            addNewNotificationItem(ni);

           // listView_WOC1.add(ni);

            updateUI();
        }

        delegate void updateUICallback();
        public void updateUI()
        {
            if (this.InvokeRequired)
            {
                while (!this.IsHandleCreated)
                {
                    if (this.Disposing || this.IsDisposed)
                        return;
                }
                updateUICallback callback = new updateUICallback(updateUI);
                this.Invoke(callback);
            }
            else
            {
                updateView();
            }
        }
        private void updateView()
        {
            Point newPoint = new Point(0, 0);
            panel1.AutoScrollPosition = newPoint;
        }


        delegate void addNotificationItemCallback(NotificationItem ni);
        public void addNewNotificationItem(NotificationItem ni)
        {
            if (this.InvokeRequired)
            {
                while (!this.IsHandleCreated)
                {
                    if (this.Disposing || this.IsDisposed)
                        return;
                }
                addNotificationItemCallback callback = new addNotificationItemCallback(addNewNotificationItem);
                this.Invoke(callback, new object[] { ni });
            }
            else
            {
                addNotificationItem2Panel(ni);
            }
        }

        public void addNotificationItem2Panel(NotificationItem item)
        {
            if (item != null)
            {
                
                this.panel1.Controls.Add(item);


                //if (itemCnt > 20)
                //{
                //    this.panel1.Controls.RemoveAt(0);

                //    itemCnt = this.panel1.Controls.Count;
                //}

                if (this.isShowing())
                {
                    RefreshView();
                }
               

                //if (itemCnt > 5)
                //{
                //    // panel1.AutoScroll = true;

                //    //panel1.VerticalScroll.Visible = true;//ÊúµÄ
                //    //panel1.HorizontalScroll.Visible = false;//ºáµÄ

                //    panel1.AutoScroll = false;
                //    panel1.HorizontalScroll.Enabled = false;
                //    panel1.HorizontalScroll.Visible = false;
                //    panel1.HorizontalScroll.Maximum = 0;
                //    panel1.AutoScroll = true;

                //    panel1.VerticalScroll.Value = 0;
                //}


            }

        }

        public void RefreshView()
        {
            int itemCnt = this.panel1.Controls.Count;
            int loc_x = 2;
            int loc_y = 2;

            if (itemCnt > 0)
            {
                Point newPoint = new Point(0, 0);
                panel1.AutoScrollPosition = newPoint;

                for (int i = itemCnt - 1; i >= 0; i--)
                {
                    Point point = new Point(loc_x, loc_y);

                    this.panel1.Controls[i].Location = point;

                    loc_y += this.panel1.Controls[i].Height + separation;
                }

                //if (itemCnt > 5)
                //{
                //    // ¹ö¶¯ÖÃ¶¥
                //    this.panel1.ScrollControlIntoView(this.panel1.Controls[0]);

                //    //Point newPoint = new Point(0, 0);
                //    //panel1.AutoScrollPosition = newPoint;
                //}
                this.panel1.Visible = true;

            }
            else
            {
                this.panel1.Visible = false;
            }
         
        }




        //static bool runningAlready = false;
        //public void ShowInSeparateThread()
        //{
        //    var thread = new Thread(() =>
        //    {
        //        if (!runningAlready)
        //            System.Windows.Forms.Application.Run(this);
        //        else
        //        {
        //            this.Show();
        //        }
        //    })
        //    { IsBackground = false };
        //    thread.SetApartmentState(ApartmentState.STA);
        //    thread.Start();
        //    thread.Join();
        //}

        #endregion // Methods

        #region Event Handlers

        void Notification_Load(object sender, EventArgs e)
        {

            this.panel1.Region = Region.FromHrgn(NativeMethods.CreateRoundRectRgn(0, 0, Width, Height, 5, 5));

            // Display the form just above the system tray.
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Width,
                                      Screen.PrimaryScreen.WorkingArea.Height - Height);

            // Move each open form upwards to make room for this one
            foreach (Notification openForm in OpenNotifications)
            {
                openForm.SetPropertyThreadSafe(() => openForm.Top, openForm.Top - Height);
                /* Notice! The line below changed by me (aleksandresukhitashvili@gmail.com)
                 *  with the line above because it threw 
                 *  an exception when I created another 
                 *  notification from different thread
                 */
                //openForm.Top -= Height;
            }

            OpenNotifications.Add(this);
            lifeTimer.Start();


            try
            {
                NativeMethods.SetWindowPos(this.Handle, -1, 0, 0, 0, 0, 1 | 2);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

      

        void Notification_Activated(object sender, EventArgs e)
        {
            // Prevent the form taking focus when it is initially shown
            if (!_allowFocus)
            {
                // Activate the window that previously had focus
                NativeMethods.SetForegroundWindow(_currentForegroundWindow);
            }
        }

        void Notification_Shown(object sender, EventArgs e)
        {
            // Once the animation has completed the form can receive focus
            _allowFocus = true;

            // Close the form by sliding down.
            _animator.Duration = 0;
            _animator.Direction = FormAnimator.AnimationDirection.Down;
        }

        public bool isShowing()
        {
            return isShow;
        }

        void Notification_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Move down any open forms above this one
            foreach (Notification openForm in OpenNotifications)
            {
                if (openForm == this)
                {
                    // Remaining forms are below this one
                    break;
                }
                openForm.Top += Height;
            }

            OpenNotifications.Remove(this);
        }

        void lifeTimer_Tick(object sender, EventArgs e)
        {
            // Close();
            Hide();
        }

        void Notification_Click(object sender, EventArgs e)
        {
            //Hide();
            ////Close();
            //InvokeClicked();
        }

        void labelTitle_Click(object sender, EventArgs e)
        {
            //InvokeClicked();
            ////Close();
            //Hide();
        }

        void labelRO_Click(object sender, EventArgs e)
        {
            InvokeClicked();
            //Close();
            Hide();
        }

        void InvokeClicked()
        {
            _extendedViewForm?.ShowDialog(this);
        }

        #endregion // Event Handlers

        private void Clear_all_Clicked(object sender, EventArgs e)
        {
            this.panel1.Controls.Clear();
        }

        private void pb_close_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void lbl_clear_all_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.panel1.Visible = false;
            this.panel1.Controls.Clear();
        }

        bool durum = false;

        private void pb_toggle_volume_Click(object sender, EventArgs e)
        {
            if (durum == true)
            {
                pb_toggle_volume.Image = WholesaleMonitor.Properties.Resources.no_sound_png;
                durum = false;

            }
            else
            {
                pb_toggle_volume.Image = WholesaleMonitor.Properties.Resources.sound_png;
                durum = true;
            }

            if (MuteEvent != null)
                MuteEvent(this, new MuteEventArgs(durum));
        }

        public void setVolume(bool v)
        {
            if (v == true)
            {
                pb_toggle_volume.Image = WholesaleMonitor.Properties.Resources.sound_png;
            }
            else
            {
                pb_toggle_volume.Image = WholesaleMonitor.Properties.Resources.no_sound_png;

            }
        }


    }


    public class MuteEventArgs : EventArgs
    {
        public bool Mute;
        public MuteEventArgs(bool mute)
        {
            this.Mute = mute;
        }
    }
}