using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler
{
    //应用程序配置项
    class SettingItems
    {
        private string _database_type = "SQL Server";   //默认为SQL Server

        public string DataBaseType
        { 
            get { return _database_type; }
            set { _database_type = value; }
        }
    }

    //通过序列化与反序列化，保存、获取应用程序配置项
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

        //获取默认设置
        public static SettingItems LoadDefault()
        {
            SettingItems settings = new SettingItems();
            settings.DataBaseType = "SQL Server";
            return settings;
        }
    }
}
