using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WholesaleMonitor
{
    class BalloonTipEventArgs:EventArgs
    {
        public int timeout;
        public string tipTitle;
        public string tipText;
        public ToolTipIcon tipIcon;
    

        public BalloonTipEventArgs(int timeout, string title, string text, ToolTipIcon icon)
        {
            this.timeout = timeout;
            this.tipTitle = title;
            this.tipText = text;
            this.tipIcon = icon;
        }
    }
}
