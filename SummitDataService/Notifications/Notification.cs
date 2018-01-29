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
           // labelTitle.Text = title;
            //if (body.Length > 100)
            //    labelBody.Font = SmallFont;
            //labelBody.Text = body;

            _animator = new FormAnimator(this, animation, direction, 500);

           // Region = Region.FromHrgn(NativeMethods.CreateRoundRectRgn(0, 0, Width - 5, Height - 5, 15, 15));
            this.FormClosed += (sender, args) => ClosingEvent.Set();
        }

        internal void RefreshBody1(string text)
        {
            if (isShow && !string.IsNullOrEmpty(text))
            {
                UpdateMessage(text);
            }
        }

       
        internal void RefreshBody2(string text)
        {
            
            if (isShow && !string.IsNullOrEmpty(text))
            {
                UpdateMessage2(text);
            }
       
        }

        private string msg = "";
        private string msg2 = "";

        internal void setMsg(string info)
        {
            msg = info;

        }

        internal void setMsg2(string status)
        {
            msg2 = status;

        }

        delegate void RefreshMsgCallback();
        internal void RefreshMsg()
        {

            if (this.InvokeRequired)
            {
                while (!this.IsHandleCreated)
                {
                    if (this.Disposing || this.IsDisposed)
                        return;
                }
                RefreshMsgCallback callback = new RefreshMsgCallback(RefreshMsg);
                this.Invoke(callback);
            }
            else
            {
                this.lbl_msg.Text = msg;
                this.lbl_msg2.Text = msg2;
            }

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
   

        delegate void UpdateMessageCallback(string v);
        private void UpdateMessage(string v)
        {
            if (this.lbl_msg.InvokeRequired)
            {
                while (!this.lbl_msg.IsHandleCreated)
                {
                    if (this.lbl_msg.Disposing || this.lbl_msg.IsDisposed)
                        return;
                }
                UpdateMessageCallback callback = new UpdateMessageCallback(UpdateMessage);
                this.lbl_msg.Invoke(callback, new object[] { v });
            }
            else
            {
                this.lbl_msg.Text = v;
            }
        }


        delegate void UpdateMessage2Callback(string v);
        private void UpdateMessage2(string v)
        {
            if (this.InvokeRequired)
            {
                while (!this.IsHandleCreated)
                {
                    if (this.Disposing || this.IsDisposed)
                        return;
                }
                UpdateMessage2Callback callback = new UpdateMessage2Callback(UpdateMessage2);
                this.Invoke(callback, new object[] { v });
            }
            else
            {
                this.lbl_msg2.Text = v;
            }
        }

        #endregion // Methods

        #region Event Handlers

        void Notification_Load(object sender, EventArgs e)
        {

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
            _extendedViewForm.ShowDialog(this);
        }

        #endregion // Event Handlers

      
        private void pb_close_Click(object sender, EventArgs e)
        {
            Hide();
        }

     
        bool durum = false;

        private void Notification_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
            Color.White, 2, ButtonBorderStyle.Solid, //×ó±ß
¡¡¡¡¡¡¡¡¡¡    Color.White, 2, ButtonBorderStyle.Solid, //ÉÏ±ß
¡¡¡¡¡¡¡¡¡¡    Color.White, 2, ButtonBorderStyle.Solid, //ÓÒ±ß
¡¡¡¡¡¡¡¡¡¡    Color.White, 2, ButtonBorderStyle.Solid);//µ×±ß
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