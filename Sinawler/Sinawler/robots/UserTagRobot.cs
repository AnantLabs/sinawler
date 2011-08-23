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
    class UserTagRobot : RobotBase
    {
        private UserQueue queueUserForUserTagRobot;        //用户标签机器人使用的用户队列引用
        private UserQueue queueUserForUserInfoRobot;        //用户信息机器人使用的用户队列引用
        private UserQueue queueUserForUserRelationRobot;    //用户关系机器人使用的用户队列引用
        private UserQueue queueUserForStatusRobot;          //微博机器人使用的用户队列引用

        //构造函数，需要传入相应的新浪微博API和主界面
        public UserTagRobot()
            : base(SysArgFor.USER_TAG)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_tag.log";
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
            long lLastUID = SysArg.GetCurrentID(SysArgFor.USER_TAG);
            if (lLastUID > 0) queueUserForUserTagRobot.Enqueue(lLastUID);
            while (queueUserForUserTagRobot.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(GlobalPool.SleepMsForThread);   //若队列为空，则等待
            }
            Thread.Sleep(500);  //waiting that user relation robot update request limit data

            SetCrawlerFreq();
            Log("The initial requesting interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString()+" requests left this hour.");

            //对队列无限循环爬行，直至有操作暂停或停止
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( GlobalPool.SleepMsForThread );
                }

                //将队头取出
                lCurrentID = queueUserForUserTagRobot.RollQueue();
                
                //日志
                Log( "Recording current UserID: " + lCurrentID.ToString()+"..." );
                SysArg.SetCurrentID( lCurrentID, SysArgFor.USER_TAG );

                #region 用户标签信息
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( GlobalPool.SleepMsForThread );
                }

                //日志
                Log( "Crawling tags of User " + lCurrentID.ToString() + "..." );
                LinkedList<Tag> lstTag = crawler.GetTagsOf( lCurrentID );
                //日志
                Log( lstTag.Count.ToString() + " tags crawled." );

                while (lstTag.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( GlobalPool.SleepMsForThread );
                    }
                    Tag tag = lstTag.First.Value;
                    if (!Tag.Exists( tag.tag_id ))
                    {
                        //日志
                        Log( "Saving Tag " + tag.tag_id.ToString() + " into database..." );
                        tag.Add();
                    }
                    else
                        //日志
                        Log( "Tag " + tag.tag_id.ToString() + " exists." );

                    if (!UserTag.Exists( lCurrentID, tag.tag_id ))
                    {
                        //日志
                        Log( "Recording User " + lCurrentID.ToString() + " has Tag " + tag.tag_id.ToString() + "..." );
                        UserTag user_tag = new UserTag();
                        user_tag.user_id = lCurrentID;
                        user_tag.tag_id = tag.tag_id;
                        user_tag.Add();
                    }
                    else
                        //日志
                        Log( "Tag " + tag.tag_id.ToString() + " of User "+ lCurrentID.ToString() + " exists." );

                    lstTag.RemoveFirst();
                }
                #endregion
                //日志
                Log( "Tags of User " + lCurrentID.ToString() + " crawled." );
                //日志
                AdjustFreq();
                Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms." + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString()+" requests left this hour.");
            }
        }

        public override void Initialize ()
        {
            //初始化相应变量
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueUserForUserTagRobot.Initialize();
        }

        sealed protected override void AdjustFreq()
        {
            base.AdjustFreq();
            SetCrawlerFreq();
        }
    }
}
