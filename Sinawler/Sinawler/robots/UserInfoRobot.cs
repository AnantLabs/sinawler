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
        private int iInitQueueLength = 100;          //��ʼ���г���

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        //���캯������Ҫ������Ӧ������΢��API��������
        public UserInfoRobot ()
            : base(SysArgFor.USER_INFO)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_userInfo.log";
            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
        }

        /// <summary>
        /// ��ָ����UserIDΪ��㿪ʼ����
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ()
        {
            //��ȡ�ϴ���ֹ�����û�ID�����
            long lLastUID = SysArg.GetCurrentID(SysArgFor.USER_INFO);
            if (lLastUID > 0) queueUserForUserInfoRobot.Enqueue(lLastUID);
            while (queueUserForUserInfoRobot.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(GlobalPool.SleepMsForThread);   //������Ϊ�գ���ȴ�
            }
            Thread.Sleep(500);  //waiting that user relation robot update request limit data
            User user;

            SetCrawlerFreq();
            Log("The initial requesting interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString()+" requests left this hour.");

            //�Զ���ѭ������
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }
                //����ͷȡ��
                lCurrentID = queueUserForUserInfoRobot.RollQueue();
                
                //��־
                Log("Recording current UserID: " + lCurrentID.ToString()+"...");
                SysArg.SetCurrentID( lCurrentID,SysArgFor.USER_INFO );

                #region �û�������Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }

                Log("Crawling information of User " + lCurrentID.ToString() + "...");
                user = crawler.GetUserInfo(lCurrentID);
                if (user!=null && user.user_id > 0)
                {
                    //�����ݿ��в����ڵ�ǰ�û��Ļ�����Ϣ������ȡ���������ݿ�
                    if (!User.ExistInDB( lCurrentID ))
                    {
                        //��־
                        Log("Saving User " + lCurrentID.ToString() + " into database...");
                        user.Add();
                    }
                    else
                    {
                        //��־
                        Log("Updating the information of User " + lCurrentID.ToString() + "...");
                        user.Update();
                    }
                    //��־
                    Log( "The information of User " + lCurrentID.ToString() + " crawled." );
                }
                else if(user==null) //�û�������
                {
                    //��־
                    Log( "User " + lCurrentID.ToString() + " not exists. Removing from all queues..." );
                    //�����û�ID�Ӹ���������ȥ��
                    queueUserForUserInfoRobot.Remove( lCurrentID );
                    queueUserForUserRelationRobot.Remove( lCurrentID );
                    queueUserForUserTagRobot.Remove( lCurrentID );
                    queueUserForStatusRobot.Remove( lCurrentID );

                    Log("User " + lCurrentID.ToString() + " not exists. Deleting related data...");
                    //Remove the data related from every table, except statuses and comments
                    User.Remove(lCurrentID);
                }
                #endregion

                AdjustFreq();
                //��־
                Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms." + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString()+" requests left this hour.");
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

        sealed protected override void AdjustFreq()
        {
            base.AdjustFreq();
            SetCrawlerFreq();
        }
    }
}
