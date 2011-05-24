using System;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using System.Threading;
using System.ComponentModel;
using System.IO;
using Sinawler.Model;
using System.Data;
using System.Xml;

namespace Sinawler
{
    public class RobotBase
    {
        protected SinaApiService api=GlobalPool.API;
        protected bool blnAsyncCancelled = false;     //ָʾ�����߳��Ƿ�ȡ������������ֹ����ѭ��
        protected string strLogFile = "";             //��־�ļ�
        private string strLogMessage = "";          //��־����
        protected bool blnSuspending = false;         //�Ƿ���ͣ��Ĭ��Ϊ����
        protected SinaMBCrawler crawler;              //������󡣹��캯���г�ʼ��
        protected long lCurrentID = 0;               //��ǰ��ȡ���û���΢��ID����ʱ�׳����ݸ�����Ļ����ˣ��ɸ�����������䱩¶��������
        protected BackgroundWorker bwAsync = null;

        //���캯������Ҫ������Ӧ������΢��API��������
        public RobotBase ()
        {
            crawler = new SinaMBCrawler();            
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

        //�����������ʣ������������������������Ƶ�Ȳ�����
        //2011-02-23 ��Ϊ�������Ϊ500ms
        //2011-05-24 ��Ϊ�������Ϊ1s
        //except user relation robot, others get and record the reset time only
        protected virtual void AdjustFreq()
        {
            lock (GlobalPool.Lock)
            {
                GlobalPool.ResetTimeInSeconds = GlobalPool.ResetTimeInSeconds - Convert.ToInt32((DateTime.Now-GlobalPool.LimitUpdateTime).TotalSeconds);
                GlobalPool.LimitUpdateTime = DateTime.Now;
                GlobalPool.RemainingHits--;
            }
            SetCrawlerFreq();
        }

        //�����������ʣ������������������������Ƶ�Ȳ�����
        //2011-02-23 ��Ϊ�������Ϊ500ms
        //2011-05-24 ��Ϊ�������Ϊ1s
        protected void SetCrawlerFreq()
        {
            int iSleep = GlobalPool.ResetTimeInSeconds * 1000;
            if (iSleep < 1000) iSleep = 1000;

            if (GlobalPool.RemainingHits > 0)
            {
                iSleep = Convert.ToInt32(GlobalPool.ResetTimeInSeconds * 1000 / GlobalPool.RemainingHits);
                if (iSleep < 1000) iSleep = 1000; //sleep at least 1s
            }//������ʣ�������ֱ�ӵȴ�ʣ��ʱ��

            crawler.SleepTime = iSleep;
        }

        public virtual void Initialize (){}
    }
}
