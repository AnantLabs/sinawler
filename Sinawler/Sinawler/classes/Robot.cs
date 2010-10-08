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
        private bool blnAsyncCancelled = false;     //ָʾ�����߳��Ƿ�ȡ������������ֹ����ѭ��
        private string strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".log";             //��־�ļ�
        private string strLog = "";                 //��־����
        private frmMain frmUI;                         //���߳��еĽ��棬ͨ��Robot�Ļص���������ʾ��Ϣ

        private LinkedList<long> lstWaitingUID;     //�ȴ����е�UID����
        private long lStartUID=0;                     //���е�����û�UID��Ӧ��Robot�ⲿ������г�ʼ��

        //���캯������Ҫ������Ӧ������΢��API��������
        public Robot ( SinaApiService oAPI, frmMain oUIForm )
        {
            this.api = oAPI;
            this.frmUI = oUIForm;
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

        public frmMain ParentUI
        { set { frmUI = value; } }

        public long StartUID
        { set { lStartUID = value; } }

        //��������API�Ľӿ�
        public SinaApiService SinaAPI
        {
            set { api = value; }
        }

        //���ı�������ʾ��־��Ҳ������д��־�ļ�
        public void Actioned(object sender, ProgressChangedEventArgs e)
        {
            Thread.Sleep(100);  //��ͣһ��

            //д����־�ļ�
            StreamWriter sw = File.AppendText(strLogFile);
            sw.WriteLine(strLog);
            sw.Close();
            sw.Dispose();

            //���ı�����׷��
            frmUI.ShowLog( strLog );
        }

        /// <summary>
        /// ��ָ����UIDΪ��㿪ʼ����
        /// </summary>
        /// <param name="lUid"></param>
        private void StartCraw ( BackgroundWorker bwAsync )
        {
            if (lStartUID == 0) return;

            lstWaitingUID = User.GetCrawedUID();

            SinaMBCrawler crawler = new SinaMBCrawler( api );
            //�Ӷ�����ȥ����ǰUID
            lstWaitingUID.Remove( lStartUID );
            //����ǰUID�ӵ���ͷ
            lstWaitingUID.AddFirst( lStartUID );
            long lCurrentUID=lStartUID;
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
                    strLog = DateTime.Now.ToString("u").Replace("Z", "\t") + "��ʼ����֮ǰ���ӵ�������...";
                    bwAsync.ReportProgress(0);

                    User.NewIterate();
                    UserRelation.NewIterate();
                    Status.NewIterate();
                    Comment.NewIterate();
                }

                //��־
                strLog = DateTime.Now.ToString("u").Replace("Z", "\t") + "��¼��ǰ�û�ID��" + lCurrentUID.ToString();
                bwAsync.ReportProgress(0);
                SysArg.SetCurrentUID( lCurrentUID );
                #endregion
                #region �û�������Ϣ
                if (blnAsyncCancelled) return;

                //�����ݿ��в����ڵ�ǰ�û��Ļ�����Ϣ������ȡ���������ݿ�
                if (!User.Exists( lCurrentUID ))
                {
                    if (blnAsyncCancelled) return;
                    //��־
                    strLog = DateTime.Now.ToString("u").Replace("Z", "\t") + "���û�" + lCurrentUID.ToString() + "�������ݿ�...";
                    bwAsync.ReportProgress(0);

                    crawler.GetUserInfo(lCurrentUID).Add();
                }
                else
                {
                    if (blnAsyncCancelled) return;
                    //��־
                    strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�����û�" + lCurrentUID.ToString() + "������...";
                    bwAsync.ReportProgress(0);

                    crawler.GetUserInfo(lCurrentUID).Update();
                }
                #endregion
                #region �û�΢����Ϣ
                if (blnAsyncCancelled) return;
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID
                long lLastStatusIDOf = Status.GetLastStatusIDOf( lCurrentUID );
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ���ݿ����û�" + lCurrentUID.ToString() + "����һ��΢����ID...";
                bwAsync.ReportProgress(0);
                if (blnAsyncCancelled) return;
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID֮���΢�����������ݿ�
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ�û�" + lCurrentUID.ToString() + "��ID��" + lLastStatusIDOf.ToString() + "֮���΢��...";
                bwAsync.ReportProgress(0);

                List<Status> lstStatus = crawler.GetStatusesOfSince( lCurrentUID, lLastStatusIDOf );

                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstStatus.Count.ToString() + "��΢����";
                bwAsync.ReportProgress(0);
                #endregion
                #region ΢����Ӧ����
                if (blnAsyncCancelled) return;

                foreach (Status status in lstStatus)
                {
                    if (!Status.Exists( status.status_id ))
                    {
                        status.Add();
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��΢��" + status.status_id.ToString() + "�������ݿ�...";
                        bwAsync.ReportProgress(0);
                    }
                    if (blnAsyncCancelled) return;
                    //��ȡ��ǰ΢��������
                    //��־
                    strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ΢��" + status.status_id.ToString() + "������...";
                    bwAsync.ReportProgress(0);

                    List<Comment> lstComment = crawler.GetCommentsOf( status.status_id );

                    //��־
                    strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstComment.Count.ToString() + "�����ۡ�";
                    bwAsync.ReportProgress(0);
                    if (blnAsyncCancelled) return;

                    foreach (Comment comment in lstComment)
                    {
                        if (!Comment.Exists( comment.comment_id ))
                        {
                            comment.Add();
                            //��־
                            strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "������" + comment.comment_id.ToString() + "�������ݿ�...";
                            bwAsync.ReportProgress(0);
                        }
                        if (blnAsyncCancelled) return;
                    }
                }
                #endregion
                #region �û���ע�б�
                //��ȡ��ǰ�û��Ĺ�ע���û�ID����¼��ϵ���������
                //��־
                if (blnAsyncCancelled) return;
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ�û�" + lCurrentUID.ToString() + "��ע�û�ID�б�...";
                bwAsync.ReportProgress(0);

                LinkedList<long> lstBuffer = crawler.GetFriendsOf( lCurrentUID, -1 );

                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstBuffer.Count.ToString() + "λ��ע�û���";
                bwAsync.ReportProgress(0);

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
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��¼�û�" + lCurrentUID.ToString() + "��ע�û�" + lstBuffer.First.Value.ToString() + "...";
                        bwAsync.ReportProgress(0);
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...����������" + lstWaitingUID.Count + "���û���";
                        bwAsync.ReportProgress(0);
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                if (blnAsyncCancelled) return;
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ�û�" + lCurrentUID.ToString() + "��ע�û�ID�б�...";
                bwAsync.ReportProgress(0);

                lstBuffer = crawler.GetFriendsOf( lCurrentUID, 0 );

                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstBuffer.Count.ToString() + "λ��ע�û���";
                bwAsync.ReportProgress(0);

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
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��¼�û�" + lCurrentUID.ToString() + "��ע�û�" + lstBuffer.First.Value.ToString() + "...";
                        bwAsync.ReportProgress(0);
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "" ).Replace( "Z", "\t" ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...����������" + lstWaitingUID.Count + "���û���";
                        bwAsync.ReportProgress(0);
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                #endregion
                #region �û���˿�б�
                //��ȡ��ǰ�û��ķ�˿��ID����¼��ϵ���������
                if (blnAsyncCancelled) return;
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ�û�" + lCurrentUID.ToString() + "�ķ�˿�û�ID�б�...";
                bwAsync.ReportProgress(0);

                lstBuffer = crawler.GetFollowersOf( lCurrentUID, -1 );

                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstBuffer.Count.ToString() + "λ��˿��";
                bwAsync.ReportProgress(0);

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
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��¼�û�" + lstBuffer.First.Value.ToString() + "��ע�û�" + lCurrentUID.ToString() + "...";
                        bwAsync.ReportProgress(0);
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...����������" + lstWaitingUID.Count + "���û���";
                        bwAsync.ReportProgress(0);
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                if (blnAsyncCancelled) return;
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��ȡ�û�" + lCurrentUID.ToString() + "�ķ�˿�û�ID�б�...";
                bwAsync.ReportProgress(0);

                lstBuffer = crawler.GetFollowersOf( lCurrentUID, 0 );

                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "����" + lstBuffer.Count.ToString() + "λ��˿��";
                bwAsync.ReportProgress(0);

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
                        strLog= DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "��¼�û�" + lstBuffer.First.Value.ToString() + "��ע�û�" + lCurrentUID.ToString() + "...";
                        bwAsync.ReportProgress(0);
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        lstWaitingUID.AddLast( lstBuffer.First.Value );
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...����������" + lstWaitingUID.Count + "���û���";
                        bwAsync.ReportProgress(0);
                    }
                    lstBuffer.RemoveFirst();
                    if (blnAsyncCancelled) return;
                }
                #endregion
                //����ٽ��ո��������UID�����β
                lstWaitingUID.AddLast( lCurrentUID );
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "\t" ) + "�û�" + lCurrentUID.ToString() + "����������ȡ��ϣ���������β...";
                bwAsync.ReportProgress(0);
            }

        }

        public void StartCrawByCurrentUser(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            this.StartCraw(bwAsync);
        }

        public void StartCrawBySearchedUser(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            this.StartCraw(bwAsync);
        }

        public void StartCrawByLastUser(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            lStartUID = SysArg.GetCurrentUID();
            this.StartCraw(bwAsync);
        }

        public void Stop( object sender, RunWorkerCompletedEventArgs e )
        {
            //ֹͣ���У���ʼ����Ӧ����
            blnAsyncCancelled = false;
            lstWaitingUID = User.GetCrawedUID();

            //��������Ӧ�仯
            frmUI.RobotStopped(e);
        }
    }
}
