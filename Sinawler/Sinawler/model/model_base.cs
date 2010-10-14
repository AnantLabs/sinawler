using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler.Model
{
    public class ModelBase
    {
        protected Database db;

        public void ReLoadDBSettings()
        {
            db.LoadSettings();
        }
    }
}
