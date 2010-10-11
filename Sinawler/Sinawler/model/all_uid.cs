using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler.Model
{
    //此类对应视图all_uid，只提供静态属性与方法获取信息
    public class AllUID
    {
        static private Database db = DatabaseFactory.CreateDatabase();

        //视图中记录数
        static public int ItemsCount
        {
            get
            {
                string strSQL = "select count(*) from all_uid";
                return db.CountByExecuteSQLSelect( strSQL );
            }
        }
    }
}
