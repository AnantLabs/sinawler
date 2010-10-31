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
        private long lRetweetedUID = 0;     //ת��΢����UID�����ڴ��ݸ��û�������

        public long CurrentSID
        { get { return lCurrentID; } }

        public long CurrentRetweetedUID
        { get { return lRetweetedUID; } }

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
                Thread.Sleep(10);   //������Ϊ�գ���ȴ�
            }
            long lStartUID = lstWaitingID.First.Value;
            long lCurrentUID = 0;
            long lHead = 0;
            //�Զ�������ѭ�����У�ֱ���в�����ͣ��ֹͣ
            while(true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }

                //����ͷȡ��
                lCurrentUID = lstWaitingID.First.Value;
                //�����ݿ���л���������Ԫ��
                lHead = queueBuffer.FirstValue;
                if (lHead > 0)
                {
                    lstWaitingID.AddLast( lHead );
                    queueBuffer.Remove( lHead );
                }
                //�����β�����Ӷ�ͷ�Ƴ�
                if (lstWaitingID.Count <= iQueueLength)
                    lstWaitingID.AddLast( lCurrentUID );
                else
                    queueBuffer.Enqueue( lCurrentUID );
                lstWaitingID.RemoveFirst();

                #region Ԥ����
                if (lCurrentUID == lStartUID)  //˵������һ��ѭ������
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
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
                    Thread.Sleep(10);
                }
                //��־
                Log("��ȡ���ݿ����û�" + lCurrentUID.ToString() + "����һ��΢����ID...");
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID
                lCurrentID = Status.GetLastStatusIDOf( lCurrentUID );

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }
                //��־
                Log("��ȡ�û�" + lCurrentUID.ToString() + "��ID��" + lCurrentID.ToString() + "֮���΢��...");
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID֮���΢�����������ݿ�
                List<Status> lstStatus = crawler.GetStatusesOfSince(lCurrentUID, lCurrentID);
                //��־
                Log("����" + lstStatus.Count.ToString() + "��΢����");

                foreach (Status status in lstStatus)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(10);
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

                        if (!Status.Exists( status.retweeted_status.status_id ))
                        {
                            lCurrentID = status.retweeted_status.status_id;
                            lRetweetedUID = status.retweeted_status.uid;
                            status.retweeted_status.Add();

                            //��־
                            Log("ת��΢��" + status.retweeted_status.status_id.ToString() + "�ѱ��档");
                        }
                        else
                        {
                            //��־
                            Log("ת��΢��" + status.retweeted_status.status_id.ToString() + "�Ѵ��ڡ�");
                        }
                    }
                }
                #endregion
                //����ٽ��ո��������UID�����β
                //��־
                Log("�û�" + lCurrentUID.ToString() + "����������ȡ��ϡ�");
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
