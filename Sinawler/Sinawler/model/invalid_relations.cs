using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
    /// <summary>
	/// ��InvalidRelation�������еķ��������������ݿ⽻�������ж�ȡ��������д�����ݡ�
    /// ����Դ�û�ID���롰Ŀ���û�ID��֮���й�ϵ��������Դ�û����롰Ŀ���û����Ĺ�ϵ����Ч��ԭ����ĳ���û��ѱ�ȡ��
    /// ���ڴ���������ԣ���ͬ��������ʱ���ܴ���ͬ���Ĺ�ϵ�����涨��������ȷָ���������ȡ��ģ�;�Ϊ���µĹ�ϵ
	/// </summary>
    
    //source_user_id: Դ�û�UserID
    //target_user_id: Ŀ���û�UserID

	public class InvalidRelation
	{
        public InvalidRelation()
        { }

		#region Model
		private long _source_user_id;
        private long _target_user_id;
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
                htValues.Add( "update_time", _update_time );

                db.Insert( "invalid_relations", htValues );
            }
            catch
            { return; }
		}
		#endregion  ��Ա����
	}
}

