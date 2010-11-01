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
        /// 记录当前爬行的UserID
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
        /// 获取当前爬行的UserID（若爬行中止，则为上次中止处的用户）
        /// </summary>
        static public long GetCurrentUserID ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "select arg_value from sys_args where arg_name='current_user_id'";
            DataRow dr = db.GetDataRow( strSQL );
            if (dr == null) return 0;
            else return Convert.ToInt64( dr["arg_value"] );
        }

		#endregion  成员方法
	}
}

