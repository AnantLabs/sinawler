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
        private UserQueue queueUserForUserRobot;            //�û�������ʹ�õ��û���������
        private UserQueue queueUserForStatusRobot;          //΢��������ʹ�õ��û���������
        private int iInitQueueLength = 100;          //��ʼ���г���
        private long lQueueBufferFirst = 0;   //���ڼ�¼��ȡ�Ĺ�ע�û��б���˿�û��б�Ķ�ͷֵ

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        //���캯������Ҫ������Ӧ������΢��API��������
        public UserRobot ( SinaApiService oAPI, UserQueue qUserForUserRobot, UserQueue qUserForStatusRobot ):base(oAPI)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_user.log";
            queueUserForUserRobot = qUserForUserRobot;
            queueUserForStatusRobot = qUserForStatusRobot;
        }

        /// <summary>
        /// ��ָ����UserIDΪ��㿪ʼ����
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ( long lStartUserID )
        {
            if (lStartUserID == 0) return;

            User user;

            //����ʼUserID���
            queueUserForUserRobot.Enqueue( lStartUserID );
            queueUserForStatusRobot.Enqueue( lStartUserID );

            #region Ԥ���ض���
            //����ѡ�ѡ������û����еķ���
            DataTable dtUserID=new DataTable();

            switch (queueUserForUserRobot.PreLoadQueue)
            {
                case EnumPreLoadQueue.PRELOAD_USER_ID:
                    //��־
                    Log("��ȡ����ȡ���ݵ��û���ID���������ڴ����...");
                    dtUserID = User.GetCrawedUserIDTable();
                    break;
                case EnumPreLoadQueue.PRELOAD_ALL_USER_ID:
                    //��־
                    Log("��ȡ���ݿ��������û���ID���������ڴ����...");
                    dtUserID = UserRelation.GetAllUserIDTable();
                    break;
            }

            if (dtUserID != null)
            {
                iInitQueueLength = dtUserID.Rows.Count;
                long lUserID;
                int i;
                for (i = 0; i < dtUserID.Rows.Count; i++)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    lUserID = Convert.ToInt64( dtUserID.Rows[i]["user_id"] );
                    if (queueUserForUserRobot.Enqueue( lUserID ))
                        //��־
                        Log( "���û�" + lUserID.ToString() + "�����û������˵��û����С����ȣ�" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%" );
                    if (queueUserForStatusRobot.Enqueue( lUserID ))
                        //��־
                        Log( "���û�" + lUserID.ToString() + "����΢�������˵��û����С����ȣ�" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%" );
                }
                dtUserID.Dispose();
                //��־
                Log( "Ԥ�����û�������ɡ�" );
            }
            #endregion
            
            lCurrentID = lStartUserID;
            //�Զ���ѭ������
            while (queueUserForUserRobot.Count > 0)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //����ͷȡ��
                lCurrentID = queueUserForUserRobot.RollQueue();
                
                #region Ԥ����
                if (lCurrentID == lStartUserID)  //˵������һ��ѭ������
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }

                    //��־
                    Log("��ʼ����֮ǰ���ӵ�������...");

                    User.NewIterate();
                    UserRelation.NewIterate();
                }
                //��־
                Log("��¼��ǰ�û�ID��" + lCurrentID.ToString());
                SysArg.SetCurrentUserID( lCurrentID );
                #endregion
                #region �û�������Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                Log( "��ȡ�û�" + lCurrentID.ToString() +"�Ļ�����Ϣ...");
                user = crawler.GetUserInfo( lCurrentID );
                if (user.user_id > 0)
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
                        ur.iteration = 0;
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    //�������
                    if (!queueUserForUserRobot.Enqueue( lQueueBufferFirst ))
                    {
                        //��־
                        Log( "�û�" + lQueueBufferFirst.ToString() + "�����û������˵��û�������..." );
                    }
                    else
                    {
                        //��־
                        Log( "���û�" + lQueueBufferFirst.ToString() + "�����û������˵��û����С�" );
                    }
                    if (!queueUserForStatusRobot.Enqueue( lQueueBufferFirst ))
                    {
                        //��־
                        Log( "�û�" + lQueueBufferFirst.ToString() + "����΢�������˵��û�������..." );
                    }
                    else
                    {
                        //��־
                        Log( "���û�" + lQueueBufferFirst.ToString() + "����΢�������˵��û����С�" );
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
                        ur.iteration = 0;
                        ur.Add();
                    }
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    //�������
                    if (!queueUserForUserRobot.Enqueue( lQueueBufferFirst ))
                    {
                        //��־
                        Log( "�û�" + lQueueBufferFirst.ToString() + "�����û������˵��û������С�" );
                    }
                    else
                    {
                        //��־
                        Log( "���û�" + lQueueBufferFirst.ToString() + "�����û������˵��û����С�" );
                    }
                    if (!queueUserForStatusRobot.Enqueue( lQueueBufferFirst ))
                    {
                        //��־
                        Log( "�û�" + lQueueBufferFirst.ToString() + "����΢�������˵��û������С�" );
                    }
                    else
                    {
                        //��־
                        Log( "���û�" + lQueueBufferFirst.ToString() + "����΢�������˵��û����С�" );
                    }
                    lstBuffer.RemoveFirst();
                }
                #endregion
                //����ٽ��ո��������UserID�����β
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
            crawler.StopCrawling = false;
            queueUserForUserRobot.Initialize();
            queueUserForStatusRobot.Initialize();
        }
    }
}
