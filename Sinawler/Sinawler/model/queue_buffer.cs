using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
    public enum QueueBufferFor { USER_INFO = 0, USER_RELATION = 1, USER_TAG = 2, STATUS = 3, COMMENT = 4 };

    /// <summary>
    /// ��QueueBuffer�����ڴ��еĴ����е�UserID���г��ȳ���ָ������ʱ����ʼʹ�����ݿⱣ����С�
    /// ���ݿ��зֱ��������û������˺�΢�������˵��������б���������ĸ����ɹ��캯���еĲ���ָ��
    /// ���ݿ��еĶ������ڴ��еĶ��еĺ��棬����enqueue_time�ֶ�����
    /// ���಻��ʵ����
    /// ��ͨ�������ṩ�ľ�̬������������в����������Add������Remove������ӡ�ɾ��ָ���ڵ�
    /// </summary>
    public class QueueBuffer
    {
        private QueueBufferFor _target = QueueBufferFor.USER_INFO;
        private int iCount = 0;     //���г���
        private long lFirstValue = 0;   //�׽ڵ�ֵ

        public long FirstValue
        {
            get { return lFirstValue; }
        }

        #region  ��Ա����
        ///���캯��
        ///<param name="target">Ҫ������Ŀ��</param>
        public QueueBuffer ( QueueBufferFor target )
        {
            _target = target;
        }

        /// <summary>
        /// �����ݿ��ж�ȡ��ͷֵ
        /// </summary>
        private void GetFirstValue ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DataRow dr;
            switch (_target)
            {
                case QueueBufferFor.USER_INFO:
                    dr = db.GetDataRow( "select top 1 user_id from queue_buffer_for_userInfo order by enqueue_time" );
                    if (dr == null) return;
                    lFirstValue = Convert.ToInt64( dr["user_id"] );
                    break;
                case QueueBufferFor.USER_RELATION:
                    dr = db.GetDataRow( "select top 1 user_id from queue_buffer_for_userRelation order by enqueue_time" );
                    if (dr == null) return;
                    lFirstValue = Convert.ToInt64( dr["user_id"] );
                    break;
                case QueueBufferFor.USER_TAG:
                    dr = db.GetDataRow( "select top 1 user_id from queue_buffer_for_tag order by enqueue_time" );
                    if (dr == null) return;
                    lFirstValue = Convert.ToInt64( dr["user_id"] );
                    break;
                case QueueBufferFor.STATUS:
                    dr = db.GetDataRow( "select top 1 user_id from queue_buffer_for_status order by enqueue_time" );
                    if (dr == null) return;
                    lFirstValue = Convert.ToInt64( dr["user_id"] );
                    break;
                case QueueBufferFor.COMMENT:
                    dr = db.GetDataRow( "select top 1 status_id from queue_buffer_for_comment order by enqueue_time" );
                    if (dr == null) return;
                    lFirstValue = Convert.ToInt64( dr["status_id"] );
                    break;
            }
        }

        /// <summary>
        /// �Ƿ���ڸü�¼
        /// </summary>
        public bool Contains ( long id )
        {
            Database db = DatabaseFactory.CreateDatabase();
            int count = 0;
            switch (_target)
            {
                case QueueBufferFor.USER_INFO:
                    count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer_for_userInfo where user_id=" + id.ToString() );
                    break;
                case QueueBufferFor.USER_RELATION:
                    count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer_for_userRelation where user_id=" + id.ToString() );
                    break;
                case QueueBufferFor.USER_TAG:
                    count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer_for_tag where user_id=" + id.ToString() );
                    break;
                case QueueBufferFor.STATUS:
                    count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer_for_status where user_id=" + id.ToString() );
                    break;
                case QueueBufferFor.COMMENT:
                    count = db.CountByExecuteSQLSelect( "select count(1) from queue_buffer_for_comment where status_id=" + id.ToString() );
                    break;
            }
            return count > 0;
        }

        /// <summary>
        /// һ��ID��ӣ�ע����Add������FirstValue��������
        /// </summary>
        public void Enqueue ( long id )
        {
            Database db = DatabaseFactory.CreateDatabase();
            Hashtable htValues = new Hashtable();

            htValues.Add( "enqueue_time", "'" + DateTime.Now.ToString() + "'" );
            switch (_target)
            {
                case QueueBufferFor.USER_INFO:
                    htValues.Add( "user_id", id );
                    db.Insert( "queue_buffer_for_userInfo", htValues );
                    break;
                case QueueBufferFor.USER_RELATION:
                    htValues.Add( "user_id", id );
                    db.Insert( "queue_buffer_for_userRelation", htValues );
                    break;
                case QueueBufferFor.USER_TAG:
                    htValues.Add( "user_id", id );
                    db.Insert( "queue_buffer_for_tag", htValues );
                    break;
                case QueueBufferFor.STATUS:
                    htValues.Add( "user_id", id );
                    db.Insert( "queue_buffer_for_status", htValues );
                    break;
                case QueueBufferFor.COMMENT:
                    htValues.Add( "status_id", id );
                    db.Insert( "queue_buffer_for_comment", htValues );
                    break;
            }
            iCount++;
            //�����µĶ�ͷֵ
            if (iCount == 1)
                lFirstValue = id;
        }

        /// <summary>
        /// ��ͷUserID����
        /// </summary>
        public long Dequeue ()
        {
            //�ȼ�¼ͷ�ڵ�,��ɾ��ͷ�ڵ�
            long lResultID = lFirstValue;
            this.Remove( lResultID );
            return lResultID;
        }

        /// <summary>
        /// ����ָ���ڵ㣬�������ʱ�䣬�������ӵĽڵ�����ڶ��е��κ�λ�ã�ע����Enqueue������
        /// </summary>
        public void Add ( long id, string enqueue_time )
        {
            Database db = DatabaseFactory.CreateDatabase();
            Hashtable htValues = new Hashtable();

            htValues.Add( "enqueue_time", "'" + enqueue_time + "'" );
            switch (_target)
            {
                case QueueBufferFor.USER_INFO:
                    htValues.Add( "user_id", id );
                    db.Insert( "queue_buffer_for_userInfo", htValues );
                    break;
                case QueueBufferFor.USER_RELATION:
                    htValues.Add( "user_id", id );
                    db.Insert( "queue_buffer_for_userRelation", htValues );
                    break;
                case QueueBufferFor.USER_TAG:
                    htValues.Add( "user_id", id );
                    db.Insert( "queue_buffer_for_tag", htValues );
                    break;
                case QueueBufferFor.STATUS:
                    htValues.Add( "user_id", id );
                    db.Insert( "queue_buffer_for_status", htValues );
                    break;
                case QueueBufferFor.COMMENT:
                    htValues.Add( "status_id", id );
                    db.Insert( "queue_buffer_for_comment", htValues );
                    break;
            }
            iCount++;
            //�����µĶ�ͷֵ
            if (iCount == 1)
                lFirstValue = id;
            else
                GetFirstValue();
        }

        /// <summary>
        /// ɾ��ָ���ڵ�
        /// </summary>
        public void Remove ( long id )
        {
            Database db = DatabaseFactory.CreateDatabase();
            switch (_target)
            {
                case QueueBufferFor.USER_INFO:
                    db.CountByExecuteSQL( "delete from queue_buffer_for_userInfo where user_id=" + id.ToString() );
                    break;
                case QueueBufferFor.USER_RELATION:
                    db.CountByExecuteSQL( "delete from queue_buffer_for_userRelation where user_id=" + id.ToString() );
                    break;
                case QueueBufferFor.USER_TAG:
                    db.CountByExecuteSQL( "delete from queue_buffer_for_tag where user_id=" + id.ToString() );
                    break;
                case QueueBufferFor.STATUS:
                    db.CountByExecuteSQL( "delete from queue_buffer_for_status where user_id=" + id.ToString() );
                    break;
                case QueueBufferFor.COMMENT:
                    db.CountByExecuteSQL( "delete from queue_buffer_for_comment where status_id=" + id.ToString() );
                    break;
            }
            iCount--;
            if(lFirstValue==id)
                //�����µĶ�ͷֵ
                GetFirstValue();
        }

        /// <summary>
        /// �������
        /// </summary>
        public void Clear ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            switch (_target)
            {
                case QueueBufferFor.USER_INFO:
                    db.CountByExecuteSQL( "delete from queue_buffer_for_userInfo" );
                    break;
                case QueueBufferFor.USER_RELATION:
                    db.CountByExecuteSQL( "delete from queue_buffer_for_userRelation" );
                    break;
                case QueueBufferFor.USER_TAG:
                    db.CountByExecuteSQL( "delete from queue_buffer_for_tag" );
                    break;
                case QueueBufferFor.STATUS:
                    db.CountByExecuteSQL( "delete from queue_buffer_for_status" );
                    break;
                case QueueBufferFor.COMMENT:
                    db.CountByExecuteSQL( "delete from queue_buffer_for_comment" );
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
                        case QueueBufferFor.USER_INFO:
                            iCount = db.CountByExecuteSQLSelect( "select count(user_id) as cnt from queue_buffer_for_userInfo" );
                            break;
                        case QueueBufferFor.USER_RELATION:
                            iCount = db.CountByExecuteSQLSelect( "select count(user_id) as cnt from queue_buffer_for_userRelation" );
                            break;
                        case QueueBufferFor.USER_TAG:
                            iCount = db.CountByExecuteSQLSelect( "select count(user_id) as cnt from queue_buffer_for_tag" );
                            break;
                        case QueueBufferFor.STATUS:
                            iCount = db.CountByExecuteSQLSelect( "select count(user_id) as cnt from queue_buffer_for_status" );
                            break;
                        case QueueBufferFor.COMMENT:
                            iCount = db.CountByExecuteSQLSelect( "select count(status_id) as cnt from queue_buffer_for_comment" );
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