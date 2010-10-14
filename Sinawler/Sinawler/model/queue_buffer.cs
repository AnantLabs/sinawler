using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
    public enum QueueBufferTarget {FOR_USER=1,FOR_STATUS=0};

	/// <summary>
	/// ��QueueBuffer�����ڴ��еĴ����е�UID���г��ȳ���ָ������ʱ����ʼʹ�����ݿⱣ����С�
    /// ���ݿ��зֱ��������û������˺�΢�������˵��������б���������ĸ����ɹ��캯���еĲ���ָ��
    /// ���ݿ��еĶ������ڴ��еĶ��еĺ��棬����enqueue_time�ֶ�����
    /// ���಻��ʵ����
    /// ��ͨ�������ṩ�ľ�̬������������в����������Add������Remove������ӡ�ɾ��ָ���ڵ�
	/// </summary>
    public class QueueBuffer : ModelBase
	{
        private QueueBufferTarget _target=QueueBufferTarget.FOR_USER;

        #region  ��Ա����
        ///���캯��
        ///<param name="target">Ҫ������Ŀ��</param>
        public QueueBuffer(QueueBufferTarget target)
        {
            _target = target;
            db = DatabaseFactory.CreateDatabase();
        }

		/// <summary>
		/// �Ƿ���ڸü�¼
		/// </summary>
		public bool Contains(long uid)
		{
            int count;
            if (_target == QueueBufferTarget.FOR_USER)
                count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer_for_user where uid="+uid.ToString() );
            else
                count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer_for_status where uid=" + uid.ToString() );
            return count > 0;
		}

        /// <summary>
		/// һ��UID���
		/// </summary>
		public void Enqueue(long uid)
		{
            Hashtable htValues = new Hashtable();
            htValues.Add( "uid", uid );
            htValues.Add( "enqueue_time", "'" + DateTime.Now.ToString() + "'" );
            if (_target == QueueBufferTarget.FOR_USER)
                db.Insert( "queue_buffer_for_user", htValues );
            else
                db.Insert( "queue_buffer_for_status", htValues );
		}

		/// <summary>
		/// ��ͷUID����
		/// </summary>
		public long Dequeue()
		{
            //�Ȼ�ȡͷ�ڵ�
            DataRow dr;
            if (_target == QueueBufferTarget.FOR_USER)
                dr = db.GetDataRow( "select top 1 uid from queue_buffer_for_user order by enqueue_time" );
            else
                dr = db.GetDataRow( "select top 1 uid from queue_buffer_for_status order by enqueue_time" );
            if (dr == null) return 0;
            long lUid = Convert.ToInt64(dr["uid"]);

            //��ɾ��ͷ�ڵ�
            if (_target == QueueBufferTarget.FOR_USER)
                db.CountByExecuteSQL( "delete from queue_buffer_for_user where uid=" + lUid.ToString() );
            else
                db.CountByExecuteSQL( "delete from queue_buffer_for_status where uid=" + lUid.ToString() );
            return lUid;
		}

        /// <summary>
        /// ����ָ���ڵ�
        /// </summary>
        public void Add(long uid, string enqueue_time)
        {
            Hashtable htValues = new Hashtable();
            htValues.Add( "uid", uid );
            htValues.Add( "enqueue_time", "'" + enqueue_time + "'" );
            if (_target == QueueBufferTarget.FOR_USER)
                db.Insert( "queue_buffer_for_user", htValues );
            else
                db.Insert( "queue_buffer_for_status", htValues );
        }

        /// <summary>
        /// ɾ��ָ���ڵ�
        /// </summary>
        public void Remove (long uid)
        {
            if (_target == QueueBufferTarget.FOR_USER)
                db.CountByExecuteSQL( "delete from queue_buffer_for_user where uid=" + uid.ToString() );
            else
                db.CountByExecuteSQL( "delete from queue_buffer_for_status where uid=" + uid.ToString() );
        }

        /// <summary>
        /// �������
        /// </summary>
        public void Clear ()
        {
            if (_target == QueueBufferTarget.FOR_USER)
                db.CountByExecuteSQL( "delete from queue_buffer_for_user" );
            else
                db.CountByExecuteSQL( "delete from queue_buffer_for_status" );
        }

        public int Count
        { 
            get 
            {
                int count=0;
                if (_target == QueueBufferTarget.FOR_USER)
                    count = db.CountByExecuteSQLSelect( "select count(uid) as cnt from queue_buffer_for_user" );
                else
                    count = db.CountByExecuteSQLSelect( "select count(uid) as cnt from queue_buffer_for_status" );
                if (count == -1) return 0;
                else return count;
            }
        }
		#endregion  ��Ա����
	}
}