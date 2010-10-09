using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sinawler;
using System.Windows.Forms;

//应用程序配置项
[Serializable]
public class SettingItems
{
    private string _app_key = "2089193565";         //不可设置
    private string _secret_key = "e513741ef312b7b5e8d9f04264b90de3";    //不可设置

    private string _database_type = "SQL Server";   //数据库类型默认为SQL Server
    private string _db_server = "localhost";        //数据库服务器
    private string _db_username = "sa";             //数据库用户名
    private string _db_pwd = "sa";                  //数据库密码
    private int _queue_length = 5000;               //用户队列长度

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

//通过序列化与反序列化，保存、获取应用程序配置项
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

    //获取默认设置
    public static SettingItems LoadDefault ()
    {
        return new SettingItems();
    }
}
