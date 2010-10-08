namespace Sinawler
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStartByLast = new System.Windows.Forms.Button();
            this.grpUserInfo = new System.Windows.Forms.GroupBox();
            this.lblVerified = new System.Windows.Forms.Label();
            this.lblFollowing = new System.Windows.Forms.Label();
            this.lblStatusesCount = new System.Windows.Forms.Label();
            this.lblCreatedAt = new System.Windows.Forms.Label();
            this.lblFriendsCount = new System.Windows.Forms.Label();
            this.lblFollowersCount = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblUID = new System.Windows.Forms.Label();
            this.btnStartBySearch = new System.Windows.Forms.Button();
            this.lblSearchUID = new System.Windows.Forms.Label();
            this.lblSearchName = new System.Windows.Forms.Label();
            this.txtUID = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnSearchOnline = new System.Windows.Forms.Button();
            this.btnSearchOffLine = new System.Windows.Forms.Button();
            this.grpSearchCondition = new System.Windows.Forms.GroupBox();
            this.grpCurrentUser = new System.Windows.Forms.GroupBox();
            this.lblCVerified = new System.Windows.Forms.Label();
            this.lblCFollowing = new System.Windows.Forms.Label();
            this.lblCStatusesCount = new System.Windows.Forms.Label();
            this.lblCCreatedAt = new System.Windows.Forms.Label();
            this.lblCFriendsCount = new System.Windows.Forms.Label();
            this.lblCFollowersCount = new System.Windows.Forms.Label();
            this.lblCGender = new System.Windows.Forms.Label();
            this.lblCLocation = new System.Windows.Forms.Label();
            this.lblCName = new System.Windows.Forms.Label();
            this.lblCUID = new System.Windows.Forms.Label();
            this.btnStartByCurrent = new System.Windows.Forms.Button();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.rtxtLog = new System.Windows.Forms.RichTextBox();
            this.grpControl = new System.Windows.Forms.GroupBox();
            this.gpSetting = new System.Windows.Forms.GroupBox();
            this.btnDefault = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.nudInterval = new System.Windows.Forms.NumericUpDown();
            this.lblInterval = new System.Windows.Forms.Label();
            this.grpUserInfo.SuspendLayout();
            this.grpSearchCondition.SuspendLayout();
            this.grpCurrentUser.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.grpControl.SuspendLayout();
            this.gpSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartByLast
            // 
            this.btnStartByLast.Location = new System.Drawing.Point( 551, 20 );
            this.btnStartByLast.Name = "btnStartByLast";
            this.btnStartByLast.Size = new System.Drawing.Size( 270, 30 );
            this.btnStartByLast.TabIndex = 0;
            this.btnStartByLast.Text = "以上次中止的用户为起点开始爬行";
            this.btnStartByLast.UseVisualStyleBackColor = true;
            this.btnStartByLast.Click += new System.EventHandler( this.btnStartByLast_Click );
            // 
            // grpUserInfo
            // 
            this.grpUserInfo.Controls.Add( this.lblVerified );
            this.grpUserInfo.Controls.Add( this.lblFollowing );
            this.grpUserInfo.Controls.Add( this.lblStatusesCount );
            this.grpUserInfo.Controls.Add( this.lblCreatedAt );
            this.grpUserInfo.Controls.Add( this.lblFriendsCount );
            this.grpUserInfo.Controls.Add( this.lblFollowersCount );
            this.grpUserInfo.Controls.Add( this.lblGender );
            this.grpUserInfo.Controls.Add( this.lblLocation );
            this.grpUserInfo.Controls.Add( this.lblName );
            this.grpUserInfo.Controls.Add( this.lblUID );
            this.grpUserInfo.Location = new System.Drawing.Point( 11, 82 );
            this.grpUserInfo.Name = "grpUserInfo";
            this.grpUserInfo.Size = new System.Drawing.Size( 806, 75 );
            this.grpUserInfo.TabIndex = 1;
            this.grpUserInfo.TabStop = false;
            this.grpUserInfo.Text = "搜索结果";
            // 
            // lblVerified
            // 
            this.lblVerified.AutoSize = true;
            this.lblVerified.Location = new System.Drawing.Point( 430, 21 );
            this.lblVerified.Name = "lblVerified";
            this.lblVerified.Size = new System.Drawing.Size( 113, 12 );
            this.lblVerified.TabIndex = 9;
            this.lblVerified.Text = "是否微博认证用户：";
            // 
            // lblFollowing
            // 
            this.lblFollowing.AutoSize = true;
            this.lblFollowing.Location = new System.Drawing.Point( 593, 21 );
            this.lblFollowing.Name = "lblFollowing";
            this.lblFollowing.Size = new System.Drawing.Size( 185, 12 );
            this.lblFollowing.TabIndex = 8;
            this.lblFollowing.Text = "当前登录帐号是否关注他（她）：";
            // 
            // lblStatusesCount
            // 
            this.lblStatusesCount.AutoSize = true;
            this.lblStatusesCount.Location = new System.Drawing.Point( 430, 48 );
            this.lblStatusesCount.Name = "lblStatusesCount";
            this.lblStatusesCount.Size = new System.Drawing.Size( 77, 12 );
            this.lblStatusesCount.TabIndex = 7;
            this.lblStatusesCount.Text = "已发微博数：";
            // 
            // lblCreatedAt
            // 
            this.lblCreatedAt.AutoSize = true;
            this.lblCreatedAt.Location = new System.Drawing.Point( 593, 48 );
            this.lblCreatedAt.Name = "lblCreatedAt";
            this.lblCreatedAt.Size = new System.Drawing.Size( 89, 12 );
            this.lblCreatedAt.TabIndex = 6;
            this.lblCreatedAt.Text = "帐号创建时间：";
            // 
            // lblFriendsCount
            // 
            this.lblFriendsCount.AutoSize = true;
            this.lblFriendsCount.Location = new System.Drawing.Point( 302, 48 );
            this.lblFriendsCount.Name = "lblFriendsCount";
            this.lblFriendsCount.Size = new System.Drawing.Size( 65, 12 );
            this.lblFriendsCount.TabIndex = 5;
            this.lblFriendsCount.Text = "关注人数：";
            // 
            // lblFollowersCount
            // 
            this.lblFollowersCount.AutoSize = true;
            this.lblFollowersCount.Location = new System.Drawing.Point( 154, 48 );
            this.lblFollowersCount.Name = "lblFollowersCount";
            this.lblFollowersCount.Size = new System.Drawing.Size( 65, 12 );
            this.lblFollowersCount.TabIndex = 4;
            this.lblFollowersCount.Text = "粉丝人数：";
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Location = new System.Drawing.Point( 326, 21 );
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size( 41, 12 );
            this.lblGender.TabIndex = 3;
            this.lblGender.Text = "性别：";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point( 9, 48 );
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size( 53, 12 );
            this.lblLocation.TabIndex = 2;
            this.lblLocation.Text = "所在地：";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point( 154, 21 );
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size( 65, 12 );
            this.lblName.TabIndex = 1;
            this.lblName.Text = "用户昵称：";
            // 
            // lblUID
            // 
            this.lblUID.AutoSize = true;
            this.lblUID.Location = new System.Drawing.Point( 9, 21 );
            this.lblUID.Name = "lblUID";
            this.lblUID.Size = new System.Drawing.Size( 53, 12 );
            this.lblUID.TabIndex = 0;
            this.lblUID.Text = "用户ID：";
            // 
            // btnStartBySearch
            // 
            this.btnStartBySearch.Location = new System.Drawing.Point( 278, 20 );
            this.btnStartBySearch.Name = "btnStartBySearch";
            this.btnStartBySearch.Size = new System.Drawing.Size( 270, 30 );
            this.btnStartBySearch.TabIndex = 10;
            this.btnStartBySearch.Text = "以搜索结果用户为起点开始爬行";
            this.btnStartBySearch.UseVisualStyleBackColor = true;
            this.btnStartBySearch.Click += new System.EventHandler( this.btnStartBySearch_Click );
            // 
            // lblSearchUID
            // 
            this.lblSearchUID.AutoSize = true;
            this.lblSearchUID.Location = new System.Drawing.Point( 13, 25 );
            this.lblSearchUID.Name = "lblSearchUID";
            this.lblSearchUID.Size = new System.Drawing.Size( 53, 12 );
            this.lblSearchUID.TabIndex = 2;
            this.lblSearchUID.Text = "用户ID：";
            // 
            // lblSearchName
            // 
            this.lblSearchName.AutoSize = true;
            this.lblSearchName.Location = new System.Drawing.Point( 233, 25 );
            this.lblSearchName.Name = "lblSearchName";
            this.lblSearchName.Size = new System.Drawing.Size( 65, 12 );
            this.lblSearchName.TabIndex = 3;
            this.lblSearchName.Text = "用户昵称：";
            // 
            // txtUID
            // 
            this.txtUID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUID.Location = new System.Drawing.Point( 64, 20 );
            this.txtUID.Name = "txtUID";
            this.txtUID.Size = new System.Drawing.Size( 155, 21 );
            this.txtUID.TabIndex = 4;
            // 
            // txtUserName
            // 
            this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserName.Location = new System.Drawing.Point( 295, 20 );
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size( 178, 21 );
            this.txtUserName.TabIndex = 5;
            // 
            // btnSearchOnline
            // 
            this.btnSearchOnline.Location = new System.Drawing.Point( 485, 18 );
            this.btnSearchOnline.Name = "btnSearchOnline";
            this.btnSearchOnline.Size = new System.Drawing.Size( 148, 23 );
            this.btnSearchOnline.TabIndex = 6;
            this.btnSearchOnline.Text = "在新浪微博中搜索用户";
            this.btnSearchOnline.UseVisualStyleBackColor = true;
            this.btnSearchOnline.Click += new System.EventHandler( this.btnSearchOnline_Click );
            // 
            // btnSearchOffLine
            // 
            this.btnSearchOffLine.Location = new System.Drawing.Point( 645, 18 );
            this.btnSearchOffLine.Name = "btnSearchOffLine";
            this.btnSearchOffLine.Size = new System.Drawing.Size( 148, 23 );
            this.btnSearchOffLine.TabIndex = 7;
            this.btnSearchOffLine.Text = "在数据库中搜索用户";
            this.btnSearchOffLine.UseVisualStyleBackColor = true;
            this.btnSearchOffLine.Click += new System.EventHandler( this.btnSearchOffLine_Click );
            // 
            // grpSearchCondition
            // 
            this.grpSearchCondition.Controls.Add( this.btnSearchOffLine );
            this.grpSearchCondition.Controls.Add( this.btnSearchOnline );
            this.grpSearchCondition.Controls.Add( this.txtUID );
            this.grpSearchCondition.Controls.Add( this.txtUserName );
            this.grpSearchCondition.Controls.Add( this.lblSearchUID );
            this.grpSearchCondition.Controls.Add( this.lblSearchName );
            this.grpSearchCondition.Location = new System.Drawing.Point( 11, 20 );
            this.grpSearchCondition.Name = "grpSearchCondition";
            this.grpSearchCondition.Size = new System.Drawing.Size( 806, 56 );
            this.grpSearchCondition.TabIndex = 8;
            this.grpSearchCondition.TabStop = false;
            this.grpSearchCondition.Text = "搜索条件";
            // 
            // grpCurrentUser
            // 
            this.grpCurrentUser.Controls.Add( this.lblCVerified );
            this.grpCurrentUser.Controls.Add( this.lblCFollowing );
            this.grpCurrentUser.Controls.Add( this.lblCStatusesCount );
            this.grpCurrentUser.Controls.Add( this.lblCCreatedAt );
            this.grpCurrentUser.Controls.Add( this.lblCFriendsCount );
            this.grpCurrentUser.Controls.Add( this.lblCFollowersCount );
            this.grpCurrentUser.Controls.Add( this.lblCGender );
            this.grpCurrentUser.Controls.Add( this.lblCLocation );
            this.grpCurrentUser.Controls.Add( this.lblCName );
            this.grpCurrentUser.Controls.Add( this.lblCUID );
            this.grpCurrentUser.Location = new System.Drawing.Point( 4, 12 );
            this.grpCurrentUser.Name = "grpCurrentUser";
            this.grpCurrentUser.Size = new System.Drawing.Size( 827, 71 );
            this.grpCurrentUser.TabIndex = 9;
            this.grpCurrentUser.TabStop = false;
            this.grpCurrentUser.Text = "当前登录帐号信息";
            // 
            // lblCVerified
            // 
            this.lblCVerified.AutoSize = true;
            this.lblCVerified.Location = new System.Drawing.Point( 430, 21 );
            this.lblCVerified.Name = "lblCVerified";
            this.lblCVerified.Size = new System.Drawing.Size( 113, 12 );
            this.lblCVerified.TabIndex = 9;
            this.lblCVerified.Text = "是否微博认证用户：";
            // 
            // lblCFollowing
            // 
            this.lblCFollowing.AutoSize = true;
            this.lblCFollowing.Location = new System.Drawing.Point( 593, 21 );
            this.lblCFollowing.Name = "lblCFollowing";
            this.lblCFollowing.Size = new System.Drawing.Size( 185, 12 );
            this.lblCFollowing.TabIndex = 8;
            this.lblCFollowing.Text = "当前登录帐号是否关注他（她）：";
            // 
            // lblCStatusesCount
            // 
            this.lblCStatusesCount.AutoSize = true;
            this.lblCStatusesCount.Location = new System.Drawing.Point( 430, 48 );
            this.lblCStatusesCount.Name = "lblCStatusesCount";
            this.lblCStatusesCount.Size = new System.Drawing.Size( 77, 12 );
            this.lblCStatusesCount.TabIndex = 7;
            this.lblCStatusesCount.Text = "已发微博数：";
            // 
            // lblCCreatedAt
            // 
            this.lblCCreatedAt.AutoSize = true;
            this.lblCCreatedAt.Location = new System.Drawing.Point( 593, 48 );
            this.lblCCreatedAt.Name = "lblCCreatedAt";
            this.lblCCreatedAt.Size = new System.Drawing.Size( 89, 12 );
            this.lblCCreatedAt.TabIndex = 6;
            this.lblCCreatedAt.Text = "帐号创建时间：";
            // 
            // lblCFriendsCount
            // 
            this.lblCFriendsCount.AutoSize = true;
            this.lblCFriendsCount.Location = new System.Drawing.Point( 302, 48 );
            this.lblCFriendsCount.Name = "lblCFriendsCount";
            this.lblCFriendsCount.Size = new System.Drawing.Size( 65, 12 );
            this.lblCFriendsCount.TabIndex = 5;
            this.lblCFriendsCount.Text = "关注人数：";
            // 
            // lblCFollowersCount
            // 
            this.lblCFollowersCount.AutoSize = true;
            this.lblCFollowersCount.Location = new System.Drawing.Point( 154, 48 );
            this.lblCFollowersCount.Name = "lblCFollowersCount";
            this.lblCFollowersCount.Size = new System.Drawing.Size( 65, 12 );
            this.lblCFollowersCount.TabIndex = 4;
            this.lblCFollowersCount.Text = "粉丝人数：";
            // 
            // lblCGender
            // 
            this.lblCGender.AutoSize = true;
            this.lblCGender.Location = new System.Drawing.Point( 325, 21 );
            this.lblCGender.Name = "lblCGender";
            this.lblCGender.Size = new System.Drawing.Size( 41, 12 );
            this.lblCGender.TabIndex = 3;
            this.lblCGender.Text = "性别：";
            // 
            // lblCLocation
            // 
            this.lblCLocation.AutoSize = true;
            this.lblCLocation.Location = new System.Drawing.Point( 9, 48 );
            this.lblCLocation.Name = "lblCLocation";
            this.lblCLocation.Size = new System.Drawing.Size( 53, 12 );
            this.lblCLocation.TabIndex = 2;
            this.lblCLocation.Text = "所在地：";
            // 
            // lblCName
            // 
            this.lblCName.AutoSize = true;
            this.lblCName.Location = new System.Drawing.Point( 154, 21 );
            this.lblCName.Name = "lblCName";
            this.lblCName.Size = new System.Drawing.Size( 65, 12 );
            this.lblCName.TabIndex = 1;
            this.lblCName.Text = "用户昵称：";
            // 
            // lblCUID
            // 
            this.lblCUID.AutoSize = true;
            this.lblCUID.Location = new System.Drawing.Point( 9, 21 );
            this.lblCUID.Name = "lblCUID";
            this.lblCUID.Size = new System.Drawing.Size( 53, 12 );
            this.lblCUID.TabIndex = 0;
            this.lblCUID.Text = "用户ID：";
            // 
            // btnStartByCurrent
            // 
            this.btnStartByCurrent.Location = new System.Drawing.Point( 6, 20 );
            this.btnStartByCurrent.Name = "btnStartByCurrent";
            this.btnStartByCurrent.Size = new System.Drawing.Size( 270, 30 );
            this.btnStartByCurrent.TabIndex = 10;
            this.btnStartByCurrent.Text = "以当前登录帐号为起点开始爬行";
            this.btnStartByCurrent.UseVisualStyleBackColor = true;
            this.btnStartByCurrent.Click += new System.EventHandler( this.btnStartByCurrent_Click );
            // 
            // grpSearch
            // 
            this.grpSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSearch.Controls.Add( this.grpSearchCondition );
            this.grpSearch.Controls.Add( this.grpUserInfo );
            this.grpSearch.Location = new System.Drawing.Point( 4, 89 );
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size( 827, 166 );
            this.grpSearch.TabIndex = 10;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "搜索用户";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point( 380, 631 );
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size( 75, 23 );
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "退出";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler( this.btnExit_Click );
            // 
            // rtxtLog
            // 
            this.rtxtLog.Location = new System.Drawing.Point( 4, 400 );
            this.rtxtLog.Name = "rtxtLog";
            this.rtxtLog.ReadOnly = true;
            this.rtxtLog.Size = new System.Drawing.Size( 827, 225 );
            this.rtxtLog.TabIndex = 13;
            this.rtxtLog.Text = "";
            // 
            // grpControl
            // 
            this.grpControl.Controls.Add( this.btnStartByCurrent );
            this.grpControl.Controls.Add( this.btnStartBySearch );
            this.grpControl.Controls.Add( this.btnStartByLast );
            this.grpControl.Location = new System.Drawing.Point( 4, 331 );
            this.grpControl.Name = "grpControl";
            this.grpControl.Size = new System.Drawing.Size( 827, 63 );
            this.grpControl.TabIndex = 14;
            this.grpControl.TabStop = false;
            this.grpControl.Text = "爬虫控制";
            // 
            // gpSetting
            // 
            this.gpSetting.Controls.Add( this.btnDefault );
            this.gpSetting.Controls.Add( this.btnSave );
            this.gpSetting.Controls.Add( this.nudInterval );
            this.gpSetting.Controls.Add( this.lblInterval );
            this.gpSetting.Location = new System.Drawing.Point( 5, 261 );
            this.gpSetting.Name = "gpSetting";
            this.gpSetting.Size = new System.Drawing.Size( 826, 64 );
            this.gpSetting.TabIndex = 15;
            this.gpSetting.TabStop = false;
            this.gpSetting.Text = "设置（改变设置后请点击“保存”）";
            // 
            // btnDefault
            // 
            this.btnDefault.Location = new System.Drawing.Point( 605, 25 );
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size( 75, 23 );
            this.btnDefault.TabIndex = 3;
            this.btnDefault.Text = "默认值";
            this.btnDefault.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point( 733, 25 );
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size( 75, 23 );
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // nudInterval
            // 
            this.nudInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudInterval.Location = new System.Drawing.Point( 120, 20 );
            this.nudInterval.Maximum = new decimal( new int[] {
            3600000,
            0,
            0,
            0} );
            this.nudInterval.Minimum = new decimal( new int[] {
            3600,
            0,
            0,
            0} );
            this.nudInterval.Name = "nudInterval";
            this.nudInterval.Size = new System.Drawing.Size( 65, 21 );
            this.nudInterval.TabIndex = 1;
            this.nudInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudInterval.Value = global::Sinawler.Properties.Settings.Default.settingInterval;
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point( 11, 25 );
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size( 113, 12 );
            this.lblInterval.TabIndex = 0;
            this.lblInterval.Text = "请求间隔（毫秒）：";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 837, 663 );
            this.ControlBox = false;
            this.Controls.Add( this.gpSetting );
            this.Controls.Add( this.grpControl );
            this.Controls.Add( this.rtxtLog );
            this.Controls.Add( this.btnExit );
            this.Controls.Add( this.grpSearch );
            this.Controls.Add( this.grpCurrentUser );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "新浪微博爬虫";
            this.Load += new System.EventHandler( this.frmMain_Load );
            this.grpUserInfo.ResumeLayout( false );
            this.grpUserInfo.PerformLayout();
            this.grpSearchCondition.ResumeLayout( false );
            this.grpSearchCondition.PerformLayout();
            this.grpCurrentUser.ResumeLayout( false );
            this.grpCurrentUser.PerformLayout();
            this.grpSearch.ResumeLayout( false );
            this.grpControl.ResumeLayout( false );
            this.gpSetting.ResumeLayout( false );
            this.gpSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval)).EndInit();
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Button btnStartByLast;
        private System.Windows.Forms.GroupBox grpUserInfo;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblUID;
        private System.Windows.Forms.Label lblStatusesCount;
        private System.Windows.Forms.Label lblCreatedAt;
        private System.Windows.Forms.Label lblFriendsCount;
        private System.Windows.Forms.Label lblFollowersCount;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label lblFollowing;
        private System.Windows.Forms.Label lblVerified;
        private System.Windows.Forms.Button btnStartBySearch;
        private System.Windows.Forms.Label lblSearchUID;
        private System.Windows.Forms.Label lblSearchName;
        private System.Windows.Forms.TextBox txtUID;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnSearchOnline;
        private System.Windows.Forms.Button btnSearchOffLine;
        private System.Windows.Forms.GroupBox grpSearchCondition;
        private System.Windows.Forms.GroupBox grpCurrentUser;
        private System.Windows.Forms.Button btnStartByCurrent;
        private System.Windows.Forms.Label lblCVerified;
        private System.Windows.Forms.Label lblCFollowing;
        private System.Windows.Forms.Label lblCStatusesCount;
        private System.Windows.Forms.Label lblCCreatedAt;
        private System.Windows.Forms.Label lblCFriendsCount;
        private System.Windows.Forms.Label lblCFollowersCount;
        private System.Windows.Forms.Label lblCGender;
        private System.Windows.Forms.Label lblCLocation;
        private System.Windows.Forms.Label lblCName;
        private System.Windows.Forms.Label lblCUID;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.RichTextBox rtxtLog;
        private System.Windows.Forms.GroupBox grpControl;
        private System.Windows.Forms.GroupBox gpSetting;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.NumericUpDown nudInterval;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Button btnSave;
    }
}

