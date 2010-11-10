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
    class UserInfoRobot:RobotBase
    {
        private UserQueue queueUserForUserInfoRobot;            //�û���Ϣ������ʹ�õ��û���������
        private UserQueue queueUserForUserRelationRobot;        //�û���ϵ������ʹ�õ��û���������
        private UserQueue queueUserForStatusRobot;          //΢��������ʹ�õ��û���������
        private int iInitQueueLength = 100;          //��ʼ���г���

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        //���캯������Ҫ������Ӧ������΢��API��������
        public UserInfoRobot ( SinaApiService oAPI, UserQueue qUserForUserInfoRobot, UserQueue qUserForUserRelationRobot, UserQueue qUserForStatusRobot )
            : base( oAPI )
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userInfo.log";
            queueUserForUserInfoRobot = qUserForUserInfoRobot;
            queueUserForUserRelationRobot = qUserForUserRelationRobot;
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
            queueUserForUserInfoRobot.Enqueue( lStartUserID );
            queueUserForUserRelationRobot.Enqueue( lStartUserID );
            queueUserForStatusRobot.Enqueue( lStartUserID );

            #region Ԥ���ض���
            //����ѡ�ѡ������û����еķ���
            DataTable dtUserID=new DataTable();

            switch (queueUserForUserInfoRobot.PreLoadQueue)
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
                    if (queueUserForUserInfoRobot.Enqueue( lUserID ))
                        //��־
                        Log( "���û�" + lUserID.ToString() + "�����û���Ϣ�����˵��û����С����ȣ�" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%" );
                    if (queueUserForUserRelationRobot.Enqueue( lUserID ))
                        //��־
                        Log( "���û�" + lUserID.ToString() + "�����û���ϵ�����˵��û����С����ȣ�" + ((int)((float)((i + 1) * 100) / (float)iInitQueueLength)).ToString() + "%" );
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
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //����ͷȡ��
                lCurrentID = queueUserForUserInfoRobot.RollQueue();
                
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
                }
                //��־
                Log("��¼��ǰ�û�ID��" + lCurrentID.ToString());
                SysArg.SetCurrentUserIDForUserInfo( lCurrentID );
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
                #endregion
                //����ٽ��ո��������UserID�����β
                //��־
                Log("�û�" + lCurrentID.ToString() + "�Ļ�����Ϣ����ȡ��ϡ�");
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
            queueUserForUserInfoRobot.Initialize();
        }
    }
}
