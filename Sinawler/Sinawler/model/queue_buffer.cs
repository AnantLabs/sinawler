using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
	/// <summary>
	/// 类QueueBuffer，当内存中的待爬行的UID队列长度超过5000时，开始使用数据库保存队列。
    /// 数据库中的队列在内存中的队列的后面，根据enqueue字段排序
    /// 此类不可实例化
    /// 可通过此类提供的静态方法做出入队列操作，或调用Add方法、Remove方法添加、删除指定节点
	/// </summary>
    public class QueueBuffer : ModelBase
	{
        #region  成员方法

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		static public bool Contains(long uid)
		{
            int count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer where uid="+uid.ToString() );
            return count > 0;
		}

        /// <summary>
		/// 一个UID入队
		/// </summary>
		static public void Enqueue(long uid)
		{
            Hashtable htValues = new Hashtable();
            htValues.Add( "uid", uid );
            htValues.Add( "enqueue_time", "'" + DateTime.Now.ToString() + "'" );
            db.Insert( "queue_buffer", htValues );
		}

		/// <summary>
		/// 队头UID出队
		/// </summary>
		static public long Dequeue()
		{
            //先获取头节点
            DataRow dr = db.GetDataRow( "select top 1 uid from queue_buffer order by enqueue_time" );
            if (dr == null) return 0;
            long lUid = Convert.ToInt64(dr["uid"]);

            //再删除头节点
            db.CountByExecuteSQL( "delete from enqueue_buffer where uid=" + lUid.ToString() );
            return lUid;
		}

        /// <summary>
        /// 增加指定节点
        /// </summary>
        static public void Add(long uid, string enqueue_time)
        {
            Hashtable htValues = new Hashtable();
            htValues.Add( "uid", uid );
            htValues.Add( "enqueue_time", "'" + enqueue_time + "'" );
            db.Insert( "queue_buffer", htValues );
        }

        /// <summary>
        /// 删除指定节点
        /// </summary>
        static public void Remove (long uid)
        {
            db.CountByExecuteSQL( "delete from enqueue_buffer where uid=" + uid.ToString() );
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        static public void Clear ()
        {
            db.CountByExecuteSQL( "delete from queue_buffer" );
        }

        static public int Count
        { 
            get 
            {
                DataRow dr = db.GetDataRow( "select count(uid) as cnt from queue_buffer" );
                if (dr == null) return 0;
                else return Convert.ToInt32(dr["cnt"]);
            }
        }
		#endregion  成员方法
	}
}

