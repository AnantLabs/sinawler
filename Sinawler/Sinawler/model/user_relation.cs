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
    
    //source_uid: 源用户UID
    //target_uid: 目的用户UID
    //relation_state: 关系状态，1为成立，0为不成立，以此记录关系动态变化，默认为1
    //iteration: 迭代次数。默认为0，每迭代一次，就加1，则为0的为最近的数据

	public class UserRelation
	{
        public UserRelation()
        { }

		#region Model
		private long _source_uid;
        private long _target_uid;
        private int _relation_state=1;
        private int _iteration;
        private string _update_time;
		/// <summary>
		/// 源用户UID
		/// </summary>
		public long source_uid
		{
			set{ _source_uid=value;}
			get{return _source_uid;}
		}
        /// <summary>
		/// 目的用户UID
		/// </summary>
		public long target_uid
		{
			set{ _target_uid=value;}
			get{return _target_uid;}
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
            set { _iteration = value; }
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
		public UserRelation(long lSourceUID,long lTargetUID)
		{
            this.GetModel( lSourceUID,lTargetUID );
		}

		/// <summary>
		/// 是否存在指定的有效关注关系（最新状态）
		/// </summary>
        static public bool Exists ( long lSourceUID, long lTargetUID)
		{
            Database db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect( "select count(relation_state) from user_relation where source_uid=" + lSourceUID.ToString()+" and target_uid="+lTargetUID.ToString()+" and relation_state=1" );
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
        /// 根据指定的源、目的Uid得到他们最新关系的一个对象实体
        /// </summary>
        public bool GetModel ( long lSourceUID, long lTargetUID)
        {
            Database db = DatabaseFactory.CreateDatabase();
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
		/// 获得指定源用户关注的UID列表
		/// </summary>
        static public LinkedList<long> GetFollowingUID(long lSourceUID)
        {
            Database db = DatabaseFactory.CreateDatabase();
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
        /// 获得关注指定目标用户的UID列表
        /// </summary>
        static public LinkedList<long> GetFollowedByUID ( long lTargetUID )
        {
            Database db = DatabaseFactory.CreateDatabase();
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
        /// 获得用户关系表中所有UID，包括source_uid和target_uid
        /// 返回DataTable，以便于调用者观察进度——本不应该将DataTable暴露给上层的，无奈
        /// </summary>
        static public DataTable GetAllUIDTable ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "select uid from all_uid order by update_time";
            DataSet ds = db.GetDataSet( strSQL );
            if (ds == null) return null;
            else return ds.Tables[0];
        }

		#endregion  成员方法
	}
}

