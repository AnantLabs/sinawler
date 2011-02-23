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
        protected SinaApiService api;
        protected bool blnAsyncCancelled = false;     //ָʾ�����߳��Ƿ�ȡ������������ֹ����ѭ��
        protected string strLogFile = "";             //��־�ļ�
        private string strLogMessage = "";          //��־����
        protected bool blnSuspending = false;         //�Ƿ���ͣ��Ĭ��Ϊ����
        protected SinaMBCrawler crawler;              //������󡣹��캯���г�ʼ��
        protected long lCurrentID = 0;               //��ǰ��ȡ���û���΢��ID����ʱ�׳����ݸ�����Ļ����ˣ��ɸ�����������䱩¶��������
        protected BackgroundWorker bwAsync = null;

        //���캯������Ҫ������Ӧ������΢��API��������
        public RobotBase ( SinaApiService oAPI )
        {
            this.api = oAPI;
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
        protected void AdjustFreq()
        {
            string strResult = api.check_hits_limit();
            if (strResult == null) return;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml( strResult );

            int iResetTimeInSeconds = Convert.ToInt32( xmlDoc.GetElementsByTagName( "reset-time-in-seconds" )[0].InnerText );
            int iRemainingHits = Convert.ToInt32( xmlDoc.GetElementsByTagName( "remaining-hits" )[0].InnerText );

            //������ʣ�������ֱ�ӵȴ�ʣ��ʱ��
            if (iRemainingHits == 0)
            {
                crawler.SleepTime = iResetTimeInSeconds * 1000;
                crawler.RemainingHits = iRemainingHits;
                crawler.ResetTimeInSeconds = iResetTimeInSeconds;
            }
            else
            {
                //����
                int iSleep = Convert.ToInt32( iResetTimeInSeconds * 1000 / iRemainingHits );
                if (iSleep <= 0) iSleep = 1;

                crawler.SleepTime = iSleep;
                crawler.RemainingHits = iRemainingHits;
                crawler.ResetTimeInSeconds = iResetTimeInSeconds;
            }
        }

        public virtual void Initialize (){}
    }
}
