namespace League_of_Legends_Updater.View
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lstLog = new System.Windows.Forms.ListBox();
            this.cmdStart = new System.Windows.Forms.Button();
            this.cmdAbout = new System.Windows.Forms.Button();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.cmdPath = new System.Windows.Forms.Button();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.tmStart = new System.Windows.Forms.Timer(this.components);
            this.cmdTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstLog
            // 
            this.lstLog.FormattingEnabled = true;
            this.lstLog.Location = new System.Drawing.Point(12, 12);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(569, 147);
            this.lstLog.TabIndex = 0;
            // 
            // cmdStart
            // 
            this.cmdStart.Location = new System.Drawing.Point(12, 197);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(165, 23);
            this.cmdStart.TabIndex = 1;
            this.cmdStart.Text = "Start";
            this.cmdStart.UseVisualStyleBackColor = true;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // cmdAbout
            // 
            this.cmdAbout.Location = new System.Drawing.Point(410, 197);
            this.cmdAbout.Name = "cmdAbout";
            this.cmdAbout.Size = new System.Drawing.Size(171, 23);
            this.cmdAbout.TabIndex = 2;
            this.cmdAbout.Text = "About";
            this.cmdAbout.UseVisualStyleBackColor = true;
            this.cmdAbout.Click += new System.EventHandler(this.cmdAbout_Click);
            // 
            // txtLocation
            // 
            this.txtLocation.Enabled = false;
            this.txtLocation.Location = new System.Drawing.Point(12, 171);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(516, 20);
            this.txtLocation.TabIndex = 3;
            this.txtLocation.Text = "C:\\Riot Games\\League of Legends\\LeagueClient.exe";
            // 
            // cmdPath
            // 
            this.cmdPath.Location = new System.Drawing.Point(534, 171);
            this.cmdPath.Name = "cmdPath";
            this.cmdPath.Size = new System.Drawing.Size(47, 20);
            this.cmdPath.TabIndex = 4;
            this.cmdPath.Text = "...";
            this.cmdPath.UseVisualStyleBackColor = true;
            this.cmdPath.Click += new System.EventHandler(this.cmdPath_Click);
            // 
            // openFile
            // 
            this.openFile.FileName = "openFileDialog1";
            // 
            // tmStart
            // 
            this.tmStart.Interval = 300000;
            this.tmStart.Tick += new System.EventHandler(this.tmStart_Tick);
            // 
            // cmdTest
            // 
            this.cmdTest.Enabled = false;
            this.cmdTest.Location = new System.Drawing.Point(183, 197);
            this.cmdTest.Name = "cmdTest";
            this.cmdTest.Size = new System.Drawing.Size(171, 23);
            this.cmdTest.TabIndex = 5;
            this.cmdTest.Text = "Test";
            this.cmdTest.UseVisualStyleBackColor = true;
            this.cmdTest.Click += new System.EventHandler(this.cmdTest_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 232);
            this.Controls.Add(this.cmdTest);
            this.Controls.Add(this.cmdPath);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.cmdAbout);
            this.Controls.Add(this.cmdStart);
            this.Controls.Add(this.lstLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "League of Legends Updater";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Button cmdAbout;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Button cmdPath;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.Timer tmStart;
        private System.Windows.Forms.Button cmdTest;
    }
}