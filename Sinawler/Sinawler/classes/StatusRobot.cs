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
        public StatusRobot ()
            : base()
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
            oUserBuffer = GlobalPool.UserBuffer;
        }

        /// <summary>
        /// 处理并保存爬取的微博数据
        /// </summary>
        private void SaveStatus ( Status status )
        {
            lCurrentID = status.status_id;
            if (!Status.Exists( lCurrentID ))
            {
                //日志
                Log( "将微博" + lCurrentID.ToString() + "存入数据库..." );
                status.Add();
            }

            if (queueStatus.Enqueue( lCurrentID ))
                Log( "将微博" + lCurrentID.ToString() + "加入微博队列。" );

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
                Log( "微博" + lCurrentID.ToString() + "有转发微博，将转发微博" + status.retweeted_status.status_id.ToString() + "存入数据库..." );

                if (!Status.Exists( status.retweeted_status.status_id ))
                {
                    status.retweeted_status.Add();

                    //日志
                    Log( "转发微博" + status.retweeted_status.status_id.ToString() + "已保存。" );
                }
                else
                {
                    //日志
                    Log( "转发微博" + status.retweeted_status.status_id.ToString() + "已存在。" );
                }

                if (queueStatus.Enqueue( status.retweeted_status.status_id ))
                    Log( "将转发微博" + status.retweeted_status.status_id.ToString() + "加入微博队列。" );

                if (queueUserForUserRelationRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("将用户" + status.retweeted_status.user.user_id.ToString() + "加入用户关系机器人的用户队列。");
                if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log( "将用户" + status.retweeted_status.user.user_id.ToString() + "加入用户信息机器人的用户队列。" );
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
        public void Start ()
        {
            while (queueUserForStatusRobot.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep( 50 );   //若队列为空，则等待
            }
            long lStartUserID = queueUserForStatusRobot.FirstValue;
            long lCurrentUserID = 0;
            //对队列无限循环爬行，直至有操作暂停或停止
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }

                //将队头取出
                lCurrentUserID = queueUserForStatusRobot.RollQueue();

                #region 用户微博信息
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
                //日志
                Log( "获取数据库中用户" + lCurrentUserID.ToString() + "最新一条微博的ID..." );
                //获取数据库中当前用户最新一条微博的ID
                lCurrentID = Status.GetLastStatusIDOf( lCurrentUserID );

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }

                Status status;
                #region 微博在页面上的ID不是实际的ID，暂时搁置
                //if(lCurrentID==0)   //通过web抓取所有微博
                //{
                //    //日志
                //    Log( "爬取用户" + lCurrentUserID.ToString()+"的基本信息..." );
                //    User user = crawler.GetUserInfo( lCurrentUserID );  //主要是为了获取用户的微博数量，以供参考
                //    //日志
                //    Log( "爬取用户" + lCurrentUserID.ToString() + "的所有微博ID列表..." );
                //    LinkedList<long> lstStatusID=crawler.GetStatusesByWeb(lCurrentUserID,user.statuses_count);
                //    //日志
                //    Log( "爬得" + lstStatusID.Count.ToString() + "条微博。" );

                //    long lStatusID = 0;
                //    while(lstStatusID.Count>0)
                //    {
                //        if (blnAsyncCancelled) return;
                //        while (blnSuspending)
                //        {
                //            if (blnAsyncCancelled) return;
                //            Thread.Sleep( 50 );
                //        }

                //        lStatusID = lstStatusID.First.Value;
                //        //日志
                //        Log( "爬取微博" + lStatusID.ToString() + "的内容..." );
                //        status = crawler.GetStatus( lStatusID );
                //        SaveStatus( status );
                //        lstStatusID.RemoveFirst();                        
                //    }
                //}
                //else
                //{
                #endregion
                #region 后续微博
                //日志
                Log( "爬取用户" + lCurrentUserID.ToString() + "的ID在" + lCurrentID.ToString() + "之后的微博..." );
                //爬取数据库中当前用户最新一条微博的ID之后的微博，存入数据库
                LinkedList<Status> lstStatus = crawler.GetStatusesOfSince( lCurrentUserID, lCurrentID );
                //日志
                Log( "爬得" + lstStatus.Count.ToString() + "条微博。" );

                while (lstStatus.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    status = lstStatus.First.Value;
                    SaveStatus( status );
                    lstStatus.RemoveFirst();
                }
                #endregion
                //}
                #endregion
                //日志
                Log( "用户" + lCurrentUserID.ToString() + "的微博数据已爬取完毕。" );
                //日志
                AdjustFreq();
                Log( "调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + crawler.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + crawler.RemainingHits.ToString() + "次" );
            }
        }

        public override void Initialize ()
        {
            //初始化相应变量
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueUserForStatusRobot.Initialize();
        }
    }
}
