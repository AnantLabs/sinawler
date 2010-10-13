using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
	/// <summary>
	/// ��QueueBuffer�����ڴ��еĴ����е�UID���г��ȳ���5000ʱ����ʼʹ�����ݿⱣ����С�
    /// ���ݿ��еĶ������ڴ��еĶ��еĺ��棬����enqueue�ֶ�����
    /// ���಻��ʵ����
    /// ��ͨ�������ṩ�ľ�̬������������в����������Add������Remove������ӡ�ɾ��ָ���ڵ�
	/// </summary>
    public class QueueBuffer : ModelBase
	{
        #region  ��Ա����

		/// <summary>
		/// �Ƿ���ڸü�¼
		/// </summary>
		static public bool Contains(long uid)
		{
            int count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer where uid="+uid.ToString() );
            return count > 0;
		}

        /// <summary>
		/// һ��UID���
		/// </summary>
		static public void Enqueue(long uid)
		{
            Hashtable htValues = new Hashtable();
            htValues.Add( "uid", uid );
            htValues.Add( "enqueue_time", "'" + DateTime.Now.ToString() + "'" );
            db.Insert( "queue_buffer", htValues );
		}

		/// <summary>
		/// ��ͷUID����
		/// </summary>
		static public long Dequeue()
		{
            //�Ȼ�ȡͷ�ڵ�
            DataRow dr = db.GetDataRow( "select top 1 uid from queue_buffer order by enqueue_time" );
            if (dr == null) return 0;
            long lUid = Convert.ToInt64(dr["uid"]);

            //��ɾ��ͷ�ڵ�
            db.CountByExecuteSQL( "delete from enqueue_buffer where uid=" + lUid.ToString() );
            return lUid;
		}

        /// <summary>
        /// ����ָ���ڵ�
        /// </summary>
        static public void Add(long uid, string enqueue_time)
        {
            Hashtable htValues = new Hashtable();
            htValues.Add( "uid", uid );
            htValues.Add( "enqueue_time", "'" + enqueue_time + "'" );
            db.Insert( "queue_buffer", htValues );
        }

        /// <summary>
        /// ɾ��ָ���ڵ�
        /// </summary>
        static public void Remove (long uid)
        {
            db.CountByExecuteSQL( "delete from enqueue_buffer where uid=" + uid.ToString() );
        }

        /// <summary>
        /// �������
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
		#endregion  ��Ա����
	}
}

