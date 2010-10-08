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
        static private Database db = DatabaseFactory.CreateDatabase();

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
        /// ��¼��ǰ���е�UID
		/// </summary>
		static public void SetCurrentUID(long lUID)
		{
            string strSQL="update sys_args set arg_value='"+lUID.ToString()+"' where arg_name='current_uid'";
            db.CountByExecuteSQL(strSQL);
		}

        /// <summary>
        /// ��ȡ��ǰ���е�UID����������ֹ����Ϊ�ϴ���ֹ�����û���
        /// </summary>
        static public long GetCurrentUID ()
        {
            string strSQL = "select arg_value from sys_args where arg_name='current_uid'";
            DataRow dr = db.GetDataRow( strSQL );
            if (dr == null) return 0;
            else return Convert.ToInt64( dr["arg_value"] );
        }

		#endregion  ��Ա����
	}
}

