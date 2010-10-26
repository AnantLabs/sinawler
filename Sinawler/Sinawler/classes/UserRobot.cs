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
    class UserRobot:RobotBase
    {
        private int iPreLoadQueue = (int)(EnumPreLoadQueue.NO_PRELOAD);       //是否从数据库中预加载用户队列。默认为“否”
        private int iInitQueueLength = 100;          //初始队列长度

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        public EnumPreLoadQueue PreLoadQueue
        {
            get { return (EnumPreLoadQueue)iPreLoadQueue; }
            set { iPreLoadQueue = (int)value; }
        }

        public long CurrentUID
        { get { return lCurrentID; } }

        //构造函数，需要传入相应的新浪微博API和主界面
        public UserRobot ( SinaApiService oAPI ):base(oAPI)
        {
            queueBuffer = new QueueBuffer( QueueBufferTarget.FOR_USER );
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_user.log";
        }

        /// <summary>
        /// 以指定的UID为起点开始爬行
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ( long lStartUID )
        {
            if (lStartUID == 0) return;

            long lQueueFirst = 0;   //队头值

            //根据选项，选择加载用户队列的方法
            DataTable dtUID=new DataTable();

            switch (iPreLoadQueue)
            {
                case (int)EnumPreLoadQueue.PRELOAD_UID:
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "获取已爬取数据的用户的ID，并加入内存队列...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep(50);
                    dtUID = User.GetCrawedUIDTable();
                    break;
                case (int)EnumPreLoadQueue.PRELOAD_ALL_UID:
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "获取数据库中所有用户的ID，并加入内存队列...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep(50);
                    dtUID = UserRelation.GetAllUIDTable();
                    break;
            }

            if (dtUID != null)
            {
                iInitQueueLength = dtUID.Rows.Count;
                long lUID;
                int i;
                for (i = 0; i < dtUID.Rows.Count && lstWaitingID.Count < iQueueLength; i++)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }
                    lUID = Convert.ToInt64( dtUID.Rows[i]["uid"] );
                    if (!lstWaitingID.Contains( lUID ))
                    {
                        lstWaitingID.AddLast( lUID );
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "将用户" + lUID.ToString() + "加入队列。内存队列中现有" + lstWaitingID.Count.ToString() + "个用户，数据库队列中现有" + queueBuffer.Count.ToString() + "个用户。进度：" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%";
                        bwAsync.ReportProgress( 5 );
                        Thread.Sleep(50);
                    }
                }

                //若还有剩余，将多余部分放入数据库队列缓存
                while (i < dtUID.Rows.Count)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }
                    lUID = Convert.ToInt64( dtUID.Rows[i]["uid"] );
                    int iLengthInDB = queueBuffer.Count;
                    if (!queueBuffer.Contains( lUID ))
                    {
                        queueBuffer.Enqueue( lUID );
                        ++iLengthInDB;
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "内存队列已满，将用户" + lUID.ToString() + "加入数据库队列，数据库队列中现有" + iLengthInDB.ToString() + "个用户。进度：" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%";
                        bwAsync.ReportProgress( 5 );
                        Thread.Sleep(50);
                    }
                    i++;
                }
            }
            dtUID.Dispose();

            //从队列中去掉当前UID
            lstWaitingID.Remove( lStartUID );
            //将当前UID加到队头
            lstWaitingID.AddFirst( lStartUID );
            //日志
            strLog = DateTime.Now.ToString() + "  " + "初始化用户队列完成。";
            bwAsync.ReportProgress(0);
            Thread.Sleep(50);
            lCurrentID = lStartUID;
            //对队列循环爬行
            while (lstWaitingID.Count > 0)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }
                //将队头取出
                lCurrentID = lstWaitingID.First.Value;
                lstWaitingID.RemoveFirst();
                //从数据库队列缓存中移入元素
                long lHead = queueBuffer.Dequeue();
                if (lHead > 0)
                    lstWaitingID.AddLast( lHead );
                #region 预处理
                if (lCurrentID == lStartUID)  //说明经过一次循环迭代
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

                    User.NewIterate();
                    UserRelation.NewIterate();
                }
                //日志
                strLog = DateTime.Now.ToString() + "  " + "记录当前用户ID：" + lCurrentID.ToString();
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
                SysArg.SetCurrentUID( lCurrentID );
                #endregion
                #region 用户基本信息
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }

                //若数据库中不存在当前用户的基本信息，则爬取，加入数据库
                if (!User.Exists( lCurrentID ))
                {
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "将用户" + lCurrentID.ToString() + "存入数据库...";
                    bwAsync.ReportProgress(0);
                    crawler.GetUserInfo( lCurrentID ).Add();
                }
                else
                {
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "更新用户" + lCurrentID.ToString() + "的数据...";
                    bwAsync.ReportProgress(0);
                    crawler.GetUserInfo( lCurrentID ).Update();
                }
                Thread.Sleep(50);

                //日志
                strLog = DateTime.Now.ToString() + "  " + "调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + crawler.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + crawler.RemainingHits.ToString() + "次";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 50 );
                #endregion
                #region 用户关注列表
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }
                //日志                
                strLog = DateTime.Now.ToString() + "  " + "爬取用户" + lCurrentID.ToString() + "关注用户ID列表...";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
                //爬取当前用户的关注的用户ID，记录关系，加入队列
                LinkedList<long> lstBuffer = crawler.GetFriendsOf( lCurrentID, -1 );
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬得" + lstBuffer.Count.ToString() + "位关注用户。";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }
                    lQueueFirst = lstBuffer.First.Value;
                    //若不存在有效关系，增加
                    if (!UserRelation.Exists( lCurrentID, lQueueFirst ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(10);
                        }
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "记录用户" + lCurrentID.ToString() + "关注用户" + lQueueFirst.ToString() + "...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep(50);
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lCurrentID;
                        ur.target_uid = lstBuffer.First.Value;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }
                    //加入队列
                    if (lstWaitingID.Contains( lQueueFirst ) || queueBuffer.Contains( lQueueFirst ))
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "用户" + lQueueFirst.ToString() + "已在队列中...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        //若内存中已达到上限，则使用数据库队列缓存
                        //否则使用数据库队列缓存
                        if (lstWaitingID.Count < iQueueLength)
                            lstWaitingID.AddLast( lQueueFirst );
                        else
                            queueBuffer.Enqueue( lQueueFirst );
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "将用户" + lQueueFirst.ToString() + "加入队列。内存队列中现有" + lstWaitingID.Count.ToString() + "个用户，数据库队列中现有" + queueBuffer.Count.ToString() + "个用户";
                        bwAsync.ReportProgress(0);
                    }
                    Thread.Sleep(50);
                    lstBuffer.RemoveFirst();
                }

                //日志
                strLog = DateTime.Now.ToString() + "  " + "调整请求间隔为" + crawler.SleepTime.ToString() + "毫秒。本小时剩余" + crawler.ResetTimeInSeconds.ToString() + "秒，剩余请求次数为" + crawler.RemainingHits.ToString() + "次";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 50 );
                #endregion
                #region 用户粉丝列表
                //爬取当前用户的粉丝的ID，记录关系，加入队列
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬取用户" + lCurrentID.ToString() + "的粉丝用户ID列表...";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
                lstBuffer = crawler.GetFollowersOf( lCurrentID, -1 );
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬得" + lstBuffer.Count.ToString() + "位粉丝。";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }
                    lQueueFirst = lstBuffer.First.Value;
                    //若不存在有效关系，增加
                    if (!UserRelation.Exists( lQueueFirst, lCurrentID ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(10);
                        }
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "记录用户" + lQueueFirst.ToString() + "关注用户" + lCurrentID.ToString() + "...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep(50);
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lstBuffer.First.Value;
                        ur.target_uid = lCurrentID;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }
                    //加入队列
                    if (lstWaitingID.Contains( lQueueFirst ) || queueBuffer.Contains( lQueueFirst ))
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "用户" + lQueueFirst.ToString() + "已在队列中...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        //若内存中已达到上限，则使用数据库队列缓存
                        //否则使用数据库队列缓存
                        if (lstWaitingID.Count < iQueueLength)
                            lstWaitingID.AddLast( lQueueFirst );
                        else
                            queueBuffer.Enqueue( lQueueFirst );
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "将用户" + lQueueFirst.ToString() + "加入队列。内存队列中现有" + lstWaitingID.Count.ToString() + "个用户，数据库队列中现有" + queueBuffer.Count.ToString() + "个用户";
                        bwAsync.ReportProgress(0);
                    }
                    Thread.Sleep(50);
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //最后再将刚刚爬行完的UID加入队尾
                //日志
                strLog = DateTime.Now.ToString() + "  " + "用户" + lCurrentID.ToString() + "的数据已爬取完毕，将其加入队尾...";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
                Enqueue( lCurrentID );

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

            //清空数据库队列缓存
            queueBuffer.Clear();
        }
    }
}
