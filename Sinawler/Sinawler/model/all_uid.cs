using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler.Model
{
    //�����Ӧ��ͼall_uid��ֻ�ṩ��̬�����뷽����ȡ��Ϣ
    public class AllUID
    {
        static private Database db = DatabaseFactory.CreateDatabase();

        //��ͼ�м�¼��
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
