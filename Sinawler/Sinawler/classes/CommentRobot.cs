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
        private UserQueue queueUserForUserInfoRobot;          //用户信息机器人使用的用户队列引用
        private UserQueue queueUserForUserRelationRobot;      //用户关系机器人使用的用户队列引用
        private UserQueue queueUserForUserTagRobot;           //用户标签机器人使用的用户队列引用
        private UserQueue queueUserForStatusRobot;            //微博机器人使用的用户队列引用
        private StatusQueue queueStatus;        //微博队列引用

        //构造函数，需要传入相应的新浪微博API
        public CommentRobot ( SinaApiService oAPI, UserQueue qUserForUserInfoRobot, UserQueue qUserForUserRelationRobot, UserQueue qUserForUserTagRobot, UserQueue qUserForStatusRobot, StatusQueue qStatus )
            : base( oAPI )
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_comment.log";

            queueUserForUserInfoRobot = qUserForUserInfoRobot;
            queueUserForUserRelationRobot = qUserForUserRelationRobot;
            queueUserForUserTagRobot = qUserForUserTagRobot;
            queueUserForStatusRobot = qUserForStatusRobot;
            queueStatus = qStatus;
        }

        /// <summary>
        /// 开始爬行取微博评论
        /// </summary>
        public void Start()
        {
            //获取上次中止处的微博ID并入队
            long lLastStatusID = SysArg.GetCurrentStatusID();
            if (lLastStatusID > 0) queueStatus.Enqueue( lLastStatusID );
            while (queueStatus.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(50);   //若队列为空，则等待
            }
            long lStartSID = queueStatus.FirstValue;
            long lCurrentSID = 0;
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
                lCurrentSID = queueStatus.RollQueue();

                #region 预处理
                //if (lCurrentSID == lStartSID)  //说明经过一次循环迭代
                //{
                //    if (blnAsyncCancelled) return;
                //    while (blnSuspending)
                //    {
                //        if (blnAsyncCancelled) return;
                //        Thread.Sleep(50);
                //    }

                //    //日志
                //    Log("开始爬行之前增加迭代次数...");
                //    Comment.NewIterate();
                //}
                //日志
                Log( "记录当前微博ID：" + lCurrentSID.ToString() );
                SysArg.SetCurrentStatusID( lCurrentSID );
                #endregion
                #region 微博相应评论
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }

                //日志
                Log("爬取微博" + lCurrentSID.ToString() + "的评论...");
                //爬取当前微博的评论
                LinkedList<Comment> lstComment = crawler.GetCommentsOf(lCurrentSID);
                //日志
                Log("爬得微博"+lCurrentSID.ToString()+"的" + lstComment.Count.ToString() + "条评论。");
                Comment comment;
                while(lstComment.Count>0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    comment = lstComment.First.Value;
                    if (!Comment.Exists( comment.comment_id ))
                    {
                        //日志
                        Log( "将评论" + comment.comment_id.ToString() + "存入数据库..." );
                        comment.Add();
                    }

                    if (queueUserForUserInfoRobot.Enqueue( comment.user_id ))
                        Log( "将评论人" + comment.user_id.ToString() + "加入用户信息机器人的用户队列。" );
                    if (queueUserForUserRelationRobot.Enqueue( comment.user_id ))
                        Log( "将评论人" + comment.user_id.ToString() + "加入用户关系机器人的用户队列。" );
                    if (queueUserForUserTagRobot.Enqueue( comment.user_id ))
                        Log( "将评论人" + comment.user_id.ToString() + "加入用户标签机器人的用户队列。" );
                    if (queueUserForStatusRobot.Enqueue( comment.user_id ))
                        Log( "将评论人" + comment.user_id.ToString() + "加入微博机器人的用户队列。" );

                    lstComment.RemoveFirst();
                }
                #endregion
                //最后再将刚刚爬行完的StatusID加入队尾
                //日志
                Log("微博" + lCurrentSID.ToString() + "的评论已爬取完毕。");
                //日志
                AdjustFreq();
                Log("调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + crawler.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + crawler.RemainingHits.ToString() + "次");
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
