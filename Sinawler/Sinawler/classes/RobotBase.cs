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
    public class RobotBase
    {
        protected SinaApiService api;
        protected bool blnAsyncCancelled = false;     //ָʾ�����߳��Ƿ�ȡ������������ֹ����ѭ��
        protected string strLogFile = "";             //��־�ļ�
        private string strLogMessage = "";          //��־����

        protected LinkedList<long> lstWaitingID = new LinkedList<long>();     //�ȴ����е�ID���С�������UserID��Ҳ������StatusID��
        protected int iQueueLength = 5000;               //�ڴ��ж��г������ޣ�Ĭ��5000

        protected bool blnSuspending = false;         //�Ƿ���ͣ��Ĭ��Ϊ����

        protected SinaMBCrawler crawler;              //������󡣹��캯���г�ʼ��
        protected QueueBuffer queueBuffer;              //���ݿ���л���
        protected long lCurrentID = 0;               //��ǰ��ȡ���û���΢��ID����ʱ�׳����ݸ�����Ļ����ˣ��ɸ�����������䱩¶��������
        protected BackgroundWorker bwAsync = null;

        //���캯������Ҫ������Ӧ������΢��API��������
        public RobotBase ( SinaApiService oAPI )
        {
            this.api = oAPI;

            SettingItems settings = AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            iQueueLength = settings.QueueLength;

            crawler = new SinaMBCrawler( this.api );
        }

        public bool AsyncCancelled
        {
            set 
            { 
                blnAsyncCancelled = value;
                crawler.StopCrawling = value;
            }
            get { return blnAsyncCancelled; }
        }

        public string LogFile
        {
            set { strLogFile = value; }
            get { return strLogFile; }
        }

        public string LogMessage
        {
            get { return strLogMessage; }
        }

        public int QueueLength
        { set { iQueueLength = value; } }

        public int LengthOfQueueInMem
        { get { return lstWaitingID.Count; } }

        public int LengthOfQueueInDB
        { get { return queueBuffer.Count; } }

        public bool Suspending
        {
            get { return blnSuspending; }
            set { blnSuspending = value; }
        }

        //��������API�Ľӿ�
        public SinaApiService SinaAPI
        { set { api = value; } }

        public BackgroundWorker AsyncWorker
        { set { bwAsync = value; } }

        /// <summary>
        /// ���ⲿ�����ж϶������Ƿ����ָ��ID
        /// </summary>
        /// <param name="lUid"></param>
        public bool QueueExists(long lID)
        {
            return (lstWaitingID.Contains( lID ) || queueBuffer.Contains( lID ));
        }

        /// <summary>
        /// ��ָ��ID�ӵ��Լ�������
        /// </summary>
        /// <param name="lid"></param>
        public void Enqueue ( long lID)
        {
            if (lID <= 0) return;
            if (!QueueExists(lID))
            {   
                //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                //����ʹ�����ݿ���л���
                if (lstWaitingID.Count < iQueueLength)
                    lstWaitingID.AddLast( lID );
                else
                    queueBuffer.Enqueue( lID );
            }
        }

        public void SetRequestFrequency(RequestFrequency rf)
        {
            crawler.SleepTime = rf.Interval;
            crawler.RemainingHits = rf.RemainingHits;
            crawler.ResetTimeInSeconds = rf.ResetTimeInSeconds;
        }

        /// <summary>
        /// ��¼��־
        /// </summary>
        /// <param name="strLog">��־����</param>
        protected void Log(string strLog)
        {
            strLogMessage = DateTime.Now.ToString() + "  " + strLog;
            StreamWriter swComment = File.AppendText( strLogFile );
            swComment.WriteLine( strLogMessage );
            swComment.Close();

            bwAsync.ReportProgress( 0 );
            Thread.Sleep(50);
        }

        public virtual void Initialize (){}
    }
}
