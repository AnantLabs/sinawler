using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Sinawler
{
    [Serializable]
    public class CurrentIDs
    {
        private long _current_user_id_for_userRelation = 0;
        private long _current_user_id_for_userInfo = 0;
        private long _current_user_id_for_userTag = 0;
        private long _current_user_id_for_status = 0;
        private long _current_status_id = 0;

        public long UIDForUserRelation
        {
            get { return _current_user_id_for_userRelation; }
            set { _current_user_id_for_userRelation = value; }
        }
        public long UIDForUserInfo
        {
            get { return _current_user_id_for_userInfo; }
            set { _current_user_id_for_userInfo = value; }
        }
        public long UIDForUserTag
        {
            get { return _current_user_id_for_userTag; }
            set { _current_user_id_for_userTag = value; }
        }
        public long UIDForStatus
        {
            get { return _current_user_id_for_status; }
            set { _current_user_id_for_status = value; }
        }
        public long StatusID
        {
            get { return _current_status_id; }
            set { _current_status_id = value; }
        }
    }

    class CurrentIDHelper
    {
        public static CurrentIDs Load()
        {
            CurrentIDs currentIDs = new CurrentIDs();
            if (!File.Exists(Application.StartupPath + "\\current_ids.cid"))
                return null;
            byte[] arrByte = new byte[1024];
            FileStream fs = new FileStream(Application.StartupPath + "\\current_ids.cid", FileMode.Open, FileAccess.Read);
            fs.Read(arrByte, 0, 1024);
            fs.Close();
            int nLength = PubHelper.byteToInt(arrByte);
            //下面这个判断，是为了防止文件中记录的长度被改写导致溢出
            if (nLength >= 1020) nLength = 1020;

            byte[] arrEncryptByte = new byte[nLength];
            for (int i = 0; i < nLength; i++)
                arrEncryptByte[i] = arrByte[i + 4];

            currentIDs = (CurrentIDs)(Serialize.DecryptToObject(arrEncryptByte));
            return currentIDs;
        }

        public static void Save(CurrentIDs currentIDs)
        {
            byte[] arrEncryptByte = Serialize.EncryptToBytes(currentIDs);
            byte[] arrLength = PubHelper.intToByte(arrEncryptByte.Length);  //将长度（整数）保存在4个元素的字节数组中
            lock (GlobalPool.Lock)
            {
                FileStream fs = new FileStream(Application.StartupPath + "\\config.ini", FileMode.OpenOrCreate);
                fs.Write(arrLength, 0, arrLength.Length);
                fs.Write(arrEncryptByte, 0, arrEncryptByte.Length);
                fs.Close();
            }
        }

        public static CurrentIDs LoadDefault()
        {
            return new CurrentIDs();
        }

        public static long GetCurrentID(SysArgFor IDFor)
        {
            CurrentIDs currentIDs = CurrentIDHelper.Load();
            if (currentIDs == null) currentIDs = CurrentIDHelper.LoadDefault();
            switch (IDFor)
            {
                case SysArgFor.USER_INFO:
                    return currentIDs.UIDForUserInfo;
                    break;
                case SysArgFor.USER_TAG:
                    return currentIDs.UIDForUserTag;
                    break;
                case SysArgFor.STATUS:
                    return currentIDs.UIDForStatus;
                    break;
                case SysArgFor.COMMENT:
                    return currentIDs.StatusID;
                    break;
                default:
                    return currentIDs.UIDForUserRelation;
                    break;
            }
        }
    }
}
