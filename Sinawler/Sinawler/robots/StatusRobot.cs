using System;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Sinawler.Model;
using System.Data;

namespace Sinawler
{
    class StatusRobot : RobotBase
    {
        private UserQueue queueUserForUserInfoRobot;        //用户信息机器人使用的用户队列引用
        private UserQueue queueUserForUserRelationRobot;    //用户关系机器人使用的用户队列引用
        private UserQueue queueUserForUserTagRobot;         //用户标签机器人使用的用户队列引用
        private UserQueue queueUserForStatusRobot;          //微博机器人使用的用户队列引用
        private StatusQueue queueStatus;                    //微博队列引用
        private UserBuffer oUserBuffer;                     //the buffer queue of users

        //构造函数，需要传入相应的新浪微博API和主界面
        public StatusRobot()
            : base()
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
            queueStatus = GlobalPool.StatusQueue;
            oUserBuffer = GlobalPool.UserBuffer;
        }

        /// <summary>
        /// 处理并保存爬取的微博数据
        /// </summary>
        private void SaveStatus(Status status)
        {
            //lCurrentID=status.status_id
            //日志
            Log("将微博" + lCurrentID.ToString() + "存入数据库...");
            status.Add();

            if (queueStatus.Enqueue(lCurrentID))
                Log("将微博" + lCurrentID.ToString() + "加入微博队列。");

            //若该微博有转发，将转发微博保存
            if (status.retweeted_status != null)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }

                //日志
                Log("微博" + lCurrentID.ToString() + "有转发微博，将转发微博" + status.retweeted_status.status_id.ToString() + "存入数据库...");

                if (!Status.Exists(status.retweeted_status.status_id))
                {
                    status.retweeted_status.Add();

                    //日志
                    Log("转发微博" + status.retweeted_status.status_id.ToString() + "已保存。");
                }
                else
                {
                    //日志
                    Log("转发微博" + status.retweeted_status.status_id.ToString() + "已存在。");
                }

                if (queueStatus.Enqueue(status.retweeted_status.status_id))
                    Log("将转发微博" + status.retweeted_status.status_id.ToString() + "加入微博队列。");

                if (queueUserForUserRelationRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("将用户" + status.retweeted_status.user.user_id.ToString() + "加入用户关系机器人的用户队列。");
                if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("将用户" + status.retweeted_status.user.user_id.ToString() + "加入用户信息机器人的用户队列。");
                if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("将用户" + status.retweeted_status.user.user_id.ToString() + "加入用户标签机器人的用户队列。");
                if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("将用户" + status.retweeted_status.user.user_id.ToString() + "加入微博机器人的用户队列。");
                //add the user into the buffer only when the user does not exist in the queue for userInfo
                if (GlobalPool.UserInfoRobotEnabled && !queueUserForUserInfoRobot.QueueExists(status.retweeted_status.user.user_id) && oUserBuffer.Enqueue(status.retweeted_status.user))
                    Log("将用户" + status.retweeted_status.user.user_id.ToString() + "加入用户缓冲池。");
            }
        }

        /// <summary>
        /// 开始爬取微博数据
        /// </summary>
        public void Start()
        {
            while (queueUserForStatusRobot.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(50);   //若队列为空，则等待
            }
            Thread.Sleep(500);  //waiting that user relation robot update limit data

            long lStartUserID = queueUserForStatusRobot.FirstValue;
            long lCurrentUserID = 0;

            AdjustFreq();
            Log("初始请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + GlobalPool.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + GlobalPool.RemainingHits.ToString() + "次");

            //对队列无限循环爬行，直至有操作暂停或停止
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }

                //将队头取出
                lCurrentUserID = queueUserForStatusRobot.RollQueue();

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }

                Status status;
                #region 通过web抓取所有微博
                //日志
                Log("爬取用户" + lCurrentUserID.ToString() + "的所有微博ID列表...");
                frmBrowser frm = new frmBrowser();
                frm.SleepTime = crawler.SleepTime;
                frm.StopCrawling = crawler.StopCrawling;
                frm.UserID = lCurrentUserID;
                LinkedList<long> lstStatusID = GlobalPool.StatusIDsListByWeb;
                //日志
                Log("爬得" + lstStatusID.Count.ToString() + "条微博。");

                while (lstStatusID.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }

                    lCurrentID = lstStatusID.First.Value;
                    if (!Status.Exists(lCurrentID))
                    {
                        //日志
                        Log("爬取微博" + lCurrentID.ToString() + "的内容...");
                        status = crawler.GetStatus(lCurrentID);
                        SaveStatus(status);
                        //日志
                        AdjustFreq();
                        Log("调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + GlobalPool.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + GlobalPool.RemainingHits.ToString() + "次");
                    }
                    lstStatusID.RemoveFirst();
                }
                #endregion

                //日志
                Log("用户" + lCurrentUserID.ToString() + "的微博数据已爬取完毕。");
                //日志
                AdjustFreq();
                Log("调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + GlobalPool.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + GlobalPool.RemainingHits.ToString() + "次");
            }
        }

        public override void Initialize()
        {
            //初始化相应变量
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueUserForStatusRobot.Initialize();
        }

        sealed protected override void AdjustFreq()
        {
            base.AdjustFreq();
        }
    }
}
