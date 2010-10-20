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
            while (lstWaitingID.Count == 0) Thread.Sleep( 1 );   //������Ϊ�գ���ȴ�
            long lStartSID = lstWaitingID.First.Value;
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
                long lCurrentSID = lstWaitingID.First.Value;
                lstWaitingID.RemoveFirst();
                //�����ݿ���л���������Ԫ��
                long lHead = queueBuffer.Dequeue();
                if (lHead > 0)
                    lstWaitingID.AddLast(lHead);
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
                    strLog = DateTime.Now.ToString() + "  " + "��ʼ����֮ǰ���ӵ�������...";
                    bwAsync.ReportProgress(0);
                    Thread.Sleep(50);
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
                strLog = DateTime.Now.ToString() + "  " + "��ȡ΢��" + lCurrentSID.ToString() + "������...";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
                //��ȡ��ǰ΢��������
                List<Comment> lstComment = crawler.GetCommentsOf(lCurrentSID);
                //��־
                strLog = DateTime.Now.ToString() + "  " + "����΢��"+lCurrentSID.ToString()+"��" + lstComment.Count.ToString() + "�����ۡ�";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);

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
                        strLog = DateTime.Now.ToString() + "  " + "������" + lCurrentID.ToString() + "�������ݿ�...";
                        bwAsync.ReportProgress(0);
                        Thread.Sleep(50);
                        comment.Add();
                    }
                }
                #endregion
                //����ٽ��ո��������StatusID�����β
                //��־
                strLog = DateTime.Now.ToString() + "  " + "΢��" + lCurrentSID.ToString() + "����������ȡ��ϣ���������β...";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
                //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                if (lstWaitingID.Count < iQueueLength)
                    lstWaitingID.AddLast(lCurrentSID);
                else
                    queueBuffer.Enqueue(lCurrentSID);

                //��־
                strLog = DateTime.Now.ToString() + "  " + "����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + crawler.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + crawler.RemainingHits.ToString() + "��";
                bwAsync.ReportProgress(0);
                Thread.Sleep(50);
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
