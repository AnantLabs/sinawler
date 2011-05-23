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
        public static Object Lock = new Object();       //用于进程间同步的锁，注意一定要在队列初始化之前，因为队列要用它

        public static UserQueue UserQueueForUserInfoRobot = new UserQueue(QueueBufferFor.USER_INFO);  //用户信息机器人使用的用户队列
        public static UserQueue UserQueueForUserRelationRobot = new UserQueue(QueueBufferFor.USER_RELATION);  //用户关系机器人使用的用户队列
        public static UserQueue UserQueueForUserTagRobot = new UserQueue(QueueBufferFor.USER_TAG);  //用户标签机器人使用的用户队列
        public static UserQueue UserQueueForStatusRobot = new UserQueue(QueueBufferFor.STATUS);  //微博机器人使用的用户队列
        public static StatusQueue StatusQueue = new StatusQueue();  //微博队列

        public static UserBuffer UserBuffer = new UserBuffer();  //buffor of the users got by status robot and comment robot

        public static bool UserInfoRobotEnabled = true;
        public static bool TagRobotEnabled = true;
        public static bool StatusRobotEnabled = true;
        public static bool CommentRobotEnabled = true;

        public static DateTime LimitUpdateTime = DateTime.Now;
        public static int ResetTimeInSeconds = 3600;
        public static int RemainingHits=1000;
    }
}
