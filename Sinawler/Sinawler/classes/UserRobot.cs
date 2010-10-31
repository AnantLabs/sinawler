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

        public long CurrentUID
        { get { return lCurrentID; } }

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

            long lQueueFirst = 0;   //��ͷֵ
            User user;

            #region Ԥ���ض���
            //����ѡ�ѡ������û����еķ���
            DataTable dtUID=new DataTable();

            switch (iPreLoadQueue)
            {
                case (int)EnumPreLoadQueue.PRELOAD_UID:
                    //��־
                    Log("��ȡ����ȡ���ݵ��û���ID���������ڴ����...");
                    dtUID = User.GetCrawedUIDTable();
                    break;
                case (int)EnumPreLoadQueue.PRELOAD_ALL_UID:
                    //��־
                    Log("��ȡ���ݿ��������û���ID���������ڴ����...");
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
                        //��־
                        Log("���û�" + lUID.ToString() + "������С��ڴ����������" + lstWaitingID.Count.ToString() + "���û������ݿ����������" + queueBuffer.Count.ToString() + "���û������ȣ�" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%");
                    }
                }

                //������ʣ�࣬�����ಿ�ַ������ݿ���л���
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
                        //��־
                        Log("�ڴ�������������û�" + lUID.ToString() + "�������ݿ���У����ݿ����������" + iLengthInDB.ToString() + "���û������ȣ�" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%");
                    }
                    i++;
                }
            }
            dtUID.Dispose();
            #endregion

            //�Ӷ�����ȥ����ǰUID
            lstWaitingID.Remove( lStartUID );
            //����ǰUID�ӵ���ͷ
            lstWaitingID.AddFirst( lStartUID );
            //��־
            Log("��ʼ���û�������ɡ�");
            lCurrentID = lStartUID;
            long lHead = 0;
            //�Զ���ѭ������
            while (lstWaitingID.Count > 0)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }
                //����ͷȡ��
                lCurrentID = lstWaitingID.First.Value;                
                //�����ݿ���л���������Ԫ��
                lHead = queueBuffer.FirstValue;
                if (lHead > 0)
                {
                    lstWaitingID.AddLast( lHead );
                    queueBuffer.Remove( lHead );
                }
                //�����β�����Ӷ�ͷ�Ƴ�
                if (lstWaitingID.Count <= iQueueLength)
                    lstWaitingID.AddLast( lCurrentID );
                else
                    queueBuffer.Enqueue( lCurrentID );
                lstWaitingID.RemoveFirst();
                
                #region Ԥ����
                if (lCurrentID == lStartUID)  //˵������һ��ѭ������
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }

                    //��־
                    Log("��ʼ����֮ǰ���ӵ�������...");

                    User.NewIterate();
                    UserRelation.NewIterate();
                }
                //��־
                Log("��¼��ǰ�û�ID��" + lCurrentID.ToString());
                SysArg.SetCurrentUID( lCurrentID );
                #endregion
                #region �û�������Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }

                user = crawler.GetUserInfo( lCurrentID );
                if (user.uid > 0)
                {
                    //�����ݿ��в����ڵ�ǰ�û��Ļ�����Ϣ������ȡ���������ݿ�
                    if (!User.Exists( lCurrentID ))
                    {
                        //��־
                        Log("���û�" + lCurrentID.ToString() + "�������ݿ�...");
                        user.Add();
                    }
                    else
                    {
                        //��־
                        Log("�����û�" + lCurrentID.ToString() + "������...");
                        user.Update();
                    }
                }

                //��־
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + crawler.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + crawler.RemainingHits.ToString() + "��");
                #endregion
                #region �û���ע�б�
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }
                //��־                
                Log("��ȡ�û�" + lCurrentID.ToString() + "��ע�û�ID�б�...");
                //��ȡ��ǰ�û��Ĺ�ע���û�ID����¼��ϵ���������
                LinkedList<long> lstBuffer = crawler.GetFriendsOf( lCurrentID, -1 );
                //��־
                Log("����" + lstBuffer.Count.ToString() + "λ��ע�û���");

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }
                    lQueueFirst = lstBuffer.First.Value;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lCurrentID, lQueueFirst ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(10);
                        }
                        //��־
                        Log("��¼�û�" + lCurrentID.ToString() + "��ע�û�" + lQueueFirst.ToString() + "...");
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
                    //�������
                    if (lstWaitingID.Contains( lQueueFirst ) || queueBuffer.Contains( lQueueFirst ))
                    {
                        //��־
                        Log("�û�" + lQueueFirst.ToString() + "���ڶ�����...");
                    }
                    else
                    {
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
                        if (lstWaitingID.Count < iQueueLength)
                            lstWaitingID.AddLast( lQueueFirst );
                        else
                            queueBuffer.Enqueue( lQueueFirst );
                        //��־
                        Log("���û�" + lQueueFirst.ToString() + "������С��ڴ����������" + lstWaitingID.Count.ToString() + "���û������ݿ����������" + queueBuffer.Count.ToString() + "���û�");
                    }
                    lstBuffer.RemoveFirst();
                }

                //��־
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + crawler.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + crawler.RemainingHits.ToString() + "��");
                #endregion
                #region �û���˿�б�
                //��ȡ��ǰ�û��ķ�˿��ID����¼��ϵ���������
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }
                //��־
                Log("��ȡ�û�" + lCurrentID.ToString() + "�ķ�˿�û�ID�б�...");
                lstBuffer = crawler.GetFollowersOf( lCurrentID, -1 );
                //��־
                Log("����" + lstBuffer.Count.ToString() + "λ��˿��");

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
                    }
                    lQueueFirst = lstBuffer.First.Value;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lQueueFirst, lCurrentID ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(10);
                        }
                        //��־
                        Log("��¼�û�" + lQueueFirst.ToString() + "��ע�û�" + lCurrentID.ToString() + "...");
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
                    //�������
                    if (lstWaitingID.Contains( lQueueFirst ) || queueBuffer.Contains( lQueueFirst ))
                    {
                        //��־
                        Log("�û�" + lQueueFirst.ToString() + "���ڶ�����...");
                    }
                    else
                    {
                        //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                        //����ʹ�����ݿ���л���
                        if (lstWaitingID.Count < iQueueLength)
                            lstWaitingID.AddLast( lQueueFirst );
                        else
                            queueBuffer.Enqueue( lQueueFirst );
                        //��־
                        Log("���û�" + lQueueFirst.ToString() + "������С��ڴ����������" + lstWaitingID.Count.ToString() + "���û������ݿ����������" + queueBuffer.Count.ToString() + "���û�");
                    }
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //����ٽ��ո��������UID�����β
                //��־
                Log("�û�" + lCurrentID.ToString() + "����������ȡ��ϡ�");
                //��־
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + crawler.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + crawler.RemainingHits.ToString() + "��");
            }
        }

        public override void Initialize ()
        {
            //��ʼ����Ӧ����
            blnAsyncCancelled = false;
            blnSuspending = false;
            if (lstWaitingID != null) lstWaitingID.Clear();

            //������ݿ���л���
            queueBuffer.Clear();
        }
    }
}
