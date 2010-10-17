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

        protected LinkedList<long> lstWaitingID = new LinkedList<long>();     //�ȴ����е�ID���С�������UID��Ҳ������StatusID��
        protected int iQueueLength = 5000;               //�ڴ��ж��г������ޣ�Ĭ��5000

        protected bool blnSuspending = false;         //�Ƿ���ͣ��Ĭ��Ϊ����
        protected bool blnOneIDCompleted = false;     //��ɶ�����һ��ID����ȡ���û�ID��΢��ID������ID��

        protected SinaMBCrawler crawler;              //������󡣹��캯���г�ʼ��

        protected int iInitQueueLength = 100;          //��ʼ���г���
        protected QueueBuffer queueBuffer;              //���ݿ���л���
        protected long lCurrentID = 0;               //��ǰ��ȡ���û���΢��ID����ʱ�׳����ݸ�����Ķ���
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

        public int QueueLength
        { set { iQueueLength = value; } }

        public int InitQueueLength
        { get { return iInitQueueLength; } }

        public int LengthOfQueueInMem
        { get { return lstWaitingID.Count; } }

        public int LengthOfQueueInDB
        { get { return queueBuffer.Count; } }

        public bool Suspending
        {
            get { return blnSuspending; }
            set { blnSuspending = value; }
        }

        public bool OneIDCompleted
        { get { return blnOneIDCompleted; } }

        //��������API�Ľӿ�
        public SinaApiService SinaAPI
        { set { api = value; } }

        public long CurrentID
        { get { return lCurrentID; } }

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
        /// ���ⲿ��ȡID�ӵ��Լ�������
        /// </summary>
        /// <param name="lid"></param>
        public void Enqueue ( long lID)
        {
            if (!lstWaitingID.Contains( lID ) && !queueBuffer.Contains( lID ))
            {   
                //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                //����ʹ�����ݿ���л���
                if (lstWaitingID.Count < iQueueLength)
                    lstWaitingID.AddLast( lID );
                else
                    queueBuffer.Enqueue( lID );
            }
        }

        public virtual void Initialize (){}
    }
}
