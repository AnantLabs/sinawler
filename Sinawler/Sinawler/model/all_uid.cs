using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler.Model
{
    //此类对应视图all_user_id，只提供静态属性与方法获取信息
    public class AllUserID
    {
        //视图中记录数
        static public int ItemsCount
        {
            get
            {
                Database db = DatabaseFactory.CreateDatabase();
                string strSQL = "select count(*) from all_user_id";
                return db.CountByExecuteSQLSelect( strSQL );
            }
        }
    }
}
