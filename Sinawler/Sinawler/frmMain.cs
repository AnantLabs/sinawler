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

        private BackgroundWorker oAsyncWorker = null;

        private Robot robot;                                //爬虫机器人

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
                MessageBox.Show("读取配置文件时发生错误，将加载默认值。", "新浪微博爬虫");
                settings = new SettingItems();
            }
            ShowSettings(settings);
        }

        //检查登录状态。若未登录，弹出登录框
        private void CheckLogin()
        {
            if (!blnAuthorized)
            {
                frmLogin login = new frmLogin(api);
                if (login.ShowDialog() == DialogResult.OK)
                {
                    blnAuthorized = true;
                }
            }
        }

        //显示登录帐号用户信息
        private void ShowCurrentUser()
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
                string[] strCreatedDate = strCreatedAt.Split(' ')[0].Split('-');
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
        private void ShowSearchedUser()
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
                string[] strCreatedDate = strCreatedAt.Split(' ')[0].Split('-');
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

        private void frmMain_Load(object sender, EventArgs e)
        {
            CheckLogin();
            drplstDBType.SelectedIndex = 0;
            if (blnAuthorized)
            {
                SinaMBCrawler crawler = new SinaMBCrawler(api);
                crawler.SleepTime = 0;  //这里不等待
                oCurrentUser = crawler.GetCurrentUserInfo();
                oSearchedUser = oCurrentUser;
                ShowCurrentUser();
                ShowSearchedUser();
                txtUID.Text = oSearchedUser.uid.ToString();
                txtUserName.Text = oSearchedUser.screen_name;

                robot = new Robot( api );   //爬虫机器人
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnSearchOnline_Click(object sender, EventArgs e)
        {
            CheckLogin();            
            if (blnAuthorized)
            {
                btnSearchOnline.Enabled = true;
                btnSearchOffLine.Enabled = true;
                btnStartByCurrent.Enabled = true;
                btnStartBySearch.Enabled = true;
                btnStartByLast.Enabled = true;

                if (txtUID.Text.Trim() == "" && txtUserName.Text.Trim() == "")
                {
                    MessageBox.Show("请至少输入“用户ID”和“用户昵称”之一。", "新浪微博爬虫");
                    txtUID.Focus();
                    return;
                }
                string strUID = txtUID.Text.Trim();
                string strScreenName = txtUserName.Text.Trim();
                long lBuffer;
                if (strUID != "" && !long.TryParse(strUID, out lBuffer))
                {
                    MessageBox.Show("请输入正确的用户ID。", "新浪微博爬虫");
                    return;
                }
                SinaMBCrawler crawler = new SinaMBCrawler(api);
                crawler.SleepTime = 0;  //这里不等待
                if (strUID != "" && strScreenName == "")
                    oSearchedUser = crawler.GetUserInfo(Convert.ToInt64(strUID));
                if (strUID == "" && strScreenName != "")
                    oSearchedUser = crawler.GetUserInfo(strScreenName);
                if (strUID != "" && strScreenName != "")
                    oSearchedUser = crawler.GetUserInfo(Convert.ToInt64(strUID), strScreenName);
                if (oSearchedUser == null) MessageBox.Show("未搜索到指定用户。", "新浪微博爬虫");
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
            if (txtUID.Text.Trim() == "" && txtUserName.Text.Trim() == "")
            {
                MessageBox.Show("请至少输入“用户ID”和“用户昵称”之一。", "新浪微博爬虫");
                txtUID.Focus();
                return;
            }
            strDataBaseStatus = PubHelper.TestDataBase();
            if (strDataBaseStatus != "OK")
            {
                MessageBox.Show("数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "新浪微博爬虫");
                return;
            }
            string strUID = txtUID.Text.Trim();
            string strScreenName = txtUserName.Text.Trim();
            long lBuffer;
            if (strUID != "" && !long.TryParse(strUID, out lBuffer))
            {
                MessageBox.Show("请输入正确的用户ID。", "新浪微博爬虫");
                return;
            }
            if (strUID != "" && strScreenName == "")
                if (!oSearchedUser.GetModel(Convert.ToInt64(strUID)))
                {
                    MessageBox.Show("未搜索到指定用户。", "新浪微博爬虫");
                    oSearchedUser = null;
                }
            if (strUID == "" && strScreenName != "")
                if (!oSearchedUser.GetModel(strScreenName))
                {
                    MessageBox.Show("未搜索到指定用户。", "新浪微博爬虫");
                    oSearchedUser = null;
                }
            if (strUID != "" && strScreenName != "")
                if (!oSearchedUser.GetModel(Convert.ToInt64(strUID), strScreenName))
                {
                    MessageBox.Show("未搜索到指定用户。", "新浪微博爬虫");
                    oSearchedUser = null;
                }
            ShowSearchedUser();
        }

        //开始任务前初始化robot等
        private void PrepareToStart()
        {
            robot.Initialize();
            robot.SinaAPI = api;
            SettingItems settings = AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            robot.QueueLength = settings.QueueLength;
            robot.LogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".log";
        }

        private void btnStartByCurrent_Click(object sender, EventArgs e)
        {
            CheckLogin();
            if (blnAuthorized)
            {
                strDataBaseStatus = PubHelper.TestDataBase();
                if (strDataBaseStatus != "OK")
                {
                    MessageBox.Show("数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "新浪微博爬虫");
                    return;
                }
                
                if (oAsyncWorker == null)
                {
                    PrepareToStart();
                    oAsyncWorker = new BackgroundWorker();
                    oAsyncWorker.WorkerReportsProgress = true;
                    oAsyncWorker.WorkerSupportsCancellation = true;
                    oAsyncWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
                    oAsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompleteWork);
                    oAsyncWorker.DoWork += new DoWorkEventHandler(StartCrawByCurrentUser);
                }
                if (oAsyncWorker.IsBusy)
                {
                    btnStartByCurrent.Enabled = false;
                    btnStartByCurrent.Text = "正在停止，请稍候...";
                    robot.AsyncCancelled = true;
                    oAsyncWorker.CancelAsync();
                }
                else
                {
                    rdNoPreLoad.Enabled = false;
                    rdPreLoadUID.Enabled = false;
                    rdPreLoadAllUID.Enabled = false;
                    btnStartByCurrent.Text = "停止爬行";
                    btnStartBySearch.Enabled = false;
                    btnStartByLast.Enabled = false;
                    oAsyncWorker.RunWorkerAsync();
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
                    MessageBox.Show(this, "无搜索结果用户，请先搜索用户。", "新浪微博爬虫");
                    return;
                }
                strDataBaseStatus = PubHelper.TestDataBase();
                if (strDataBaseStatus != "OK")
                {
                    MessageBox.Show("数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "新浪微博爬虫");
                    return;
                }
                
                if (oAsyncWorker == null)
                {
                    PrepareToStart();
                    oAsyncWorker = new BackgroundWorker();
                    oAsyncWorker.WorkerReportsProgress = true;
                    oAsyncWorker.WorkerSupportsCancellation = true;
                    oAsyncWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
                    oAsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( CompleteWork );
                    oAsyncWorker.DoWork += new DoWorkEventHandler(StartCrawBySearchedUser);
                }
                if (oAsyncWorker.IsBusy)
                {
                    btnStartBySearch.Enabled = false;
                    btnStartBySearch.Text = "正在停止，请稍候...";
                    robot.AsyncCancelled = true;
                    oAsyncWorker.CancelAsync();
                }
                else
                {
                    rdNoPreLoad.Enabled = false;
                    rdPreLoadUID.Enabled = false;
                    rdPreLoadAllUID.Enabled = false;
                    btnStartBySearch.Text = "停止爬行";
                    btnStartByCurrent.Enabled = false;
                    btnStartByLast.Enabled = false;
                    oAsyncWorker.RunWorkerAsync();
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
                    MessageBox.Show("数据库错误：" + strDataBaseStatus + "。\n请正确设置数据库。", "新浪微博爬虫");
                    return;
                }
                if (SysArg.GetCurrentUID() == 0)
                {
                    MessageBox.Show(this, "无上次中止用户的记录，请选择其它爬行起点。", "新浪微博爬虫");
                    return;
                }
                
                if (oAsyncWorker == null)
                {
                    PrepareToStart();
                    oAsyncWorker = new BackgroundWorker();
                    oAsyncWorker.WorkerReportsProgress = true;
                    oAsyncWorker.WorkerSupportsCancellation = true;
                    oAsyncWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
                    oAsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( CompleteWork );
                    oAsyncWorker.DoWork += new DoWorkEventHandler(StartCrawByLastUser);
                }
                if (oAsyncWorker.IsBusy)
                {
                    btnStartByLast.Enabled = false;
                    btnStartByLast.Text = "正在停止，请稍候...";
                    robot.AsyncCancelled = true;
                    oAsyncWorker.CancelAsync();
                }
                else
                {
                    rdNoPreLoad.Enabled = false;
                    rdPreLoadUID.Enabled = false;
                    rdPreLoadAllUID.Enabled = false;
                    btnStartByLast.Text = "停止爬行";
                    btnStartBySearch.Enabled = false;
                    btnStartByCurrent.Enabled = false;
                    oAsyncWorker.RunWorkerAsync();
                }
            }
        }
                
        private void StartCrawByCurrentUser(Object sender,DoWorkEventArgs e)
        {
            robot.Start( oCurrentUser.uid, oAsyncWorker );
        }

        private void StartCrawBySearchedUser ( Object sender, DoWorkEventArgs e )
        {
            robot.Start( oSearchedUser.uid, oAsyncWorker );
        }

        private void StartCrawByLastUser ( Object sender, DoWorkEventArgs e )
        {
            long lLastUID = SysArg.GetCurrentUID(); //能走到这一步，说明lLastUID没问题
            robot.Start( lLastUID,oAsyncWorker );
        }

        private void ProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            robot.Actioned(lblStatusMessage);
        }

        private void CompleteWork(Object sender, RunWorkerCompletedEventArgs e)
        {
            robot.Initialize();
            lblStatusMessage.Text = "停止。";
            btnStartByCurrent.Text = "以当前登录帐号为起点开始爬行";
            btnStartBySearch.Text = "以搜索结果用户为起点开始爬行";
            btnStartByLast.Text = "以上次中止的用户为起点开始爬行";
            btnStartByCurrent.Enabled = true;
            btnStartBySearch.Enabled = true;
            btnStartByLast.Enabled = true;
            rdNoPreLoad.Enabled = true;
            rdPreLoadUID.Enabled = true;
            rdPreLoadAllUID.Enabled = true;
            if (e.Error != null)
            {
                MessageBox.Show( this, e.Error.Message );
                return;
            }
            if (e.Cancelled)
            {
                //Cancelled
                MessageBox.Show(this, "爬虫已停止。", "新浪微博爬虫");
            }
            else
            {
                //Completed...
                MessageBox.Show(this, "爬虫已停止。", "新浪微博爬虫");
            }
            oAsyncWorker = null;
        }

        private void ShowSettings(SettingItems settings)
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
            SettingItems settings=AppSettings.LoadDefault();
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

            MessageBox.Show("设置已保存。启动新的爬虫任务时将使用新的设置。", "新浪微博爬虫");
        }

        private void numQueueLength_ValueChanged ( object sender, EventArgs e )
        {
            tbQueueLength.Value = Convert.ToInt32(numQueueLength.Value);
        }

        private void tbQueueLength_ValueChanged ( object sender, EventArgs e )
        {
            numQueueLength.Value = tbQueueLength.Value;
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            CheckLogin();
            if (blnAuthorized)
            {
                if (PubHelper.PostAdvertisement(api))
                    MessageBox.Show("您已经帮忙发布了一条推广此应用的微博。\n感谢您对本应用的支持！","新浪微博爬虫");
                else
                    MessageBox.Show("对不起，发布推广微博失败，请重试，或到应用主页提出您的宝贵意见。", "新浪微博爬虫");
            }
        }

        private void rdNoPreLoad_CheckedChanged ( object sender, EventArgs e )
        {
            rdPreLoadUID.Checked = !rdNoPreLoad.Checked;
            rdPreLoadAllUID.Checked = !rdNoPreLoad.Checked;
            if (rdNoPreLoad.Checked)
                robot.PreLoadQueue = EnumPreLoadQueue.NO_PRELOAD;
        }

        private void rdPreLoadUID_CheckedChanged ( object sender, EventArgs e )
        {
            rdNoPreLoad.Checked = !rdPreLoadUID.Checked;
            rdPreLoadAllUID.Checked = !rdPreLoadUID.Checked;
            if (rdPreLoadUID.Checked)
                robot.PreLoadQueue = EnumPreLoadQueue.PRELOAD_UID;
        }

        private void rdPreLoadAllUID_CheckedChanged ( object sender, EventArgs e )
        {
            rdNoPreLoad.Checked = !rdPreLoadAllUID.Checked;
            rdPreLoadUID.Checked = !rdPreLoadAllUID.Checked;
            if (rdPreLoadAllUID.Checked)
                robot.PreLoadQueue = EnumPreLoadQueue.PRELOAD_ALL_UID;
        }
    }
}
