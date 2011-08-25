using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
    /// <summary>
	/// 类InvalidRelation。此类中的方法，都是与数据库交互，从中读取或向其中写入数据。
    /// 若“源用户ID”与“目的用户ID”之间有关系，表明“源用户”与“目的用户”的关系已无效，原因是某个用户已被取消
    /// 鉴于此类的特殊性（不同迭代次数时可能存在同样的关系），规定：除非明确指定，否则获取的模型均为最新的关系
	/// </summary>
    
    //source_user_id: 源用户UserID
    //target_user_id: 目的用户UserID

	public class InvalidRelation
	{
        public InvalidRelation()
        { }

		#region Model
		private long _source_user_id;
        private long _target_user_id;
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
                htValues.Add( "update_time", _update_time );

                db.Insert( "invalid_relations", htValues );
            }
            catch
            { return; }
		}
		#endregion  成员方法
	}
}

