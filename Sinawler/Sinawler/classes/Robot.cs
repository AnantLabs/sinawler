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
        /// ��ָ����UIDΪ��㿪ʼ����
        /// </summary>
        /// <param name="lUid"></param>
        private void StartCraw ( long lStartUID, BackgroundWorker bwAsync )
        {
            SinaMBCrawler crawler = new SinaMBCrawler( api );
            //�Ӷ�����ȥ����ǰUID
            lstWaitingUID.Remove( lStartUID );
            //����ǰUID�ӵ���ͷ
            lstWaitingUID.AddFirst( lStartUID );
            //�Զ���ѭ������
            while (lstWaitingUID.Count > 0)
            {
                if (blnAsyncCancelled) return;
                //����ͷȡ��
                lCurrentUID = lstWaitingUID.First.Value;
                lstWaitingUID.RemoveFirst();
                #region Ԥ����
                if (lCurrentUID == lStartUID)  //˵������һ��ѭ������
                {
                    if (blnAsyncCancelled) return;
                    //��־
                    AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ʼ����֮ǰ���ӵ�������...", bwAsync );

                    User.NewIterate();
                    UserRelation.NewIterate();
                    Status.NewIterate();
                    Comment.NewIterate();
                }

                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��¼��ǰ�û�ID��" + lCurrentUID.ToString(), bwAsync );
                SysArg.SetCurrentUID( lCurrentUID );
                #endregion
                #region �û�������Ϣ
                if (blnAsyncCancelled) return;
                //�����ݿ��в����ڵ�ǰ�û��Ļ�����Ϣ������ȡ���������ݿ�
                if (!User.Exists( lCurrentUID ))
                {
                    if (blnAsyncCancelled) return;
                    //��־
                    AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "���û�" + lCurrentUID.ToString() + "�������ݿ�...", bwAsync );

                    oCurrentUser = crawler.GetUserInfo( lCurrentUID );
                    oCurrentUser.Add();
                }
                else
                {
                    if (blnAsyncCancelled) return;
                    //��־
                    AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�����û�" + lCurrentUID.ToString() + "������...", bwAsync );

                    oCurrentUser = crawler.GetUserInfo( lCurrentUID );
                    oCurrentUser.Update();
                }
                #endregion
                #region �û�΢����Ϣ
                if (blnAsyncCancelled) return;
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID
                long lLastStatusIDOf = Status.GetLastStatusIDOf( lCurrentUID );
                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ���ݿ����û�" + lCurrentUID.ToString() + "����һ��΢����ID...", bwAsync );
                if (blnAsyncCancelled) return;
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID֮���΢�����������ݿ�
                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ�û�" + lCurrentUID.ToString() + "��ID��" + lLastStatusIDOf.ToString() + "֮���΢��...", bwAsync );

                List<Status> lstStatus = crawler.GetStatusesOfSince( lCurrentUID, lLastStatusIDOf );

                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstStatus.Count.ToString() + "��΢����", bwAsync );
                #endregion
                #region ΢����Ӧ����
                if (blnAsyncCancelled) return;

                foreach (Status status in lstStatus)
                {
                    if (!Status.Exists( status.status_id ))
                    {
                        status.Add();
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��΢��" + status.status_id.ToString() + "�������ݿ�...", bwAsync );
                    }
                    if (blnAsyncCancelled) return;
                    //��ȡ��ǰ΢��������
                    //��־
                    AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ΢��" + status.status_id.ToString() + "������...", bwAsync );

                    List<Comment> lstComment = crawler.GetCommentsOf( status.status_id );

                    //��־
                    AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstComment.Count.ToString() + "�����ۡ�", bwAsync );
                    if (blnAsyncCancelled) return;

                    foreach (Comment comment in lstComment)
                    {
                        if (!Comment.Exists( comment.comment_id ))
                        {
                            comment.Add();
                            //��־
                            AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "������" + comment.comment_id.ToString() + "�������ݿ�...", bwAsync );
                        }
                        if (blnAsyncCancelled) return;
                    }
                }
                #endregion
                #region �û���ע�б�
                //��ȡ��ǰ�û��Ĺ�ע���û�ID����¼��ϵ���������
                //��־
                if (blnAsyncCancelled) return;
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ�û�" + lCurrentUID.ToString() + "��ע�û�ID�б�...", bwAsync );

                LinkedList<long> lstBuffer = crawler.GetFriendsOf( lCurrentUID, -1 );

                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstBuffer.Count.ToString() + "λ��ע�û���", bwAsync );

                if (blnAsyncCancelled) return;

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lCurrentUID, lstBuffer.First.Value ))
                    {
                        if (blnAsyncCancelled) return;
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lCurrentUID;
                        ur.target_uid = lstBuffer.First.Value;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��¼�û�" + lCurrentUID.ToString() + "��ע�û�" + lstBuffer.First.Value.ToString() + "...", bwAsync );
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...", bwAsync );
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...����������" + lstWaitingUID.Count + "���û���", bwAsync );
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                if (blnAsyncCancelled) return;
                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ�û�" + lCurrentUID.ToString() + "��ע�û�ID�б�...", bwAsync );

                lstBuffer = crawler.GetFriendsOf( lCurrentUID, 0 );

                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstBuffer.Count.ToString() + "λ��ע�û���", bwAsync );

                if (blnAsyncCancelled) return;

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lCurrentUID, lstBuffer.First.Value ))
                    {
                        if (blnAsyncCancelled) return;
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lCurrentUID;
                        ur.target_uid = lstBuffer.First.Value;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��¼�û�" + lCurrentUID.ToString() + "��ע�û�" + lstBuffer.First.Value.ToString() + "...", bwAsync );
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...", bwAsync );
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "" ).Replace( "Z", "\t" ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...����������" + lstWaitingUID.Count + "���û���", bwAsync );
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                #endregion
                #region �û���˿�б�
                //��ȡ��ǰ�û��ķ�˿��ID����¼��ϵ���������
                if (blnAsyncCancelled) return;
                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ�û�" + lCurrentUID.ToString() + "�ķ�˿�û�ID�б�...", bwAsync );

                lstBuffer = crawler.GetFollowersOf( lCurrentUID, -1 );

                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstBuffer.Count.ToString() + "λ��˿��", bwAsync );

                if (blnAsyncCancelled) return;

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lstBuffer.First.Value, lCurrentUID ))
                    {
                        if (blnAsyncCancelled) return;
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lstBuffer.First.Value;
                        ur.target_uid = lCurrentUID;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��¼�û�" + lstBuffer.First.Value.ToString() + "��ע�û�" + lCurrentUID.ToString() + "...", bwAsync );
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...", bwAsync );
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...����������" + lstWaitingUID.Count + "���û���", bwAsync );
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                if (blnAsyncCancelled) return;
                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ�û�" + lCurrentUID.ToString() + "�ķ�˿�û�ID�б�...", bwAsync );

                lstBuffer = crawler.GetFollowersOf( lCurrentUID, 0 );

                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstBuffer.Count.ToString() + "λ��˿��", bwAsync );

                if (blnAsyncCancelled) return;

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lstBuffer.First.Value, lCurrentUID ))
                    {
                        if (blnAsyncCancelled) return;
                        UserRelation ur = new UserRelation();
                        ur.source_uid = lstBuffer.First.Value;
                        ur.target_uid = lCurrentUID;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
                        ur.iteration = 0;
                        ur.Add();
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��¼�û�" + lstBuffer.First.Value.ToString() + "��ע�û�" + lCurrentUID.ToString() + "...", bwAsync );
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...", bwAsync );
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //��־
                        AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...����������" + lstWaitingUID.Count + "���û���", bwAsync );
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                #endregion
                //����ٽ��ո��������UID�����β
                lstWaitingUID.AddLast( lCurrentUID );
                //��־
                AppendLog( DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�û�" + lCurrentUID.ToString() + "����������ȡ��ϣ���������β...", bwAsync );
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
