using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace SinaMBCrawler
{
	/// <summary>
	/// ��followed_by��
	/// </summary>
	public class followed_by
	{
		public followed_by()
		{}
		#region Model
		private long _uid;
		private long _followed_by_uid;
		/// <summary>
		/// 
		/// </summary>
		public long uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// ��˿UID
		/// </summary>
		public long followed_by_uid
		{
			set{ _followed_by_uid=value;}
			get{return _followed_by_uid;}
		}
		#endregion Model


		#region  ��Ա����

		/// <summary>
		/// �õ�һ������ʵ��
		/// </summary>
		public followed_by(long uid,long followed_by_uid)
		{
            //StringBuilder strSql=new StringBuilder();
            //strSql.Append("select uid,followed_by_uid ");
            //strSql.Append(" FROM followed_by ");
            //strSql.Append(" where uid=@uid and followed_by_uid=@followed_by_uid ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@uid", SqlDbType.BigInt),
            //        new SqlParameter("@followed_by_uid", SqlDbType.BigInt)};
            //parameters[0].Value = uid;
            //parameters[1].Value = followed_by_uid;

            //DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    uid=ds.Tables[0].Rows[0]["uid"].ToString();
            //    followed_by_uid=ds.Tables[0].Rows[0]["followed_by_uid"].ToString();
            //}
		}

		/// <summary>
		/// �Ƿ���ڸü�¼
		/// </summary>
		public bool Exists(long uid,long followed_by_uid)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from followed_by");
			strSql.Append(" where uid=@uid and followed_by_uid=@followed_by_uid ");

			SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.BigInt),
					new SqlParameter("@followed_by_uid", SqlDbType.BigInt)};
			parameters[0].Value = uid;
			parameters[1].Value = followed_by_uid;

            return true;
		}


		/// <summary>
		/// ����һ������
		/// </summary>
		public void Add()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into followed_by(");
			strSql.Append("uid,followed_by_uid)");
			strSql.Append(" values (");
			strSql.Append("@uid,@followed_by_uid)");
			SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.BigInt,8),
					new SqlParameter("@followed_by_uid", SqlDbType.BigInt,8)};
			parameters[0].Value = uid;
			parameters[1].Value = followed_by_uid;

			
		}
		/// <summary>
		/// ����һ������
		/// </summary>
//        public void Update()
//        {
//            StringBuilder strSql=new StringBuilder();
//            strSql.Append("update followed_by set ");
//");
//            strSql.Append(" where uid=@uid and followed_by_uid=@followed_by_uid ");
//            SqlParameter[] parameters = {
//                    new SqlParameter("@uid", SqlDbType.BigInt,8),
//                    new SqlParameter("@followed_by_uid", SqlDbType.BigInt,8)};
//            parameters[0].Value = uid;
//            parameters[1].Value = followed_by_uid;

//            DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
//        }

		/// <summary>
		/// ɾ��һ������
		/// </summary>
		public void Delete(long uid,long followed_by_uid)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from followed_by ");
			strSql.Append(" where uid=@uid and followed_by_uid=@followed_by_uid ");
			SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.BigInt),
					new SqlParameter("@followed_by_uid", SqlDbType.BigInt)};
			parameters[0].Value = uid;
			parameters[1].Value = followed_by_uid;

			
		}


		/// <summary>
		/// �õ�һ������ʵ��
		/// </summary>
		public void GetModel(long uid,long followed_by_uid)
		{
            //StringBuilder strSql=new StringBuilder();
            //strSql.Append("select  top 1 uid,followed_by_uid ");
            //strSql.Append(" FROM followed_by ");
            //strSql.Append(" where uid=@uid and followed_by_uid=@followed_by_uid ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@uid", SqlDbType.BigInt),
            //        new SqlParameter("@followed_by_uid", SqlDbType.BigInt)};
            //parameters[0].Value = uid;
            //parameters[1].Value = followed_by_uid;

            //DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    if(ds.Tables[0].Rows[0]["uid"].ToString()!="")
            //    {
            //        model.uid=long.Parse(ds.Tables[0].Rows[0]["uid"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["followed_by_uid"].ToString()!="")
            //    {
            //        model.followed_by_uid=long.Parse(ds.Tables[0].Rows[0]["followed_by_uid"].ToString());
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
			strSql.Append(" FROM followed_by ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return null;
		}

		#endregion  ��Ա����
	}
}

