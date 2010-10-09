using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sinawler;
using System.Windows.Forms;

//Ӧ�ó���������
[Serializable]
public class SettingItems
{
    private string _app_key = "2089193565";         //��������
    private string _secret_key = "e513741ef312b7b5e8d9f04264b90de3";    //��������

    private string _database_type = "SQL Server";   //���ݿ�����Ĭ��ΪSQL Server
    private string _db_server = "localhost";        //���ݿ������
    private string _db_username = "sa";             //���ݿ��û���
    private string _db_pwd = "sa";                  //���ݿ�����
    private string _db_name = "Sinawler";           //���ݿ�����
    private int _queue_length = 5000;               //�û����г���

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

    public int QueueLength
    {
        get { return _queue_length; }
        set { _queue_length = value; }
    }
}

//ͨ�����л��뷴���л������桢��ȡӦ�ó���������
class AppSettings
{
    public static SettingItems Load ()
    {
        SettingItems settings = new SettingItems();
        if (!File.Exists(Application.StartupPath + "\\config.ini"))
            return null;
        FileStream fs = new FileStream( Application.StartupPath + "\\config.ini", FileMode.Open, FileAccess.Read );
        byte[] arrByte = new byte[1024];
        fs.Read( arrByte, 0, 1024 );
        fs.Close();
        int nLength = PubHelper.byteToInt(arrByte);

        byte[] arrEncryptByte = new byte[nLength];
        for (int i = 0; i < nLength; i++)
            arrEncryptByte[i] = arrByte[i + 4];

        settings = (SettingItems)(Serialize.DecryptToObject(arrEncryptByte));
        return settings;
    }

    public static void Save ( SettingItems settings )
    {
        FileStream fs = new FileStream( Application.StartupPath + "\\config.ini", FileMode.OpenOrCreate );
        byte[] arrEncryptByte = Serialize.EncryptToBytes(settings);
        byte[] arrLength = PubHelper.intToByte(arrEncryptByte.Length);  //�����ȣ�������������4��Ԫ�ص��ֽ�������
        fs.Write(arrLength, 0, arrLength.Length);
        fs.Write(arrEncryptByte, 0, arrEncryptByte.Length);
        fs.Close();
    }

    //��ȡĬ������
    public static SettingItems LoadDefault ()
    {
        return new SettingItems();
    }
}
