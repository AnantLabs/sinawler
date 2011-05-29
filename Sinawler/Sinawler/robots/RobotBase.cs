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
        protected SinaApiService api=GlobalPool.API;
        protected bool blnAsyncCancelled = false;     //指示爬虫线程是否被取消，来帮助中止爬虫循环
        protected string strLogFile = "";             //日志文件
        private string strLogMessage = "";          //日志内容
        protected bool blnSuspending = false;         //是否暂停，默认为“否”
        protected SinaMBCrawler crawler;              //爬虫对象。构造函数中初始化
        protected long lCurrentID = 0;               //当前爬取的用户或微博ID，随时抛出传递给另外的机器人，由各子类决定由其暴露的属性名
        protected BackgroundWorker bwAsync = null;

        //构造函数，需要传入相应的新浪微博API和主界面
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

        //重新设置API的接口
        public SinaApiService SinaAPI
        { set { api = value; } }

        public BackgroundWorker AsyncWorker
        { set { bwAsync = value; } }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="strLog">日志内容</param>
        protected void Log(string strLog)
        {
            strLogMessage = DateTime.Now.ToString() + "  " + strLog;
            StreamWriter swComment = File.AppendText( strLogFile );
            swComment.WriteLine( strLogMessage );
            swComment.Close();

            bwAsync.ReportProgress( 0 );
            Thread.Sleep(50);
        }

        //检查真实请求限制剩余次数，并根据情况调整访问频度并返回
        //2011-02-23 改为间隔下限为500ms
        //2011-05-24 改为间隔下限为1s
        //except user relation robot, others get and record the reset time only
        protected virtual void AdjustRealFreq()
        {
            string strResult = api.check_hits_limit();
            while (strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null)
            {
                System.Threading.Thread.Sleep(100);
                strResult = api.check_hits_limit();
            }
            int iResetTimeInSeconds = 0;
            int iRemainingHits = 0;
            if (api.Format == "xml")
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
            GlobalPool.LimitUpdateTime = DateTime.Now;
            GlobalPool.RemainingHits = iRemainingHits;
            GlobalPool.ResetTimeInSeconds = iResetTimeInSeconds;
            SetCrawlerFreq();
        }

        //从GlobalPool中检查请求限制剩余次数，并根据情况调整访问频度并返回
        //2011-02-23 改为间隔下限为500ms
        //2011-05-24 改为间隔下限为1s
        //except user relation robot, others get and record the reset time only
        protected virtual void AdjustFreq()
        {
            lock (GlobalPool.Lock)
            {
                GlobalPool.ResetTimeInSeconds = GlobalPool.ResetTimeInSeconds - Convert.ToInt32((DateTime.Now-GlobalPool.LimitUpdateTime).TotalSeconds);
                if (GlobalPool.ResetTimeInSeconds <= 0) GlobalPool.ResetTimeInSeconds = 1;
                GlobalPool.LimitUpdateTime = DateTime.Now;
                GlobalPool.RemainingHits--;
                if (GlobalPool.RemainingHits < 0) GlobalPool.RemainingHits=0;
            }
            SetCrawlerFreq();
        }

        //set the frequency to crawler
        protected void SetCrawlerFreq()
        {
            int iSleep = GlobalPool.ResetTimeInSeconds * 1000;
            if (iSleep < 1000) iSleep = 1000;

            if (GlobalPool.RemainingHits > 0)
            {
                iSleep = Convert.ToInt32(GlobalPool.ResetTimeInSeconds * 1000 / GlobalPool.RemainingHits);
                if (iSleep < 1000) iSleep = 1000; //sleep at least 1s
            }//若已无剩余次数，直接等待剩余时间

            crawler.SleepTime = iSleep;
        }

        public virtual void Initialize (){}
    }
}
