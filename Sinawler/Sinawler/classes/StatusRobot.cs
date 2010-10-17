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
            long lStartUID = 0;
            long lCurrentUID = 0;
            //获取第一个UID，获取到之后，记为StartID，用于比较并确定迭代次数
            while (lstWaitingID.Count == 0) bwAsync.ReportProgress(0);//触发该事件，从而从用户机器人传递的用户队列中获取队头
            lStartUID = lstWaitingID.First.Value;

            //对队列无限循环爬行，直至有操作暂停或停止
            while(true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
                //将队头取出
                lCurrentUID = lstWaitingID.First.Value;
                lstWaitingID.RemoveFirst();
                //从数据库队列缓存中移入元素
                long lHead = queueBuffer.Dequeue();
                if (lHead > 0)
                    lstWaitingID.AddLast( lHead );
                blnOneIDCompleted = false;  //开始新的ID
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
                lCurrentID = Status.GetLastStatusIDOf( lCurrentUID );

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬取用户" + lCurrentUID.ToString() + "的ID在" + lCurrentID.ToString() + "之后的微博...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                //爬取数据库中当前用户最新一条微博的ID之后的微博，存入数据库
                List<Status> lstStatus = crawler.GetStatusesOfSince(lCurrentUID, lCurrentID);
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬得" + lstStatus.Count.ToString() + "条微博。";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );

                foreach (Status status in lstStatus)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    lCurrentID = status.status_id;
                    if (!Status.Exists(lCurrentID))
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "将微博" + lCurrentID.ToString() + "存入数据库...";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );
                        status.Add();
                    }
                    //若该微博有转发，将转发微博ID入队
                    if (status.retweeted_status_id > 0)
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "微博" + lCurrentID.ToString() + "有转发微博，将转发微博" + status.retweeted_status_id.ToString() + "加入队列等待爬取...";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );
                        lstRetweetedStatus.AddLast( status.retweeted_status_id );
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

                    lCurrentID = lstRetweetedStatus.First.Value;
                    lstRetweetedStatus.RemoveFirst();

                    Status status = crawler.GetStatus(lCurrentID);
                    if(status!=null)
                    {
                        if (!Status.Exists(lCurrentID))
                        {
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "将微博" + lCurrentID.ToString() + "存入数据库...";
                            bwAsync.ReportProgress( 100 );
                            Thread.Sleep( 5 );
                            status.Add();
                        }
                        //若该微博有转发，将转发微博ID入队
                        if (status.retweeted_status_id > 0)
                        {
                            //日志
                            strLog = DateTime.Now.ToString() + "  " + "微博" + lCurrentID.ToString() + "有转发微博，将转发微博" + status.retweeted_status_id.ToString() + "加入微博队列...";
                            bwAsync.ReportProgress( 100 );
                            Thread.Sleep( 5 );
                            lstRetweetedStatus.AddLast( status.retweeted_status_id );
                        }
                    }
                }
                #endregion
                blnOneIDCompleted = true;  //完成一个ID
                //最后再将刚刚爬行完的UID加入队尾
                //日志
                strLog = DateTime.Now.ToString() + "  " + "用户" + lCurrentUID.ToString() + "的数据已爬取完毕，将其加入队尾...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                //若内存中已达到上限，则使用数据库队列缓存
                if (lstWaitingID.Count < iQueueLength)
                    lstWaitingID.AddLast( lCurrentUID );
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
            if (lstWaitingID != null) lstWaitingID.Clear();

            //清空数据库队列缓存
            queueBuffer.Clear();
        }
    }
}
