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
        private bool blnAsyncCancelled = false;     //ָʾ�����߳��Ƿ�ȡ������������ֹ����ѭ��
        private string strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_user.log";             //��־�ļ�
        private string strLog = "";                 //��־����

        private LinkedList<long> lstWaitingUID = new LinkedList<long>();     //�ȴ����е�UID����
        private int iQueueLength = 5000;               //�ڴ��ж��г������ޣ�Ĭ��5000

        private int iPreLoadQueue = (int)(EnumPreLoadQueue.NO_PRELOAD);       //�Ƿ�����ݿ���Ԥ�����û����С�Ĭ��Ϊ����
        private bool blnSuspending = false;         //�Ƿ���ͣ��Ĭ��Ϊ����

        private SinaMBCrawler crawler;              //������󡣹��캯���г�ʼ��

        private int iInitQueueLength = 100;          //��ʼ���г���
        private QueueBuffer queueBuffer = new QueueBuffer( QueueBufferTarget.FOR_USER );    //���ݿ���л���
        private long lCurrentUID = 0;               //��ǰ��ȡ���û�����ʱ�׳���StatusRobot
        BackgroundWorker bwAsync = null;

        //���캯������Ҫ������Ӧ������΢��API��������
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

        //��������API�Ľӿ�
        public SinaApiService SinaAPI
        { set { api = value; } }

        public long ThrownUID
        { get { return lCurrentUID; } }

        public BackgroundWorker AsyncWorker
        { set { bwAsync = value; } }

        //д��־�ļ���Ҳ���������ı�������ʾ��־
        //oControl������ΪͬʱҪ�����Ŀؼ�
        public void Actioned ( Object oControl )
        {
            //д����־�ļ�
            StreamWriter sw = File.AppendText( strLogFile );
            sw.WriteLine( strLog );
            sw.Close();
            sw.Dispose();

            //���ı�����׷��
            Label lblLog = (Label)oControl;
            lblLog.Text = strLog;
        }

        /// <summary>
        /// ���ⲿ��ȡUID�ӵ��Լ�������
        /// </summary>
        /// <param name="lUid"></param>
        public void Enqueue ( long lUID)
        {
            if (lstWaitingUID.Contains( lUID ) || queueBuffer.Contains( lUID ))
            {
                //��־
                strLog = DateTime.Now.ToString() + "  " + "�û�" + lUID.ToString() + "���ڶ�����...";
                bwAsync.ReportProgress( 100 );
            }
            else
            {
                //��־
                strLog = DateTime.Now.ToString() + "  " + "���û�" + lUID + "������С��ڴ��������" + lstWaitingUID.Count + "���û������ݿ��������" + queueBuffer.Count.ToString() + "���û�";
                bwAsync.ReportProgress( 100 );
                //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                //����ʹ�����ݿ���л���
                if (lstWaitingUID.Count < iQueueLength)
                    lstWaitingUID.AddLast( lUID );
                else
                    queueBuffer.Enqueue( lUID );
            }
            Thread.Sleep( 5 );
        }

        /// <summary>
        /// ��ָ����UIDΪ��㿪ʼ����
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ( long lStartUID )
        {
            if (lStartUID == 0) return;

            //����ѡ�ѡ������û����еķ���
            DataTable dtUID=new DataTable();

            switch (iPreLoadQueue)
            {
                case (int)EnumPreLoadQueue.PRELOAD_UID:
                    //��־
                    strLog = DateTime.Now.ToString() + "  " + "��ʼ���û����У���ȡ����ȡ���ݵ��û���ID���������ڴ����...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 5 );
                    dtUID = User.GetCrawedUIDTable();
                    break;
                case (int)EnumPreLoadQueue.PRELOAD_ALL_UID:
                    //��־
                    strLog = DateTime.Now.ToString() + "  " + "��ʼ���û����У���ȡ���ݿ��������û���ID���������ڴ����...";
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
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "��ʼ���û����У����û�" + lUID.ToString() + "������С��ڴ��������" + lstWaitingUID.Count + "���û������ݿ��������" + queueBuffer.Count.ToString() + "���û������ȣ�" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%";
                        bwAsync.ReportProgress( 5 );
                        Thread.Sleep( 5 );
                        lstWaitingUID.AddLast( lUID );
                    }
                }

                //������ʣ�࣬�����ಿ�ַ������ݿ���л���
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
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "��ʼ���û����У��ڴ�������������û�" + lUID.ToString() + "�������ݿ���У����ݿ��������" + iLengthInDB.ToString() + "���û������ȣ�" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%";
                        bwAsync.ReportProgress( 5 );
                        Thread.Sleep( 5 );
                    }
                    i++;
                }
            }
            dtUID.Dispose();

            //�Ӷ�����ȥ����ǰUID
            lstWaitingUID.Remove( lStartUID );
            //����ǰUID�ӵ���ͷ
            lstWaitingUID.AddFirst( lStartUID );
            //��־
            strLog = DateTime.Now.ToString() + "  " + "��ʼ���û�������ɡ�";
            bwAsync.ReportProgress( 100 );
            Thread.Sleep( 5 );
            lCurrentUID = lStartUID;
            //�Զ���ѭ������
            while (lstWaitingUID.Count > 0)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending) Thread.Sleep( 50 );
                //����ͷȡ��
                lCurrentUID = lstWaitingUID.First.Value;
                lstWaitingUID.RemoveFirst();
                //�����ݿ���л���������Ԫ��
                long lHead = queueBuffer.Dequeue();
                if (lHead > 0)
                    lstWaitingUID.AddLast( lHead );
                #region Ԥ����
                if (lCurrentUID == lStartUID)  //˵������һ��ѭ������
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending) Thread.Sleep( 50 );

                    //��־
                    strLog = DateTime.Now.ToString() + "  " + "��ʼ����֮ǰ���ӵ�������...";
                    bwAsync.ReportProgress( 100 );
                    Thread.Sleep( 5 );

                    User.NewIterate();
                    UserRelation.NewIterate();
                }
                //��־
                strLog = DateTime.Now.ToString() + "  " + "��¼��ǰ�û�ID��" + lCurrentUID.ToString();
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                SysArg.SetCurrentUID( lCurrentUID );
                #endregion
                #region �û�������Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending) Thread.Sleep( 50 );

                //�����ݿ��в����ڵ�ǰ�û��Ļ�����Ϣ������ȡ���������ݿ�
                if (!User.Exists( lCurrentUID ))
                {
                    //��־
                    strLog = DateTime.Now.ToString() + "  " + "���û�" + lCurrentUID.ToString() + "�������ݿ�...";
                    bwAsync.ReportProgress( 100 );
                    crawler.GetUserInfo( lCurrentUID ).Add();
                }
                else
                {
                    //��־
                    strLog = DateTime.Now.ToString() + "  " + "�����û�" + lCurrentUID.ToString() + "������...";
                    bwAsync.ReportProgress( 100 );
                    crawler.GetUserInfo( lCurrentUID ).Update();
                }
                Thread.Sleep( 5 );
                #endregion
                #region �û���ע�б�
                if (blnAsyncCancelled) return;
                while (blnSuspending) Thread.Sleep( 50 );
                //��־                
                strLog = DateTime.Now.ToString() + "  " + "��ȡ�û�" + lCurrentUID.ToString() + "��ע�û�ID�б�...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                //��ȡ��ǰ�û��Ĺ�ע���û�ID����¼��ϵ���������
                LinkedList<long> lstBuffer = crawler.GetFriendsOf( lCurrentUID, -1 );
                //��־
                strLog = DateTime.Now.ToString() + "  " + "����" + lstBuffer.Count.ToString() + "λ��ע�û���";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending) Thread.Sleep( 50 );
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lCurrentUID, lstBuffer.First.Value ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending) Thread.Sleep( 50 );
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "��¼�û�" + lCurrentUID.ToString() + "��ע�û�" + lstBuffer.First.Value.ToString() + "...";
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
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || queueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress( 100 );
                    }
                    else
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "���û�" + lstBuffer.First.Value.ToString() + "������С��ڴ��������" + lstWaitingUID.Count + "���û������ݿ��������" + queueBuffer.Count.ToString() + "���û�";
                        bwAsync.ReportProgress( 100 );
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
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
                //��־
                strLog = DateTime.Now.ToString() + "  " + "��ȡ�û�" + lCurrentUID.ToString() + "��ע�û�ID�б�...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                lstBuffer = crawler.GetFriendsOf( lCurrentUID, 0 );
                //��־
                strLog = DateTime.Now.ToString() + "  " + "����" + lstBuffer.Count.ToString() + "λ��ע�û���";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending) Thread.Sleep( 50 );
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lCurrentUID, lstBuffer.First.Value ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending) Thread.Sleep( 50 );
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "��¼�û�" + lCurrentUID.ToString() + "��ע�û�" + lstBuffer.First.Value.ToString() + "...";
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
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || queueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress( 100 );
                    }
                    else
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "���û�" + lstBuffer.First.Value.ToString() + "������С��ڴ��������" + lstWaitingUID.Count + "���û������ݿ��������" + queueBuffer.Count.ToString() + "���û�";
                        bwAsync.ReportProgress( 100 );
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            queueBuffer.Enqueue( lstBuffer.First.Value );
                    }
                    Thread.Sleep( 5 );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                #region �û���˿�б�
                //��ȡ��ǰ�û��ķ�˿��ID����¼��ϵ���������
                if (blnAsyncCancelled) return;
                while (blnSuspending) Thread.Sleep( 50 );
                //��־
                strLog = DateTime.Now.ToString() + "  " + "��ȡ�û�" + lCurrentUID.ToString() + "�ķ�˿�û�ID�б�...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                lstBuffer = crawler.GetFollowersOf( lCurrentUID, -1 );
                //��־
                strLog = DateTime.Now.ToString() + "  " + "����" + lstBuffer.Count.ToString() + "λ��˿��";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending) Thread.Sleep( 50 );
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lstBuffer.First.Value, lCurrentUID ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending) Thread.Sleep( 50 );
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "��¼�û�" + lstBuffer.First.Value.ToString() + "��ע�û�" + lCurrentUID.ToString() + "...";
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
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || queueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress( 100 );
                    }
                    else
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "���û�" + lstBuffer.First.Value.ToString() + "������С��ڴ��������" + lstWaitingUID.Count + "���û������ݿ��������" + queueBuffer.Count.ToString() + "���û�";
                        bwAsync.ReportProgress( 100 );
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
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
                //��־
                strLog = DateTime.Now.ToString() + "  " + "��ȡ�û�" + lCurrentUID.ToString() + "�ķ�˿�û�ID�б�...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                lstBuffer = crawler.GetFollowersOf( lCurrentUID, 0 );
                //��־
                strLog = DateTime.Now.ToString() + "  " + "����" + lstBuffer.Count.ToString() + "λ��˿��";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending) Thread.Sleep( 50 );
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lstBuffer.First.Value, lCurrentUID ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending) Thread.Sleep( 50 );
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "��¼�û�" + lstBuffer.First.Value.ToString() + "��ע�û�" + lCurrentUID.ToString() + "...";
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
                    //�������
                    if (lstWaitingUID.Contains( lstBuffer.First.Value ) || queueBuffer.Contains( lstBuffer.First.Value ))
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "�û�" + lstBuffer.First.Value.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress( 100 );
                    }
                    else
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "���û�" + lstBuffer.First.Value.ToString() + "������С��ڴ��������" + lstWaitingUID.Count + "���û������ݿ��������" + queueBuffer.Count.ToString() + "���û�";
                        bwAsync.ReportProgress( 100 );
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lstBuffer.First.Value );
                        else
                            queueBuffer.Enqueue( lstBuffer.First.Value );
                    }
                    Thread.Sleep( 5 );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //����ٽ��ո��������UID�����β�����׳���UID
                //��־
                strLog = DateTime.Now.ToString() + "  " + "�û�" + lCurrentUID.ToString() + "����������ȡ��ϣ���������β...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                if (lstWaitingUID.Count < iQueueLength)
                    lstWaitingUID.AddLast( lCurrentUID );
                else
                    queueBuffer.Enqueue( lCurrentUID );
                //��������Ƶ��
                //����û�����Ƶ��
                crawler.AdjustFreq();
                //��־
                strLog = DateTime.Now.ToString() + "  " + "����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + crawler.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + crawler.RemainingHits.ToString() + "��";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
            }
        }

        public void Initialize ()
        {
            //��ʼ����Ӧ����
            blnAsyncCancelled = false;
            if (lstWaitingUID != null) lstWaitingUID.Clear();

            //������ݿ���л���
            queueBuffer.Clear();
        }
    }
}
