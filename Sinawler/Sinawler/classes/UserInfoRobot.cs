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
        private UserQueue queueUserForUserTagRobot;             //用户标签机器人使用的用户队列引用
        private UserQueue queueUserForStatusRobot;          //微博机器人使用的用户队列引用
        private UserBuffer oUserBuffer;               //the buffer queue of users
        private int iInitQueueLength = 100;          //初始队列长度

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        //构造函数，需要传入相应的新浪微博API和主界面
        public UserInfoRobot ()
            : base()
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userInfo.log";
            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
            oUserBuffer = GlobalPool.UserBuffer;
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
            queueUserForUserTagRobot.Enqueue( lStartUserID );
            queueUserForStatusRobot.Enqueue( lStartUserID );
            
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
                
                //日志
                Log("记录当前用户ID：" + lCurrentID.ToString());
                SysArg.SetCurrentUserIDForUserInfo( lCurrentID );

                #region 用户基本信息
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }

                if (oUserBuffer.UserExists(lCurrentID))   //current user exists in the user buffer
                {
                    Log("用户" + lCurrentID.ToString() + "已在缓冲池中，将直接获取其信息...");
                    user = oUserBuffer.GetUser(lCurrentID);
                    oUserBuffer.Remove(user);
                }
                else
                {
                    Log("爬取用户" + lCurrentID.ToString() + "的基本信息...");
                    user = crawler.GetUserInfo(lCurrentID);
                }
                if (user!=null && user.user_id > 0)
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
                    //日志
                    Log( "用户" + lCurrentID.ToString() + "的基本信息已爬取完毕。" );
                }
                else if(user==null) //用户不存在
                {
                    //日志
                    Log( "用户" + lCurrentID.ToString() + "不存在，将其从队列中移除..." );
                    //将该用户ID从各个队列中去掉
                    queueUserForUserInfoRobot.Remove( lCurrentID );
                    queueUserForUserRelationRobot.Remove( lCurrentID );
                    queueUserForUserTagRobot.Remove( lCurrentID );
                    queueUserForStatusRobot.Remove( lCurrentID );
                }
                #endregion
                
                //日志
                AdjustFreq();
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
