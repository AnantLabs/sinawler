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
            long lStartUID = 0;
            long lCurrentUID = 0;
            //��ȡ��һ��UID����ȡ��֮�󣬼�ΪStartID�����ڱȽϲ�ȷ����������
            while (lstWaitingID.Count == 0) bwAsync.ReportProgress(0);//�������¼����Ӷ����û������˴��ݵ��û������л�ȡ��ͷ
            lStartUID = lstWaitingID.First.Value;

            //�Զ�������ѭ�����У�ֱ���в�����ͣ��ֹͣ
            while(true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
                //����ͷȡ��
                lCurrentUID = lstWaitingID.First.Value;
                lstWaitingID.RemoveFirst();
                //�����ݿ���л���������Ԫ��
                long lHead = queueBuffer.Dequeue();
                if (lHead > 0)
                    lstWaitingID.AddLast( lHead );
                blnOneIDCompleted = false;  //��ʼ�µ�ID
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

                    Status.NewIterate();
                }
                #endregion                
                #region �û�΢����Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
                //��־
                strLog = DateTime.Now.ToString() + "  " + "��ȡ���ݿ����û�" + lCurrentUID.ToString() + "����һ��΢����ID...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID
                lCurrentID = Status.GetLastStatusIDOf( lCurrentUID );

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
                //��־
                strLog = DateTime.Now.ToString() + "  " + "��ȡ�û�" + lCurrentUID.ToString() + "��ID��" + lCurrentID.ToString() + "֮���΢��...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID֮���΢�����������ݿ�
                List<Status> lstStatus = crawler.GetStatusesOfSince(lCurrentUID, lCurrentID);
                //��־
                strLog = DateTime.Now.ToString() + "  " + "����" + lstStatus.Count.ToString() + "��΢����";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );

                foreach (Status status in lstStatus)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    lCurrentID = status.status_id;
                    if (!Status.Exists(lCurrentID))
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "��΢��" + lCurrentID.ToString() + "�������ݿ�...";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );
                        status.Add();
                    }
                    //����΢����ת������ת��΢��ID���
                    if (status.retweeted_status_id > 0)
                    {
                        //��־
                        strLog = DateTime.Now.ToString() + "  " + "΢��" + lCurrentID.ToString() + "��ת��΢������ת��΢��" + status.retweeted_status_id.ToString() + "������еȴ���ȡ...";
                        bwAsync.ReportProgress( 100 );
                        Thread.Sleep( 5 );
                        lstRetweetedStatus.AddLast( status.retweeted_status_id );
                    }
                }
                #endregion                
                #region ��ȡ��ȡ��ת��΢��
                while(lstRetweetedStatus.Count>0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }

                    lCurrentID = lstRetweetedStatus.First.Value;
                    lstRetweetedStatus.RemoveFirst();

                    Status status = crawler.GetStatus(lCurrentID);
                    if(status!=null)
                    {
                        if (!Status.Exists(lCurrentID))
                        {
                            //��־
                            strLog = DateTime.Now.ToString() + "  " + "��΢��" + lCurrentID.ToString() + "�������ݿ�...";
                            bwAsync.ReportProgress( 100 );
                            Thread.Sleep( 5 );
                            status.Add();
                        }
                        //����΢����ת������ת��΢��ID���
                        if (status.retweeted_status_id > 0)
                        {
                            //��־
                            strLog = DateTime.Now.ToString() + "  " + "΢��" + lCurrentID.ToString() + "��ת��΢������ת��΢��" + status.retweeted_status_id.ToString() + "����΢������...";
                            bwAsync.ReportProgress( 100 );
                            Thread.Sleep( 5 );
                            lstRetweetedStatus.AddLast( status.retweeted_status_id );
                        }
                    }
                }
                #endregion
                blnOneIDCompleted = true;  //���һ��ID
                //����ٽ��ո��������UID�����β
                //��־
                strLog = DateTime.Now.ToString() + "  " + "�û�" + lCurrentUID.ToString() + "����������ȡ��ϣ���������β...";
                bwAsync.ReportProgress( 100 );
                Thread.Sleep( 5 );
                //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                if (lstWaitingID.Count < iQueueLength)
                    lstWaitingID.AddLast( lCurrentUID );
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
            if (lstWaitingID != null) lstWaitingID.Clear();

            //������ݿ���л���
            queueBuffer.Clear();
        }
    }
}
