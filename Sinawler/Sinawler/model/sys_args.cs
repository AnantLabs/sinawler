using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
	/// <summary>
	/// ��SysArg�������еķ��������������ݿ⽻�������ж�ȡ��������д�����ݡ�
	/// </summary>
    
    //arg_name: ��������
    //arg_value: ����ֵ

    public class SysArg
	{
        public SysArg()
		{}

		#region Model
		private string _arg_name;
        private string _arg_value;
		/// <summary>
		/// ��������
		/// </summary>
		public string arg_name
		{
			set{ _arg_name=value;}
			get{return _arg_name;}
		}
        /// <summary>
		/// ����ֵ
		/// </summary>
		public string arg_value
		{
			set{ _arg_value=value;}
			get{return _arg_value;}
		}
		#endregion Model
        
		#region  ��Ա����

		/// <summary>
        /// ��¼��ǰ���е�UserID
		/// </summary>
		static public void SetCurrentUserID(long lUserID)
		{
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "delete from sys_args where arg_name='current_user_id'";
            db.CountByExecuteSQL(strSQL);
            strSQL = "insert into sys_args(arg_name,arg_value) values('current_user_id','" + lUserID.ToString() + "')";
            db.CountByExecuteSQL(strSQL);
		}

        /// <summary>
        /// ��ȡ��ǰ���е�UserID����������ֹ����Ϊ�ϴ���ֹ�����û���
        /// </summary>
        static public long GetCurrentUserID ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "select arg_value from sys_args where arg_name='current_user_id'";
            DataRow dr = db.GetDataRow( strSQL );
            if (dr == null) return 0;
            else return Convert.ToInt64( dr["arg_value"] );
        }

		#endregion  ��Ա����
	}
}

