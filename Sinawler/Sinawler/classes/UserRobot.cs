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
        private int iPreLoadQueue = (int)(EnumPreLoadQueue.NO_PRELOAD);       //�Ƿ�����ݿ���Ԥ�����û����С�Ĭ��Ϊ����
        private int iInitQueueLength = 100;          //��ʼ���г���

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        public EnumPreLoadQueue PreLoadQueue
        {
            get { return (EnumPreLoadQueue)iPreLoadQueue; }
            set { iPreLoadQueue = (int)value; }
        }

        //���캯������Ҫ������Ӧ������΢��API��������
        public UserRobot ( SinaApiService oAPI ):base(oAPI)
        {
            queueBuffer = new QueueBuffer( QueueBufferTarget.FOR_USER );
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_user.log";
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
                    strLog = DateTime.Now.ToString() + "  " + "��ȡ����ȡ���ݵ��û���ID���������ڴ����...";
                    bwAsync.ReportProgress( 0 );
                    Thread.Sleep( 5 );
                    dtUID = User.GetCrawedUIDTable();
                    break;
                case (int)EnumPreLoadQueue.PRELOAD_ALL_UID:
                    //��־
                    strLog = DateTime.Now.ToString() + "  " + "��ȡ���ݿ��������û���ID���������ڴ����...";
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
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    lUID = Convert.ToInt64( dtUID.Rows[i]["uid"] );
                    if (!lstWaitingUID.Contains( lUID ))
                    {
                        lstWaitingUID.AddLast( lUID );
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "���û�" + lUID.ToString() + "������С��ڴ��������" + lstWaitingUID.Count + "���û������ݿ��������" + queueBuffer.Count.ToString() + "���û������ȣ�" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%";
                        strQueueInfo = DateTime.Now.ToString() + "  " + "�û������˵��ڴ����������" + lstWaitingUID.Count + "���û������ݿ����������" + queueBuffer.Count.ToString() + "���û���";
                        bwAsync.ReportProgress( 5 );
                        Thread.Sleep( 5 );
                    }
                }

                //������ʣ�࣬�����ಿ�ַ������ݿ���л���
                while (i < dtUID.Rows.Count)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    lUID = Convert.ToInt64( dtUID.Rows[i]["uid"] );
                    int iLengthInDB = queueBuffer.Count;
                    if (!queueBuffer.Contains( lUID ))
                    {
                        queueBuffer.Enqueue( lUID );
                        ++iLengthInDB;
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "�ڴ�������������û�" + lUID.ToString() + "�������ݿ���У����ݿ��������" + iLengthInDB.ToString() + "���û������ȣ�" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%";
                        strQueueInfo = DateTime.Now.ToString() + "  " + "�û������˵��ڴ����������" + lstWaitingUID.Count + "���û������ݿ����������" + queueBuffer.Count.ToString() + "���û���";
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
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
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
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }

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
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }

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
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
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
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    long lQueueFirst = lstBuffer.First.Value;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lCurrentUID, lQueueFirst ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep( 50 );
                        }
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "��¼�û�" + lCurrentUID.ToString() + "��ע�û�" + lQueueFirst.ToString() + "...";
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
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    //�������
                    if (lstWaitingUID.Contains( lQueueFirst ) || queueBuffer.Contains( lQueueFirst ))
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "�û�" + lQueueFirst.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress( 100 );
                    }
                    else
                    {
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lQueueFirst );
                        else
                            queueBuffer.Enqueue( lQueueFirst );
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "���û�" + lQueueFirst.ToString() + "������С��ڴ��������" + lstWaitingUID.Count + "���û������ݿ��������" + queueBuffer.Count.ToString() + "���û���";
                        strQueueInfo = DateTime.Now.ToString() + "  " + "�û������˵��ڴ����������" + lstWaitingUID.Count + "���û������ݿ����������" + queueBuffer.Count.ToString() + "���û���";
                        bwAsync.ReportProgress( 100 );
                    }
                    Thread.Sleep( 5 );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                #region �û���˿�б�
                //��ȡ��ǰ�û��ķ�˿��ID����¼��ϵ���������
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
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
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    long lQueueFirst = lstBuffer.First.Value;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lQueueFirst, lCurrentUID ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep( 50 );
                        }
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "��¼�û�" + lQueueFirst.ToString() + "��ע�û�" + lCurrentUID.ToString() + "...";
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
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    //�������
                    if (lstWaitingUID.Contains( lQueueFirst ) || queueBuffer.Contains( lQueueFirst ))
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "�û�" + lQueueFirst.ToString() + "���ڶ�����...";
                        bwAsync.ReportProgress( 100 );
                    }
                    else
                    {
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
                        if (lstWaitingUID.Count < iQueueLength)
                            lstWaitingUID.AddLast( lQueueFirst );
                        else
                            queueBuffer.Enqueue( lQueueFirst );
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "���û�" + lQueueFirst.ToString() + "������С��ڴ��������" + lstWaitingUID.Count + "���û������ݿ��������" + queueBuffer.Count.ToString() + "���û���";
                        strQueueInfo = DateTime.Now.ToString() + "  " + "�û������˵��ڴ����������" + lstWaitingUID.Count + "���û������ݿ����������" + queueBuffer.Count.ToString() + "���û���";
                        bwAsync.ReportProgress( 100 );
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

        public override void Initialize ()
        {
            //��ʼ����Ӧ����
            blnAsyncCancelled = false;
            blnSuspending = false;
            if (lstWaitingUID != null) lstWaitingUID.Clear();

            //������ݿ���л���
            queueBuffer.Clear();
        }
    }
}
