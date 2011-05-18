using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Sinawler.Model;

namespace Sinawler
{
    public class UserQueue : QueueBase
    {
        public UserQueue (QueueBufferFor who)
            : base()
        {
            //this is a queue for of user ids, so the value COMMENT, which will make the queue store status ids, is not allowed, default value USER_INFO will be used.
            if(who==QueueBufferFor.COMMENT)
                lstWaitingIDInDB = new QueueBuffer( QueueBufferFor.USER_INFO );
            else
                lstWaitingIDInDB = new QueueBuffer(who);
        }
    }
}
