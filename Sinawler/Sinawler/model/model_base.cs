using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler.Model
{
    public class ModelBase
    {
        static protected Database db = DatabaseFactory.CreateDatabase();

        public void ReLoadDBSettings()
        {
            db.LoadSettings();
        }
    }
}
