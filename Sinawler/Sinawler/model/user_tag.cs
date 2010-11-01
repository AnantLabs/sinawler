using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace Sinawler.Model
{
	/// <summary>
	/// ��user_tag��
	/// </summary>
    public class user_tag
	{
        public user_tag()
        { }
		#region Model
		private long _user_id;
		private long _tag_id;
		/// <summary>
		/// 
		/// </summary>
		public long user_id
		{
			set{ _user_id=value;}
			get{return _user_id;}
		}
		/// <summary>
		/// ��ǩID
		/// </summary>
		public long tag_id
		{
			set{ _tag_id=value;}
			get{return _tag_id;}
		}
		#endregion Model

		#region  ��Ա����

		/// <summary>
		/// �õ�һ������ʵ��
		/// </summary>
		public user_tag(long user_id,long tag_id)
		{
            //StringBuilder strSql=new StringBuilder();
            //strSql.Append("select user_id,tag_id ");
            //strSql.Append(" FROM user_tag ");
            //strSql.Append(" where user_id=@user_id and tag_id=@tag_id ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@user_id", SqlDbType.BigInt),
            //        new SqlParameter("@tag_id", SqlDbType.BigInt)};
            //parameters[0].Value = user_id;
            //parameters[1].Value = tag_id;

            //DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    _user_id=ds.Tables[0].Rows[0]["user_id"].ToString();
            //    _tag_id=ds.Tables[0].Rows[0]["tag_id"].ToString();
            //}
		}

		/// <summary>
		/// �Ƿ���ڸü�¼
		/// </summary>
		public bool Exists(long user_id,long tag_id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from user_tag");
			strSql.Append(" where user_id=@user_id and tag_id=@tag_id ");

			SqlParameter[] parameters = {
					new SqlParameter("@user_id", SqlDbType.BigInt),
					new SqlParameter("@tag_id", SqlDbType.BigInt)};
			parameters[0].Value = user_id;
			parameters[1].Value = tag_id;

			return true;
		}


		/// <summary>
		/// ����һ������
		/// </summary>
		public void Add()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into user_tag(");
			strSql.Append("user_id,tag_id)");
			strSql.Append(" values (");
			strSql.Append("@user_id,@tag_id)");
			SqlParameter[] parameters = {
					new SqlParameter("@user_id", SqlDbType.BigInt,8),
					new SqlParameter("@tag_id", SqlDbType.BigInt,8)};
			parameters[0].Value = user_id;
			parameters[1].Value = tag_id;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}
		/// <summary>
		/// ����һ������
		/// </summary>
//        public void Update()
//        {
//            StringBuilder strSql=new StringBuilder();
//            strSql.Append("update user_tag set ");
//");
//            strSql.Append(" where user_id=@user_id and tag_id=@tag_id ");
//            SqlParameter[] parameters = {
//                    new SqlParameter("@user_id", SqlDbType.BigInt,8),
//                    new SqlParameter("@tag_id", SqlDbType.BigInt,8)};
//            parameters[0].Value = user_id;
//            parameters[1].Value = tag_id;

//            DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
//        }

		/// <summary>
		/// ɾ��һ������
		/// </summary>
		public void Delete(long user_id,long tag_id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from user_tag ");
			strSql.Append(" where user_id=@user_id and tag_id=@tag_id ");
			SqlParameter[] parameters = {
					new SqlParameter("@user_id", SqlDbType.BigInt),
					new SqlParameter("@tag_id", SqlDbType.BigInt)};
			parameters[0].Value = user_id;
			parameters[1].Value = tag_id;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}


		/// <summary>
		/// �õ�һ������ʵ��
		/// </summary>
		public void GetModel(long user_id,long tag_id)
		{
            //StringBuilder strSql = new StringBuilder();
            //strSql.Append("select  top 1 user_id,tag_id ");
            //strSql.Append(" FROM user_tag ");
            //strSql.Append(" where user_id=@user_id and tag_id=@tag_id ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@user_id", SqlDbType.BigInt),
            //        new SqlParameter("@tag_id", SqlDbType.BigInt)};
            //parameters[0].Value = user_id;
            //parameters[1].Value = tag_id;

            //DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    if (ds.Tables[0].Rows[0]["user_id"].ToString() != "")
            //    {
            //        model.user_id = long.Parse(ds.Tables[0].Rows[0]["user_id"].ToString());
            //    }
            //    if (ds.Tables[0].Rows[0]["tag_id"].ToString() != "")
            //    {
            //        model.tag_id = long.Parse(ds.Tables[0].Rows[0]["tag_id"].ToString());
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
			strSql.Append(" FROM user_tag ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return null;
		}

		#endregion  ��Ա����
	}
}

