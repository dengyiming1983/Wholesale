namespace gfcaDataService.View
{
    partial class frmConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tb_odbc_dsn_wholesale = new System.Windows.Forms.TextBox();
            this.tb_odbc_pwd_wholesale = new System.Windows.Forms.TextBox();
            this.tb_odbc_usrid_wholesale = new System.Windows.Forms.TextBox();
            this.btn_odbc_testcnn_wholesale = new System.Windows.Forms.Button();
            this.btn_config_ok = new System.Windows.Forms.Button();
            this.btn_config_cancel = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.nudInerval = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.cb_writeLog = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_odbc_dsn_mkt = new System.Windows.Forms.TextBox();
            this.btn_odbc_testcnn_mkt = new System.Windows.Forms.Button();
            this.tb_odbc_pwd_mkt = new System.Windows.Forms.TextBox();
            this.tb_odbc_usrid_mkt = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudInerval)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_odbc_dsn_wholesale
            // 
            this.tb_odbc_dsn_wholesale.Location = new System.Drawing.Point(83, 26);
            this.tb_odbc_dsn_wholesale.Name = "tb_odbc_dsn_wholesale";
            this.tb_odbc_dsn_wholesale.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_dsn_wholesale.TabIndex = 0;
            // 
            // tb_odbc_pwd_wholesale
            // 
            this.tb_odbc_pwd_wholesale.Location = new System.Drawing.Point(83, 85);
            this.tb_odbc_pwd_wholesale.Name = "tb_odbc_pwd_wholesale";
            this.tb_odbc_pwd_wholesale.PasswordChar = '*';
            this.tb_odbc_pwd_wholesale.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_pwd_wholesale.TabIndex = 2;
            // 
            // tb_odbc_usrid_wholesale
            // 
            this.tb_odbc_usrid_wholesale.Location = new System.Drawing.Point(83, 55);
            this.tb_odbc_usrid_wholesale.Name = "tb_odbc_usrid_wholesale";
            this.tb_odbc_usrid_wholesale.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_usrid_wholesale.TabIndex = 1;
            // 
            // btn_odbc_testcnn_wholesale
            // 
            this.btn_odbc_testcnn_wholesale.Location = new System.Drawing.Point(83, 121);
            this.btn_odbc_testcnn_wholesale.Name = "btn_odbc_testcnn_wholesale";
            this.btn_odbc_testcnn_wholesale.Size = new System.Drawing.Size(144, 25);
            this.btn_odbc_testcnn_wholesale.TabIndex = 10;
            this.btn_odbc_testcnn_wholesale.Text = "Test Connection";
            this.btn_odbc_testcnn_wholesale.UseVisualStyleBackColor = true;
            this.btn_odbc_testcnn_wholesale.Click += new System.EventHandler(this.btn_odbc_testcnn_wholesale_Click);
            // 
            // btn_config_ok
            // 
            this.btn_config_ok.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_config_ok.Location = new System.Drawing.Point(269, 314);
            this.btn_config_ok.Name = "btn_config_ok";
            this.btn_config_ok.Size = new System.Drawing.Size(88, 25);
            this.btn_config_ok.TabIndex = 11;
            this.btn_config_ok.Text = "OK";
            this.btn_config_ok.UseVisualStyleBackColor = true;
            this.btn_config_ok.Click += new System.EventHandler(this.btn_config_ok_Click);
            // 
            // btn_config_cancel
            // 
            this.btn_config_cancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_config_cancel.Location = new System.Drawing.Point(363, 314);
            this.btn_config_cancel.Name = "btn_config_cancel";
            this.btn_config_cancel.Size = new System.Drawing.Size(88, 25);
            this.btn_config_cancel.TabIndex = 12;
            this.btn_config_cancel.Text = "Cancel";
            this.btn_config_cancel.UseVisualStyleBackColor = true;
            this.btn_config_cancel.Click += new System.EventHandler(this.btn_config_cancel_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Interval:";
            // 
            // nudInerval
            // 
            this.nudInerval.Location = new System.Drawing.Point(72, 25);
            this.nudInerval.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.nudInerval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudInerval.Name = "nudInerval";
            this.nudInerval.Size = new System.Drawing.Size(45, 21);
            this.nudInerval.TabIndex = 28;
            this.nudInerval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(123, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "seconds";
            // 
            // cb_writeLog
            // 
            this.cb_writeLog.AutoSize = true;
            this.cb_writeLog.Location = new System.Drawing.Point(13, 61);
            this.cb_writeLog.Name = "cb_writeLog";
            this.cb_writeLog.Size = new System.Drawing.Size(72, 17);
            this.cb_writeLog.TabIndex = 40;
            this.cb_writeLog.Text = "Write Log";
            this.cb_writeLog.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(47, 29);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 13);
            this.label16.TabIndex = 1;
            this.label16.Text = "Dsn:";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(19, 88);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(57, 13);
            this.label17.TabIndex = 5;
            this.label17.Text = "Password:";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(29, 59);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(47, 13);
            this.label18.TabIndex = 3;
            this.label18.Text = "User ID:";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.tb_odbc_dsn_wholesale);
            this.groupBox3.Controls.Add(this.btn_odbc_testcnn_wholesale);
            this.groupBox3.Controls.Add(this.tb_odbc_pwd_wholesale);
            this.groupBox3.Controls.Add(this.tb_odbc_usrid_wholesale);
            this.groupBox3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(251, 160);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Database Connection for Wholesale  ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.nudInerval);
            this.groupBox1.Controls.Add(this.cb_writeLog);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(269, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(179, 160);
            this.groupBox1.TabIndex = 46;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Service";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.tb_odbc_dsn_mkt);
            this.groupBox2.Controls.Add(this.btn_odbc_testcnn_mkt);
            this.groupBox2.Controls.Add(this.tb_odbc_pwd_mkt);
            this.groupBox2.Controls.Add(this.tb_odbc_usrid_mkt);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 179);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(251, 160);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database Connection for Market  ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Dsn:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Password:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "User ID:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tb_odbc_dsn_mkt
            // 
            this.tb_odbc_dsn_mkt.Location = new System.Drawing.Point(83, 26);
            this.tb_odbc_dsn_mkt.Name = "tb_odbc_dsn_mkt";
            this.tb_odbc_dsn_mkt.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_dsn_mkt.TabIndex = 0;
            // 
            // btn_odbc_testcnn_mkt
            // 
            this.btn_odbc_testcnn_mkt.Location = new System.Drawing.Point(83, 121);
            this.btn_odbc_testcnn_mkt.Name = "btn_odbc_testcnn_mkt";
            this.btn_odbc_testcnn_mkt.Size = new System.Drawing.Size(144, 25);
            this.btn_odbc_testcnn_mkt.TabIndex = 10;
            this.btn_odbc_testcnn_mkt.Text = "Test Connection";
            this.btn_odbc_testcnn_mkt.UseVisualStyleBackColor = true;
            this.btn_odbc_testcnn_mkt.Click += new System.EventHandler(this.btn_odbc_testcnn_mkt_Click);
            // 
            // tb_odbc_pwd_mkt
            // 
            this.tb_odbc_pwd_mkt.Location = new System.Drawing.Point(83, 85);
            this.tb_odbc_pwd_mkt.Name = "tb_odbc_pwd_mkt";
            this.tb_odbc_pwd_mkt.PasswordChar = '*';
            this.tb_odbc_pwd_mkt.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_pwd_mkt.TabIndex = 2;
            // 
            // tb_odbc_usrid_mkt
            // 
            this.tb_odbc_usrid_mkt.Location = new System.Drawing.Point(83, 55);
            this.tb_odbc_usrid_mkt.Name = "tb_odbc_usrid_mkt";
            this.tb_odbc_usrid_mkt.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_usrid_mkt.TabIndex = 1;
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 360);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btn_config_cancel);
            this.Controls.Add(this.btn_config_ok);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setting";
            this.Load += new System.EventHandler(this.frmConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudInerval)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_odbc_testcnn_wholesale;
        private System.Windows.Forms.Button btn_config_ok;
        private System.Windows.Forms.Button btn_config_cancel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudInerval;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.CheckBox cb_writeLog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;

        private System.Windows.Forms.TextBox tb_odbc_dsn_wholesale;
        private System.Windows.Forms.TextBox tb_odbc_pwd_wholesale;
        private System.Windows.Forms.TextBox tb_odbc_usrid_wholesale;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_odbc_dsn_mkt;
        private System.Windows.Forms.Button btn_odbc_testcnn_mkt;
        private System.Windows.Forms.TextBox tb_odbc_pwd_mkt;
        private System.Windows.Forms.TextBox tb_odbc_usrid_mkt;
    }
}