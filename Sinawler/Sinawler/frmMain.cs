using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Sinawler.Model;
using Sina.Api;
using System.IO;
using System.Threading;

namespace Sinawler
{
    public partial class frmMain : Form
    {
        private bool blnAuthorized = false;
        private User oCurrentUser = new User();     //当前登录用户
        private User oSearchedUser = new User();    //搜索到的用户

        //分别用于五个机器人的线程
        private BackgroundWorker oAsyncWorkerUserInfo = null;
        private BackgroundWorker oAsyncWorkerUserRelation = null;
        private BackgroundWorker oAsyncWorkerUserTag = null;
        private BackgroundWorker oAsyncWorkerStatus = null;
        private BackgroundWorker oAsyncWorkerComment = null;

        //爬虫机器人，一个是爬取用户信息的，一个是爬取用户关系的，一个是爬取用户标签数据的，一个是爬取微博数据的，一个是爬取评论数据的，分别在五个线程中运行
        private UserInfoRobot robotUserInfo;
        private UserRelationRobot robotUserRelation;
        private UserTagRobot robotUserTag;
        private StatusRobot robotStatus;
        private CommentRobot robotComment;        

        private string strDataBaseStatus = "";      //数据库测试状态结果，OK为正常

        public frmMain()
        {
            InitializeComponent();
        }

        //加载设置
        private void LoadSettings()
        {
            SettingItems settings = AppSettings.Load();
            if (settings == null)
            {
                MessageBox.Show("Reading configuration failed. Defaults will be loaded.", "Sinawler");
                settings = AppSettings.LoadDefault();
            }
            ShowSettings(settings);
        }

        //关闭窗口时调用
        private bool CanBeClosed()
        {
            if (robotUserInfo != null && robotUserRelation != null && robotUserTag != null && robotStatus != null && robotComment != null)
            {
                if (!btnStartByCurrent.Enabled || !btnStartBySearch.Enabled || !btnStartByLast.Enabled)
                {
                    robotUserInfo.Suspending = true;    //先暂停
                    robotUserRelation.Suspending = true;    //先暂停
                    robotUserTag.Suspending = true;    //先暂停
                    robotStatus.Suspending = true;    //先暂停
                    robotComment.Suspending = true; //先暂停
                    if (MessageBox.Show("The crawler is working. Are you sure to stop it and exit?", "Sinawler", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        robotUserInfo.Suspending = false;
                        robotUserRelation.Suspending = false;
                        robotUserTag.Suspending = false;
                        robotStatus.Suspending = false;
                        robotComment.Suspending = false;
                        return false;
                    }
                }
            }
            return true;
        }

        //检查登录状态。若未登录，弹出登录框。登录后，设定机器人
        private void CheckLogin()
        {
            if (!blnAuthorized)
            {
                frmLogin login = new frmLogin();
                if (login.ShowDialog() == DialogResult.OK)
                {
                    blnAuthorized = true;
                    SinaMBCrawler crawler = new SinaMBCrawler(SysArgFor.USER_RELATION);
                    crawler.SleepTime = 0;  //这里不等待
                    oCurrentUser = crawler.GetCurrentUserInfo();
                    ShowCurrentUser();
                    oSearchedUser = oCurrentUser;

                    btnStartByCurrent.Enabled = true;
                    btnStartBySearch.Enabled = true;

                    if (robotUserInfo == null)
                        robotUserInfo = new UserInfoRobot();

                    if (robotUserRelation == null)
                        robotUserRelation = new UserRelationRobot();

                    if (robotUserTag == null)
                        robotUserTag = new UserTagRobot();

                    if (robotStatus == null)
                        robotStatus = new StatusRobot();

                    if (robotComment == null)
                        robotComment = new CommentRobot();
                }
            }
        }

        //显示登录帐号用户信息
        private void ShowCurrentUser()
        {
            if (oCurrentUser != null)
            {
                lblCUserID.Text = "UserID: " + oCurrentUser.user_id.ToString();
                lblCName.Text = "Nickname: " + oCurrentUser.screen_name;
                if (oCurrentUser.gender == "m")
                    lblCGender.Text = "Gender: Male";
                else
                    lblCGender.Text = "Gender: Female";
                if (oCurrentUser.verified)
                    lblCVerified.Text = "Verified: Yes";
                else
                    lblCVerified.Text = "Verified: No";
                if (oCurrentUser.following)
                    lblCFollowing.Text = "Followed by Current User: Yes";
                else
                    lblCFollowing.Text = "Followed by Current User: No";
                lblCLocation.Text = "Location: " + oCurrentUser.location;
                lblCFollowersCount.Text = "Followers: " + oCurrentUser.followers_count.ToString();
                lblCFriendsCount.Text = "Followings: " + oCurrentUser.friends_count.ToString();
                lblCStatusesCount.Text = "Tweets: " + oCurrentUser.statuses_count.ToString();
                lblCCreatedAt.Text = "Created At: " + oCurrentUser.created_at.Split(' ')[0];
            }
            else
            {
                lblCUserID.Text = "UserID:";
                lblCName.Text = "Nickname:";
                lblCGender.Text = "Gender:";
                lblCVerified.Text = "Verified:";
                lblCFollowing.Text = "Followed by Current User:";
                lblCLocation.Text = "Location:";
                lblCFollowersCount.Text = "Followers:";
                lblCFriendsCount.Text = "Followings:";
                lblCStatusesCount.Text = "Tweets:";
                lblCCreatedAt.Text = "Created At:";
            }
        }

        //显示搜索结果用户信息
        private void ShowSearchedUser()
        {
            if (oSearchedUser != null)
            {
                lblUserID.Text = "UserID：" + oSearchedUser.user_id.ToString();
                lblName.Text = "Nickname：" + oSearchedUser.screen_name;
                if (oSearchedUser.gender == "m")
                    lblGender.Text = "Gender: Male";
                else
                    lblGender.Text = "Gender: Female";
                if (oSearchedUser.verified)
                    lblVerified.Text = "Verified: Yes";
                else
                    lblVerified.Text = "Verified: No";
                if (oSearchedUser.following)
                    lblFollowing.Text = "Followed by Current User：Yes";
                else
                    lblFollowing.Text = "Followed by Current User：No";
                lblLocation.Text = "Location:" + oSearchedUser.location;
                lblFollowersCount.Text = "Followers: " + oSearchedUser.followers_count.ToString();
                lblFriendsCount.Text = "Followings: " + oSearchedUser.friends_count.ToString();
                lblStatusesCount.Text = "Tweets: " + oSearchedUser.statuses_count.ToString();
                lblCreatedAt.Text = "Created At: " + oSearchedUser.created_at.Split(' ')[0];
            }
            else
            {
                lblUserID.Text = "UserID:";
                lblName.Text = "Nickname:";
                lblGender.Text = "Gender:";
                lblVerified.Text = "Verified:";
                lblFollowing.Text = "Followed by Current User:";
                lblLocation.Text = "Location:";
                lblFollowersCount.Text = "Followers:";
                lblFriendsCount.Text = "Followings:";
                lblStatusesCount.Text = "Tweets:";
                lblCreatedAt.Text = "Created At:";
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            CheckLogin();
            drplstDBType.SelectedIndex = 0;
            LoadSettings();            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnSearchOnline_Click(object sender, EventArgs e)
        {
            CheckLogin();
            if (blnAuthorized)
            {
                if (txtUserID.Text.Trim() == "" && txtUserName.Text.Trim() == "")
                {
                    MessageBox.Show("UserID or Nickname Required.", "Sinawler");
                    txtUserID.Focus();
                    return;
                }
                string strUserID = txtUserID.Text.Trim();
                string strScreenName = txtUserName.Text.Trim();
                long lBuffer;
                if (strUserID != "" && !long.TryParse(strUserID, out lBuffer))
                {
                    MessageBox.Show("Invalid UserID.", "Sinawler");
                    return;
                }
                SinaMBCrawler crawler = new SinaMBCrawler(SysArgFor.USER_RELATION);
                crawler.SleepTime = 0;  //这里不等待
                if (strUserID != "" && strScreenName == "")
                    oSearchedUser = crawler.GetUserInfo(Convert.ToInt64(strUserID));
                if (strUserID == "" && strScreenName != "")
                    oSearchedUser = crawler.GetUserInfo(strScreenName);
                if (strUserID != "" && strScreenName != "")
                    oSearchedUser = crawler.GetUserInfo(Convert.ToInt64(strUserID), strScreenName);
                if (oSearchedUser == null) MessageBox.Show("User not found.", "Sinawler");
                ShowSearchedUser();
            }
            else
            {
                btnSearchOnline.Enabled = true;
                btnSearchOffLine.Enabled = true;
                btnStartByCurrent.Enabled = false;
                btnStartBySearch.Enabled = false;
                btnStartByLast.Enabled = true;
            }
        }

        private void btnSearchOffLine_Click(object sender, EventArgs e)
        {
            if (txtUserID.Text.Trim() == "" && txtUserName.Text.Trim() == "")
            {
                MessageBox.Show("UserID or Nickname Required.", "Sinawler");
                txtUserID.Focus();
                return;
            }
            strDataBaseStatus = PubHelper.TestDataBase();
            if (strDataBaseStatus != "OK")
            {
                MessageBox.Show("Database Error: " + strDataBaseStatus, "Sinawler");
                return;
            }
            string strUserID = txtUserID.Text.Trim();
            string strScreenName = txtUserName.Text.Trim();
            long lBuffer;
            if (strUserID != "" && !long.TryParse(strUserID, out lBuffer))
            {
                MessageBox.Show("Invalid UserID.", "Sinawer");
                return;
            }
            if (strUserID != "" && strScreenName == "")
            {
                if (!oSearchedUser.GetModel(Convert.ToInt64(strUserID)))
                {
                    MessageBox.Show("User not found.", "Sinawler");
                }
                else
                    ShowSearchedUser();
            }
            if (strUserID == "" && strScreenName != "")
            {
                if (!oSearchedUser.GetModel(strScreenName))
                {
                    MessageBox.Show("User not found.", "Sinawler");
                }
                else
                    ShowSearchedUser();
            }
            if (strUserID != "" && strScreenName != "")
            {
                if (!oSearchedUser.GetModel(Convert.ToInt64(strUserID), strScreenName))
                {
                    MessageBox.Show("User not found.", "Sinawler");
                }
                else
                    ShowSearchedUser();
            }
        }

        //开始任务前初始化robot等
        private void PrepareToStart()
        {
            robotUserInfo.Initialize();
            robotUserRelation.Initialize();
            robotUserTag.Initialize();
            robotStatus.Initialize();
            robotComment.Initialize();
            GlobalPool.UserBuffer.Initialize();

            robotUserInfo.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userInfo.log";
            robotUserRelation.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userRelation.log";
            robotUserTag.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userTag.log";
            robotStatus.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
            robotComment.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_comment.log";

            SettingItems settings = AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            GlobalPool.UserQueueForUserInfoRobot.MaxLengthInMem = settings.MaxLengthInMem;
            GlobalPool.UserQueueForUserRelationRobot.MaxLengthInMem = settings.MaxLengthInMem;
            GlobalPool.UserQueueForUserTagRobot.MaxLengthInMem = settings.MaxLengthInMem;
            GlobalPool.UserQueueForStatusRobot.MaxLengthInMem = settings.MaxLengthInMem;
            GlobalPool.StatusQueue.MaxLengthInMem = settings.MaxLengthInMem;
            GlobalPool.UserBuffer.MaxLengthInMem = settings.MaxLengthInMem;
            GlobalPool.UserInfoRobotEnabled = chkUserInfo.Checked;
            GlobalPool.TagRobotEnabled = chkTag.Checked;
            GlobalPool.StatusRobotEnabled = chkStatus.Checked;
            GlobalPool.CommentRobotEnabled = chkComment.Checked;
            if (optJSON.Checked)
            {
                settings.Format = DataFormat.JSON;
                GlobalPool.GetAPI(SysArgFor.USER_RELATION).API.Format = "json";
                GlobalPool.GetAPI(SysArgFor.USER_INFO).API.Format = "json";
                GlobalPool.GetAPI(SysArgFor.USER_TAG).API.Format = "json";
                GlobalPool.GetAPI(SysArgFor.STATUS).API.Format = "json";
                GlobalPool.GetAPI(SysArgFor.COMMENT).API.Format = "json";
            }
            if (optXML.Checked)
            {
                settings.Format = DataFormat.XML;
                GlobalPool.GetAPI(SysArgFor.USER_RELATION).API.Format = "xml";
                GlobalPool.GetAPI(SysArgFor.USER_INFO).API.Format = "xml";
                GlobalPool.GetAPI(SysArgFor.USER_TAG).API.Format = "xml";
                GlobalPool.GetAPI(SysArgFor.STATUS).API.Format = "xml";
                GlobalPool.GetAPI(SysArgFor.COMMENT).API.Format = "xml";
            }
        }

        private void btnStartByCurrent_Click(object sender, EventArgs e)
        {
            CheckLogin();
            if (blnAuthorized)
            {
                strDataBaseStatus = PubHelper.TestDataBase();
                if (strDataBaseStatus != "OK")
                {
                    MessageBox.Show("Database Error: " + strDataBaseStatus, "Sinawler");
                    return;
                }

                btnStartByCurrent.Text = "Initializing. Please wait...";
                btnStartByCurrent.Enabled = false;
                btnStartBySearch.Enabled = false;
                btnStartByLast.Enabled = false;

                chkUserInfo.Enabled = false;
                chkTag.Enabled = false;
                chkStatus.Enabled = false;
                chkComment.Enabled = false;
                optJSON.Enabled = false;
                optXML.Enabled = false;
                Application.DoEvents();

                PrepareToStart();

                if (oAsyncWorkerUserRelation == null)
                {
                    oAsyncWorkerUserRelation = new BackgroundWorker();
                    oAsyncWorkerUserRelation.WorkerReportsProgress = true;
                    oAsyncWorkerUserRelation.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserRelation.ProgressChanged += new ProgressChangedEventHandler(UserRelationProgressChanged);
                    oAsyncWorkerUserRelation.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserRelationCompleteWork);
                    oAsyncWorkerUserRelation.DoWork += new DoWorkEventHandler(StartCrawUserRelationByCurrentUser);
                    robotUserRelation.AsyncWorker = oAsyncWorkerUserRelation;
                }

                if (oAsyncWorkerUserInfo == null && chkUserInfo.Checked)
                {
                    oAsyncWorkerUserInfo = new BackgroundWorker();
                    oAsyncWorkerUserInfo.WorkerReportsProgress = true;
                    oAsyncWorkerUserInfo.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserInfo.ProgressChanged += new ProgressChangedEventHandler(UserInfoProgressChanged);
                    oAsyncWorkerUserInfo.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserInfoCompleteWork);
                    oAsyncWorkerUserInfo.DoWork += new DoWorkEventHandler(StartCrawUserInfo);
                    robotUserInfo.AsyncWorker = oAsyncWorkerUserInfo;
                }

                if (oAsyncWorkerUserTag == null && chkTag.Checked)
                {
                    oAsyncWorkerUserTag = new BackgroundWorker();
                    oAsyncWorkerUserTag.WorkerReportsProgress = true;
                    oAsyncWorkerUserTag.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserTag.ProgressChanged += new ProgressChangedEventHandler(UserTagProgressChanged);
                    oAsyncWorkerUserTag.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserTagCompleteWork);
                    oAsyncWorkerUserTag.DoWork += new DoWorkEventHandler(StartCrawUserTag);
                    robotUserTag.AsyncWorker = oAsyncWorkerUserTag;
                }

                if (oAsyncWorkerStatus == null && chkStatus.Checked)
                {
                    oAsyncWorkerStatus = new BackgroundWorker();
                    oAsyncWorkerStatus.WorkerReportsProgress = true;
                    oAsyncWorkerStatus.WorkerSupportsCancellation = true;
                    oAsyncWorkerStatus.ProgressChanged += new ProgressChangedEventHandler(StatusProgressChanged);
                    oAsyncWorkerStatus.RunWorkerCompleted += new RunWorkerCompletedEventHandler(StatusCompleteWork);
                    oAsyncWorkerStatus.DoWork += new DoWorkEventHandler(StartCrawStatus);
                    robotStatus.AsyncWorker = oAsyncWorkerStatus;
                }

                if (oAsyncWorkerComment == null && chkComment.Checked)
                {
                    oAsyncWorkerComment = new BackgroundWorker();
                    oAsyncWorkerComment.WorkerReportsProgress = true;
                    oAsyncWorkerComment.WorkerSupportsCancellation = true;
                    oAsyncWorkerComment.ProgressChanged += new ProgressChangedEventHandler(CommentProgressChanged);
                    oAsyncWorkerComment.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CommentCompleteWork);
                    oAsyncWorkerComment.DoWork += new DoWorkEventHandler(StartCrawComment);
                    robotComment.AsyncWorker = oAsyncWorkerComment;
                }

                if (oAsyncWorkerUserRelation.IsBusy || 
                    (oAsyncWorkerUserInfo != null && oAsyncWorkerUserInfo.IsBusy) || 
                    (oAsyncWorkerUserTag != null && oAsyncWorkerUserTag.IsBusy) || 
                    (oAsyncWorkerStatus!=null && oAsyncWorkerStatus.IsBusy) || 
                    (oAsyncWorkerComment!=null && oAsyncWorkerComment.IsBusy))
                {
                    //记录原状态
                    bool userInfoState = robotUserInfo.Suspending;
                    bool userRelationState = robotUserRelation.Suspending;
                    bool userTagState = robotUserTag.Suspending;
                    bool statusState = robotStatus.Suspending;
                    bool commentState = robotComment.Suspending;
                    robotUserInfo.Suspending = true;    //暂停
                    robotUserRelation.Suspending = true;    //暂停
                    robotUserTag.Suspending = true;    //暂停
                    robotStatus.Suspending = true;    //暂停
                    robotComment.Suspending = true; //暂停
                    if (MessageBox.Show("Are you sure to stop crawling?", "Sinawler", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        //恢复状态
                        robotUserInfo.Suspending = userInfoState;
                        robotUserRelation.Suspending = userRelationState;
                        robotUserTag.Suspending = userTagState;
                        robotStatus.Suspending = statusState;
                        robotComment.Suspending = commentState;
                        return;
                    }
                    btnStartByCurrent.Enabled = false;
                    btnStartByCurrent.Text = "Stopping. Please wait...";
                    btnPauseContinue.Enabled = false;
                    robotUserInfo.AsyncCancelled = true;
                    robotUserRelation.AsyncCancelled = true;
                    robotUserTag.AsyncCancelled = true;
                    robotStatus.AsyncCancelled = true;
                    robotComment.AsyncCancelled = true;

                    oAsyncWorkerUserRelation.CancelAsync();
                    if(oAsyncWorkerUserInfo!=null)  oAsyncWorkerUserInfo.CancelAsync();
                    if(oAsyncWorkerUserTag!=null)   oAsyncWorkerUserTag.CancelAsync();
                    if(oAsyncWorkerStatus!=null)    oAsyncWorkerStatus.CancelAsync();
                    if(oAsyncWorkerComment!=null)   oAsyncWorkerComment.CancelAsync();
                }
                else
                {
                    btnStartByCurrent.Text = "STOP";
                    btnStartByCurrent.Enabled = true;
                    btnStartBySearch.Enabled = false;
                    btnStartByLast.Enabled = false;
                    btnPauseContinue.Text = "PAUSE";
                    btnPauseContinue.Enabled = true;

                    oAsyncWorkerUserRelation.RunWorkerAsync();
                    if(oAsyncWorkerUserInfo!=null)  oAsyncWorkerUserInfo.RunWorkerAsync();
                    if(oAsyncWorkerUserTag!=null)   oAsyncWorkerUserTag.RunWorkerAsync();
                    if(oAsyncWorkerStatus!=null)    oAsyncWorkerStatus.RunWorkerAsync();
                    if(oAsyncWorkerComment!=null)   oAsyncWorkerComment.RunWorkerAsync();
                }
            }
        }

        private void btnStartBySearch_Click(object sender, EventArgs e)
        {
            CheckLogin();
            if (blnAuthorized)
            {
                if (oSearchedUser == null)
                {
                    MessageBox.Show(this, "No searched user. Please search user first.", "Sinawler");
                    return;
                }
                strDataBaseStatus = PubHelper.TestDataBase();
                if (strDataBaseStatus != "OK")
                {
                    MessageBox.Show("Database Error: " + strDataBaseStatus, "Sinawler");
                    return;
                }

                btnStartBySearch.Text = "Initializing. Please wait...";
                btnStartBySearch.Enabled = false;
                btnStartByCurrent.Enabled = false;
                btnStartByLast.Enabled = false;

                chkUserInfo.Enabled = false;
                chkTag.Enabled = false;
                chkStatus.Enabled = false;
                chkComment.Enabled = false;
                optJSON.Enabled = false;
                optXML.Enabled = false;
                Application.DoEvents();

                PrepareToStart();

                if (oAsyncWorkerUserRelation == null)
                {
                    oAsyncWorkerUserRelation = new BackgroundWorker();
                    oAsyncWorkerUserRelation.WorkerReportsProgress = true;
                    oAsyncWorkerUserRelation.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserRelation.ProgressChanged += new ProgressChangedEventHandler(UserRelationProgressChanged);
                    oAsyncWorkerUserRelation.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserRelationCompleteWork);
                    oAsyncWorkerUserRelation.DoWork += new DoWorkEventHandler(StartCrawUserRelationBySearchedUser);
                    robotUserRelation.AsyncWorker = oAsyncWorkerUserRelation;
                }

                if (oAsyncWorkerUserInfo == null && chkUserInfo.Checked)
                {
                    oAsyncWorkerUserInfo = new BackgroundWorker();
                    oAsyncWorkerUserInfo.WorkerReportsProgress = true;
                    oAsyncWorkerUserInfo.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserInfo.ProgressChanged += new ProgressChangedEventHandler(UserInfoProgressChanged);
                    oAsyncWorkerUserInfo.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserInfoCompleteWork);
                    oAsyncWorkerUserInfo.DoWork += new DoWorkEventHandler(StartCrawUserInfo);
                    robotUserInfo.AsyncWorker = oAsyncWorkerUserInfo;
                }

                if (oAsyncWorkerUserTag == null && chkTag.Checked)
                {
                    oAsyncWorkerUserTag = new BackgroundWorker();
                    oAsyncWorkerUserTag.WorkerReportsProgress = true;
                    oAsyncWorkerUserTag.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserTag.ProgressChanged += new ProgressChangedEventHandler(UserTagProgressChanged);
                    oAsyncWorkerUserTag.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserTagCompleteWork);
                    oAsyncWorkerUserTag.DoWork += new DoWorkEventHandler(StartCrawUserTag);
                    robotUserTag.AsyncWorker = oAsyncWorkerUserTag;
                }

                if (oAsyncWorkerStatus == null && chkStatus.Checked)
                {
                    oAsyncWorkerStatus = new BackgroundWorker();
                    oAsyncWorkerStatus.WorkerReportsProgress = true;
                    oAsyncWorkerStatus.WorkerSupportsCancellation = true;
                    oAsyncWorkerStatus.ProgressChanged += new ProgressChangedEventHandler(StatusProgressChanged);
                    oAsyncWorkerStatus.RunWorkerCompleted += new RunWorkerCompletedEventHandler(StatusCompleteWork);
                    oAsyncWorkerStatus.DoWork += new DoWorkEventHandler(StartCrawStatus);
                    robotStatus.AsyncWorker = oAsyncWorkerStatus;
                }

                if (oAsyncWorkerComment == null && chkComment.Checked)
                {
                    oAsyncWorkerComment = new BackgroundWorker();
                    oAsyncWorkerComment.WorkerReportsProgress = true;
                    oAsyncWorkerComment.WorkerSupportsCancellation = true;
                    oAsyncWorkerComment.ProgressChanged += new ProgressChangedEventHandler(CommentProgressChanged);
                    oAsyncWorkerComment.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CommentCompleteWork);
                    oAsyncWorkerComment.DoWork += new DoWorkEventHandler(StartCrawComment);
                    robotComment.AsyncWorker = oAsyncWorkerComment;
                }

                if (oAsyncWorkerUserRelation.IsBusy || 
                    (oAsyncWorkerUserInfo!=null && oAsyncWorkerUserInfo.IsBusy) || 
                    (oAsyncWorkerUserTag!=null && oAsyncWorkerUserTag.IsBusy) || 
                    (oAsyncWorkerStatus!=null && oAsyncWorkerStatus.IsBusy) || 
                    (oAsyncWorkerComment!=null && oAsyncWorkerComment.IsBusy))
                {
                    //记录原状态
                    bool userInfoState = robotUserInfo.Suspending;
                    bool userRelationState = robotUserRelation.Suspending;
                    bool userTagState = robotUserTag.Suspending;
                    bool statusState = robotStatus.Suspending;
                    bool commentState = robotComment.Suspending;
                    robotUserInfo.Suspending = true;    //暂停
                    robotUserRelation.Suspending = true;    //暂停
                    robotUserTag.Suspending = true;    //暂停
                    robotStatus.Suspending = true;    //暂停
                    robotComment.Suspending = true; //暂停
                    if (MessageBox.Show("Are you sure to stop crawling?", "Sinawler", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        //恢复状态
                        robotUserInfo.Suspending = userInfoState;
                        robotUserRelation.Suspending = userRelationState;
                        robotUserTag.Suspending = userTagState;
                        robotStatus.Suspending = statusState;
                        robotComment.Suspending = commentState;
                        return;
                    }
                    btnStartBySearch.Enabled = false;
                    btnStartBySearch.Text = "Stopping. Please wait...";
                    btnPauseContinue.Enabled = false;
                    robotUserInfo.AsyncCancelled = true;
                    robotUserRelation.AsyncCancelled = true;
                    robotUserTag.AsyncCancelled = true;
                    robotStatus.AsyncCancelled = true;
                    robotComment.AsyncCancelled = true;

                    oAsyncWorkerUserRelation.CancelAsync();
                    if(oAsyncWorkerUserInfo!=null)  oAsyncWorkerUserInfo.CancelAsync();
                    if(oAsyncWorkerUserTag!=null)   oAsyncWorkerUserTag.CancelAsync();
                    if(oAsyncWorkerStatus!=null)    oAsyncWorkerStatus.CancelAsync();
                    if(oAsyncWorkerComment!=null)   oAsyncWorkerComment.CancelAsync();
                }
                else
                {
                    btnStartBySearch.Text = "STOP";
                    btnStartBySearch.Enabled = true;
                    btnStartByCurrent.Enabled = false;
                    btnStartByLast.Enabled = false;
                    btnPauseContinue.Text = "PAUSE";
                    btnPauseContinue.Enabled = true;

                    oAsyncWorkerUserRelation.RunWorkerAsync();
                    if(oAsyncWorkerUserInfo!=null)  oAsyncWorkerUserInfo.RunWorkerAsync();
                    if (oAsyncWorkerUserTag != null) oAsyncWorkerUserTag.RunWorkerAsync();
                    if (oAsyncWorkerStatus != null) oAsyncWorkerStatus.RunWorkerAsync();
                    if (oAsyncWorkerComment != null) oAsyncWorkerComment.RunWorkerAsync();
                }
            }
        }

        private void btnStartByLast_Click(object sender, EventArgs e)
        {
            CheckLogin();
            if (blnAuthorized)
            {
                strDataBaseStatus = PubHelper.TestDataBase();
                if (strDataBaseStatus != "OK")
                {
                    MessageBox.Show("Database Error: " + strDataBaseStatus, "Sinawler");
                    return;
                }
                if (SysArg.GetCurrentID(SysArgFor.USER_RELATION) == 0)
                {
                    MessageBox.Show(this, "No user stopped last time. Please select another start point.", "Sinawler");
                    return;
                }

                btnStartByLast.Text = "Initializing. Please wait...";
                btnStartByLast.Enabled = false;
                btnStartBySearch.Enabled = false;
                btnStartByCurrent.Enabled = false;

                chkUserInfo.Enabled = false;
                chkTag.Enabled = false;
                chkStatus.Enabled = false;
                chkComment.Enabled = false;
                optJSON.Enabled = false;
                optXML.Enabled = false;
                Application.DoEvents();

                PrepareToStart();

                if (oAsyncWorkerUserRelation == null)
                {
                    oAsyncWorkerUserRelation = new BackgroundWorker();
                    oAsyncWorkerUserRelation.WorkerReportsProgress = true;
                    oAsyncWorkerUserRelation.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserRelation.ProgressChanged += new ProgressChangedEventHandler(UserRelationProgressChanged);
                    oAsyncWorkerUserRelation.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserRelationCompleteWork);
                    oAsyncWorkerUserRelation.DoWork += new DoWorkEventHandler(StartCrawUserRelationByLastUser);
                    robotUserRelation.AsyncWorker = oAsyncWorkerUserRelation;
                }

                if (oAsyncWorkerUserInfo == null && chkUserInfo.Checked)
                {
                    oAsyncWorkerUserInfo = new BackgroundWorker();
                    oAsyncWorkerUserInfo.WorkerReportsProgress = true;
                    oAsyncWorkerUserInfo.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserInfo.ProgressChanged += new ProgressChangedEventHandler(UserInfoProgressChanged);
                    oAsyncWorkerUserInfo.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserInfoCompleteWork);
                    oAsyncWorkerUserInfo.DoWork += new DoWorkEventHandler(StartCrawUserInfo);
                    robotUserInfo.AsyncWorker = oAsyncWorkerUserInfo;
                }

                if (oAsyncWorkerUserTag == null && chkTag.Checked)
                {
                    oAsyncWorkerUserTag = new BackgroundWorker();
                    oAsyncWorkerUserTag.WorkerReportsProgress = true;
                    oAsyncWorkerUserTag.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserTag.ProgressChanged += new ProgressChangedEventHandler(UserTagProgressChanged);
                    oAsyncWorkerUserTag.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserTagCompleteWork);
                    oAsyncWorkerUserTag.DoWork += new DoWorkEventHandler(StartCrawUserTag);
                    robotUserTag.AsyncWorker = oAsyncWorkerUserTag;
                }

                if (oAsyncWorkerStatus == null && chkStatus.Checked)
                {
                    oAsyncWorkerStatus = new BackgroundWorker();
                    oAsyncWorkerStatus.WorkerReportsProgress = true;
                    oAsyncWorkerStatus.WorkerSupportsCancellation = true;
                    oAsyncWorkerStatus.ProgressChanged += new ProgressChangedEventHandler(StatusProgressChanged);
                    oAsyncWorkerStatus.RunWorkerCompleted += new RunWorkerCompletedEventHandler(StatusCompleteWork);
                    oAsyncWorkerStatus.DoWork += new DoWorkEventHandler(StartCrawStatus);
                    robotStatus.AsyncWorker = oAsyncWorkerStatus;
                }

                if (oAsyncWorkerComment == null && chkComment.Checked)
                {
                    oAsyncWorkerComment = new BackgroundWorker();
                    oAsyncWorkerComment.WorkerReportsProgress = true;
                    oAsyncWorkerComment.WorkerSupportsCancellation = true;
                    oAsyncWorkerComment.ProgressChanged += new ProgressChangedEventHandler(CommentProgressChanged);
                    oAsyncWorkerComment.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CommentCompleteWork);
                    oAsyncWorkerComment.DoWork += new DoWorkEventHandler(StartCrawComment);
                    robotComment.AsyncWorker = oAsyncWorkerComment;
                }

                if (oAsyncWorkerUserRelation.IsBusy || 
                    (oAsyncWorkerUserInfo!=null && oAsyncWorkerUserInfo.IsBusy) || 
                    (oAsyncWorkerUserTag!=null && oAsyncWorkerUserTag.IsBusy) || 
                    (oAsyncWorkerStatus!=null && oAsyncWorkerStatus.IsBusy) || 
                    (oAsyncWorkerComment!=null && oAsyncWorkerComment.IsBusy))
                {
                    //记录原状态
                    bool userInfoState = robotUserInfo.Suspending;
                    bool userRelationState = robotUserRelation.Suspending;
                    bool userTagState = robotUserTag.Suspending;
                    bool statusState = robotStatus.Suspending;
                    bool commentState = robotComment.Suspending;
                    robotUserInfo.Suspending = true;    //暂停
                    robotUserRelation.Suspending = true;    //暂停
                    robotUserTag.Suspending = true;    //暂停
                    robotStatus.Suspending = true;    //暂停
                    robotComment.Suspending = true; //暂停
                    if (MessageBox.Show("Are you sure to stop crawling?", "Sinawler", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        //恢复状态
                        robotUserInfo.Suspending = userInfoState;
                        robotUserRelation.Suspending = userRelationState;
                        robotUserTag.Suspending = userTagState;
                        robotStatus.Suspending = statusState;
                        robotComment.Suspending = commentState;
                        return;
                    }
                    btnStartByLast.Enabled = false;
                    btnStartByLast.Text = "Stopping. Please wait...";
                    btnPauseContinue.Enabled = false;
                    robotUserInfo.AsyncCancelled = true;
                    robotUserRelation.AsyncCancelled = true;
                    robotUserTag.AsyncCancelled = true;
                    robotStatus.AsyncCancelled = true;
                    robotComment.AsyncCancelled = true;

                    oAsyncWorkerUserRelation.CancelAsync();
                    if(oAsyncWorkerUserInfo!=null)  oAsyncWorkerUserInfo.CancelAsync();
                    if (oAsyncWorkerUserTag != null) oAsyncWorkerUserTag.CancelAsync();
                    if (oAsyncWorkerStatus != null) oAsyncWorkerStatus.CancelAsync();
                    if (oAsyncWorkerComment != null) oAsyncWorkerComment.CancelAsync();
                }
                else
                {
                    btnStartByLast.Text = "STOP";
                    btnStartByLast.Enabled = true;
                    btnStartBySearch.Enabled = false;
                    btnStartByCurrent.Enabled = false;
                    btnPauseContinue.Text = "PAUSE";
                    btnPauseContinue.Enabled = true;

                    oAsyncWorkerUserRelation.RunWorkerAsync();
                    if (oAsyncWorkerUserInfo != null) oAsyncWorkerUserInfo.RunWorkerAsync();
                    if (oAsyncWorkerUserTag != null) oAsyncWorkerUserTag.RunWorkerAsync();
                    if (oAsyncWorkerStatus != null) oAsyncWorkerStatus.RunWorkerAsync();
                    if (oAsyncWorkerComment != null) oAsyncWorkerComment.RunWorkerAsync();
                }
            }
        }

        #region 用户信息机器人
        private void StartCrawUserInfo(Object sender, DoWorkEventArgs e)
        {
            robotUserInfo.Start();
        }

        private void UserInfoProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            lblUserInfoMessage.Text = robotUserInfo.LogMessage;
            lblUserInfoQueueInfo.Text = "Queue of User Information Robot: " + GlobalPool.UserQueueForUserInfoRobot.CountInMem.ToString() + " users in memory and " + GlobalPool.UserQueueForUserInfoRobot.CountInDB.ToString() + " in database.";            
        }

        private void UserInfoCompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            robotUserInfo.Initialize();
            lblUserInfoMessage.Text = "Stopped.";
            lblUserInfoQueueInfo.Text = "Queue of User Information Robot: 0 user in memory and 0 user in datebase.";

            if (oAsyncWorkerUserRelation == null && oAsyncWorkerUserTag == null && oAsyncWorkerStatus == null && oAsyncWorkerComment == null)  //如果另外四个机器人也已停止
            {
                btnStartByCurrent.Text = "Start Crawling by Current User";
                btnStartBySearch.Text = "Start Crawling by Searched User";
                btnStartByLast.Text = "Start Crawling by Last Stopped User";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "Pause/Continue";

                MessageBox.Show(this, "Crawler stopped.", "Sinawler");

                chkUserInfo.Enabled = true;
                chkTag.Enabled = true;
                chkStatus.Enabled = true;
                chkComment.Enabled = true;
                optJSON.Enabled = true;
                optXML.Enabled = true;
            }
            oAsyncWorkerUserInfo = null;
        }
        #endregion

        #region 用户关系机器人
        private void StartCrawUserRelationByCurrentUser(Object sender, DoWorkEventArgs e)
        {
            robotUserRelation.Start(oCurrentUser.user_id);
        }

        private void StartCrawUserRelationBySearchedUser(Object sender, DoWorkEventArgs e)
        {
            robotUserRelation.Start(oSearchedUser.user_id);
        }

        private void StartCrawUserRelationByLastUser(Object sender, DoWorkEventArgs e)
        {
            long lLastUserID = SysArg.GetCurrentID(SysArgFor.USER_RELATION);
            if (lLastUserID == 0)
            {
                lLastUserID = SysArg.GetCurrentID(SysArgFor.USER_INFO);
                if (lLastUserID == 0)
                {
                    lLastUserID = SysArg.GetCurrentID(SysArgFor.USER_TAG);
                    if (lLastUserID == 0)
                    {
                        MessageBox.Show(this, "No user stopped last time. Please select another start point.", "Sinawler");
                        return;
                    }
                }
            }
            robotUserRelation.Start(lLastUserID);
        }

        private void UserRelationProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            lblUserRelationMessage.Text = robotUserRelation.LogMessage;
            lblUserRelationQueueInfo.Text = "Queue of User Relation Robot: " + GlobalPool.UserQueueForUserRelationRobot.CountInMem.ToString() + " users in memory and " + GlobalPool.UserQueueForUserRelationRobot.CountInDB.ToString() + " in database.";            
        }

        private void UserRelationCompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            robotUserRelation.Initialize();
            lblUserRelationMessage.Text = "Stopped.";
            lblUserRelationQueueInfo.Text = "Queue of User Relation Robot: 0 user in memory and 0 user in datebase.";


            if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserTag == null && oAsyncWorkerStatus == null && oAsyncWorkerComment == null)  //如果另外四个机器人也已停止
            {
                btnStartByCurrent.Text = "Start Crawling by Current User";
                btnStartBySearch.Text = "Start Crawling by Searched User";
                btnStartByLast.Text = "Start Crawling by Last Stopped User";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "Pause/Continue";

                MessageBox.Show(this, "Crawler stopped.", "Sinawler");

                chkUserInfo.Enabled = true;
                chkTag.Enabled = true;
                chkStatus.Enabled = true;
                chkComment.Enabled = true;
                optJSON.Enabled = true;
                optXML.Enabled = true;
            }
            oAsyncWorkerUserRelation = null;
        }
        #endregion

        #region 用户标签机器人
        private void StartCrawUserTag(Object sender, DoWorkEventArgs e)
        {
            robotUserTag.Start();
        }

        private void UserTagProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            lblUserTagMessage.Text = robotUserTag.LogMessage;
            lblUserTagQueueInfo.Text = "Queue of User Tag Robot: " + GlobalPool.UserQueueForUserTagRobot.CountInMem.ToString() + " users in memory and " + GlobalPool.UserQueueForUserTagRobot.CountInDB.ToString() + " in database.";
        }

        private void UserTagCompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            robotUserTag.Initialize();
            lblUserTagMessage.Text = "Stopped.";
            lblUserTagQueueInfo.Text = "Queue of User Tag Robot: 0 user in memory and 0 user in datebase.";

            if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserRelation == null && oAsyncWorkerStatus == null && oAsyncWorkerComment == null)  //如果另外四个机器人也已停止
            {
                btnStartByCurrent.Text = "Start Crawling by Current User";
                btnStartBySearch.Text = "Start Crawling by Searched User";
                btnStartByLast.Text = "Start Crawling by Last Stopped User";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "Pause/Continue";

                MessageBox.Show(this, "Crawler stopped.", "Sinawler");

                chkUserInfo.Enabled = true;
                chkTag.Enabled = true;
                chkStatus.Enabled = true;
                chkComment.Enabled = true;
                optJSON.Enabled = true;
                optXML.Enabled = true;
            }
            oAsyncWorkerUserTag = null;
        }
        #endregion

        #region 微博机器人
        private void StartCrawStatus(Object sender, DoWorkEventArgs e)
        {
            robotStatus.Start();
        }

        private void StatusProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            lblStatusMessage.Text = robotStatus.LogMessage;
            lblStatusQueueInfo.Text = "Queue of Status Robot: " + GlobalPool.UserQueueForStatusRobot.CountInMem.ToString() + " users in memory and " + GlobalPool.UserQueueForStatusRobot.CountInDB.ToString() + " in database.";
            lblUserBufferInfo.Text = "User Buffer: " + GlobalPool.UserBuffer.CountInMem.ToString() + " users in memory and " + GlobalPool.UserBuffer.CountInDB.ToString() + " in database.";
        }

        private void StatusCompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            robotStatus.Initialize();
            lblStatusMessage.Text = "Stopped.";
            lblStatusQueueInfo.Text = "Queue of Status Robot: 0 user in memory and 0 user in datebase.";

            if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserRelation == null && oAsyncWorkerUserTag == null && oAsyncWorkerComment == null)  //如果另外四个机器人也已停止
            {
                btnStartByCurrent.Text = "Start Crawling by Current User";
                btnStartBySearch.Text = "Start Crawling by Searched User";
                btnStartByLast.Text = "Start Crawling by Last Stopped User";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "Pause/Continue";

                MessageBox.Show(this, "Crawler stopped.", "Sinawler");

                chkUserInfo.Enabled = true;
                chkTag.Enabled = true;
                chkStatus.Enabled = true;
                chkComment.Enabled = true;
                optJSON.Enabled = true;
                optXML.Enabled = true;
            }
            oAsyncWorkerStatus = null;
        }
        #endregion

        #region 微博评论机器人
        private void StartCrawComment(Object sender, DoWorkEventArgs e)
        {
            robotComment.Start();
        }

        private void CommentProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            lblCommentMessage.Text = robotComment.LogMessage;
            lblCommentQueueInfo.Text = "Queue of Comment Robot: " + GlobalPool.StatusQueue.CountInMem.ToString() + " statuses in memory and " + GlobalPool.StatusQueue.CountInDB.ToString() + " in database.";
            lblUserBufferInfo.Text = "User Buffer: " + GlobalPool.UserBuffer.CountInMem.ToString() + " users in memory and " + GlobalPool.UserBuffer.CountInDB.ToString() + " in database.";
        }

        private void CommentCompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            robotComment.Initialize();
            lblCommentMessage.Text = "Stopped.";
            lblCommentQueueInfo.Text = "Queue of Comment Robot: 0 status in memory and 0 status in database.";

            if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserRelation == null && oAsyncWorkerUserTag == null && oAsyncWorkerStatus == null)  //如果另外四个机器人也已停止
            {
                btnStartByCurrent.Text = "Start Crawling by Current User";
                btnStartBySearch.Text = "Start Crawling by Searched User";
                btnStartByLast.Text = "Start Crawling by Last Stopped User";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "Pause/Continue";

                MessageBox.Show(this, "Crawler stopped.", "Sinawler");

                chkUserInfo.Enabled = true;
                chkTag.Enabled = true;
                chkStatus.Enabled = true;
                optJSON.Enabled = true;
                optXML.Enabled = true; chkComment.Enabled = true;
            }
            oAsyncWorkerComment = null;
        }
        #endregion

        private void ShowSettings(SettingItems settings)
        {
            tbQueueLength.Value = settings.MaxLengthInMem;
            numQueueLength.Value = settings.MaxLengthInMem;

            if (settings.DBType == "SQL Server")
                drplstDBType.SelectedIndex = 0;
            else
                drplstDBType.SelectedIndex = 1;

            txtDBServer.Text = settings.DBServer;
            txtDBUserName.Text = settings.DBUserName;
            txtDBPwd.Text = settings.DBPwd;
            txtDBName.Text = settings.DBName;

            GlobalPool.UserInfoRobotEnabled = settings.UserInfoRobot;
            GlobalPool.TagRobotEnabled = settings.TagsRobot;
            GlobalPool.StatusRobotEnabled = settings.StatusesRobot;
            GlobalPool.CommentRobotEnabled = settings.CommentsRobot;

            chkUserInfo.Checked = settings.UserInfoRobot;
            chkTag.Checked = settings.TagsRobot;
            chkStatus.Checked = settings.StatusesRobot;
            chkComment.Checked = settings.CommentsRobot;

            switch(settings.Format)
            {
                case DataFormat.JSON:
                    optJSON.Checked = true;
                    GlobalPool.GetAPI(SysArgFor.USER_RELATION).API.Format = "json";
                    GlobalPool.GetAPI(SysArgFor.USER_INFO).API.Format = "json";
                    GlobalPool.GetAPI(SysArgFor.USER_TAG).API.Format = "json";
                    GlobalPool.GetAPI(SysArgFor.STATUS).API.Format = "json";
                    GlobalPool.GetAPI(SysArgFor.COMMENT).API.Format = "json";
                    break;
                case DataFormat.XML:
                    optXML.Checked = true;
                    GlobalPool.GetAPI(SysArgFor.USER_RELATION).API.Format = "xml";
                    GlobalPool.GetAPI(SysArgFor.USER_INFO).API.Format = "xml";
                    GlobalPool.GetAPI(SysArgFor.USER_TAG).API.Format = "xml";
                    GlobalPool.GetAPI(SysArgFor.STATUS).API.Format = "xml";
                    GlobalPool.GetAPI(SysArgFor.COMMENT).API.Format = "xml";
                    break;
            }
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            SettingItems settings = AppSettings.LoadDefault();
            ShowSettings(settings);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            GlobalPool.UserInfoRobotEnabled = chkUserInfo.Checked;
            GlobalPool.TagRobotEnabled = chkTag.Checked;
            GlobalPool.StatusRobotEnabled = chkStatus.Checked;
            GlobalPool.CommentRobotEnabled = chkComment.Checked;

            SettingItems settings = new SettingItems();
            settings.MaxLengthInMem = tbQueueLength.Value;
            settings.DBType = drplstDBType.SelectedItem.ToString();
            settings.DBServer = txtDBServer.Text.Trim();
            settings.DBUserName = txtDBUserName.Text.Trim();
            settings.DBPwd = txtDBPwd.Text;
            settings.DBName = txtDBName.Text.Trim();

            settings.UserInfoRobot = chkUserInfo.Checked;
            settings.TagsRobot = chkTag.Checked;
            settings.StatusesRobot = chkStatus.Checked;
            settings.CommentsRobot = chkComment.Checked;

            if (optJSON.Checked)
            {
                settings.Format = DataFormat.JSON;
                GlobalPool.GetAPI(SysArgFor.USER_RELATION).API.Format = "json";
                GlobalPool.GetAPI(SysArgFor.USER_INFO).API.Format = "json";
                GlobalPool.GetAPI(SysArgFor.USER_TAG).API.Format = "json";
                GlobalPool.GetAPI(SysArgFor.STATUS).API.Format = "json";
                GlobalPool.GetAPI(SysArgFor.COMMENT).API.Format = "json";
            }
            if (optXML.Checked)
            {
                settings.Format = DataFormat.XML;
                GlobalPool.GetAPI(SysArgFor.USER_RELATION).API.Format = "xml";
                GlobalPool.GetAPI(SysArgFor.USER_INFO).API.Format = "xml";
                GlobalPool.GetAPI(SysArgFor.USER_TAG).API.Format = "xml";
                GlobalPool.GetAPI(SysArgFor.STATUS).API.Format = "xml";
                GlobalPool.GetAPI(SysArgFor.COMMENT).API.Format = "xml";
            }

            AppSettings.Save(settings);

            MessageBox.Show("Settings have been saved and will be used when new crawling starts. ", "Sinawler");
        }

        private void numQueueLength_ValueChanged(object sender, EventArgs e)
        {
            tbQueueLength.Value = Convert.ToInt32(numQueueLength.Value);
        }

        private void tbQueueLength_ValueChanged(object sender, EventArgs e)
        {
            numQueueLength.Value = tbQueueLength.Value;
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            CheckLogin();
            if (blnAuthorized)
            {
                if (oAsyncWorkerUserInfo != null)  //工作中
                    if (PubHelper.PostAdvertisement(GlobalPool.UserQueueForUserRelationRobot.Count, GlobalPool.StatusQueue.Count))
                        MessageBox.Show("A status has been posted to advertise Sinawler.\nThank you very much！", "Sinawler");
                    else
                        MessageBox.Show("Sorry, posting status failed. Please try again, or comment on the homepage of Sinawler.", "Sinawler");
                else
                    if (PubHelper.PostAdvertisement(0, 0))
                        MessageBox.Show("A status has been posted to advertise Sinawler.\nThank you very much！", "Sinawler");
                    else
                        MessageBox.Show("Sorry, posting status failed. Please try again, or comment on the homepage of Sinawler.", "Sinawler");
            }
        }

        private void btnPauseContinue_Click(object sender, EventArgs e)
        {
            if (btnPauseContinue.Text == "PAUSE")
                btnPauseContinue.Text = "CONTINUE";
            else
                btnPauseContinue.Text = "PAUSE";
            robotUserInfo.Suspending = !robotUserInfo.Suspending;
            robotUserRelation.Suspending = !robotUserRelation.Suspending;
            robotUserTag.Suspending = !robotUserTag.Suspending;
            robotStatus.Suspending = !robotStatus.Suspending;
            robotComment.Suspending = !robotComment.Suspending;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanBeClosed()) e.Cancel = true;
        }

        private void chkComment_CheckedChanged(object sender, EventArgs e)
        {
            if (chkComment.Checked) chkStatus.Checked = true;
        }

        private void chkStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkStatus.Checked) chkComment.Checked = false;
        }
    }
}
