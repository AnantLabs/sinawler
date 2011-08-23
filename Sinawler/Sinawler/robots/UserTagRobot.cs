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
    class UserTagRobot : RobotBase
    {
        private UserQueue queueUserForUserTagRobot;        //�û���ǩ������ʹ�õ��û���������
        private UserQueue queueUserForUserInfoRobot;        //�û���Ϣ������ʹ�õ��û���������
        private UserQueue queueUserForUserRelationRobot;    //�û���ϵ������ʹ�õ��û���������
        private UserQueue queueUserForStatusRobot;          //΢��������ʹ�õ��û���������

        //���캯������Ҫ������Ӧ������΢��API��������
        public UserTagRobot()
            : base(SysArgFor.USER_TAG)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_tag.log";
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
            long lLastUID = SysArg.GetCurrentID(SysArgFor.USER_TAG);
            if (lLastUID > 0) queueUserForUserTagRobot.Enqueue(lLastUID);
            while (queueUserForUserTagRobot.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(GlobalPool.SleepMsForThread);   //������Ϊ�գ���ȴ�
            }
            Thread.Sleep(500);  //waiting that user relation robot update request limit data

            SetCrawlerFreq();
            Log("The initial requesting interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString()+" requests left this hour.");

            //�Զ�������ѭ�����У�ֱ���в�����ͣ��ֹͣ
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( GlobalPool.SleepMsForThread );
                }

                //����ͷȡ��
                lCurrentID = queueUserForUserTagRobot.RollQueue();
                
                //��־
                Log( "Recording current UserID: " + lCurrentID.ToString()+"..." );
                SysArg.SetCurrentID( lCurrentID, SysArgFor.USER_TAG );

                #region �û���ǩ��Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( GlobalPool.SleepMsForThread );
                }

                //��־
                Log( "Crawling tags of User " + lCurrentID.ToString() + "..." );
                LinkedList<Tag> lstTag = crawler.GetTagsOf( lCurrentID );
                //��־
                Log( lstTag.Count.ToString() + " tags crawled." );

                while (lstTag.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( GlobalPool.SleepMsForThread );
                    }
                    Tag tag = lstTag.First.Value;
                    if (!Tag.Exists( tag.tag_id ))
                    {
                        //��־
                        Log( "Saving Tag " + tag.tag_id.ToString() + " into database..." );
                        tag.Add();
                    }
                    else
                        //��־
                        Log( "Tag " + tag.tag_id.ToString() + " exists." );

                    if (!UserTag.Exists( lCurrentID, tag.tag_id ))
                    {
                        //��־
                        Log( "Recording User " + lCurrentID.ToString() + " has Tag " + tag.tag_id.ToString() + "..." );
                        UserTag user_tag = new UserTag();
                        user_tag.user_id = lCurrentID;
                        user_tag.tag_id = tag.tag_id;
                        user_tag.Add();
                    }
                    else
                        //��־
                        Log( "Tag " + tag.tag_id.ToString() + " of User "+ lCurrentID.ToString() + " exists." );

                    lstTag.RemoveFirst();
                }
                #endregion
                //��־
                Log( "Tags of User " + lCurrentID.ToString() + " crawled." );
                //��־
                AdjustFreq();
                Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms." + api.ResetTimeInSeconds.ToString() + "s and " + api.RemainingHits.ToString()+" requests left this hour.");
            }
        }

        public override void Initialize ()
        {
            //��ʼ����Ӧ����
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueUserForUserTagRobot.Initialize();
        }

        sealed protected override void AdjustFreq()
        {
            base.AdjustFreq();
            SetCrawlerFreq();
        }
    }
}
