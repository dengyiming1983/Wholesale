namespace WholesaleMonitor.View
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
            this.btn_odbc_testcnn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_odbc_dsn = new System.Windows.Forms.TextBox();
            this.tb_odbc_pwd = new System.Windows.Forms.TextBox();
            this.tb_odbc_usrid = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_config_ok = new System.Windows.Forms.Button();
            this.btn_config_cancel = new System.Windows.Forms.Button();
            this.cb_writeLog = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.nudInterval = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_clear_cache = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_odbc_testcnn
            // 
            this.btn_odbc_testcnn.Location = new System.Drawing.Point(250, 68);
            this.btn_odbc_testcnn.Name = "btn_odbc_testcnn";
            this.btn_odbc_testcnn.Size = new System.Drawing.Size(105, 23);
            this.btn_odbc_testcnn.TabIndex = 4;
            this.btn_odbc_testcnn.Text = "Test Connection";
            this.btn_odbc_testcnn.UseVisualStyleBackColor = true;
            this.btn_odbc_testcnn.Click += new System.EventHandler(this.btn_odbc_testcnn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.tb_odbc_dsn);
            this.groupBox2.Controls.Add(this.tb_odbc_pwd);
            this.groupBox2.Controls.Add(this.tb_odbc_usrid);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.btn_odbc_testcnn);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(361, 97);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database Connection";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "Dsn:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 73);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "Password:";
            // 
            // tb_odbc_dsn
            // 
            this.tb_odbc_dsn.Location = new System.Drawing.Point(67, 20);
            this.tb_odbc_dsn.Name = "tb_odbc_dsn";
            this.tb_odbc_dsn.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_dsn.TabIndex = 0;
            // 
            // tb_odbc_pwd
            // 
            this.tb_odbc_pwd.Location = new System.Drawing.Point(67, 70);
            this.tb_odbc_pwd.Name = "tb_odbc_pwd";
            this.tb_odbc_pwd.PasswordChar = '*';
            this.tb_odbc_pwd.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_pwd.TabIndex = 2;
            // 
            // tb_odbc_usrid
            // 
            this.tb_odbc_usrid.Location = new System.Drawing.Point(67, 45);
            this.tb_odbc_usrid.Name = "tb_odbc_usrid";
            this.tb_odbc_usrid.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_usrid.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "User ID:";
            // 
            // btn_config_ok
            // 
            this.btn_config_ok.Location = new System.Drawing.Point(385, 127);
            this.btn_config_ok.Name = "btn_config_ok";
            this.btn_config_ok.Size = new System.Drawing.Size(88, 23);
            this.btn_config_ok.TabIndex = 11;
            this.btn_config_ok.Text = "OK";
            this.btn_config_ok.UseVisualStyleBackColor = true;
            this.btn_config_ok.Click += new System.EventHandler(this.btn_config_ok_Click);
            // 
            // btn_config_cancel
            // 
            this.btn_config_cancel.Location = new System.Drawing.Point(488, 127);
            this.btn_config_cancel.Name = "btn_config_cancel";
            this.btn_config_cancel.Size = new System.Drawing.Size(88, 23);
            this.btn_config_cancel.TabIndex = 12;
            this.btn_config_cancel.Text = "Cancel";
            this.btn_config_cancel.UseVisualStyleBackColor = true;
            this.btn_config_cancel.Click += new System.EventHandler(this.btn_config_cancel_Click);
            // 
            // cb_writeLog
            // 
            this.cb_writeLog.AutoSize = true;
            this.cb_writeLog.Location = new System.Drawing.Point(6, 40);
            this.cb_writeLog.Name = "cb_writeLog";
            this.cb_writeLog.Size = new System.Drawing.Size(78, 16);
            this.cb_writeLog.TabIndex = 39;
            this.cb_writeLog.Text = "Write Log";
            this.cb_writeLog.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(118, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 12);
            this.label10.TabIndex = 38;
            this.label10.Text = "seconds";
            // 
            // nudInterval
            // 
            this.nudInterval.Location = new System.Drawing.Point(67, 13);
            this.nudInterval.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.nudInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudInterval.Name = "nudInterval";
            this.nudInterval.Size = new System.Drawing.Size(45, 21);
            this.nudInterval.TabIndex = 37;
            this.nudInterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 17);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 36;
            this.label11.Text = "Interval:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_clear_cache);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.cb_writeLog);
            this.groupBox1.Controls.Add(this.nudInterval);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Location = new System.Drawing.Point(379, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 97);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Service";
            // 
            // btn_clear_cache
            // 
            this.btn_clear_cache.Location = new System.Drawing.Point(8, 68);
            this.btn_clear_cache.Name = "btn_clear_cache";
            this.btn_clear_cache.Size = new System.Drawing.Size(105, 23);
            this.btn_clear_cache.TabIndex = 40;
            this.btn_clear_cache.Text = "Reset";
            this.btn_clear_cache.UseVisualStyleBackColor = true;
            this.btn_clear_cache.Click += new System.EventHandler(this.btn_clear_cache_Click);
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 163);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_config_cancel);
            this.Controls.Add(this.btn_config_ok);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setting";
            this.Load += new System.EventHandler(this.frmConfig_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_odbc_testcnn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_odbc_dsn;
        private System.Windows.Forms.TextBox tb_odbc_pwd;
        private System.Windows.Forms.TextBox tb_odbc_usrid;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btn_config_ok;
        private System.Windows.Forms.Button btn_config_cancel;
        private System.Windows.Forms.CheckBox cb_writeLog;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nudInterval;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_clear_cache;
    }
}