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
    class CommentRobot : RobotBase
    {
        private UserQueue queueUserForUserInfoRobot;        //�û���Ϣ������ʹ�õ��û���������
        private UserQueue queueUserForUserRelationRobot;    //�û���ϵ������ʹ�õ��û���������
        private UserQueue queueUserForUserTagRobot;         //�û���ǩ������ʹ�õ��û���������
        private UserQueue queueUserForStatusRobot;          //΢��������ʹ�õ��û���������
        private StatusQueue queueStatus;                    //΢����������
        private UserBuffer oUserBuffer;                     //the buffer queue of users

        //���캯������Ҫ������Ӧ������΢��API
        public CommentRobot ()
            : base(SysArgFor.COMMENT)
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_comment.log";

            queueUserForUserInfoRobot = GlobalPool.UserQueueForUserInfoRobot;
            queueUserForUserRelationRobot = GlobalPool.UserQueueForUserRelationRobot;
            queueUserForUserTagRobot = GlobalPool.UserQueueForUserTagRobot;
            queueUserForStatusRobot = GlobalPool.UserQueueForStatusRobot;
            queueStatus = GlobalPool.StatusQueue;
            oUserBuffer = GlobalPool.UserBuffer;
        }

        /// <summary>
        /// ��ʼ����ȡ΢������
        /// </summary>
        public void Start()
        {
            //��ȡ�ϴ���ֹ����΢��ID�����
            long lLastStatusID = SysArg.GetCurrentID(SysArgFor.COMMENT);
            if (lLastStatusID > 0) queueStatus.Enqueue( lLastStatusID );
            while (queueStatus.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(50);   //������Ϊ�գ���ȴ�
            }
            Thread.Sleep(500);  //waiting that user relation robot update limit data

            SetCrawlerFreq();
            Log("��ʼ������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + api.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + api.RemainingHits.ToString() + "��");

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
                lCurrentID = queueStatus.RollQueue();

                //��־
                Log("��¼��ǰ΢��ID��" + lCurrentID.ToString());
                SysArg.SetCurrentID(lCurrentID,SysArgFor.COMMENT);

                #region ΢����Ӧ����
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }

                //��־
                Log("��ȡ΢��" + lCurrentID.ToString() + "������...");
                int iPage = 1;
                //��ȡ��ǰ΢��������
                LinkedList<Comment> lstComment = new LinkedList<Comment>();
                LinkedList<Comment> lstTemp=new LinkedList<Comment>();
                lstTemp = crawler.GetCommentsOf(lCurrentID, iPage);
                AdjustFreq();
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + api.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + api.RemainingHits.ToString() + "��");
                while (lstTemp.Count > 0)
                {
                    while (lstTemp.Count > 0)
                    {
                        lstComment.AddLast(lstTemp.First.Value);
                        lstTemp.RemoveFirst();
                    }
                    iPage++;
                    lstTemp = crawler.GetCommentsOf(lCurrentID, iPage);
                    AdjustFreq();
                    Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + api.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + api.RemainingHits.ToString() + "��");
                }
                //��־
                Log("����΢��" + lCurrentID.ToString() + "��" + lstComment.Count.ToString() + "�����ۡ�");
                Comment comment;
                while(lstComment.Count>0)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    comment = lstComment.First.Value;
                    if (!Comment.Exists( comment.comment_id ))
                    {
                        //��־
                        Log( "������" + comment.comment_id.ToString() + "�������ݿ�..." );
                        comment.Add();
                    }

                    if (queueUserForUserRelationRobot.Enqueue(comment.user.user_id))
                        Log("��������" + comment.user.user_id.ToString() + "�����û���ϵ�����˵��û����С�");
                    if (GlobalPool.UserInfoRobotEnabled && queueUserForUserInfoRobot.Enqueue(comment.user.user_id))
                        Log("��������" + comment.user.user_id.ToString() + "�����û���Ϣ�����˵��û����С�");
                    if (GlobalPool.TagRobotEnabled && queueUserForUserTagRobot.Enqueue(comment.user.user_id))
                        Log("��������" + comment.user.user_id.ToString() + "�����û���ǩ�����˵��û����С�");
                    if (GlobalPool.StatusRobotEnabled && queueUserForStatusRobot.Enqueue(comment.user.user_id))
                        Log("��������" + comment.user.user_id.ToString() + "����΢�������˵��û����С�");
                    //add the user into the buffer only when the user does not exist in the queue for userInfo
                    if (GlobalPool.UserInfoRobotEnabled && !queueUserForUserInfoRobot.QueueExists(comment.user.user_id) && oUserBuffer.Enqueue(comment.user))
                        Log("��������" + comment.user.user_id.ToString() + "�����û�����ء�");

                    lstComment.RemoveFirst();
                }
                #endregion
                //����ٽ��ո��������StatusID�����β
                //��־
                Log("΢��" + lCurrentID.ToString() + "����������ȡ��ϡ�");
            }
        }

        public override void Initialize()
        {
            //��ʼ����Ӧ����
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueStatus.Initialize();
        }

        sealed protected override void AdjustFreq()
        {
            base.AdjustFreq();
            SetCrawlerFreq();
        }
    }
}
