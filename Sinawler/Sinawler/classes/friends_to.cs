using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace SinaMBCrawler
{
	/// <summary>
	/// 类friends_to。
	/// </summary>
	public class friends_to
	{
		public friends_to()
		{}
		#region Model
		private long _uid;
		private long _friend_to_uid;
		/// <summary>
		/// 
		/// </summary>
		public long uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// 关注的UID
		/// </summary>
		public long friend_to_uid
		{
			set{ _friend_to_uid=value;}
			get{return _friend_to_uid;}
		}
		#endregion Model


		#region  成员方法

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public friends_to(long uid,long friend_to_uid)
		{
            //StringBuilder strSql=new StringBuilder();
            //strSql.Append("select uid,friend_to_uid ");
            //strSql.Append(" FROM friends_to ");
            //strSql.Append(" where uid=@uid and friend_to_uid=@friend_to_uid ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@uid", SqlDbType.BigInt),
            //        new SqlParameter("@friend_to_uid", SqlDbType.BigInt)};
            //parameters[0].Value = uid;
            //parameters[1].Value = friend_to_uid;

            //DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    uid=ds.Tables[0].Rows[0]["uid"].ToString();
            //    friend_to_uid=ds.Tables[0].Rows[0]["friend_to_uid"].ToString();
            //}
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(long uid,long friend_to_uid)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from friends_to");
			strSql.Append(" where uid=@uid and friend_to_uid=@friend_to_uid ");

			SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.BigInt),
					new SqlParameter("@friend_to_uid", SqlDbType.BigInt)};
			parameters[0].Value = uid;
			parameters[1].Value = friend_to_uid;

			return true;
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into friends_to(");
			strSql.Append("uid,friend_to_uid)");
			strSql.Append(" values (");
			strSql.Append("@uid,@friend_to_uid)");
			SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.BigInt,8),
					new SqlParameter("@friend_to_uid", SqlDbType.BigInt,8)};
			parameters[0].Value = uid;
			parameters[1].Value = friend_to_uid;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
//        public void Update()
//        {
//            StringBuilder strSql=new StringBuilder();
//            strSql.Append("update friends_to set ");
//");
//            strSql.Append(" where uid=@uid and friend_to_uid=@friend_to_uid ");
//            SqlParameter[] parameters = {
//                    new SqlParameter("@uid", SqlDbType.BigInt,8),
//                    new SqlParameter("@friend_to_uid", SqlDbType.BigInt,8)};
//            parameters[0].Value = uid;
//            parameters[1].Value = friend_to_uid;

//            DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
//        }

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public void Delete(long uid,long friend_to_uid)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from friends_to ");
			strSql.Append(" where uid=@uid and friend_to_uid=@friend_to_uid ");
			SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.BigInt),
					new SqlParameter("@friend_to_uid", SqlDbType.BigInt)};
			parameters[0].Value = uid;
			parameters[1].Value = friend_to_uid;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public void GetModel(long uid,long friend_to_uid)
		{
            //StringBuilder strSql=new StringBuilder();
            //strSql.Append("select  top 1 uid,friend_to_uid ");
            //strSql.Append(" FROM friends_to ");
            //strSql.Append(" where uid=@uid and friend_to_uid=@friend_to_uid ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@uid", SqlDbType.BigInt),
            //        new SqlParameter("@friend_to_uid", SqlDbType.BigInt)};
            //parameters[0].Value = uid;
            //parameters[1].Value = friend_to_uid;

            //DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    if(ds.Tables[0].Rows[0]["uid"].ToString()!="")
            //    {
            //        model.uid=long.Parse(ds.Tables[0].Rows[0]["uid"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["friend_to_uid"].ToString()!="")
            //    {
            //        model.friend_to_uid=long.Parse(ds.Tables[0].Rows[0]["friend_to_uid"].ToString());
            //    }
            //}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select * ");
			strSql.Append(" FROM friends_to ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return null;
		}

		#endregion  成员方法
	}
}

