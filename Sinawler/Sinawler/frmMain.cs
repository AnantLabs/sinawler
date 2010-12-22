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
        private SinaApiService api = new SinaApiService();

        private User oCurrentUser;     //当前登录用户
        private User oSearchedUser;    //搜索到的用户

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

        private UserQueue queueUserForUserInfoRobot = new UserQueue(QueueBufferFor.USER_INFO);  //用户信息机器人使用的用户队列
        private UserQueue queueUserForUserRelationRobot = new UserQueue(QueueBufferFor.USER_RELATION);  //用户关系机器人使用的用户队列
        private UserQueue queueUserForUserTagRobot = new UserQueue(QueueBufferFor.USER_TAG);  //用户标签机器人使用的用户队列
        private UserQueue queueUserForStatusRobot = new UserQueue(QueueBufferFor.STATUS);  //微博机器人使用的用户队列
        private StatusQueue queueStatus = new StatusQueue();  //微博队列

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
                MessageBox.Show("读取配置文件时发生错误，将加载默认值。", "爬虫小新");
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
                    if (MessageBox.Show("爬虫似乎在工作，您确定要中止它的工作并退出程序吗？", "爬虫小新", MessageBoxButtons.YesNo) == DialogResult.No)
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
                frmLogin login = new frmLogin(api);
                if (login.ShowDialog() == DialogResult.OK)
                {
                    blnAuthorized = true;
                    SinaMBCrawler crawler = new SinaMBCrawler(api);
                    crawler.SleepTime = 0;  //这里不等待
                    oCurrentUser = crawler.GetCurrentUserInfo();
                    ShowCurrentUser();
                    oSearchedUser = oCurrentUser;

                    btnStartByCurrent.Enabled = true;
                    btnStartBySearch.Enabled = true;

                    if (robotUserInfo == null)
                        robotUserInfo = new UserInfoRobot(api, queueUserForUserInfoRobot, queueUserForUserRelationRobot, queueUserForUserTagRobot, queueUserForStatusRobot);
                    else
                        robotUserInfo.SinaAPI = api;

                    if (robotUserRelation == null)
                        robotUserRelation = new UserRelationRobot(api, queueUserForUserInfoRobot, queueUserForUserRelationRobot, queueUserForUserTagRobot, queueUserForStatusRobot);
                    else
                        robotUserRelation.SinaAPI = api;

                    if (robotUserTag == null)
                        robotUserTag = new UserTagRobot(api, queueUserForUserInfoRobot, queueUserForUserRelationRobot, queueUserForUserTagRobot, queueUserForStatusRobot);
                    else
                        robotUserTag.SinaAPI = api;

                    if (robotStatus == null)
                        robotStatus = new StatusRobot(api, queueUserForUserInfoRobot, queueUserForUserRelationRobot, queueUserForUserTagRobot, queueUserForStatusRobot, queueStatus);
                    else
                        robotStatus.SinaAPI = api;

                    if (robotComment == null)
                        robotComment = new CommentRobot(api, queueUserForUserInfoRobot, queueUserForUserRelationRobot, queueUserForUserTagRobot, queueUserForStatusRobot, queueStatus);
                    else
                        robotComment.SinaAPI = api;
                }
                else
                {
                    btnStartByCurrent.Enabled = false;
                    btnStartBySearch.Enabled = false;
                }
                btnSearchOnline.Enabled = true;
                btnSearchOffLine.Enabled = true;
                btnStartByLast.Enabled = true;
            }
        }

        //显示登录帐号用户信息
        private void ShowCurrentUser()
        {
            if (oCurrentUser != null)
            {
                lblCUserID.Text = "用户ID：" + oCurrentUser.user_id.ToString();
                lblCName.Text = "用户昵称：" + oCurrentUser.screen_name;
                if (oCurrentUser.gender == "m")
                    lblCGender.Text = "性别：男";
                else
                    lblCGender.Text = "性别：女";
                if (oCurrentUser.verified)
                    lblCVerified.Text = "是否微博认证用户：是";
                else
                    lblCVerified.Text = "是否微博认证用户：否";
                if (oCurrentUser.following)
                    lblCFollowing.Text = "当前登录帐号是否关注他（她）：是";
                else
                    lblCFollowing.Text = "当前登录帐号是否关注他（她）：否";
                lblCLocation.Text = "所在地：" + oCurrentUser.location;
                lblCFollowersCount.Text = "粉丝人数：" + oCurrentUser.followers_count.ToString();
                lblCFriendsCount.Text = "关注人数：" + oCurrentUser.friends_count.ToString();
                lblCStatusesCount.Text = "已发微博数：" + oCurrentUser.statuses_count.ToString();
                string strCreatedAt = oCurrentUser.created_at;
                string[] strCreatedDate = strCreatedAt.Split(' ')[0].Split('-');
                lblCCreatedAt.Text = "帐号创建时间：" + strCreatedDate[0] + "年" + strCreatedDate[1] + "月" + strCreatedDate[2] + "日";
            }
            else
            {
                lblCUserID.Text = "用户ID：";
                lblCName.Text = "用户昵称：";
                lblCGender.Text = "性别：";
                lblCVerified.Text = "是否微博认证用户：";
                lblCFollowing.Text = "当前登录帐号是否关注他（她）：";
                lblCLocation.Text = "所在地：";
                lblCFollowersCount.Text = "粉丝人数：";
                lblCFriendsCount.Text = "关注人数：";
                lblCStatusesCount.Text = "已发微博数：";
                lblCCreatedAt.Text = "";
            }
        }

        //显示搜索结果用户信息
        private void ShowSearchedUser()
        {
            if (oSearchedUser != null)
            {
                lblUserID.Text = "用户ID：" + oSearchedUser.user_id.ToString();
                lblName.Text = "用户昵称：" + oSearchedUser.screen_name;
                if (oSearchedUser.gender == "m")
                    lblGender.Text = "性别：男";
                else
                    lblGender.Text = "性别：女";
                if (oSearchedUser.verified)
                    lblVerified.Text = "是否微博认证用户：是";
                else
                    lblVerified.Text = "是否微博认证用户：否";
                if (oSearchedUser.following)
                    lblFollowing.Text = "当前登录帐号是否关注他（她）：是";
                else
                    lblFollowing.Text = "当前登录帐号是否关注他（她）：否";
                lblLocation.Text = "所在地：" + oSearchedUser.location;
                lblFollowersCount.Text = "粉丝人数：" + oSearchedUser.followers_count.ToString();
                lblFriendsCount.Text = "关注人数：" + oSearchedUser.friends_count.ToString();
                lblStatusesCount.Text = "已发微博数：" + oSearchedUser.statuses_count.ToString();
                string strCreatedAt = oSearchedUser.created_at;
                string[] strCreatedDate = strCreatedAt.Split(' ')[0].Split('-');
                lblCreatedAt.Text = "帐号创建时间：" + strCreatedDate[0] + "年" + strCreatedDate[1] + "月" + strCreatedDate[2] + "日";
            }
            else
            {
                lblUserID.Text = "用户ID：";
                lblName.Text = "用户昵称：";
                lblGender.Text = "性别：";
                lblVerified.Text = "是否微博认证用户：";
                lblFollowing.Text = "当前登录帐号是否关注他（她）：";
                lblLocation.Text = "所在地：";
                lblFollowersCount.Text = "粉丝人数：";
                lblFriendsCount.Text = "关注人数：";
                lblStatusesCount.Text = "已发微博数：";
                lblCreatedAt.Text = "帐号创建时间：";
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
                    MessageBox.Show("请至少输入“用户ID”和“用户昵称”之一。", "爬虫小新");
                    txtUserID.Focus();
                    return;
                }
                string strUserID = txtUserID.Text.Trim();
                string strScreenName = txtUserName.Text.Trim();
                long lBuffer;
                if (strUserID != "" && !long.TryParse(strUserID, out lBuffer))
                {
                    MessageBox.Show("请输入正确的用户ID。", "爬虫小新");
                    return;
                }
                SinaMBCrawler crawler = new SinaMBCrawler(api);
                crawler.SleepTime = 0;  //这里不等待
                if (strUserID != "" && strScreenName == "")
                    oSearchedUser = crawler.GetUserInfo(Convert.ToInt64(strUserID));
                if (strUserID == "" && strScreenName != "")
                    oSearchedUser = crawler.GetUserInfo(strScreenName);
                if (strUserID != "" && strScreenName != "")
                    oSearchedUser = crawler.GetUserInfo(Convert.ToInt64(strUserID), strScreenName);
                if (oSearchedUser == null) MessageBox.Show("未搜索到指定用户。", "爬虫小新");
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
                MessageBox.Show("请至少输入“用户ID”和“用户昵称”之一。", "爬虫小新");
                txtUserID.Focus();
                return;
            }
            strDataBaseStatus = PubHelper.TestDataBase();
            if (strDataBaseStatus != "OK")
            {
                MessageBox.Show("数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "爬虫小新");
                return;
            }
            string strUserID = txtUserID.Text.Trim();
            string strScreenName = txtUserName.Text.Trim();
            long lBuffer;
            if (strUserID != "" && !long.TryParse(strUserID, out lBuffer))
            {
                MessageBox.Show("请输入正确的用户ID。", "爬虫小新");
                return;
            }
            if (strUserID != "" && strScreenName == "")
            {
                if (!oSearchedUser.GetModel(Convert.ToInt64(strUserID)))
                {
                    MessageBox.Show("未搜索到指定用户。", "爬虫小新");
                }
                else
                    ShowSearchedUser();
            }
            if (strUserID == "" && strScreenName != "")
            {
                if (!oSearchedUser.GetModel( strScreenName ))
                {
                    MessageBox.Show( "未搜索到指定用户。", "爬虫小新" );
                }
                else
                    ShowSearchedUser();
            }
            if (strUserID != "" && strScreenName != "")
            {
                if (!oSearchedUser.GetModel( Convert.ToInt64( strUserID ), strScreenName ))
                {
                    MessageBox.Show( "未搜索到指定用户。", "爬虫小新" );
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

            robotUserInfo.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userInfo.log";
            robotUserRelation.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userRelation.log";
            robotUserTag.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userTag.log";
            robotStatus.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
            robotComment.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_comment.log";

            SettingItems settings = AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            queueUserForUserInfoRobot.MaxLengthInMem = settings.MaxLengthInMem;
            queueUserForUserRelationRobot.MaxLengthInMem = settings.MaxLengthInMem;
            queueUserForUserTagRobot.MaxLengthInMem = settings.MaxLengthInMem;
            queueUserForStatusRobot.MaxLengthInMem = settings.MaxLengthInMem;
            queueStatus.MaxLengthInMem = settings.MaxLengthInMem;
        }

        private void btnStartByCurrent_Click(object sender, EventArgs e)
        {
            CheckLogin();
            if (blnAuthorized)
            {
                strDataBaseStatus = PubHelper.TestDataBase();
                if (strDataBaseStatus != "OK")
                {
                    MessageBox.Show("数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "爬虫小新");
                    return;
                }

                if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserRelation == null && oAsyncWorkerUserTag == null && oAsyncWorkerStatus == null && oAsyncWorkerComment == null)
                {
                    btnStartByCurrent.Text = "正在初始化，请稍候...";
                    btnStartByCurrent.Enabled = false;
                    btnStartBySearch.Enabled = false;
                    btnStartByLast.Enabled = false;
                    Application.DoEvents();
                    PrepareToStart();
                    oAsyncWorkerUserInfo = new BackgroundWorker();
                    oAsyncWorkerUserInfo.WorkerReportsProgress = true;
                    oAsyncWorkerUserInfo.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserInfo.ProgressChanged += new ProgressChangedEventHandler(UserInfoProgressChanged);
                    oAsyncWorkerUserInfo.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserInfoCompleteWork);
                    oAsyncWorkerUserInfo.DoWork += new DoWorkEventHandler(StartCrawUserInfoByCurrentUser);
                    robotUserInfo.AsyncWorker = oAsyncWorkerUserInfo;

                    oAsyncWorkerUserRelation = new BackgroundWorker();
                    oAsyncWorkerUserRelation.WorkerReportsProgress = true;
                    oAsyncWorkerUserRelation.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserRelation.ProgressChanged += new ProgressChangedEventHandler(UserRelationProgressChanged);
                    oAsyncWorkerUserRelation.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserRelationCompleteWork);
                    oAsyncWorkerUserRelation.DoWork += new DoWorkEventHandler(StartCrawUserRelationByCurrentUser);
                    robotUserRelation.AsyncWorker = oAsyncWorkerUserRelation;

                    oAsyncWorkerUserTag = new BackgroundWorker();
                    oAsyncWorkerUserTag.WorkerReportsProgress = true;
                    oAsyncWorkerUserTag.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserTag.ProgressChanged += new ProgressChangedEventHandler(UserTagProgressChanged);
                    oAsyncWorkerUserTag.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserTagCompleteWork);
                    oAsyncWorkerUserTag.DoWork += new DoWorkEventHandler(StartCrawUserTagByCurrentUser);
                    robotUserTag.AsyncWorker = oAsyncWorkerUserTag;

                    oAsyncWorkerStatus = new BackgroundWorker();
                    oAsyncWorkerStatus.WorkerReportsProgress = true;
                    oAsyncWorkerStatus.WorkerSupportsCancellation = true;
                    oAsyncWorkerStatus.ProgressChanged += new ProgressChangedEventHandler(StatusProgressChanged);
                    oAsyncWorkerStatus.RunWorkerCompleted += new RunWorkerCompletedEventHandler(StatusCompleteWork);
                    oAsyncWorkerStatus.DoWork += new DoWorkEventHandler(StartCrawStatus);
                    robotStatus.AsyncWorker = oAsyncWorkerStatus;

                    oAsyncWorkerComment = new BackgroundWorker();
                    oAsyncWorkerComment.WorkerReportsProgress = true;
                    oAsyncWorkerComment.WorkerSupportsCancellation = true;
                    oAsyncWorkerComment.ProgressChanged += new ProgressChangedEventHandler(CommentProgressChanged);
                    oAsyncWorkerComment.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CommentCompleteWork);
                    oAsyncWorkerComment.DoWork += new DoWorkEventHandler(StartCrawComment);
                    robotComment.AsyncWorker = oAsyncWorkerComment;
                }
                if (oAsyncWorkerUserInfo.IsBusy || oAsyncWorkerUserRelation.IsBusy || oAsyncWorkerUserTag.IsBusy || oAsyncWorkerStatus.IsBusy || oAsyncWorkerComment.IsBusy)
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
                    if (MessageBox.Show("您确定要中止爬虫吗？", "爬虫小新", MessageBoxButtons.YesNo) == DialogResult.No)
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
                    btnStartByCurrent.Text = "正在停止，请稍候...";
                    btnPauseContinue.Enabled = false;
                    robotUserInfo.AsyncCancelled = true;
                    robotUserRelation.AsyncCancelled = true;
                    robotUserTag.AsyncCancelled = true;
                    robotStatus.AsyncCancelled = true;
                    robotComment.AsyncCancelled = true;
                    oAsyncWorkerUserInfo.CancelAsync();
                    oAsyncWorkerUserRelation.CancelAsync();
                    oAsyncWorkerUserTag.CancelAsync();
                    oAsyncWorkerStatus.CancelAsync();
                    oAsyncWorkerComment.CancelAsync();
                }
                else
                {
                    btnStartByCurrent.Text = "停止爬行";
                    btnStartByCurrent.Enabled = true;
                    btnStartBySearch.Enabled = false;
                    btnStartByLast.Enabled = false;
                    btnPauseContinue.Text = "暂停";
                    btnPauseContinue.Enabled = true;
                    oAsyncWorkerUserInfo.RunWorkerAsync();
                    oAsyncWorkerUserRelation.RunWorkerAsync();
                    oAsyncWorkerUserTag.RunWorkerAsync();
                    oAsyncWorkerStatus.RunWorkerAsync();
                    oAsyncWorkerComment.RunWorkerAsync();
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
                    MessageBox.Show(this, "无搜索结果用户，请先搜索用户。", "爬虫小新");
                    return;
                }
                strDataBaseStatus = PubHelper.TestDataBase();
                if (strDataBaseStatus != "OK")
                {
                    MessageBox.Show("数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "爬虫小新");
                    return;
                }

                if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserRelation == null && oAsyncWorkerUserTag == null && oAsyncWorkerStatus == null && oAsyncWorkerComment == null)
                {
                    btnStartBySearch.Text = "正在初始化，请稍候...";
                    btnStartBySearch.Enabled = false;
                    btnStartByCurrent.Enabled = false;
                    btnStartByLast.Enabled = false;
                    Application.DoEvents();
                    PrepareToStart();
                    oAsyncWorkerUserInfo = new BackgroundWorker();
                    oAsyncWorkerUserInfo.WorkerReportsProgress = true;
                    oAsyncWorkerUserInfo.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserInfo.ProgressChanged += new ProgressChangedEventHandler(UserInfoProgressChanged);
                    oAsyncWorkerUserInfo.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserInfoCompleteWork);
                    oAsyncWorkerUserInfo.DoWork += new DoWorkEventHandler(StartCrawUserInfoBySearchedUser);
                    robotUserInfo.AsyncWorker = oAsyncWorkerUserInfo;

                    oAsyncWorkerUserRelation = new BackgroundWorker();
                    oAsyncWorkerUserRelation.WorkerReportsProgress = true;
                    oAsyncWorkerUserRelation.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserRelation.ProgressChanged += new ProgressChangedEventHandler(UserRelationProgressChanged);
                    oAsyncWorkerUserRelation.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserRelationCompleteWork);
                    oAsyncWorkerUserRelation.DoWork += new DoWorkEventHandler(StartCrawUserRelationBySearchedUser);
                    robotUserRelation.AsyncWorker = oAsyncWorkerUserRelation;

                    oAsyncWorkerUserTag = new BackgroundWorker();
                    oAsyncWorkerUserTag.WorkerReportsProgress = true;
                    oAsyncWorkerUserTag.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserTag.ProgressChanged += new ProgressChangedEventHandler(UserTagProgressChanged);
                    oAsyncWorkerUserTag.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserTagCompleteWork);
                    oAsyncWorkerUserTag.DoWork += new DoWorkEventHandler(StartCrawUserTagBySearchedUser);
                    robotUserTag.AsyncWorker = oAsyncWorkerUserTag;

                    oAsyncWorkerStatus = new BackgroundWorker();
                    oAsyncWorkerStatus.WorkerReportsProgress = true;
                    oAsyncWorkerStatus.WorkerSupportsCancellation = true;
                    oAsyncWorkerStatus.ProgressChanged += new ProgressChangedEventHandler(StatusProgressChanged);
                    oAsyncWorkerStatus.RunWorkerCompleted += new RunWorkerCompletedEventHandler(StatusCompleteWork);
                    oAsyncWorkerStatus.DoWork += new DoWorkEventHandler(StartCrawStatus);
                    robotStatus.AsyncWorker = oAsyncWorkerStatus;

                    oAsyncWorkerComment = new BackgroundWorker();
                    oAsyncWorkerComment.WorkerReportsProgress = true;
                    oAsyncWorkerComment.WorkerSupportsCancellation = true;
                    oAsyncWorkerComment.ProgressChanged += new ProgressChangedEventHandler(CommentProgressChanged);
                    oAsyncWorkerComment.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CommentCompleteWork);
                    oAsyncWorkerComment.DoWork += new DoWorkEventHandler(StartCrawComment);
                    robotComment.AsyncWorker = oAsyncWorkerComment;
                }
                if (oAsyncWorkerUserInfo.IsBusy || oAsyncWorkerUserRelation.IsBusy || oAsyncWorkerUserTag.IsBusy || oAsyncWorkerStatus.IsBusy || oAsyncWorkerComment.IsBusy)
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
                    if (MessageBox.Show("您确定要中止爬虫吗？", "爬虫小新", MessageBoxButtons.YesNo) == DialogResult.No)
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
                    btnStartBySearch.Text = "正在停止，请稍候...";
                    btnPauseContinue.Enabled = false;
                    robotUserInfo.AsyncCancelled = true;
                    robotUserRelation.AsyncCancelled = true;
                    robotUserTag.AsyncCancelled = true;
                    robotStatus.AsyncCancelled = true;
                    robotComment.AsyncCancelled = true;
                    oAsyncWorkerUserInfo.CancelAsync();
                    oAsyncWorkerUserRelation.CancelAsync();
                    oAsyncWorkerUserTag.CancelAsync();
                    oAsyncWorkerStatus.CancelAsync();
                    oAsyncWorkerComment.CancelAsync();
                }
                else
                {
                    btnStartBySearch.Text = "停止爬行";
                    btnStartBySearch.Enabled = true;
                    btnStartByCurrent.Enabled = false;
                    btnStartByLast.Enabled = false;
                    btnPauseContinue.Text = "暂停";
                    btnPauseContinue.Enabled = true;
                    oAsyncWorkerUserInfo.RunWorkerAsync();
                    oAsyncWorkerUserRelation.RunWorkerAsync();
                    oAsyncWorkerUserTag.RunWorkerAsync();
                    oAsyncWorkerStatus.RunWorkerAsync();
                    oAsyncWorkerComment.RunWorkerAsync();
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
                    MessageBox.Show("数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "爬虫小新");
                    return;
                }
                if (SysArg.GetCurrentUserIDForUserInfo() == 0 && SysArg.GetCurrentUserIDForUserRelation() == 0)
                {
                    MessageBox.Show(this, "无上次中止用户的记录，请选择其它爬行起点。", "爬虫小新");
                    return;
                }

                if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserRelation == null && oAsyncWorkerUserTag == null && oAsyncWorkerStatus == null && oAsyncWorkerComment == null)
                {
                    btnStartByLast.Text = "正在初始化，请稍候...";
                    btnStartByLast.Enabled = false;
                    btnStartBySearch.Enabled = false;
                    btnStartByCurrent.Enabled = false;
                    Application.DoEvents();
                    PrepareToStart();
                    oAsyncWorkerUserInfo = new BackgroundWorker();
                    oAsyncWorkerUserInfo.WorkerReportsProgress = true;
                    oAsyncWorkerUserInfo.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserInfo.ProgressChanged += new ProgressChangedEventHandler(UserInfoProgressChanged);
                    oAsyncWorkerUserInfo.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserInfoCompleteWork);
                    oAsyncWorkerUserInfo.DoWork += new DoWorkEventHandler(StartCrawUserInfoByLastUser);
                    robotUserInfo.AsyncWorker = oAsyncWorkerUserInfo;

                    oAsyncWorkerUserRelation = new BackgroundWorker();
                    oAsyncWorkerUserRelation.WorkerReportsProgress = true;
                    oAsyncWorkerUserRelation.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserRelation.ProgressChanged += new ProgressChangedEventHandler(UserRelationProgressChanged);
                    oAsyncWorkerUserRelation.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserRelationCompleteWork);
                    oAsyncWorkerUserRelation.DoWork += new DoWorkEventHandler(StartCrawUserRelationByLastUser);
                    robotUserRelation.AsyncWorker = oAsyncWorkerUserRelation;

                    oAsyncWorkerUserTag = new BackgroundWorker();
                    oAsyncWorkerUserTag.WorkerReportsProgress = true;
                    oAsyncWorkerUserTag.WorkerSupportsCancellation = true;
                    oAsyncWorkerUserTag.ProgressChanged += new ProgressChangedEventHandler(UserTagProgressChanged);
                    oAsyncWorkerUserTag.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UserTagCompleteWork);
                    oAsyncWorkerUserTag.DoWork += new DoWorkEventHandler(StartCrawUserTagByLastUser);
                    robotUserTag.AsyncWorker = oAsyncWorkerUserTag;

                    oAsyncWorkerStatus = new BackgroundWorker();
                    oAsyncWorkerStatus.WorkerReportsProgress = true;
                    oAsyncWorkerStatus.WorkerSupportsCancellation = true;
                    oAsyncWorkerStatus.ProgressChanged += new ProgressChangedEventHandler(StatusProgressChanged);
                    oAsyncWorkerStatus.RunWorkerCompleted += new RunWorkerCompletedEventHandler(StatusCompleteWork);
                    oAsyncWorkerStatus.DoWork += new DoWorkEventHandler(StartCrawStatus);
                    robotStatus.AsyncWorker = oAsyncWorkerStatus;

                    oAsyncWorkerComment = new BackgroundWorker();
                    oAsyncWorkerComment.WorkerReportsProgress = true;
                    oAsyncWorkerComment.WorkerSupportsCancellation = true;
                    oAsyncWorkerComment.ProgressChanged += new ProgressChangedEventHandler(CommentProgressChanged);
                    oAsyncWorkerComment.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CommentCompleteWork);
                    oAsyncWorkerComment.DoWork += new DoWorkEventHandler(StartCrawComment);
                    robotComment.AsyncWorker = oAsyncWorkerComment;
                }
                if (oAsyncWorkerUserInfo.IsBusy || oAsyncWorkerUserRelation.IsBusy || oAsyncWorkerUserTag.IsBusy || oAsyncWorkerStatus.IsBusy || oAsyncWorkerComment.IsBusy)
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
                    if (MessageBox.Show("您确定要中止爬虫吗？", "爬虫小新", MessageBoxButtons.YesNo) == DialogResult.No)
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
                    btnStartByLast.Text = "正在停止，请稍候...";
                    btnPauseContinue.Enabled = false;
                    robotUserInfo.AsyncCancelled = true;
                    robotUserRelation.AsyncCancelled = true;
                    robotUserTag.AsyncCancelled = true;
                    robotStatus.AsyncCancelled = true;
                    robotComment.AsyncCancelled = true;
                    oAsyncWorkerUserInfo.CancelAsync();
                    oAsyncWorkerUserRelation.CancelAsync();
                    oAsyncWorkerUserTag.CancelAsync();
                    oAsyncWorkerStatus.CancelAsync();
                    oAsyncWorkerComment.CancelAsync();
                }
                else
                {
                    btnStartByLast.Text = "停止爬行";
                    btnStartByLast.Enabled = true;
                    btnStartBySearch.Enabled = false;
                    btnStartByCurrent.Enabled = false;
                    btnPauseContinue.Text = "暂停";
                    btnPauseContinue.Enabled = true;
                    oAsyncWorkerUserInfo.RunWorkerAsync();
                    oAsyncWorkerUserRelation.RunWorkerAsync();
                    oAsyncWorkerUserTag.RunWorkerAsync();
                    oAsyncWorkerStatus.RunWorkerAsync();
                    oAsyncWorkerComment.RunWorkerAsync();
                }
            }
        }

        #region 用户信息机器人
        private void StartCrawUserInfoByCurrentUser(Object sender, DoWorkEventArgs e)
        {
            robotUserInfo.Start(oCurrentUser.user_id);
        }

        private void StartCrawUserInfoBySearchedUser(Object sender, DoWorkEventArgs e)
        {
            robotUserInfo.Start(oSearchedUser.user_id);
        }

        private void StartCrawUserInfoByLastUser(Object sender, DoWorkEventArgs e)
        {
            long lLastUserID = SysArg.GetCurrentUserIDForUserInfo();
            if (lLastUserID == 0)
            {
                lLastUserID = SysArg.GetCurrentUserIDForUserRelation();
                if (lLastUserID == 0)
                {
                    lLastUserID = SysArg.GetCurrentUserIDForUserTag();
                    if (lLastUserID == 0)
                    {
                        MessageBox.Show(this, "未找到上次中止的用户，请选择其它起点。", "爬虫小新");
                        return;
                    }
                }
            }
            robotUserInfo.Start(lLastUserID);
        }

        private void UserInfoProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            lblUserInfoMessage.Text = robotUserInfo.LogMessage;
            lblUserInfoQueueInfo.Text = "用户信息机器人的内存队列中有" + queueUserForUserInfoRobot.CountInMem.ToString() + "个用户，数据库队列中有" + queueUserForUserInfoRobot.CountInDB.ToString() + "个用户。";
        }

        private void UserInfoCompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            robotUserInfo.Initialize();
            lblUserInfoMessage.Text = "停止。";
            lblUserInfoQueueInfo.Text = "用户信息机器人的内存队列中有0个用户，数据库队列中有0个用户。";

            if (oAsyncWorkerUserRelation == null && oAsyncWorkerUserTag == null && oAsyncWorkerStatus == null && oAsyncWorkerComment == null)  //如果另外四个机器人也已停止
            {
                btnStartByCurrent.Text = "以当前登录帐号为起点开始爬行";
                btnStartBySearch.Text = "以搜索结果用户为起点开始爬行";
                btnStartByLast.Text = "以上次中止的用户为起点开始爬行";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "暂停/继续";

                MessageBox.Show(this, "爬虫已停止。", "爬虫小新");
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
            long lLastUserID = SysArg.GetCurrentUserIDForUserRelation();
            if (lLastUserID == 0)
            {
                lLastUserID = SysArg.GetCurrentUserIDForUserInfo();
                if (lLastUserID == 0)
                {
                    lLastUserID = SysArg.GetCurrentUserIDForUserTag();
                    if (lLastUserID == 0)
                    {
                        MessageBox.Show(this, "未找到上次中止的用户，请选择其它起点。", "爬虫小新");
                        return;
                    }
                }
            }
            robotUserRelation.Start(lLastUserID);
        }

        private void UserRelationProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            lblUserRelationMessage.Text = robotUserRelation.LogMessage;
            lblUserRelationQueueInfo.Text = "用户关系机器人的内存队列中有" + queueUserForUserRelationRobot.CountInMem.ToString() + "个用户，数据库队列中有" + queueUserForUserRelationRobot.CountInDB.ToString() + "个用户。";
        }

        private void UserRelationCompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            robotUserRelation.Initialize();
            lblUserRelationMessage.Text = "停止。";
            lblUserRelationQueueInfo.Text = "用户关系机器人的内存队列中有0个用户，数据库队列中有0个用户。";

            if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserTag == null && oAsyncWorkerStatus == null && oAsyncWorkerComment == null)  //如果另外四个机器人也已停止
            {
                btnStartByCurrent.Text = "以当前登录帐号为起点开始爬行";
                btnStartBySearch.Text = "以搜索结果用户为起点开始爬行";
                btnStartByLast.Text = "以上次中止的用户为起点开始爬行";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "暂停/继续";

                MessageBox.Show(this, "爬虫已停止。", "爬虫小新");
            }
            oAsyncWorkerUserRelation = null;
        }
        #endregion

        #region 用户标签机器人
        private void StartCrawUserTagByCurrentUser(Object sender, DoWorkEventArgs e)
        {
            robotUserTag.Start(oCurrentUser.user_id);
        }

        private void StartCrawUserTagBySearchedUser(Object sender, DoWorkEventArgs e)
        {
            robotUserTag.Start(oSearchedUser.user_id);
        }

        private void StartCrawUserTagByLastUser(Object sender, DoWorkEventArgs e)
        {
            long lLastUserID = SysArg.GetCurrentUserIDForUserTag();
            if (lLastUserID == 0)
            {
                lLastUserID = SysArg.GetCurrentUserIDForUserInfo();
                if (lLastUserID == 0)
                {
                    lLastUserID = SysArg.GetCurrentUserIDForUserRelation();
                    if (lLastUserID == 0)
                    {
                        MessageBox.Show(this, "未找到上次中止的用户，请选择其它起点。", "爬虫小新");
                        return;
                    }
                }
            }
            robotUserTag.Start(lLastUserID);
        }

        private void UserTagProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            lblUserTagMessage.Text = robotUserTag.LogMessage;
            lblUserTagQueueInfo.Text = "用户标签机器人的内存队列中有" + queueUserForUserTagRobot.CountInMem.ToString() + "个用户，数据库队列中有" + queueUserForUserTagRobot.CountInDB.ToString() + "个用户。";
        }

        private void UserTagCompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            robotUserTag.Initialize();
            lblUserTagMessage.Text = "停止。";
            lblUserTagQueueInfo.Text = "用户标签机器人的内存队列中有0个用户，数据库队列中有0个用户。";

            if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserRelation == null && oAsyncWorkerStatus == null && oAsyncWorkerComment == null)  //如果另外四个机器人也已停止
            {
                btnStartByCurrent.Text = "以当前登录帐号为起点开始爬行";
                btnStartBySearch.Text = "以搜索结果用户为起点开始爬行";
                btnStartByLast.Text = "以上次中止的用户为起点开始爬行";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "暂停/继续";

                MessageBox.Show(this, "爬虫已停止。", "爬虫小新");
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
            lblStatusQueueInfo.Text = "微博机器人的内存队列中有" + queueUserForStatusRobot.CountInMem.ToString() + "个用户，数据库队列中有" + queueUserForStatusRobot.CountInDB.ToString() + "个用户。";
        }

        private void StatusCompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            robotStatus.Initialize();
            lblStatusMessage.Text = "停止。";
            lblStatusQueueInfo.Text = "微博机器人的内存队列中有0个用户，数据库队列中有0个用户。";

            if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserRelation == null && oAsyncWorkerUserTag == null && oAsyncWorkerComment == null)  //如果另外四个机器人也已停止
            {
                btnStartByCurrent.Text = "以当前登录帐号为起点开始爬行";
                btnStartBySearch.Text = "以搜索结果用户为起点开始爬行";
                btnStartByLast.Text = "以上次中止的用户为起点开始爬行";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "暂停/继续";

                MessageBox.Show(this, "爬虫已停止。", "爬虫小新");
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
            lblCommentQueueInfo.Text = "评论机器人的内存队列中有" + queueStatus.CountInMem.ToString() + "条微博，数据库队列中有" + queueStatus.CountInDB.ToString() + "条微博。";
        }

        private void CommentCompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            robotComment.Initialize();
            lblCommentMessage.Text = "停止。";
            lblCommentQueueInfo.Text = "微博机器人的内存队列中有0个用户，数据库队列中有0个用户。";

            if (oAsyncWorkerUserInfo == null && oAsyncWorkerUserRelation == null && oAsyncWorkerUserTag == null && oAsyncWorkerStatus == null)  //如果另外四个机器人也已停止
            {
                btnStartByCurrent.Text = "以当前登录帐号为起点开始爬行";
                btnStartBySearch.Text = "以搜索结果用户为起点开始爬行";
                btnStartByLast.Text = "以上次中止的用户为起点开始爬行";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "暂停/继续";

                MessageBox.Show(this, "爬虫已停止。", "爬虫小新");
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
            SettingItems settings = new SettingItems();
            settings.MaxLengthInMem = tbQueueLength.Value;
            settings.DBType = drplstDBType.SelectedItem.ToString();
            settings.DBServer = txtDBServer.Text.Trim();
            settings.DBUserName = txtDBUserName.Text.Trim();
            settings.DBPwd = txtDBPwd.Text;
            settings.DBName = txtDBName.Text.Trim();

            AppSettings.Save(settings);

            MessageBox.Show("设置已保存。启动新的爬虫任务时将使用新的设置。", "爬虫小新");
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
                    if (PubHelper.PostAdvertisement(api, queueUserForUserInfoRobot.Count, queueStatus.Count))
                        MessageBox.Show("您已经帮忙发布了一条推广此应用的微博。\n感谢您对本应用的支持！", "爬虫小新");
                    else
                        MessageBox.Show("对不起，发布推广微博失败，请重试，或到应用主页提出您的宝贵意见。", "爬虫小新");
                else
                    if (PubHelper.PostAdvertisement(api, 0, 0))
                        MessageBox.Show("您已经帮忙发布了一条推广此应用的微博。\n感谢您对本应用的支持！", "爬虫小新");
                    else
                        MessageBox.Show("对不起，发布推广微博失败，请重试，或到应用主页提出您的宝贵意见。", "爬虫小新");
            }
        }

        private void btnPauseContinue_Click(object sender, EventArgs e)
        {
            if (btnPauseContinue.Text == "暂停")
                btnPauseContinue.Text = "继续";
            else
                btnPauseContinue.Text = "暂停";
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
    }
}
