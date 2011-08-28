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
        private bool blnConfirmRelationship = false;

        public bool ConfirmRelationship
        {
            set { blnConfirmRelationship = value; }
        }

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
            AdjustRealFreq();
            SetCrawlerFreq();
            Log("The initial requesting interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString() + " requests left this hour.");

            //����ʼUserID���
            queueUserForUserRelationRobot.Enqueue(lStartUserID);
            if(GlobalPool.UserInfoRobotEnabled)
                queueUserForUserInfoRobot.Enqueue(lStartUserID);
            if(GlobalPool.TagRobotEnabled)
                queueUserForUserTagRobot.Enqueue(lStartUserID);
            if(GlobalPool.StatusRobotEnabled)
                queueUserForStatusRobot.Enqueue(lStartUserID);
            lCurrentID = lStartUserID;

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
                lCurrentID = queueUserForUserRelationRobot.RollQueue();

                //��־
                Log("Recording current UserID��" + lCurrentID.ToString() + "...");
                SysArg.SetCurrentID(lCurrentID, SysArgFor.USER_RELATION);

                #region �û���ע�б�
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }
                //��־                
                Log("Crawling the followings of User " + lCurrentID.ToString() + "...");
                //��ȡ��ǰ�û��Ĺ�ע���û�ID����¼��ϵ���������
                LinkedList<long> lstBuffer = crawler.GetFriendsOf(lCurrentID, -1);
                //��־
                Log(lstBuffer.Count.ToString() + " followings crawled.");
                //��־
                AdjustFreq();
                SetCrawlerFreq();
                Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms." + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString() + " requests left this hour.");

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(GlobalPool.SleepMsForThread);
                    }
                    lQueueBufferFirst = lstBuffer.First.Value;
                    bool blnRecordRelation = true;
                    if (blnConfirmRelationship)
                    {
                        //��־                
                        Log("Confirming the relationship between User " + lCurrentID.ToString() + " and User " + lQueueBufferFirst.ToString());
                        blnRecordRelation = crawler.RelationExistBetween(lCurrentID, lQueueBufferFirst);
                        //��־
                        AdjustFreq();
                        SetCrawlerFreq();
                        Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms." + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString() + " requests left this hour.");
                        if (blnRecordRelation)
                        {
                            //��־
                            Log("Relationship confirmed. Recording User " + lCurrentID.ToString() + " follows User " + lQueueBufferFirst.ToString() + "...");
                        }
                        else
                        {
                            //��־
                            Log("Relationship not exists. Recording invalid relationship...");
                            InvalidRelation ir = new InvalidRelation();
                            ir.source_user_id = lCurrentID;
                            ir.target_user_id = lQueueBufferFirst;
                            ir.Add();

                            Log("Recording invalid User " + lQueueBufferFirst.ToString() + "...");
                            InvalidUser iu = new InvalidUser();
                            iu.user_id = lQueueBufferFirst;
                            iu.Add();

                            //�����û�ID�Ӹ���������ȥ��
                            Log("Removing invalid User " + lQueueBufferFirst.ToString() + " from all queues...");
                            queueUserForUserRelationRobot.Remove(lQueueBufferFirst);
                            if(GlobalPool.UserInfoRobotEnabled)
                                queueUserForUserInfoRobot.Remove(lQueueBufferFirst);
                            if(GlobalPool.TagRobotEnabled)
                                queueUserForUserTagRobot.Remove(lQueueBufferFirst);
                            if(GlobalPool.StatusRobotEnabled)
                                queueUserForStatusRobot.Remove(lQueueBufferFirst);
                        }
                    }
                    else
                    {
                        //��־
                        Log("Recording User " + lCurrentID.ToString() + " follows User " + lQueueBufferFirst.ToString() + "...");
                    }
                    if (blnRecordRelation)
                    {
                        if (UserRelation.RelationshipExist(lCurrentID, lQueueBufferFirst))
                        {
                            //��־
                            Log("Relationship exists.");
                        }
                        else
                        {
                            UserRelation ur = new UserRelation();
                            ur.source_user_id = lCurrentID;
                            ur.target_user_id = lQueueBufferFirst;
                            ur.Add();
                        }

                        //�������
                        if (queueUserForUserRelationRobot.Enqueue(lQueueBufferFirst))
                            //��־
                            Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of User Relation Robot...");
                        if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue(lQueueBufferFirst))
                            //��־
                            Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of User Information Robot...");
                        if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue(lQueueBufferFirst))
                            //��־
                            Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of User Tag Robot...");
                        if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue(lQueueBufferFirst))
                            //��־
                            Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of Status Robot...");
                    }
                    lstBuffer.RemoveFirst();
                }
                #endregion
                #region �û���˿�б�
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }
                //��־                
                Log("Crawling the followers of User " + lCurrentID.ToString() + "...");
                //��ȡ��ǰ�û��ķ�˿���û�ID����¼��ϵ���������
                lstBuffer = crawler.GetFollowersOf(lCurrentID, -1);
                //��־
                Log(lstBuffer.Count.ToString() + " followers crawled.");
                //��־
                AdjustFreq();
                SetCrawlerFreq();
                Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms." + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString() + " requests left this hour.");

                while (lstBuffer.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(GlobalPool.SleepMsForThread);
                    }
                    lQueueBufferFirst = lstBuffer.First.Value;
                    bool blnRecordRelation = true;
                    if (blnConfirmRelationship)
                    {
                        //��־                
                        Log("Confirming the relationship between User " + lQueueBufferFirst.ToString() + " and User " + lCurrentID.ToString());
                        blnRecordRelation = crawler.RelationExistBetween(lQueueBufferFirst, lCurrentID);
                        //��־
                        AdjustFreq();
                        SetCrawlerFreq();
                        Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms." + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString() + " requests left this hour.");
                        if (blnRecordRelation)
                        {
                            //��־
                            Log("Relationship confirmed. Recording User " + lQueueBufferFirst.ToString() + " follows User " + lCurrentID.ToString() + "...");
                        }
                        else
                        {
                            //��־
                            Log("Relationship not exists. Recording invalid relationship...");
                            InvalidRelation ir = new InvalidRelation();
                            ir.source_user_id = lQueueBufferFirst;
                            ir.target_user_id = lCurrentID;
                            ir.Add();

                            Log("Recording invalid User " + lQueueBufferFirst.ToString() + "...");
                            InvalidUser iu = new InvalidUser();
                            iu.user_id = lQueueBufferFirst;
                            iu.Add();

                            //�����û�ID�Ӹ���������ȥ��
                            Log("Removing invalid User " + lQueueBufferFirst.ToString() + " from all queues...");
                            queueUserForUserRelationRobot.Remove(lQueueBufferFirst);
                            if (GlobalPool.UserInfoRobotEnabled)
                                queueUserForUserInfoRobot.Remove(lQueueBufferFirst);
                            if (GlobalPool.TagRobotEnabled)
                                queueUserForUserTagRobot.Remove(lQueueBufferFirst);
                            if (GlobalPool.StatusRobotEnabled)
                                queueUserForStatusRobot.Remove(lQueueBufferFirst);
                        }
                    }
                    else
                    {
                        //��־
                        Log("Recording User " + lQueueBufferFirst.ToString() + " follows User " + lCurrentID.ToString() + "...");
                    }
                    if (blnRecordRelation)
                    {
                        if (UserRelation.RelationshipExist(lQueueBufferFirst, lCurrentID))
                        {
                            //��־
                            Log("Relationship exists.");
                        }
                        else
                        {
                            UserRelation ur = new UserRelation();
                            ur.source_user_id = lQueueBufferFirst;
                            ur.target_user_id = lCurrentID;
                            ur.Add();
                        }

                        //�������
                        if (queueUserForUserRelationRobot.Enqueue(lQueueBufferFirst))
                            //��־
                            Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of User Relation Robot...");
                        if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue(lQueueBufferFirst))
                            //��־
                            Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of User Information Robot...");
                        if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue(lQueueBufferFirst))
                            //��־
                            Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of User Tag Robot...");
                        if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue(lQueueBufferFirst))
                            //��־
                            Log("Adding User " + lQueueBufferFirst.ToString() + " to the user queue of Status Robot...");
                    }
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //��־
                Log("Social grapgh of User " + lCurrentID.ToString() + " crawled.");
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
    }
}
