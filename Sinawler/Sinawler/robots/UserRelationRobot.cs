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
using System.Xml;

namespace Sinawler
{
    class UserRelationRobot : RobotBase
    {
        private UserQueue queueUserForUserInfoRobot;        //�û���Ϣ������ʹ�õ��û���������
        private UserQueue queueUserForUserRelationRobot;    //�û���ϵ������ʹ�õ��û���������
        private UserQueue queueUserForUserTagRobot;         //�û���ǩ������ʹ�õ��û���������
        private UserQueue queueUserForStatusRobot;          //΢��������ʹ�õ��û���������
        private long lQueueBufferFirst = 0;   //���ڼ�¼��ȡ�Ĺ�ע�û��б���˿�û��б�Ķ�ͷֵ

        //���캯������Ҫ������Ӧ������΢��API��������
        public UserRelationRobot()
            : base(SysArgFor.USER_RELATION)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userRelation.log";
            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
        }

        /// <summary>
        /// ��ָ����UserIDΪ��㿪ʼ����
        /// </summary>
        /// <param name="lUid"></param>
        public void Start(long lStartUserID)
        {
            if (lStartUserID == 0) return;
            AdjustFreq();
            Log("The initial request interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString()+" requests remained this hour.");

            //����ʼUserID���
            queueUserForUserRelationRobot.Enqueue(lStartUserID);
            queueUserForUserInfoRobot.Enqueue(lStartUserID);
            queueUserForUserTagRobot.Enqueue(lStartUserID);
            queueUserForStatusRobot.Enqueue(lStartUserID);
            lCurrentID = lStartUserID;

            //�Զ�������ѭ�����У�ֱ���в�����ͣ��ֹͣ
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //����ͷȡ��
                lCurrentID = queueUserForUserRelationRobot.RollQueue();

                //��־
                Log("Recording current UserID��" + lCurrentID.ToString());
                SysArg.SetCurrentID(lCurrentID,SysArgFor.USER_RELATION);

                #region �û���ע�б�
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //��־                
                Log("Crawling the followings of User " + lCurrentID.ToString() + "...");
                //��ȡ��ǰ�û��Ĺ�ע���û�ID����¼��ϵ���������
                LinkedList<long> lstBuffer = crawler.GetFriendsOf(lCurrentID, -1);
                //��־
                Log(lstBuffer.Count.ToString() + " followings crawled.");

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    lQueueBufferFirst = lstBuffer.First.Value;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists(lCurrentID, lQueueBufferFirst))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(50);
                        }
                        //��־
                        Log("Recording User " + lCurrentID.ToString() + " follows User " + lQueueBufferFirst.ToString() + "...");
                        UserRelation ur = new UserRelation();
                        ur.source_user_id = lCurrentID;
                        ur.target_user_id = lstBuffer.First.Value;
                        ur.relation_state = Convert.ToInt32(RelationState.RelationExists);
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    //�������
                    if (queueUserForUserRelationRobot.Enqueue(lQueueBufferFirst))
                        //��־
                        Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of User Relation Robot.");
                    if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue(lQueueBufferFirst))
                        //��־
                        Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of User Information Robot.");
                    if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue(lQueueBufferFirst))
                        //��־
                        Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of User Tag Robot.");
                    if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue(lQueueBufferFirst))
                        //��־
                        Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of Status Robot.");
                    lstBuffer.RemoveFirst();
                }

                //��־
                AdjustFreq();
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + api.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + api.RemainingHits.ToString() + "��");
                #endregion
                #region �û���˿�б�
                //��ȡ��ǰ�û��ķ�˿��ID����¼��ϵ���������
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //��־
                Log("��ȡ�û�" + lCurrentID.ToString() + "�ķ�˿�û�ID�б�...");
                lstBuffer = crawler.GetFollowersOf(lCurrentID, -1);
                //��־
                Log("����" + lstBuffer.Count.ToString() + "λ��˿��");

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    lQueueBufferFirst = lstBuffer.First.Value;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists(lQueueBufferFirst, lCurrentID))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(50);
                        }
                        //��־
                        Log("��¼�û�" + lQueueBufferFirst.ToString() + "��ע�û�" + lCurrentID.ToString() + "...");
                        UserRelation ur = new UserRelation();
                        ur.source_user_id = lstBuffer.First.Value;
                        ur.target_user_id = lCurrentID;
                        ur.relation_state = Convert.ToInt32(RelationState.RelationExists);
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    //�������
                    if (queueUserForUserRelationRobot.Enqueue(lQueueBufferFirst))
                        //��־
                        Log("���û�" + lQueueBufferFirst.ToString() + "�����û���ϵ�����˵��û����С�");
                    if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue(lQueueBufferFirst))
                        //��־
                        Log("���û�" + lQueueBufferFirst.ToString() + "�����û���Ϣ�����˵��û����С�");
                    if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue(lQueueBufferFirst))
                        //��־
                        Log("���û�" + lQueueBufferFirst.ToString() + "�����û���ǩ�����˵��û����С�");
                    if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue(lQueueBufferFirst))
                        //��־
                        Log("���û�" + lQueueBufferFirst.ToString() + "����΢�������˵��û����С�");
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //����ٽ��ո��������UserID�����β
                //��־
                Log("�û�" + lCurrentID.ToString() + "�Ĺ�ϵ����ȡ��ϡ�");
                //��־
                AdjustFreq();
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + api.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + api.RemainingHits.ToString() + "��");
            }
        }

        public override void Initialize()
        {
            //��ʼ����Ӧ����
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueUserForUserRelationRobot.Initialize();
        }

        sealed protected override void AdjustFreq()
        {
            base.AdjustRealFreq();
            SetCrawlerFreq();
        }
    }
}
