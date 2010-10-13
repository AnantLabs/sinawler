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
        + <user>
        + <status>
    </comment>
    */
    public class Comment : ModelBase
	{
        #region Model

		private long _comment_id;
		private string _content;
		private string _created_at;
		private long _uid;
		private long _status_id;
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
		/// 评论人ID
		/// </summary>
		public long uid
		{
			set{ _uid=value;}
			get{return _uid;}
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
		/// 得到一个对象实体
		/// </summary>
		public Comment()
		{
//            StringBuilder strSql=new StringBuilder();
//            strSql.Append("select comment_id,content,source_url,source_name,favorited,truncated,created_at,uid,status_id,reply_comment_id ");
//            strSql.Append(" FROM comments ");
//            //strSql.Append(" where 条件);
//            SqlParameter[] parameters = {
//};

//            DataSet ds = DbHelperSQL.GetDataSet(strSql.ToString(), parameters);
//            if(ds.Tables[0].Rows.Count>0)
//            {
//                comment_id=(long)(ds.Tables[0].Rows[0]["comment_id"]);
//                content=ds.Tables[0].Rows[0]["content"].ToString();
//                source_url=ds.Tables[0].Rows[0]["source_url"].ToString();
//                source_name=ds.Tables[0].Rows[0]["source_name"].ToString();
//                if(ds.Tables[0].Rows[0]["favorited"].ToString()!="")
//                {
//                    if((ds.Tables[0].Rows[0]["favorited"].ToString()=="1")||(ds.Tables[0].Rows[0]["favorited"].ToString().ToLower()=="true"))
//                    {
//                        favorited=true;
//                    }
//                    else
//                    {
//                        favorited=false;
//                    }
//                }

//                if(ds.Tables[0].Rows[0]["truncated"].ToString()!="")
//                {
//                    if((ds.Tables[0].Rows[0]["truncated"].ToString()=="1")||(ds.Tables[0].Rows[0]["truncated"].ToString().ToLower()=="true"))
//                    {
//                        truncated=true;
//                    }
//                    else
//                    {
//                        truncated=false;
//                    }
//                }

//                if(ds.Tables[0].Rows[0]["created_at"].ToString()!="")
//                {
//                    created_at=DateTime.Parse(ds.Tables[0].Rows[0]["created_at"].ToString());
//                }
//                uid=(long)(ds.Tables[0].Rows[0]["uid"]);
//                status_id=(long)(ds.Tables[0].Rows[0]["status_id"]);
//                reply_comment_id=(long)(ds.Tables[0].Rows[0]["reply_comment_id"]);
//            }
		}

		/// <summary>
		/// 数据库中是否存在指定ID的评论
		/// </summary>
		static public bool Exists(long lCommentID)
		{
            int count = db.CountByExecuteSQLSelect( "select comment_id from comments where comment_id=" + lCommentID.ToString() );
            return count > 0;
		}

        /// <summary>
        /// 更新数据库中已有数据的迭代次数
        /// </summary>
        static public void NewIterate ()
        {
            db.CountByExecuteSQL( "update comments set iteration=iteration+1" );
        }

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add()
		{
            try
            {
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString( "u" ).Replace( "Z", "" ) + "'";
                htValues.Add( "comment_id", _comment_id );
                htValues.Add( "created_at", "'" + _created_at + "'" );
                htValues.Add( "content", "'" + _content.Replace( "'", "''" ) + "'" );
                htValues.Add( "uid", _uid );
                htValues.Add( "status_id", _status_id );
                htValues.Add( "iteration", iteration );
                htValues.Add( "update_time", _update_time );

                db.Insert( "comments", htValues );
            }
            catch
            { return; }
		}

		#endregion  成员方法
	}
}

