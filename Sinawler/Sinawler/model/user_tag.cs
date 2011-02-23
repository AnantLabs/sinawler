using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
	/// <summary>
	/// ��user_tag��
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
		/// ��ǩID
		/// </summary>
		public long tag_id
		{
			set{ _tag_id=value;}
			get{return _tag_id;}
		}
        /// <summary>
        /// ����������Ĭ��Ϊ0��ÿ����һ�Σ��ͼ�1����Ϊ0��Ϊ���������
        /// </summary>
        public int iteration
        {
            get { return _iteration; }
        }
        /// <summary>
        /// ��¼����ʱ��
        /// </summary>
        public string update_time
        {
            set { _update_time = value; }
            get { return _update_time; }
        }
		#endregion Model

		#region  ��Ա����

        /// <summary>
        /// �Ƿ����ָ������Ч��ע��ϵ������״̬��
        /// </summary>
        static public bool Exists ( long lUserID, long lTagID )
        {
            Database db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect( "select count(*) from user_tag where user_id=" + lUserID.ToString() + " and tag_id=" + lTagID.ToString() );
            return count > 0;
        }

        /// <summary>
        /// �������ݿ����������ݵĵ�������
        /// </summary>
        static public void NewIterate ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            db.CountByExecuteSQL( "update user_tag set iteration=iteration+1" );
        }

        /// <summary>
        /// ����һ������
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

		#endregion  ��Ա����
	}
}

