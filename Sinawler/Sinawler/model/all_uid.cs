using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler.Model
{
    //�����Ӧ��ͼall_uid��ֻ�ṩ��̬�����뷽����ȡ��Ϣ
    public class AllUID
    {
        //��ͼ�м�¼��
        static public int ItemsCount
        {
            get
            {
                Database db = DatabaseFactory.CreateDatabase();
                string strSQL = "select count(*) from all_uid";
                return db.CountByExecuteSQLSelect( strSQL );
            }
        }
    }
}
