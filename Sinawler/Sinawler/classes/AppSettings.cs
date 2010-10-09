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
    private int _queue_length = 5000;               //�û����г���

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
        if (!File.Exists( Application.StartupPath + "\\config.ini" ))
            return null;
        //StreamReader sr = new StreamReader( Application.StartupPath + "\\config.ini" );
        FileStream fs = new FileStream( Application.StartupPath + "\\config.ini", FileMode.Open );
        byte[] arrByte = new byte[1024];
        fs.Read( arrByte, 0, 1024 );
        fs.Close();
        //byte[] arrByte = Encoding.ASCII.GetBytes( sr.ReadToEnd() );

        settings = (SettingItems)(Serialize.DecryptToObject( arrByte ));
        //sr.Close();
        return settings;
    }

    public static void Save ( SettingItems settings )
    {
        StreamWriter sw = new StreamWriter( Application.StartupPath + "\\config.ini" );
        sw.Write( Encoding.ASCII.GetChars(Serialize.EncryptToBytes( settings )) );
        sw.Close();
        //FileStream fs = new FileStream( Application.StartupPath + "\\config.ini", FileMode.OpenOrCreate );
        //fs.
    }

    //��ȡĬ������
    public static SettingItems LoadDefault ()
    {
        return new SettingItems();
    }
}
