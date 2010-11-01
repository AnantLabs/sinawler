using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sinawler;
using System.Windows.Forms;

namespace Sinawler
{
    //Ӧ�ó���������

    [Serializable]
    public class SettingItems
    {
        //app_key����
        private byte[] byteEncryptKey ={
            0xed,0xbd,0x07,0x60,0x1c,0x49,0x96,0x25,0x26,0x2f,0x6d,0xca,0x7b,0x7f,0x4a,0xf5,
            0x4a,0xd7,0xe0,0x74,0xa1,0x08,0x80,0x60,0x13,0x24,0xd8,0x90,0x40,0x10,0xec,0xc1,
            0x88,0xcd,0xe6,0x92,0xec,0x1d,0x69,0x47,0x23,0x29,0xab,0x2a,0x81,0xca,0x65,0x56,
            0x65,0x5d,0x66,0x16,0x40,0xcc,0xed,0x9d,0xbc,0xf7,0xde,0x7b,0xef,0xbd,0xf7,0xde,
            0x7b,0xef,0xbd,0xf7,0xba,0x3b,0x9d,0x4e,0x27,0xf7,0xdf,0xff,0x3f,0x5c,0x66,0x64,
            0x01,0x6c,0xf6,0xce,0x4a,0xda,0xc9,0x9e,0x21,0x80,0xaa,0xc8,0x1f,0x3f,0x7e,0x7c,
            0x1f,0x3f,0x22,0x7e,0x8d,0x5f,0xf3,0xd7,0xf8,0x35,0x7e,0x8d,0xff,0xfb,0xff,0xfe,
            0xbf,0xff,0x6f,0xfc,0xc4,0xff,0x7e,0x3d,0xfc,0xf2,0xad,0xbd,0x9d,0x83,0x87,0xbb,
            0x0f,0xef,0xdd,0xff,0xf4,0x7e,0x7e,0x7f,0xf7,0xde,0x83,0xfd,0xdd,0xfc,0xfc,0xde,
            0xee,0xde,0xe4,0xc1,0xe4,0x7e,0x7e,0x30,0x7b,0x78,0xbe,0xb3,0xbf,0xf7,0xe9,0xfe,
            0xe4,0xe1,0xce,0x2c,0xbf,0xf7,0x1b,0xfe,0x3f
            };
        private string _app_key = "";
        private string _secret_key = "";
        private string _database_type = "SQL Server";   //���ݿ�����Ĭ��ΪSQL Server
        private string _db_server = "localhost";        //���ݿ������
        private string _db_username = "sa";             //���ݿ��û���
        private string _db_pwd = "sa";                  //���ݿ�����
        private string _db_name = "Sinawler";           //���ݿ�����
        private int _max_length_in_mem = 5000;          //�ڴ�������󳤶�

        public SettingItems()
        {
            string strKey = Serialize.DecompressToObject(byteEncryptKey).ToString();
            if (strKey != "")
            {
                _app_key = strKey.Substring(0,10);
                _secret_key = strKey.Substring(10);
            }
        }

        public string AppKey
        { get { return _app_key; } }

        public string SecretKey
        { get { return _secret_key; } }

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
    }

    //ͨ�����л��뷴���л������桢��ȡӦ�ó���������
    class AppSettings
    {
        public static SettingItems Load()
        {
            SettingItems settings = new SettingItems();
            if (!File.Exists(Application.StartupPath + "\\config.ini"))
                return null;
            FileStream fs = new FileStream(Application.StartupPath + "\\config.ini", FileMode.Open, FileAccess.Read);
            byte[] arrByte = new byte[1024];
            fs.Read(arrByte, 0, 1024);
            fs.Close();
            int nLength = PubHelper.byteToInt(arrByte);
            //��������жϣ���Ϊ�˷�ֹ�ļ��м�¼�ĳ��ȱ���д�������
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
            byte[] arrLength = PubHelper.intToByte(arrEncryptByte.Length);  //�����ȣ�������������4��Ԫ�ص��ֽ�������
            fs.Write(arrLength, 0, arrLength.Length);
            fs.Write(arrEncryptByte, 0, arrEncryptByte.Length);
            fs.Close();
        }

        //��ȡĬ������
        public static SettingItems LoadDefault()
        {
            return new SettingItems();
        }
    }
}