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
            : base()
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
            while (queueUserForUserInfoRobot.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(50);   //������Ϊ�գ���ȴ�
            }
            Thread.Sleep(500);  //waiting that user relation robot update request limit data

            SetCrawlerFreq();
            Log("��ʼ������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + GlobalPool.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + GlobalPool.RemainingHits.ToString() + "��");

            //�Զ�������ѭ�����У�ֱ���в�����ͣ��ֹͣ
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }

                //����ͷȡ��
                lCurrentID = queueUserForUserTagRobot.RollQueue();
                
                //��־
                Log( "��¼��ǰ�û�ID��" + lCurrentID.ToString() );
                SysArg.SetCurrentUserIDForUserTag( lCurrentID );

                #region �û���ǩ��Ϣ
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }

                //��־
                Log( "��ȡ�û�" + lCurrentID.ToString() + "�ı�ǩ..." );
                LinkedList<Tag> lstTag = crawler.GetTagsOf( lCurrentID );
                //��־
                Log( "����" + lstTag.Count.ToString() + "����ǩ��" );

                while (lstTag.Count > 0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }
                    Tag tag = lstTag.First.Value;
                    if (!Tag.Exists( tag.tag_id ))
                    {
                        //��־
                        Log( "����ǩ" + tag.tag_id.ToString() + "�������ݿ�..." );
                        tag.Add();
                    }
                    else
                        //��־
                        Log( "��ǩ" + tag.tag_id.ToString() + "�Ѵ��ڡ�" );

                    if (!UserTag.Exists( lCurrentID, tag.tag_id ))
                    {
                        //��־
                        Log( "��¼�û�" + lCurrentID.ToString() + "ӵ�б�ǩ" + tag.tag_id.ToString() + "..." );
                        UserTag user_tag = new UserTag();
                        user_tag.user_id = lCurrentID;
                        user_tag.tag_id = tag.tag_id;
                        user_tag.Add();
                    }
                    else
                        //��־
                        Log( "�û�" + lCurrentID.ToString() + "ӵ�б�ǩ" + tag.tag_id.ToString() + "�Ѵ��ڡ�" );

                    lstTag.RemoveFirst();
                }
                #endregion
                //��־
                Log( "�û�" + lCurrentID.ToString() + "�ı�ǩ��������ȡ��ϡ�" );
                //��־
                AdjustFreq();
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + GlobalPool.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + GlobalPool.RemainingHits.ToString() + "��");
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
