using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
	/// <summary>
    /// 类Comment。此类中的方法，都是与数据库交互，从中读取或向其中写入数据
	/// </summary>
    /// 
    /*
     <comment>
        <created_at>Thu Sep 16 19:47:22 +0800 2010</created_at> 
        <id>3311494857</id> 
        <text>回复@四正:测试2</text> 
        <mid>xxxxxxxx</mid>
        + <user>
        + <status>
    </comment>
    */
    public class Comment
	{
        #region Model

		private long _comment_id;
		private string _content;
		private string _created_at;
        private string _source_url;
        private string _source_name;
		private User _user=new User();
		private long _status_id;
        private long _mid;
		private int _iteration;
        private string _update_time;
		/// <summary>
		/// 评论ID（XML中为id）
		/// </summary>
		public long comment_id
		{
			set{ _comment_id=value;}
			get{return _comment_id;}
		}
		/// <summary>
		/// 评论内容（XML中为text）
		/// </summary>
		public string content
		{
			set{ _content=value;}
			get{return _content;}
		}
		/// <summary>
		/// 评论时间
		/// </summary>
		public string created_at
		{
			set{ _created_at=value;}
			get{return _created_at;}
		}
        /// <summary>
        /// 评论来源中的URL
        /// </summary>
        public string source_url
        {
            set { _source_url = value; }
            get { return _source_url; }
        }
        /// <summary>
        /// 评论来源的名字
        /// </summary>
        public string source_name
        {
            set { _source_name = value; }
            get { return _source_name; }
        }
		/// <summary>
		/// 评论人
		/// </summary>
		public User user
		{
			set{ _user=value;}
			get{return _user;}
		}
		/// <summary>
		/// 评论的微博
		/// </summary>
		public long status_id
		{
			set{ _status_id=value;}
			get{return _status_id;}
		}
        /// <summary>
        /// mid
        /// </summary>
        public long mid
        {
            set { _mid = value; }
            get { return _mid; }
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
		/// 得到一个对象实体
		/// </summary>
		public Comment()
        { }

		/// <summary>
		/// 数据库中是否存在指定ID的评论
		/// </summary>
		static public bool Exists(long lCommentID)
		{
            Database db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect( "select count(comment_id) from comments where comment_id=" + lCommentID.ToString() );
            return count > 0;
		}

        /// <summary>
        /// 更新数据库中已有数据的迭代次数
        /// </summary>
        static public void NewIterate ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            db.CountByExecuteSQL( "update comments set iteration=iteration+1" );
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
                htValues.Add( "comment_id", _comment_id );
                htValues.Add( "created_at", "'" + _created_at + "'" );
                htValues.Add( "content", "'" + _content.Replace( "'", "''" ) + "'" );
                htValues.Add( "user_id", _user.user_id );
                htValues.Add( "status_id", _status_id );
                htValues.Add("mid", _mid);
                htValues.Add( "iteration", 0 );
                htValues.Add( "update_time", _update_time );

                db.Insert( "comments", htValues );
            }
            catch
            { return; }
		}

		#endregion  成员方法
	}
}

