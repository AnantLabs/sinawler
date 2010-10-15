using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler
{
    public class DatabaseFactory
    {
        public static Database CreateDatabase()
        {
            Database db;
            string strDBType = AppSettings.LoadDefault().DBType;

            if (strDBType == "SQL Server")
                db = new SqlDatabase();
            else
                db = new OracleDatabase();

            return db;
        }
    }
}