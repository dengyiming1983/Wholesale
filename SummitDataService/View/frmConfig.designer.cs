namespace SummitDataService.View
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
            this.tb_odbc_dsn = new System.Windows.Forms.TextBox();
            this.tb_odbc_pwd = new System.Windows.Forms.TextBox();
            this.tb_odbc_usrid = new System.Windows.Forms.TextBox();
            this.btn_odbc_testcnn = new System.Windows.Forms.Button();
            this.btn_config_ok = new System.Windows.Forms.Button();
            this.btn_config_cancel = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.nudInerval = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.cb_writeLog = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnItemBrowse = new System.Windows.Forms.Button();
            this.btn_output_browse = new System.Windows.Forms.Button();
            this.tb_input = new System.Windows.Forms.TextBox();
            this.tb_output = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_insertCustomer = new System.Windows.Forms.CheckBox();
            this.cb_insertItems = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudInerval)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_odbc_dsn
            // 
            this.tb_odbc_dsn.Location = new System.Drawing.Point(83, 26);
            this.tb_odbc_dsn.Name = "tb_odbc_dsn";
            this.tb_odbc_dsn.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_dsn.TabIndex = 0;
            // 
            // tb_odbc_pwd
            // 
            this.tb_odbc_pwd.Location = new System.Drawing.Point(83, 85);
            this.tb_odbc_pwd.Name = "tb_odbc_pwd";
            this.tb_odbc_pwd.PasswordChar = '*';
            this.tb_odbc_pwd.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_pwd.TabIndex = 2;
            // 
            // tb_odbc_usrid
            // 
            this.tb_odbc_usrid.Location = new System.Drawing.Point(83, 55);
            this.tb_odbc_usrid.Name = "tb_odbc_usrid";
            this.tb_odbc_usrid.Size = new System.Drawing.Size(144, 21);
            this.tb_odbc_usrid.TabIndex = 1;
            // 
            // btn_odbc_testcnn
            // 
            this.btn_odbc_testcnn.Location = new System.Drawing.Point(83, 121);
            this.btn_odbc_testcnn.Name = "btn_odbc_testcnn";
            this.btn_odbc_testcnn.Size = new System.Drawing.Size(144, 25);
            this.btn_odbc_testcnn.TabIndex = 10;
            this.btn_odbc_testcnn.Text = "Test Connection";
            this.btn_odbc_testcnn.UseVisualStyleBackColor = true;
            this.btn_odbc_testcnn.Click += new System.EventHandler(this.btn_odbc_testcnn_Click);
            // 
            // btn_config_ok
            // 
            this.btn_config_ok.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_config_ok.Location = new System.Drawing.Point(267, 377);
            this.btn_config_ok.Name = "btn_config_ok";
            this.btn_config_ok.Size = new System.Drawing.Size(88, 25);
            this.btn_config_ok.TabIndex = 11;
            this.btn_config_ok.Text = "&OK";
            this.btn_config_ok.UseVisualStyleBackColor = true;
            this.btn_config_ok.Click += new System.EventHandler(this.btn_config_ok_Click);
            // 
            // btn_config_cancel
            // 
            this.btn_config_cancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_config_cancel.Location = new System.Drawing.Point(361, 377);
            this.btn_config_cancel.Name = "btn_config_cancel";
            this.btn_config_cancel.Size = new System.Drawing.Size(88, 25);
            this.btn_config_cancel.TabIndex = 12;
            this.btn_config_cancel.Text = "&Cancel";
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.btnItemBrowse);
            this.groupBox2.Controls.Add(this.btn_output_browse);
            this.groupBox2.Controls.Add(this.tb_input);
            this.groupBox2.Controls.Add(this.tb_output);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 185);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(436, 180);
            this.groupBox2.TabIndex = 45;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Import \\ Export Setting";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "Output Folder:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 43;
            this.label9.Text = "Input Folder:";
            // 
            // btnItemBrowse
            // 
            this.btnItemBrowse.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnItemBrowse.Location = new System.Drawing.Point(13, 64);
            this.btnItemBrowse.Name = "btnItemBrowse";
            this.btnItemBrowse.Size = new System.Drawing.Size(75, 25);
            this.btnItemBrowse.TabIndex = 25;
            this.btnItemBrowse.Text = "Browse...";
            this.btnItemBrowse.UseVisualStyleBackColor = true;
            this.btnItemBrowse.Click += new System.EventHandler(this.btnItemBrowse_Click);
            // 
            // btn_output_browse
            // 
            this.btn_output_browse.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_output_browse.Location = new System.Drawing.Point(13, 146);
            this.btn_output_browse.Name = "btn_output_browse";
            this.btn_output_browse.Size = new System.Drawing.Size(75, 25);
            this.btn_output_browse.TabIndex = 45;
            this.btn_output_browse.Text = "Browse...";
            this.btn_output_browse.UseVisualStyleBackColor = true;
            this.btn_output_browse.Click += new System.EventHandler(this.btn_output_browse_Click);
            // 
            // tb_input
            // 
            this.tb_input.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_input.Location = new System.Drawing.Point(13, 38);
            this.tb_input.Name = "tb_input";
            this.tb_input.Size = new System.Drawing.Size(413, 22);
            this.tb_input.TabIndex = 5;
            // 
            // tb_output
            // 
            this.tb_output.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_output.Location = new System.Drawing.Point(13, 120);
            this.tb_output.Name = "tb_output";
            this.tb_output.Size = new System.Drawing.Size(413, 22);
            this.tb_output.TabIndex = 44;
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
            this.groupBox3.Controls.Add(this.tb_odbc_dsn);
            this.groupBox3.Controls.Add(this.btn_odbc_testcnn);
            this.groupBox3.Controls.Add(this.tb_odbc_pwd);
            this.groupBox3.Controls.Add(this.tb_odbc_usrid);
            this.groupBox3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(251, 160);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "VPOS Database Connection for ODBC  ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_insertCustomer);
            this.groupBox1.Controls.Add(this.cb_insertItems);
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
            // cb_insertCustomer
            // 
            this.cb_insertCustomer.AutoSize = true;
            this.cb_insertCustomer.Location = new System.Drawing.Point(13, 110);
            this.cb_insertCustomer.Name = "cb_insertCustomer";
            this.cb_insertCustomer.Size = new System.Drawing.Size(104, 17);
            this.cb_insertCustomer.TabIndex = 43;
            this.cb_insertCustomer.Text = "Insert Customer";
            this.cb_insertCustomer.UseVisualStyleBackColor = true;
            // 
            // cb_insertItems
            // 
            this.cb_insertItems.AutoSize = true;
            this.cb_insertItems.Location = new System.Drawing.Point(13, 87);
            this.cb_insertItems.Name = "cb_insertItems";
            this.cb_insertItems.Size = new System.Drawing.Size(85, 17);
            this.cb_insertItems.TabIndex = 42;
            this.cb_insertItems.Text = "Insert Items";
            this.cb_insertItems.UseVisualStyleBackColor = true;
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 420);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btn_config_cancel);
            this.Controls.Add(this.btn_config_ok);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setting";
            this.Load += new System.EventHandler(this.frmConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudInerval)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_odbc_testcnn;
        private System.Windows.Forms.Button btn_config_ok;
        private System.Windows.Forms.Button btn_config_cancel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudInerval;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.CheckBox cb_writeLog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnItemBrowse;
        private System.Windows.Forms.TextBox tb_input;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;

        private System.Windows.Forms.TextBox tb_odbc_dsn;
        private System.Windows.Forms.TextBox tb_odbc_pwd;
        private System.Windows.Forms.TextBox tb_odbc_usrid;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_output_browse;
        private System.Windows.Forms.TextBox tb_output;
        private System.Windows.Forms.CheckBox cb_insertItems;
        private System.Windows.Forms.CheckBox cb_insertCustomer;
    }
}