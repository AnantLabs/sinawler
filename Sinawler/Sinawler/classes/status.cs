using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace SinaMBCrawler
{
	/// <summary>
	/// ��statuses��
	/// </summary>
	public class Status
	{
		public Status()
		{}
		#region Model
		private long _status_id;
		private DateTime _created_at;
		private string _content;
		private string _source_url;
		private string _source_name;
		private bool _favorited;
		private bool _truncated;
		private long _in_reply_to_status_id;
		private long _in_reply_to_user_id;
		private string _in_reply_to_screen_name;
		private string _thumbnail_pic;
		private string _bmiddle_pic;
		private string _original_pic;
		private long _uid;
		private long _retweeted_status_id;
		/// <summary>
		/// ΢��ID��XML��Ϊid��
		/// </summary>
		public long status_id
		{
			set{ _status_id=value;}
			get{return _status_id;}
		}
		/// <summary>
		/// ����ʱ��
		/// </summary>
		public DateTime created_at
		{
			set{ _created_at=value;}
			get{return _created_at;}
		}
		/// <summary>
		/// ΢����Ϣ����(XML��Ϊtext)
		/// </summary>
		public string content
		{
			set{ _content=value;}
			get{return _content;}
		}
		/// <summary>
		/// ΢����Դ�е�URL
		/// </summary>
		public string source_url
		{
			set{ _source_url=value;}
			get{return _source_url;}
		}
		/// <summary>
		/// ΢����Դ������
		/// </summary>
		public string source_name
		{
			set{ _source_name=value;}
			get{return _source_name;}
		}
		/// <summary>
		/// �Ƿ����ղأ�Ӧ���Ƕ��ڵ�ǰ��¼�˺ţ�
		/// </summary>
		public bool favorited
		{
			set{ _favorited=value;}
			get{return _favorited;}
		}
		/// <summary>
		/// �Ƿ񱻽ض�
		/// </summary>
		public bool truncated
		{
			set{ _truncated=value;}
			get{return _truncated;}
		}
		/// <summary>
		/// �ظ�ID
		/// </summary>
		public long in_reply_to_status_id
		{
			set{ _in_reply_to_status_id=value;}
			get{return _in_reply_to_status_id;}
		}
		/// <summary>
		/// �ظ���UID
		/// </summary>
		public long in_reply_to_user_id
		{
			set{ _in_reply_to_user_id=value;}
			get{return _in_reply_to_user_id;}
		}
		/// <summary>
		/// �ظ����ǳ�
		/// </summary>
		public string in_reply_to_screen_name
		{
			set{ _in_reply_to_screen_name=value;}
			get{return _in_reply_to_screen_name;}
		}
		/// <summary>
		/// ����ͼ
		/// </summary>
		public string thumbnail_pic
		{
			set{ _thumbnail_pic=value;}
			get{return _thumbnail_pic;}
		}
		/// <summary>
		/// ����ͼƬ
		/// </summary>
		public string bmiddle_pic
		{
			set{ _bmiddle_pic=value;}
			get{return _bmiddle_pic;}
		}
		/// <summary>
		/// ԭʼͼƬ
		/// </summary>
		public string original_pic
		{
			set{ _original_pic=value;}
			get{return _original_pic;}
		}
		/// <summary>
		/// �û�ID
		/// </summary>
		public long uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// ͬʱת����΢��ID
		/// </summary>
		public long retweeted_status_id
		{
			set{ _retweeted_status_id=value;}
			get{return _retweeted_status_id;}
		}
		#endregion Model


		#region  ��Ա����

		/// <summary>
		/// �õ�һ������ʵ��
		/// </summary>
        //public statuses(long status_id)
        //{
        //    StringBuilder strSql=new StringBuilder();
        //    strSql.Append("select status_id,created_at,content,source_url,source_name,favorited,truncated,in_reply_to_status_id,in_reply_to_user_id,in_reply_to_screen_name,thumbnail_pic,bmiddle_pic,original_pic,uid,retweeted_status_id ");
        //    strSql.Append(" FROM statuses ");
        //    strSql.Append(" where status_id=@status_id ");
        //    SqlParameter[] parameters = {
        //            new SqlParameter("@status_id", SqlDbType.BigInt)};
        //    parameters[0].Value = status_id;

        //    DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
        //    if(ds.Tables[0].Rows.Count>0)
        //    {
        //        status_id=ds.Tables[0].Rows[0]["status_id"].ToString();
        //        if(ds.Tables[0].Rows[0]["created_at"].ToString()!="")
        //        {
        //            created_at=DateTime.Parse(ds.Tables[0].Rows[0]["created_at"].ToString());
        //        }
        //        content=ds.Tables[0].Rows[0]["content"].ToString();
        //        source_url=ds.Tables[0].Rows[0]["source_url"].ToString();
        //        source_name=ds.Tables[0].Rows[0]["source_name"].ToString();
        //        if(ds.Tables[0].Rows[0]["favorited"].ToString()!="")
        //        {
        //            if((ds.Tables[0].Rows[0]["favorited"].ToString()=="1")||(ds.Tables[0].Rows[0]["favorited"].ToString().ToLower()=="true"))
        //            {
        //                favorited=true;
        //            }
        //            else
        //            {
        //                favorited=false;
        //            }
        //        }

        //        if(ds.Tables[0].Rows[0]["truncated"].ToString()!="")
        //        {
        //            if((ds.Tables[0].Rows[0]["truncated"].ToString()=="1")||(ds.Tables[0].Rows[0]["truncated"].ToString().ToLower()=="true"))
        //            {
        //                truncated=true;
        //            }
        //            else
        //            {
        //                truncated=false;
        //            }
        //        }

        //        in_reply_to_status_id=ds.Tables[0].Rows[0]["in_reply_to_status_id"].ToString();
        //        in_reply_to_user_id=ds.Tables[0].Rows[0]["in_reply_to_user_id"].ToString();
        //        in_reply_to_screen_name=ds.Tables[0].Rows[0]["in_reply_to_screen_name"].ToString();
        //        thumbnail_pic=ds.Tables[0].Rows[0]["thumbnail_pic"].ToString();
        //        bmiddle_pic=ds.Tables[0].Rows[0]["bmiddle_pic"].ToString();
        //        original_pic=ds.Tables[0].Rows[0]["original_pic"].ToString();
        //        uid=ds.Tables[0].Rows[0]["uid"].ToString();
        //        retweeted_status_id=ds.Tables[0].Rows[0]["retweeted_status_id"].ToString();
        //    }
        //}

		/// <summary>
		/// �Ƿ���ڸü�¼
		/// </summary>
		public bool Exists(long status_id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from statuses");
			strSql.Append(" where status_id=@status_id ");

			SqlParameter[] parameters = {
					new SqlParameter("@status_id", SqlDbType.BigInt)};
			parameters[0].Value = status_id;

			return true;
		}


		/// <summary>
		/// ����һ������
		/// </summary>
		public void Add()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into statuses(");
			strSql.Append("status_id,created_at,content,source_url,source_name,favorited,truncated,in_reply_to_status_id,in_reply_to_user_id,in_reply_to_screen_name,thumbnail_pic,bmiddle_pic,original_pic,uid,retweeted_status_id)");
			strSql.Append(" values (");
			strSql.Append("@status_id,@created_at,@content,@source_url,@source_name,@favorited,@truncated,@in_reply_to_status_id,@in_reply_to_user_id,@in_reply_to_screen_name,@thumbnail_pic,@bmiddle_pic,@original_pic,@uid,@retweeted_status_id)");
			SqlParameter[] parameters = {
					new SqlParameter("@status_id", SqlDbType.BigInt,8),
					new SqlParameter("@created_at", SqlDbType.DateTime),
					new SqlParameter("@content", SqlDbType.VarChar,500),
					new SqlParameter("@source_url", SqlDbType.VarChar,200),
					new SqlParameter("@source_name", SqlDbType.VarChar,100),
					new SqlParameter("@favorited", SqlDbType.Bit,1),
					new SqlParameter("@truncated", SqlDbType.Bit,1),
					new SqlParameter("@in_reply_to_status_id", SqlDbType.BigInt,8),
					new SqlParameter("@in_reply_to_user_id", SqlDbType.BigInt,8),
					new SqlParameter("@in_reply_to_screen_name", SqlDbType.VarChar,50),
					new SqlParameter("@thumbnail_pic", SqlDbType.VarChar,500),
					new SqlParameter("@bmiddle_pic", SqlDbType.VarChar,500),
					new SqlParameter("@original_pic", SqlDbType.VarChar,500),
					new SqlParameter("@uid", SqlDbType.BigInt,8),
					new SqlParameter("@retweeted_status_id", SqlDbType.BigInt,8)};
			parameters[0].Value = status_id;
			parameters[1].Value = created_at;
			parameters[2].Value = content;
			parameters[3].Value = source_url;
			parameters[4].Value = source_name;
			parameters[5].Value = favorited;
			parameters[6].Value = truncated;
			parameters[7].Value = in_reply_to_status_id;
			parameters[8].Value = in_reply_to_user_id;
			parameters[9].Value = in_reply_to_screen_name;
			parameters[10].Value = thumbnail_pic;
			parameters[11].Value = bmiddle_pic;
			parameters[12].Value = original_pic;
			parameters[13].Value = uid;
			parameters[14].Value = retweeted_status_id;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}
		/// <summary>
		/// ����һ������
		/// </summary>
		public void Update()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update statuses set ");
			strSql.Append("created_at=@created_at,");
			strSql.Append("content=@content,");
			strSql.Append("source_url=@source_url,");
			strSql.Append("source_name=@source_name,");
			strSql.Append("favorited=@favorited,");
			strSql.Append("truncated=@truncated,");
			strSql.Append("in_reply_to_status_id=@in_reply_to_status_id,");
			strSql.Append("in_reply_to_user_id=@in_reply_to_user_id,");
			strSql.Append("in_reply_to_screen_name=@in_reply_to_screen_name,");
			strSql.Append("thumbnail_pic=@thumbnail_pic,");
			strSql.Append("bmiddle_pic=@bmiddle_pic,");
			strSql.Append("original_pic=@original_pic,");
			strSql.Append("uid=@uid,");
			strSql.Append("retweeted_status_id=@retweeted_status_id");
			strSql.Append(" where status_id=@status_id ");
			SqlParameter[] parameters = {
					new SqlParameter("@status_id", SqlDbType.BigInt,8),
					new SqlParameter("@created_at", SqlDbType.DateTime),
					new SqlParameter("@content", SqlDbType.VarChar,500),
					new SqlParameter("@source_url", SqlDbType.VarChar,200),
					new SqlParameter("@source_name", SqlDbType.VarChar,100),
					new SqlParameter("@favorited", SqlDbType.Bit,1),
					new SqlParameter("@truncated", SqlDbType.Bit,1),
					new SqlParameter("@in_reply_to_status_id", SqlDbType.BigInt,8),
					new SqlParameter("@in_reply_to_user_id", SqlDbType.BigInt,8),
					new SqlParameter("@in_reply_to_screen_name", SqlDbType.VarChar,50),
					new SqlParameter("@thumbnail_pic", SqlDbType.VarChar,500),
					new SqlParameter("@bmiddle_pic", SqlDbType.VarChar,500),
					new SqlParameter("@original_pic", SqlDbType.VarChar,500),
					new SqlParameter("@uid", SqlDbType.BigInt,8),
					new SqlParameter("@retweeted_status_id", SqlDbType.BigInt,8)};
			parameters[0].Value = status_id;
			parameters[1].Value = created_at;
			parameters[2].Value = content;
			parameters[3].Value = source_url;
			parameters[4].Value = source_name;
			parameters[5].Value = favorited;
			parameters[6].Value = truncated;
			parameters[7].Value = in_reply_to_status_id;
			parameters[8].Value = in_reply_to_user_id;
			parameters[9].Value = in_reply_to_screen_name;
			parameters[10].Value = thumbnail_pic;
			parameters[11].Value = bmiddle_pic;
			parameters[12].Value = original_pic;
			parameters[13].Value = uid;
			parameters[14].Value = retweeted_status_id;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}

		/// <summary>
		/// ɾ��һ������
		/// </summary>
		public void Delete(long status_id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from statuses ");
			strSql.Append(" where status_id=@status_id ");
			SqlParameter[] parameters = {
					new SqlParameter("@status_id", SqlDbType.BigInt)};
			parameters[0].Value = status_id;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}


		/// <summary>
		/// �õ�һ������ʵ��
		/// </summary>
		public void GetModel(long status_id)
		{
            //StringBuilder strSql=new StringBuilder();
            //strSql.Append("select  top 1 status_id,created_at,content,source_url,source_name,favorited,truncated,in_reply_to_status_id,in_reply_to_user_id,in_reply_to_screen_name,thumbnail_pic,bmiddle_pic,original_pic,uid,retweeted_status_id ");
            //strSql.Append(" FROM statuses ");
            //strSql.Append(" where status_id=@status_id ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@status_id", SqlDbType.BigInt)};
            //parameters[0].Value = status_id;

            //DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    if(ds.Tables[0].Rows[0]["status_id"].ToString()!="")
            //    {
            //        model.status_id=long.Parse(ds.Tables[0].Rows[0]["status_id"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["created_at"].ToString()!="")
            //    {
            //        model.created_at=DateTime.Parse(ds.Tables[0].Rows[0]["created_at"].ToString());
            //    }
            //    model.content=ds.Tables[0].Rows[0]["content"].ToString();
            //    model.source_url=ds.Tables[0].Rows[0]["source_url"].ToString();
            //    model.source_name=ds.Tables[0].Rows[0]["source_name"].ToString();
            //    if(ds.Tables[0].Rows[0]["favorited"].ToString()!="")
            //    {
            //        if((ds.Tables[0].Rows[0]["favorited"].ToString()=="1")||(ds.Tables[0].Rows[0]["favorited"].ToString().ToLower()=="true"))
            //        {
            //            model.favorited=true;
            //        }
            //        else
            //        {
            //            model.favorited=false;
            //        }
            //    }
            //    if(ds.Tables[0].Rows[0]["truncated"].ToString()!="")
            //    {
            //        if((ds.Tables[0].Rows[0]["truncated"].ToString()=="1")||(ds.Tables[0].Rows[0]["truncated"].ToString().ToLower()=="true"))
            //        {
            //            model.truncated=true;
            //        }
            //        else
            //        {
            //            model.truncated=false;
            //        }
            //    }
            //    if(ds.Tables[0].Rows[0]["in_reply_to_status_id"].ToString()!="")
            //    {
            //        model.in_reply_to_status_id=long.Parse(ds.Tables[0].Rows[0]["in_reply_to_status_id"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["in_reply_to_user_id"].ToString()!="")
            //    {
            //        model.in_reply_to_user_id=long.Parse(ds.Tables[0].Rows[0]["in_reply_to_user_id"].ToString());
            //    }
            //    model.in_reply_to_screen_name=ds.Tables[0].Rows[0]["in_reply_to_screen_name"].ToString();
            //    model.thumbnail_pic=ds.Tables[0].Rows[0]["thumbnail_pic"].ToString();
            //    model.bmiddle_pic=ds.Tables[0].Rows[0]["bmiddle_pic"].ToString();
            //    model.original_pic=ds.Tables[0].Rows[0]["original_pic"].ToString();
            //    if(ds.Tables[0].Rows[0]["uid"].ToString()!="")
            //    {
            //        model.uid=long.Parse(ds.Tables[0].Rows[0]["uid"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["retweeted_status_id"].ToString()!="")
            //    {
            //        model.retweeted_status_id=long.Parse(ds.Tables[0].Rows[0]["retweeted_status_id"].ToString());
            //    }
            //}
		}

		/// <summary>
		/// ��������б�
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select * ");
			strSql.Append(" FROM statuses ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return null;
		}

		#endregion  ��Ա����
	}
}

