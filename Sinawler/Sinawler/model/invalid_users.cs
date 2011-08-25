using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
    /// <summary>
	/// 类InvalidUser。此类中的方法，都是与数据库交互，从中读取或向其中写入数据。
	/// </summary>
    
    //user_id: 无效用户UserID

	public class InvalidUser
	{
        public InvalidUser()
        { }

		#region Model
		private long _user_id;
        private string _update_time;
		/// <summary>
		/// 用户UserID
		/// </summary>
		public long user_id
		{
			set{ _user_id=value;}
			get{return _user_id;}
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
                htValues.Add( "user_id", _user_id );
                htValues.Add( "update_time", _update_time );

                db.Insert( "invalid_users", htValues );
            }
            catch
            { return; }
		}
		#endregion  成员方法
	}
}

