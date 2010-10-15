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

        private User oCurrentUser = new User();     //当前登录用户
        private User oSearchedUser = new User();    //搜索到的用户

        //分别用于两个机器人的线程
        private BackgroundWorker oAsyncWorkerUser = null;
        private BackgroundWorker oAsyncWorkerStatus = null;

        //爬虫机器人，一个是爬取用户信息的，另一个是爬取微博数据的，分别在两个线程中运行
        private UserRobot robotUser;
        private StatusRobot robotStatus;

        private string strDataBaseStatus = "";      //数据库测试状态结果，OK为正常

        private LinkedList<long> lstQueueUserToStatus = new LinkedList<long>(); //UserRobt抛给StatusRobot的UID队列
        private LinkedList<long> lstQueueStatusToUser = new LinkedList<long>(); //StatusRobot抛给UserRobt的UID队列

        public frmMain ()
        {
            InitializeComponent();
        }

        //加载设置
        private void LoadSettings ()
        {
            SettingItems settings = AppSettings.Load();
            if (settings == null)
            {
                MessageBox.Show( "读取配置文件时发生错误，将加载默认值。", "新浪微博爬虫" );
                settings = AppSettings.LoadDefault();
            }
            ShowSettings( settings );
        }

        //关闭窗口时调用
        private bool CanBeClosed()
        {
            if (robotUser != null && robotStatus != null)
            {
                if (!btnStartByCurrent.Enabled || !btnStartBySearch.Enabled || !btnStartByLast.Enabled)
                {
                    robotUser.Suspending = true;    //先暂停
                    robotStatus.Suspending = true;    //先暂停
                    if (MessageBox.Show( "爬虫似乎在工作，您确定要中止它的工作并退出程序吗？", "新浪微博爬虫", MessageBoxButtons.YesNo ) == DialogResult.No)
                    {
                        robotUser.Suspending = false;
                        robotStatus.Suspending = false;
                        return false;
                    }
                }
            }
            return true;
        }

        //检查登录状态。若未登录，弹出登录框
        private void CheckLogin ()
        {
            if (!blnAuthorized)
            {
                frmLogin login = new frmLogin( api );
                if (login.ShowDialog() == DialogResult.OK)
                {
                    blnAuthorized = true;
                }
            }
        }

        //显示登录帐号用户信息
        private void ShowCurrentUser ()
        {
            if (oCurrentUser != null)
            {
                lblCUID.Text = "用户ID：" + oCurrentUser.uid.ToString();
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
                string[] strCreatedDate = strCreatedAt.Split( ' ' )[0].Split( '-' );
                lblCCreatedAt.Text = "帐号创建时间：" + strCreatedDate[0] + "年" + strCreatedDate[1] + "月" + strCreatedDate[2] + "日";
            }
            else
            {
                lblCUID.Text = "用户ID：";
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
        private void ShowSearchedUser ()
        {
            if (oSearchedUser != null)
            {
                lblUID.Text = "用户ID：" + oSearchedUser.uid.ToString();
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
                string[] strCreatedDate = strCreatedAt.Split( ' ' )[0].Split( '-' );
                lblCreatedAt.Text = "帐号创建时间：" + strCreatedDate[0] + "年" + strCreatedDate[1] + "月" + strCreatedDate[2] + "日";
            }
            else
            {
                lblUID.Text = "用户ID：";
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

        private void frmMain_Load ( object sender, EventArgs e )
        {
            CheckLogin();
            drplstDBType.SelectedIndex = 0;
            if (blnAuthorized)
            {
                SinaMBCrawler crawler = new SinaMBCrawler( api );
                crawler.SleepTime = 0;  //这里不等待
                oCurrentUser = crawler.GetCurrentUserInfo();
                oSearchedUser = oCurrentUser;
                ShowCurrentUser();
                ShowSearchedUser();
                txtUID.Text = oSearchedUser.uid.ToString();
                txtUserName.Text = oSearchedUser.screen_name;

                robotUser = new UserRobot( api );   //用户爬虫机器人
                robotStatus = new StatusRobot( api );   //微博爬虫机器人
            }
            else
            {
                btnSearchOnline.Enabled = true;
                btnSearchOffLine.Enabled = true;
                btnStartByCurrent.Enabled = false;
                btnStartBySearch.Enabled = false;
                btnStartByLast.Enabled = true;
                btnExit.Enabled = true;
            }
            //通过点击按钮，加载设置
            LoadSettings();
        }

        private void btnExit_Click ( object sender, EventArgs e )
        {
            if (CanBeClosed())  Application.Exit();
        }

        private void btnSearchOnline_Click ( object sender, EventArgs e )
        {
            CheckLogin();
            if (blnAuthorized)
            {
                if (txtUID.Text.Trim() == "" && txtUserName.Text.Trim() == "")
                {
                    MessageBox.Show( "请至少输入“用户ID”和“用户昵称”之一。", "新浪微博爬虫" );
                    txtUID.Focus();
                    return;
                }
                string strUID = txtUID.Text.Trim();
                string strScreenName = txtUserName.Text.Trim();
                long lBuffer;
                if (strUID != "" && !long.TryParse( strUID, out lBuffer ))
                {
                    MessageBox.Show( "请输入正确的用户ID。", "新浪微博爬虫" );
                    return;
                }
                SinaMBCrawler crawler = new SinaMBCrawler( api );
                crawler.SleepTime = 0;  //这里不等待
                if (strUID != "" && strScreenName == "")
                    oSearchedUser = crawler.GetUserInfo( Convert.ToInt64( strUID ) );
                if (strUID == "" && strScreenName != "")
                    oSearchedUser = crawler.GetUserInfo( strScreenName );
                if (strUID != "" && strScreenName != "")
                    oSearchedUser = crawler.GetUserInfo( Convert.ToInt64( strUID ), strScreenName );
                if (oSearchedUser == null) MessageBox.Show( "未搜索到指定用户。", "新浪微博爬虫" );
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

        private void btnSearchOffLine_Click ( object sender, EventArgs e )
        {
            if (txtUID.Text.Trim() == "" && txtUserName.Text.Trim() == "")
            {
                MessageBox.Show( "请至少输入“用户ID”和“用户昵称”之一。", "新浪微博爬虫" );
                txtUID.Focus();
                return;
            }
            strDataBaseStatus = PubHelper.TestDataBase();
            if (strDataBaseStatus != "OK")
            {
                MessageBox.Show( "数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "新浪微博爬虫" );
                return;
            }
            string strUID = txtUID.Text.Trim();
            string strScreenName = txtUserName.Text.Trim();
            long lBuffer;
            if (strUID != "" && !long.TryParse( strUID, out lBuffer ))
            {
                MessageBox.Show( "请输入正确的用户ID。", "新浪微博爬虫" );
                return;
            }
            if (strUID != "" && strScreenName == "")
                if (!oSearchedUser.GetModel( Convert.ToInt64( strUID ) ))
                {
                    MessageBox.Show( "未搜索到指定用户。", "新浪微博爬虫" );
                    oSearchedUser = null;
                }
            if (strUID == "" && strScreenName != "")
                if (!oSearchedUser.GetModel( strScreenName ))
                {
                    MessageBox.Show( "未搜索到指定用户。", "新浪微博爬虫" );
                    oSearchedUser = null;
                }
            if (strUID != "" && strScreenName != "")
                if (!oSearchedUser.GetModel( Convert.ToInt64( strUID ), strScreenName ))
                {
                    MessageBox.Show( "未搜索到指定用户。", "新浪微博爬虫" );
                    oSearchedUser = null;
                }
            ShowSearchedUser();
        }

        //开始任务前初始化robot等
        private void PrepareToStart ()
        {
            robotUser.Initialize();
            robotUser.SinaAPI = api;
            robotStatus.Initialize();
            robotStatus.SinaAPI = api;
            SettingItems settings = AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            robotUser.QueueLength = settings.QueueLength;
            robotUser.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_user.log";
            robotStatus.QueueLength = settings.QueueLength;
            robotStatus.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
        }

        private void btnStartByCurrent_Click ( object sender, EventArgs e )
        {
            CheckLogin();
            if (blnAuthorized)
            {
                strDataBaseStatus = PubHelper.TestDataBase();
                if (strDataBaseStatus != "OK")
                {
                    MessageBox.Show( "数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "新浪微博爬虫" );
                    return;
                }

                if (oAsyncWorkerUser == null && oAsyncWorkerStatus == null)
                {
                    PrepareToStart();
                    oAsyncWorkerUser = new BackgroundWorker();
                    oAsyncWorkerUser.WorkerReportsProgress = true;
                    oAsyncWorkerUser.WorkerSupportsCancellation = true;
                    oAsyncWorkerUser.ProgressChanged += new ProgressChangedEventHandler( UserProgressChanged );
                    oAsyncWorkerUser.RunWorkerCompleted += new RunWorkerCompletedEventHandler( UserCompleteWork );
                    oAsyncWorkerUser.DoWork += new DoWorkEventHandler( StartCrawUserByCurrentUser );
                    robotUser.AsyncWorker = oAsyncWorkerUser;

                    oAsyncWorkerStatus = new BackgroundWorker();
                    oAsyncWorkerStatus.WorkerReportsProgress = true;
                    oAsyncWorkerStatus.WorkerSupportsCancellation = true;
                    oAsyncWorkerStatus.ProgressChanged += new ProgressChangedEventHandler( StatusProgressChanged );
                    oAsyncWorkerStatus.RunWorkerCompleted += new RunWorkerCompletedEventHandler( StatusCompleteWork );
                    oAsyncWorkerStatus.DoWork += new DoWorkEventHandler( StartCrawStatusByCurrentUser );
                    robotStatus.AsyncWorker = oAsyncWorkerStatus;
                }
                if (oAsyncWorkerUser.IsBusy || oAsyncWorkerStatus.IsBusy)
                {
                    //记录原状态
                    bool userState = robotUser.Suspending;
                    bool statusState = robotStatus.Suspending;
                    robotUser.Suspending = true;    //暂停
                    robotStatus.Suspending = true;    //暂停
                    if (MessageBox.Show( "您确定要中止爬虫吗？", "新浪微博爬虫", MessageBoxButtons.YesNo ) == DialogResult.No)
                    {
                        //恢复状态
                        robotUser.Suspending = userState;
                        robotStatus.Suspending = statusState;
                        return;
                    }
                    btnStartByCurrent.Enabled = false;
                    btnStartByCurrent.Text = "正在停止，请稍候...";
                    btnPauseContinue.Enabled = false;
                    robotUser.AsyncCancelled = true;
                    robotStatus.AsyncCancelled = true;
                    oAsyncWorkerUser.CancelAsync();
                    oAsyncWorkerStatus.CancelAsync();
                }
                else
                {
                    rdNoPreLoad.Enabled = false;
                    rdPreLoadUID.Enabled = false;
                    rdPreLoadAllUID.Enabled = false;
                    btnStartByCurrent.Text = "停止爬行";
                    btnStartBySearch.Enabled = false;
                    btnStartByLast.Enabled = false;
                    btnPauseContinue.Text = "暂停";
                    btnPauseContinue.Enabled = true;
                    oAsyncWorkerUser.RunWorkerAsync();
                    oAsyncWorkerStatus.RunWorkerAsync();
                }
            }
        }

        private void btnStartBySearch_Click ( object sender, EventArgs e )
        {
            CheckLogin();
            if (blnAuthorized)
            {
                if (oSearchedUser == null)
                {
                    MessageBox.Show( this, "无搜索结果用户，请先搜索用户。", "新浪微博爬虫" );
                    return;
                }
                strDataBaseStatus = PubHelper.TestDataBase();
                if (strDataBaseStatus != "OK")
                {
                    MessageBox.Show( "数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "新浪微博爬虫" );
                    return;
                }

                if (oAsyncWorkerUser == null && oAsyncWorkerStatus == null)
                {
                    PrepareToStart();
                    oAsyncWorkerUser = new BackgroundWorker();
                    oAsyncWorkerUser.WorkerReportsProgress = true;
                    oAsyncWorkerUser.WorkerSupportsCancellation = true;
                    oAsyncWorkerUser.ProgressChanged += new ProgressChangedEventHandler( UserProgressChanged );
                    oAsyncWorkerUser.RunWorkerCompleted += new RunWorkerCompletedEventHandler( UserCompleteWork );
                    oAsyncWorkerUser.DoWork += new DoWorkEventHandler( StartCrawUserBySearchedUser );
                    robotUser.AsyncWorker = oAsyncWorkerUser;

                    oAsyncWorkerStatus = new BackgroundWorker();
                    oAsyncWorkerStatus.WorkerReportsProgress = true;
                    oAsyncWorkerStatus.WorkerSupportsCancellation = true;
                    oAsyncWorkerStatus.ProgressChanged += new ProgressChangedEventHandler( StatusProgressChanged );
                    oAsyncWorkerStatus.RunWorkerCompleted += new RunWorkerCompletedEventHandler( StatusCompleteWork );
                    oAsyncWorkerStatus.DoWork += new DoWorkEventHandler( StartCrawStatusBySearchedUser );
                    robotStatus.AsyncWorker = oAsyncWorkerStatus;
                }
                if (oAsyncWorkerUser.IsBusy || oAsyncWorkerStatus.IsBusy)
                {
                    //记录原状态
                    bool userState = robotUser.Suspending;
                    bool statusState = robotStatus.Suspending;
                    robotUser.Suspending = true;    //暂停
                    robotStatus.Suspending = true;    //暂停
                    if (MessageBox.Show( "您确定要中止爬虫吗？", "新浪微博爬虫", MessageBoxButtons.YesNo ) == DialogResult.No)
                    {
                        //恢复状态
                        robotUser.Suspending = userState;
                        robotStatus.Suspending = statusState;
                        return;
                    }
                    btnStartBySearch.Enabled = false;
                    btnStartBySearch.Text = "正在停止，请稍候...";
                    btnPauseContinue.Enabled = false;
                    robotUser.AsyncCancelled = true;
                    robotStatus.AsyncCancelled = true;
                    oAsyncWorkerUser.CancelAsync();
                    oAsyncWorkerStatus.CancelAsync();
                }
                else
                {
                    rdNoPreLoad.Enabled = false;
                    rdPreLoadUID.Enabled = false;
                    rdPreLoadAllUID.Enabled = false;
                    btnStartBySearch.Text = "停止爬行";
                    btnStartByCurrent.Enabled = false;
                    btnStartByLast.Enabled = false;
                    btnPauseContinue.Text = "暂停";
                    btnPauseContinue.Enabled = true;
                    oAsyncWorkerUser.RunWorkerAsync();
                    oAsyncWorkerStatus.RunWorkerAsync();
                }
            }
        }

        private void btnStartByLast_Click ( object sender, EventArgs e )
        {
            CheckLogin();
            if (blnAuthorized)
            {
                strDataBaseStatus = PubHelper.TestDataBase();
                if (strDataBaseStatus != "OK")
                {
                    MessageBox.Show( "数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "新浪微博爬虫" );
                    return;
                }
                if (SysArg.GetCurrentUID() == 0)
                {
                    MessageBox.Show( this, "无上次中止用户的记录，请选择其它爬行起点。", "新浪微博爬虫" );
                    return;
                }

                if (oAsyncWorkerUser == null && oAsyncWorkerStatus == null)
                {
                    PrepareToStart();
                    oAsyncWorkerUser = new BackgroundWorker();
                    oAsyncWorkerUser.WorkerReportsProgress = true;
                    oAsyncWorkerUser.WorkerSupportsCancellation = true;
                    oAsyncWorkerUser.ProgressChanged += new ProgressChangedEventHandler( UserProgressChanged );
                    oAsyncWorkerUser.RunWorkerCompleted += new RunWorkerCompletedEventHandler( UserCompleteWork );
                    oAsyncWorkerUser.DoWork += new DoWorkEventHandler( StartCrawUserByLastUser );
                    robotUser.AsyncWorker = oAsyncWorkerUser;

                    oAsyncWorkerStatus = new BackgroundWorker();
                    oAsyncWorkerStatus.WorkerReportsProgress = true;
                    oAsyncWorkerStatus.WorkerSupportsCancellation = true;
                    oAsyncWorkerStatus.ProgressChanged += new ProgressChangedEventHandler( StatusProgressChanged );
                    oAsyncWorkerStatus.RunWorkerCompleted += new RunWorkerCompletedEventHandler( StatusCompleteWork );
                    oAsyncWorkerStatus.DoWork += new DoWorkEventHandler( StartCrawStatusByLastUser );
                    robotStatus.AsyncWorker = oAsyncWorkerStatus;
                }
                if (oAsyncWorkerUser.IsBusy || oAsyncWorkerStatus.IsBusy)
                {
                    //记录原状态
                    bool userState = robotUser.Suspending;
                    bool statusState = robotStatus.Suspending;
                    robotUser.Suspending = true;    //暂停
                    robotStatus.Suspending = true;    //暂停
                    if (MessageBox.Show( "您确定要中止爬虫吗？", "新浪微博爬虫", MessageBoxButtons.YesNo ) == DialogResult.No)
                    {
                        //恢复状态
                        robotUser.Suspending = userState;
                        robotStatus.Suspending = statusState;
                        return;
                    }
                    btnStartByLast.Enabled = false;
                    btnStartByLast.Text = "正在停止，请稍候...";
                    btnPauseContinue.Enabled = false;
                    robotUser.AsyncCancelled = true;
                    robotStatus.AsyncCancelled = true;
                    oAsyncWorkerUser.CancelAsync();
                    oAsyncWorkerStatus.CancelAsync();
                }
                else
                {
                    rdNoPreLoad.Enabled = false;
                    rdPreLoadUID.Enabled = false;
                    rdPreLoadAllUID.Enabled = false;
                    btnStartByLast.Text = "停止爬行";
                    btnStartBySearch.Enabled = false;
                    btnStartByCurrent.Enabled = false;
                    btnPauseContinue.Text = "暂停";
                    btnPauseContinue.Enabled = true;
                    oAsyncWorkerUser.RunWorkerAsync();
                    oAsyncWorkerStatus.RunWorkerAsync();
                }
            }
        }

        private void StartCrawUserByCurrentUser ( Object sender, DoWorkEventArgs e )
        {
            robotUser.Start( oCurrentUser.uid );
        }

        private void StartCrawUserBySearchedUser ( Object sender, DoWorkEventArgs e )
        {
            robotUser.Start( oSearchedUser.uid );
        }

        private void StartCrawUserByLastUser ( Object sender, DoWorkEventArgs e )
        {
            long lLastUID = SysArg.GetCurrentUID(); //能走到这一步，说明lLastUID没问题，故不用验证
            robotUser.Start( lLastUID );
        }

        private void UserProgressChanged ( Object sender, ProgressChangedEventArgs e )
        {
            //抛给StatusRobot
            if (robotUser.ThrownUID != 0 && !lstQueueUserToStatus.Contains( robotUser.ThrownUID ))
            {
                lstQueueUserToStatus.AddLast( robotUser.ThrownUID );
                lblUserToStatus.Text = "用户机器人传递给微博机器人的用户队列长度为：" + lstQueueUserToStatus.Count.ToString();
            }
            //完成一个用户的迭代时，获取StatusRobot抛来的。
            //第一个条件防止进程陷入获取元素增长队列的过程
            if (robotUser.OneUserCompleted && lstQueueStatusToUser.Count > 0)
            {
                long lUID = lstQueueStatusToUser.First.Value;
                if (lUID > 0)
                {
                    if (robotUser.QueueExists( lUID ))
                    {
                        //日志
                        robotUser.LogMessage= DateTime.Now.ToString() + "  " + "用户" + lUID.ToString() + "已在队列中...";
                    }
                    else
                    {
                        //日志
                        robotUser.LogMessage = DateTime.Now.ToString() + "  " + "将用户" + lUID.ToString() + "加入队列。内存队列中有" + robotUser.LengthOfQueueInMem.ToString() + "个用户；数据库队列中有" + robotUser.LengthOfQueueInDB.ToString() + "个用户";
                        robotUser.Enqueue( lUID );
                    }
                    lstQueueStatusToUser.RemoveFirst();
                }
            }

            robotUser.Actioned( lblUserMessage );
        }

        private void UserCompleteWork ( Object sender, RunWorkerCompletedEventArgs e )
        {
            if (e.Error != null)
            {
                MessageBox.Show( this, e.Error.Message );
                return;
            }
            robotUser.Initialize();
            lblUserMessage.Text = "停止。";

            if (oAsyncWorkerStatus == null)  //如果微博机器人也已停止
            {
                btnStartByCurrent.Text = "以当前登录帐号为起点开始爬行";
                btnStartBySearch.Text = "以搜索结果用户为起点开始爬行";
                btnStartByLast.Text = "以上次中止的用户为起点开始爬行";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                rdNoPreLoad.Enabled = true;
                rdPreLoadUID.Enabled = true;
                rdPreLoadAllUID.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "暂停/停止";

                MessageBox.Show( this, "爬虫已停止。", "新浪微博爬虫" );
            }
            oAsyncWorkerUser = null;
            lstQueueUserToStatus.Clear();
            lblUserToStatus.Text = "用户机器人传递给微博机器人的用户队列长度为：0";
        }

        private void StartCrawStatusByCurrentUser ( Object sender, DoWorkEventArgs e )
        {
            robotStatus.Start( oCurrentUser.uid );
        }

        private void StartCrawStatusBySearchedUser ( Object sender, DoWorkEventArgs e )
        {
            robotStatus.Start( oSearchedUser.uid );
        }

        private void StartCrawStatusByLastUser ( Object sender, DoWorkEventArgs e )
        {
            long lLastUID = SysArg.GetCurrentUID(); //能走到这一步，说明lLastUID没问题，故不用验证
            robotStatus.Start( lLastUID );
        }

        private void StatusProgressChanged ( Object sender, ProgressChangedEventArgs e )
        {
            //抛给UserRobot
            if (robotStatus.ThrownUID != 0 && !lstQueueStatusToUser.Contains( robotStatus.ThrownUID ))
            {
                lstQueueStatusToUser.AddLast( robotStatus.ThrownUID );
                lblStatusToUser.Text = "微博机器人传递给用户机器人的用户队列长度为：" + lstQueueStatusToUser.Count.ToString();
            }
            //完成一个用户的迭代时，获取UserRobot抛来的。
            //第一个条件防止进程陷入获取元素增长队列的过程
            if (robotStatus.OneUserCompleted && lstQueueUserToStatus.Count > 0)
            {
                long lUID = lstQueueUserToStatus.First.Value;
                if (lUID > 0)
                {
                    if (robotStatus.QueueExists( lUID ))
                    {
                        //日志
                        robotStatus.LogMessage = DateTime.Now.ToString() + "  " + "用户" + lUID.ToString() + "已在队列中...";
                    }
                    else
                    {
                        //日志
                        robotStatus.LogMessage = DateTime.Now.ToString() + "  " + "将用户" + lUID.ToString() + "加入队列。内存队列中有" + robotStatus.LengthOfQueueInMem.ToString() + "个用户；数据库队列中有" + robotStatus.LengthOfQueueInDB.ToString() + "个用户";
                        robotStatus.Enqueue( lUID );
                    }
                    lstQueueUserToStatus.RemoveFirst();
                }
            }
            robotStatus.Actioned( lblStatusMessage );
        }

        private void StatusCompleteWork ( Object sender, RunWorkerCompletedEventArgs e )
        {
            if (e.Error != null)
            {
                MessageBox.Show( this, e.Error.Message );
                return;
            }
            robotStatus.Initialize();
            lblStatusMessage.Text = "停止。";

            if (oAsyncWorkerUser == null)  //如果用户机器人也已停止
            {
                btnStartByCurrent.Text = "以当前登录帐号为起点开始爬行";
                btnStartBySearch.Text = "以搜索结果用户为起点开始爬行";
                btnStartByLast.Text = "以上次中止的用户为起点开始爬行";
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;
                rdNoPreLoad.Enabled = true;
                rdPreLoadUID.Enabled = true;
                rdPreLoadAllUID.Enabled = true;
                btnPauseContinue.Enabled = false;
                btnPauseContinue.Text = "暂停/停止";

                MessageBox.Show( this, "爬虫已停止。", "新浪微博爬虫" );
            }
            oAsyncWorkerStatus = null;
            lstQueueStatusToUser.Clear();
            lblStatusToUser.Text = "微博机器人传递给用户机器人的用户队列长度为：0";
        }

        private void ShowSettings ( SettingItems settings )
        {
            tbQueueLength.Value = settings.QueueLength;
            numQueueLength.Value = settings.QueueLength;

            if (settings.DBType == "SQL Server")
                drplstDBType.SelectedIndex = 0;
            else
                drplstDBType.SelectedIndex = 1;

            txtDBServer.Text = settings.DBServer;
            txtDBUserName.Text = settings.DBUserName;
            txtDBPwd.Text = settings.DBPwd;
            txtDBName.Text = settings.DBName;
        }

        private void btnDefault_Click ( object sender, EventArgs e )
        {
            SettingItems settings = AppSettings.LoadDefault();
            ShowSettings( settings );
        }

        private void btnLoad_Click ( object sender, EventArgs e )
        {
            LoadSettings();
        }

        private void btnSave_Click ( object sender, EventArgs e )
        {
            SettingItems settings = new SettingItems();
            settings.QueueLength = tbQueueLength.Value;
            settings.DBType = drplstDBType.SelectedItem.ToString();
            settings.DBServer = txtDBServer.Text.Trim();
            settings.DBUserName = txtDBUserName.Text.Trim();
            settings.DBPwd = txtDBPwd.Text;
            settings.DBName = txtDBName.Text.Trim();

            AppSettings.Save( settings );

            MessageBox.Show( "设置已保存。启动新的爬虫任务时将使用新的设置。", "新浪微博爬虫" );
        }

        private void numQueueLength_ValueChanged ( object sender, EventArgs e )
        {
            tbQueueLength.Value = Convert.ToInt32( numQueueLength.Value );
        }

        private void tbQueueLength_ValueChanged ( object sender, EventArgs e )
        {
            numQueueLength.Value = tbQueueLength.Value;
        }

        private void btnPost_Click ( object sender, EventArgs e )
        {
            CheckLogin();
            if (blnAuthorized)
            {
                if (PubHelper.PostAdvertisement( api ))
                    MessageBox.Show( "您已经帮忙发布了一条推广此应用的微博。\n感谢您对本应用的支持！", "新浪微博爬虫" );
                else
                    MessageBox.Show( "对不起，发布推广微博失败，请重试，或到应用主页提出您的宝贵意见。", "新浪微博爬虫" );
            }
        }

        private void rdNoPreLoad_Click ( object sender, EventArgs e )
        {
            rdPreLoadUID.Checked = !rdNoPreLoad.Checked;
            rdPreLoadAllUID.Checked = !rdNoPreLoad.Checked;
            if (rdNoPreLoad.Checked)
            {
                robotUser.PreLoadQueue = EnumPreLoadQueue.NO_PRELOAD;
                robotStatus.PreLoadQueue = EnumPreLoadQueue.NO_PRELOAD;
            }
        }

        private void rdPreLoadUID_Click ( object sender, EventArgs e )
        {
            rdNoPreLoad.Checked = !rdPreLoadUID.Checked;
            rdPreLoadAllUID.Checked = !rdPreLoadUID.Checked;
            if (rdPreLoadUID.Checked)
            {
                robotUser.PreLoadQueue = EnumPreLoadQueue.PRELOAD_UID;
                robotStatus.PreLoadQueue = EnumPreLoadQueue.PRELOAD_UID;
            }
        }

        private void rdPreLoadAllUID_Click ( object sender, EventArgs e )
        {
            rdNoPreLoad.Checked = !rdPreLoadAllUID.Checked;
            rdPreLoadUID.Checked = !rdPreLoadAllUID.Checked;
            if (rdPreLoadAllUID.Checked)
            {
                robotUser.PreLoadQueue = EnumPreLoadQueue.PRELOAD_ALL_UID;
                robotStatus.PreLoadQueue = EnumPreLoadQueue.PRELOAD_ALL_UID;
            }
        }

        private void btnPauseContinue_Click ( object sender, EventArgs e )
        {
            if (btnPauseContinue.Text == "暂停")
                btnPauseContinue.Text = "继续";
            else
                btnPauseContinue.Text = "暂停";
            robotUser.Suspending = !robotUser.Suspending;
            robotStatus.Suspending = !robotStatus.Suspending;
        }

        private void frmMain_FormClosing ( object sender, FormClosingEventArgs e )
        {
            if (!CanBeClosed()) e.Cancel=true;
        }
    }
}
