using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Sinawler.Model;

namespace Sinawler
{
    public class StatusQueue : QueueBase
    {
        public StatusQueue ()
            : base()
        {
            lstWaitingIDInDB = new QueueBuffer( QueueBufferFor.COMMENT );
        }
    }
}
