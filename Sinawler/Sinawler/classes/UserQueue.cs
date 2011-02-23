using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Sinawler.Model;

namespace Sinawler
{
    class UserQueue : QueueBase
    {
        public UserQueue (QueueBufferFor who)
            : base()
        {
            if(who==QueueBufferFor.COMMENT) //Ä¬ÈÏÎªUSER_INFO
                lstWaitingIDInDB = new QueueBuffer( QueueBufferFor.USER_INFO );
            else
                lstWaitingIDInDB = new QueueBuffer(who);
        }
    }
}
