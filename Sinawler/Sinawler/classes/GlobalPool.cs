using System;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using Sinawler.Model;

namespace Sinawler
{
    public class APIInfo
    {
        public SinaApiService API = new SinaApiService();
        public DateTime LimitUpdateTime = DateTime.Now;
        public int ResetTimeInSeconds = 3600;
        public int RemainingHits = 1000;
    }

    static public class GlobalPool
    {
        private static APIInfo ApiForUserRelation = new APIInfo();
        private static APIInfo ApiForUserInfo = new APIInfo();
        private static APIInfo ApiForUserTag = new APIInfo();
        private static APIInfo ApiForStatus = new APIInfo();
        private static APIInfo ApiForComment = new APIInfo();
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

        public static int MinSleepMsForUserRelation = 500;
        public static int MinSleepMsForUserInfo = 500;
        public static int MinSleepMsForUserTag = 500;
        public static int MinSleepMsForStatus = 500;
        public static int MinSleepMsForComment = 500;

        public static int SleepMsForThread = 1;

        public static APIInfo GetAPI(SysArgFor apiType)
        {
            switch (apiType)
            {
                case SysArgFor.USER_RELATION:
                    return ApiForUserRelation;
                    break;
                case SysArgFor.USER_INFO:
                    return ApiForUserInfo;
                    break;
                case SysArgFor.USER_TAG:
                    return ApiForUserTag;
                    break;
                case SysArgFor.STATUS:
                    return ApiForStatus;
                    break;
                case SysArgFor.COMMENT:
                    return ApiForComment;
                    break;
                default:
                    return null;
                    break;
            }
        }
    }
}
