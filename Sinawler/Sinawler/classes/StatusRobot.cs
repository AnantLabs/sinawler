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
        /// 开始爬取微博数据
        /// </summary>
        public void Start()
        {
            //不加载用户队列，完全依靠UserRobot传递过来
            while (lstWaitingID.Count == 0) Thread.Sleep( 1 );   //若队列为空，则等待
            long lStartUID = lstWaitingID.First.Value;
            long lCurrentUID = 0;
            long lHead = 0;
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
                lCurrentUID = lstWaitingID.First.Value;
                lstWaitingID.RemoveFirst();
                //从数据库队列缓存中移入元素
                lHead = queueBuffer.Dequeue();
                if (lHead > 0)
                    lstWaitingID.AddLast( lHead );
                //移入队尾
                Enqueue( lCurrentUID );
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
                lCurrentID = Status.GetLastStatusIDOf( lCurrentUID );

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
                List<Status> lstStatus = crawler.GetStatusesOfSince(lCurrentUID, lCurrentID);
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
                    lCurrentID = status.status_id;
                    if (!Status.Exists(lCurrentID))
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
                        //若该微博有转发，将转发微博ID入队
                        if (status.retweeted_status_id > 0)
                        {
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "微博" + lCurrentID.ToString() + "有转发微博，将转发微博" + status.retweeted_status_id.ToString() + "加入微博队列...";
                            bwAsync.ReportProgress(0);
                            Thread.Sleep(50);
                            lstRetweetedStatus.AddLast( status.retweeted_status_id );
                        }
                    }
                }
                #endregion
                //最后再将刚刚爬行完的UID加入队尾
                //日志
                strLog = DateTime.Now.ToString() + "  " + "用户" + lCurrentUID.ToString() + "的数据已爬取完毕。";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);

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
            if (lstWaitingID != null) lstWaitingID.Clear();
            if (lstRetweetedStatus != null) lstRetweetedStatus.Clear();

            //清空数据库队列缓存
            queueBuffer.Clear();
        }
    }
}
