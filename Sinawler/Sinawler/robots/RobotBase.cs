using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using System.Threading;
using System.ComponentModel;
using System.IO;
using Sinawler.Model;
using System.Data;
using System.Xml;
using Newtonsoft.Json;

namespace Sinawler
{
    public class RobotBase
    {
        protected APIInfo api;
        protected bool blnAsyncCancelled = false;     //ָʾ�����߳��Ƿ�ȡ������������ֹ����ѭ��
        protected string strLogFile = "";             //��־�ļ�
        private string strLogMessage = "";          //��־����
        protected bool blnSuspending = false;         //�Ƿ���ͣ��Ĭ��Ϊ����
        protected SinaMBCrawler crawler;              //������󡣹��캯���г�ʼ��
        protected long lCurrentID = 0;               //��ǰ��ȡ���û���΢��ID����ʱ�׳����ݸ�����Ļ����ˣ��ɸ�����������䱩¶��������
        protected BackgroundWorker bwAsync = null;

        //���캯������Ҫ������Ӧ������΢��API��������
        public RobotBase (SysArgFor robotType)
        {
            crawler = new SinaMBCrawler(robotType);
            api = GlobalPool.GetAPI(robotType);
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

        //�����ʵ��������ʣ������������������������Ƶ�Ȳ�����
        //2011-02-23 ��Ϊ�������Ϊ500ms
        //2011-05-24 ��Ϊ�������Ϊ1s
        //except user relation robot, others get and record the reset time only
        protected virtual void AdjustRealFreq()
        {
            if (api == null) return;
            string strResult = api.API.check_hits_limit();
            while (strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null)
            {
                System.Threading.Thread.Sleep(100);
                strResult = api.API.check_hits_limit();
            }
            int iResetTimeInSeconds = 0;
            int iRemainingHits = 0;
            if (api.API.Format == "xml")
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strResult);

                iResetTimeInSeconds = Convert.ToInt32(xmlDoc.GetElementsByTagName("reset-time-in-seconds")[0].InnerText);
                iRemainingHits = Convert.ToInt32(xmlDoc.GetElementsByTagName("remaining-hits")[0].InnerText);
            }//xml
            else
            {
                //such as {"remaining_hits":804,"hourly_limit":1000,"reset_time_in_seconds":2462,"reset_time":"Sun May 22 17:00:00 +0800 2011"}
                Hashtable oJsonHitsLimit = (Hashtable)JsonConvert.DeserializeObject(strResult, typeof(Hashtable));
                foreach (DictionaryEntry de in oJsonHitsLimit)
                {
                    if (de.Key.ToString() == "reset_time_in_seconds") iResetTimeInSeconds = Convert.ToInt32(de.Value);
                    if (de.Key.ToString() == "remaining_hits") iRemainingHits = Convert.ToInt32(de.Value);
                }
            }

            if(api!=null)
            {
                api.LimitUpdateTime = DateTime.Now;
                api.RemainingHits = iRemainingHits;
                api.ResetTimeInSeconds = iResetTimeInSeconds;
            }
        }

        //��GlobalPool�м����������ʣ������������������������Ƶ�Ȳ�����
        //2011-02-23 ��Ϊ�������Ϊ500ms
        //2011-05-24 ��Ϊ�������Ϊ1s
        //except user relation robot, others get and record the reset time only
        protected virtual void AdjustFreq()
        {
            if(api!=null)
            {
                api.RemainingHits--;
                api.ResetTimeInSeconds = api.ResetTimeInSeconds - Convert.ToInt32((DateTime.Now - api.LimitUpdateTime).TotalSeconds);
                api.LimitUpdateTime = DateTime.Now;
                if (api.ResetTimeInSeconds <= 0 || api.RemainingHits<0) AdjustRealFreq();//GlobalPool.ResetTimeInSeconds = 1;
            }
        }

        //set the frequency to crawler
        protected void SetCrawlerFreq()
        {
            if (api != null)
            {
                int iSleep = api.ResetTimeInSeconds * 1000;
                if (iSleep < 1000) iSleep = 1000;
                if (api.RemainingHits > 0)
                {
                    iSleep = Convert.ToInt32(api.ResetTimeInSeconds * 1000 / api.RemainingHits);
                    if (iSleep < 1000) iSleep = 1000; //sleep at least 1s
                }
                crawler.SleepTime = iSleep;
            }
        }

        public virtual void Initialize (){}
    }
}
