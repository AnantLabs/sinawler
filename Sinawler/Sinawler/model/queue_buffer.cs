using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
    public enum QueueBufferFor { USER_INFO = 0, USER_RELATION = 1, USER_TAG = 2, STATUS = 3, COMMENT = 4, USER_BUFFER = 5 };

    /// <summary>
    /// 类QueueBuffer，当内存中的待爬行的UserID队列长度超过指定长度时，开始使用数据库保存队列。
    /// 数据库中分别有用于用户机器人和微博机器人的两个队列表，具体操作哪个，由构造函数中的参数指明
    /// 数据库中的队列在内存中的队列的后面，根据enqueue_time字段排序
    /// 可通过此类提供的方法做出入队列操作，或调用Add方法、Remove方法添加、删除指定节点
    /// </summary>
    public class QueueBuffer
    {
        private QueueBufferFor _target = QueueBufferFor.USER_INFO;
        private int iCount = 0;     //队列长度
        private Object oFirstValue = null;   //首节点值

        public object FirstValue
        {
            get { return oFirstValue; }
        }

        #region  成员方法
        ///构造函数
        ///<param name="target">要操作的目标</param>
        public QueueBuffer(QueueBufferFor target)
        {
            _target = target;
        }

        /// <summary>
        /// 从数据库中读取队头值
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
                    oFirstValue = Convert.ToInt64(dr["user_id"]);
                    break;
                case QueueBufferFor.USER_RELATION:
                    dr = db.GetDataRow("select top 1 user_id from queue_buffer_for_userRelation order by enqueue_time");
                    if (dr == null) return;
                    oFirstValue = Convert.ToInt64(dr["user_id"]);
                    break;
                case QueueBufferFor.USER_TAG:
                    dr = db.GetDataRow("select top 1 user_id from queue_buffer_for_tag order by enqueue_time");
                    if (dr == null) return;
                    oFirstValue = Convert.ToInt64(dr["user_id"]);
                    break;
                case QueueBufferFor.STATUS:
                    dr = db.GetDataRow("select top 1 user_id from queue_buffer_for_status order by enqueue_time");
                    if (dr == null) return;
                    oFirstValue = Convert.ToInt64(dr["user_id"]);
                    break;
                case QueueBufferFor.COMMENT:
                    dr = db.GetDataRow("select top 1 status_id from queue_buffer_for_comment order by enqueue_time");
                    if (dr == null) return;
                    oFirstValue = Convert.ToInt64(dr["status_id"]);
                    break;
                case QueueBufferFor.USER_BUFFER:
                    User user = new User();
                    dr = db.GetDataRow("select top 1 user_id from queue_buffer_for_userBuffer order by enqueue_time");
                    if (dr == null) return;
                    if (user.GetModelFromUserBuffer(Convert.ToInt64(dr["user_id"])))
                        oFirstValue = user;
                    break;
            }
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Contains(Object obj)
        {
            Database db = DatabaseFactory.CreateDatabase();
            int count = 0;
            if (obj.GetType() == typeof(Int64))
                switch (_target)
                {
                    case QueueBufferFor.USER_INFO:
                        count = db.CountByExecuteSQLSelect("select count(1) from queue_buffer_for_userInfo where user_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.USER_RELATION:
                        count = db.CountByExecuteSQLSelect("select count(1) from queue_buffer_for_userRelation where user_id=" + obj.ToString());
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
        /// 一个对象入队（注意与Add函数在FirstValue处的区别），该对象或者为ID，或者为模型类的实例
        /// </summary>
        public void Enqueue(Object obj)
        {
            Database db = DatabaseFactory.CreateDatabase();
            Hashtable htValues = new Hashtable();

            htValues.Add("enqueue_time", "'" + DateTime.Now.ToString() + "'");

            if (obj.GetType() == typeof(Int64))
                switch (_target)
                {
                    case QueueBufferFor.USER_INFO:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_userInfo", htValues);
                        break;
                    case QueueBufferFor.USER_RELATION:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_userRelation", htValues);
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
            //更新新的队头值
            if (iCount == 1)
                oFirstValue = obj;
        }

        /// <summary>
        /// 队头User出队
        /// </summary>
        public Object Dequeue()
        {
            //先记录头节点,再删除头节点
            Object oResult = oFirstValue;
            this.Remove(oResult);
            return oResult;
        }

        /// <summary>
        /// 增加指定节点，带有入队时间，所以增加的节点可能在队列的任何位置（注意与Enqueue的区别）
        /// </summary>
        public void Add(Object obj, string enqueue_time)
        {
            Database db = DatabaseFactory.CreateDatabase();
            Hashtable htValues = new Hashtable();

            htValues.Add("enqueue_time", "'" + enqueue_time + "'");
            if (obj.GetType() == typeof(Int64))
                switch (_target)
                {
                    case QueueBufferFor.USER_INFO:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_userInfo", htValues);
                        break;
                    case QueueBufferFor.USER_RELATION:
                        htValues.Add("user_id", obj.ToString());
                        db.Insert("queue_buffer_for_userRelation", htValues);
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
            //更新新的队头值
            if (iCount == 1)
                oFirstValue = obj;
        }

        /// <summary>
        /// 删除指定节点
        /// </summary>
        public void Remove(Object obj)
        {
            int iRowsDeleted = 0;
            Database db = DatabaseFactory.CreateDatabase();
            if (obj.GetType() == typeof(Int64))
                switch (_target)
                {
                    case QueueBufferFor.USER_INFO:
                        iRowsDeleted=db.CountByExecuteSQL("delete from queue_buffer_for_userInfo where user_id=" + obj.ToString());
                        break;
                    case QueueBufferFor.USER_RELATION:
                        iRowsDeleted = db.CountByExecuteSQL("delete from queue_buffer_for_userRelation where user_id=" + obj.ToString());
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
            if (oFirstValue!=null && oFirstValue.Equals(obj))
                //更新新的队头值
                GetFirstValue();
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            Database db = DatabaseFactory.CreateDatabase();
            switch (_target)
            {
                case QueueBufferFor.USER_INFO:
                    db.CountByExecuteSQL("truncate table queue_buffer_for_userInfo");
                    break;
                case QueueBufferFor.USER_RELATION:
                    db.CountByExecuteSQL("truncate table queue_buffer_for_userRelation");
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
            oFirstValue = null;
        }

        public int Count
        {
            get
            {
                if (iCount % 5000 == 0)    //每增长5000条记录就重新查询一次，以减少数据库查询，提高性能
                {
                    Database db = DatabaseFactory.CreateDatabase();
                    switch (_target)
                    {
                        case QueueBufferFor.USER_INFO:
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from queue_buffer_for_userInfo");
                            break;
                        case QueueBufferFor.USER_RELATION:
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from queue_buffer_for_userRelation");
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

        #endregion  成员方法
    }
}