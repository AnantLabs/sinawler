using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
    public enum QueueBufferTarget {FOR_USER=0,FOR_STATUS=1,FOR_COMMENT=2};

	/// <summary>
	/// 类QueueBuffer，当内存中的待爬行的UID队列长度超过指定长度时，开始使用数据库保存队列。
    /// 数据库中分别有用于用户机器人和微博机器人的两个队列表，具体操作哪个，由构造函数中的参数指明
    /// 数据库中的队列在内存中的队列的后面，根据enqueue_time字段排序
    /// 此类不可实例化
    /// 可通过此类提供的静态方法做出入队列操作，或调用Add方法、Remove方法添加、删除指定节点
	/// </summary>
    public class QueueBuffer
	{
        private QueueBufferTarget _target=QueueBufferTarget.FOR_USER;
        
        #region  成员方法
        ///构造函数
        ///<param name="target">要操作的目标</param>
        public QueueBuffer(QueueBufferTarget target)
        {
            _target = target;
        }

        /// <summary>
        /// 队头值
        /// </summary>
        public long FirstValue
        {
            get
            {
                Database db = DatabaseFactory.CreateDatabase();
                DataRow dr;
                long lResultID = 0;
                switch (_target)
                {
                    case QueueBufferTarget.FOR_USER:
                        dr = db.GetDataRow( "select top 1 uid from queue_buffer_for_user order by enqueue_time" );
                        if (dr == null) return 0;
                        lResultID = Convert.ToInt64( dr["uid"] );
                        break;
                    case QueueBufferTarget.FOR_STATUS:
                        dr = db.GetDataRow( "select top 1 uid from queue_buffer_for_status order by enqueue_time" );
                        if (dr == null) return 0;
                        lResultID = Convert.ToInt64( dr["uid"] );
                        break;
                    case QueueBufferTarget.FOR_COMMENT:
                        dr = db.GetDataRow( "select top 1 status_id from queue_buffer_for_comment order by enqueue_time" );
                        if (dr == null) return 0;
                        lResultID = Convert.ToInt64( dr["status_id"] );
                        break;
                }
                return lResultID;
            }
        }

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Contains(long id)
		{
            Database db = DatabaseFactory.CreateDatabase();
            int count=0;
            switch(_target)
            {
                case QueueBufferTarget.FOR_USER:
                    count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer_for_user where uid="+id.ToString() );
                    break;
                case QueueBufferTarget.FOR_STATUS:
                    count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer_for_status where uid=" + id.ToString() );
                    break;
                case QueueBufferTarget.FOR_COMMENT:
                    count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer_for_comment where status_id=" + id.ToString() );
                    break;
            }   
            return count > 0;
		}

        /// <summary>
		/// 一个UID入队
		/// </summary>
		public void Enqueue(long id)
		{
            Database db = DatabaseFactory.CreateDatabase();
            Hashtable htValues = new Hashtable();
            htValues.Add( "enqueue_time", "'" + DateTime.Now.ToString() + "'" );
            switch(_target)
            {
                case QueueBufferTarget.FOR_USER:
                    htValues.Add( "uid", id );
                    db.Insert( "queue_buffer_for_user", htValues );
                    break;
                case QueueBufferTarget.FOR_STATUS:
                    htValues.Add( "uid", id );
                    db.Insert( "queue_buffer_for_status", htValues );
                    break;
                case QueueBufferTarget.FOR_COMMENT:
                    htValues.Add( "status_id", id );
                    db.Insert( "queue_buffer_for_comment", htValues );
                    break;
            }
		}

		/// <summary>
		/// 队头UID出队
		/// </summary>
		public long Dequeue()
		{
            //先获取头节点,再删除头节点
            long lResultID = this.FirstValue;
            this.Remove( lResultID );
            return lResultID;
		}

        /// <summary>
        /// 增加指定节点
        /// </summary>
        public void Add(long id, string enqueue_time)
        {
            Database db = DatabaseFactory.CreateDatabase();
            Hashtable htValues = new Hashtable();
            
            htValues.Add( "enqueue_time", "'" + enqueue_time + "'" );
            switch (_target)
            {
                case QueueBufferTarget.FOR_USER:
                    htValues.Add("uid", id);
                    db.Insert("queue_buffer_for_user", htValues);
                    break;
                case QueueBufferTarget.FOR_STATUS:
                    htValues.Add("uid", id);
                    db.Insert("queue_buffer_for_status", htValues);
                    break;
                case QueueBufferTarget.FOR_COMMENT:
                    htValues.Add("status_id", id);
                    db.Insert("queue_buffer_for_comment", htValues);
                    break;
            }
        }

        /// <summary>
        /// 删除指定节点
        /// </summary>
        public void Remove (long id)
        {
            Database db = DatabaseFactory.CreateDatabase();
            switch (_target)
            {
                case QueueBufferTarget.FOR_USER:
                    db.CountByExecuteSQL("delete from queue_buffer_for_user where uid=" + id.ToString());
                    break;
                case QueueBufferTarget.FOR_STATUS:
                    db.CountByExecuteSQL("delete from queue_buffer_for_status where uid=" + id.ToString());
                    break;
                case QueueBufferTarget.FOR_COMMENT:
                    db.CountByExecuteSQL("delete from queue_buffer_for_comment where status_id=" + id.ToString());
                    break;
            }
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            switch (_target)
            {
                case QueueBufferTarget.FOR_USER:
                    db.CountByExecuteSQL("delete from queue_buffer_for_user");
                    break;
                case QueueBufferTarget.FOR_STATUS:
                    db.CountByExecuteSQL("delete from queue_buffer_for_status");
                    break;
                case QueueBufferTarget.FOR_COMMENT:
                    db.CountByExecuteSQL("delete from queue_buffer_for_comment");
                    break;
            }
        }

        public int Count
        { 
            get 
            {
                Database db = DatabaseFactory.CreateDatabase();
                int count=0;
                switch (_target)
                {
                    case QueueBufferTarget.FOR_USER:
                        count = db.CountByExecuteSQLSelect("select count(uid) as cnt from queue_buffer_for_user");
                        break;
                    case QueueBufferTarget.FOR_STATUS:
                        count = db.CountByExecuteSQLSelect("select count(uid) as cnt from queue_buffer_for_status");
                        break;
                    case QueueBufferTarget.FOR_COMMENT:
                        count = db.CountByExecuteSQLSelect("select count(uid) as cnt from queue_buffer_for_comment");
                        break;
                }
                if (count == -1) return 0;
                else return count;
            }
        }

		#endregion  成员方法
	}
}