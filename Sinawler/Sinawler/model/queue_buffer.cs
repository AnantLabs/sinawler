using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
    /// <summary>
    /// ��QueueBuffer�����ڴ��еĴ����е�UserID���г��ȳ���ָ������ʱ����ʼʹ�����ݿⱣ����С�
    /// ���ݿ��зֱ��������û������˺�΢�������˵��������б���������ĸ����ɹ��캯���еĲ���ָ��
    /// ���ݿ��еĶ������ڴ��еĶ��еĺ��棬����enqueue_time�ֶ�����
    /// ��ͨ�������ṩ�ķ�����������в����������Add������Remove������ӡ�ɾ��ָ���ڵ�
    /// </summary>
    public class QueueBuffer
    {
        private QueueBufferFor _target = QueueBufferFor.USER_INFO;
        private int iCount=0;
        private long lFirstValue;   //�׽ڵ�ֵ

        public object FirstValue
        {
            get { return lFirstValue; }
        }

        #region  ��Ա����
        ///���캯��
        ///<param name="target">Ҫ������Ŀ��</param>
        public QueueBuffer(QueueBufferFor target)
        {
            _target = target;
        }

        /// <summary>
        /// �����ݿ��ж�ȡ��ͷֵ
        /// because user buffer class does not need this operation, lstWaitingIDInDB is treated as long type here,
        /// and the case of QueueBufferFor.USER_BUFFER is ignored
        /// </summary>
        private void GetFirstValue()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DataRow dr;
            switch (_target)
            {
                case QueueBufferFor.USER_INFO:
                    dr = db.GetDataRow("select top 1 user_id from queue_buffer_for_userInfo order by enqueue_time");
                    if (dr == null) return;
                    lFirstValue = Convert.ToInt64(dr["user_id"]);
                    break;
                case QueueBufferFor.USER_RELATION:
                    dr = db.GetDataRow("select top 1 user_id from queue_buffer_for_userRelation order by enqueue_time");
                    if (dr == null) return;
                    lFirstValue = Convert.ToInt64(dr["user_id"]);
                    break;
                case QueueBufferFor.USER_TAG:
                    dr = db.GetDataRow("select top 1 user_id from queue_buffer_for_tag order by enqueue_time");
                    if (dr == null) return;
                    lFirstValue = Convert.ToInt64(dr["user_id"]);
                    break;
                case QueueBufferFor.STATUS:
                    dr = db.GetDataRow("select top 1 user_id from queue_buffer_for_status order by enqueue_time");
                    if (dr == null) return;
                    lFirstValue = Convert.ToInt64(dr["user_id"]);
                    break;
                case QueueBufferFor.COMMENT:
                    dr = db.GetDataRow("select top 1 status_id from queue_buffer_for_comment order by enqueue_time");
                    if (dr == null) return;
                    lFirstValue = Convert.ToInt64(dr["status_id"]);
                    break;
            }
        }

        /// <summary>
        /// �����ݿ��ж�ȡ��ͷָ��������ֵ������������ʽ���أ���ֱ��������ָ�������
        /// because user buffer class does not need this operation, lstWaitingIDInDB is treated as long type here,
        /// and the case of QueueBufferFor.USER_BUFFER is ignored
        /// </summary>
        public LinkedList<long> GetFirstValues(int iCount)
        {
            LinkedList<long> lstResult = new LinkedList<long>();
            Database db = DatabaseFactory.CreateDatabase();
            DataSet ds=null;
            switch (_target)
            {
                case QueueBufferFor.USER_RELATION:
                    ds = db.GetDataSet("select top " + iCount.ToString() + " user_id from queue_buffer_for_userRelation order by enqueue_time");
                    break;
                case QueueBufferFor.USER_INFO:
                    ds = db.GetDataSet("select top "+iCount.ToString()+" user_id from queue_buffer_for_userInfo order by enqueue_time");
                    break;
                case QueueBufferFor.USER_TAG:
                    ds = db.GetDataSet("select top " + iCount.ToString() + " user_id from queue_buffer_for_tag order by enqueue_time");
                    break;
                case QueueBufferFor.STATUS:
                    ds = db.GetDataSet("select top " + iCount.ToString() + " user_id from queue_buffer_for_status order by enqueue_time");
                    break;
                case QueueBufferFor.COMMENT:
                    ds = db.GetDataSet("select top " + iCount.ToString() + " status_id from queue_buffer_for_comment order by enqueue_time");
                    break;
            }
            if (ds != null)
            { 
                string strIDsToBeDeleted="(";
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    lstResult.AddLast(Convert.ToInt64(dr[0]));
                    strIDsToBeDeleted += dr[0].ToString() + ",";
                }
                strIDsToBeDeleted += "0)";
                //delete the records from DB
                switch (_target)
                {
                    case QueueBufferFor.USER_RELATION:
                        db.CountByExecuteSQL("delete from queue_buffer_for_userRelation where user_id in " + strIDsToBeDeleted);
                        break;
                    case QueueBufferFor.USER_INFO:
                        db.CountByExecuteSQL("delete from queue_buffer_for_userInfo where user_id in "+strIDsToBeDeleted);
                        break;
                    case QueueBufferFor.USER_TAG:
                        db.CountByExecuteSQL("delete from queue_buffer_for_tag where user_id in " + strIDsToBeDeleted);
                        break;
                    case QueueBufferFor.STATUS:
                        db.CountByExecuteSQL("delete from queue_buffer_for_status where user_id in " + strIDsToBeDeleted);
                        break;
                    case QueueBufferFor.COMMENT:
                        db.CountByExecuteSQL("delete from queue_buffer_for_comment where status_id in " + strIDsToBeDeleted);
                        break;
                }
            }
            return lstResult;
        }

        /// <summary>
        /// �Ƿ���ڸü�¼
        /// </summary>
        public bool Contains(Object obj)
        {
            Database db = DatabaseFactory.CreateDatabase();
            int count = 0;
            if (obj.GetType() == typeof(Int64))
                switch (_target)
                {
                    case QueueBufferFor.USER_RELATION:
                        count = db.CountByExecuteSQLSelect("select count(1) from queue_buffer_for_userRelation where user_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.USER_INFO:
                        count = db.CountByExecuteSQLSelect("select count(1) from queue_buffer_for_userInfo where user_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.USER_TAG:
                        count = db.CountByExecuteSQLSelect("select count(1) from queue_buffer_for_tag where user_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.STATUS:
                        count = db.CountByExecuteSQLSelect("select count(1) from queue_buffer_for_status where user_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.COMMENT:
                        count = db.CountByExecuteSQLSelect("select count(1) from queue_buffer_for_comment where status_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.USER_BUFFER:
                        count = db.CountByExecuteSQLSelect("select count(1) from queue_buffer_for_userBuffer where user_id=" + ((User)obj).user_id.ToString());
                        break;
                }
            if (obj.GetType() == typeof(User) && _target == QueueBufferFor.USER_BUFFER)
                count = db.CountByExecuteSQLSelect("select count(1) from queue_buffer_for_userBuffer where user_id=" + ((User)obj).user_id.ToString());
            return count > 0;
        }

        /// <summary>
        /// һ��������ӣ�ע����Add������FirstValue�������𣩣��ö������ΪID������Ϊģ�����ʵ��
        /// because user buffer does not use FirstValue, lFirstValue record id only for other 5 buffers
        /// </summary>
        public void Enqueue(Object obj)
        {
            Database db = DatabaseFactory.CreateDatabase();
            Hashtable htValues = new Hashtable();

            htValues.Add("enqueue_time", "'" + DateTime.Now.ToString() + "'");

            if (obj.GetType() == typeof(Int64))
                switch (_target)
                {
                    case QueueBufferFor.USER_RELATION:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_userRelation", htValues);
                        break;
                    case QueueBufferFor.USER_INFO:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_userInfo", htValues);
                        break;
                    case QueueBufferFor.USER_TAG:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_tag", htValues);
                        break;
                    case QueueBufferFor.STATUS:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_status", htValues);
                        break;
                    case QueueBufferFor.COMMENT:
                        htValues.Add("status_id", obj.ToString());
                        db.Insert("queue_buffer_for_comment", htValues);
                        break;
                }
            if (obj.GetType() == typeof(User) && _target == QueueBufferFor.USER_BUFFER)
            {
                User user = (User)obj;
                user.AddToUserBuffer();
            }
            iCount++;
            //�����µĶ�ͷֵ
            if (iCount == 1)
                lFirstValue = Convert.ToInt64(obj);
        }

        /// <summary>
        /// ��ͷUser����
        /// </summary>
        public Object Dequeue()
        {
            //�ȼ�¼ͷ�ڵ�,��ɾ��ͷ�ڵ�
            Object oResult = lFirstValue;
            this.Remove(oResult);
            return oResult;
        }

        /// <summary>
        /// ����ָ���ڵ㣬�������ʱ�䣬�������ӵĽڵ�����ڶ��е��κ�λ�ã�ע����Enqueue������
        /// </summary>
        public void Add(Object obj, string enqueue_time)
        {
            Database db = DatabaseFactory.CreateDatabase();
            Hashtable htValues = new Hashtable();

            htValues.Add("enqueue_time", "'" + enqueue_time + "'");
            if (obj.GetType() == typeof(Int64))
                switch (_target)
                {
                    case QueueBufferFor.USER_RELATION:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_userRelation", htValues);
                        break;
                    case QueueBufferFor.USER_INFO:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_userInfo", htValues);
                        break;
                    case QueueBufferFor.USER_TAG:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_tag", htValues);
                        break;
                    case QueueBufferFor.STATUS:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_status", htValues);
                        break;
                    case QueueBufferFor.COMMENT:
                        htValues.Add("status_id", obj.ToString());
                        db.Insert("queue_buffer_for_comment", htValues);
                        break;
                }
            if (obj.GetType() == typeof(User) && _target == QueueBufferFor.USER_BUFFER)
            {
                User user = (User)obj;
                user.AddToUserBuffer();
            }
            iCount++;
            //�����µĶ�ͷֵ
            //only available to long ids
            if (iCount == 1 && obj.GetType()==typeof(Int64))
                lFirstValue = Convert.ToInt64(obj);
        }

        /// <summary>
        /// ɾ��ָ���ڵ�
        /// </summary>
        public void Remove(Object obj)
        {
            int iRowsDeleted = 0;
            Database db = DatabaseFactory.CreateDatabase();
            if (obj.GetType() == typeof(Int64))
                switch (_target)
                {
                    case QueueBufferFor.USER_RELATION:
                        iRowsDeleted = db.CountByExecuteSQL("delete from queue_buffer_for_userRelation where user_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.USER_INFO:
                        iRowsDeleted=db.CountByExecuteSQL("delete from queue_buffer_for_userInfo where user_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.USER_TAG:
                        iRowsDeleted = db.CountByExecuteSQL("delete from queue_buffer_for_tag where user_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.STATUS:
                        iRowsDeleted = db.CountByExecuteSQL("delete from queue_buffer_for_status where user_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.COMMENT:
                        iRowsDeleted = db.CountByExecuteSQL("delete from queue_buffer_for_comment where status_id=" + obj.ToString());
                        break;
                }
            if (obj.GetType() == typeof(User) && _target == QueueBufferFor.USER_BUFFER)
                iRowsDeleted = db.CountByExecuteSQL("delete from queue_buffer_for_userBuffer where user_id=" + ((User)obj).user_id.ToString());

            if(iRowsDeleted>0)  iCount=iCount-iRowsDeleted;
            if (obj.GetType()==typeof(Int64) && lFirstValue==Convert.ToInt64(obj))
                //�����µĶ�ͷֵ
                GetFirstValue();
        }

        /// <summary>
        /// �������
        /// </summary>
        public void Clear()
        {
            Database db = DatabaseFactory.CreateDatabase();
            switch (_target)
            {
                case QueueBufferFor.USER_RELATION:
                    db.CountByExecuteSQL("truncate table queue_buffer_for_userRelation");
                    break;
                case QueueBufferFor.USER_INFO:
                    db.CountByExecuteSQL("truncate table queue_buffer_for_userInfo");
                    break;
                case QueueBufferFor.USER_TAG:
                    db.CountByExecuteSQL("truncate table queue_buffer_for_tag");
                    break;
                case QueueBufferFor.STATUS:
                    db.CountByExecuteSQL("truncate table queue_buffer_for_status");
                    break;
                case QueueBufferFor.COMMENT:
                    db.CountByExecuteSQL("truncate table queue_buffer_for_comment");
                    break;
                case QueueBufferFor.USER_BUFFER:
                    db.CountByExecuteSQL("truncate table queue_buffer_for_userBuffer");
                    break;
            }
            iCount = 0;
            lFirstValue = 0;
        }

        public int Count
        {
            get
            {
                if (iCount % 5000 == 0)    //ÿ����5000����¼�����²�ѯһ�Σ��Լ������ݿ��ѯ���������
                {
                    Database db = DatabaseFactory.CreateDatabase();
                    switch (_target)
                    {
                        case QueueBufferFor.USER_RELATION:
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from queue_buffer_for_userRelation");
                            break;
                        case QueueBufferFor.USER_INFO:
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from queue_buffer_for_userInfo");
                            break;
                        case QueueBufferFor.USER_TAG:
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from queue_buffer_for_tag");
                            break;
                        case QueueBufferFor.STATUS:
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from queue_buffer_for_status");
                            break;
                        case QueueBufferFor.COMMENT:
                            iCount = db.CountByExecuteSQLSelect("select count(status_id) as cnt from queue_buffer_for_comment");
                            break;
                        case QueueBufferFor.USER_BUFFER:
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from queue_buffer_for_userBuffer");
                            break;
                    }
                    if (iCount == -1) iCount = 0;
                }
                return iCount;
            }
        }

        #endregion  ��Ա����
    }
}