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
        /// ��¼�����˵�ǰ���е�ID
        /// </summary>
        static public void SetCurrentID(long lID, SysArgFor eFor)
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strDeleteSQL = "";
            string strInsertSQL = "";
            switch (eFor)
            { 
                case SysArgFor.USER_RELATION:
                    strDeleteSQL = "delete from sys_args where arg_name='current_user_id_userRelation'";
                    strInsertSQL = "insert into sys_args(arg_name,arg_value) values('current_user_id_userRelation','" + lID.ToString() + "')";
                    break;
                case SysArgFor.USER_INFO:
                    strDeleteSQL = "delete from sys_args where arg_name='current_user_id_userInfo'";
                    strInsertSQL = "insert into sys_args(arg_name,arg_value) values('current_user_id_userInfo','" + lID.ToString() + "')";
                    break;
                case SysArgFor.USER_TAG:
                    strDeleteSQL = "delete from sys_args where arg_name='current_user_id_userTag'";
                    strInsertSQL = "insert into sys_args(arg_name,arg_value) values('current_user_id_userTag','" + lID.ToString() + "')";
                    break;
                case SysArgFor.STATUS:
                    strDeleteSQL = "delete from sys_args where arg_name='current_user_id_status'";
                    strInsertSQL = "insert into sys_args(arg_name,arg_value) values('current_user_id_status','" + lID.ToString() + "')";
                    break;
                case SysArgFor.COMMENT:
                    strDeleteSQL = "delete from sys_args where arg_name='current_status_id'";
                    strInsertSQL = "insert into sys_args(arg_name,arg_value) values('current_status_id','" + lID.ToString() + "')";
                    break;
            }            
            db.CountByExecuteSQL(strDeleteSQL);            
            db.CountByExecuteSQL(strInsertSQL);
        }

        /// <summary>
        /// Ϊ�����˻�ȡ��ǰ���е�ID����������ֹ����Ϊ�ϴ���ֹ����ID��
        /// </summary>
        static public long GetCurrentID(SysArgFor eFor)
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "";
            switch (eFor)
            {
                case SysArgFor.USER_RELATION:
                    strSQL = "select arg_value from sys_args where arg_name='current_user_id_userRelation'";
                    break;
                case SysArgFor.USER_INFO:
                    strSQL = "select arg_value from sys_args where arg_name='current_user_id_userInfo'";
                    break;
                case SysArgFor.USER_TAG:
                    strSQL = "select arg_value from sys_args where arg_name='current_user_id_userTag'";
                    break;
                case SysArgFor.STATUS:
                    strSQL = "select arg_value from sys_args where arg_name='current_user_id_status'";
                    break;
                case SysArgFor.COMMENT:
                    strSQL = "select arg_value from sys_args where arg_name='current_status_id'";
                    break;
            }
            DataRow dr = db.GetDataRow(strSQL);
            if (dr == null) return 0;
            else return Convert.ToInt64(dr["arg_value"]);
        }
		#endregion  ��Ա����
	}
}

