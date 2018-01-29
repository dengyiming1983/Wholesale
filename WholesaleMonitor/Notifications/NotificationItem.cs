using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Notifications
{
    public partial class NotificationItem : UserControl
    {
        public NotificationItem()
        {
            InitializeComponent();

            //Region = Region.FromHrgn(NativeMethods.CreateRoundRectRgn(0, 0, Width , Height , 5, 5));
        }

        public void setBody(string v)
        {
            this.label1.Text = v;
        }
    }
}
