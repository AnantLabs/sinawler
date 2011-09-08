using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
    /// <summary>
	/// ��InvalidUser�������еķ��������������ݿ⽻�������ж�ȡ��������д�����ݡ�
	/// </summary>
    
    //user_id: ��Ч�û�UserID

	public class InvalidUser
	{
        public InvalidUser()
        { }

		#region Model
		private long _user_id;
        private string _update_time;
		/// <summary>
		/// �û�UserID
		/// </summary>
		public long user_id
		{
			set{ _user_id=value;}
			get{return _user_id;}
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
                htValues.Add( "user_id", _user_id );
                htValues.Add( "update_time", _update_time );

                db.Insert( "invalid_users", htValues );
            }
            catch
            { return; }
		}

        /// <summary>
        /// �Ƿ���ڸü�¼
        /// </summary>
        public static bool ExistInDB(long lUid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect("select count(user_id) from invalid_users where user_id=" + lUid.ToString());
            return count > 0;
        }

        /// <summary>
        /// �����ݿ���ɾ��ָ����¼
        /// </summary>
        public static void RemoveFromDB(long lUid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQL("delete from invalid_users where user_id=" + lUid.ToString());
        }
		#endregion  ��Ա����
	}
}

