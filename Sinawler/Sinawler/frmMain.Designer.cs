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

        #region Generated code of Windows Form Designer

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
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
            this.lblUserID = new System.Windows.Forms.Label();
            this.btnStartBySearch = new System.Windows.Forms.Button();
            this.lblSearchUserID = new System.Windows.Forms.Label();
            this.lblSearchName = new System.Windows.Forms.Label();
            this.txtUserID = new System.Windows.Forms.TextBox();
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
            this.lblCUserID = new System.Windows.Forms.Label();
            this.btnStartByCurrent = new System.Windows.Forms.Button();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.grpControl = new System.Windows.Forms.GroupBox();
            this.btnPauseContinue = new System.Windows.Forms.Button();
            this.gpSetting = new System.Windows.Forms.GroupBox();
            this.chkCrawlRetweets = new System.Windows.Forms.CheckBox();
            this.chkComment = new System.Windows.Forms.CheckBox();
            this.chkStatus = new System.Windows.Forms.CheckBox();
            this.chkTag = new System.Windows.Forms.CheckBox();
            this.chkUserInfo = new System.Windows.Forms.CheckBox();
            this.lblRobotSelect = new System.Windows.Forms.Label();
            this.chkConfirmRelationship = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.grpDBSettings = new System.Windows.Forms.GroupBox();
            this.txtDBName = new System.Windows.Forms.TextBox();
            this.lblDBName = new System.Windows.Forms.Label();
            this.drplstDBType = new System.Windows.Forms.ComboBox();
            this.txtDBPwd = new System.Windows.Forms.TextBox();
            this.txtDBUserName = new System.Windows.Forms.TextBox();
            this.txtDBServer = new System.Windows.Forms.TextBox();
            this.lblDBType = new System.Windows.Forms.Label();
            this.lblDBUserName = new System.Windows.Forms.Label();
            this.lblDBServer = new System.Windows.Forms.Label();
            this.lblDBPwd = new System.Windows.Forms.Label();
            this.grpQueueLength = new System.Windows.Forms.GroupBox();
            this.lblQueueLength = new System.Windows.Forms.Label();
            this.numQueueLength = new System.Windows.Forms.NumericUpDown();
            this.tbQueueLength = new System.Windows.Forms.TrackBar();
            this.btnDefault = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblUserInfoTitle = new System.Windows.Forms.Label();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.lblCommentQueueInfo = new System.Windows.Forms.Label();
            this.lblStatusQueueInfo = new System.Windows.Forms.Label();
            this.lblUserTagQueueInfo = new System.Windows.Forms.Label();
            this.lblUserRelationQueueInfo = new System.Windows.Forms.Label();
            this.lblUserInfoQueueInfo = new System.Windows.Forms.Label();
            this.lblCommentMessage = new System.Windows.Forms.Label();
            this.lblCommentTitle = new System.Windows.Forms.Label();
            this.lblStatusMessage = new System.Windows.Forms.Label();
            this.lblUserTagMessage = new System.Windows.Forms.Label();
            this.lblUserRelationMessage = new System.Windows.Forms.Label();
            this.lblUserTagTitle = new System.Windows.Forms.Label();
            this.lblUserInfoMessage = new System.Windows.Forms.Label();
            this.lblUserRelationTitle = new System.Windows.Forms.Label();
            this.lblStatusTitle = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.grpUserInfo.SuspendLayout();
            this.grpSearchCondition.SuspendLayout();
            this.grpCurrentUser.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.grpControl.SuspendLayout();
            this.gpSetting.SuspendLayout();
            this.grpDBSettings.SuspendLayout();
            this.grpQueueLength.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQueueLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbQueueLength)).BeginInit();
            this.grpStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartByLast
            // 
            this.btnStartByLast.Location = new System.Drawing.Point(640, 24);
            this.btnStartByLast.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStartByLast.Name = "btnStartByLast";
            this.btnStartByLast.Size = new System.Drawing.Size(312, 38);
            this.btnStartByLast.TabIndex = 19;
            this.btnStartByLast.Text = "Start Crawling by Last Stopped User";
            this.btnStartByLast.UseVisualStyleBackColor = true;
            this.btnStartByLast.Click += new System.EventHandler(this.btnStartByLast_Click);
            // 
            // grpUserInfo
            // 
            this.grpUserInfo.Controls.Add(this.lblVerified);
            this.grpUserInfo.Controls.Add(this.lblFollowing);
            this.grpUserInfo.Controls.Add(this.lblStatusesCount);
            this.grpUserInfo.Controls.Add(this.lblCreatedAt);
            this.grpUserInfo.Controls.Add(this.lblFriendsCount);
            this.grpUserInfo.Controls.Add(this.lblFollowersCount);
            this.grpUserInfo.Controls.Add(this.lblGender);
            this.grpUserInfo.Controls.Add(this.lblLocation);
            this.grpUserInfo.Controls.Add(this.lblName);
            this.grpUserInfo.Controls.Add(this.lblUserID);
            this.grpUserInfo.Location = new System.Drawing.Point(15, 101);
            this.grpUserInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpUserInfo.Name = "grpUserInfo";
            this.grpUserInfo.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpUserInfo.Size = new System.Drawing.Size(1075, 79);
            this.grpUserInfo.TabIndex = 1;
            this.grpUserInfo.TabStop = false;
            this.grpUserInfo.Text = "Search Results";
            // 
            // lblVerified
            // 
            this.lblVerified.AutoSize = true;
            this.lblVerified.Location = new System.Drawing.Point(560, 26);
            this.lblVerified.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVerified.Name = "lblVerified";
            this.lblVerified.Size = new System.Drawing.Size(79, 15);
            this.lblVerified.TabIndex = 9;
            this.lblVerified.Text = "Verified:";
            // 
            // lblFollowing
            // 
            this.lblFollowing.AutoSize = true;
            this.lblFollowing.Location = new System.Drawing.Point(773, 26);
            this.lblFollowing.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFollowing.Name = "lblFollowing";
            this.lblFollowing.Size = new System.Drawing.Size(207, 15);
            this.lblFollowing.TabIndex = 8;
            this.lblFollowing.Text = "Followed by Current User:";
            // 
            // lblStatusesCount
            // 
            this.lblStatusesCount.AutoSize = true;
            this.lblStatusesCount.Location = new System.Drawing.Point(560, 51);
            this.lblStatusesCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusesCount.Name = "lblStatusesCount";
            this.lblStatusesCount.Size = new System.Drawing.Size(63, 15);
            this.lblStatusesCount.TabIndex = 7;
            this.lblStatusesCount.Text = "Tweets:";
            // 
            // lblCreatedAt
            // 
            this.lblCreatedAt.AutoSize = true;
            this.lblCreatedAt.Location = new System.Drawing.Point(773, 51);
            this.lblCreatedAt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCreatedAt.Name = "lblCreatedAt";
            this.lblCreatedAt.Size = new System.Drawing.Size(95, 15);
            this.lblCreatedAt.TabIndex = 6;
            this.lblCreatedAt.Text = "Created At:";
            // 
            // lblFriendsCount
            // 
            this.lblFriendsCount.AutoSize = true;
            this.lblFriendsCount.Location = new System.Drawing.Point(387, 51);
            this.lblFriendsCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFriendsCount.Name = "lblFriendsCount";
            this.lblFriendsCount.Size = new System.Drawing.Size(95, 15);
            this.lblFriendsCount.TabIndex = 5;
            this.lblFriendsCount.Text = "Followings:";
            // 
            // lblFollowersCount
            // 
            this.lblFollowersCount.AutoSize = true;
            this.lblFollowersCount.Location = new System.Drawing.Point(200, 51);
            this.lblFollowersCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFollowersCount.Name = "lblFollowersCount";
            this.lblFollowersCount.Size = new System.Drawing.Size(87, 15);
            this.lblFollowersCount.TabIndex = 4;
            this.lblFollowersCount.Text = "Followers:";
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Location = new System.Drawing.Point(420, 26);
            this.lblGender.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(63, 15);
            this.lblGender.TabIndex = 3;
            this.lblGender.Text = "Gender:";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(12, 51);
            this.lblLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(79, 15);
            this.lblLocation.TabIndex = 2;
            this.lblLocation.Text = "Location:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(200, 26);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(79, 15);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Nickname:";
            // 
            // lblUserID
            // 
            this.lblUserID.AutoSize = true;
            this.lblUserID.Location = new System.Drawing.Point(12, 26);
            this.lblUserID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserID.Name = "lblUserID";
            this.lblUserID.Size = new System.Drawing.Size(63, 15);
            this.lblUserID.TabIndex = 0;
            this.lblUserID.Text = "UserID:";
            // 
            // btnStartBySearch
            // 
            this.btnStartBySearch.Location = new System.Drawing.Point(324, 24);
            this.btnStartBySearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStartBySearch.Name = "btnStartBySearch";
            this.btnStartBySearch.Size = new System.Drawing.Size(312, 38);
            this.btnStartBySearch.TabIndex = 18;
            this.btnStartBySearch.Text = "Start Crawling by Searched User";
            this.btnStartBySearch.UseVisualStyleBackColor = true;
            this.btnStartBySearch.Click += new System.EventHandler(this.btnStartBySearch_Click);
            // 
            // lblSearchUserID
            // 
            this.lblSearchUserID.AutoSize = true;
            this.lblSearchUserID.Location = new System.Drawing.Point(17, 31);
            this.lblSearchUserID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearchUserID.Name = "lblSearchUserID";
            this.lblSearchUserID.Size = new System.Drawing.Size(70, 15);
            this.lblSearchUserID.TabIndex = 2;
            this.lblSearchUserID.Text = "UserID：";
            // 
            // lblSearchName
            // 
            this.lblSearchName.AutoSize = true;
            this.lblSearchName.Location = new System.Drawing.Point(300, 31);
            this.lblSearchName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearchName.Name = "lblSearchName";
            this.lblSearchName.Size = new System.Drawing.Size(71, 15);
            this.lblSearchName.TabIndex = 3;
            this.lblSearchName.Text = "Nickname";
            // 
            // txtUserID
            // 
            this.txtUserID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserID.Location = new System.Drawing.Point(85, 25);
            this.txtUserID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(193, 25);
            this.txtUserID.TabIndex = 0;
            // 
            // txtUserName
            // 
            this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserName.Location = new System.Drawing.Point(380, 25);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(213, 25);
            this.txtUserName.TabIndex = 1;
            // 
            // btnSearchOnline
            // 
            this.btnSearchOnline.Location = new System.Drawing.Point(620, 22);
            this.btnSearchOnline.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSearchOnline.Name = "btnSearchOnline";
            this.btnSearchOnline.Size = new System.Drawing.Size(207, 29);
            this.btnSearchOnline.TabIndex = 2;
            this.btnSearchOnline.Text = "Search Online";
            this.btnSearchOnline.UseVisualStyleBackColor = true;
            this.btnSearchOnline.Click += new System.EventHandler(this.btnSearchOnline_Click);
            // 
            // btnSearchOffLine
            // 
            this.btnSearchOffLine.Location = new System.Drawing.Point(847, 22);
            this.btnSearchOffLine.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSearchOffLine.Name = "btnSearchOffLine";
            this.btnSearchOffLine.Size = new System.Drawing.Size(213, 29);
            this.btnSearchOffLine.TabIndex = 3;
            this.btnSearchOffLine.Text = "Search in DateBase";
            this.btnSearchOffLine.UseVisualStyleBackColor = true;
            this.btnSearchOffLine.Click += new System.EventHandler(this.btnSearchOffLine_Click);
            // 
            // grpSearchCondition
            // 
            this.grpSearchCondition.Controls.Add(this.btnSearchOffLine);
            this.grpSearchCondition.Controls.Add(this.btnSearchOnline);
            this.grpSearchCondition.Controls.Add(this.txtUserID);
            this.grpSearchCondition.Controls.Add(this.txtUserName);
            this.grpSearchCondition.Controls.Add(this.lblSearchUserID);
            this.grpSearchCondition.Controls.Add(this.lblSearchName);
            this.grpSearchCondition.Location = new System.Drawing.Point(15, 25);
            this.grpSearchCondition.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpSearchCondition.Name = "grpSearchCondition";
            this.grpSearchCondition.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpSearchCondition.Size = new System.Drawing.Size(1075, 70);
            this.grpSearchCondition.TabIndex = 8;
            this.grpSearchCondition.TabStop = false;
            this.grpSearchCondition.Text = "Search By";
            // 
            // grpCurrentUser
            // 
            this.grpCurrentUser.Controls.Add(this.lblCVerified);
            this.grpCurrentUser.Controls.Add(this.lblCFollowing);
            this.grpCurrentUser.Controls.Add(this.lblCStatusesCount);
            this.grpCurrentUser.Controls.Add(this.lblCCreatedAt);
            this.grpCurrentUser.Controls.Add(this.lblCFriendsCount);
            this.grpCurrentUser.Controls.Add(this.lblCFollowersCount);
            this.grpCurrentUser.Controls.Add(this.lblCGender);
            this.grpCurrentUser.Controls.Add(this.lblCLocation);
            this.grpCurrentUser.Controls.Add(this.lblCName);
            this.grpCurrentUser.Controls.Add(this.lblCUserID);
            this.grpCurrentUser.Location = new System.Drawing.Point(5, 15);
            this.grpCurrentUser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpCurrentUser.Name = "grpCurrentUser";
            this.grpCurrentUser.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpCurrentUser.Size = new System.Drawing.Size(1103, 76);
            this.grpCurrentUser.TabIndex = 9;
            this.grpCurrentUser.TabStop = false;
            this.grpCurrentUser.Text = "Current User Information:";
            // 
            // lblCVerified
            // 
            this.lblCVerified.AutoSize = true;
            this.lblCVerified.Location = new System.Drawing.Point(573, 26);
            this.lblCVerified.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCVerified.Name = "lblCVerified";
            this.lblCVerified.Size = new System.Drawing.Size(79, 15);
            this.lblCVerified.TabIndex = 9;
            this.lblCVerified.Text = "Verified:";
            // 
            // lblCFollowing
            // 
            this.lblCFollowing.AutoSize = true;
            this.lblCFollowing.Location = new System.Drawing.Point(791, 26);
            this.lblCFollowing.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCFollowing.Name = "lblCFollowing";
            this.lblCFollowing.Size = new System.Drawing.Size(207, 15);
            this.lblCFollowing.TabIndex = 8;
            this.lblCFollowing.Text = "Followed by Current User:";
            // 
            // lblCStatusesCount
            // 
            this.lblCStatusesCount.AutoSize = true;
            this.lblCStatusesCount.Location = new System.Drawing.Point(573, 51);
            this.lblCStatusesCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCStatusesCount.Name = "lblCStatusesCount";
            this.lblCStatusesCount.Size = new System.Drawing.Size(63, 15);
            this.lblCStatusesCount.TabIndex = 7;
            this.lblCStatusesCount.Text = "Tweets:";
            // 
            // lblCCreatedAt
            // 
            this.lblCCreatedAt.AutoSize = true;
            this.lblCCreatedAt.Location = new System.Drawing.Point(791, 51);
            this.lblCCreatedAt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCCreatedAt.Name = "lblCCreatedAt";
            this.lblCCreatedAt.Size = new System.Drawing.Size(95, 15);
            this.lblCCreatedAt.TabIndex = 6;
            this.lblCCreatedAt.Text = "Created At:";
            // 
            // lblCFriendsCount
            // 
            this.lblCFriendsCount.AutoSize = true;
            this.lblCFriendsCount.Location = new System.Drawing.Point(403, 51);
            this.lblCFriendsCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCFriendsCount.Name = "lblCFriendsCount";
            this.lblCFriendsCount.Size = new System.Drawing.Size(95, 15);
            this.lblCFriendsCount.TabIndex = 5;
            this.lblCFriendsCount.Text = "Followings:";
            // 
            // lblCFollowersCount
            // 
            this.lblCFollowersCount.AutoSize = true;
            this.lblCFollowersCount.Location = new System.Drawing.Point(205, 51);
            this.lblCFollowersCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCFollowersCount.Name = "lblCFollowersCount";
            this.lblCFollowersCount.Size = new System.Drawing.Size(87, 15);
            this.lblCFollowersCount.TabIndex = 4;
            this.lblCFollowersCount.Text = "Followers:";
            // 
            // lblCGender
            // 
            this.lblCGender.AutoSize = true;
            this.lblCGender.Location = new System.Drawing.Point(433, 26);
            this.lblCGender.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCGender.Name = "lblCGender";
            this.lblCGender.Size = new System.Drawing.Size(63, 15);
            this.lblCGender.TabIndex = 3;
            this.lblCGender.Text = "Gender:";
            // 
            // lblCLocation
            // 
            this.lblCLocation.AutoSize = true;
            this.lblCLocation.Location = new System.Drawing.Point(12, 51);
            this.lblCLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCLocation.Name = "lblCLocation";
            this.lblCLocation.Size = new System.Drawing.Size(79, 15);
            this.lblCLocation.TabIndex = 2;
            this.lblCLocation.Text = "Location:";
            // 
            // lblCName
            // 
            this.lblCName.AutoSize = true;
            this.lblCName.Location = new System.Drawing.Point(205, 26);
            this.lblCName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCName.Name = "lblCName";
            this.lblCName.Size = new System.Drawing.Size(79, 15);
            this.lblCName.TabIndex = 1;
            this.lblCName.Text = "Nickname:";
            // 
            // lblCUserID
            // 
            this.lblCUserID.AutoSize = true;
            this.lblCUserID.Location = new System.Drawing.Point(12, 26);
            this.lblCUserID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCUserID.Name = "lblCUserID";
            this.lblCUserID.Size = new System.Drawing.Size(63, 15);
            this.lblCUserID.TabIndex = 0;
            this.lblCUserID.Text = "UserID:";
            // 
            // btnStartByCurrent
            // 
            this.btnStartByCurrent.Location = new System.Drawing.Point(8, 24);
            this.btnStartByCurrent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStartByCurrent.Name = "btnStartByCurrent";
            this.btnStartByCurrent.Size = new System.Drawing.Size(312, 38);
            this.btnStartByCurrent.TabIndex = 17;
            this.btnStartByCurrent.Text = "Start Crawling by Current User";
            this.btnStartByCurrent.UseVisualStyleBackColor = true;
            this.btnStartByCurrent.Click += new System.EventHandler(this.btnStartByCurrent_Click);
            // 
            // grpSearch
            // 
            this.grpSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSearch.Controls.Add(this.grpSearchCondition);
            this.grpSearch.Controls.Add(this.grpUserInfo);
            this.grpSearch.Location = new System.Drawing.Point(5, 95);
            this.grpSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpSearch.Size = new System.Drawing.Size(1103, 189);
            this.grpSearch.TabIndex = 10;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "Search Users";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(604, 897);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(227, 29);
            this.btnExit.TabIndex = 22;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // grpControl
            // 
            this.grpControl.Controls.Add(this.btnPauseContinue);
            this.grpControl.Controls.Add(this.btnStartByCurrent);
            this.grpControl.Controls.Add(this.btnStartBySearch);
            this.grpControl.Controls.Add(this.btnStartByLast);
            this.grpControl.Location = new System.Drawing.Point(7, 564);
            this.grpControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpControl.Name = "grpControl";
            this.grpControl.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpControl.Size = new System.Drawing.Size(1103, 76);
            this.grpControl.TabIndex = 14;
            this.grpControl.TabStop = false;
            this.grpControl.Text = "Crawler Controlling";
            // 
            // btnPauseContinue
            // 
            this.btnPauseContinue.Enabled = false;
            this.btnPauseContinue.Location = new System.Drawing.Point(956, 24);
            this.btnPauseContinue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPauseContinue.Name = "btnPauseContinue";
            this.btnPauseContinue.Size = new System.Drawing.Size(140, 38);
            this.btnPauseContinue.TabIndex = 20;
            this.btnPauseContinue.Text = "Pause/Continue";
            this.btnPauseContinue.UseVisualStyleBackColor = true;
            this.btnPauseContinue.Click += new System.EventHandler(this.btnPauseContinue_Click);
            // 
            // gpSetting
            // 
            this.gpSetting.Controls.Add(this.chkCrawlRetweets);
            this.gpSetting.Controls.Add(this.chkComment);
            this.gpSetting.Controls.Add(this.chkStatus);
            this.gpSetting.Controls.Add(this.chkTag);
            this.gpSetting.Controls.Add(this.chkUserInfo);
            this.gpSetting.Controls.Add(this.lblRobotSelect);
            this.gpSetting.Controls.Add(this.chkConfirmRelationship);
            this.gpSetting.Controls.Add(this.btnLoad);
            this.gpSetting.Controls.Add(this.grpDBSettings);
            this.gpSetting.Controls.Add(this.grpQueueLength);
            this.gpSetting.Controls.Add(this.btnDefault);
            this.gpSetting.Controls.Add(this.btnSave);
            this.gpSetting.Location = new System.Drawing.Point(7, 290);
            this.gpSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpSetting.Name = "gpSetting";
            this.gpSetting.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gpSetting.Size = new System.Drawing.Size(1101, 266);
            this.gpSetting.TabIndex = 15;
            this.gpSetting.TabStop = false;
            this.gpSetting.Text = "Options";
            // 
            // chkCrawlRetweets
            // 
            this.chkCrawlRetweets.AutoSize = true;
            this.chkCrawlRetweets.Checked = true;
            this.chkCrawlRetweets.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCrawlRetweets.Location = new System.Drawing.Point(520, 238);
            this.chkCrawlRetweets.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkCrawlRetweets.Name = "chkCrawlRetweets";
            this.chkCrawlRetweets.Size = new System.Drawing.Size(357, 19);
            this.chkCrawlRetweets.TabIndex = 32;
            this.chkCrawlRetweets.Text = "Crawl retweeting statuses for each status";
            this.chkCrawlRetweets.UseVisualStyleBackColor = true;
            // 
            // chkComment
            // 
            this.chkComment.AutoSize = true;
            this.chkComment.Checked = true;
            this.chkComment.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkComment.Location = new System.Drawing.Point(888, 238);
            this.chkComment.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkComment.Name = "chkComment";
            this.chkComment.Size = new System.Drawing.Size(141, 19);
            this.chkComment.TabIndex = 31;
            this.chkComment.Text = "Comments Robot";
            this.chkComment.UseVisualStyleBackColor = true;
            this.chkComment.CheckedChanged += new System.EventHandler(this.chkComment_CheckedChanged);
            // 
            // chkStatus
            // 
            this.chkStatus.AutoSize = true;
            this.chkStatus.Checked = true;
            this.chkStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStatus.Location = new System.Drawing.Point(368, 238);
            this.chkStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkStatus.Name = "chkStatus";
            this.chkStatus.Size = new System.Drawing.Size(141, 19);
            this.chkStatus.TabIndex = 30;
            this.chkStatus.Text = "Statuses Robot";
            this.chkStatus.UseVisualStyleBackColor = true;
            this.chkStatus.CheckedChanged += new System.EventHandler(this.chkStatus_CheckedChanged);
            // 
            // chkTag
            // 
            this.chkTag.AutoSize = true;
            this.chkTag.Checked = true;
            this.chkTag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTag.Location = new System.Drawing.Point(248, 238);
            this.chkTag.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkTag.Name = "chkTag";
            this.chkTag.Size = new System.Drawing.Size(109, 19);
            this.chkTag.TabIndex = 29;
            this.chkTag.Text = "Tags Robot";
            this.chkTag.UseVisualStyleBackColor = true;
            // 
            // chkUserInfo
            // 
            this.chkUserInfo.AutoSize = true;
            this.chkUserInfo.Checked = true;
            this.chkUserInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUserInfo.Location = new System.Drawing.Point(32, 238);
            this.chkUserInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkUserInfo.Name = "chkUserInfo";
            this.chkUserInfo.Size = new System.Drawing.Size(205, 19);
            this.chkUserInfo.TabIndex = 28;
            this.chkUserInfo.Text = "User Information Robot";
            this.chkUserInfo.UseVisualStyleBackColor = true;
            // 
            // lblRobotSelect
            // 
            this.lblRobotSelect.AutoSize = true;
            this.lblRobotSelect.Location = new System.Drawing.Point(33, 210);
            this.lblRobotSelect.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRobotSelect.Name = "lblRobotSelect";
            this.lblRobotSelect.Size = new System.Drawing.Size(431, 15);
            this.lblRobotSelect.TabIndex = 27;
            this.lblRobotSelect.Text = "Select Robots (User Relation Robot works by default):";
            // 
            // chkConfirmRelationship
            // 
            this.chkConfirmRelationship.AutoSize = true;
            this.chkConfirmRelationship.Location = new System.Drawing.Point(519, 210);
            this.chkConfirmRelationship.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkConfirmRelationship.Name = "chkConfirmRelationship";
            this.chkConfirmRelationship.Size = new System.Drawing.Size(389, 19);
            this.chkConfirmRelationship.TabIndex = 17;
            this.chkConfirmRelationship.Text = "Confirm Relationship(more precisely but slow)";
            this.chkConfirmRelationship.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(933, 158);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(160, 29);
            this.btnLoad.TabIndex = 12;
            this.btnLoad.Text = "Load Latest Value";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // grpDBSettings
            // 
            this.grpDBSettings.Controls.Add(this.txtDBName);
            this.grpDBSettings.Controls.Add(this.lblDBName);
            this.grpDBSettings.Controls.Add(this.drplstDBType);
            this.grpDBSettings.Controls.Add(this.txtDBPwd);
            this.grpDBSettings.Controls.Add(this.txtDBUserName);
            this.grpDBSettings.Controls.Add(this.txtDBServer);
            this.grpDBSettings.Controls.Add(this.lblDBType);
            this.grpDBSettings.Controls.Add(this.lblDBUserName);
            this.grpDBSettings.Controls.Add(this.lblDBServer);
            this.grpDBSettings.Controls.Add(this.lblDBPwd);
            this.grpDBSettings.Location = new System.Drawing.Point(13, 105);
            this.grpDBSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpDBSettings.Name = "grpDBSettings";
            this.grpDBSettings.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpDBSettings.Size = new System.Drawing.Size(907, 98);
            this.grpDBSettings.TabIndex = 13;
            this.grpDBSettings.TabStop = false;
            this.grpDBSettings.Text = "DateBase Options";
            // 
            // txtDBName
            // 
            this.txtDBName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBName.Location = new System.Drawing.Point(707, 58);
            this.txtDBName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDBName.MaxLength = 30;
            this.txtDBName.Name = "txtDBName";
            this.txtDBName.Size = new System.Drawing.Size(190, 25);
            this.txtDBName.TabIndex = 10;
            // 
            // lblDBName
            // 
            this.lblDBName.AutoSize = true;
            this.lblDBName.Location = new System.Drawing.Point(633, 66);
            this.lblDBName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDBName.Name = "lblDBName";
            this.lblDBName.Size = new System.Drawing.Size(71, 15);
            this.lblDBName.TabIndex = 14;
            this.lblDBName.Text = "DB Name:";
            this.lblDBName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drplstDBType
            // 
            this.drplstDBType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drplstDBType.FormattingEnabled = true;
            this.drplstDBType.Items.AddRange(new object[] {
            "SQL Server",
            "Oracle"});
            this.drplstDBType.Location = new System.Drawing.Point(120, 20);
            this.drplstDBType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.drplstDBType.Name = "drplstDBType";
            this.drplstDBType.Size = new System.Drawing.Size(160, 23);
            this.drplstDBType.TabIndex = 6;
            // 
            // txtDBPwd
            // 
            this.txtDBPwd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBPwd.Location = new System.Drawing.Point(413, 59);
            this.txtDBPwd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDBPwd.MaxLength = 50;
            this.txtDBPwd.Name = "txtDBPwd";
            this.txtDBPwd.PasswordChar = '*';
            this.txtDBPwd.Size = new System.Drawing.Size(193, 25);
            this.txtDBPwd.TabIndex = 9;
            // 
            // txtDBUserName
            // 
            this.txtDBUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBUserName.Location = new System.Drawing.Point(120, 59);
            this.txtDBUserName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDBUserName.MaxLength = 20;
            this.txtDBUserName.Name = "txtDBUserName";
            this.txtDBUserName.Size = new System.Drawing.Size(163, 25);
            this.txtDBUserName.TabIndex = 8;
            // 
            // txtDBServer
            // 
            this.txtDBServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBServer.Location = new System.Drawing.Point(584, 20);
            this.txtDBServer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDBServer.Name = "txtDBServer";
            this.txtDBServer.Size = new System.Drawing.Size(193, 25);
            this.txtDBServer.TabIndex = 7;
            // 
            // lblDBType
            // 
            this.lblDBType.AutoSize = true;
            this.lblDBType.Location = new System.Drawing.Point(36, 25);
            this.lblDBType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDBType.Name = "lblDBType";
            this.lblDBType.Size = new System.Drawing.Size(71, 15);
            this.lblDBType.TabIndex = 12;
            this.lblDBType.Text = "DB Type:";
            // 
            // lblDBUserName
            // 
            this.lblDBUserName.AutoSize = true;
            this.lblDBUserName.Location = new System.Drawing.Point(20, 66);
            this.lblDBUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDBUserName.Name = "lblDBUserName";
            this.lblDBUserName.Size = new System.Drawing.Size(87, 15);
            this.lblDBUserName.TabIndex = 7;
            this.lblDBUserName.Text = "DB UserID:";
            // 
            // lblDBServer
            // 
            this.lblDBServer.AutoSize = true;
            this.lblDBServer.Location = new System.Drawing.Point(467, 25);
            this.lblDBServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDBServer.Name = "lblDBServer";
            this.lblDBServer.Size = new System.Drawing.Size(87, 15);
            this.lblDBServer.TabIndex = 6;
            this.lblDBServer.Text = "DB Server:";
            // 
            // lblDBPwd
            // 
            this.lblDBPwd.AutoSize = true;
            this.lblDBPwd.Location = new System.Drawing.Point(307, 66);
            this.lblDBPwd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDBPwd.Name = "lblDBPwd";
            this.lblDBPwd.Size = new System.Drawing.Size(103, 15);
            this.lblDBPwd.TabIndex = 8;
            this.lblDBPwd.Text = "DB PassWord:";
            // 
            // grpQueueLength
            // 
            this.grpQueueLength.Controls.Add(this.lblQueueLength);
            this.grpQueueLength.Controls.Add(this.numQueueLength);
            this.grpQueueLength.Controls.Add(this.tbQueueLength);
            this.grpQueueLength.Location = new System.Drawing.Point(13, 20);
            this.grpQueueLength.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpQueueLength.Name = "grpQueueLength";
            this.grpQueueLength.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpQueueLength.Size = new System.Drawing.Size(1075, 82);
            this.grpQueueLength.TabIndex = 12;
            this.grpQueueLength.TabStop = false;
            this.grpQueueLength.Text = "Queue Length";
            // 
            // lblQueueLength
            // 
            this.lblQueueLength.AutoSize = true;
            this.lblQueueLength.Location = new System.Drawing.Point(11, 32);
            this.lblQueueLength.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblQueueLength.Name = "lblQueueLength";
            this.lblQueueLength.Size = new System.Drawing.Size(455, 15);
            this.lblQueueLength.TabIndex = 0;
            this.lblQueueLength.Text = "Queue Length in Memory(High performance by more memory):";
            // 
            // numQueueLength
            // 
            this.numQueueLength.Location = new System.Drawing.Point(980, 28);
            this.numQueueLength.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numQueueLength.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numQueueLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numQueueLength.Name = "numQueueLength";
            this.numQueueLength.Size = new System.Drawing.Size(81, 25);
            this.numQueueLength.TabIndex = 5;
            this.numQueueLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numQueueLength.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numQueueLength.ValueChanged += new System.EventHandler(this.numQueueLength_ValueChanged);
            // 
            // tbQueueLength
            // 
            this.tbQueueLength.LargeChange = 100;
            this.tbQueueLength.Location = new System.Drawing.Point(480, 18);
            this.tbQueueLength.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbQueueLength.Maximum = 100000;
            this.tbQueueLength.Minimum = 1;
            this.tbQueueLength.Name = "tbQueueLength";
            this.tbQueueLength.Size = new System.Drawing.Size(489, 53);
            this.tbQueueLength.SmallChange = 100;
            this.tbQueueLength.TabIndex = 4;
            this.tbQueueLength.TickFrequency = 1000;
            this.tbQueueLength.Value = 5000;
            this.tbQueueLength.ValueChanged += new System.EventHandler(this.tbQueueLength_ValueChanged);
            // 
            // btnDefault
            // 
            this.btnDefault.Location = new System.Drawing.Point(933, 121);
            this.btnDefault.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(160, 29);
            this.btnDefault.TabIndex = 11;
            this.btnDefault.Text = "Load Defaults";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(933, 194);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(160, 29);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "OK/Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblUserInfoTitle
            // 
            this.lblUserInfoTitle.AutoSize = true;
            this.lblUserInfoTitle.Location = new System.Drawing.Point(12, 44);
            this.lblUserInfoTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserInfoTitle.Name = "lblUserInfoTitle";
            this.lblUserInfoTitle.Size = new System.Drawing.Size(271, 15);
            this.lblUserInfoTitle.TabIndex = 16;
            this.lblUserInfoTitle.Text = "Status of User Information Robot:";
            this.lblUserInfoTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.lblCommentQueueInfo);
            this.grpStatus.Controls.Add(this.lblStatusQueueInfo);
            this.grpStatus.Controls.Add(this.lblUserTagQueueInfo);
            this.grpStatus.Controls.Add(this.lblUserRelationQueueInfo);
            this.grpStatus.Controls.Add(this.lblUserInfoQueueInfo);
            this.grpStatus.Controls.Add(this.lblCommentMessage);
            this.grpStatus.Controls.Add(this.lblCommentTitle);
            this.grpStatus.Controls.Add(this.lblStatusMessage);
            this.grpStatus.Controls.Add(this.lblUserTagMessage);
            this.grpStatus.Controls.Add(this.lblUserRelationMessage);
            this.grpStatus.Controls.Add(this.lblUserTagTitle);
            this.grpStatus.Controls.Add(this.lblUserInfoMessage);
            this.grpStatus.Controls.Add(this.lblUserRelationTitle);
            this.grpStatus.Controls.Add(this.lblStatusTitle);
            this.grpStatus.Controls.Add(this.lblUserInfoTitle);
            this.grpStatus.Location = new System.Drawing.Point(7, 648);
            this.grpStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpStatus.Size = new System.Drawing.Size(1103, 236);
            this.grpStatus.TabIndex = 17;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Crawler Status";
            // 
            // lblCommentQueueInfo
            // 
            this.lblCommentQueueInfo.AutoSize = true;
            this.lblCommentQueueInfo.Location = new System.Drawing.Point(15, 208);
            this.lblCommentQueueInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCommentQueueInfo.Name = "lblCommentQueueInfo";
            this.lblCommentQueueInfo.Size = new System.Drawing.Size(495, 15);
            this.lblCommentQueueInfo.TabIndex = 24;
            this.lblCommentQueueInfo.Text = "Queue of Comment Robot: 0 status in memory and 0 in database.";
            // 
            // lblStatusQueueInfo
            // 
            this.lblStatusQueueInfo.AutoSize = true;
            this.lblStatusQueueInfo.Location = new System.Drawing.Point(15, 188);
            this.lblStatusQueueInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusQueueInfo.Name = "lblStatusQueueInfo";
            this.lblStatusQueueInfo.Size = new System.Drawing.Size(471, 15);
            this.lblStatusQueueInfo.TabIndex = 23;
            this.lblStatusQueueInfo.Text = "Queue of Status Robot: 0 user in memory and 0 in datebase.";
            // 
            // lblUserTagQueueInfo
            // 
            this.lblUserTagQueueInfo.AutoSize = true;
            this.lblUserTagQueueInfo.Location = new System.Drawing.Point(15, 168);
            this.lblUserTagQueueInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserTagQueueInfo.Name = "lblUserTagQueueInfo";
            this.lblUserTagQueueInfo.Size = new System.Drawing.Size(487, 15);
            this.lblUserTagQueueInfo.TabIndex = 22;
            this.lblUserTagQueueInfo.Text = "Queue of User Tag Robot: 0 user in memory and 0 in datebase.";
            // 
            // lblUserRelationQueueInfo
            // 
            this.lblUserRelationQueueInfo.AutoSize = true;
            this.lblUserRelationQueueInfo.Location = new System.Drawing.Point(15, 128);
            this.lblUserRelationQueueInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserRelationQueueInfo.Name = "lblUserRelationQueueInfo";
            this.lblUserRelationQueueInfo.Size = new System.Drawing.Size(527, 15);
            this.lblUserRelationQueueInfo.TabIndex = 22;
            this.lblUserRelationQueueInfo.Text = "Queue of User Relation Robot: 0 user in memory and 0 in datebase.";
            // 
            // lblUserInfoQueueInfo
            // 
            this.lblUserInfoQueueInfo.AutoSize = true;
            this.lblUserInfoQueueInfo.Location = new System.Drawing.Point(15, 148);
            this.lblUserInfoQueueInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserInfoQueueInfo.Name = "lblUserInfoQueueInfo";
            this.lblUserInfoQueueInfo.Size = new System.Drawing.Size(551, 15);
            this.lblUserInfoQueueInfo.TabIndex = 22;
            this.lblUserInfoQueueInfo.Text = "Queue of User Information Robot: 0 user in memory and 0 in datebase.";
            // 
            // lblCommentMessage
            // 
            this.lblCommentMessage.AutoSize = true;
            this.lblCommentMessage.Location = new System.Drawing.Point(284, 105);
            this.lblCommentMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCommentMessage.Name = "lblCommentMessage";
            this.lblCommentMessage.Size = new System.Drawing.Size(71, 15);
            this.lblCommentMessage.TabIndex = 21;
            this.lblCommentMessage.Text = "Stopped.";
            this.lblCommentMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCommentTitle
            // 
            this.lblCommentTitle.AutoSize = true;
            this.lblCommentTitle.Location = new System.Drawing.Point(84, 105);
            this.lblCommentTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCommentTitle.Name = "lblCommentTitle";
            this.lblCommentTitle.Size = new System.Drawing.Size(199, 15);
            this.lblCommentTitle.TabIndex = 20;
            this.lblCommentTitle.Text = "Status of Comment Robot:";
            this.lblCommentTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblStatusMessage
            // 
            this.lblStatusMessage.AutoSize = true;
            this.lblStatusMessage.Location = new System.Drawing.Point(284, 85);
            this.lblStatusMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusMessage.Name = "lblStatusMessage";
            this.lblStatusMessage.Size = new System.Drawing.Size(71, 15);
            this.lblStatusMessage.TabIndex = 17;
            this.lblStatusMessage.Text = "Stopped.";
            this.lblStatusMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUserTagMessage
            // 
            this.lblUserTagMessage.AutoSize = true;
            this.lblUserTagMessage.Location = new System.Drawing.Point(284, 65);
            this.lblUserTagMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserTagMessage.Name = "lblUserTagMessage";
            this.lblUserTagMessage.Size = new System.Drawing.Size(71, 15);
            this.lblUserTagMessage.TabIndex = 17;
            this.lblUserTagMessage.Text = "Stopped.";
            this.lblUserTagMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUserRelationMessage
            // 
            this.lblUserRelationMessage.AutoSize = true;
            this.lblUserRelationMessage.Location = new System.Drawing.Point(284, 22);
            this.lblUserRelationMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserRelationMessage.Name = "lblUserRelationMessage";
            this.lblUserRelationMessage.Size = new System.Drawing.Size(71, 15);
            this.lblUserRelationMessage.TabIndex = 17;
            this.lblUserRelationMessage.Text = "Stopped.";
            this.lblUserRelationMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUserTagTitle
            // 
            this.lblUserTagTitle.AutoSize = true;
            this.lblUserTagTitle.Location = new System.Drawing.Point(76, 65);
            this.lblUserTagTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserTagTitle.Name = "lblUserTagTitle";
            this.lblUserTagTitle.Size = new System.Drawing.Size(207, 15);
            this.lblUserTagTitle.TabIndex = 16;
            this.lblUserTagTitle.Text = "Status of User Tag Robot:";
            this.lblUserTagTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblUserInfoMessage
            // 
            this.lblUserInfoMessage.AutoSize = true;
            this.lblUserInfoMessage.Location = new System.Drawing.Point(284, 44);
            this.lblUserInfoMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserInfoMessage.Name = "lblUserInfoMessage";
            this.lblUserInfoMessage.Size = new System.Drawing.Size(71, 15);
            this.lblUserInfoMessage.TabIndex = 17;
            this.lblUserInfoMessage.Text = "Stopped.";
            this.lblUserInfoMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUserRelationTitle
            // 
            this.lblUserRelationTitle.AutoSize = true;
            this.lblUserRelationTitle.Location = new System.Drawing.Point(37, 22);
            this.lblUserRelationTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserRelationTitle.Name = "lblUserRelationTitle";
            this.lblUserRelationTitle.Size = new System.Drawing.Size(247, 15);
            this.lblUserRelationTitle.TabIndex = 16;
            this.lblUserRelationTitle.Text = "Status of User Relation Robot:";
            this.lblUserRelationTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblStatusTitle
            // 
            this.lblStatusTitle.AutoSize = true;
            this.lblStatusTitle.Location = new System.Drawing.Point(92, 85);
            this.lblStatusTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusTitle.Name = "lblStatusTitle";
            this.lblStatusTitle.Size = new System.Drawing.Size(191, 15);
            this.lblStatusTitle.TabIndex = 16;
            this.lblStatusTitle.Text = "Status of Status Robot:";
            this.lblStatusTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnPost
            // 
            this.btnPost.Location = new System.Drawing.Point(240, 897);
            this.btnPost.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(333, 29);
            this.btnPost.TabIndex = 21;
            this.btnPost.Text = "Post a status to advertise :)THX";
            this.btnPost.UseVisualStyleBackColor = true;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1116, 939);
            this.Controls.Add(this.btnPost);
            this.Controls.Add(this.grpStatus);
            this.Controls.Add(this.gpSetting);
            this.Controls.Add(this.grpControl);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.grpSearch);
            this.Controls.Add(this.grpCurrentUser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sinawler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.grpUserInfo.ResumeLayout(false);
            this.grpUserInfo.PerformLayout();
            this.grpSearchCondition.ResumeLayout(false);
            this.grpSearchCondition.PerformLayout();
            this.grpCurrentUser.ResumeLayout(false);
            this.grpCurrentUser.PerformLayout();
            this.grpSearch.ResumeLayout(false);
            this.grpControl.ResumeLayout(false);
            this.gpSetting.ResumeLayout(false);
            this.gpSetting.PerformLayout();
            this.grpDBSettings.ResumeLayout(false);
            this.grpDBSettings.PerformLayout();
            this.grpQueueLength.ResumeLayout(false);
            this.grpQueueLength.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQueueLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbQueueLength)).EndInit();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartByLast;
        private System.Windows.Forms.GroupBox grpUserInfo;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblUserID;
        private System.Windows.Forms.Label lblStatusesCount;
        private System.Windows.Forms.Label lblCreatedAt;
        private System.Windows.Forms.Label lblFriendsCount;
        private System.Windows.Forms.Label lblFollowersCount;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label lblFollowing;
        private System.Windows.Forms.Label lblVerified;
        private System.Windows.Forms.Button btnStartBySearch;
        private System.Windows.Forms.Label lblSearchUserID;
        private System.Windows.Forms.Label lblSearchName;
        private System.Windows.Forms.TextBox txtUserID;
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
        private System.Windows.Forms.Label lblCUserID;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox grpControl;
        private System.Windows.Forms.GroupBox gpSetting;
        private System.Windows.Forms.Label lblQueueLength;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblUserInfoTitle;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.Label lblUserInfoMessage;
        private System.Windows.Forms.TrackBar tbQueueLength;
        private System.Windows.Forms.Label lblDBServer;
        private System.Windows.Forms.NumericUpDown numQueueLength;
        private System.Windows.Forms.TextBox txtDBPwd;
        private System.Windows.Forms.TextBox txtDBUserName;
        private System.Windows.Forms.TextBox txtDBServer;
        private System.Windows.Forms.Label lblDBPwd;
        private System.Windows.Forms.Label lblDBUserName;
        private System.Windows.Forms.GroupBox grpQueueLength;
        private System.Windows.Forms.GroupBox grpDBSettings;
        private System.Windows.Forms.ComboBox drplstDBType;
        private System.Windows.Forms.Label lblDBType;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblDBName;
        private System.Windows.Forms.TextBox txtDBName;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnPauseContinue;
        private System.Windows.Forms.Label lblStatusMessage;
        private System.Windows.Forms.Label lblStatusTitle;
        private System.Windows.Forms.Label lblCommentMessage;
        private System.Windows.Forms.Label lblCommentTitle;
        private System.Windows.Forms.Label lblCommentQueueInfo;
        private System.Windows.Forms.Label lblStatusQueueInfo;
        private System.Windows.Forms.Label lblUserInfoQueueInfo;
        private System.Windows.Forms.Label lblUserRelationMessage;
        private System.Windows.Forms.Label lblUserRelationTitle;
        private System.Windows.Forms.Label lblUserRelationQueueInfo;
        private System.Windows.Forms.Label lblUserTagQueueInfo;
        private System.Windows.Forms.Label lblUserTagMessage;
        private System.Windows.Forms.Label lblUserTagTitle;
        private System.Windows.Forms.CheckBox chkConfirmRelationship;
        private System.Windows.Forms.CheckBox chkCrawlRetweets;
        private System.Windows.Forms.CheckBox chkComment;
        private System.Windows.Forms.CheckBox chkStatus;
        private System.Windows.Forms.CheckBox chkTag;
        private System.Windows.Forms.CheckBox chkUserInfo;
        private System.Windows.Forms.Label lblRobotSelect;
    }
}

