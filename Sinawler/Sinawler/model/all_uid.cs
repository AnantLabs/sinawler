using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler.Model
{
    //�����Ӧ��ͼall_user_id��ֻ�ṩ��̬�����뷽����ȡ��Ϣ
    public class AllUserID
    {
        //��ͼ�м�¼��
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
