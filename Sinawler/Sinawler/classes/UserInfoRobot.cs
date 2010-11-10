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
    class UserInfoRobot:RobotBase
    {
        private UserQueue queueUserForUserInfoRobot;            //用户信息机器人使用的用户队列引用
        private UserQueue queueUserForUserRelationRobot;        //用户关系机器人使用的用户队列引用
        private UserQueue queueUserForStatusRobot;          //微博机器人使用的用户队列引用
        private int iInitQueueLength = 100;          //初始队列长度

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        //构造函数，需要传入相应的新浪微博API和主界面
        public UserInfoRobot ( SinaApiService oAPI, UserQueue qUserForUserInfoRobot, UserQueue qUserForUserRelationRobot, UserQueue qUserForStatusRobot )
            : base( oAPI )
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userInfo.log";
            queueUserForUserInfoRobot = qUserForUserInfoRobot;
            queueUserForUserRelationRobot = qUserForUserRelationRobot;
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
            queueUserForUserInfoRobot.Enqueue( lStartUserID );
            queueUserForUserRelationRobot.Enqueue( lStartUserID );
            queueUserForStatusRobot.Enqueue( lStartUserID );

            #region 预加载队列
            //根据选项，选择加载用户队列的方法
            DataTable dtUserID=new DataTable();

            switch (queueUserForUserInfoRobot.PreLoadQueue)
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
                    if (queueUserForUserInfoRobot.Enqueue( lUserID ))
                        //日志
                        Log( "将用户" + lUserID.ToString() + "加入用户信息机器人的用户队列。进度：" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%" );
                    if (queueUserForUserRelationRobot.Enqueue( lUserID ))
                        //日志
                        Log( "将用户" + lUserID.ToString() + "加入用户关系机器人的用户队列。进度：" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%" );
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
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //将队头取出
                lCurrentID = queueUserForUserInfoRobot.RollQueue();
                
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
                }
                //日志
                Log("记录当前用户ID：" + lCurrentID.ToString());
                SysArg.SetCurrentUserIDForUserInfo( lCurrentID );
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
                #endregion
                //最后再将刚刚爬行完的UserID加入队尾
                //日志
                Log("用户" + lCurrentID.ToString() + "的基本信息已爬取完毕。");
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
            queueUserForUserInfoRobot.Initialize();
        }
    }
}
