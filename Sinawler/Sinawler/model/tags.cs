using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Sinawler.Model
{
	/// <summary>
	/// ��tags��
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
		/// ��ǩ
		/// </summary>
		public string tag
		{
			set{ _tag=value;}
			get{return _tag;}
		}
        /// <summary>
        /// Ȩ��
        /// </summary>
        public long weight
        {
            set { _weight = value; }
            get { return _weight; }
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
		/// �Ƿ���ڸü�¼
		/// </summary>
        static public bool Exists ( long lTagID )
        {
            Database db = DatabaseFactory.CreateDatabase();
            db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect( "select count(tag_id) from tags where tag_id=" + lTagID.ToString() );
            return count > 0;
        }

        /// <summary>
        /// �������ݿ����������ݵĵ�������
        /// </summary>
        static public void NewIterate ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            db = DatabaseFactory.CreateDatabase();
            db.CountByExecuteSQL( "update tags set iteration=iteration+1" );
        }

		/// <summary>
		/// ����һ������
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
        /// ��������
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

		#endregion  ��Ա����
	}
}

