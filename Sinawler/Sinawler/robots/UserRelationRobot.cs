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
using System.Xml;

namespace Sinawler
{
    class UserRelationRobot:RobotBase
    {
        private UserQueue queueUserForUserInfoRobot;        //用户信息机器人使用的用户队列引用
        private UserQueue queueUserForUserRelationRobot;    //用户关系机器人使用的用户队列引用
        private UserQueue queueUserForUserTagRobot;         //用户标签机器人使用的用户队列引用
        private UserQueue queueUserForStatusRobot;          //微博机器人使用的用户队列引用
        private long lQueueBufferFirst = 0;   //用于记录获取的关注用户列表、粉丝用户列表的队头值

        //构造函数，需要传入相应的新浪微博API和主界面
        public UserRelationRobot ()
            : base()
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userRelation.log";
            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
        }

        /// <summary>
        /// 以指定的UserID为起点开始爬行
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ( long lStartUserID )
        {
            if (lStartUserID == 0) return;

            AdjustFreq();
            Log("初始请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + GlobalPool.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + GlobalPool.RemainingHits.ToString() + "次");

            //将起始UserID入队
            queueUserForUserRelationRobot.Enqueue( lStartUserID );
            queueUserForUserInfoRobot.Enqueue( lStartUserID );
            queueUserForUserTagRobot.Enqueue( lStartUserID );
            queueUserForStatusRobot.Enqueue( lStartUserID );
            lCurrentID = lStartUserID;

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
                lCurrentID = queueUserForUserRelationRobot.RollQueue();
                
                //日志
                Log("记录当前用户ID：" + lCurrentID.ToString());
                SysArg.SetCurrentUserIDForUserRelation( lCurrentID );

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
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    //加入队列
                    if (queueUserForUserRelationRobot.Enqueue(lQueueBufferFirst))
                        //日志
                        Log("将用户" + lQueueBufferFirst.ToString() + "加入用户关系机器人的用户队列。");
                    if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue( lQueueBufferFirst ))
                        //日志
                        Log( "将用户" + lQueueBufferFirst.ToString() + "加入用户信息机器人的用户队列。" );                    
                    if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue( lQueueBufferFirst ))
                        //日志
                        Log( "将用户" + lQueueBufferFirst.ToString() + "加入用户标签机器人的用户队列。" );
                    if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue( lQueueBufferFirst ))
                        //日志
                        Log( "将用户" + lQueueBufferFirst.ToString() + "加入微博机器人的用户队列。" );
                    lstBuffer.RemoveFirst();
                }

                //日志
                AdjustFreq();
                Log("调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + GlobalPool.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + GlobalPool.RemainingHits.ToString() + "次");
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
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    //加入队列
                    if (queueUserForUserRelationRobot.Enqueue(lQueueBufferFirst))
                        //日志
                        Log("将用户" + lQueueBufferFirst.ToString() + "加入用户关系机器人的用户队列。");
                    if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue( lQueueBufferFirst ))
                        //日志
                        Log( "将用户" + lQueueBufferFirst.ToString() + "加入用户信息机器人的用户队列。" );
                    if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue( lQueueBufferFirst ))
                        //日志
                        Log( "将用户" + lQueueBufferFirst.ToString() + "加入用户标签机器人的用户队列。" );
                    if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue( lQueueBufferFirst ))
                        //日志
                        Log( "将用户" + lQueueBufferFirst.ToString() + "加入微博机器人的用户队列。" );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //最后再将刚刚爬行完的UserID加入队尾
                //日志
                Log("用户" + lCurrentID.ToString() + "的关系已爬取完毕。");
                //日志
                AdjustFreq();
                Log("调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + GlobalPool.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + GlobalPool.RemainingHits.ToString() + "次");
            }
        }

        public override void Initialize ()
        {
            //初始化相应变量
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueUserForUserRelationRobot.Initialize();
        }

        sealed protected override void AdjustFreq()
        {
            string strResult = api.check_hits_limit();
            while (strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null)
            {
                System.Threading.Thread.Sleep(100);
                strResult = api.check_hits_limit();
            }
            //if (strResult == null) return;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strResult);

            int iResetTimeInSeconds = Convert.ToInt32(xmlDoc.GetElementsByTagName("reset-time-in-seconds")[0].InnerText);
            int iRemainingHits = Convert.ToInt32(xmlDoc.GetElementsByTagName("remaining-hits")[0].InnerText);

            //若已无剩余次数，直接等待剩余时间
            if (iRemainingHits == 0)
            {
                crawler.SleepTime = iResetTimeInSeconds * 1000;
            }
            else
            {
                //计算
                int iSleep = Convert.ToInt32(iResetTimeInSeconds * 1000 / iRemainingHits);
                //if (iSleep <= 0) iSleep = 1;
                if (iSleep < 500) iSleep = 500; //sleep at least 500ms

                crawler.SleepTime = iSleep;
            }

            GlobalPool.LimitUpdateTime = DateTime.Now;
            GlobalPool.RemainingHits = iRemainingHits;
            GlobalPool.ResetTimeInSeconds = iResetTimeInSeconds;
        }
    }
}
