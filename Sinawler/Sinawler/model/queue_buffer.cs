using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
    public enum QueueBufferTarget {FOR_USER=0,FOR_STATUS=1,FOR_COMMENT=2};

	/// <summary>
	/// ��QueueBuffer�����ڴ��еĴ����е�UID���г��ȳ���ָ������ʱ����ʼʹ�����ݿⱣ����С�
    /// ���ݿ��зֱ��������û������˺�΢�������˵��������б���������ĸ����ɹ��캯���еĲ���ָ��
    /// ���ݿ��еĶ������ڴ��еĶ��еĺ��棬����enqueue_time�ֶ�����
    /// ���಻��ʵ����
    /// ��ͨ�������ṩ�ľ�̬������������в����������Add������Remove������ӡ�ɾ��ָ���ڵ�
	/// </summary>
    public class QueueBuffer
	{
        private QueueBufferTarget _target=QueueBufferTarget.FOR_USER;
        
        #region  ��Ա����
        ///���캯��
        ///<param name="target">Ҫ������Ŀ��</param>
        public QueueBuffer(QueueBufferTarget target)
        {
            _target = target;
        }

		/// <summary>
		/// �Ƿ���ڸü�¼
		/// </summary>
		public bool Contains(long uid)
		{
            Database db = DatabaseFactory.CreateDatabase();
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
            Database db = DatabaseFactory.CreateDatabase();
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
            Database db = DatabaseFactory.CreateDatabase();
            //�Ȼ�ȡͷ�ڵ�,��ɾ��ͷ�ڵ�
            DataRow dr;
            long lResultID=0;
            switch (_target)
            {
                case QueueBufferTarget.FOR_USER:
                    dr = db.GetDataRow( "select top 1 uid from queue_buffer_for_user order by enqueue_time" );
                    if (dr == null) return 0;
                    lResultID = Convert.ToInt64(dr["uid"]);
                    db.CountByExecuteSQL("delete from queue_buffer_for_user where uid=" + lResultID.ToString());
                    break;
                case QueueBufferTarget.FOR_STATUS:
                    dr = db.GetDataRow( "select top 1 uid from queue_buffer_for_status order by enqueue_time" );
                    if (dr == null) return 0;
                    lResultID = Convert.ToInt64(dr["uid"]);
                    db.CountByExecuteSQL("delete from queue_buffer_for_status where uid=" + lResultID.ToString());
                    break;
                case QueueBufferTarget.FOR_COMMENT:
                    dr = db.GetDataRow( "select top 1 status_id from queue_buffer_for_comment order by enqueue_time" );
                    if (dr == null) return 0;
                    lResultID = Convert.ToInt64(dr["status_id"]);
                    db.CountByExecuteSQL("delete from queue_buffer_for_comment where status_id=" + lResultID.ToString());
                    break;
            }
            return lResultID;
		}

        /// <summary>
        /// ����ָ���ڵ�
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
        /// ɾ��ָ���ڵ�
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
        /// �������
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

		#endregion  ��Ա����
	}
}