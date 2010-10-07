using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler
{
    //Ӧ�ó���������
    class SettingItems
    {
        private string _database_type = "SQL Server";   //Ĭ��ΪSQL Server

        public string DataBaseType
        { 
            get { return _database_type; }
            set { _database_type = value; }
        }
    }

    //ͨ�����л��뷴���л������桢��ȡӦ�ó���������
    class AppSettings
    {
        public static SettingItems Load()
        {
            SettingItems settings = new SettingItems();
            return settings;
        }

        public static void Save(SettingItems settings)
        {
            //SettingItems settings = new SettingItems();
        }

        //��ȡĬ������
        public static SettingItems LoadDefault()
        {
            SettingItems settings = new SettingItems();
            settings.DataBaseType = "SQL Server";
            return settings;
        }
    }
}
