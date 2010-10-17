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
        private LinkedList<long> lstWaitingStatusID = new LinkedList<long>();   //�ȴ����е�΢��ID����
        private long lCommentUID = 0;    //�����˵�UID

        //���캯������Ҫ������Ӧ������΢��API
        public CommentRobot(SinaApiService oAPI)
            : base(oAPI)
        {
            queueBuffer = new QueueBuffer(QueueBufferTarget.FOR_COMMENT);
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_comment.log";
        }

        /// <summary>
        /// ��ʼ����ȡ΢������
        /// </summary>
        public void Start()
        {
            long lStartSID = 0;
            long lCurrentSID = 0;
            //��ȡ��һ��StatusID����ȡ��֮�󣬼�ΪStartID�����ڱȽϲ�ȷ����������
            while (lstWaitingID.Count == 0) bwAsync.ReportProgress(0);//�������¼����Ӷ���΢�������˴��ݵ�΢�������л�ȡ��ͷ
            lStartSID = lstWaitingID.First.Value;

            //�Զ�������ѭ�����У�ֱ���в�����ͣ��ֹͣ
            while(true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep(50);
                }
                //����ͷȡ��
                lCurrentSID = lstWaitingID.First.Value;
                lstWaitingID.RemoveFirst();
                //�����ݿ���л���������Ԫ��
                long lHead = queueBuffer.Dequeue();
                if (lHead > 0)
                    lstWaitingID.AddLast(lHead);
                blnOneIDCompleted = false;  //��ʼ�µ�ID
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
                    bwAsync.ReportProgress(100);
                    Thread.Sleep(5);
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
                bwAsync.ReportProgress(100);
                Thread.Sleep(5);
                //��ȡ��ǰ΢��������
                List<Comment> lstComment = crawler.GetCommentsOf(lCurrentSID);
                //��־
                strLog = DateTime.Now.ToString() + "  " + "����" + lstComment.Count.ToString() + "�����ۡ�";
                bwAsync.ReportProgress(100);
                Thread.Sleep(5);

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
                        bwAsync.ReportProgress(100);
                        Thread.Sleep(5);
                        comment.Add();
                    }
                }
                #endregion
                blnOneIDCompleted = true;  //���һ��ID
                //����ٽ��ո��������StatusID�����β
                //��־
                strLog = DateTime.Now.ToString() + "  " + "΢��" + lCurrentSID.ToString() + "����������ȡ��ϣ���������β...";
                bwAsync.ReportProgress(100);
                Thread.Sleep(5);
                //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                if (lstWaitingID.Count < iQueueLength)
                    lstWaitingID.AddLast(lCurrentSID);
                else
                    queueBuffer.Enqueue(lCurrentSID);
                //��������Ƶ��
                //����û�����Ƶ��
                crawler.AdjustFreq();
                //��־
                strLog = DateTime.Now.ToString() + "  " + "����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + crawler.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + crawler.RemainingHits.ToString() + "��";
                bwAsync.ReportProgress(100);
                Thread.Sleep(5);
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
