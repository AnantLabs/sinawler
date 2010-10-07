using System;
using System.Collections.Generic;
using System.Text;
using Sina.Api;

namespace Sinawler
{
    class Robot
    {
        private SinaApiService api;
        private bool blnAsyncCancelled = false;

        public Robot(SinaApiService oAPI)
        {
            this.api = oAPI;
        }

        public bool AsyncCancelled
        {
            set { blnAsyncCancelled = value; }
            get { return blnAsyncCancelled; }
        }

        /// <summary>
        /// 以指定的UID为起点开始爬行
        /// </summary>
        /// <param name="lUid"></param>
        private void StartCraw ( long lStartUID, BackgroundWorker bwAsync )
        {
            SinaMBCrawler crawler = new SinaMBCrawler( api );
            //从队列中去掉当前UID
            lstWaitingUID.Remove( lStartUID );
            //将当前UID加到队头
            lstWaitingUID.AddFirst( lStartUID );
            //对队列循环爬行
            while (lstWaitingUID.Count > 0)
            {
                if (blnAsyncCancelled) return;
                //将队头取出
                lCurrentUID = lstWaitingUID.First.Value;
                lstWaitingUID.RemoveFirst();
                #region 预处理
                if (lCurrentUID == lStartUID)  //说明经过一次循环迭代
                {
                    if (blnAsyncCancelled) return;
                    //日志
                    AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "开始爬行之前增加迭代次数...", bwAsync );

                    User.NewIterate();
                    UserRelation.NewIterate();
                    Status.NewIterate();
                    Comment.NewIterate();
                }

                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "记录当前用户ID：" + lCurrentUID.ToString(), bwAsync );
                SysArg.SetCurrentUID( lCurrentUID );
                #endregion
                #region 用户基本信息
                if (blnAsyncCancelled) return;
                //若数据库中不存在当前用户的基本信息，则爬取，加入数据库
                if (!User.Exists( lCurrentUID ))
                {
                    if (blnAsyncCancelled) return;
                    //日志
                    AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "将用户" + lCurrentUID.ToString() + "存入数据库...", bwAsync );

                    oCurrentUser = crawler.GetUserInfo( lCurrentUID );
                    oCurrentUser.Add();
                }
                else
                {
                    if (blnAsyncCancelled) return;
                    //日志
                    AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "更新用户" + lCurrentUID.ToString() + "的数据...", bwAsync );

                    oCurrentUser = crawler.GetUserInfo( lCurrentUID );
                    oCurrentUser.Update();
                }
                #endregion
                #region 用户微博信息
                if (blnAsyncCancelled) return;
                //获取数据库中当前用户最新一条微博的ID
                long lLastStatusIDOf = Status.GetLastStatusIDOf( lCurrentUID );
                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "获取数据库中用户" + lCurrentUID.ToString() + "最新一条微博的ID...", bwAsync );
                if (blnAsyncCancelled) return;
                //爬取数据库中当前用户最新一条微博的ID之后的微博，存入数据库
                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬取用户" + lCurrentUID.ToString() + "的ID在" + lLastStatusIDOf.ToString() + "之后的微博...", bwAsync );

                List<Status> lstStatus = crawler.GetStatusesOfSince( lCurrentUID, lLastStatusIDOf );

                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬得" + lstStatus.Count.ToString() + "条微博。", bwAsync );
                #endregion
                #region 微博相应评论
                if (blnAsyncCancelled) return;

                foreach (Status status in lstStatus)
                {
                    if (!Status.Exists( status.status_id ))
                    {
                        status.Add();
                        //日志
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "将微博" + status.status_id.ToString() + "存入数据库...", bwAsync );
                    }
                    if (blnAsyncCancelled) return;
                    //爬取当前微博的评论
                    //日志
                    AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬取微博" + status.status_id.ToString() + "的评论...", bwAsync );

                    List<Comment> lstComment = crawler.GetCommentsOf( status.status_id );

                    //日志
                    AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬得" + lstComment.Count.ToString() + "条评论。", bwAsync );
                    if (blnAsyncCancelled) return;

                    foreach (Comment comment in lstComment)
                    {
                        if (!Comment.Exists( comment.comment_id ))
                        {
                            comment.Add();
                            //日志
                            AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "将评论" + comment.comment_id.ToString() + "存入数据库...", bwAsync );
                        }
                        if (blnAsyncCancelled) return;
                    }
                }
                #endregion
                #region 用户关注列表
                //爬取当前用户的关注的用户ID，记录关系，加入队列
                //日志
                if (blnAsyncCancelled) return;
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬取用户" + lCurrentUID.ToString() + "关注用户ID列表...", bwAsync );

                LinkedList<long> lstBuffer = crawler.GetFriendsOf( lCurrentUID, -1 );

                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬得" + lstBuffer.Count.ToString() + "位关注用户。", bwAsync );

                if (blnAsyncCancelled) return;

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
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "记录用户" + lCurrentUID.ToString() + "关注用户" + lstBuffer.First.Value.ToString() + "...", bwAsync );
                    }
                    if (blnAsyncCancelled) return;
                    //加入队列
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //日志
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "用户" + lstBuffer.First.Value.ToString() + "已在队列中...", bwAsync );
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //日志
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "将用户" + lstBuffer.First.Value.ToString() + "加入队列...队列中现有" + lstWaitingUID.Count + "个用户。", bwAsync );
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                if (blnAsyncCancelled) return;
                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬取用户" + lCurrentUID.ToString() + "关注用户ID列表...", bwAsync );

                lstBuffer = crawler.GetFriendsOf( lCurrentUID, 0 );

                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬得" + lstBuffer.Count.ToString() + "位关注用户。", bwAsync );

                if (blnAsyncCancelled) return;

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
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "记录用户" + lCurrentUID.ToString() + "关注用户" + lstBuffer.First.Value.ToString() + "...", bwAsync );
                    }
                    if (blnAsyncCancelled) return;
                    //加入队列
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //日志
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "用户" + lstBuffer.First.Value.ToString() + "已在队列中...", bwAsync );
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //日志
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "" ).Replace( "Z", "\t" ) + "将用户" + lstBuffer.First.Value.ToString() + "加入队列...队列中现有" + lstWaitingUID.Count + "个用户。", bwAsync );
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                #endregion
                #region 用户粉丝列表
                //爬取当前用户的粉丝的ID，记录关系，加入队列
                if (blnAsyncCancelled) return;
                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬取用户" + lCurrentUID.ToString() + "的粉丝用户ID列表...", bwAsync );

                lstBuffer = crawler.GetFollowersOf( lCurrentUID, -1 );

                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬得" + lstBuffer.Count.ToString() + "位粉丝。", bwAsync );

                if (blnAsyncCancelled) return;

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
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "记录用户" + lstBuffer.First.Value.ToString() + "关注用户" + lCurrentUID.ToString() + "...", bwAsync );
                    }
                    if (blnAsyncCancelled) return;
                    //加入队列
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //日志
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "用户" + lstBuffer.First.Value.ToString() + "已在队列中...", bwAsync );
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //日志
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "将用户" + lstBuffer.First.Value.ToString() + "加入队列...队列中现有" + lstWaitingUID.Count + "个用户。", bwAsync );
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                if (blnAsyncCancelled) return;
                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬取用户" + lCurrentUID.ToString() + "的粉丝用户ID列表...", bwAsync );

                lstBuffer = crawler.GetFollowersOf( lCurrentUID, 0 );

                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "爬得" + lstBuffer.Count.ToString() + "位粉丝。", bwAsync );

                if (blnAsyncCancelled) return;

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
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "记录用户" + lstBuffer.First.Value.ToString() + "关注用户" + lCurrentUID.ToString() + "...", bwAsync );
                    }
                    if (blnAsyncCancelled) return;
                    //加入队列
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //日志
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "用户" + lstBuffer.First.Value.ToString() + "已在队列中...", bwAsync );
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //日志
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "将用户" + lstBuffer.First.Value.ToString() + "加入队列...队列中现有" + lstWaitingUID.Count + "个用户。", bwAsync );
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                #endregion
                //最后再将刚刚爬行完的UID加入队尾
                lstWaitingUID.AddLast( lCurrentUID );
                //日志
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "用户" + lCurrentUID.ToString() + "的数据已爬取完毕，将其加入队尾...", bwAsync );
            }

        }

        public void StartCrawByCurrentUser ( object sender, DoWorkEventArgs e )
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            StartCraw( oCurrentUser.uid, bwAsync );
        }

        public void StartCrawBySearchedUser ( object sender, DoWorkEventArgs e )
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            StartCraw( oSearchedUser.uid, bwAsync );
        }

        public void StartCrawByLastUser ( object sender, DoWorkEventArgs e )
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            lCurrentUID = SysArg.GetCurrentUID();
            StartCraw( lCurrentUID, bwAsync );
        }
    }
}
