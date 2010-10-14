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
        public override void Start ( long lStartUID )
        {
            if (lStartUID == 0) return;

            //根据选项，选择加载用户队列的方法
            DataTable dtUID=new DataTable();

            switch (iPreLoadQueue)
            {
                case (int)EnumPreLoadQueue.PRELOAD_UID:
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "初始化用户队列：获取已爬取数据的用户的ID，并加入内存队列...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 5 );
                    dtUID = User.GetCrawedUIDTable();
                    break;
                case (int)EnumPreLoadQueue.PRELOAD_ALL_UID:
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "初始化用户队列：获取数据库中所有用户的ID，并加入内存队列...";
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
                    while (blnSuspending) Thread.Sleep( 50 );
                    lUID = Convert.ToInt64( dtUID.Rows[i]["uid"] );
                    if (!lstWaitingUID.Contains( lUID ))
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "初始化用户队列：将用户" + lUID.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count + "个用户；数据库队列中有" + queueBuffer.Count.ToString() + "个用户。进度：" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%";
                        bwAsync.ReportProgress( 5 );
                        Thread.Sleep( 5 );
                        lstWaitingUID.AddLast( lUID );
                    }
                }

                //若还有剩余，将多余部分放入数据库队列缓存
                while (i < dtUID.Rows.Count)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending) Thread.Sleep( 50 );
                    lUID = Convert.ToInt64( dtUID.Rows[i]["uid"] );
                    int iLengthInDB = queueBuffer.Count;
                    if (!queueBuffer.Contains( lUID ))
                    {
                        queueBuffer.Enqueue( lUID );
                        ++iLengthInDB;
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "初始化用户队列：内存队列已满，将用户" + lUID.ToString() + "加入数据库队列；数据库队列中有" + iLengthInDB.ToString() + "个用户。进度：" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%";
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
            lCurrentUID = lStartUID;
            //对队列循环爬行
            while (lstWaitingUID.Count > 0)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending) Thread.Sleep( 50 );
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
                    while (blnSuspending) Thread.Sleep( 50 );

                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "开始爬行之前增加迭代次数...";
                    bwAsync.ReportProgress( 100 );
                    Thread.Sleep( 5 );

                    User.NewIterate();
                    UserRelation.NewIterate();
                }
                //日志
                strLog = DateTime.Now.ToString() + "  " + "记录当前用户ID：" + lCurrentUID.ToString();
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                SysArg.SetCurrentUID( lCurrentUID );
                #endregion
                #region 用户基本信息
                if (blnAsyncCancelled) return;
                while (blnSuspending) Thread.Sleep( 50 );

                //若数据库中不存在当前用户的基本信息，则爬取，加入数据库
                if (!User.Exists( lCurrentUID ))
                {
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "将用户" + lCurrentUID.ToString() + "存入数据库...";
                    bwAsync.ReportProgress( 100 );
                    crawler.GetUserInfo( lCurrentUID ).Add();
                }
                else
                {
                    //日志
                    strLog = DateTime.Now.ToString() + "  " + "更新用户" + lCurrentUID.ToString() + "的数据...";
                    bwAsync.ReportProgress( 100 );
                    crawler.GetUserInfo( lCurrentUID ).Update();
                }
                Thread.Sleep( 5 );
                #endregion
                #region 用户关注列表
                if (blnAsyncCancelled) return;
                while (blnSuspending) Thread.Sleep( 50 );
                //日志                
                strLog = DateTime.Now.ToString() + "  " + "爬取用户" + lCurrentUID.ToString() + "关注用户ID列表...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                //爬取当前用户的关注的用户ID，记录关系，加入队列
                LinkedList<long> lstBuffer = crawler.GetFriendsOf( lCurrentUID, -1 );
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬得" + lstBuffer.Count.ToString() + "位关注用户。";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending) Thread.Sleep( 50 );
                    //若不存在有效关系，增加
                    if (!UserRelation.Exists( lCurrentUID, lstBuffer.First.Value ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending) Thread.Sleep( 50 );
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "记录用户" + lCurrentUID.ToString() + "关注用户" + lstBuffer.First.Value.ToString() + "...";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lCurrentUID;
                        ur.target_uid = lstBuffer.First.Value;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending) Thread.Sleep( 50 );
                    //加入队列
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || queueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "用户" + lstBuffer.First.Value.ToString() + "已在队列中...";
                        bwAsync.ReportProgress( 100 );
                    }
                    else
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "将用户" + lstBuffer.First.Value.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count + "个用户；数据库队列中有" + queueBuffer.Count.ToString() + "个用户";
                        bwAsync.ReportProgress( 100 );
                        //若内存中已达到上限，则使用数据库队列缓存
                        //否则使用数据库队列缓存
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            queueBuffer.Enqueue( lstBuffer.First.Value );
                    }
                    Thread.Sleep( 5 );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                #region 用户粉丝列表
                //爬取当前用户的粉丝的ID，记录关系，加入队列
                if (blnAsyncCancelled) return;
                while (blnSuspending) Thread.Sleep( 50 );
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬取用户" + lCurrentUID.ToString() + "的粉丝用户ID列表...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                lstBuffer = crawler.GetFollowersOf( lCurrentUID, -1 );
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬得" + lstBuffer.Count.ToString() + "位粉丝。";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending) Thread.Sleep( 50 );
                    //若不存在有效关系，增加
                    if (!UserRelation.Exists( lstBuffer.First.Value, lCurrentUID ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending) Thread.Sleep( 50 );
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "记录用户" + lstBuffer.First.Value.ToString() + "关注用户" + lCurrentUID.ToString() + "...";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lstBuffer.First.Value;
                        ur.target_uid = lCurrentUID;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending) Thread.Sleep( 50 );
                    //加入队列
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || queueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "用户" + lstBuffer.First.Value.ToString() + "已在队列中...";
                        bwAsync.ReportProgress( 100 );
                    }
                    else
                    {
                        //日志
                        strLog = DateTime.Now.ToString() + "  " + "将用户" + lstBuffer.First.Value.ToString() + "加入队列。内存队列中有" + lstWaitingUID.Count + "个用户；数据库队列中有" + queueBuffer.Count.ToString() + "个用户";
                        bwAsync.ReportProgress( 100 );
                        //若内存中已达到上限，则使用数据库队列缓存
                        //否则使用数据库队列缓存
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            queueBuffer.Enqueue( lstBuffer.First.Value );
                    }
                    Thread.Sleep( 5 );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //最后再将刚刚爬行完的UID加入队尾，并抛出该UID
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
            if (lstWaitingUID != null) lstWaitingUID.Clear();

            //清空数据库队列缓存
            queueBuffer.Clear();
        }
    }
}
