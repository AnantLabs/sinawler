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
    class StatusRobot : RobotBase
    {
        private UserQueue queueUserForUserInfoRobot;        //�û���Ϣ������ʹ�õ��û���������
        private UserQueue queueUserForUserRelationRobot;    //�û���ϵ������ʹ�õ��û���������
        private UserQueue queueUserForUserTagRobot;         //�û���ǩ������ʹ�õ��û���������
        private UserQueue queueUserForStatusRobot;          //΢��������ʹ�õ��û���������
        private StatusQueue queueStatus;                    //΢����������
        private UserBuffer oUserBuffer;                     //the buffer queue of users
        private long lCurrentSID = 0;                       //currently processing status id

        //���캯������Ҫ������Ӧ������΢��API��������
        public StatusRobot ()
            : base(SysArgFor.STATUS)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
            queueStatus = GlobalPool.StatusQueue;
            oUserBuffer = GlobalPool.UserBuffer;
        }

        /// <summary>
        /// ����������ȡ��΢������
        /// </summary>
        private void SaveStatus ( Status status )
        {
            lCurrentSID = status.status_id;
            if (!Status.Exists(lCurrentSID))
            {
                //��־
                Log("��΢��" + lCurrentSID.ToString() + "�������ݿ�...");
                status.Add();
            }

            if (queueStatus.Enqueue(lCurrentSID))
                Log("��΢��" + lCurrentSID.ToString() + "����΢�����С�");

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
                Log("΢��" + lCurrentSID.ToString() + "��ת��΢������ת��΢��" + status.retweeted_status.status_id.ToString() + "�������ݿ�...");

                if (!Status.Exists( status.retweeted_status.status_id ))
                {
                    status.retweeted_status.Add();

                    //��־
                    Log( "ת��΢��" + status.retweeted_status.status_id.ToString() + "�ѱ��档" );
                }
                else
                {
                    //��־
                    Log( "ת��΢��" + status.retweeted_status.status_id.ToString() + "�Ѵ��ڡ�" );
                }

                if (queueStatus.Enqueue( status.retweeted_status.status_id ))
                    Log( "��ת��΢��" + status.retweeted_status.status_id.ToString() + "����΢�����С�" );

                if (queueUserForUserRelationRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("���û�" + status.retweeted_status.user.user_id.ToString() + "�����û���ϵ�����˵��û����С�");
                if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log( "���û�" + status.retweeted_status.user.user_id.ToString() + "�����û���Ϣ�����˵��û����С�" );
                if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("���û�" + status.retweeted_status.user.user_id.ToString() + "�����û���ǩ�����˵��û����С�");
                if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("���û�" + status.retweeted_status.user.user_id.ToString() + "����΢�������˵��û����С�");
                //add the user into the buffer only when the user does not exist in the queue for userInfo
                if (GlobalPool.UserInfoRobotEnabled && !queueUserForUserInfoRobot.QueueExists(status.retweeted_status.user.user_id) && oUserBuffer.Enqueue(status.retweeted_status.user))
                    Log("���û�" + status.retweeted_status.user.user_id.ToString() + "�����û�����ء�");
            }
        }

        /// <summary>
        /// ��ʼ��ȡ΢������
        /// </summary>
        public void Start ()
        {
            //��ȡ�ϴ���ֹ�����û�ID�����
            long lLastUID = SysArg.GetCurrentID(SysArgFor.STATUS);
            if (lLastUID > 0) queueUserForStatusRobot.Enqueue(lLastUID);
            while (queueUserForStatusRobot.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep( 50 );   //������Ϊ�գ���ȴ�
            }
            Thread.Sleep(500);  //waiting that user relation robot update list data

            SetCrawlerFreq();
            Log("��ʼ������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + api.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + api.RemainingHits.ToString() + "��");

            //�Զ�������ѭ�����У�ֱ���в�����ͣ��ֹͣ
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }

                //����ͷȡ��
                lCurrentID = queueUserForStatusRobot.RollQueue();

                //��־
                Log("��¼��ǰ�û�ID��" + lCurrentID.ToString());
                SysArg.SetCurrentID(lCurrentID, SysArgFor.STATUS);

                #region �û�΢����Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }
                //��־
                Log("��ȡ���ݿ����û�" + lCurrentID.ToString() + "����һ��΢����ID...");
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID
                long lCurrentSID = Status.GetLastStatusIDOf(lCurrentID);

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }

                Status status;
                #region ����΢��
                //��־
                Log("��ȡ�û�" + lCurrentID.ToString() + "��ID��" + lCurrentSID.ToString() + "֮���΢��...");
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID֮���΢�����������ݿ�
                LinkedList<Status> lstStatus = crawler.GetStatusesOfSince(lCurrentID, lCurrentSID);
                //��־
                Log( "����" + lstStatus.Count.ToString() + "��΢����" );

                while (lstStatus.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    status = lstStatus.First.Value;
                    SaveStatus( status );
                    lstStatus.RemoveFirst();
                }
                #endregion
                //}
                #endregion
                //��־
                Log( "�û�" + lCurrentID.ToString() + "��΢����������ȡ��ϡ�" );
                //��־
                AdjustFreq();
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + api.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + api.RemainingHits.ToString() + "��");
            }
        }

        public override void Initialize ()
        {
            //��ʼ����Ӧ����
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueUserForStatusRobot.Initialize();
        }

        sealed protected override void AdjustFreq()
        {
            base.AdjustFreq();
            SetCrawlerFreq();
        }
    }
}
