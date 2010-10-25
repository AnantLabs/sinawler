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
    class StatusRobot:RobotBase
    {
        private long lCommentUID = 0;    //评论人的UID
        private LinkedList<long> lstRetweetedStatus = new LinkedList<long>();   //转发微博ID
        private long lRetweetedUID = 0;     //转发微博的UID，用于传递给用户机器人

        public long CurrentSID
        { get { return lCurrentID; } }

        public long CurrentRetweetedUID
        { get { return lRetweetedUID; } }

        //构造函数，需要传入相应的新浪微博API和主界面
        public StatusRobot ( SinaApiService oAPI ):base(oAPI)
        {
            queueBuffer = new QueueBuffer( QueueBufferTarget.FOR_STATUS );
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
        }

        /// <summary>
        /// 开始爬行微博
        /// </summary>
        public void Start()
        {
            //不加载用户队列，完全依靠UserRobot传递过来
            while (lstWaitingID.Count == 0) Thread.Sleep( 1 );   //若队列为空，则等待
            long lStartUID = lstWaitingID.First.Value;
            //对队列无限循环爬行，直至有操作暂停或停止
            while(true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }
                //将队头取出
                long lCurrentUID = lstWaitingID.First.Value;
                lstWaitingID.RemoveFirst();
                //从数据库队列缓存中移入元素
                long lHead = queueBuffer.Dequeue();
                if (lHead > 0)
                    lstWaitingID.AddLast( lHead );
                #region 预处理
                if (lCurrentUID == lStartUID)  //说明经过一次循环迭代
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }

                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "开始爬行之前增加迭代次数...";
                    bwAsync.ReportProgress(0);
                    Thread.Sleep(50);

                    Status.NewIterate();
                    Comment.NewIterate();
                }

                #endregion                
                #region 用户微博信息
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }
                //日志
                strLog = DateTime.Now.ToString() + "  " + "获取数据库中用户" + lCurrentUID.ToString() + "最新一条微博的ID...";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
                //获取数据库中当前用户最新一条微博的ID
                long lLastStatusIDOf = Status.GetLastStatusIDOf( lCurrentUID );

                ///@2010-10-11
                ///考虑到中止爬行时可能会中断评论的保存，故此处先重新爬取最新一条微博的评论
                #region 爬取数据库中最新一条微博的评论
                if (lLastStatusIDOf > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }

                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "爬取微博" + lLastStatusIDOf.ToString() + "的评论...";
                    bwAsync.ReportProgress( 100 );
                    Thread.Sleep( 5 );
                    //爬取当前微博的评论
                    List<Comment> lstComment = crawler.GetCommentsOf( lLastStatusIDOf );
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "爬得" + lstComment.Count.ToString() + "条评论。";
                    bwAsync.ReportProgress( 100 );
                    Thread.Sleep( 5 );
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }

                    foreach (Comment comment in lstComment)
                    {
                        //Thread.Sleep( 5 );
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep( 50 );
                        }
                        if (!Comment.Exists( comment.comment_id ))
                        {
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "将评论" + comment.comment_id.ToString() + "存入数据库...";
                            bwAsync.ReportProgress( 100 );
                            Thread.Sleep( 5 );
                            comment.Add();

                            //将评论人加入队列——2010-10-11
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "将评论人" + comment.uid.ToString() + "加入队列...";
                            bwAsync.ReportProgress( 100 );
                            Thread.Sleep( 5 );
                            lCommentUID = comment.uid;
                            if (lstWaitingUID.Contains( lCommentUID ) || queueBuffer.Contains( lCommentUID ))
                            {
                                //日志
                                strLog = DateTime.Now.ToString() + "  " + "用户" + lCommentUID.ToString() + "已在队列中...";
                                bwAsync.ReportProgress( 100 );
                            }
                            else
                            {
                                //若内存中已达到上限，则使用数据库队列缓存
                                //否则使用数据库队列缓存
                                if (lstWaitingUID.Count < iQueueLength)
                                    lstWaitingUID.AddLast( lCommentUID );
                                else
                                    queueBuffer.Enqueue( lCommentUID );

                                //日志
                                strLog = DateTime.Now.ToString() + "  " + "将用户" + lCommentUID.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count.ToString() + "个用户，数据库队列中有" + queueBuffer.Count.ToString() + "个用户。";
                                strQueueInfo = DateTime.Now.ToString() + "  " + "微博机器人的内存队列中现有" + lstWaitingUID.Count + "个用户，数据库队列中现有" + queueBuffer.Count.ToString() + "个用户。";
                                bwAsync.ReportProgress( 100 );                                
                            }
                        }
                    }
                }
                #endregion

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬取用户" + lCurrentUID.ToString() + "的ID在" + lCurrentID.ToString() + "之后的微博...";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
                //爬取数据库中当前用户最新一条微博的ID之后的微博，存入数据库
                List<Status> lstStatus = crawler.GetStatusesOfSince( lCurrentUID, lLastStatusIDOf );
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬得" + lstStatus.Count.ToString() + "条微博。";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);

                foreach (Status status in lstStatus)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }
                    if (!Status.Exists( status.status_id ))
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "将微博" + lCurrentID.ToString() + "存入数据库...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep(50);
                        status.Add();
                    }
                    //若该微博有转发，将转发微博ID入队
                    if (status.retweeted_status_id > 0)
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "微博" + lCurrentID.ToString() + "有转发微博，将转发微博" + status.retweeted_status_id.ToString() + "加入队列等待爬取...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep(50);
                        lstRetweetedStatus.AddLast( status.retweeted_status_id );
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }

                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "爬取微博" + status.status_id.ToString() + "的评论...";
                    bwAsync.ReportProgress( 100 );
                    Thread.Sleep( 5 );
                    //爬取当前微博的评论
                    List<Comment> lstComment = crawler.GetCommentsOf( status.status_id );
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "爬得" + lstComment.Count.ToString() + "条评论。";
                    bwAsync.ReportProgress( 100 );
                    Thread.Sleep( 5 );

                    foreach (Comment comment in lstComment)
                    {
                        //Thread.Sleep( 5 );
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep( 50 );
                        }
                        if (!Comment.Exists( comment.comment_id ))
                        {
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "将评论" + comment.comment_id.ToString() + "存入数据库...";
                            bwAsync.ReportProgress( 100 );
                            Thread.Sleep( 5 );
                            comment.Add();

                            //将评论人加入队列——2010-10-11
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "将评论人" + comment.uid.ToString() + "加入队列...";
                            bwAsync.ReportProgress( 100 );
                            Thread.Sleep( 5 );
                            long lCommentUID = comment.uid;
                            if (lstWaitingUID.Contains( lCommentUID ) || queueBuffer.Contains( lCommentUID ))
                            {
                                //日志
                                strLog = DateTime.Now.ToString() + "  " + "用户" + lCommentUID.ToString() + "已在队列中...";
                                bwAsync.ReportProgress( 100 );
                            }
                            else
                            {
                                //若内存中已达到上限，则使用数据库队列缓存
                                //否则使用数据库队列缓存
                                if (lstWaitingUID.Count < iQueueLength)
                                    lstWaitingUID.AddLast( lCommentUID );
                                else
                                    queueBuffer.Enqueue( lCommentUID );

                                //日志
                                strLog = DateTime.Now.ToString() + "  " + "将用户" + lCommentUID.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count.ToString() + "个用户，数据库队列中有" + queueBuffer.Count.ToString() + "个用户。";
                                strQueueInfo = DateTime.Now.ToString() + "  " + "微博机器人的内存队列中现有" + lstWaitingUID.Count + "个用户，数据库队列中现有" + queueBuffer.Count.ToString() + "个用户。";
                                bwAsync.ReportProgress( 100 );                                
                            }
                            Thread.Sleep( 5 );
                        }
                    }
                }
                #endregion                
                #region 爬取获取的转发微博
                //日志
                strLog = DateTime.Now.ToString() + "  " + "所有"+lstStatus.Count.ToString()+"条微博爬取完毕，共获得"+lstRetweetedStatus.Count.ToString()+"条转发微博。";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
                if(lstRetweetedStatus.Count>0)
                {
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "开始爬取获得的" + lstRetweetedStatus.Count.ToString() + "条转发微博...";
                    bwAsync.ReportProgress(0);
                    Thread.Sleep(50);
                }
                while(lstRetweetedStatus.Count>0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }

                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + crawler.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + crawler.RemainingHits.ToString() + "次";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 50 );

                    lCurrentID = lstRetweetedStatus.First.Value;
                    lstRetweetedStatus.RemoveFirst();

                    Status status = crawler.GetStatus(lCurrentID);
                    if(status!=null)
                    {
                        //记录转发微博的UID
                        lRetweetedUID = status.uid;
                        if (!Status.Exists(lCurrentID))
                        {
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "将微博" + lCurrentID.ToString() + "存入数据库...";
                            bwAsync.ReportProgress(0);
                            Thread.Sleep(50);
                            status.Add();
                        }
                        //将该转发微博的UID入队——2010-10-18
                        long lRetweetUID = status.uid;
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "将微博的发布人" + lRetweetUID.ToString() + "加入队列...";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );

                        if (lstWaitingUID.Contains( lRetweetUID ) || queueBuffer.Contains( lRetweetUID ))
                        {
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "用户" + lRetweetUID.ToString() + "已在队列中...";
                            bwAsync.ReportProgress( 100 );
                        }
                        else
                        {
                            //若内存中已达到上限，则使用数据库队列缓存
                            //否则使用数据库队列缓存
                            if (lstWaitingUID.Count < iQueueLength)
                                lstWaitingUID.AddLast( lRetweetUID );
                            else
                                queueBuffer.Enqueue( lRetweetUID );

                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "将用户" + lRetweetUID.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count + "个用户，数据库队列中有" + queueBuffer.Count.ToString() + "个用户。";
                            strQueueInfo = DateTime.Now.ToString() + "  " + "微博机器人的内存队列中现有" + lstWaitingUID.Count + "个用户，数据库队列中现有" + queueBuffer.Count.ToString() + "个用户。";
                            bwAsync.ReportProgress( 100 );                            
                        }
                        Thread.Sleep( 5 );

                        //若该微博有转发，将转发微博ID入队
                        if (status.retweeted_status_id > 0)
                        {
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "微博" + lCurrentID.ToString() + "有转发微博，将转发微博" + status.retweeted_status_id.ToString() + "加入微博队列...";
                            bwAsync.ReportProgress(0);
                            Thread.Sleep(50);
                            lstRetweetedStatus.AddLast( status.retweeted_status_id );
                        }
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep( 50 );
                        }

                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "爬取微博" + lRetweetedStatusID.ToString() + "的评论...";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );
                        //爬取当前微博的评论
                        List<Comment> lstComment = crawler.GetCommentsOf( lRetweetedStatusID );
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "爬得" + lstComment.Count.ToString() + "条评论。";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );

                        foreach (Comment comment in lstComment)
                        {
                            //Thread.Sleep( 5 );
                            if (blnAsyncCancelled) return;
                            while (blnSuspending)
                            {
                                if (blnAsyncCancelled) return;
                                Thread.Sleep( 50 );
                            }
                            if (!Comment.Exists( comment.comment_id ))
                            {
                                //日志
                                strLog = DateTime.Now.ToString() + "  " + "将评论" + comment.comment_id.ToString() + "存入数据库...";
                                bwAsync.ReportProgress( 100 );
                                Thread.Sleep( 5 );
                                comment.Add();

                                //将评论人加入队列——2010-10-11
                                long lCommentUID = comment.uid;
                                //日志
                                strLog = DateTime.Now.ToString() + "  " + "将评论人" + lCommentUID.ToString() + "加入队列...";
                                bwAsync.ReportProgress( 100 );
                                Thread.Sleep( 5 );                                
                                if (lstWaitingUID.Contains( lCommentUID ) || queueBuffer.Contains( lCommentUID ))
                                {
                                    //日志
                                    strLog = DateTime.Now.ToString() + "  " + "用户" + lCommentUID.ToString() + "已在队列中...";
                                    bwAsync.ReportProgress( 100 );
                                }
                                else
                                {
                                    //若内存中已达到上限，则使用数据库队列缓存
                                    //否则使用数据库队列缓存
                                    if (lstWaitingUID.Count < iQueueLength)
                                        lstWaitingUID.AddLast( lCommentUID );
                                    else
                                        queueBuffer.Enqueue( lCommentUID );

                                    //日志
                                    strLog = DateTime.Now.ToString() + "  " + "将用户" + lCommentUID.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count + "个用户，数据库队列中有" + queueBuffer.Count.ToString() + "个用户。";
                                    strQueueInfo = DateTime.Now.ToString() + "  " + "微博机器人的内存队列中现有" + lstWaitingUID.Count + "个用户，数据库队列中现有" + queueBuffer.Count.ToString() + "个用户。";
                                    bwAsync.ReportProgress( 100 );                                    
                                }
                                Thread.Sleep( 5 );
                            }
                        }
                    }
                    lstRetweetedStatus.RemoveFirst();
                }
                #endregion
                //最后再将刚刚爬行完的UID加入队尾
                //日志
                strLog = DateTime.Now.ToString() + "  " + "用户" + lCurrentUID.ToString() + "的数据已爬取完毕，将其加入队尾...";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
                //若内存中已达到上限，则使用数据库队列缓存
                if (lstWaitingUID.Count < iQueueLength)
                    lstWaitingUID.AddLast( lCurrentUID );
                else
                    queueBuffer.Enqueue( lCurrentUID );

                //日志
                strLog = DateTime.Now.ToString() + "  " + "调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + crawler.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + crawler.RemainingHits.ToString() + "次";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
            }
        }

        public override void Initialize ()
        {
            //初始化相应变量
            blnAsyncCancelled = false;
            blnSuspending = false;
            if (lstWaitingUID != null) lstWaitingUID.Clear();

            //清空数据库队列缓存
            queueBuffer.Clear();
        }
    }
}
