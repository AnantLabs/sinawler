using System;
using System.Collections.Generic;
using System.Text;
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
        private long lCurrentSID = 0;                       //currently processing status id

        //���캯������Ҫ������Ӧ������΢��API��������
        public StatusRobot()
            : base(SysArgFor.STATUS)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_status.log";
            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
            queueStatus = GlobalPool.StatusQueue;
        }

        /// <summary>
        /// ����������ȡ��΢������
        /// </summary>
        private void SaveStatus(Status status)
        {
            lCurrentSID = status.status_id;
            if (!Status.Exists(lCurrentSID))
            {
                //��־
                Log("Saving Status " + lCurrentSID.ToString() + " into database...");
                status.Add();
            }

            if (queueStatus.Enqueue(lCurrentSID))
                Log("Adding Status " + lCurrentSID.ToString() + " to status queue...");

            //����΢����ת������ת��΢������
            if (status.retweeted_status != null)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(10);
                }

                //��־
                Log("Status " + status.retweeted_status.status_id.ToString() + " is retweeted by Status " + lCurrentSID.ToString() + ", saving it into database...");

                if (!Status.Exists(status.retweeted_status.status_id))
                {
                    status.retweeted_status.Add();

                    //��־
                    Log("Retweeted Status " + status.retweeted_status.status_id.ToString() + " saved.");
                }
                else
                {
                    //��־
                    Log("Retweeted Status " + status.retweeted_status.status_id.ToString() + " exists.");
                }

                if (queueStatus.Enqueue(status.retweeted_status.status_id))
                    Log("Adding retweeted Status " + status.retweeted_status.status_id.ToString() + " to status queue...");

                if (queueUserForUserRelationRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("Adding User " + status.retweeted_status.user.user_id.ToString() + " to the user queue of User Relation Robot...");
                if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("Adding User " + status.retweeted_status.user.user_id.ToString() + " to the user queue of User Information Robot...");
                if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("Adding User " + status.retweeted_status.user.user_id.ToString() + " to the user queue of User Tag Robot...");
                if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue(status.retweeted_status.user.user_id))
                    Log("Adding User " + status.retweeted_status.user.user_id.ToString() + " to the user queue of Status Robot...");
                if (!User.ExistInDB(status.retweeted_status.user.user_id))
                {
                    Log("Saving User " + status.retweeted_status.user.user_id.ToString() + " into database...");
                    status.retweeted_status.user.Add();
                }
            }
        }

        /// <summary>
        /// ��ʼ��ȡ΢������
        /// </summary>
        public void Start()
        {
            //��ȡ�ϴ���ֹ�����û�ID�����
            long lLastUID = SysArg.GetCurrentID(SysArgFor.STATUS);
            if (lLastUID > 0) queueUserForStatusRobot.Enqueue(lLastUID);
            while (queueUserForStatusRobot.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(GlobalPool.SleepMsForThread);   //������Ϊ�գ���ȴ�
            }

            AdjustRealFreq();
            SetCrawlerFreq();
            Log("The initial requesting interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString() + " requests left this hour.");

            //�Զ�������ѭ�����У�ֱ���в�����ͣ��ֹͣ
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }

                //����ͷȡ��
                //lCurrentID = queueUserForStatusRobot.RollQueue();
                lCurrentID = queueUserForStatusRobot.FirstValue;

                //��־
                Log("Recording current UserID: " + lCurrentID.ToString() + "...");
                SysArg.SetCurrentID(lCurrentID, SysArgFor.STATUS);

                #region �û�΢����Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }
                //��־
                Log("Getting the latest Status ID of User " + lCurrentID.ToString() + "...");
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID
                long lCurrentSID = Status.GetLastStatusIDOf(lCurrentID);

                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }

                Status status;
                #region ����΢��
                //��־
                Log("Crawling statuses after Status " + lCurrentSID.ToString() + " of User " + lCurrentID.ToString() + "...");
                //��ȡ���ݿ��е�ǰ�û�����һ��΢����ID֮���΢�����������ݿ�
                LinkedList<Status> lstStatus = crawler.GetStatusesOfSince(lCurrentID, lCurrentSID);
                if (lstStatus.Count>0 && lstStatus.First.Value.status_id > 0)
                {
                    //��־
                    Log(lstStatus.Count.ToString() + " statuses crawled.");
                    //��־
                    AdjustFreq();
                    SetCrawlerFreq();
                    Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString() + " requests left this hour.");

                    while (lstStatus.Count > 0)
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(GlobalPool.SleepMsForThread);
                        }
                        status = lstStatus.First.Value;
                        SaveStatus(status);
                        lstStatus.RemoveFirst();
                    }
                    queueUserForStatusRobot.RollQueue();
                    //��־
                    Log("Statuses of User " + lCurrentID.ToString() + " crawled.");
                }
                else if (lstStatus.Count > 0 && lstStatus.First.Value.status_id == -1)
                {
                    lstStatus.Clear();
                    int iSleepSeconds = GlobalPool.GetAPI(SysArgFor.USER_INFO).ResetTimeInSeconds;
                    Log("Service is forbidden now. I will wait for " + iSleepSeconds.ToString() + "s to continue...");
                    for (int i = 0; i < iSleepSeconds; i++)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(1000);
                    }
                    continue;
                }
                else
                {
                    queueUserForStatusRobot.RollQueue();
                    //��־
                    Log("Statuses of User " + lCurrentID.ToString() + " crawled.");
                }
                #endregion
                #endregion
            }
        }

        public override void Initialize()
        {
            //��ʼ����Ӧ����
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueUserForStatusRobot.Initialize();
        }
    }
}
