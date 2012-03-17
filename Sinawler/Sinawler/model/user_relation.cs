using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
    /// <summary>
	/// ��UserRelation�������еķ��������������ݿ⽻�������ж�ȡ��������д�����ݡ�
    /// ����Դ�û�ID���롰Ŀ���û�ID��֮���й�ϵ��������Դ�û�����ע��Ŀ���û���
    /// ���ڴ���������ԣ���ͬ��������ʱ���ܴ���ͬ���Ĺ�ϵ�����涨��������ȷָ���������ȡ��ģ�;�Ϊ���µĹ�ϵ
	/// </summary>
    
    //source_user_id: Դ�û�UserID
    //target_user_id: Ŀ���û�UserID
    //iteration: ����������Ĭ��Ϊ0��ÿ����һ�Σ��ͼ�1����Ϊ0��Ϊ���������

	public class UserRelation
	{
        public UserRelation()
        { }

		#region Model
		private long _source_user_id;
        private long _target_user_id;
        private int _iteration;
        private string _update_time;
		/// <summary>
		/// Դ�û�UserID
		/// </summary>
		public long source_user_id
		{
			set{ _source_user_id=value;}
			get{return _source_user_id;}
		}
        /// <summary>
		/// Ŀ���û�UserID
		/// </summary>
		public long target_user_id
		{
			set{ _target_user_id=value;}
			get{return _target_user_id;}
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
		/// ����һ������
		/// </summary>
		public void Add()
		{
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString( "u" ).Replace( "Z", "" ) + "'";
                htValues.Add( "source_user_id", _source_user_id );
                htValues.Add( "target_user_id", _target_user_id );
                htValues.Add( "iteration", 0 );
                htValues.Add( "update_time", _update_time );

                db.Insert( "user_relation", htValues );
            }
            catch
            { return; }
		}

        /// <summary>
        /// �ж��Ƿ����ָ���Ĺ�ϵ
        /// </summary>
        public static bool RelationshipExist(long lSourceUID, long lTargetUID)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DataRow dr=db.GetDataRow("select count(*) from user_relation where source_user_id=" + lSourceUID.ToString() + " and target_user_id=" + lTargetUID.ToString());
                if (dr == null) return false;
                else
                {
                    if (Convert.ToInt32(dr[0]) == 0) return false;
                    else return true;
                }
            }
            catch
            { return false; }
        }

		#endregion  ��Ա����
	}

    #region ��ϵ��Ϣ
    public class RelationShip
    {
        public RelationInfo source { get; set; }
        public RelationInfo target { get; set; }
        public bool UserNotExist { get; set; }
    }

    public class RelationInfo
    {
        /// <summary>
        /// �û�ID
        /// </summary>
        public Int64 id { get; set; }
        /// <summary>
        /// ΢���ǳ�
        /// </summary>
        public String screen_name { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        public Boolean following { get; set; }
        /// <summary>
        /// ����ע
        /// </summary>
        public Boolean followed_by { get; set; }
        /// <summary>
        /// ����֪ͨ
        /// </summary>
        public Boolean notifications_enabled { get; set; }
    }
    #endregion
}

