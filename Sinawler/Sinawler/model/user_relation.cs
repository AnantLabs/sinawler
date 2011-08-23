using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
    /// <summary>
    /// UserRelation状态枚举
    /// </summary>
    public enum RelationState {RelationExists=1,RelationCanceled=0};

	/// <summary>
	/// 类UserRelation。此类中的方法，都是与数据库交互，从中读取或向其中写入数据。
    /// 若“源用户ID”与“目的用户ID”之间有关系，表明“源用户”关注“目的用户”
    /// 鉴于此类的特殊性（不同迭代次数时可能存在同样的关系），规定：除非明确指定，否则获取的模型均为最新的关系
	/// </summary>
    
    //source_user_id: 源用户UserID
    //target_user_id: 目的用户UserID
    //relation_state: 关系状态，1为成立，0为不成立，以此记录关系动态变化，默认为1
    //iteration: 迭代次数。默认为0，每迭代一次，就加1，则为0的为最近的数据

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
		/// 源用户UserID
		/// </summary>
		public long source_user_id
		{
			set{ _source_user_id=value;}
			get{return _source_user_id;}
		}
        /// <summary>
		/// 目的用户UserID
		/// </summary>
		public long target_user_id
		{
			set{ _target_user_id=value;}
			get{return _target_user_id;}
		}
		/// <summary>
		/// 关系状态，1为成立，0为不成立，以此记录关系动态变化，默认为1
		/// </summary>
		public int relation_state
		{
			set{ _relation_state=value;}
			get{return _relation_state;}
		}
        /// <summary>
        /// 迭代次数。默认为0，每迭代一次，就加1，则为0的为最近的数据
        /// </summary>
        public int iteration
        {
            get { return _iteration; }
        }
        /// <summary>
        /// 记录更新时间
        /// </summary>
        public string update_time
        {
            set { _update_time = value; }
            get { return _update_time; }
        }
		#endregion Model
        
		#region  成员方法

		/// <summary>
        /// 根据指定的源、目的Uid、关系状态得到最新关系的一个对象实体
		/// </summary>
		public UserRelation(long lSourceUserID,long lTargetUserID)
		{
            this.GetModel( lSourceUserID,lTargetUserID );
		}

		/// <summary>
		/// 是否存在指定的有效关注关系（最新状态）
		/// </summary>
        static public bool Exists ( long lSourceUserID, long lTargetUserID)
		{
            Database db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect( "select count(*) from user_relation where source_user_id=" + lSourceUserID.ToString()+" and target_user_id="+lTargetUserID.ToString() );
            return count > 0;
		}

        /// <summary>
        /// 更新数据库中已有数据的迭代次数
        /// </summary>
        static public void NewIterate ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            db.CountByExecuteSQL("update user_relation set iteration=iteration+1");
        }

		/// <summary>
		/// 增加一条数据
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
        /// 根据指定的源、目的Uid得到他们最新关系的一个对象实体
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
		/// 获得指定源用户关注的UserID列表
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
        /// 获得关注指定目标用户的UserID列表
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
        /// 获得用户关系表中所有UserID，包括source_user_id和target_user_id
        /// 返回DataTable，以便于调用者观察进度——本不应该将DataTable暴露给上层的，无奈
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

		#endregion  成员方法
	}
}

