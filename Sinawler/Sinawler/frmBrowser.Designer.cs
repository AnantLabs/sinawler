namespace Sinawler
{
    partial class frmBrowser
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
            this.wbForStatusRobot = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wbForStatusRobot
            // 
            this.wbForStatusRobot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbForStatusRobot.Location = new System.Drawing.Point(0, 0);
            this.wbForStatusRobot.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbForStatusRobot.Name = "wbForStatusRobot";
            this.wbForStatusRobot.Size = new System.Drawing.Size(273, 212);
            this.wbForStatusRobot.TabIndex = 0;
            // 
            // frmBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 212);
            this.Controls.Add(this.wbForStatusRobot);
            this.Name = "frmBrowser";
            this.ShowInTaskbar = false;
            this.Text = "frmBrowser";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.frmBrowser_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbForStatusRobot;
    }
}