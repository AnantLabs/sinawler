using System;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using Sinawler.Model;

namespace Sinawler
{
    static public class GlobalPool
    {
        public static SinaApiService API = new SinaApiService();

        public static UserQueue UserQueueForUserInfoRobot = new UserQueue(QueueBufferFor.USER_INFO);  //用户信息机器人使用的用户队列
        public static UserQueue UserQueueForUserRelationRobot = new UserQueue(QueueBufferFor.USER_RELATION);  //用户关系机器人使用的用户队列
        public static UserQueue UserQueueForUserTagRobot = new UserQueue(QueueBufferFor.USER_TAG);  //用户标签机器人使用的用户队列
        public static UserQueue UserQueueForStatusRobot = new UserQueue(QueueBufferFor.STATUS);  //微博机器人使用的用户队列
        public static StatusQueue StatusQueue = new StatusQueue();  //微博队列

        public static UserBuffer UserBuffer = new UserBuffer();  //buffor of the users got by status robot and comment robot
    }
}
