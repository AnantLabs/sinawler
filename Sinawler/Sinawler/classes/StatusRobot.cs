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
    class StatusRobot:RobotBase
    {
        private LinkedList<long> lstRetweetedStatus = new LinkedList<long>();   //ת��΢��ID
        private long lRetweetedUserID = 0;     //ת��΢����UserID�����ڴ��ݸ��û�������

        public long CurrentSID
        { get { return lCurrentID; } }

        public long CurrentRetweetedUserID
        { get { return lRetweetedUserID; } }

        //���캯������Ҫ������Ӧ������΢��API��������
        public StatusRobot ( SinaApiService oAPI ):base(oAPI)
        {
            queueBuffer = new QueueBuffer( QueueBufferTarget.FOR_STATUS );
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
        }

        /// <summary>
        /// ��ʼ��ȡ΢������
        /// </summary>
        public void Start()
        {
            //�������û����У���ȫ����UserRobot���ݹ���
            while (lstWaitingID.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(50);   //������Ϊ�գ���ȴ�
            }
            long lStartUserID = lstWaitingID.First.Value;
            long lCurrentUserID = 0;
            long lHead = 0;
            //�Զ�������ѭ�����У�ֱ���в�����ͣ��ֹͣ
            while(true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }

                //����ͷȡ��
                lCurrentUserID = lstWaitingID.First.Value;
                //�����ݿ���л���������Ԫ��
                lHead = queueBuffer.FirstValue;
                if (lHead > 0)
                {
                    lstWaitingID.AddLast( lHead );
                    queueBuffer.Remove( lHead );
                }
                //�����β�����Ӷ�ͷ�Ƴ�
                if (lstWaitingID.Count <= iQueueLength)
                    lstWaitingID.AddLast( lCurrentUserID );
                else
                    queueBuffer.Enqueue( lCurrentUserID );
                lstWaitingID.RemoveFirst();

                #region Ԥ����
                if (lCurrentUserID == lStartUserID)  //˵������һ��ѭ������
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }

                    //��־
                    Log("��ʼ����֮ǰ���ӵ�������...");

                    Status.NewIterate();
                }
                #endregion                
                #region �û�΢����Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //��־
                Log("��ȡ���ݿ����û�" + lCurrentUserID.ToString() + "����һ��΢����ID...");
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID
                lCurrentID = Status.GetLastStatusIDOf( lCurrentUserID );

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //��־
                Log("��ȡ�û�" + lCurrentUserID.ToString() + "��ID��" + lCurrentID.ToString() + "֮���΢��...");
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID֮���΢�����������ݿ�
                List<Status> lstStatus = crawler.GetStatusesOfSince(lCurrentUserID, lCurrentID);
                //��־
                Log("����" + lstStatus.Count.ToString() + "��΢����");

                foreach (Status status in lstStatus)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    lCurrentID = status.status_id;
                    if (!Status.Exists(lCurrentID))
                    {
                        //��־
                        Log("��΢��" + lCurrentID.ToString() + "�������ݿ�...");
                        status.Add();
                    }
                    
                    //����΢����ת������ת��΢������
                    if (status.retweeted_status != null)
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep( 10 );
                        }

                        //��־
                        Log("΢��" + lCurrentID.ToString() + "��ת��΢������ת��΢��" + status.retweeted_status.status_id.ToString() + "�������ݿ�...");

                        lCurrentID = status.retweeted_status.status_id;
                        lRetweetedUserID = status.retweeted_status.user_id;
                        if (!Status.Exists( status.retweeted_status.status_id ))
                        {   
                            status.retweeted_status.Add();

                            //��־
                            Log("ת��΢��" + status.retweeted_status.status_id.ToString() + "�ѱ��档");
                        }
                        else
                        {
                            //��־
                            Log("ת��΢��" + status.retweeted_status.status_id.ToString() + "�Ѵ��ڡ�");
                        }

                        //���������ӣ���ʵ���Ǻ���Enqueue�ĵ��ã���Ϊ�˼�¼��־�������øú����������Լ�ʵ��
                        if (!QueueExists( lRetweetedUserID ))
                        {
                            Log( "���û�" + lRetweetedUserID.ToString() + "�������..." );
                            if (lstWaitingID.Count < iQueueLength)
                                lstWaitingID.AddLast( lRetweetedUserID );
                            else
                                queueBuffer.Enqueue( lRetweetedUserID );
                        }
                        else
                            Log( "�û�" + lRetweetedUserID.ToString() + "���ڶ����С�" );
                    }
                }
                #endregion
                //����ٽ��ո��������UserID�����β
                //��־
                Log("�û�" + lCurrentUserID.ToString() + "����������ȡ��ϡ�");
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
            if (lstRetweetedStatus != null) lstRetweetedStatus.Clear();

            //������ݿ���л���
            queueBuffer.Clear();
        }
    }
}
