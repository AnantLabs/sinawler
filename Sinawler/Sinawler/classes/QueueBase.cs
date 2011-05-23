using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sinawler.Model;

namespace Sinawler
{
    public class QueueBase
    {
        protected string strLogFile = "";             //日志文件
        private string strLogMessage = "";          //日志内容

        protected LinkedList<long> lstWaitingID = new LinkedList<long>();     //等待爬行的ID队列。可能是UserID，也可能是StatusID等
        protected int iMaxLengthInMem = 5000;               //内存中队列长度上限，默认5000
        protected QueueBuffer lstWaitingIDInDB;              //数据库队列缓存

        protected Object oLock = new Object();                    //锁。用于各机器人线程之间同步
        
        //构造函数
        public QueueBase()
        {
            SettingItems settings = AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            iMaxLengthInMem = settings.MaxLengthInMem;
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

        public int MaxLengthInMem
        { set { iMaxLengthInMem = value; } }

        public int CountInMem
        { get { return lstWaitingID.Count; } }

        public int CountInDB
        { get { return lstWaitingIDInDB.Count; } }

        public int Count
        {
            get { return lstWaitingID.Count + lstWaitingIDInDB.Count; }
        }

        public long FirstValue
        {
            get 
            {
                if (lstWaitingID.Count > 0)
                    return lstWaitingID.First.Value;
                else
                    return 0;
            }
        }

        public Object Lock
        { get{ return oLock; }}

        /// <summary>
        /// 从外部调用判断队列中是否存在指定ID
        /// </summary>
        /// <param name="lUid"></param>
        public bool QueueExists(long lID)
        {
            bool blnResult = false;
            lock(oLock)
            {
                blnResult=PubHelper.ContainsInQueue<long>(lstWaitingID,lID ) || lstWaitingIDInDB.Contains( lID );
            }
            return blnResult;
        }

        /// <summary>
        /// 取出队头，并放在队尾
        /// 若已取空，从DB中移入
        /// because user buffer class does not have this operation, lstWaitingIDInDB is treated as long type here
        /// on 2011-5-22, it is modified that only lstWaitingID.Count>0, MaxLengthInMem items are removed from DB into memory, so that the performance is enhanced
        /// </summary>
        public long RollQueue () 
        {
            if (lstWaitingID.Count == 0) return 0;
            //记录队头
            long lFirstValue = lstWaitingID.First.Value;
            lock (oLock)
            {
                //移入队尾，并从队头移除
                if (lstWaitingIDInDB.Count==0 && lstWaitingID.Count < iMaxLengthInMem)
                    lstWaitingID.AddLast( lFirstValue );
                else
                    lstWaitingIDInDB.Enqueue( lFirstValue );
                lstWaitingID.RemoveFirst();

                //从数据库队列缓存中移入元素
                if (lstWaitingID.Count == 0)
                    lstWaitingID = lstWaitingIDInDB.GetFirstValues(iMaxLengthInMem);
            }
            return lFirstValue;
        }

        /// <summary>
        /// 将指定ID加到自己队列中，返回的布尔值表示是否做了入队操作
        /// </summary>
        /// <param name="lid"></param>
        public bool Enqueue ( long lID)
        {
            if (lID <= 0) return false;
            if (!QueueExists( lID ))
            {
                //若内存中已达到上限，则使用数据库队列缓存
                //否则使用数据库队列缓存
                lock (oLock)
                {
                    if (lstWaitingIDInDB.Count==0 && lstWaitingID.Count < iMaxLengthInMem)
                        lstWaitingID.AddLast(lID);
                    else
                        lstWaitingIDInDB.Enqueue(lID);
                }

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 将指定ID从队列中移除
        /// </summary>
        /// <param name="lid"></param>
        public void Remove ( long lID )
        {
            if (lID <= 0) return;
            lstWaitingID.Remove( lID );
            lstWaitingIDInDB.Remove( lID );
        }

        public void Initialize ()
        {
            if (lstWaitingID != null) lstWaitingID.Clear();
            //清空数据库队列缓存
            if (lstWaitingIDInDB != null) lstWaitingIDInDB.Clear();
        }
    }
}
