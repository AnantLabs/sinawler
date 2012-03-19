using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
	/// <summary>
	/// 类tags。
	/// </summary>
    public class Tag
	{
        public Tag()
        { }

		#region Model
		private long _tag_id;
		private string _tag;
        private long _weight;
        private int _iteration=0;
        private string _update_time;
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
        /// <summary>
        /// 权重
        /// </summary>
        public long weight
        {
            set { _weight = value; }
            get { return _weight; }
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
		/// 是否存在该记录
		/// </summary>
        static public bool Exists ( long lTagID )
        {
            Database db = DatabaseFactory.CreateDatabase();
            db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect( "select count(tag_id) from tags where tag_id=" + lTagID.ToString() );
            return count > 0;
        }

        /// <summary>
        /// 更新数据库中已有数据的迭代次数
        /// </summary>
        static public void NewIterate ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            db = DatabaseFactory.CreateDatabase();
            db.CountByExecuteSQL( "update tags set iteration=iteration+1" );
        }

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add()
		{
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString( "u" ).Replace( "Z", "" ) + "'";
                htValues.Add( "tag_id", _tag_id );
                htValues.Add( "tag", "'"+_tag+"'" );
                htValues.Add( "iteration", 0 );
                htValues.Add( "update_time", _update_time );

                db.Insert( "tags", htValues );
            }
            catch
            { return; }
		}

        /// <summary>
        /// 更新数据
        /// </summary>
        public void Update()
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString("u").Replace("Z", "") + "'";
                htValues.Add("tag_id", _tag_id);
                htValues.Add("tag", "'" + _tag.Replace("'", "''") + "'");
                htValues.Add("weight", _weight);
                htValues.Add("iteration", 0);
                htValues.Add("update_time", _update_time);

                db.Update("tags", htValues, "tag_id=" + _tag_id.ToString());
            }
            catch
            { return; }
        }

		#endregion  成员方法
	}
}

