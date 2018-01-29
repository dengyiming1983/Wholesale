using SummitDataService.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SummitDataService.View
{
    public partial class frmRequestPwd : Form
    {
        public event EventHandler PwdConfirmed;
        public delegate void EventHandler(object sender, EventArgs e);
        public frmRequestPwd()
        {
            InitializeComponent();
        }

        private void btn_quit_Click(object sender, EventArgs e)
        {

           
            if (PwdConfirmed != null)
            {
                PwdConfirmed(this, new EventArgs());
            }

           
        }

        private void tb_pwd_TextChanged(object sender, EventArgs e)
        {
            string pwd = tb_pwd.Text;

            string confirmPwd = DateTime.Now.ToString("yyyyMMdd");

            if (pwd.Length < confirmPwd.Length)
                return;

            if (pwd == confirmPwd)
            {
                tb_pwd.Enabled = false;
                btn_quit.Visible = true;

            }
            else
            {
                MessageBox.Show("Invaild password. Please try again.");
                tb_pwd.Text = "";
            }
        }

        private void frmRequestPwd_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.FromHandle(Resources.ic_quit_16px.GetHicon());

            tb_pwd.Enabled = true;
            btn_quit.Visible = false;
        }
    }
}
