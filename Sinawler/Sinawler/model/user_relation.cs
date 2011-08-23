using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
    /// <summary>
    /// UserRelation״̬ö��
    /// </summary>
    public enum RelationState {RelationExists=1,RelationCanceled=0};

	/// <summary>
	/// ��UserRelation�������еķ��������������ݿ⽻�������ж�ȡ��������д�����ݡ�
    /// ����Դ�û�ID���롰Ŀ���û�ID��֮���й�ϵ��������Դ�û�����ע��Ŀ���û���
    /// ���ڴ���������ԣ���ͬ��������ʱ���ܴ���ͬ���Ĺ�ϵ�����涨��������ȷָ���������ȡ��ģ�;�Ϊ���µĹ�ϵ
	/// </summary>
    
    //source_user_id: Դ�û�UserID
    //target_user_id: Ŀ���û�UserID
    //relation_state: ��ϵ״̬��1Ϊ������0Ϊ���������Դ˼�¼��ϵ��̬�仯��Ĭ��Ϊ1
    //iteration: ����������Ĭ��Ϊ0��ÿ����һ�Σ��ͼ�1����Ϊ0��Ϊ���������

	public class UserRelation
	{
        public UserRelation()
        { }

		#region Model
		private long _source_user_id;
        private long _target_user_id;
        private int _relation_state=1;
        private int _iteration;
        private string _update_time;
		/// <summary>
		/// Դ�û�UserID
		/// </summary>
		public long source_user_id
		{
			set{ _source_user_id=value;}
			get{return _source_user_id;}
		}
        /// <summary>
		/// Ŀ���û�UserID
		/// </summary>
		public long target_user_id
		{
			set{ _target_user_id=value;}
			get{return _target_user_id;}
		}
		/// <summary>
		/// ��ϵ״̬��1Ϊ������0Ϊ���������Դ˼�¼��ϵ��̬�仯��Ĭ��Ϊ1
		/// </summary>
		public int relation_state
		{
			set{ _relation_state=value;}
			get{return _relation_state;}
		}
        /// <summary>
        /// ����������Ĭ��Ϊ0��ÿ����һ�Σ��ͼ�1����Ϊ0��Ϊ���������
        /// </summary>
        public int iteration
        {
            get { return _iteration; }
        }
        /// <summary>
        /// ��¼����ʱ��
        /// </summary>
        public string update_time
        {
            set { _update_time = value; }
            get { return _update_time; }
        }
		#endregion Model
        
		#region  ��Ա����

		/// <summary>
        /// ����ָ����Դ��Ŀ��Uid����ϵ״̬�õ����¹�ϵ��һ������ʵ��
		/// </summary>
		public UserRelation(long lSourceUserID,long lTargetUserID)
		{
            this.GetModel( lSourceUserID,lTargetUserID );
		}

		/// <summary>
		/// �Ƿ����ָ������Ч��ע��ϵ������״̬��
		/// </summary>
        static public bool Exists ( long lSourceUserID, long lTargetUserID)
		{
            Database db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect( "select count(*) from user_relation where source_user_id=" + lSourceUserID.ToString()+" and target_user_id="+lTargetUserID.ToString() );
            return count > 0;
		}

        /// <summary>
        /// �������ݿ����������ݵĵ�������
        /// </summary>
        static public void NewIterate ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            db.CountByExecuteSQL("update user_relation set iteration=iteration+1");
        }

		/// <summary>
		/// ����һ������
		/// </summary>
		public void Add()
		{
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString( "u" ).Replace( "Z", "" ) + "'";
                htValues.Add( "source_user_id", _source_user_id );
                htValues.Add( "target_user_id", _target_user_id );
                htValues.Add( "relation_state", _relation_state );
                htValues.Add( "iteration", 0 );
                htValues.Add( "update_time", _update_time );

                db.Insert( "user_relation", htValues );
            }
            catch
            { return; }
		}

        /// <summary>
        /// ����ָ����Դ��Ŀ��Uid�õ��������¹�ϵ��һ������ʵ��
        /// </summary>
        public bool GetModel ( long lSourceUserID, long lTargetUserID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "select top 1 * from user_relation where source_user_id=" + lSourceUserID.ToString() + " and target_user_id=" + lTargetUserID.ToString() + " order by iteration";

            DataRow dr = db.GetDataRow( strSQL );
            if (dr != null)
            {
                _source_user_id = Convert.ToInt64( dr["source_user_id"] );
                _target_user_id = Convert.ToInt64( dr["target_user_id"] );
                _relation_state = Convert.ToInt32( dr["relation_state"] );
                _iteration = Convert.ToInt32( dr["iteration"] );
                _update_time = dr["update_time"].ToString();
                return true;
            }
            else
                return false;
        }

		/// <summary>
		/// ���ָ��Դ�û���ע��UserID�б�
		/// </summary>
        public static LinkedList<long> GetFollowingUserID(long lSourceUserID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "select target_user_id,relation_state from user_relation where source_user_id=" + lSourceUserID.ToString() + " order by update_time";
            LinkedList<long> lstTargetUserID = new LinkedList<long>();
            DataSet ds = db.GetDataSet( strSQL );
            if (ds == null) return lstTargetUserID;
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int state=Convert.ToInt32(dt.Rows[i]["relation_state"]);
                if(state==Convert.ToInt32(RelationState.RelationExists))
                    lstTargetUserID.AddLast( state );
                else
                    lstTargetUserID.Remove( state );
            }
            return lstTargetUserID;
        }

        /// <summary>
        /// ��ù�עָ��Ŀ���û���UserID�б�
        /// </summary>
        public static LinkedList<long> GetFollowedByUserID(long lTargetUserID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "select source_user_id,relation_state from user_relation where target_user_id=" + lTargetUserID.ToString() + " order by update_time";
            LinkedList<long> lstSourceUserID = new LinkedList<long>();
            DataSet ds = db.GetDataSet( strSQL );
            if (ds == null) return lstSourceUserID;
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int state = Convert.ToInt32( dt.Rows[i]["relation_state"] );
                if (state == Convert.ToInt32( RelationState.RelationExists ))
                    lstSourceUserID.AddLast( state );
                else
                    lstSourceUserID.Remove( state );
            }
            return lstSourceUserID;
        }

        /// <summary>
        /// ����û���ϵ��������UserID������source_user_id��target_user_id
        /// ����DataTable���Ա��ڵ����߹۲���ȡ�������Ӧ�ý�DataTable��¶���ϲ�ģ�����
        /// </summary>
        public static DataTable GetAllUserIDTable()
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "select user_id from all_user_id order by update_time";
            DataSet ds = db.GetDataSet( strSQL );
            if (ds == null) return null;
            else return ds.Tables[0];
        }

        /// <summary>
        /// remove data of specific user
        /// </summary>
        public static bool Remove(long lUID)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                Hashtable ht=new Hashtable();
                ht.Add("relation_state",0);
                return db.Update("user_relation",ht,"source_user_id=" + lUID.ToString()+" or target_user_id="+lUID.ToString());
                //if (db.CountByExecuteSQL("delete from user_relation where source_user_id=" + lUID.ToString()+" or target_user_id="+lUID.ToString()) == 0) return true;
                //else return false;
            }
            catch
            { return false; }
        }

		#endregion  ��Ա����
	}
}

