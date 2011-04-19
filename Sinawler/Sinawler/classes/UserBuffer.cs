using System;
using System.Collections.Generic;
using System.Text;
using Sinawler.Model;

namespace Sinawler
{
    public class UserBuffer
    {
        private LinkedList<User> lstBufferedUsersInMem=new LinkedList<User>();
        private QueueBuffer lstBufferedUsersInDB = new QueueBuffer(QueueBufferFor.USER_BUFFER);              //数据库队列缓存
        private Object oLock = new Object();                    //锁。用于各机器人线程之间同步
        private int iMaxLengthInMem = 5000;               //内存中队列长度上限，默认5000

        //构造函数        
        public UserBuffer()
        {
            SettingItems settings = AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            iMaxLengthInMem = settings.MaxLengthInMem;
        }

        public int MaxLengthInMem
        { set { iMaxLengthInMem = value; } }

        public int CountInMem
        { get { return lstBufferedUsersInMem.Count; } }

        public int CountInDB
        { get { return lstBufferedUsersInDB.Count; } }

        public int Count
        {
            get { return lstBufferedUsersInMem.Count + lstBufferedUsersInDB.Count; }
        }

        public Object Lock
        { get { return oLock; } }

        //return an object of User from the buffer by user id
        public User GetUser(long lUserID)
        {
            lock (oLock)
            {
                if (lstBufferedUsersInMem == null || lUserID == null) return null;
                if (lstBufferedUsersInMem.Count == 0) return null;
                if (lstBufferedUsersInMem.Count == 1 && lstBufferedUsersInMem.First.Value.user_id == lUserID) return lstBufferedUsersInMem.First.Value;

                LinkedListNode<User> nodeHead = lstBufferedUsersInMem.First;
                LinkedListNode<User> nodeTail = lstBufferedUsersInMem.Last;
                if (nodeHead.Next == nodeTail)
                {
                    if (nodeHead.Value.user_id == lUserID) return nodeHead.Value;
                    if (nodeTail.Value.user_id == lUserID) return nodeTail.Value;
                }

                while (nodeHead.Next != nodeTail && nodeHead != nodeTail)
                {
                    if (nodeHead.Value.user_id==lUserID) return nodeHead.Value;
                    else nodeHead = nodeHead.Next;

                    if (nodeTail.Value.user_id==lUserID) return nodeTail.Value;
                    else nodeTail = nodeTail.Previous;
                }
                if (nodeHead.Value.user_id == lUserID) return nodeHead.Value;
                if (nodeTail.Value.user_id == lUserID) return nodeTail.Value;

                User user = new User();
                if (user.GetModelFromUserBuffer(lUserID)) return user;
                else return null;
            }
        }

        /// <summary>
        /// judge that whether specific user exists in the buffer by user id
        /// </summary>
        /// <param name="lUid"></param>
        public bool UserExists(long lUserID)
        {
            lock (oLock)
            {
                if (lstBufferedUsersInMem == null || lUserID == null) return false;
                if (lstBufferedUsersInMem.Count == 0) return false;
                if (lstBufferedUsersInMem.Count == 1) return lstBufferedUsersInMem.First.Value.user_id == lUserID;

                LinkedListNode<User> nodeHead = lstBufferedUsersInMem.First;
                LinkedListNode<User> nodeTail = lstBufferedUsersInMem.Last;
                if (nodeHead.Next == nodeTail) return (nodeHead.Value.user_id==lUserID || nodeTail.Value.user_id==lUserID);

                while (nodeHead.Next != nodeTail && nodeHead != nodeTail)
                {
                    if (nodeHead.Value.user_id==lUserID) return true;
                    else nodeHead = nodeHead.Next;

                    if (nodeTail.Value.user_id==lUserID) return true;
                    else nodeTail = nodeTail.Previous;
                }
                return (nodeHead.Value.user_id==lUserID || nodeTail.Value.user_id==lUserID);
                return lstBufferedUsersInDB.Contains(lUserID);
            }
        }

        /// <summary>
        /// 从外部调用判断队列中是否存在指定用户
        /// </summary>
        /// <param name="lUid"></param>
        public bool UserExists(User user)
        {
            lock (oLock)
            {
                if (lstBufferedUsersInMem == null || user == null) return false;
                if (lstBufferedUsersInMem.Count == 0) return false;
                if (lstBufferedUsersInMem.Count == 1) return lstBufferedUsersInMem.First.Value.Equals(user);

                LinkedListNode<User> nodeHead = lstBufferedUsersInMem.First;
                LinkedListNode<User> nodeTail = lstBufferedUsersInMem.Last;
                if (nodeHead.Next == nodeTail) return (nodeHead.Value.Equals(user) || nodeTail.Value.Equals(user));

                while (nodeHead.Next != nodeTail && nodeHead != nodeTail)
                {
                    if (nodeHead.Value.Equals(user)) return true;
                    else nodeHead = nodeHead.Next;

                    if (nodeTail.Value.Equals(user)) return true;
                    else nodeTail = nodeTail.Previous;
                }
                return (nodeHead.Value.Equals(user) || nodeTail.Value.Equals(user));
                return lstBufferedUsersInDB.Contains(user.user_id);
            }
        }

        /// <summary>
        /// 将指定ID加到自己队列中，返回的布尔值表示是否做了入队操作
        /// </summary>
        /// <param name="lid"></param>
        public bool Enqueue(User user)
        {
            if (user==null) return false;
            if (!UserExists(user))
            {
                //若内存中已达到上限，则使用数据库队列缓存
                //否则使用数据库队列缓存
                lock (oLock)
                {
                    if (lstBufferedUsersInMem.Count < iMaxLengthInMem)
                        lstBufferedUsersInMem.AddLast(user);
                    else
                        lstBufferedUsersInDB.Enqueue(user);
                }
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 将指定ID的用户从队列中移除
        /// </summary>
        /// <param name="lid"></param>
        public void Remove(User user)
        {
            if (user==null) return;
            lstBufferedUsersInMem.Remove(user);
            lstBufferedUsersInDB.Remove(user);
        }

        public void Initialize()
        {
            if (lstBufferedUsersInMem != null) lstBufferedUsersInMem.Clear();
            //清空数据库队列缓存
            if (lstBufferedUsersInDB != null) lstBufferedUsersInDB.Clear();
        }
    }
}
