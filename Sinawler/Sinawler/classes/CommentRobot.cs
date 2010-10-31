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
        private long lCommentUID = 0;    //�����˵�UID

        public long CurrentUID
        { get { return lCommentUID; } }

        //���캯������Ҫ������Ӧ������΢��API
        public CommentRobot(SinaApiService oAPI) : base(oAPI)
        {
            queueBuffer = new QueueBuffer(QueueBufferTarget.FOR_COMMENT);
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_comment.log";
        }

        /// <summary>
        /// ��ʼ����ȡ΢������
        /// </summary>
        public void Start()
        {
            //�������û����У���ȫ����UserRobot���ݹ���
            while (lstWaitingID.Count == 0)
            {
                if (blnAsyncCancelled) return;
                Thread.Sleep(10);   //������Ϊ�գ���ȴ�
            }
            long lStartSID = lstWaitingID.First.Value;
            long lCurrentSID = 0;
            long lHead = 0;
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
                lCurrentSID = lstWaitingID.First.Value;
                //�����ݿ���л���������Ԫ��
                //ֻ���ڴ���в���ʱ������
                lHead = queueBuffer.FirstValue;
                if (lHead > 0 && lstWaitingID.Count < iQueueLength)
                {
                    lstWaitingID.AddLast( lHead );
                    queueBuffer.Remove( lHead );
                }
                //�Ӷ�ͷ�Ƴ����������β
                lstWaitingID.RemoveFirst();
                Enqueue( lCurrentSID );

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
                List<Comment> lstComment = crawler.GetCommentsOf(lCurrentSID);
                //��־
                Log("����΢��"+lCurrentSID.ToString()+"��" + lstComment.Count.ToString() + "�����ۡ�");

                foreach (Comment comment in lstComment)
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep(50);
                    }
                    lCurrentID = comment.comment_id;
                    lCommentUID = comment.uid;
                    if (!Comment.Exists(lCurrentID))
                    {
                        //��־
                        Log("������" + lCurrentID.ToString() + "�������ݿ�...");
                        comment.Add();
                    }
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
            if (lstWaitingID != null) lstWaitingID.Clear();

            //������ݿ���л���
            queueBuffer.Clear();
        }
    }
}
