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
    class UserRobot:RobotBase
    {
        private UserQueue queueUserForUserRobot;            //用户机器人使用的用户队列引用
        private UserQueue queueUserForStatusRobot;          //微博机器人使用的用户队列引用
        private int iInitQueueLength = 100;          //初始队列长度
        private long lQueueBufferFirst = 0;   //用于记录获取的关注用户列表、粉丝用户列表的队头值

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        //构造函数，需要传入相应的新浪微博API和主界面
        public UserRobot ( SinaApiService oAPI, UserQueue qUserForUserRobot, UserQueue qUserForStatusRobot ):base(oAPI)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_user.log";
            queueUserForUserRobot = qUserForUserRobot;
            queueUserForStatusRobot = qUserForStatusRobot;
        }

        /// <summary>
        /// 以指定的UserID为起点开始爬行
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ( long lStartUserID )
        {
            if (lStartUserID == 0) return;

            User user;

            //将起始UserID入队
            queueUserForUserRobot.Enqueue( lStartUserID );
            queueUserForStatusRobot.Enqueue( lStartUserID );

            #region 预加载队列
            //根据选项，选择加载用户队列的方法
            DataTable dtUserID=new DataTable();

            switch (queueUserForUserRobot.PreLoadQueue)
            {
                case EnumPreLoadQueue.PRELOAD_USER_ID:
                    //日志
                    Log("获取已爬取数据的用户的ID，并加入内存队列...");
                    dtUserID = User.GetCrawedUserIDTable();
                    break;
                case EnumPreLoadQueue.PRELOAD_ALL_USER_ID:
                    //日志
                    Log("获取数据库中所有用户的ID，并加入内存队列...");
                    dtUserID = UserRelation.GetAllUserIDTable();
                    break;
            }

            if (dtUserID != null)
            {
                iInitQueueLength = dtUserID.Rows.Count;
                long lUserID;
                int i;
                for (i = 0; i < dtUserID.Rows.Count; i++)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    lUserID = Convert.ToInt64( dtUserID.Rows[i]["user_id"] );
                    if (queueUserForUserRobot.Enqueue( lUserID ))
                        //日志
                        Log( "将用户" + lUserID.ToString() + "加入用户机器人的用户队列。进度：" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%" );
                    if (queueUserForStatusRobot.Enqueue( lUserID ))
                        //日志
                        Log( "将用户" + lUserID.ToString() + "加入微博机器人的用户队列。进度：" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%" );
                }
                dtUserID.Dispose();
                //日志
                Log( "预加载用户队列完成。" );
            }
            #endregion
            
            lCurrentID = lStartUserID;
            //对队列循环爬行
            while (queueUserForUserRobot.Count > 0)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //将队头取出
                lCurrentID = queueUserForUserRobot.RollQueue();
                
                #region 预处理
                if (lCurrentID == lStartUserID)  //说明经过一次循环迭代
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }

                    //日志
                    Log("开始爬行之前增加迭代次数...");

                    User.NewIterate();
                    UserRelation.NewIterate();
                }
                //日志
                Log("记录当前用户ID：" + lCurrentID.ToString());
                SysArg.SetCurrentUserID( lCurrentID );
                #endregion
                #region 用户基本信息
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                Log( "爬取用户" + lCurrentID.ToString() +"的基本信息...");
                user = crawler.GetUserInfo( lCurrentID );
                if (user.user_id > 0)
                {
                    //若数据库中不存在当前用户的基本信息，则爬取，加入数据库
                    if (!User.Exists( lCurrentID ))
                    {
                        //日志
                        Log("将用户" + lCurrentID.ToString() + "存入数据库...");
                        user.Add();
                    }
                    else
                    {
                        //日志
                        Log("更新用户" + lCurrentID.ToString() + "的数据...");
                        user.Update();
                    }
                }

                //日志
                Log("调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + crawler.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + crawler.RemainingHits.ToString() + "次");
                #endregion
                #region 用户关注列表
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //日志                
                Log("爬取用户" + lCurrentID.ToString() + "关注用户ID列表...");
                //爬取当前用户的关注的用户ID，记录关系，加入队列
                LinkedList<long> lstBuffer = crawler.GetFriendsOf( lCurrentID, -1 );
                //日志
                Log("爬得" + lstBuffer.Count.ToString() + "位关注用户。");

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    lQueueBufferFirst = lstBuffer.First.Value;
                    //若不存在有效关系，增加
                    if (!UserRelation.Exists( lCurrentID, lQueueBufferFirst ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(50);
                        }
                        //日志
                        Log( "记录用户" + lCurrentID.ToString() + "关注用户" + lQueueBufferFirst.ToString() + "..." );
                        UserRelation ur = new UserRelation();
                        ur.source_user_id = lCurrentID;
                        ur.target_user_id = lstBuffer.First.Value;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    //加入队列
                    if (!queueUserForUserRobot.Enqueue( lQueueBufferFirst ))
                    {
                        //日志
                        Log( "用户" + lQueueBufferFirst.ToString() + "已在用户机器人的用户队列中..." );
                    }
                    else
                    {
                        //日志
                        Log( "将用户" + lQueueBufferFirst.ToString() + "加入用户机器人的用户队列。" );
                    }
                    if (!queueUserForStatusRobot.Enqueue( lQueueBufferFirst ))
                    {
                        //日志
                        Log( "用户" + lQueueBufferFirst.ToString() + "已在微博机器人的用户队列中..." );
                    }
                    else
                    {
                        //日志
                        Log( "将用户" + lQueueBufferFirst.ToString() + "加入微博机器人的用户队列。" );
                    }
                    lstBuffer.RemoveFirst();
                }

                //日志
                Log("调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + crawler.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + crawler.RemainingHits.ToString() + "次");
                #endregion
                #region 用户粉丝列表
                //爬取当前用户的粉丝的ID，记录关系，加入队列
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //日志
                Log("爬取用户" + lCurrentID.ToString() + "的粉丝用户ID列表...");
                lstBuffer = crawler.GetFollowersOf( lCurrentID, -1 );
                //日志
                Log("爬得" + lstBuffer.Count.ToString() + "位粉丝。");

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    lQueueBufferFirst = lstBuffer.First.Value;
                    //若不存在有效关系，增加
                    if (!UserRelation.Exists( lQueueBufferFirst, lCurrentID ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(50);
                        }
                        //日志
                        Log( "记录用户" + lQueueBufferFirst.ToString() + "关注用户" + lCurrentID.ToString() + "..." );
                        UserRelation ur = new UserRelation();
                        ur.source_user_id = lstBuffer.First.Value;
                        ur.target_user_id = lCurrentID;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    //加入队列
                    if (!queueUserForUserRobot.Enqueue( lQueueBufferFirst ))
                    {
                        //日志
                        Log( "用户" + lQueueBufferFirst.ToString() + "已在用户机器人的用户队列中。" );
                    }
                    else
                    {
                        //日志
                        Log( "将用户" + lQueueBufferFirst.ToString() + "加入用户机器人的用户队列。" );
                    }
                    if (!queueUserForStatusRobot.Enqueue( lQueueBufferFirst ))
                    {
                        //日志
                        Log( "用户" + lQueueBufferFirst.ToString() + "已在微博机器人的用户队列中。" );
                    }
                    else
                    {
                        //日志
                        Log( "将用户" + lQueueBufferFirst.ToString() + "加入微博机器人的用户队列。" );
                    }
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //最后再将刚刚爬行完的UserID加入队尾
                //日志
                Log("用户" + lCurrentID.ToString() + "的数据已爬取完毕。");
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
        }
    }
}
