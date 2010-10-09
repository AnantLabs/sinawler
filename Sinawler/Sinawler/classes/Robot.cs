using System;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Sinawler.Model;

namespace Sinawler
{
    class Robot
    {
        private SinaApiService api;
        private bool blnAsyncCancelled = false;     //指示爬虫线程是否被取消，来帮助中止爬虫循环
        private string strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".log";             //日志文件
        private string strLog = "";                 //日志内容

        private LinkedList<long> lstWaitingUID;     //等待爬行的UID队列
        private int iQueueLength=5000;               //内存中队列长度上限，默认5000

        //构造函数，需要传入相应的新浪微博API和主界面
        public Robot ( SinaApiService oAPI)
        {
            this.api = oAPI;

            iQueueLength = AppSettings.Load().QueueLength;
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

        //重新设置API的接口
        public SinaApiService SinaAPI
        {
            set { api = value; }
        }

        //写日志文件，也可增加在文本框中显示日志
        //oControl参数即为同时要操作的控件
        public void Actioned(Object oControl)
        {
            //写入日志文件
            StreamWriter sw = File.AppendText(strLogFile);
            sw.WriteLine(strLog);
            sw.Close();
            sw.Dispose();
            
            //向文本框中追加
            Label lblLog = (Label)oControl;
            lblLog.Text = strLog;
        }

        /// <summary>
        /// 以指定的UID为起点开始爬行
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ( long lStartUID, BackgroundWorker bwAsync )
        {
            //先初始化
            this.Initialize();

            if (lStartUID == 0) return;
            SinaMBCrawler crawler = new SinaMBCrawler( api );

            lstWaitingUID = User.GetCrawedUID();
            //从队列中去掉当前UID
            lstWaitingUID.Remove( lStartUID );
            //将当前UID加到队头
            lstWaitingUID.AddFirst( lStartUID );
            //若内存队列长度超出上限，将多余部分放入数据库队列缓存
            if (lstWaitingUID.Count > iQueueLength)
            {
                for (int i = lstWaitingUID.Count - 1; i >= iQueueLength; i--)
                {
                    long lTmp = lstWaitingUID.Last.Value;
                    User usrTmp = new User( lTmp );
                    QueueBuffer.Add( lTmp,usrTmp.update_time );
                    lstWaitingUID.RemoveLast();
                }
            }

            long lCurrentUID=lStartUID;
            //对队列循环爬行
            while (lstWaitingUID.Count > 0)
            {
                if (blnAsyncCancelled) return;
                //将队头取出
                lCurrentUID = lstWaitingUID.First.Value;
                lstWaitingUID.RemoveFirst();
                //从数据库队列缓存中移入元素
                long lHead = QueueBuffer.Dequeue();
                if(lHead>0)
                    lstWaitingUID.AddLast( lHead );
                #region 预处理
                if (lCurrentUID == lStartUID)  //说明经过一次循环迭代
                {
                    if (blnAsyncCancelled) return;
                    
                    User.NewIterate();
                    UserRelation.NewIterate();
                    Status.NewIterate();
                    Comment.NewIterate();

                    //日志
                    strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "开始爬行之前增加迭代次数...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 100 );
                }

                SysArg.SetCurrentUID( lCurrentUID );
                //日志
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "记录当前用户ID：" + lCurrentUID.ToString();
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                #endregion
                #region 用户基本信息
                if (blnAsyncCancelled) return;

                //若数据库中不存在当前用户的基本信息，则爬取，加入数据库
                if (!User.Exists( lCurrentUID ))
                {
                    if (blnAsyncCancelled) return;
                    crawler.GetUserInfo(lCurrentUID).Add();
                    //日志
                    strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "将用户" + lCurrentUID.ToString() + "存入数据库...";
                    bwAsync.ReportProgress( 0 );
                }
                else
                {
                    if (blnAsyncCancelled) return;
                    crawler.GetUserInfo(lCurrentUID).Update();
                    //日志
                    strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "更新用户" + lCurrentUID.ToString() + "的数据...";
                    bwAsync.ReportProgress( 0 );
                }
                Thread.Sleep( 100 );
                #endregion
                #region 用户微博信息
                if (blnAsyncCancelled) return;
                //获取数据库中当前用户最新一条微博的ID
                long lLastStatusIDOf = Status.GetLastStatusIDOf( lCurrentUID );
                //日志
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "获取数据库中用户" + lCurrentUID.ToString() + "最新一条微博的ID...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                if (blnAsyncCancelled) return;
                //爬取数据库中当前用户最新一条微博的ID之后的微博，存入数据库
                List<Status> lstStatus = crawler.GetStatusesOfSince( lCurrentUID, lLastStatusIDOf );
                //日志
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬取用户" + lCurrentUID.ToString() + "的ID在" + lLastStatusIDOf.ToString() + "之后的微博...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                //日志
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬得" + lstStatus.Count.ToString() + "条微博。";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                #endregion
                #region 微博相应评论
                if (blnAsyncCancelled) return;
                foreach (Status status in lstStatus)
                {
                    Thread.Sleep( 100 );
                    if (blnAsyncCancelled) return;
                    if (!Status.Exists( status.status_id ))
                    {
                        status.Add();
                        //日志
                        strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "将微博" + status.status_id.ToString() + "存入数据库...";
                        bwAsync.ReportProgress( 0 );
                        Thread.Sleep( 100 );
                    }
                    if (blnAsyncCancelled) return;
                    
                    //日志
                    strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬取微博" + status.status_id.ToString() + "的评论...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 100 );
                    //爬取当前微博的评论
                    List<Comment> lstComment = crawler.GetCommentsOf( status.status_id );
                    //日志
                    strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬得" + lstComment.Count.ToString() + "条评论。";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 100 );
                    if (blnAsyncCancelled) return;

                    foreach (Comment comment in lstComment)
                    {
                        Thread.Sleep( 100 );
                        if (blnAsyncCancelled) return;
                        if (!Comment.Exists( comment.comment_id ))
                        {
                            //日志
                            strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "将评论" + comment.comment_id.ToString() + "存入数据库...";
                            bwAsync.ReportProgress( 0 );
                            Thread.Sleep( 100 );
                            comment.Add();
                        }
                    }
                }
                #endregion
                #region 用户关注列表
                if (blnAsyncCancelled) return;
                //爬取当前用户的关注的用户ID，记录关系，加入队列
                LinkedList<long> lstBuffer = crawler.GetFriendsOf( lCurrentUID, -1 );
                //日志                
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬取用户" + lCurrentUID.ToString() + "关注用户ID列表...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                //日志
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬得" + lstBuffer.Count.ToString() + "位关注用户。";
                bwAsync.ReportProgress(0);
                Thread.Sleep( 100 );

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    //若不存在有效关系，增加
                    if (!UserRelation.Exists( lCurrentUID, lstBuffer.First.Value ))
                    {
                        if (blnAsyncCancelled) return;
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lCurrentUID;
                        ur.target_uid = lstBuffer.First.Value;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                        //日志
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "记录用户" + lCurrentUID.ToString() + "关注用户" + lstBuffer.First.Value.ToString() + "...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep( 100 );
                    }
                    if (blnAsyncCancelled) return;
                    //加入队列
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || QueueBuffer.Contains(lstBuffer.First.Value))
                    {
                        //日志
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "用户" + lstBuffer.First.Value.ToString() + "已在队列中...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        //若内存中已达到上限，则使用数据库队列缓存
                        //否则使用数据库队列缓存
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            QueueBuffer.Enqueue( lstBuffer.First.Value );
                        
                        //日志
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "将用户" + lstBuffer.First.Value.ToString() + "加入队列...内存队列中现有" + lstWaitingUID.Count + "个用户；数据库队列中现有"+QueueBuffer.Count.ToString()+"个用户";
                        bwAsync.ReportProgress(0);
                    }
                    Thread.Sleep( 100 );
                    lstBuffer.RemoveFirst();
                }
                if (blnAsyncCancelled) return;
                lstBuffer = crawler.GetFriendsOf( lCurrentUID, 0 );
                //日志
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬取用户" + lCurrentUID.ToString() + "关注用户ID列表...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                //日志
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬得" + lstBuffer.Count.ToString() + "位关注用户。";
                bwAsync.ReportProgress(0);
                Thread.Sleep( 100 );

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    //若不存在有效关系，增加
                    if (!UserRelation.Exists( lCurrentUID, lstBuffer.First.Value ))
                    {
                        if (blnAsyncCancelled) return;
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lCurrentUID;
                        ur.target_uid = lstBuffer.First.Value;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                        //日志
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "记录用户" + lCurrentUID.ToString() + "关注用户" + lstBuffer.First.Value.ToString() + "...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep( 100 );
                    }
                    if (blnAsyncCancelled) return;
                    //加入队列
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || QueueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //日志
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "用户" + lstBuffer.First.Value.ToString() + "已在队列中...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        //若内存中已达到上限，则使用数据库队列缓存
                        //否则使用数据库队列缓存
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            QueueBuffer.Enqueue( lstBuffer.First.Value );

                        //日志
                        strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "将用户" + lstBuffer.First.Value.ToString() + "加入队列...内存队列中现有" + lstWaitingUID.Count + "个用户；数据库队列中现有" + QueueBuffer.Count.ToString() + "个用户";
                        bwAsync.ReportProgress(0);
                    }
                    Thread.Sleep( 100 );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                #region 用户粉丝列表
                //爬取当前用户的粉丝的ID，记录关系，加入队列
                if (blnAsyncCancelled) return;
                lstBuffer = crawler.GetFollowersOf( lCurrentUID, -1 );
                //日志
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬取用户" + lCurrentUID.ToString() + "的粉丝用户ID列表...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                //日志
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬得" + lstBuffer.Count.ToString() + "位粉丝。";
                bwAsync.ReportProgress(0);
                Thread.Sleep( 100 );

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    //若不存在有效关系，增加
                    if (!UserRelation.Exists( lstBuffer.First.Value, lCurrentUID ))
                    {
                        if (blnAsyncCancelled) return;
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lstBuffer.First.Value;
                        ur.target_uid = lCurrentUID;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                        //日志
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "记录用户" + lstBuffer.First.Value.ToString() + "关注用户" + lCurrentUID.ToString() + "...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep( 100 );
                    }
                    if (blnAsyncCancelled) return;
                    //加入队列
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || QueueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //日志
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "用户" + lstBuffer.First.Value.ToString() + "已在队列中...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        //若内存中已达到上限，则使用数据库队列缓存
                        //否则使用数据库队列缓存
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            QueueBuffer.Enqueue( lstBuffer.First.Value );

                        //日志
                        strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "将用户" + lstBuffer.First.Value.ToString() + "加入队列...内存队列中现有" + lstWaitingUID.Count + "个用户；数据库队列中现有" + QueueBuffer.Count.ToString() + "个用户";
                        bwAsync.ReportProgress(0);
                    }
                    Thread.Sleep( 100 );
                    lstBuffer.RemoveFirst();
                }
                if (blnAsyncCancelled) return;
                lstBuffer = crawler.GetFollowersOf( lCurrentUID, 0 );
                //日志
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬取用户" + lCurrentUID.ToString() + "的粉丝用户ID列表...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                //日志
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "爬得" + lstBuffer.Count.ToString() + "位粉丝。";
                bwAsync.ReportProgress(0);
                Thread.Sleep( 100 );

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    //若不存在有效关系，增加
                    if (!UserRelation.Exists( lstBuffer.First.Value, lCurrentUID ))
                    {
                        if (blnAsyncCancelled) return;
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lstBuffer.First.Value;
                        ur.target_uid = lCurrentUID;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                        //日志
                        strLog= DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "记录用户" + lstBuffer.First.Value.ToString() + "关注用户" + lCurrentUID.ToString() + "...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep( 100 );
                    }
                    if (blnAsyncCancelled) return;
                    //加入队列
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || QueueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //日志
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "用户" + lstBuffer.First.Value.ToString() + "已在队列中...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        //若内存中已达到上限，则使用数据库队列缓存
                        //否则使用数据库队列缓存
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            QueueBuffer.Enqueue( lstBuffer.First.Value );

                        //日志
                        strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "将用户" + lstBuffer.First.Value.ToString() + "加入队列...内存队列中现有" + lstWaitingUID.Count + "个用户；数据库队列中现有" + QueueBuffer.Count.ToString() + "个用户";
                        bwAsync.ReportProgress(0);
                    }
                    Thread.Sleep( 100 );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //最后再将刚刚爬行完的UID加入队尾
                //若内存中已达到上限，则使用数据库队列缓存
                if (lstWaitingUID.Count < iQueueLength)
                    lstWaitingUID.AddLast( lCurrentUID );
                else
                    QueueBuffer.Enqueue( lCurrentUID );
                //日志
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "用户" + lCurrentUID.ToString() + "的数据已爬取完毕，将其加入队尾...";
                bwAsync.ReportProgress(0);
                Thread.Sleep( 100 );
            }

        }

        public void Initialize()
        {
            //停止爬行，初始化相应变量
            blnAsyncCancelled = false;
            lstWaitingUID = User.GetCrawedUID();

            //清空数据库队列缓存
            QueueBuffer.Clear();
        }
    }
}
