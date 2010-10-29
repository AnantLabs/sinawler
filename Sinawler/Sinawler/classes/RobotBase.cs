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
        protected bool blnAsyncCancelled = false;     //指示爬虫线程是否被取消，来帮助中止爬虫循环
        protected string strLogFile = "";             //日志文件
        protected string strLog = "";                 //日志内容

        protected LinkedList<long> lstWaitingID = new LinkedList<long>();     //等待爬行的ID队列。可能是UID，也可能是StatusID等
        protected int iQueueLength = 5000;               //内存中队列长度上限，默认5000

        protected bool blnSuspending = false;         //是否暂停，默认为“否”

        protected SinaMBCrawler crawler;              //爬虫对象。构造函数中初始化
        protected QueueBuffer queueBuffer;              //数据库队列缓存
        protected long lCurrentID = 0;               //当前爬取的用户或微博ID，随时抛出传递给另外的机器人，由各子类决定由其暴露的属性名
        protected BackgroundWorker bwAsync = null;

        //构造函数，需要传入相应的新浪微博API和主界面
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
            set { strLog = value; }
            get { return strLog; }
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

        //重新设置API的接口
        public SinaApiService SinaAPI
        { set { api = value; } }

        public BackgroundWorker AsyncWorker
        { set { bwAsync = value; } }

        /// <summary>
        /// 从外部调用判断队列中是否存在指定ID
        /// </summary>
        /// <param name="lUid"></param>
        public bool QueueExists(long lID)
        {
            return (lstWaitingID.Contains( lID ) || queueBuffer.Contains( lID ));
        }

        /// <summary>
        /// 将指定ID加到自己队列中
        /// </summary>
        /// <param name="lid"></param>
        public void Enqueue ( long lID)
        {
            if (!QueueExists(lID))
            {   
                //若内存中已达到上限，则使用数据库队列缓存
                //否则使用数据库队列缓存
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

        public virtual void Initialize (){}
    }
}
