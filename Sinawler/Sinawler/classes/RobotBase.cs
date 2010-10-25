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
        protected string strLog = "";                 //��־����
        protected string strQueueInfo = "";           //�ȴ����е������Ϣ

        protected LinkedList<long> lstWaitingUID = new LinkedList<long>();     //�ȴ����е�UID����
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
            set { blnAsyncCancelled = value; }
            get { return blnAsyncCancelled; }
        }

        public string LogFile
        {
            set { strLogFile = value; }
            get { return strLogFile; }
        }

        public string LogMessage
        {
            set { strLog = value; }
            get { return strLog; }
        }

        public string QueueInfo
        {
            set { strQueueInfo = value; }
            get { return strQueueInfo; }
        }

        public int QueueLength
        { set { iQueueLength = value; } }

        public int LengthOfQueueInMem
        { get { return lstWaitingUID.Count; } }

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
        /// ���ⲿ�����ж϶������Ƿ����ָ��UID
        /// </summary>
        /// <param name="lUid"></param>
        public bool QueueExists(long lUID)
        {
            return (lstWaitingUID.Contains( lUID ) || queueBuffer.Contains( lUID ));
        }

        /// <summary>
        /// ���ⲿ��ȡUID�ӵ��Լ�������
        /// </summary>
        /// <param name="lUid"></param>
        public void Enqueue ( long lUID)
        {
            if (!lstWaitingUID.Contains( lUID ) && !queueBuffer.Contains( lUID ))
            {   
                //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                //����ʹ�����ݿ���л���
                if (lstWaitingUID.Count < iQueueLength)
                    lstWaitingUID.AddLast( lUID );
                else
                    queueBuffer.Enqueue( lUID );
            }
        }

        public void SetRequestFrequency(RequestFrequency rf)
        {
            crawler.SleepTime = rf.Interval;
            crawler.RemainingHits = rf.RemainingHits;
            crawler.ResetTimeInSeconds = rf.ResetTimeInSeconds;
        }

        public virtual void Initialize (){}
    }
}
