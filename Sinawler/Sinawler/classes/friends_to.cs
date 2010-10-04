using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace SinaMBCrawler
{
	/// <summary>
	/// ��friends_to��
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
		/// ��ע��UID
		/// </summary>
		public long friend_to_uid
		{
			set{ _friend_to_uid=value;}
			get{return _friend_to_uid;}
		}
		#endregion Model


		#region  ��Ա����

		/// <summary>
		/// �õ�һ������ʵ��
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
		/// �Ƿ���ڸü�¼
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
		/// ����һ������
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
		/// ����һ������
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
		/// ɾ��һ������
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
		/// �õ�һ������ʵ��
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
		/// ��������б�
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

		#endregion  ��Ա����
	}
}

