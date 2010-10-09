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

        private LinkedList<long> lstWaitingUID;     //�ȴ����е�UID����
        private int iQueueLength=5000;               //�ڴ��ж��г������ޣ�Ĭ��5000

        //���캯������Ҫ������Ӧ������΢��API��������
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

        //��������API�Ľӿ�
        public SinaApiService SinaAPI
        {
            set { api = value; }
        }

        //д��־�ļ���Ҳ���������ı�������ʾ��־
        //oControl������ΪͬʱҪ�����Ŀؼ�
        public void Actioned(Object oControl)
        {
            //д����־�ļ�
            StreamWriter sw = File.AppendText(strLogFile);
            sw.WriteLine(strLog);
            sw.Close();
            sw.Dispose();
            
            //���ı�����׷��
            Label lblLog = (Label)oControl;
            lblLog.Text = strLog;
        }

        /// <summary>
        /// ��ָ����UIDΪ��㿪ʼ����
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ( long lStartUID, BackgroundWorker bwAsync )
        {
            //�ȳ�ʼ��
            this.Initialize();

            if (lStartUID == 0) return;
            SinaMBCrawler crawler = new SinaMBCrawler( api );

            lstWaitingUID = User.GetCrawedUID();
            //�Ӷ�����ȥ����ǰUID
            lstWaitingUID.Remove( lStartUID );
            //����ǰUID�ӵ���ͷ
            lstWaitingUID.AddFirst( lStartUID );
            //���ڴ���г��ȳ������ޣ������ಿ�ַ������ݿ���л���
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
            //�Զ���ѭ������
            while (lstWaitingUID.Count > 0)
            {
                if (blnAsyncCancelled) return;
                //����ͷȡ��
                lCurrentUID = lstWaitingUID.First.Value;
                lstWaitingUID.RemoveFirst();
                //�����ݿ���л���������Ԫ��
                long lHead = QueueBuffer.Dequeue();
                if(lHead>0)
                    lstWaitingUID.AddLast( lHead );
                #region Ԥ����
                if (lCurrentUID == lStartUID)  //˵������һ��ѭ������
                {
                    if (blnAsyncCancelled) return;
                    
                    User.NewIterate();
                    UserRelation.NewIterate();
                    Status.NewIterate();
                    Comment.NewIterate();

                    //��־
                    strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��ʼ����֮ǰ���ӵ�������...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 100 );
                }

                SysArg.SetCurrentUID( lCurrentUID );
                //��־
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��¼��ǰ�û�ID��" + lCurrentUID.ToString();
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                #endregion
                #region �û�������Ϣ
                if (blnAsyncCancelled) return;

                //�����ݿ��в����ڵ�ǰ�û��Ļ�����Ϣ������ȡ���������ݿ�
                if (!User.Exists( lCurrentUID ))
                {
                    if (blnAsyncCancelled) return;
                    crawler.GetUserInfo(lCurrentUID).Add();
                    //��־
                    strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "���û�" + lCurrentUID.ToString() + "�������ݿ�...";
                    bwAsync.ReportProgress( 0 );
                }
                else
                {
                    if (blnAsyncCancelled) return;
                    crawler.GetUserInfo(lCurrentUID).Update();
                    //��־
                    strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "�����û�" + lCurrentUID.ToString() + "������...";
                    bwAsync.ReportProgress( 0 );
                }
                Thread.Sleep( 100 );
                #endregion
                #region �û�΢����Ϣ
                if (blnAsyncCancelled) return;
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID
                long lLastStatusIDOf = Status.GetLastStatusIDOf( lCurrentUID );
                //��־
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��ȡ���ݿ����û�" + lCurrentUID.ToString() + "����һ��΢����ID...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                if (blnAsyncCancelled) return;
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID֮���΢�����������ݿ�
                List<Status> lstStatus = crawler.GetStatusesOfSince( lCurrentUID, lLastStatusIDOf );
                //��־
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��ȡ�û�" + lCurrentUID.ToString() + "��ID��" + lLastStatusIDOf.ToString() + "֮���΢��...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                //��־
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "����" + lstStatus.Count.ToString() + "��΢����";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                #endregion
                #region ΢����Ӧ����
                if (blnAsyncCancelled) return;
                foreach (Status status in lstStatus)
                {
                    Thread.Sleep( 100 );
                    if (blnAsyncCancelled) return;
                    if (!Status.Exists( status.status_id ))
                    {
                        status.Add();
                        //��־
                        strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��΢��" + status.status_id.ToString() + "�������ݿ�...";
                        bwAsync.ReportProgress( 0 );
                        Thread.Sleep( 100 );
                    }
                    if (blnAsyncCancelled) return;
                    
                    //��־
                    strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��ȡ΢��" + status.status_id.ToString() + "������...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 100 );
                    //��ȡ��ǰ΢��������
                    List<Comment> lstComment = crawler.GetCommentsOf( status.status_id );
                    //��־
                    strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "����" + lstComment.Count.ToString() + "�����ۡ�";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 100 );
                    if (blnAsyncCancelled) return;

                    foreach (Comment comment in lstComment)
                    {
                        Thread.Sleep( 100 );
                        if (blnAsyncCancelled) return;
                        if (!Comment.Exists( comment.comment_id ))
                        {
                            //��־
                            strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "������" + comment.comment_id.ToString() + "�������ݿ�...";
                            bwAsync.ReportProgress( 0 );
                            Thread.Sleep( 100 );
                            comment.Add();
                        }
                    }
                }
                #endregion
                #region �û���ע�б�
                if (blnAsyncCancelled) return;
                //��ȡ��ǰ�û��Ĺ�ע���û�ID����¼��ϵ���������
                LinkedList<long> lstBuffer = crawler.GetFriendsOf( lCurrentUID, -1 );
                //��־                
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��ȡ�û�" + lCurrentUID.ToString() + "��ע�û�ID�б�...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "����" + lstBuffer.Count.ToString() + "λ��ע�û���";
                bwAsync.ReportProgress(0);
                Thread.Sleep( 100 );

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
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��¼�û�" + lCurrentUID.ToString() + "��ע�û�" + lstBuffer.First.Value.ToString() + "...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep( 100 );
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || QueueBuffer.Contains(lstBuffer.First.Value))
                    {
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            QueueBuffer.Enqueue( lstBuffer.First.Value );
                        
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...�ڴ����������" + lstWaitingUID.Count + "���û������ݿ����������"+QueueBuffer.Count.ToString()+"���û�";
                        bwAsync.ReportProgress(0);
                    }
                    Thread.Sleep( 100 );
                    lstBuffer.RemoveFirst();
                }
                if (blnAsyncCancelled) return;
                lstBuffer = crawler.GetFriendsOf( lCurrentUID, 0 );
                //��־
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��ȡ�û�" + lCurrentUID.ToString() + "��ע�û�ID�б�...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "����" + lstBuffer.Count.ToString() + "λ��ע�û���";
                bwAsync.ReportProgress(0);
                Thread.Sleep( 100 );

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
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��¼�û�" + lCurrentUID.ToString() + "��ע�û�" + lstBuffer.First.Value.ToString() + "...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep( 100 );
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || QueueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            QueueBuffer.Enqueue( lstBuffer.First.Value );

                        //��־
                        strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...�ڴ����������" + lstWaitingUID.Count + "���û������ݿ����������" + QueueBuffer.Count.ToString() + "���û�";
                        bwAsync.ReportProgress(0);
                    }
                    Thread.Sleep( 100 );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                #region �û���˿�б�
                //��ȡ��ǰ�û��ķ�˿��ID����¼��ϵ���������
                if (blnAsyncCancelled) return;
                lstBuffer = crawler.GetFollowersOf( lCurrentUID, -1 );
                //��־
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��ȡ�û�" + lCurrentUID.ToString() + "�ķ�˿�û�ID�б�...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "����" + lstBuffer.Count.ToString() + "λ��˿��";
                bwAsync.ReportProgress(0);
                Thread.Sleep( 100 );

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
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��¼�û�" + lstBuffer.First.Value.ToString() + "��ע�û�" + lCurrentUID.ToString() + "...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep( 100 );
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || QueueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            QueueBuffer.Enqueue( lstBuffer.First.Value );

                        //��־
                        strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...�ڴ����������" + lstWaitingUID.Count + "���û������ݿ����������" + QueueBuffer.Count.ToString() + "���û�";
                        bwAsync.ReportProgress(0);
                    }
                    Thread.Sleep( 100 );
                    lstBuffer.RemoveFirst();
                }
                if (blnAsyncCancelled) return;
                lstBuffer = crawler.GetFollowersOf( lCurrentUID, 0 );
                //��־
                strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��ȡ�û�" + lCurrentUID.ToString() + "�ķ�˿�û�ID�б�...";
                bwAsync.ReportProgress( 0 );
                Thread.Sleep( 100 );
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "����" + lstBuffer.Count.ToString() + "λ��˿��";
                bwAsync.ReportProgress(0);
                Thread.Sleep( 100 );

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
                        strLog= DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "��¼�û�" + lstBuffer.First.Value.ToString() + "��ע�û�" + lCurrentUID.ToString() + "...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep( 100 );
                    }
                    if (blnAsyncCancelled) return;
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || QueueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress(0);
                    }
                    else
                    {
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            QueueBuffer.Enqueue( lstBuffer.First.Value );

                        //��־
                        strLog = DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "���û�" + lstBuffer.First.Value.ToString() + "�������...�ڴ����������" + lstWaitingUID.Count + "���û������ݿ����������" + QueueBuffer.Count.ToString() + "���û�";
                        bwAsync.ReportProgress(0);
                    }
                    Thread.Sleep( 100 );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //����ٽ��ո��������UID�����β
                //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                if (lstWaitingUID.Count < iQueueLength)
                    lstWaitingUID.AddLast( lCurrentUID );
                else
                    QueueBuffer.Enqueue( lCurrentUID );
                //��־
                strLog=DateTime.Now.ToString( "u" ).Replace( "Z", "  " ) + "�û�" + lCurrentUID.ToString() + "����������ȡ��ϣ���������β...";
                bwAsync.ReportProgress(0);
                Thread.Sleep( 100 );
            }

        }

        public void Initialize()
        {
            //ֹͣ���У���ʼ����Ӧ����
            blnAsyncCancelled = false;
            lstWaitingUID = User.GetCrawedUID();

            //������ݿ���л���
            QueueBuffer.Clear();
        }
    }
}
