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
        private int iInitQueueLength = 100;          //初始队列长度

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        //构造函数，需要传入相应的新浪微博API和主界面
        public UserInfoRobot ()
            : base(SysArgFor.USER_INFO)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userInfo.log";
            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
        }

        /// <summary>
        /// 以指定的UserID为起点开始爬行
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ()
        {
            //获取上次中止处的用户ID并入队
            long lLastUID = SysArg.GetCurrentID(SysArgFor.USER_INFO);
            if (lLastUID > 0) queueUserForUserInfoRobot.Enqueue(lLastUID);
            while (queueUserForUserInfoRobot.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(GlobalPool.SleepMsForThread);   //若队列为空，则等待
            }
            Thread.Sleep(500);  //waiting that user relation robot update request limit data
            User user;

            SetCrawlerFreq();
            Log("The initial requesting interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString()+" requests left this hour.");

            //对队列循环爬行
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }
                //将队头取出
                lCurrentID = queueUserForUserInfoRobot.RollQueue();
                
                //日志
                Log("Recording current UserID: " + lCurrentID.ToString()+"...");
                SysArg.SetCurrentID( lCurrentID,SysArgFor.USER_INFO );

                #region 用户基本信息
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }

                Log("Crawling information of User " + lCurrentID.ToString() + "...");
                user = crawler.GetUserInfo(lCurrentID);
                if (user!=null && user.user_id > 0)
                {
                    //若数据库中不存在当前用户的基本信息，则爬取，加入数据库
                    if (!User.ExistInDB( lCurrentID ))
                    {
                        //日志
                        Log("Saving User " + lCurrentID.ToString() + " into database...");
                        user.Add();
                    }
                    else
                    {
                        //日志
                        Log("Updating the information of User " + lCurrentID.ToString() + "...");
                        user.Update();
                    }
                    //日志
                    Log( "The information of User " + lCurrentID.ToString() + " crawled." );
                }
                else if(user==null) //用户不存在
                {
                    //日志
                    Log( "User " + lCurrentID.ToString() + " not exists. Removing from all queues..." );
                    //将该用户ID从各个队列中去掉
                    queueUserForUserInfoRobot.Remove( lCurrentID );
                    queueUserForUserRelationRobot.Remove( lCurrentID );
                    queueUserForUserTagRobot.Remove( lCurrentID );
                    queueUserForStatusRobot.Remove( lCurrentID );

                    Log("User " + lCurrentID.ToString() + " not exists. Deleting related data...");
                    //Remove the data related from every table, except statuses and comments
                    User.Remove(lCurrentID);
                }
                #endregion

                AdjustFreq();
                //日志
                Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms." + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString()+" requests left this hour.");
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

        sealed protected override void AdjustFreq()
        {
            base.AdjustFreq();
            SetCrawlerFreq();
        }
    }
}
