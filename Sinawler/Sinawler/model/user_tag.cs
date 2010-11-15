using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
	/// <summary>
	/// 类user_tag。
	/// </summary>
    public class UserTag
	{
        public UserTag()
        { }
		#region Model
		private long _user_id;
		private long _tag_id;
        private int _iteration;
        private string _update_time;
		/// <summary>
		/// 
		/// </summary>
		public long user_id
		{
			set{ _user_id=value;}
			get{return _user_id;}
		}
		/// <summary>
		/// 标签ID
		/// </summary>
		public long tag_id
		{
			set{ _tag_id=value;}
			get{return _tag_id;}
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
        /// 是否存在指定的有效关注关系（最新状态）
        /// </summary>
        static public bool Exists ( long lUserID, long lTagID )
        {
            Database db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect( "select count(*) from user_tag where user_id=" + lUserID.ToString() + " and tag_id=" + lTagID.ToString() );
            return count > 0;
        }

        /// <summary>
        /// 更新数据库中已有数据的迭代次数
        /// </summary>
        static public void NewIterate ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            db.CountByExecuteSQL( "update user_tag set iteration=iteration+1" );
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add ()
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString( "u" ).Replace( "Z", "" ) + "'";
                htValues.Add( "user_id", _user_id );
                htValues.Add( "tag_id", _tag_id );
                htValues.Add( "iteration", 0 );
                htValues.Add( "update_time", _update_time );

                db.Insert( "user_tag", htValues );
            }
            catch
            { return; }
        }

		/// <summary>
		/// 得到一个对象实体
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

		#endregion  成员方法
	}
}

