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
    class StatusRobot:RobotBase
    {
        private UserQueue queueUserForUserRobot;            //用户机器人使用的用户队列引用
        private UserQueue queueUserForStatusRobot;          //微博机器人使用的用户队列引用
        private StatusQueue queueStatus;        //微博队列引用

        //构造函数，需要传入相应的新浪微博API和主界面
        public StatusRobot ( SinaApiService oAPI,UserQueue qUserForUserRobot, UserQueue qUserForStatusRobot,StatusQueue qStatus ):base(oAPI)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
            queueUserForUserRobot = qUserForUserRobot;
            queueUserForStatusRobot = qUserForStatusRobot;
            queueStatus = qStatus;
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
            long lStartUserID = queueUserForStatusRobot.FirstValue;
            long lCurrentUserID = 0;
            //对队列无限循环爬行，直至有操作暂停或停止
            while(true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }

                //将队头取出
                lCurrentUserID = queueUserForStatusRobot.RollQueue();

                #region 预处理
                if (lCurrentUserID == lStartUserID)  //说明经过一次循环迭代
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }

                    //日志
                    Log("开始爬行之前增加迭代次数...");

                    Status.NewIterate();
                }
                #endregion                
                #region 用户微博信息
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //日志
                Log("获取数据库中用户" + lCurrentUserID.ToString() + "最新一条微博的ID...");
                //获取数据库中当前用户最新一条微博的ID
                lCurrentID = Status.GetLastStatusIDOf( lCurrentUserID );

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //日志
                Log("爬取用户" + lCurrentUserID.ToString() + "的ID在" + lCurrentID.ToString() + "之后的微博...");
                //爬取数据库中当前用户最新一条微博的ID之后的微博，存入数据库
                List<Status> lstStatus = crawler.GetStatusesOfSince(lCurrentUserID, lCurrentID);
                //日志
                Log("爬得" + lstStatus.Count.ToString() + "条微博。");

                foreach (Status status in lstStatus)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    lCurrentID = status.status_id;
                    if (!Status.Exists(lCurrentID))
                    {
                        //日志
                        Log("将微博" + lCurrentID.ToString() + "存入数据库...");
                        status.Add();
                    }

                    if(queueStatus.Enqueue(lCurrentID))
                        Log( "将微博" + lCurrentID.ToString() + "加入微博队列。" );
                    else
                        Log( "微博" + lCurrentID.ToString() + "已在微博队列中。" );
                    
                    //若该微博有转发，将转发微博保存
                    if (status.retweeted_status != null)
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep( 10 );
                        }

                        //日志
                        Log("微博" + lCurrentID.ToString() + "有转发微博，将转发微博" + status.retweeted_status.status_id.ToString() + "存入数据库...");

                        if (!Status.Exists( status.retweeted_status.status_id ))
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

                        if (queueStatus.Enqueue( status.retweeted_status.status_id ))
                            Log( "将转发微博" + status.retweeted_status.status_id.ToString() + "加入微博队列。" );
                        else
                            Log( "转发微博" + status.retweeted_status.status_id.ToString() + "已在微博队列中。" );

                        if (queueUserForUserRobot.Enqueue( status.retweeted_status.user_id ))
                            Log( "将用户" + status.retweeted_status.user_id.ToString() + "加入用户机器人的用户队列。" );
                        else
                            Log( "用户" + status.retweeted_status.user_id.ToString() + "已在用户机器人的用户队列中。" );

                        if (queueUserForStatusRobot.Enqueue( status.retweeted_status.user_id ))
                            Log( "将用户" + status.retweeted_status.user_id.ToString() + "加入微博机器人的用户队列。" );
                        else
                            Log( "用户" + status.retweeted_status.user_id.ToString() + "已在微博机器人的用户队列中。" );
                    }
                }
                #endregion
                //日志
                Log("用户" + lCurrentUserID.ToString() + "的数据已爬取完毕。");
                //日志
                Log("调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + crawler.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + crawler.RemainingHits.ToString() + "次");
            }
        }

        public override void Initialize ()
        {
            //初始化相应变量
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;

            queueUserForUserRobot.Initialize();
            queueUserForStatusRobot.Initialize();
            queueStatus.Initialize();
        }
    }
}
