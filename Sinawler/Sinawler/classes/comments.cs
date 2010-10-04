using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace SinaMBCrawler
{
	/// <summary>
	/// 类comments。
	/// </summary>
	public class comments
	{
		#region Model

		private long _comment_id;
		private string _content;
		private string _source_url;
		private string _source_name;
		private bool _favorited;
		private bool _truncated;
		private DateTime? _created_at;
		private long _uid;
		private long _status_id;
		private long _reply_comment_id;
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
		/// 来源中的URL
		/// </summary>
		public string source_url
		{
			set{ _source_url=value;}
			get{return _source_url;}
		}
		/// <summary>
		/// 来源名称
		/// </summary>
		public string source_name
		{
			set{ _source_name=value;}
			get{return _source_name;}
		}
		/// <summary>
		/// 是否收藏
		/// </summary>
		public bool favorited
		{
			set{ _favorited=value;}
			get{return _favorited;}
		}
		/// <summary>
		/// 是否被截断
		/// </summary>
		public bool truncated
		{
			set{ _truncated=value;}
			get{return _truncated;}
		}
		/// <summary>
		/// 评论时间
		/// </summary>
		public DateTime? created_at
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
		/// 评论来源（不懂）
		/// </summary>
		public long reply_comment_id
		{
			set{ _reply_comment_id=value;}
			get{return _reply_comment_id;}
		}
		#endregion Model


		#region  成员方法

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public comments()
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
		/// 是否存在该记录
		/// </summary>
		public bool Exists()
		{
            return true;
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into comments(");
			strSql.Append("comment_id,content,source_url,source_name,favorited,truncated,created_at,uid,status_id,reply_comment_id)");
			strSql.Append(" values (");
			strSql.Append("@comment_id,@content,@source_url,@source_name,@favorited,@truncated,@created_at,@uid,@status_id,@reply_comment_id)");
			SqlParameter[] parameters = {
					new SqlParameter("@comment_id", SqlDbType.BigInt,8),
					new SqlParameter("@content", SqlDbType.VarChar,500),
					new SqlParameter("@source_url", SqlDbType.VarChar,200),
					new SqlParameter("@source_name", SqlDbType.VarChar,100),
					new SqlParameter("@favorited", SqlDbType.Bit,1),
					new SqlParameter("@truncated", SqlDbType.Bit,1),
					new SqlParameter("@created_at", SqlDbType.DateTime),
					new SqlParameter("@uid", SqlDbType.BigInt,8),
					new SqlParameter("@status_id", SqlDbType.BigInt,8),
					new SqlParameter("@reply_comment_id", SqlDbType.BigInt,8)};
			parameters[0].Value = comment_id;
			parameters[1].Value = content;
			parameters[2].Value = source_url;
			parameters[3].Value = source_name;
			parameters[4].Value = favorited;
			parameters[5].Value = truncated;
			parameters[6].Value = created_at;
			parameters[7].Value = uid;
			parameters[8].Value = status_id;
			parameters[9].Value = reply_comment_id;
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public void Update()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update comments set ");
			strSql.Append("comment_id=@comment_id,");
			strSql.Append("content=@content,");
			strSql.Append("source_url=@source_url,");
			strSql.Append("source_name=@source_name,");
			strSql.Append("favorited=@favorited,");
			strSql.Append("truncated=@truncated,");
			strSql.Append("created_at=@created_at,");
			strSql.Append("uid=@uid,");
			strSql.Append("status_id=@status_id,");
			strSql.Append("reply_comment_id=@reply_comment_id");
			//strSql.Append(" where 条件);
			SqlParameter[] parameters = {
					new SqlParameter("@comment_id", SqlDbType.BigInt,8),
					new SqlParameter("@content", SqlDbType.VarChar,500),
					new SqlParameter("@source_url", SqlDbType.VarChar,200),
					new SqlParameter("@source_name", SqlDbType.VarChar,100),
					new SqlParameter("@favorited", SqlDbType.Bit,1),
					new SqlParameter("@truncated", SqlDbType.Bit,1),
					new SqlParameter("@created_at", SqlDbType.DateTime),
					new SqlParameter("@uid", SqlDbType.BigInt,8),
					new SqlParameter("@status_id", SqlDbType.BigInt,8),
					new SqlParameter("@reply_comment_id", SqlDbType.BigInt,8)};
			parameters[0].Value = comment_id;
			parameters[1].Value = content;
			parameters[2].Value = source_url;
			parameters[3].Value = source_name;
			parameters[4].Value = favorited;
			parameters[5].Value = truncated;
			parameters[6].Value = created_at;
			parameters[7].Value = uid;
			parameters[8].Value = status_id;
			parameters[9].Value = reply_comment_id;
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public void Delete()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from comments ");
			//strSql.Append(" where 条件);
			SqlParameter[] parameters = {
};
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public void GetModel()
		{
//            StringBuilder strSql=new StringBuilder();
//            strSql.Append("select  top 1 comment_id,content,source_url,source_name,favorited,truncated,created_at,uid,status_id,reply_comment_id ");
//            strSql.Append(" FROM comments ");
//            //strSql.Append(" where 条件);
//            SqlParameter[] parameters = {
//};

//            DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
//            if(ds.Tables[0].Rows.Count>0)
//            {
//                if(ds.Tables[0].Rows[0]["comment_id"].ToString()!="")
//                {
//                    model.comment_id=long.Parse(ds.Tables[0].Rows[0]["comment_id"].ToString());
//                }
//                model.content=ds.Tables[0].Rows[0]["content"].ToString();
//                model.source_url=ds.Tables[0].Rows[0]["source_url"].ToString();
//                model.source_name=ds.Tables[0].Rows[0]["source_name"].ToString();
//                if(ds.Tables[0].Rows[0]["favorited"].ToString()!="")
//                {
//                    if((ds.Tables[0].Rows[0]["favorited"].ToString()=="1")||(ds.Tables[0].Rows[0]["favorited"].ToString().ToLower()=="true"))
//                    {
//                        model.favorited=true;
//                    }
//                    else
//                    {
//                        model.favorited=false;
//                    }
//                }
//                if(ds.Tables[0].Rows[0]["truncated"].ToString()!="")
//                {
//                    if((ds.Tables[0].Rows[0]["truncated"].ToString()=="1")||(ds.Tables[0].Rows[0]["truncated"].ToString().ToLower()=="true"))
//                    {
//                        model.truncated=true;
//                    }
//                    else
//                    {
//                        model.truncated=false;
//                    }
//                }
//                if(ds.Tables[0].Rows[0]["created_at"].ToString()!="")
//                {
//                    model.created_at=DateTime.Parse(ds.Tables[0].Rows[0]["created_at"].ToString());
//                }
//                if(ds.Tables[0].Rows[0]["uid"].ToString()!="")
//                {
//                    model.uid=long.Parse(ds.Tables[0].Rows[0]["uid"].ToString());
//                }
//                if(ds.Tables[0].Rows[0]["status_id"].ToString()!="")
//                {
//                    model.status_id=long.Parse(ds.Tables[0].Rows[0]["status_id"].ToString());
//                }
//                if(ds.Tables[0].Rows[0]["reply_comment_id"].ToString()!="")
//                {
//                    model.reply_comment_id=long.Parse(ds.Tables[0].Rows[0]["reply_comment_id"].ToString());
//                }
//            }
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select * ");
			strSql.Append(" FROM comments ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            return null;
		}

		#endregion  成员方法
	}
}

