using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Sinawler.Model;

namespace Sinawler
{
    class UserQueue : QueueBase
    {
        private EnumPreLoadQueue iPreLoadQueue = EnumPreLoadQueue.NO_PRELOAD;       //是否从数据库中预加载用户队列。默认为“否”
        //头节点被读取的次数。应可供用户机器人和微博机器人读取。每被读取一次值加1，当加为2，即被读取两次时（用户机器人和微博机器人各一次），头节点移入队尾，本计数器清零

        public EnumPreLoadQueue PreLoadQueue
        {
            get { return iPreLoadQueue; }
            set { iPreLoadQueue = value; }
        }

        public UserQueue (QueueBufferFor who)
            : base()
        {
            if(who==QueueBufferFor.COMMENT) //默认为USER
                lstWaitingIDInDB = new QueueBuffer( QueueBufferFor.USER );
            else
                lstWaitingIDInDB = new QueueBuffer(who);
        }
    }
}
