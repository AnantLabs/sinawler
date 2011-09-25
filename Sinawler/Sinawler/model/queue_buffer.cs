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
        private int iCount = 0;
        
        
        #region  ��Ա����
        ///���캯��
        ///<param name="target">Ҫ������Ŀ��</param>
        public QueueBuffer(QueueBufferFor target)
        {
            _target = target;
        }

        /// <summary>
        /// �����ݿ��ж�ȡ��ͷָ��������ֵ������������ʽ���أ���ֱ��������ָ�������
        /// </summary>
        public LinkedList<long> GetFirstValues(int iCount)
        {
            LinkedList<long> lstResult = new LinkedList<long>();
            Database db = DatabaseFactory.CreateDatabase();
            DataSet ds = null;
            switch (_target)
            {
                case QueueBufferFor.USER_RELATION:
                    ds = db.GetDataSet("select top " + iCount.ToString() + " user_id from "+GlobalPool.UserRelationBufferTable+" order by enqueue_time");
                    break;
                case QueueBufferFor.USER_INFO:
                    ds = db.GetDataSet("select top " + iCount.ToString() + " user_id from " + GlobalPool.UserInfoBufferTable + " order by enqueue_time");
                    break;
                case QueueBufferFor.USER_TAG:
                    ds = db.GetDataSet("select top " + iCount.ToString() + " user_id from " + GlobalPool.UserTagBufferTable + " order by enqueue_time");
                    break;
                case QueueBufferFor.STATUS:
                    ds = db.GetDataSet("select top " + iCount.ToString() + " user_id from " + GlobalPool.StatusBufferTable + " order by enqueue_time");
                    break;
                case QueueBufferFor.COMMENT:
                    ds = db.GetDataSet("select top " + iCount.ToString() + " status_id from " + GlobalPool.CommentBufferTable + " order by enqueue_time");
                    break;
            }
            if (ds != null)
            {
                string strIDsToBeDeleted = "(";
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
                        db.CountByExecuteSQL("delete from " + GlobalPool.UserRelationBufferTable + " where user_id in " + strIDsToBeDeleted);
                        break;
                    case QueueBufferFor.USER_INFO:
                        db.CountByExecuteSQL("delete from " + GlobalPool.UserInfoBufferTable + " where user_id in " + strIDsToBeDeleted);
                        break;
                    case QueueBufferFor.USER_TAG:
                        db.CountByExecuteSQL("delete from " + GlobalPool.UserTagBufferTable + " where user_id in " + strIDsToBeDeleted);
                        break;
                    case QueueBufferFor.STATUS:
                        db.CountByExecuteSQL("delete from " + GlobalPool.StatusBufferTable + " where user_id in " + strIDsToBeDeleted);
                        break;
                    case QueueBufferFor.COMMENT:
                        db.CountByExecuteSQL("delete from " + GlobalPool.CommentBufferTable + " where status_id in " + strIDsToBeDeleted);
                        break;
                }
            }
            return lstResult;
        }

        /// <summary>
        /// �Ƿ���ڸü�¼
        /// </summary>
        public bool Contains(long lID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            int count = 0;
            switch (_target)
            {
                case QueueBufferFor.USER_RELATION:
                    count = db.CountByExecuteSQLSelect("select count(1) from " + GlobalPool.UserRelationBufferTable + " where user_id=" + lID.ToString());
                    break;
                case QueueBufferFor.USER_INFO:
                    count = db.CountByExecuteSQLSelect("select count(1) from " + GlobalPool.UserInfoBufferTable + " where user_id=" + lID.ToString());
                    break;
                case QueueBufferFor.USER_TAG:
                    count = db.CountByExecuteSQLSelect("select count(1) from " + GlobalPool.UserTagBufferTable + " where user_id=" + lID.ToString());
                    break;
                case QueueBufferFor.STATUS:
                    count = db.CountByExecuteSQLSelect("select count(1) from " + GlobalPool.StatusBufferTable + " where user_id=" + lID.ToString());
                    break;
                case QueueBufferFor.COMMENT:
                    count = db.CountByExecuteSQLSelect("select count(1) from " + GlobalPool.CommentBufferTable + " where status_id=" + lID.ToString());
                    break;
                default:
                    return false;
            }
            return count > 0;
        }

        /// <summary>
        /// һ��ID���
        /// </summary>
        public void Enqueue(long lID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            Hashtable htValues = new Hashtable();

            htValues.Add("enqueue_time", "'" + DateTime.Now.ToString() + "'");

            switch (_target)
            {
                case QueueBufferFor.USER_RELATION:
                    htValues.Add("user_id", lID);
                    db.Insert(GlobalPool.UserRelationBufferTable, htValues);
                    break;
                case QueueBufferFor.USER_INFO:
                    htValues.Add("user_id", lID);
                    db.Insert(GlobalPool.UserInfoBufferTable, htValues);
                    break;
                case QueueBufferFor.USER_TAG:
                    htValues.Add("user_id", lID);
                    db.Insert(GlobalPool.UserTagBufferTable, htValues);
                    break;
                case QueueBufferFor.STATUS:
                    htValues.Add("user_id", lID);
                    db.Insert(GlobalPool.StatusBufferTable, htValues);
                    break;
                case QueueBufferFor.COMMENT:
                    htValues.Add("status_id", lID);
                    db.Insert(GlobalPool.CommentBufferTable, htValues);
                    break;
                default:
                    return;
            }

            iCount++;
        }

        /// <summary>
        /// ����ָ���ڵ㣬�������ʱ�䣬�������ӵĽڵ�����ڶ��е��κ�λ�ã�ע����Enqueue������
        /// </summary>
        public void Add(long lID, string enqueue_time)
        {
            Database db = DatabaseFactory.CreateDatabase();
            Hashtable htValues = new Hashtable();

            htValues.Add("enqueue_time", "'" + enqueue_time + "'");
            switch (_target)
            {
                case QueueBufferFor.USER_RELATION:
                    htValues.Add("user_id", lID);
                    db.Insert(GlobalPool.UserRelationBufferTable, htValues);
                    break;
                case QueueBufferFor.USER_INFO:
                    htValues.Add("user_id", lID);
                    db.Insert(GlobalPool.UserInfoBufferTable, htValues);
                    break;
                case QueueBufferFor.USER_TAG:
                    htValues.Add("user_id", lID);
                    db.Insert(GlobalPool.UserTagBufferTable, htValues);
                    break;
                case QueueBufferFor.STATUS:
                    htValues.Add("user_id", lID);
                    db.Insert(GlobalPool.StatusBufferTable, htValues);
                    break;
                case QueueBufferFor.COMMENT:
                    htValues.Add("status_id", lID);
                    db.Insert(GlobalPool.CommentBufferTable, htValues);
                    break;
                default:
                    return;
            }

            iCount++;
        }

        /// <summary>
        /// ɾ��ָ���ڵ�
        /// </summary>
        public void Remove(long lID)
        {
            int iRowsDeleted = 0;
            Database db = DatabaseFactory.CreateDatabase();

            switch (_target)
            {
                case QueueBufferFor.USER_RELATION:
                    iRowsDeleted = db.CountByExecuteSQL("delete from " + GlobalPool.UserRelationBufferTable + " where user_id=" + lID.ToString());
                    break;
                case QueueBufferFor.USER_INFO:
                    iRowsDeleted = db.CountByExecuteSQL("delete from " + GlobalPool.UserInfoBufferTable + " where user_id=" + lID.ToString());
                    break;
                case QueueBufferFor.USER_TAG:
                    iRowsDeleted = db.CountByExecuteSQL("delete from " + GlobalPool.UserTagBufferTable + " where user_id=" + lID.ToString());
                    break;
                case QueueBufferFor.STATUS:
                    iRowsDeleted = db.CountByExecuteSQL("delete from " + GlobalPool.StatusBufferTable + " where user_id=" + lID.ToString());
                    break;
                case QueueBufferFor.COMMENT:
                    iRowsDeleted = db.CountByExecuteSQL("delete from " + GlobalPool.CommentBufferTable + " where status_id=" + lID.ToString());
                    break;
                default:
                    return;
            }

            if (iRowsDeleted > 0) iCount = iCount - iRowsDeleted;
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
                    db.CountByExecuteSQL("drop table " + GlobalPool.UserRelationBufferTable);
                    break;
                case QueueBufferFor.USER_INFO:
                    db.CountByExecuteSQL("drop table " + GlobalPool.UserInfoBufferTable);
                    break;
                case QueueBufferFor.USER_TAG:
                    db.CountByExecuteSQL("drop table " + GlobalPool.UserTagBufferTable);
                    break;
                case QueueBufferFor.STATUS:
                    db.CountByExecuteSQL("drop table " + GlobalPool.StatusBufferTable);
                    break;
                case QueueBufferFor.COMMENT:
                    db.CountByExecuteSQL("drop table " + GlobalPool.CommentBufferTable);
                    break;
                default:
                    return;
            }
            iCount = 0;
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
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from " + GlobalPool.UserRelationBufferTable);
                            break;
                        case QueueBufferFor.USER_INFO:
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from " + GlobalPool.UserInfoBufferTable);
                            break;
                        case QueueBufferFor.USER_TAG:
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from " + GlobalPool.UserTagBufferTable);
                            break;
                        case QueueBufferFor.STATUS:
                            iCount = db.CountByExecuteSQLSelect("select count(user_id) as cnt from " + GlobalPool.StatusBufferTable);
                            break;
                        case QueueBufferFor.COMMENT:
                            iCount = db.CountByExecuteSQLSelect("select count(status_id) as cnt from " + GlobalPool.CommentBufferTable);
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