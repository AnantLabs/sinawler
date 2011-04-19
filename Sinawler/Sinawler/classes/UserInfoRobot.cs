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
        private UserQueue queueUserForUserTagRobot;             //�û���ǩ������ʹ�õ��û���������
        private UserQueue queueUserForStatusRobot;          //΢��������ʹ�õ��û���������
        private UserBuffer oUserBuffer;               //the buffer queue of users
        private int iInitQueueLength = 100;          //��ʼ���г���

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        //���캯������Ҫ������Ӧ������΢��API��������
        public UserInfoRobot ()
            : base()
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userInfo.log";
            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
            oUserBuffer = GlobalPool.UserBuffer;
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
            queueUserForUserTagRobot.Enqueue( lStartUserID );
            queueUserForStatusRobot.Enqueue( lStartUserID );
            
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
                
                //��־
                Log("��¼��ǰ�û�ID��" + lCurrentID.ToString());
                SysArg.SetCurrentUserIDForUserInfo( lCurrentID );

                #region �û�������Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }

                if (oUserBuffer.UserExists(lCurrentID))   //current user exists in the user buffer
                {
                    Log("�û�" + lCurrentID.ToString() + "���ڻ�����У���ֱ�ӻ�ȡ����Ϣ...");
                    user = oUserBuffer.GetUser(lCurrentID);
                    oUserBuffer.Remove(user);
                }
                else
                {
                    Log("��ȡ�û�" + lCurrentID.ToString() + "�Ļ�����Ϣ...");
                    user = crawler.GetUserInfo(lCurrentID);
                }
                if (user!=null && user.user_id > 0)
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
                    //��־
                    Log( "�û�" + lCurrentID.ToString() + "�Ļ�����Ϣ����ȡ��ϡ�" );
                }
                else if(user==null) //�û�������
                {
                    //��־
                    Log( "�û�" + lCurrentID.ToString() + "�����ڣ�����Ӷ������Ƴ�..." );
                    //�����û�ID�Ӹ���������ȥ��
                    queueUserForUserInfoRobot.Remove( lCurrentID );
                    queueUserForUserRelationRobot.Remove( lCurrentID );
                    queueUserForUserTagRobot.Remove( lCurrentID );
                    queueUserForStatusRobot.Remove( lCurrentID );
                }
                #endregion
                
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
            queueUserForUserInfoRobot.Initialize();
        }
    }
}
