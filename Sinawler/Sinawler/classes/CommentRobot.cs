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
        private UserQueue queueUserForUserInfoRobot;          //�û���Ϣ������ʹ�õ��û���������
        private UserQueue queueUserForTagRobot;               //��ǩ������ʹ�õ��û���������
        private UserQueue queueUserForUserRelationRobot;      //�û���ϵ������ʹ�õ��û���������
        private UserQueue queueUserForStatusRobot;            //΢��������ʹ�õ��û���������
        private StatusQueue queueStatus;        //΢����������

        //���캯������Ҫ������Ӧ������΢��API
        public CommentRobot ( SinaApiService oAPI, UserQueue qUserForUserInfoRobot, UserQueue qUserForTagRobot, UserQueue qUserForUserRelationRobot, UserQueue qUserForStatusRobot, StatusQueue qStatus )
            : base( oAPI,false )
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_comment.log";

            queueUserForUserInfoRobot = qUserForUserInfoRobot;
            queueUserForTagRobot = qUserForTagRobot;
            queueUserForUserRelationRobot = qUserForUserRelationRobot;
            queueUserForStatusRobot = qUserForStatusRobot;
            queueStatus = qStatus;
        }

        /// <summary>
        /// ��ʼ����ȡ΢������
        /// </summary>
        public void Start()
        {
            //��ȡ�ϴ���ֹ����΢��ID�����
            long lLastStatusID = SysArg.GetCurrentStatusID();
            if (lLastStatusID > 0) queueStatus.Enqueue( lLastStatusID );
            while (queueStatus.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(50);   //������Ϊ�գ���ȴ�
            }
            long lStartSID = queueStatus.FirstValue;
            long lCurrentSID = 0;
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
                lCurrentSID = queueStatus.RollQueue();

                #region Ԥ����
                if (lCurrentSID == lStartSID)  //˵������һ��ѭ������
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }

                    //��־
                    Log("��ʼ����֮ǰ���ӵ�������...");
                    Comment.NewIterate();
                }
                //��־
                Log( "��¼��ǰ΢��ID��" + lCurrentSID.ToString() );
                SysArg.SetCurrentStatusID( lCurrentSID );
                #endregion
                #region ΢����Ӧ����
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }

                //��־
                Log("��ȡ΢��" + lCurrentSID.ToString() + "������...");
                //��ȡ��ǰ΢��������
                LinkedList<Comment> lstComment = crawler.GetCommentsOf(lCurrentSID);
                //��־
                Log("����΢��"+lCurrentSID.ToString()+"��" + lstComment.Count.ToString() + "�����ۡ�");
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

                    if (queueUserForUserInfoRobot.Enqueue( comment.user_id ))
                        Log( "��������" + comment.user_id.ToString() + "�����û���Ϣ�����˵��û����С�" );
                    if (queueUserForTagRobot.Enqueue( comment.user_id ))
                        Log( "��������" + comment.user_id.ToString() + "�����ǩ�����˵��û����С�" );
                    if (queueUserForUserRelationRobot.Enqueue( comment.user_id ))
                        Log( "��������" + comment.user_id.ToString() + "�����û���ϵ�����˵��û����С�" );
                    if (queueUserForStatusRobot.Enqueue( comment.user_id ))
                        Log( "��������" + comment.user_id.ToString() + "����΢�������˵��û����С�" );

                    lstComment.RemoveFirst();
                }
                #endregion
                //����ٽ��ո��������StatusID�����β
                //��־
                Log("΢��" + lCurrentSID.ToString() + "����������ȡ��ϡ�");
                //��־
                Log("����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + crawler.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + crawler.RemainingHits.ToString() + "��");
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
    }
}
