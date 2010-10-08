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

        public frmMain()
        {
            InitializeComponent();
        }

        private void CheckLogin()   //检查登录状态。若未登录，弹出登录框
        {
            if (!blnAuthorized)
            {
                frmLogin login = new frmLogin(api);
                if (login.ShowDialog() == DialogResult.OK)
                    blnAuthorized = true;
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
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnSearchOnline_Click(object sender, EventArgs e)
        {
            if (!blnAuthorized)
            {
                CheckLogin();
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

                    //初始化变量
                    lstWaitingUID = User.GetCrawedUID();
                    lCurrentUID = oCurrentUser.uid;
                }
                else
                {
                    btnSearchOnline.Enabled = true;
                    btnSearchOffLine.Enabled = true;
                    btnStartByCurrent.Enabled = false;
                    btnStartBySearch.Enabled = false;
                    btnStartByLast.Enabled = true;
                    btnExit.Enabled = true;
                    return;
                }
            }
            if (blnAuthorized)
            {
                if (txtUID.Text.Trim() == "" && txtUserName.Text.Trim() == "")
                {
                    MessageBox.Show("请至少输入“用户ID”和“用户昵称”之一。", "请输入搜索条件");
                    txtUID.Focus();
                    return;
                }
                string strUID = txtUID.Text.Trim();
                string strScreenName = txtUserName.Text.Trim();
                long lBuffer;
                if (strUID != "" && !long.TryParse(strUID, out lBuffer))
                {
                    MessageBox.Show("请输入正确的用户ID。", "输入错误");
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
                if (oSearchedUser == null) MessageBox.Show("未搜索到指定用户。", "搜索结果");
                ShowSearchedUser();
            }
        }

        private void btnSearchOffLine_Click(object sender, EventArgs e)
        {
            if (txtUID.Text.Trim() == "" && txtUserName.Text.Trim() == "")
            {
                MessageBox.Show("请至少输入“用户ID”和“用户昵称”之一。", "请输入搜索条件");
                txtUID.Focus();
                return;
            }
            string strUID = txtUID.Text.Trim();
            string strScreenName = txtUserName.Text.Trim();
            long lBuffer;
            if (strUID != "" && !long.TryParse(strUID, out lBuffer))
            {
                MessageBox.Show("请输入正确的用户ID。", "输入错误");
                return;
            }
            if (strUID != "" && strScreenName == "")
                if (!oSearchedUser.GetModel(Convert.ToInt64(strUID)))
                {
                    MessageBox.Show("未搜索到指定用户。", "搜索结果");
                    oSearchedUser = null;
                }
            if (strUID == "" && strScreenName != "")
                if (!oSearchedUser.GetModel(strScreenName))
                {
                    MessageBox.Show("未搜索到指定用户。", "搜索结果");
                    oSearchedUser = null;
                }
            if (strUID != "" && strScreenName != "")
                if (!oSearchedUser.GetModel(Convert.ToInt64(strUID), strScreenName))
                {
                    MessageBox.Show("未搜索到指定用户。", "搜索结果");
                    oSearchedUser = null;
                }
            ShowSearchedUser();
        }

        private void btnStartByCurrent_Click(object sender, EventArgs e)
        {
            CheckLogin();
            if (blnAuthorized)
            {
                Robot robot = new Robot(api, rtxtLog);
                if (oAsyncWorker == null)
                {
                    //清除日志窗口
                    rtxtLog.Clear();
                    
                    robot.StartUID = oCurrentUser.uid;

                    oAsyncWorker = new BackgroundWorker();
                    oAsyncWorker.WorkerReportsProgress = true;
                    oAsyncWorker.WorkerSupportsCancellation = true;
                    oAsyncWorker.ProgressChanged += new ProgressChangedEventHandler(robot.Actioned);
                    oAsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(oAsyncWorker_RunWorkerCompleted);
                    oAsyncWorker.DoWork += new DoWorkEventHandler(robot.StartCrawByCurrentUser);
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
            if (oSearchedUser == null)
            {
                MessageBox.Show(this, "无搜索结果用户，请先搜索用户。");
                return;
            }
            if (blnAuthorized)
            {
                if (oAsyncWorker == null)
                {
                    //清除日志窗口
                    rtxtLog.Clear();
                    sw = new StreamWriter(Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".log");
                    oAsyncWorker = new BackgroundWorker();
                    oAsyncWorker.WorkerReportsProgress = true;
                    oAsyncWorker.WorkerSupportsCancellation = true;
                    oAsyncWorker.ProgressChanged += new ProgressChangedEventHandler(oAsyncWorker_ProgressChanged);
                    oAsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(oAsyncWorker_RunWorkerCompleted);
                    oAsyncWorker.DoWork += new DoWorkEventHandler(StartCrawBySearchedUser);
                }
                if (oAsyncWorker.IsBusy)
                {
                    btnStartBySearch.Enabled = false;
                    btnStartBySearch.Text = "正在停止，请稍候...";
                    blnAsyncCancelled = true;
                    oAsyncWorker.CancelAsync();
                }
                else
                {
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
            if (SysArg.GetCurrentUID() == 0)
            {
                MessageBox.Show(this, "无上次中止用户的记录，请选择其它爬行起点。");
                return;
            }
            if (blnAuthorized)
            {
                if (oAsyncWorker == null)
                {
                    //清除日志窗口
                    rtxtLog.Clear();
                    sw = new StreamWriter(Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".log");
                    oAsyncWorker = new BackgroundWorker();
                    oAsyncWorker.WorkerReportsProgress = true;
                    oAsyncWorker.WorkerSupportsCancellation = true;
                    oAsyncWorker.ProgressChanged += new ProgressChangedEventHandler(oAsyncWorker_ProgressChanged);
                    oAsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(oAsyncWorker_RunWorkerCompleted);
                    oAsyncWorker.DoWork += new DoWorkEventHandler(StartCrawByLastUser);
                }
                if (oAsyncWorker.IsBusy)
                {
                    btnStartByLast.Enabled = false;
                    btnStartByLast.Text = "正在停止，请稍候...";
                    blnAsyncCancelled = true;
                    oAsyncWorker.CancelAsync();
                }
                else
                {
                    btnStartByLast.Text = "停止爬行";
                    btnStartBySearch.Enabled = false;
                    btnStartByCurrent.Enabled = false;
                    oAsyncWorker.RunWorkerAsync();
                }
            }
        }

        private void oAsyncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnStartByCurrent.Text = "以当前登录帐号为起点开始爬行";
            btnStartBySearch.Text = "以搜索结果用户为起点开始爬行";
            btnStartByLast.Text = "以上次中止的用户为起点开始爬行";
            btnStartByCurrent.Enabled = true;
            btnStartBySearch.Enabled = true;
            btnStartByLast.Enabled = true;
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message);
                return;
            }
            if (e.Cancelled)
            {
                //Cancelled
                MessageBox.Show(this, "爬虫已停止。");
            }
            else
            {
                //Completed...
                MessageBox.Show(this, "爬虫已停止。");
            }
            oAsyncWorker = null;
            blnAsyncCancelled = false;
        }
    }
}
