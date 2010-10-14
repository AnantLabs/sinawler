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
    class UserRobot
    {
        private SinaApiService api;
        private bool blnAsyncCancelled = false;     //指示爬虫线程是否被取消，来帮助中止爬虫循环
        private string strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_user.log";             //日志文件
        private string strLog = "";                 //日志内容

        private LinkedList<long> lstWaitingUID = new LinkedList<long>();     //等待爬行的UID队列
        private int iQueueLength = 5000;               //内存中队列长度上限，默认5000

        private int iPreLoadQueue = (int)(EnumPreLoadQueue.NO_PRELOAD);       //是否从数据库中预加载用户队列。默认为“否”
        private bool blnSuspending = false;         //是否暂停，默认为“否”

        private SinaMBCrawler crawler;              //爬虫对象。构造函数中初始化

        private int iInitQueueLength = 100;          //初始队列长度
        private QueueBuffer queueBuffer = new QueueBuffer( QueueBufferTarget.FOR_USER );    //数据库队列缓存
        private long lCurrentUID = 0;               //当前爬取的用户，随时抛出给StatusRobot
        BackgroundWorker bwAsync = null;

        //构造函数，需要传入相应的新浪微博API和主界面
        public UserRobot ( SinaApiService oAPI )
        {
            this.api = oAPI;

            SettingItems settings = AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            iQueueLength = settings.QueueLength;

            crawler = new SinaMBCrawler( this.api );
        }

        public bool AsyncCancelled
        {
            set { blnAsyncCancelled = value; }
            get { return blnAsyncCancelled; }
        }

        public string LogFile
        {
            set { strLogFile = value; }
            get { return strLogFile; }
        }

        public int QueueLength
        { set { iQueueLength = value; } }

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        public EnumPreLoadQueue PreLoadQueue
        {
            get { return (EnumPreLoadQueue)iPreLoadQueue; }
            set { iPreLoadQueue = (int)value; }
        }

        public bool Suspending
        {
            get { return blnSuspending; }
            set { blnSuspending = value; }
        }

        //重新设置API的接口
        public SinaApiService SinaAPI
        { set { api = value; } }

        public long ThrownUID
        { get { return lCurrentUID; } }

        public BackgroundWorker AsyncWorker
        { set { bwAsync = value; } }

        //写日志文件，也可增加在文本框中显示日志
        //oControl参数即为同时要操作的控件
        public void Actioned ( Object oControl )
        {
            //写入日志文件
            StreamWriter sw = File.AppendText( strLogFile );
            sw.WriteLine( strLog );
            sw.Close();
            sw.Dispose();

            //向文本框中追加
            Label lblLog = (Label)oControl;
            lblLog.Text = strLog;
        }

        /// <summary>
        /// 从外部获取UID加到自己队列中
        /// </summary>
        /// <param name="lUid"></param>
        public void Enqueue ( long lUID)
        {
            if (lstWaitingUID.Contains( lUID ) || queueBuffer.Contains( lUID ))
            {
                //日志
                strLog = DateTime.Now.ToString() + "  " + "用户" + lUID.ToString() + "已在队列中...";
                bwAsync.ReportProgress( 100 );
            }
            else
            {
                //日志
                strLog = DateTime.Now.ToString() + "  " + "将用户" + lUID + "加入队列。内存队列中有" + lstWaitingUID.Count + "个用户；数据库队列中有" + queueBuffer.Count.ToString() + "个用户";
                bwAsync.ReportProgress( 100 );
                //若内存中已达到上限，则使用数据库队列缓存
                //否则使用数据库队列缓存
                if (lstWaitingUID.Count < iQueueLength)
                    lstWaitingUID.AddLast( lUID );
                else
                    queueBuffer.Enqueue( lUID );
            }
            Thread.Sleep( 5 );
        }

        /// <summary>
        /// 以指定的UID为起点开始爬行
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ( long lStartUID )
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
                if (blnAsyncCancelled) return;
                while (blnSuspending) Thread.Sleep( 50 );
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬取用户" + lCurrentUID.ToString() + "关注用户ID列表...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                lstBuffer = crawler.GetFriendsOf( lCurrentUID, 0 );
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
                if (blnAsyncCancelled) return;
                while (blnSuspending) Thread.Sleep( 50 );
                //日志
                strLog = DateTime.Now.ToString() + "  " + "爬取用户" + lCurrentUID.ToString() + "的粉丝用户ID列表...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                lstBuffer = crawler.GetFollowersOf( lCurrentUID, 0 );
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

        public void Initialize ()
        {
            //初始化相应变量
            blnAsyncCancelled = false;
            if (lstWaitingUID != null) lstWaitingUID.Clear();

            //清空数据库队列缓存
            queueBuffer.Clear();
        }
    }
}
