using System;
using System.Collections.Generic;
using System.Text;
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
            AdjustFreq();
            SetCrawlerFreq();
            Log("The initial requesting interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s, " + api.RemainingIPHits.ToString() + " IP hits and " + api.RemainingUserHits.ToString() + " user hits left this hour.");
            User user;
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
                //lCurrentID = queueUserForUserInfoRobot.RollQueue();
                lCurrentID = queueUserForUserInfoRobot.FirstValue;
                
                //日志
                Log("Recording current UserID: " + lCurrentID.ToString()+"...");
                SysArg.SetCurrentID(lCurrentID, SysArgFor.USER_INFO);

                #region 用户基本信息
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }

                Log("Crawling information of User " + lCurrentID.ToString() + "...");
                user = crawler.GetUserInfo(lCurrentID);
                //日志
                AdjustFreq();
                SetCrawlerFreq();
                Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s, " + api.RemainingIPHits.ToString() + " IP hits and " + api.RemainingUserHits.ToString() + " user hits left this hour.");
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
                    if(InvalidUser.ExistInDB(lCurrentID))
                    {
                        //日志
                        Log("Removing User " + lCurrentID.ToString() + " from invalid users...");
                        InvalidUser.RemoveFromDB(lCurrentID);
                    }
                    //日志
                    Log( "The information of User " + lCurrentID.ToString() + " crawled." );
                    queueUserForUserInfoRobot.RollQueue();
                }
                else if(user==null) //用户不存在
                {
                    Log("Recording invalid User " + lCurrentID.ToString() + "...");
                    InvalidUser iu = new InvalidUser();
                    iu.user_id = lCurrentID;
                    iu.Add();

                    //将该用户ID从各个队列中去掉
                    Log("Removing invalid User " + lCurrentID.ToString() + " from all queues...");
                    queueUserForUserRelationRobot.Remove(lCurrentID);
                    queueUserForUserInfoRobot.Remove(lCurrentID);
                    if (GlobalPool.TagRobotEnabled)
                        queueUserForUserTagRobot.Remove(lCurrentID);
                    if (GlobalPool.StatusRobotEnabled)
                        queueUserForStatusRobot.Remove(lCurrentID);
                }
                else if (user.user_id == -1)   //forbidden
                {
                    int iSleepSeconds = GlobalPool.GetAPI(SysArgFor.USER_INFO).ResetTimeInSeconds;
                    Log("Service is forbidden now. I will wait for " + iSleepSeconds .ToString()+ "s to continue...");
                    for(int i=0;i<iSleepSeconds;i++)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(1000);
                    }
                }
                #endregion
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
