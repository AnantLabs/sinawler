using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sinawler;
using System.Windows.Forms;

namespace Sinawler
{
    [Serializable]
    public class SettingItems
    {
        private string _database_type = "SQL Server";   //数据库类型默认为SQL Server
        private string _db_server = "localhost";        //数据库服务器
        private string _db_username = "sa";             //数据库用户名
        private string _db_pwd = "sa";                  //数据库密码
        private string _db_name = "Sinawler";           //数据库名称
        private int _max_length_in_mem = 5000;          //内存中列最大长度

        private bool _user_info_robot = true;           //the state of user information robot
        private bool _tags_robot = true;                 //the state of tag robot
        private bool _statuses_robot = true;            //the state of statuses robot
        private bool _comments_robot = true;            //the state of comments robot

        private bool _confirm_relationship = true;      //whethe confirm relationship

        public string DBType
        {
            get { return _database_type; }
            set { _database_type = value; }
        }

        public string DBServer
        {
            get { return _db_server; }
            set { _db_server = value; }
        }

        public string DBUserName
        {
            get { return _db_username; }
            set { _db_username = value; }
        }

        public string DBPwd
        {
            get { return _db_pwd; }
            set { _db_pwd = value; }
        }

        public string DBName
        {
            get { return _db_name; }
            set { _db_name = value; }
        }

        public int MaxLengthInMem
        {
            get { return _max_length_in_mem; }
            set { _max_length_in_mem = value; }
        }

        public bool UserInfoRobot
        {
            get { return _user_info_robot; }
            set { _user_info_robot = value; }
        }

        public bool TagsRobot
        {
            get { return _tags_robot; }
            set { _tags_robot = value; }
        }

        public bool StatusesRobot
        {
            get { return _statuses_robot; }
            set { _statuses_robot = value; }
        }

        public bool CommentsRobot
        {
            get { return _comments_robot; }
            set { _comments_robot = value; }
        }

        public bool ConfirmRelationship
        {
            get { return _confirm_relationship; }
            set { _confirm_relationship = value; }
        }
    }

    //通过序列化与反序列化，保存、获取应用程序配置项
    class AppSettings
    {
        public static SettingItems Load()
        {
            SettingItems settings = new SettingItems();
            if (!File.Exists(Application.StartupPath + "\\config.ini"))
                return null;
            byte[] arrByte = new byte[1024];
            FileStream fs = new FileStream(Application.StartupPath + "\\config.ini", FileMode.Open, FileAccess.Read);
            fs.Read(arrByte, 0, 1024);
            fs.Close();
            int nLength = PubHelper.byteToInt(arrByte);
            //下面这个判断，是为了防止文件中记录的长度被改写导致溢出
            if (nLength >= 1020) nLength = 1020;

            byte[] arrEncryptByte = new byte[nLength];
            for (int i = 0; i < nLength; i++)
                arrEncryptByte[i] = arrByte[i + 4];

            settings = (SettingItems)(Serialize.DecryptToObject(arrEncryptByte));
            return settings;
        }

        public static void Save(SettingItems settings)
        {
            FileStream fs = new FileStream(Application.StartupPath + "\\config.ini", FileMode.OpenOrCreate);
            byte[] arrEncryptByte = Serialize.EncryptToBytes(settings);
            byte[] arrLength = PubHelper.intToByte(arrEncryptByte.Length);  //将长度（整数）保存在4个元素的字节数组中
            fs.Write(arrLength, 0, arrLength.Length);
            fs.Write(arrEncryptByte, 0, arrEncryptByte.Length);
            fs.Close();
        }

        //获取默认设置
        public static SettingItems LoadDefault()
        {
            return new SettingItems();
        }
    }
}