using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
    /// <summary>
	/// 类SysArg。此类中的方法，都是与数据库交互，从中读取或向其中写入数据。
	/// </summary>
    
    //arg_name: 参数名称
    //arg_value: 参数值

    public class SysArg
	{
        public SysArg()
		{}

		#region Model
		private string _arg_name;
        private string _arg_value;
		/// <summary>
		/// 参数名称
		/// </summary>
		public string arg_name
		{
			set{ _arg_name=value;}
			get{return _arg_name;}
		}
        /// <summary>
		/// 参数值
		/// </summary>
		public string arg_value
		{
			set{ _arg_value=value;}
			get{return _arg_value;}
		}
		#endregion Model
        
		#region  成员方法

        /// <summary>
        /// 记录机器人当前爬行的ID
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
        /// 为机器人获取当前爬行的ID（若爬行中止，则为上次中止处的ID）
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
		#endregion  成员方法
	}
}

