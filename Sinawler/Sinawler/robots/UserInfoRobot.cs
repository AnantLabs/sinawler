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
    class UserInfoRobot:RobotBase
    {
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
            AdjustFreq();
            SetCrawlerFreq();
            Log("The initial requesting interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s, " + api.RemainingIPHits.ToString() + " IP hits and " + api.RemainingUserHits.ToString() + " user hits left this hour.");
            User user;
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
                //lCurrentID = queueUserForUserInfoRobot.RollQueue();
                lCurrentID = queueUserForUserInfoRobot.FirstValue;
                
                //��־
                Log("Recording current UserID: " + lCurrentID.ToString()+"...");
                SysArg.SetCurrentID(lCurrentID, SysArgFor.USER_INFO);

                #region �û�������Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(GlobalPool.SleepMsForThread);
                }

                Log("Crawling information of User " + lCurrentID.ToString() + "...");
                user = crawler.GetUserInfo(lCurrentID);
                //��־
                AdjustFreq();
                SetCrawlerFreq();
                Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s, " + api.RemainingIPHits.ToString() + " IP hits and " + api.RemainingUserHits.ToString() + " user hits left this hour.");
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
                    if(InvalidUser.ExistInDB(lCurrentID))
                    {
                        //��־
                        Log("Removing User " + lCurrentID.ToString() + " from invalid users...");
                        InvalidUser.RemoveFromDB(lCurrentID);
                    }
                    //��־
                    Log( "The information of User " + lCurrentID.ToString() + " crawled." );
                    queueUserForUserInfoRobot.RollQueue();
                }
                else if(user==null) //�û�������
                {
                    Log("Recording invalid User " + lCurrentID.ToString() + "...");
                    InvalidUser iu = new InvalidUser();
                    iu.user_id = lCurrentID;
                    iu.Add();

                    //�����û�ID�Ӹ���������ȥ��
                    Log("Removing invalid User " + lCurrentID.ToString() + " from all queues...");
                    queueUserForUserRelationRobot.Remove(lCurrentID);
                    queueUserForUserInfoRobot.Remove(lCurrentID);
                    if (GlobalPool.TagRobotEnabled)
                        queueUserForUserTagRobot.Remove(lCurrentID);
                    if (GlobalPool.StatusRobotEnabled)
                        queueUserForStatusRobot.Remove(lCurrentID);
                }
                else if (user.user_id == -1)   //forbidden
                {
                    int iSleepSeconds = GlobalPool.GetAPI(SysArgFor.USER_INFO).ResetTimeInSeconds;
                    Log("Service is forbidden now. I will wait for " + iSleepSeconds .ToString()+ "s to continue...");
                    for(int i=0;i<iSleepSeconds;i++)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(1000);
                    }
                }
                #endregion
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
