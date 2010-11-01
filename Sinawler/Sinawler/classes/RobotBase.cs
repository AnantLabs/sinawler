using System;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using System.Threading;
using System.ComponentModel;
using System.IO;
using Sinawler.Model;
using System.Data;

namespace Sinawler
{
    public class RobotBase
    {
        protected SinaApiService api;
        protected bool blnAsyncCancelled = false;     //指示爬虫线程是否被取消，来帮助中止爬虫循环
        protected string strLogFile = "";             //日志文件
        private string strLogMessage = "";          //日志内容
        protected bool blnSuspending = false;         //是否暂停，默认为“否”
        protected SinaMBCrawler crawler;              //爬虫对象。构造函数中初始化
        protected long lCurrentID = 0;               //当前爬取的用户或微博ID，随时抛出传递给另外的机器人，由各子类决定由其暴露的属性名
        protected BackgroundWorker bwAsync = null;

        //构造函数，需要传入相应的新浪微博API和主界面
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

        //重新设置API的接口
        public SinaApiService SinaAPI
        { set { api = value; } }

        public BackgroundWorker AsyncWorker
        { set { bwAsync = value; } }

        public void SetRequestFrequency(RequestFrequency rf)
        {
            crawler.SleepTime = rf.Interval;
            crawler.RemainingHits = rf.RemainingHits;
            crawler.ResetTimeInSeconds = rf.ResetTimeInSeconds;
        }

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

        public virtual void Initialize (){}
    }
}
