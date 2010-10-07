using System;
using System.Collections.Generic;
using System.Text;


public class DatabaseFactory
{
    public static Database CreateDatabase()
    {
        Database db;
        SettingItems settings=AppSettings.LoadDefault();
        string strDBType = settings.DataBaseType;

        if (strDBType == "SQL Server")
            db = new SqlDatabase();
        else
            db = new OracleDatabase();

        return db;
    }
}
