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
    class CommentRobot : RobotBase
    {
        private UserQueue queueUserForUserInfoRobot;        //用户信息机器人使用的用户队列引用
        private UserQueue queueUserForUserRelationRobot;    //用户关系机器人使用的用户队列引用
        private UserQueue queueUserForUserTagRobot;         //用户标签机器人使用的用户队列引用
        private UserQueue queueUserForStatusRobot;          //微博机器人使用的用户队列引用
        private StatusQueue queueStatus;                    //微博队列引用
        
        //构造函数，需要传入相应的新浪微博API
        public CommentRobot ()
            : base(SysArgFor.COMMENT)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_comment.log";

            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
            queueStatus = GlobalPool.StatusQueue;
        }

        /// <summary>
        /// 开始爬行取微博评论
        /// </summary>
        public void Start()
        {
            //获取上次中止处的微博ID并入队
            long lLastStatusID = SysArg.GetCurrentID(SysArgFor.COMMENT);
            if (lLastStatusID > 0) queueStatus.Enqueue( lLastStatusID );
            while (queueStatus.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(GlobalPool.SleepMsForThread);   //若队列为空，则等待
            }

            AdjustRealFreq();
            SetCrawlerFreq();
            Log("The initial requesting interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString()+" requests left this hour.");

            //对队列无限循环爬行，直至有操作暂停或停止
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }

                //将队头取出
                lCurrentID = queueStatus.RollQueue();

                //日志
                Log("Recording current StatusID: " + lCurrentID.ToString()+"...");
                SysArg.SetCurrentID(lCurrentID,SysArgFor.COMMENT);

                #region 微博相应评论
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }

                //日志
                Log("Crawling the comments of Status " + lCurrentID.ToString() + "...");
                int iPage = 1;
                //爬取当前微博的评论
                LinkedList<Comment> lstComment = new LinkedList<Comment>();
                LinkedList<Comment> lstTemp=new LinkedList<Comment>();
                lstTemp = crawler.GetCommentsOf(lCurrentID, iPage);
                //日志
                AdjustFreq();
                SetCrawlerFreq();
                Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString() + " requests left this hour.");
                while (lstTemp.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(GlobalPool.SleepMsForThread);
                    }
                    while (lstTemp.Count > 0)
                    {
                        lstComment.AddLast(lstTemp.First.Value);
                        lstTemp.RemoveFirst();
                    }
                    iPage++;
                    lstTemp = crawler.GetCommentsOf(lCurrentID, iPage);
                    //日志
                    AdjustFreq();
                    SetCrawlerFreq();
                    Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString() + " requests left this hour.");
                }
                //日志
                Log(lstComment.Count.ToString() + " comments of Status " + lCurrentID.ToString() + " crawled.");
                Comment comment;
                while(lstComment.Count>0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(GlobalPool.SleepMsForThread);
                    }
                    comment = lstComment.First.Value;
                    if (!Comment.Exists( comment.comment_id ))
                    {
                        //日志
                        Log( "Saving Comment " + comment.comment_id.ToString() + " into database..." );
                        comment.Add();
                    }

                    if (queueUserForUserRelationRobot.Enqueue(comment.user.user_id))
                        Log("Adding Commenter " + comment.user.user_id.ToString() + " to the user queue of User Relation Robot...");
                    if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue(comment.user.user_id))
                        Log("Adding Commenter " + comment.user.user_id.ToString() + " to the user queue of User Information Robot...");
                    if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue(comment.user.user_id))
                        Log("Adding Commenter " + comment.user.user_id.ToString() + " to the user queue of User Tag Robot...");
                    if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue(comment.user.user_id))
                        Log("Adding Commenter " + comment.user.user_id.ToString() + " to the user queue of Status Robot...");
                    if (!User.ExistInDB(comment.user.user_id))
                    {
                        Log("Saving Commenter " + comment.user.user_id.ToString() + " into database...");
                        comment.user.Add();
                    }

                    lstComment.RemoveFirst();
                }
                #endregion
                //日志
                Log("Comments of Status " + lCurrentID.ToString() + " crawled.");
            }
        }

        public override void Initialize()
        {
            //初始化相应变量
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueStatus.Initialize();
        }
    }
}
