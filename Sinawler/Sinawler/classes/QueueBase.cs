using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sinawler.Model;

namespace Sinawler
{
    public class QueueBase
    {
        protected string strLogFile = "";             //��־�ļ�
        private string strLogMessage = "";          //��־����

        protected LinkedList<long> lstWaitingID = new LinkedList<long>();     //�ȴ����е�ID���С�������UserID��Ҳ������StatusID��
        protected int iMaxLengthInMem = 5000;               //�ڴ��ж��г������ޣ�Ĭ��5000
        protected QueueBuffer lstWaitingIDInDB;              //���ݿ���л���

        protected Object oLock = new Object();                    //�������ڸ��������߳�֮��ͬ��
        
        //���캯��
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
        /// ���ⲿ�����ж϶������Ƿ����ָ��ID
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
        /// ȡ����ͷ�������ڶ�β
        /// ����ȡ�գ���DB������
        /// because user buffer class does not have this operation, lstWaitingIDInDB is treated as long type here
        /// on 2011-5-22, it is modified that only lstWaitingID.Count>0, MaxLengthInMem items are removed from DB into memory, so that the performance is enhanced
        /// </summary>
        public long RollQueue () 
        {
            if (lstWaitingID.Count == 0) return 0;
            //��¼��ͷ
            long lFirstValue = lstWaitingID.First.Value;
            lock (oLock)
            {
                //�����β�����Ӷ�ͷ�Ƴ�
                if (lstWaitingIDInDB.Count==0 && lstWaitingID.Count < iMaxLengthInMem)
                    lstWaitingID.AddLast( lFirstValue );
                else
                    lstWaitingIDInDB.Enqueue( lFirstValue );
                lstWaitingID.RemoveFirst();

                //�����ݿ���л���������Ԫ��
                if (lstWaitingID.Count == 0)
                    lstWaitingID = lstWaitingIDInDB.GetFirstValues(iMaxLengthInMem);
            }
            return lFirstValue;
        }

        /// <summary>
        /// ��ָ��ID�ӵ��Լ������У����صĲ���ֵ��ʾ�Ƿ�������Ӳ���
        /// </summary>
        /// <param name="lid"></param>
        public bool Enqueue ( long lID)
        {
            if (lID <= 0) return false;
            if (!QueueExists( lID ))
            {
                //���ڴ����Ѵﵽ���ޣ���ʹ�����ݿ���л���
                //����ʹ�����ݿ���л���
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
        /// ��ָ��ID�Ӷ������Ƴ�
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
            //������ݿ���л���
            if (lstWaitingIDInDB != null) lstWaitingIDInDB.Clear();
        }
    }
}
