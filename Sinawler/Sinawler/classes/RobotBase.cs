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

        protected LinkedList<long> lstWaitingUID = new LinkedList<long>();     //等待爬行的UID队列
        protected int iQueueLength = 5000;               //内存中队列长度上限，默认5000

        protected int iPreLoadQueue = (int)(EnumPreLoadQueue.NO_PRELOAD);       //是否从数据库中预加载用户队列。默认为“否”
        protected bool blnSuspending = false;         //是否暂停，默认为“否”

        protected SinaMBCrawler crawler;              //爬虫对象。构造函数中初始化

        protected int iInitQueueLength = 100;          //初始队列长度
        protected QueueBuffer queueBuffer;              //数据库队列缓存
        protected long lCurrentUID = 0;               //当前爬取的用户，随时抛出给StatusRobot
        protected BackgroundWorker bwAsync = null;
        protected bool blnOneUserCompleted = false;     //完成一个用户信息（微博）的爬取，即完成队列中一个元素的一次迭代

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
        { get { return lstWaitingUID.Count; } }

        public int LengthOfQueueInDB
        { get { return queueBuffer.Count; } }

        public EnumPreLoadQueue PreLoadQueue
        {
            get { return (EnumPreLoadQueue)iPreLoadQueue; }
            set { iPreLoadQueue = (int)value; }
        }

        public bool Suspending
        {
            get { return blnSuspending; }
            set { blnSuspending = value; }
        }

        //重新设置API的接口
        public SinaApiService SinaAPI
        { set { api = value; } }

        public long ThrownUID
        { get { return lCurrentUID; } }

        public BackgroundWorker AsyncWorker
        { set { bwAsync = value; } }

        public bool OneUserCompleted
        { get { return blnOneUserCompleted; } }

        /// <summary>
        /// 从外部调用判断队列中是否存在指定UID
        /// </summary>
        /// <param name="lUid"></param>
        public bool QueueExists(long lUID)
        {
            return (lstWaitingUID.Contains( lUID ) || queueBuffer.Contains( lUID ));
        }

        /// <summary>
        /// 从外部获取UID加到自己队列中
        /// </summary>
        /// <param name="lUid"></param>
        public void Enqueue ( long lUID)
        {
            if (!lstWaitingUID.Contains( lUID ) && !queueBuffer.Contains( lUID ))
            {   
                //若内存中已达到上限，则使用数据库队列缓存
                //否则使用数据库队列缓存
                if (lstWaitingUID.Count < iQueueLength)
                    lstWaitingUID.AddLast( lUID );
                else
                    queueBuffer.Enqueue( lUID );
            }
        }

        /// <summary>
        /// 以指定的UID为起点开始爬行
        /// </summary>
        /// <param name="lUid"></param>
        public virtual void Start ( long lStartUID ){}

        public virtual void Initialize (){}
    }
}
