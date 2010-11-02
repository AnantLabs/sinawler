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
        private UserQueue queueUserForUserRobot;            //�û�������ʹ�õ��û���������
        private UserQueue queueUserForStatusRobot;          //΢��������ʹ�õ��û���������
        private StatusQueue queueStatus;        //΢����������

        //���캯������Ҫ������Ӧ������΢��API��������
        public StatusRobot ( SinaApiService oAPI,UserQueue qUserForUserRobot, UserQueue qUserForStatusRobot,StatusQueue qStatus ):base(oAPI)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
            queueUserForUserRobot = qUserForUserRobot;
            queueUserForStatusRobot = qUserForStatusRobot;
            queueStatus = qStatus;
        }

        /// <summary>
        /// ��ʼ��ȡ΢������
        /// </summary>
        public void Start()
        {
            while (queueUserForStatusRobot.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(50);   //������Ϊ�գ���ȴ�
            }
            long lStartUserID = queueUserForStatusRobot.FirstValue;
            long lCurrentUserID = 0;
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
                lCurrentUserID = queueUserForStatusRobot.RollQueue();

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

                    if(queueStatus.Enqueue(lCurrentID))
                        Log( "��΢��" + lCurrentID.ToString() + "����΢�����С�" );
                    else
                        Log( "΢��" + lCurrentID.ToString() + "����΢�������С�" );
                    
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
                            status.retweeted_status.Add();

                            //��־
                            Log("ת��΢��" + status.retweeted_status.status_id.ToString() + "�ѱ��档");
                        }
                        else
                        {
                            //��־
                            Log("ת��΢��" + status.retweeted_status.status_id.ToString() + "�Ѵ��ڡ�");
                        }

                        if (queueStatus.Enqueue( status.retweeted_status.status_id ))
                            Log( "��ת��΢��" + status.retweeted_status.status_id.ToString() + "����΢�����С�" );
                        else
                            Log( "ת��΢��" + status.retweeted_status.status_id.ToString() + "����΢�������С�" );

                        if (queueUserForUserRobot.Enqueue( status.retweeted_status.user_id ))
                            Log( "���û�" + status.retweeted_status.user_id.ToString() + "�����û������˵��û����С�" );
                        else
                            Log( "�û�" + status.retweeted_status.user_id.ToString() + "�����û������˵��û������С�" );

                        if (queueUserForStatusRobot.Enqueue( status.retweeted_status.user_id ))
                            Log( "���û�" + status.retweeted_status.user_id.ToString() + "����΢�������˵��û����С�" );
                        else
                            Log( "�û�" + status.retweeted_status.user_id.ToString() + "����΢�������˵��û������С�" );
                    }
                }
                #endregion
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
            crawler.StopCrawling = false;

            queueUserForUserRobot.Initialize();
            queueUserForStatusRobot.Initialize();
            queueStatus.Initialize();
        }
    }
}
