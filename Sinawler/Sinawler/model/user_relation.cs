using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
    /// <summary>
	/// 类UserRelation。此类中的方法，都是与数据库交互，从中读取或向其中写入数据。
    /// 若“源用户ID”与“目的用户ID”之间有关系，表明“源用户”关注“目的用户”
    /// 鉴于此类的特殊性（不同迭代次数时可能存在同样的关系），规定：除非明确指定，否则获取的模型均为最新的关系
	/// </summary>
    
    //source_user_id: 源用户UserID
    //target_user_id: 目的用户UserID
    //iteration: 迭代次数。默认为0，每迭代一次，就加1，则为0的为最近的数据

	public class UserRelation
	{
        public UserRelation()
        { }

		#region Model
		private long _source_user_id;
        private long _target_user_id;
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
                htValues.Add( "iteration", 0 );
                htValues.Add( "update_time", _update_time );

                db.Insert( "user_relation", htValues );
            }
            catch
            { return; }
		}

        /// <summary>
        /// 判断是否存在指定的关系
        /// </summary>
        public static bool RelationshipExist(long lSourceUID, long lTargetUID)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DataRow dr=db.GetDataRow("select count(*) from user_relation where source_user_id=" + lSourceUID.ToString() + " and target_user_id=" + lTargetUID.ToString());
                if (dr == null) return false;
                else
                {
                    if (Convert.ToInt32(dr[0]) == 0) return false;
                    else return true;
                }
            }
            catch
            { return false; }
        }

		#endregion  成员方法
	}

    #region 关系信息
    public class RelationShip
    {
        public RelationInfo source { get; set; }
        public RelationInfo target { get; set; }
        public bool UserNotExist { get; set; }
    }

    public class RelationInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Int64 id { get; set; }
        /// <summary>
        /// 微博昵称
        /// </summary>
        public String screen_name { get; set; }
        /// <summary>
        /// 关注
        /// </summary>
        public Boolean following { get; set; }
        /// <summary>
        /// 被关注
        /// </summary>
        public Boolean followed_by { get; set; }
        /// <summary>
        /// 启用通知
        /// </summary>
        public Boolean notifications_enabled { get; set; }
    }
    #endregion
}

