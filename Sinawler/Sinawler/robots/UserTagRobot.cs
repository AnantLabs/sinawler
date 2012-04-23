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
    class UserTagRobot : RobotBase
    {
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

            AdjustRealFreq();
            SetCrawlerFreq();
            Log("The initial requesting interval is " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s, " + api.RemainingIPHits.ToString() + " IP hits and " + api.RemainingUserHits.ToString() + " user hits left this hour.");

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
                //lCurrentID = queueUserForUserTagRobot.RollQueue();
                lCurrentID = queueUserForUserTagRobot.FirstValue;
                
                //��־
                Log( "Recording current UserID: " + lCurrentID.ToString()+"..." );
                SysArg.SetCurrentID(lCurrentID, SysArgFor.USER_TAG);

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
                if (lstTag.Count>0 && lstTag.First.Value.tag_id > 0)
                {
                    //��־
                    Log(lstTag.Count.ToString() + " tags crawled.");
                    //��־
                    AdjustFreq();
                    SetCrawlerFreq();
                    Log("Requesting interval is adjusted as " + crawler.SleepTime.ToString() + "ms. " + api.ResetTimeInSeconds.ToString() + "s, " + api.RemainingIPHits.ToString() + " IP hits and "+api.RemainingUserHits.ToString()+" user hits left this hour.");

                    while (lstTag.Count > 0)
                    {
                        if (blnAsyncCancelled) return;
                        while (blnSuspending)
                        {
                            if (blnAsyncCancelled) return;
                            Thread.Sleep(GlobalPool.SleepMsForThread);
                        }
                        Tag tag = lstTag.First.Value;
                        if (!Tag.Exists(tag.tag_id))
                        {
                            //��־
                            Log("Saving Tag " + tag.tag_id.ToString() + " into database...");
                            tag.Add();
                        }
                        else
                        {
                            //��־
                            //Log( "Tag " + tag.tag_id.ToString() + " exists." );
                            Log("Updating Tag " + tag.tag_id.ToString() + " into database...");
                            tag.Update();
                        }

                        if (!UserTag.Exists(lCurrentID, tag.tag_id))
                        {
                            //��־
                            Log("Recording User " + lCurrentID.ToString() + " has Tag " + tag.tag_id.ToString() + "...");
                            UserTag user_tag = new UserTag();
                            user_tag.user_id = lCurrentID;
                            user_tag.tag_id = tag.tag_id;
                            user_tag.Add();
                        }
                        else
                            //��־
                            Log("Tag " + tag.tag_id.ToString() + " of User " + lCurrentID.ToString() + " exists.");

                        lstTag.RemoveFirst();
                    }
                    queueUserForUserTagRobot.RollQueue();
                    //��־
                    Log("Tags of User " + lCurrentID.ToString() + " crawled.");
                }
                else if (lstTag.Count > 0 && lstTag.First.Value.tag_id == -1)
                {
                    lstTag.Clear();
                    int iSleepSeconds = GlobalPool.GetAPI(SysArgFor.USER_INFO).ResetTimeInSeconds;
                    Log("Service is forbidden now. I will wait for " + iSleepSeconds.ToString() + "s to continue...");
                    for (int i = 0; i < iSleepSeconds; i++)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(1000);
                    }
                    continue;
                }
                else if (lstTag.Count > 0 && lstTag.First.Value.tag_id == -2)
                {   
                    int iSleepSeconds = GlobalPool.GetAPI(SysArgFor.USER_INFO).ResetTimeInSeconds;
                    Log("Error! The error message is \""+lstTag.First.Value.tag+"\". I will wait for " + iSleepSeconds.ToString() + "s to continue...");
                    lstTag.Clear();
                    for (int i = 0; i < iSleepSeconds; i++)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(1000);
                    }
                    continue;
                }
                else
                {
                    queueUserForUserTagRobot.RollQueue();
                    //��־
                    Log("Tags of User " + lCurrentID.ToString() + " crawled.");
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
            queueUserForUserTagRobot.Initialize();
        }
    }
}
