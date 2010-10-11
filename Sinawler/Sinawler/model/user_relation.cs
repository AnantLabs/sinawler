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
    
    //source_uid: Դ�û�UID
    //target_uid: Ŀ���û�UID
    //relation_state: ��ϵ״̬��1Ϊ������0Ϊ���������Դ˼�¼��ϵ��̬�仯��Ĭ��Ϊ1
    //iteration: ����������Ĭ��Ϊ0��ÿ����һ�Σ��ͼ�1����Ϊ0��Ϊ���������

	public class UserRelation
	{
        static private Database db = DatabaseFactory.CreateDatabase();

        public UserRelation()
		{}

		#region Model
		private long _source_uid;
        private long _target_uid;
        private int _relation_state=1;
        private int _iteration;
        private string _update_time;
		/// <summary>
		/// Դ�û�UID
		/// </summary>
		public long source_uid
		{
			set{ _source_uid=value;}
			get{return _source_uid;}
		}
        /// <summary>
		/// Ŀ���û�UID
		/// </summary>
		public long target_uid
		{
			set{ _target_uid=value;}
			get{return _target_uid;}
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
            set { _iteration = value; }
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
		public UserRelation(long lSourceUID,long lTargetUID)
		{
            this.GetModel( lSourceUID,lTargetUID );
		}

		/// <summary>
		/// �Ƿ����ָ������Ч��ע��ϵ������״̬��
		/// </summary>
        static public bool Exists ( long lSourceUID, long lTargetUID)
		{
            int count = db.CountByExecuteSQLSelect( "select top 1 relation_state from user_relation where source_uid=" + lSourceUID.ToString()+" and target_uid="+lTargetUID.ToString()+" and relation_state=1 order by iteration" );
            return count > 0;
		}

        /// <summary>
        /// �������ݿ����������ݵĵ�������
        /// </summary>
        static public void NewIterate ()
        {
            db.CountByExecuteSQL("update user_relation set iteration=iteration+1");
        }

		/// <summary>
		/// ����һ������
		/// </summary>
		public void Add()
		{
            try
            {
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString( "u" ).Replace( "Z", "" ) + "'";
                htValues.Add( "source_uid", _source_uid );
                htValues.Add( "target_uid", _target_uid );
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
        public bool GetModel ( long lSourceUID, long lTargetUID)
        {
            string strSQL = "select top 1 * from user_relation where source_uid=" + lSourceUID.ToString() + " and target_uid=" + lTargetUID.ToString() + " order by iteration";

            DataRow dr = db.GetDataRow( strSQL );
            if (dr != null)
            {
                _source_uid = Convert.ToInt64( dr["source_uid"] );
                _target_uid = Convert.ToInt64( dr["target_uid"] );
                _relation_state = Convert.ToInt32( dr["relation_state"] );
                _iteration = Convert.ToInt32( dr["iteration"] );
                _update_time = dr["update_time"].ToString();
                return true;
            }
            else
                return false;
        }

		/// <summary>
		/// ���ָ��Դ�û���ע��UID�б�
		/// </summary>
        static public LinkedList<long> GetFollowingUID(long lSourceUID)
        {
            string strSQL = "select target_uid,relation_state from user_relation where source_uid=" + lSourceUID.ToString() + " order by update_time";
            LinkedList<long> lstTargetUID = new LinkedList<long>();
            DataSet ds = db.GetDataSet( strSQL );
            if (ds == null) return lstTargetUID;
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int state=Convert.ToInt32(dt.Rows[i]["relation_state"]);
                if(state==Convert.ToInt32(RelationState.RelationExists))
                    lstTargetUID.AddLast( state );
                else
                    lstTargetUID.Remove( state );
            }
            return lstTargetUID;
        }

        /// <summary>
        /// ��ù�עָ��Ŀ���û���UID�б�
        /// </summary>
        static public LinkedList<long> GetFollowedByUID ( long lTargetUID )
        {
            string strSQL = "select source_uid,relation_state from user_relation where target_uid=" + lTargetUID.ToString() + " order by update_time";
            LinkedList<long> lstSourceUID = new LinkedList<long>();
            DataSet ds = db.GetDataSet( strSQL );
            if (ds == null) return lstSourceUID;
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int state = Convert.ToInt32( dt.Rows[i]["relation_state"] );
                if (state == Convert.ToInt32( RelationState.RelationExists ))
                    lstSourceUID.AddLast( state );
                else
                    lstSourceUID.Remove( state );
            }
            return lstSourceUID;
        }

        /// <summary>
        /// ����û���ϵ��������UID������source_uid��target_uid
        /// ����DataTable���Ա��ڵ����߹۲���ȡ�������Ӧ�ý�DataTable��¶���ϲ�ģ�����
        /// </summary>
        static public DataTable GetAllUIDTable ()
        {
            string strSQL = "select * from all_uid";
            DataSet ds = db.GetDataSet( strSQL );
            if (ds == null) return null;
            else return ds.Tables[0];
        }

		#endregion  ��Ա����
	}
}

