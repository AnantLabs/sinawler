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
    class UserRelationRobot:RobotBase
    {
        private UserQueue queueUserForUserInfoRobot;        //�û���Ϣ������ʹ�õ��û���������
        private UserQueue queueUserForUserRelationRobot;    //�û���ϵ������ʹ�õ��û���������
        private UserQueue queueUserForUserTagRobot;         //�û���ǩ������ʹ�õ��û���������
        private UserQueue queueUserForStatusRobot;          //΢��������ʹ�õ��û���������
        private long lQueueBufferFirst = 0;   //���ڼ�¼��ȡ�Ĺ�ע�û��б���˿�û��б�Ķ�ͷֵ

        //���캯������Ҫ������Ӧ������΢��API��������
        public UserRelationRobot ()
            : base()
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
        public void Start ( long lStartUserID )
        {
            if (lStartUserID == 0) return;
            //����ʼUserID���
            queueUserForUserRelationRobot.Enqueue( lStartUserID );
            queueUserForUserInfoRobot.Enqueue( lStartUserID );
            queueUserForUserTagRobot.Enqueue( lStartUserID );
            queueUserForStatusRobot.Enqueue( lStartUserID );
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
                Log("��¼��ǰ�û�ID��" + lCurrentID.ToString());
                SysArg.SetCurrentUserIDForUserRelation( lCurrentID );

                #region �û���ע�б�
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
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
                        Thread.Sleep(50);
                    }
                    lQueueBufferFirst = lstBuffer.First.Value;
                    //����������Ч��ϵ������
                    if (!UserRelation.Exists( lCurrentID, lQueueBufferFirst ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(50);
                        }
                        //��־
                        Log( "��¼�û�" + lCurrentID.ToString() + "��ע�û�" + lQueueBufferFirst.ToString() + "..." );
                        UserRelation ur = new UserRelation();
                        ur.source_user_id = lCurrentID;
                        ur.target_user_id = lstBuffer.First.Value;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
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
                    if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue( lQueueBufferFirst ))
                        //��־
                        Log( "���û�" + lQueueBufferFirst.ToString() + "�����û���Ϣ�����˵��û����С�" );                    
                    if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue( lQueueBufferFirst ))
                        //��־
                        Log( "���û�" + lQueueBufferFirst.ToString() + "�����û���ǩ�����˵��û����С�" );
                    if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue( lQueueBufferFirst ))
                        //��־
                        Log( "���û�" + lQueueBufferFirst.ToString() + "����΢�������˵��û����С�" );
                    lstBuffer.RemoveFirst();
                }

                //��־
                AdjustFreq();
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + crawler.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + crawler.RemainingHits.ToString() + "��");
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
                lstBuffer = crawler.GetFollowersOf( lCurrentID, -1 );
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
                    if (!UserRelation.Exists( lQueueBufferFirst, lCurrentID ))
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(50);
                        }
                        //��־
                        Log( "��¼�û�" + lQueueBufferFirst.ToString() + "��ע�û�" + lCurrentID.ToString() + "..." );
                        UserRelation ur = new UserRelation();
                        ur.source_user_id = lstBuffer.First.Value;
                        ur.target_user_id = lCurrentID;
                        ur.relation_state = Convert.ToInt32( RelationState.RelationExists );
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
                    if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue( lQueueBufferFirst ))
                        //��־
                        Log( "���û�" + lQueueBufferFirst.ToString() + "�����û���Ϣ�����˵��û����С�" );
                    if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue( lQueueBufferFirst ))
                        //��־
                        Log( "���û�" + lQueueBufferFirst.ToString() + "�����û���ǩ�����˵��û����С�" );
                    if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue( lQueueBufferFirst ))
                        //��־
                        Log( "���û�" + lQueueBufferFirst.ToString() + "����΢�������˵��û����С�" );
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //����ٽ��ո��������UserID�����β
                //��־
                Log("�û�" + lCurrentID.ToString() + "�Ĺ�ϵ����ȡ��ϡ�");
                //��־
                AdjustFreq();
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + crawler.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + crawler.RemainingHits.ToString() + "��");
            }
        }

        public override void Initialize ()
        {
            //��ʼ����Ӧ����
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueUserForUserRelationRobot.Initialize();
        }
    }
}
