using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace Sinawler.Model
{
	/// <summary>
	/// 类tags。
	/// </summary>
    public class tags
	{
        static Database db = DatabaseFactory.CreateDatabase();

		public tags()
        { }

		#region Model
		private long _tag_id;
		private string _tag;
		/// <summary>
		/// 
		/// </summary>
		public long tag_id
		{
			set{ _tag_id=value;}
			get{return _tag_id;}
		}
		/// <summary>
		/// 标签
		/// </summary>
		public string tag
		{
			set{ _tag=value;}
			get{return _tag;}
		}
		#endregion Model

		#region  成员方法

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public tags(long tag_id)
		{
            //StringBuilder strSql=new StringBuilder();
            //strSql.Append("select tag_id,tag ");
            //strSql.Append(" FROM tags ");
            //strSql.Append(" where tag_id=@tag_id ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@tag_id", SqlDbType.BigInt)};
            //parameters[0].Value = tag_id;

            //DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    tag_id=ds.Tables[0].Rows[0]["tag_id"].ToString();
            //    tag=ds.Tables[0].Rows[0]["tag"].ToString();
            //}
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(long tag_id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from tags");
			strSql.Append(" where tag_id=@tag_id ");

			SqlParameter[] parameters = {
					new SqlParameter("@tag_id", SqlDbType.BigInt)};
			parameters[0].Value = tag_id;

			return true;
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into tags(");
			strSql.Append("tag_id,tag)");
			strSql.Append(" values (");
			strSql.Append("@tag_id,@tag)");
			SqlParameter[] parameters = {
					new SqlParameter("@tag_id", SqlDbType.BigInt,8),
					new SqlParameter("@tag", SqlDbType.VarChar,50)};
			parameters[0].Value = tag_id;
			parameters[1].Value = tag;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public void Update()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update tags set ");
			strSql.Append("tag=@tag");
			strSql.Append(" where tag_id=@tag_id ");
			SqlParameter[] parameters = {
					new SqlParameter("@tag_id", SqlDbType.BigInt,8),
					new SqlParameter("@tag", SqlDbType.VarChar,50)};
			parameters[0].Value = tag_id;
			parameters[1].Value = tag;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public void Delete(long tag_id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tags ");
			strSql.Append(" where tag_id=@tag_id ");
			SqlParameter[] parameters = {
					new SqlParameter("@tag_id", SqlDbType.BigInt)};
			parameters[0].Value = tag_id;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public void GetModel(long tag_id)
		{
            //StringBuilder strSql=new StringBuilder();
            //strSql.Append("select  top 1 tag_id,tag ");
            //strSql.Append(" FROM tags ");
            //strSql.Append(" where tag_id=@tag_id ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@tag_id", SqlDbType.BigInt)};
            //parameters[0].Value = tag_id;

            //DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    if(ds.Tables[0].Rows[0]["tag_id"].ToString()!="")
            //    {
            //        model.tag_id=long.Parse(ds.Tables[0].Rows[0]["tag_id"].ToString());
            //    }
            //    model.tag=ds.Tables[0].Rows[0]["tag"].ToString();
            //}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select * ");
			strSql.Append(" FROM tags ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return null;
		}

        public void ReLoadDBSettings()
        {
            db.LoadSettings();
        }

		#endregion  成员方法
	}
}

