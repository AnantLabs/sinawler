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

        //构造函数，需要传入相应的新浪微博API和主界面
        public StatusRobot ( SinaApiService oAPI ):base(oAPI)
        {
            queueBuffer = new QueueBuffer( QueueBufferTarget.FOR_STATUS );
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
        }

        /// <summary>
        /// 以指定的UID为起点开始爬行
        /// </summary>
        /// <param name="lUid"></param>
        public override void Start ( long lStartUID )
        {
            if (lStartUID == 0) return;

            //根据选项，选择加载用户队列的方法
            DataTable dtUID=new DataTable();

            switch (iPreLoadQueue)
            {
                case (int)EnumPreLoadQueue.PRELOAD_UID:
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "获取已爬取数据的用户的ID，并加入内存队列...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 5 );
                    dtUID = User.GetCrawedUIDTable();
                    break;
                case (int)EnumPreLoadQueue.PRELOAD_ALL_UID:
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "获取数据库中所有用户的ID，并加入内存队列...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 5 );
                    dtUID = UserRelation.GetAllUIDTable();
                    break;
            }

            if (dtUID != null)
            {
                iInitQueueLength = dtUID.Rows.Count;
                long lUID;
                int i;
                for (i = 0; i < dtUID.Rows.Count && lstWaitingUID.Count < iQueueLength; i++)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    lUID = Convert.ToInt64( dtUID.Rows[i]["uid"] );
                    if (!lstWaitingUID.Contains( lUID ))
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "将用户" + lUID.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count + "个用户；数据库队列中有" + queueBuffer.Count.ToString() + "个用户。进度：" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%";
                        bwAsync.ReportProgress( 5 );
                        Thread.Sleep( 5 );
                        lstWaitingUID.AddLast( lUID );
                    }
                }

                //若还有剩余，将多余部分放入数据库队列缓存
                while (i < dtUID.Rows.Count)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    lUID = Convert.ToInt64( dtUID.Rows[i]["uid"] );
                    int iLengthInDB = queueBuffer.Count;
                    if (!queueBuffer.Contains( lUID ))
                    {
                        queueBuffer.Enqueue( lUID );
                        ++iLengthInDB;
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "内存队列已满，将用户" + lUID.ToString() + "加入数据库队列；数据库队列中有" + iLengthInDB.ToString() + "个用户。进度：" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%";
                        bwAsync.ReportProgress( 5 );
                        Thread.Sleep( 5 );
                    }
                    i++;
                }
            }
            dtUID.Dispose();

            //从队列中去掉当前UID
            lstWaitingUID.Remove( lStartUID );
            //将当前UID加到队头
            lstWaitingUID.AddFirst( lStartUID );
            //日志
            strLog = DateTime.Now.ToString() + "  " + "初始化用户队列完成。";
            bwAsync.ReportProgress( 100 );
            Thread.Sleep( 5 );
            long lCurrentUID = lStartUID;
            //对队列循环爬行
            while (lstWaitingUID.Count > 0)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
                blnOneUserCompleted = false;    //开始新的用户的迭代
                //将队头取出
                lCurrentUID = lstWaitingUID.First.Value;
                lstWaitingUID.RemoveFirst();
                //从数据库队列缓存中移入元素
                long lHead = queueBuffer.Dequeue();
                if (lHead > 0)
                    lstWaitingUID.AddLast( lHead );
                #region 预处理
                if (lCurrentUID == lStartUID)  //说明经过一次循环迭代
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }

                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "开始爬行之前增加迭代次数...";
                    bwAsync.ReportProgress( 100 );
                    Thread.Sleep( 5 );

                    Status.NewIterate();
                    Comment.NewIterate();
                }

                #endregion                
                #region 用户微博信息
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
                //日志
                strLog = DateTime.Now.ToString() + "  " + "获取数据库中用户" + lCurrentUID.ToString() + "最新一条微博的ID...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
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
                                //日志
                                strLog = DateTime.Now.ToString() + "  " + "将用户" + lCommentUID.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count.ToString() + "个用户；数据库队列中有" + queueBuffer.Count.ToString() + "个用户";
                                bwAsync.ReportProgress( 100 );
                                //若内存中已达到上限，则使用数据库队列缓存
                                //否则使用数据库队列缓存
                                if (lstWaitingUID.Count < iQueueLength)
                                    lstWaitingUID.AddLast( lCommentUID );
                                else
                                    queueBuffer.Enqueue( lCommentUID );
                            }
                        }
                    }
                }
                #endregion

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬取用户" + lCurrentUID.ToString() + "的ID在" + lLastStatusIDOf.ToString() + "之后的微博...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                //爬取数据库中当前用户最新一条微博的ID之后的微博，存入数据库
                List<Status> lstStatus = crawler.GetStatusesOfSince( lCurrentUID, lLastStatusIDOf );
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬得" + lstStatus.Count.ToString() + "条微博。";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                #endregion
                #region 微博相应评论
                foreach (Status status in lstStatus)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    if (!Status.Exists( status.status_id ))
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "将微博" + status.status_id.ToString() + "存入数据库...";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );
                        status.Add();
                    }
                    //若该微博有转发，将转发微博ID入队
                    if (status.retweeted_status_id > 0)
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "微博" + status.status_id.ToString() + "有转发微博，将转发微博" + status.retweeted_status_id.ToString() + "加入微博队列...";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );
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
                                //日志
                                strLog = DateTime.Now.ToString() + "  " + "将用户" + lCommentUID.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count.ToString() + "个用户；数据库队列中有" + queueBuffer.Count.ToString() + "个用户";
                                bwAsync.ReportProgress( 100 );
                                //若内存中已达到上限，则使用数据库队列缓存
                                //否则使用数据库队列缓存
                                if (lstWaitingUID.Count < iQueueLength)
                                    lstWaitingUID.AddLast( lCommentUID );
                                else
                                    queueBuffer.Enqueue( lCommentUID );
                            }
                            Thread.Sleep( 5 );
                        }
                    }
                }
                #endregion                
                #region 爬取获取的转发微博
                while(lstRetweetedStatus.Count>0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }

                    long lRetweetedStatusID = lstRetweetedStatus.First.Value;
                    Status status = crawler.GetStatus( lRetweetedStatusID );
                    if(status!=null)
                    {
                        if (!Status.Exists( lRetweetedStatusID ))
                        {
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "将微博" + lRetweetedStatusID.ToString() + "存入数据库...";
                            bwAsync.ReportProgress( 100 );
                            Thread.Sleep( 5 );
                            status.Add();
                        }
                        //若该微博有转发，将转发微博ID入队
                        if (status.retweeted_status_id > 0)
                        {
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "微博" + status.status_id.ToString() + "有转发微博，将转发微博" + status.retweeted_status_id.ToString() + "加入微博队列...";
                            bwAsync.ReportProgress( 100 );
                            Thread.Sleep( 5 );
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
                                    //日志
                                    strLog = DateTime.Now.ToString() + "  " + "将用户" + lCommentUID.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count + "个用户；数据库队列中有" + queueBuffer.Count.ToString() + "个用户";
                                    bwAsync.ReportProgress( 100 );
                                    //若内存中已达到上限，则使用数据库队列缓存
                                    //否则使用数据库队列缓存
                                    if (lstWaitingUID.Count < iQueueLength)
                                        lstWaitingUID.AddLast( lCommentUID );
                                    else
                                        queueBuffer.Enqueue( lCommentUID );
                                }
                                Thread.Sleep( 5 );
                            }
                        }
                    }
                    lstRetweetedStatus.RemoveFirst();
                }
                #endregion
                //最后再将刚刚爬行完的UID加入队尾
                blnOneUserCompleted = true; //结束一个用户的迭代
                //日志
                strLog = DateTime.Now.ToString() + "  " + "用户" + lCurrentUID.ToString() + "的数据已爬取完毕，将其加入队尾...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                //若内存中已达到上限，则使用数据库队列缓存
                if (lstWaitingUID.Count < iQueueLength)
                    lstWaitingUID.AddLast( lCurrentUID );
                else
                    queueBuffer.Enqueue( lCurrentUID );
                //调整请求频度
                //针对用户计算频度
                crawler.AdjustFreq();
                //日志
                strLog = DateTime.Now.ToString() + "  " + "调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + crawler.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + crawler.RemainingHits.ToString() + "次";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
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
